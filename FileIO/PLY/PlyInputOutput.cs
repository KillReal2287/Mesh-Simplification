using MeshSimplification.Types;

namespace MeshSimplification.FileIO.PLY;

public class PlyInputOutput {
    private readonly PlyImport _import;
    private readonly PlyExport _export;

    public  PlyInputOutput() {
        _import = new PlyImport();
        _export = new PlyExport();
    }
    
    public Model Import(string fileName) {
        return _import.Import(fileName);
    }

    public Model Import(string fileName, string directory) {
        string path = Path.Combine(directory, fileName);
        return _import.Import(path);
    }

    public void Export(string fileName, Model model, bool isBinary) {
        _export.Export(fileName, model, isBinary);
    }
    
    public void Export(string fileName, string directory, Model model, bool isBinary) {
        string path = Path.Combine(directory, fileName);
        _export.Export(path, model, isBinary);
    }
}