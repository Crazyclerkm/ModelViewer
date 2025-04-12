using OpenTK.Mathematics;

public class Model {

    public List<Mesh> Meshes;

    public Model(List<Mesh> meshes) {
        Meshes = meshes;
    }

    public Matrix4 GetModelMatrix() {
        return Matrix4.Identity;
    }
}