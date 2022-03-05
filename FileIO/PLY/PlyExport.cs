using System.Text;
using System.Globalization;
using MeshSimplification.Types;

namespace MeshSimplification.FileIO.PLY;

internal class PlyExport {
        private bool _hasNormal;

        internal void Export(string fileName, Model model, bool isBinary) {
            try {
                if (isBinary) {
                    /* WriteBinary in development */
                }
                else {
                    using (StreamWriter writer =
                           new StreamWriter(new FileStream(fileName, FileMode.Create), Encoding.ASCII)) {
                        CultureInfo info = CultureInfo.CurrentCulture;
                        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    
                        WriteHeaderAscii(writer, model, "ascii");
                        WriteAscii(writer, model);
                    
                        Thread.CurrentThread.CurrentCulture = info;
                    }
                }
            }
            catch (Exception e) {
                Console.Error.WriteLine("Writing file \"{0}\" failed: {1}", fileName, e.Message);
            }
        }

        private void WriteHeaderAscii(StreamWriter writer, Model model, string type) { 
            int countVertex = 0;
            int countFace = 0;
            int countEdge = 0;
            _hasNormal = false;

            foreach (Mesh mesh in model.Meshes) {
                countVertex += mesh.Vertices.Count;
                
                if (mesh.Normals.Count > 0)
                    _hasNormal = true;
                
                countFace += mesh.Faces.Count;
                countEdge += mesh.Edges.Count;
            }

            writer.WriteLine("ply\n" +
                             "format {0} 1.0\n" +
                             "element vertex {1}\n" +
                             "property double x\n" +
                             "property double y\n" +
                             "property double z", type, countVertex);

            if (_hasNormal) {
                writer.WriteLine("property double nx\n" +
                                 "property double ny\n" +
                                 "property double nz");
            }
            
            writer.WriteLine("element face {0}\nproperty list int int vertex_index", countFace);
            
            if (countEdge > 0) {
                writer.WriteLine("element edge {0}" +
                                 "property int vertex1\n" +
                                 "property int vertex2", countEdge);
            }
            
            writer.WriteLine("end_header");
        }
        
        private void WriteAscii(StreamWriter writer, Model model) {
            foreach (Mesh mesh in model.Meshes) {
                for (int i = 0; i < mesh.Vertices.Count; i++) {
                    if (_hasNormal) {
                        if (mesh.Normals.Count > 0) {
                            writer.WriteLine("{0} {1} {2} {3} {4} {5}",
                                mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z,
                                mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z);
                        }
                        else {
                            writer.WriteLine("{0} {1} {2} nan nan nan",
                                mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z);
                        }
                    }
                    else {
                        writer.WriteLine("{0} {1} {2}",
                            mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z);
                    }
                }
            }

            foreach (Mesh mesh in model.Meshes) {
                foreach (Face f in mesh.Faces) {
                    writer.Write(f.Count + " ");
                    foreach (int i in f.Vertices) {
                        writer.Write(i + " ");
                    }
                    writer.WriteLine();
                }
            }

            foreach (Mesh mesh in model.Meshes) {
                foreach (Edge e in mesh.Edges) {
                    writer.WriteLine("{0} {1}", e.Vertex1, e.Vertex2);
                }
            }
        }
    }