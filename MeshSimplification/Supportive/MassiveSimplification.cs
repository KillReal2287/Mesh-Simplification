using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MeshSimplification.FileIO.PLY;
using MeshSimplification.Types;

namespace MeshSimplification.Supportive;

public class MassiveSimplification
{
    private static readonly String location = typeof(MassiveSimplification).Assembly.Location;
    private readonly Assembly assembly = Assembly.LoadFrom(location);
    private readonly String algorithmsLocation;
    private readonly String pattern = "*.ply";

    private PlyInputOutput plyio;

    private String pathIn;
    private String pathOut;

    public MassiveSimplification(String algorithmsLocation, String pathIn, String pathOut)
    {
        this.algorithmsLocation = algorithmsLocation;
        this.pathIn = pathIn;
        this.pathOut = pathOut;
        plyio = new PlyInputOutput();
        Console.WriteLine(Directory.GetCurrentDirectory());
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
        foreach (String s in algorithmsNames) algorithmsTypes.Add(assembly.GetType("MeshSimplification.Algorithms" + s));

        return algorithmsTypes;
    }
    
    private List<Model> getModels()
    {
        List<Model> models = new List<Model>();
        
        foreach (String s in getFiles()) models.Add(plyio.Import(s));
        
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
            var obj = Activator.CreateInstance(type, model);
            var method = type.GetMethod("GetSimplifiedModel");
            refactorModels.Add(method.Invoke(obj, new object[] {}) as Model);
        }

        return refactorModels;
    }

    private void exportModels(List<Model> refactorModels, List<String> algorithmsNames, List<String> modelNames)
    {
        int cnt = 0;
        foreach (Model model in refactorModels) plyio.Export(pathOut + algorithmsNames[cnt % algorithmsNames.Count] + ": " + modelNames[cnt++ % modelNames.Count]+ ".ply",model, false);
    }
}