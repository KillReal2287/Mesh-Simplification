using System.Collections.Generic;
using Xunit;
using MeshSimplification.Types;

namespace TestingProject; 

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
        Face f = new Face(new List<int>() {0, 1, 2});
        
        Assert.Equal(3, f.Count);
        Assert.Collection(f.Vertices,
            num => Assert.Equal(0, num),
            num => Assert.Equal(1, num),
            num => Assert.Equal(2, num)
            );
        
        f.Vertices.Add(3);
        
        /* Face.Count automatically changes */
        Assert.Equal(4, f.Count);
        Assert.Collection(f.Vertices,
            num => Assert.Equal(0, num),
            num => Assert.Equal(1, num),
            num => Assert.Equal(2, num),
            num => Assert.Equal(3, num)
        );

        f.Vertices.Remove(2);
        Assert.Equal(3, f.Count);
        Assert.Collection(f.Vertices,
            num => Assert.Equal(0, num),
            num => Assert.Equal(1, num),
            num => Assert.Equal(3, num)
        );
    }

    [Fact]
    public void Testing_Edge() {
        Edge e = new Edge(10, 11);
        
        Assert.Equal(10, e.Vertex1);
        Assert.Equal(11, e.Vertex2);
    }
    
    [Fact]
    public void Testing_Property() {
        Property p = new Property("Name", false, "type");
        
        Assert.Equal("Name", p.Name);
        Assert.False(p.IsScalar);
        Assert.Equal("", p.CountType);
        Assert.Equal("type", p.ElemType);

        p = new Property("Name2", true, "Count", "Elem");
        
        Assert.Equal("Name2", p.Name);
        Assert.True(p.IsScalar);
        Assert.Equal("Count", p.CountType);
        Assert.Equal("Elem", p.ElemType);
    }

    [Fact]
    public void Testing_Element() {
        Element e = new Element("vertex", 10);

        Property p1 = new Property("x", true, "double");
        Property p2 = new Property("y", true, "double");
        Property p3 = new Property("z", true, "double");
        Property p4 = new Property("vertex_index", false, "int", "double");
        
        e.AddProperty(p1);
        e.AddProperty(p2);
        e.AddProperty(p3);
        e.AddProperty(p4);
        
        Assert.Equal(10, e.Count);
        Assert.Equal("vertex", e.Name);
        Assert.Equal(4, e.Properties.Count);
        Assert.Collection(e.Properties,
            prop => Assert.Equal(p1, prop),
            prop => Assert.Equal(p2, prop),
            prop => Assert.Equal(p3, prop),
            prop => Assert.Equal(p4, prop)
        );

        e.Properties.Remove(p4);
        
        Assert.Equal(10, e.Count);
        Assert.Equal("vertex", e.Name);
        Assert.Equal(3, e.Properties.Count);
        Assert.Collection(e.Properties,
            prop => Assert.Equal(p1, prop),
            prop => Assert.Equal(p2, prop),
            prop => Assert.Equal(p3, prop)
        );
        
        Assert.Equal(0, e.PropertyIndex("x"));
        Assert.Equal(1, e.PropertyIndex("y"));
        Assert.Equal(2, e.PropertyIndex("z"));
        Assert.Equal(-1, e.PropertyIndex("vertex_index"));
        
        Property p5 = new Property("x", true, "double");
        e.AddProperty(p5);
        Assert.Equal(0, e.PropertyIndex("x"));

        e.Properties.Remove(p1);
        Assert.Equal(2, e.PropertyIndex("x"));
    }

    [Fact]
    public void Testing_Mesh() {
        Mesh m = new Mesh(new List<Vertex>(), new List<Face>());
        
        Assert.NotNull(m.Vertices);
        Assert.NotNull(m.Normals);
        Assert.NotNull(m.Faces);
        Assert.NotNull(m.Edges);
        
        m.AddVertex(new Vertex(0, 1, 2));
        m.AddNormal(new Vertex(0, -1, -2));
        m.AddFace(new Face(new List<int>() {5, 6, 7}));
        m.AddEdge(new Edge(8, 9));
        
        Assert.NotEmpty(m.Vertices);
        Assert.NotEmpty(m.Normals);
        Assert.NotEmpty(m.Faces);
        Assert.NotEmpty(m.Edges);
        
        Assert.Collection(m.Vertices, v => {
            Assert.Equal(0, v.X);
            Assert.Equal(1, v.Y);
            Assert.Equal(2, v.Z);
        });
        
        Assert.Collection(m.Normals, n => {
            Assert.Equal(0, n.X);
            Assert.Equal(-1, n.Y);
            Assert.Equal(-2, n.Z);
        });
        
        Assert.Collection(m.Faces[0].Vertices,
            index => Assert.Equal(5, index),
            index => Assert.Equal(6, index),
            index => Assert.Equal(7, index)
            );
        
        Assert.Collection(m.Edges, edge => {
            Assert.Equal(8, edge.Vertex1);
            Assert.Equal(9, edge.Vertex2);
        });
    }

    [Fact]
    public void Testing_Model() {
        Model m = new Model();
        Assert.NotNull(m.Meshes);

        m = new Model(new Mesh());
        Assert.NotEmpty(m.Meshes);
        
        m.AddMesh(new Mesh());
        Assert.Equal(2, m.Meshes.Count);
    }
}