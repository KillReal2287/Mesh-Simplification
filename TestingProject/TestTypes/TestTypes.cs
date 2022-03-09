using Xunit;
using MeshSimplification.Types;

namespace TestingProject.TestTypes; 

public class TestTypes {
    [Fact]
    public void Testing_Vertex() {
        Vertex v = new Vertex(0, 1, 2);
        
        Assert.Equal(0, v.X);
        Assert.Equal(1, v.Y);
        Assert.Equal(2, v.Z);

        Vertex v1 = new Vertex(5.5, 1.1, -5.5);
        Vertex v2 = new Vertex(6, 2.2, 0);
        
        Assert.True(v2 > v1);
        Assert.True(v2 >= v1);
        
        Assert.False(v2 < v1);
        Assert.False(v2 <= v1);

        v2 = new Vertex(5.5, 1.1, 100);
        
        Assert.True(v2 >= v1);
        Assert.False(v2 > v1);
    }

    [Fact]
    public void Testing_Face() {
        
    }
}