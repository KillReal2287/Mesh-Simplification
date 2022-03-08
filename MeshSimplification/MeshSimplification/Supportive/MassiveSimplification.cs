using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MeshSimplification.Readers.Exporter;
using MeshSimplification.Readers.Importer;
using MeshSimplification.Types;

namespace MeshSimplification.Algorithms{

    public class MassiveSimplification{
        private static readonly String location = typeof(MassiveSimplification).Assembly.Location;
        private readonly Assembly assembly = Assembly.LoadFrom(location);
        private readonly String algorithmsLocation;
        private readonly String pattern = "*.ply";

        private String pathIn;
        private String pathOut;

        public MassiveSimplification(String algorithmsLocation, String pathIn, String pathOut){
            this.algorithmsLocation = algorithmsLocation;
            this.pathIn = pathIn;
            this.pathOut = pathOut;
            exportModels(getRefactorModels(getModels(), getAlgorithmsTypes(getAlgorithmNames())), getAlgorithmNames(),
                getModelsNames());
        }

        private List<String> getAlgorithmNames(){
            List<String> algorithmsNames = new List<string>();

            foreach (String s in Directory.GetFiles(algorithmsLocation))
                if (!Path.GetFileNameWithoutExtension(s).Equals("Algorithm"))
                    algorithmsNames.Add(Path.GetFileNameWithoutExtension(s));

            return algorithmsNames;
        }

        private List<Type> getAlgorithmsTypes(List<String> algorithmsNames){
            List<Type> algorithmsTypes = new List<Type>();

            foreach (String s in algorithmsNames)
                algorithmsTypes.Add(assembly.GetType("MeshSimplification.Algorithms." + s));

            return algorithmsTypes;
        }

        private List<Model> getModels(){
            ImporterPly importerPly = new ImporterPly();
            List<Model> models = new List<Model>();

            foreach (String s in getFiles()) models.Add(importerPly.Import(s));

            return models;
        }

        private List<String> getModelsNames(){
            List<String> modelsNames = new List<string>();

            foreach (String path in getFiles()) modelsNames.Add(Path.GetFileName(path));

            return modelsNames;
        }

        private string[] getFiles(){
            return Directory.GetFiles(pathIn, pattern);
        }

        private List<Model> getRefactorModels(List<Model> models, List<Type> algorithmsTypes){
            List<Model> refactorModels = new List<Model>();

            foreach (Type type in algorithmsTypes) 
                foreach (Model model in models) {
                    var obj = Activator.CreateInstance(type, model);
                    refactorModels.Add(type.GetMethod("GetSimplifiedModel").Invoke(obj, new object[] { }) as Model);
                }

            return refactorModels;
        }

        private void exportModels(List<Model> refactorModels, List<String> algorithmsNames, List<String> modelNames){
            ExporterPly exporterPly = new ExporterPly();
            int cnt = 0;
            foreach (Model model in refactorModels) {
                String path = pathOut + algorithmsNames[cnt / modelNames.Count];
                if (!Directory.Exists(path)) 
                    Directory.CreateDirectory(path);
                exporterPly.Export(path + "/" + modelNames[cnt++ % modelNames.Count], model, false, true);
            }
        }
    }
}