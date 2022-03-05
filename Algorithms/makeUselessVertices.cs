using System;
using System.Collections.Generic;
using MeshSimplification.Types;

namespace MeshSimplification{
    public class makeUselessVertices{
        public void makeVertices(Model model){
            foreach (Mesh mesh in model.Meshes)
                changeMesh(mesh);
        }
//raw
        private static void changeMesh(Mesh mesh){
            List<Face> newFaces = mesh.Faces;
            foreach (Face face in newFaces) {
                
            }
        }
//raw
        private static void removeInMesh(Mesh mesh){
            int before = mesh.Vertices.Count;
            int after;

            for (int i = 0; i < mesh.Vertices.Count; i++) {
                if (!checkVertice(mesh, i)) {
                    changeIndex(mesh, i);
                }
            }
            
            after = mesh.Vertices.Count;
//raw
            Console.WriteLine("vertices in mesh before: {0}", before);
            Console.WriteLine("vertices in mesh after: {0}", after);
            Console.WriteLine("useless vertices in mesh: {0}", before - after);
            Console.WriteLine("percentage of useless vertices in mesh: {0}", (before - after) / before);
        }
//raw
        private static bool checkVertice(Mesh mesh, int index){
            foreach (Face face in mesh.Faces) {
                if (face.Vertices.Contains(index))
                    return true;
            }
            return false;
        }
//raw
        private static void changeIndex(Mesh mesh, int index){
            mesh.Vertices.RemoveAt(index);
            foreach (Face face in mesh.Faces) {
                if (face.Vertices[0] > index)
                    face.Vertices[0] -= 1;
                if (face.Vertices[1] > index)
                    face.Vertices[1] -= 1;
                if (face.Vertices[2] > index)
                    face.Vertices[2] -= 1;
            }
        }
    }
}