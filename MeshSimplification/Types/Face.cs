namespace MeshSimplification.Types;

public class Face {
    public int Count { get; }
    public List<int> Vertices { get; }

    public Face(int count, List<int> vertices) {
        Count = count;
        Vertices = vertices;
    }
}