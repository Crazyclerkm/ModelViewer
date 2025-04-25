using OpenTK.Mathematics;

public class Model {

    public List<Mesh> Meshes;

    public Vector3 AABBmin {get; private set;}
    public Vector3 AABBmax {get; private set;}

    public Vector3 Position {get; set;}
    public Vector3 Rotation {get; set;}
    public Vector3 Scale {get; set;}

    public string Name {get; private set;}

    public Model(List<Mesh> meshes, string name) {
        Meshes = meshes;
        Name = name;
        ComputeAABB();
    }

    public Matrix4 GetModelMatrix() {
        return Matrix4.Identity;
    }

    private void ComputeAABB() {
        Vector3 min = new Vector3(float.MaxValue);
        Vector3 max = new Vector3(float.MinValue);

        foreach (var mesh in Meshes) {
            min = Vector3.ComponentMin(min, mesh.AABBmin);
            max = Vector3.ComponentMax(max, mesh.AABBmax);
        }

        AABBmin = min;
        AABBmax = max;
    }
}