using System;
using System.Collections.Generic;
using System.Numerics;
using MeshSimplification.Types;

/*
 * these algorithms chooses which edge should be
 * deleted based on its length or on an angle between
 * two faces which contains this edge
 */

/*
 * note: now if you want use algorithm based on an
 * angle between you should call
 * like this:
 * "new EdgeContraction(model, ratio, (boolean)true)"
 *
 * algorithm based on length:
 * "new EdgeContraction(model, ratio, (boolean)false)"
 * or
 * "new EdgeContraction(model, ratio)"
 */ 
namespace MeshSimplification.Algorithms {
    public class EdgeContraction : Algorithm{
        private readonly Model model;
        private Model simplifiedModel;
        private readonly double ratio;
        private readonly Boolean angle;

        private double longest = double.MinValue;

        public EdgeContraction(Model model, double ratio){
            this.model = model;
            this.ratio = ratio;
        }
        
        public EdgeContraction(Model model, double ratio, Boolean angle){
            this.model = model;
            this.ratio = ratio;
            this.angle = angle;
        }
        
        public override Model GetSimplifiedModel(){
            simplifiedModel = angle ? SimplifyAngle() : SimplifyLength();
            return simplifiedModel;
        }

        //general methods   ↓↓↓
        private bool EdgeInFace(Edge edge, Face face) {
            return face.Vertices.Exists(x => x == edge.Vertex1) && 
                   face.Vertices.Exists(x => x == edge.Vertex2);
        }
        
        private bool IfEdge(Edge edge, List<Edge> edges) {
            return edges.Exists(x =>
                x.Vertex1 == edge.Vertex1 && x.Vertex2 == edge.Vertex2 ||
                x.Vertex1 == edge.Vertex2 && x.Vertex2 == edge.Vertex1);
        }
        
        private List<Edge> GetEdges(Mesh mesh){
            List<Edge> answer = new List<Edge>();

            foreach (Face f in mesh.Faces) {
                List<double> length = new List<double>();
                
                if (!IfEdge(new Edge(f.Vertices[0], f.Vertices[1]), answer)) {
                    answer.Add(new Edge(f.Vertices[0], f.Vertices[1])); 
                    length.Add(EdgeLength(mesh, answer[answer.Count - 1]));
                }
                
                if (!IfEdge(new Edge(f.Vertices[0], f.Vertices[2]), answer)) {
                    answer.Add(new Edge(f.Vertices[0], f.Vertices[2]));
                    length.Add(EdgeLength(mesh, answer[answer.Count - 1]));

                }
                
                if (!IfEdge(new Edge(f.Vertices[1], f.Vertices[2]), answer)) {
                    answer.Add(new Edge(f.Vertices[1], f.Vertices[2]));
                    length.Add(EdgeLength(mesh, answer[answer.Count - 1]));
                }

                if (length.Count > 0) {
                    length.Sort();                    
                    longest = length[length.Count - 1] > longest ? length[length.Count - 1] : longest;                    
                }
            }
            return answer;
        }

        //code based on angles    ↓↓↓
        private Model SimplifyAngle(){
            Model modelNew = new Model();

            foreach (Mesh mesh in model.Meshes) {
                //CheckThisMesh(mesh);
                Mesh simple = new Mesh(new List<Vertex>(mesh.Vertices), new List<Vertex>(mesh.Normals),
                    new List<Face>(mesh.Faces), new List<Edge>(mesh.Edges));
                modelNew.AddMesh(SimplifyMeshAngle(simple));
            }
            return modelNew;
        }

        private Mesh SimplifyMeshAngle(Mesh mesh){
            List<Edge> edges = GetEdges(mesh);
            Mesh meshNew = BasedAngle(mesh, edges);
            return meshNew;
        }

        private double CountAngle(Vector3 normal1, Vector3 normal2){
            double scalar;
            double length;

            scalar = Math.Abs(normal1.X * normal2.X + normal1.Y * normal2.Y + normal1.Z * normal2.Z);
            length = Math.Sqrt(normal1.X * normal1.X + normal1.Y * normal1.Y + normal1.Z * normal1.Z);
            length *= Math.Sqrt(normal2.X * normal2.X + normal2.Y * normal2.Y + normal2.Z * normal2.Z);
            return scalar / length;
        }

        private Vector3 GetNormal(Mesh mesh, Face face){
            Vector3 vector3 = new Vector3();

            vector3.X = (float) ((mesh.Vertices[face.Vertices[1]].Y - mesh.Vertices[face.Vertices[0]].Y) * 
                                 (mesh.Vertices[face.Vertices[2]].Z - mesh.Vertices[face.Vertices[0]].Z));
            vector3.X -= (float) ((mesh.Vertices[face.Vertices[1]].Z - mesh.Vertices[face.Vertices[0]].Z) * 
                                  (mesh.Vertices[face.Vertices[2]].Y - mesh.Vertices[face.Vertices[0]].Y));
            
            vector3.Y = (float) ((mesh.Vertices[face.Vertices[2]].X - mesh.Vertices[face.Vertices[0]].X) * 
                                 (mesh.Vertices[face.Vertices[1]].Z - mesh.Vertices[face.Vertices[0]].Z));
            vector3.Y -= (float) ((mesh.Vertices[face.Vertices[2]].Z - mesh.Vertices[face.Vertices[0]].Z) *
                                  (mesh.Vertices[face.Vertices[1]].X - mesh.Vertices[face.Vertices[0]].X));
            
            vector3.Z = (float) ((mesh.Vertices[face.Vertices[1]].X - mesh.Vertices[face.Vertices[0]].X) * 
                                 (mesh.Vertices[face.Vertices[2]].Y - mesh.Vertices[face.Vertices[0]].Y));
            vector3.Z -= (float) ((mesh.Vertices[face.Vertices[1]].Y - mesh.Vertices[face.Vertices[0]].Y) *
                                  (mesh.Vertices[face.Vertices[2]].X - mesh.Vertices[face.Vertices[0]].X));
            
            return vector3;
        }
        
        private Mesh BasedAngle(Mesh mesh, List<Edge> edges){
            List <Vertex> vertices = mesh.Vertices;
            List <Face> faces = mesh.Faces;
            int before = mesh.Faces.Count;
            int v1Index, v2Index;

            int iterator = 0;
            
            while (iterator != edges.Count) {
                Edge edge = edges[iterator];
                
                List<Face> facesFounded = faces.FindAll(face => EdgeInFace(edge, face));
                if (facesFounded.Count != 2) {
                    iterator += 1;
                    continue;
                }

                Vector3 normal1 = GetNormal(mesh, facesFounded[0]);
                Vector3 normal2 = GetNormal(mesh, facesFounded[1]);

                double angleCosValueInput = Math.Cos(ratio * 0.01745); 

                double angleCosValue = CountAngle(normal1, normal2);

                angleCosValue = angleCosValue > 1 ? 1 : angleCosValue;
                angleCosValue = angleCosValue < -1 ? -1 : angleCosValue;


                if (angleCosValue.CompareTo(angleCosValueInput) >= 0) {
                    v1Index = edge.Vertex1;
                    v2Index = edge.Vertex2;

                    Vertex v1 = vertices[v1Index];
                    Vertex v2 = vertices[v2Index];
                    
                    Vertex newVert = new Vertex((v1.X + v2.X) / 2,
                        (v1.Y + v2.Y) / 2, (v1.Z + v2.Z) / 2);
                    
                    faces.RemoveAll(x => EdgeInFace(edge, x));
                    
                    vertices.Add(newVert);
                    for (int iter = 0; iter < faces.Count; iter++) {
                        if (faces[iter].Vertices[0] == v1Index || faces[iter].Vertices[0] == v2Index)
                            faces[iter].Vertices[0] = vertices.Count - 1;
                        
                        if (faces[iter].Vertices[1] == v1Index || faces[iter].Vertices[1] == v2Index)
                            faces[iter].Vertices[1] = vertices.Count - 1;
                        
                        if (faces[iter].Vertices[2] == v1Index || faces[iter].Vertices[2] == v2Index)
                            faces[iter].Vertices[2] = vertices.Count - 1;
                    }
                }
                else
                    iterator += 1;
            }


            Console.WriteLine("Stat:");
            Console.WriteLine("faces before: {0}", before);
            Console.WriteLine("faces after: {0}", faces.Count);
            Console.WriteLine("percentage of faces remaining: {0:F5}", (double)faces.Count/before);

            return new Mesh(vertices, new List<Vertex>(), faces, new List<Edge>());
        }
        
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////          
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////  
        
        //code based on length    ↓↓↓
        private Model SimplifyLength(){
            Model modelNew = new Model();

            foreach (Mesh mesh in model.Meshes) {
                Mesh simple = new Mesh(new List<Vertex>(mesh.Vertices), new List<Vertex>(mesh.Normals),
                    new List<Face>(mesh.Faces), new List<Edge>(mesh.Edges));
                modelNew.AddMesh(SimplifyMeshLength(simple));
            }

            return modelNew;
        }
        private Mesh SimplifyMeshLength(Mesh mesh) {
            List<Edge> edges = GetEdges(mesh);
            
            Mesh deleteEdges = DeleteEdge(mesh, edges);
            
            return deleteEdges;
        }

        private double EdgeLength(Mesh mesh, Edge edge) {
            double x1 = mesh.Vertices[edge.Vertex1].X;
            double x2 = mesh.Vertices[edge.Vertex2].X;
                    
            double y1 = mesh.Vertices[edge.Vertex1].Y;
            double y2 = mesh.Vertices[edge.Vertex2].Y;
                    
            double z1 = mesh.Vertices[edge.Vertex1].Z;
            double z2 = mesh.Vertices[edge.Vertex2].Z;
                
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) + (z2 - z1) * (z2 - z1));
        }

        private Mesh DeleteEdge(Mesh mesh, List<Edge> edges) {
            List <Vertex> vertices = mesh.Vertices;
            List <Face> faces = mesh.Faces;
            int before = mesh.Faces.Count;
            int v1Index, v2Index;
            
            int newVertices = 0;
            foreach (Edge edge in edges) {
                if (EdgeLength(mesh, edge) < ratio * longest) {
                    v1Index = edge.Vertex1;
                    v2Index = edge.Vertex2;
                    
                    Vertex v1 = vertices[v1Index];
                    Vertex v2 = vertices[v2Index];

                    Vertex newVert = new Vertex((v1.X + v2.X) / 2,
                        (v1.Y + v2.Y) / 2, (v1.Z + v2.Z) / 2);
                    
                    faces.RemoveAll(x => EdgeInFace(edge, x));
                    
                    vertices.Add(newVert);
                    newVertices += 1;
                    for (int iter = 0; iter < faces.Count; iter++) {
                        if (faces[iter].Vertices[0] == v1Index || faces[iter].Vertices[0] == v2Index)
                            faces[iter].Vertices[0] = vertices.Count - 1;
                        
                        if (faces[iter].Vertices[1] == v1Index || faces[iter].Vertices[1] == v2Index)
                            faces[iter].Vertices[1] = vertices.Count - 1;
                        
                        if (faces[iter].Vertices[2] == v1Index || faces[iter].Vertices[2] == v2Index)
                            faces[iter].Vertices[2] = vertices.Count - 1;
                    }
                }
            }
            //Console.WriteLine("New vertices: {0}", newVertices);

            Console.WriteLine("Stat:");
            Console.WriteLine("faces before: {0}", before);
            Console.WriteLine("faces after: {0}", faces.Count);
            Console.WriteLine("percentage of faces remaining: {0:F5}", (double)faces.Count/before);

            return new Mesh(vertices, new List<Vertex>(), faces, new List<Edge>());
        }
    }
}