using System.Reflection;
using ConsoleApp1;
using Exporter;
using Importer;
using Types;

namespace Algorithms;

public class MassiveSimplification
{
    private static readonly String location = typeof(MassiveSimplification).Assembly.Location;
    private readonly Assembly assembly = Assembly.LoadFrom(location);
    private readonly String algorithmsLocation = "/home/egor/RiderProjects/Solution1/ConsoleApp1/ConsoleApp1/";
    private readonly String pattern = "*.ply";

    private String pathIn;
    private String pathOut;
    private double simplificationCoefficient; 

    public MassiveSimplification(String pathIn, String pathOut, double simplificationCoefficient)
    {
        this.pathIn = pathIn;
        this.pathOut = pathOut;
        this.simplificationCoefficient = simplificationCoefficient;
        
        exportModels(getRefactorModels(getModels(), getAlgorithmsTypes(getAlgorithmNames())),getAlgorithmNames(), getModelsNames());
    }

    private List<String> getAlgorithmNames()
    {
        List<String> algorithmsNames = new List<string>();
        String[] words;
        
        foreach (String s in Directory.GetFiles(algorithmsLocation))
        {
            words = s.Split('\\', '/', '.');
            if(!words[words.Length-2].Equals("Algorithm")) algorithmsNames.Add(words[words.Length-2]);
        }

        return algorithmsNames;
    }

    private List<Type> getAlgorithmsTypes(List<String> algorithmsNames)
    {
        List<Type> algorithmsTypes = new List<Type>();
        foreach (String s in algorithmsNames) algorithmsTypes.Add(assembly.GetType("ConsoleApp1." + s)); // namespase. + algorithm name

        return algorithmsTypes;
    }
    
    private List<Model> getModels()
    {
        ImporterPly importerPly = new ImporterPly();
        List<Model> models = new List<Model>();
        
        foreach (String s in getFiles()) models.Add(importerPly.Import(s));
        
        return models;
    }

    private List<String> getModelsNames()
    {
        List<String> modelsNames = new List<string>();
        foreach (String path in getFiles())
        {
            String[] tmp = path.Split('\\', '/', '.');
            modelsNames.Add(tmp[tmp.Length - 2]);
        }

        return modelsNames;
    }

    private string[] getFiles() { return Directory.GetFiles(pathIn, pattern); }

    private List<Model> getRefactorModels(List<Model> models, List<Type> algorithmsTypes)
    {
        List<Model> refactorModels = new List<Model>();
        
        foreach (Model model in models)
        foreach (Type type in algorithmsTypes)
        {
            var obj = Activator.CreateInstance(type, model, 0.05);
            var method = type.GetMethod("GetSimplifiedModel");
            refactorModels.Add(method.Invoke(obj, new object[] {}) as Model);
        }

        return refactorModels;
    }

    private void exportModels(List<Model> refactorModels, List<String> algorithmsNames, List<String> modelNames)
    {
        ExporterPly exporterPly = new ExporterPly();
        int cnt = 0;
        foreach (Model model in refactorModels) exporterPly.Export(pathOut + algorithmsNames[cnt % algorithmsNames.Count] + ": " + modelNames[cnt++ % modelNames.Count]+ ".ply",model, false);
    }
}