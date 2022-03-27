using System;
using MeshSimplification.Types;

namespace MeshSimplification{
    public class rmUselessVertices{
        /*
         * first we take index of the vertex
         * in the next step the algorithm checks all faces
         * and tries to find the index
         */
        public void RemoveVertices(Model model){
            foreach (Mesh mesh in model.Meshes) {
                int deleted = removeInMesh(mesh);
                int total = deleted;
                while (deleted > 0) {
                    deleted = removeInMesh(mesh);
                    total += deleted;
                }
                Console.WriteLine("deleted vertices in mesh: {0}", total);
            }
        }

        /*
         * if you remove the comments you will see which
         * indices are removed and the coordinates of those vertices
         */
        private static int removeInMesh(Mesh mesh){
            int deleted = 0;

            int index = 0;
            while (index < mesh.Vertices.Count) {
                if (!checkVertice(mesh, index - deleted)) {
                    //Console.Write("{0}, {1}, {2} -- ", mesh.Vertices[i - deleted].X, mesh.Vertices[i - deleted].Y, mesh.Vertices[i - deleted].Z);
                    changeIndex(mesh, index - deleted);
                    //Console.WriteLine(i);
                    deleted += 1;
                    continue;
                }
                index += 1;
            }

            return deleted;
        }
        

        private static bool checkVertice(Mesh mesh, int index){
            int indexDel = index < 0 ? 0 : index;
            foreach (Face face in mesh.Faces) {
                if (face.Vertices.Contains(indexDel))
                    return true;
            }
            return false;
        }

        private static void changeIndex(Mesh mesh, int index){
            int indexDel = index < 0 ? 0 : index;
            
            mesh.Vertices.RemoveAt(indexDel);
            
            foreach (Face face in mesh.Faces) {
                face.Vertices[0] = face.Vertices[0] > indexDel ? face.Vertices[0] - 1 : face.Vertices[0];
                face.Vertices[1] = face.Vertices[1] > indexDel ? face.Vertices[1] - 1 : face.Vertices[1];
                face.Vertices[2] = face.Vertices[2] > indexDel ? face.Vertices[2] - 1 : face.Vertices[2];
            }
        }
    }
}