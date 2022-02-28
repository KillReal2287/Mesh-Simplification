using NUnit.Framework;
using MeshSimplification.Algorithms;
using MeshSimplification.Types;
using MeshSimplification.Readers.Importer;

namespace Tests{
    [TestFixture]
    public class Tests{
        string path = @"/home/andrey/Downloads/help/ascii/bunny.ply";
        
        [Test]
        public void TestFaceContraction(){
            FaceContraction faceContraction = new FaceContraction();
            BoundBoxAABB boundBoxAABB = new BoundBoxAABB();
            
            ImporterPly importer = new ImporterPly();
            Model figure = importer.Import(path);
            
            Model simple = faceContraction.Simplify(figure, 0.2);
            Model box = boundBoxAABB.Simplify(figure);
            Model boxSimple = boundBoxAABB.Simplify(simple);

            //check count of meshes
            Assert.AreEqual(figure.Meshes.Count, simple.Meshes.Count);

            //check volume
            for (int i = 0; i < box.Meshes.Count; i++) {
                for (int j = 0; j < box.Meshes[i].Vertices.Count; j++) { 
                    Assert.AreEqual(box.Meshes[i].Vertices[j].X, boxSimple.Meshes[i].Vertices[j].X);   
                    Assert.AreEqual(box.Meshes[i].Vertices[j].Y, boxSimple.Meshes[i].Vertices[j].Y);   
                    Assert.AreEqual(box.Meshes[i].Vertices[j].Z, boxSimple.Meshes[i].Vertices[j].Z);   
                }                
            } 
            
        }
        
        [Test]
        public void TestEdgeContraction(){

            
            
            BoundBoxAABB boundBoxAABB = new BoundBoxAABB();
            
            ImporterPly importer = new ImporterPly();
            Model figure = importer.Import(path);

            
            Algorithm algo = new EdgeContraction(figure);
            Algorithm algorithm = new EdgeContraction(figure);
            Model simple = algorithm.GetSimplifiedModel();

            Model box = boundBoxAABB.Simplify(figure);
            Model boxSimple = boundBoxAABB.Simplify(simple);
            
            //check count of meshes
            Assert.AreEqual(figure.Meshes.Count, simple.Meshes.Count);

            //check volume
            for (int i = 0; i < box.Meshes.Count; i++) {
                for (int j = 0; j < box.Meshes[i].Vertices.Count; j++) { 
                    Assert.AreEqual(box.Meshes[i].Vertices[j].X, boxSimple.Meshes[i].Vertices[j].X, 0.0001);   
                    Assert.AreEqual(box.Meshes[i].Vertices[j].Y, boxSimple.Meshes[i].Vertices[j].Y, 0.0001);   
                    Assert.AreEqual(box.Meshes[i].Vertices[j].Z, boxSimple.Meshes[i].Vertices[j].Z, 0.0001);   
                }                
            } 
            
        }
    }
}