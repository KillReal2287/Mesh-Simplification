using System;
using System.Diagnostics;
using MeshSimplification.Algorithms;
using MeshSimplification.Readers.Exporter;
using MeshSimplification.Readers.Importer;
using MeshSimplification.Types;

namespace MeshSimplification{
    internal class Program{
        public static void Main(string[] args){
            Stopwatch stopWatch = new Stopwatch();
            TimeSpan ts;
            string elapsedTime;
            //string path = @"/home/andrey/Downloads/help/check/aa.ply";
            string path = @"/home/andrey/Downloads/help/ascii/helix.ply";
            
            rmUselessVertices rm = new rmUselessVertices();

            ImporterPly importer = new ImporterPly();
            ExporterPly exporterPly = new ExporterPly();
            
            Model figure = importer.Import(path);

            string path1 = @"/home/andrey/RiderProjects/project/MeshSimplification/MeshSimplification/Algorithms/";
            string path2 = @"/home/andrey/Downloads/help/test/";
            string path3 = @"/home/andrey/Downloads/help/test/";
            
            
            stopWatch.Start();

            //Algorithm algorithm = new VertexCollapsingInRadius(figure, 0.1);
            Algorithm algorithm = new EdgeContractionAngle(figure, 90);
            Model simple = algorithm.GetSimplifiedModel();
            
            stopWatch.Stop();

            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Console.WriteLine("Algorithm runtime: " + elapsedTime + "\n");
            
            
            stopWatch.Restart();
            rm.RemoveVertices(simple);
            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Console.WriteLine("removing useless vertices runtime: " + elapsedTime + "\n");    
            
            stopWatch.Restart();

            //exporterPly.Export(path, figure, false, false);
            exporterPly.Export(path, simple, false, false);
            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Console.WriteLine("Writing runtime: " + elapsedTime);
            Console.WriteLine();
        }
    }
}