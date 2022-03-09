// Original idea by https://github.com/kovacsv/Online3DViewer

namespace MeshSimplification.Types;

public class Element {
    public string Name { get; }
    public int Count { get; }
    public List<Property> Properties { get; }

    public Element(string name, int count) {
        Name = name;
        Count = count;
        Properties = new List<Property>();
    }

    public int PropertyIndex(string propertyName) {
        for (int i = 0; i < Properties.Count; i++) {
            if (Properties[i].Name.Equals(propertyName))
                return i;
        }

        return -1;
    }

    public void AddProperty(Property property) {
        Properties.Add(property);
    }
}