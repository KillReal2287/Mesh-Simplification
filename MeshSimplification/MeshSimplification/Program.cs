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
            string path = @"/home/andrey/Downloads/help/ascii/hind.ply";

            EdgeContraction edgeContraction = new EdgeContraction();
            FaceContraction faceContraction = new FaceContraction();
            MyAlgorithm myAlgorithm = new MyAlgorithm();

            rmUselessVertices rm = new rmUselessVertices();
            
            
            ImporterPly importer = new ImporterPly();
            ExporterPly exporterPly = new ExporterPly();
            
            Model figure = importer.Import(path);
            
            
            
            stopWatch.Start();

            //Model simple = figure;
            Model simple = edgeContraction.Simplify(figure, 1.1);
            //Model simple = edgeContraction.Simplify(figure);
            //Model simple = faceContraction.Simplify(figure, 0.02);
            //Model simple = myAlgorithm.Simplify(figure, 0.56);
            
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