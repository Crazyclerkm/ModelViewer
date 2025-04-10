public class Model {

    private List<Mesh> Meshes;
    private List<Texture> Textures;
    private readonly string FilePath;

    public Model(string filePath, List<Mesh> meshes, List<Texture> textures) {
        FilePath = filePath;
        Meshes = meshes;
        Textures = textures;
    }

    public void Draw() {
        foreach (Mesh mesh in Meshes) {   
            mesh.Draw();
        }
    }
}