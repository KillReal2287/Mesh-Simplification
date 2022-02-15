using Types;
using Importer;
using Exporter;
using Algorithms;

namespace Algorithm
{
    class Program
    {
        static void Main(string[] args) 
        {
            ImporterPly importerPly = new ImporterPly();
            String path = "/home/egor/Downloads/bunny.ply";
            Model model = importerPly.Import(path);
            
            VertexCollapsingInRadius my = new VertexCollapsingInRadius(model, 0.5);
            
            ExporterPly exporterPly = new ExporterPly();
            path = "/home/egor/Downloads/ref0.ply";
            exporterPly.Export(path, my.GetSimplifiedModel(), false);
        }
    }
}
