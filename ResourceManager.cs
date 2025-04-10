
using OpenTK.Graphics.OpenGL4;
public static class ResourceManager {

    private static readonly Dictionary<string, Shader> shaders = new();
    private static readonly Dictionary<string, Texture> textures = new();

    public static Shader LoadShader(string name, string vertexPath, string fragmentPath) {
        if(shaders.TryGetValue(name, out Shader cachedShader)) {
            return cachedShader;
        }
        
        string VertexShaderSource = File.ReadAllText(vertexPath);
        string FragmentShaderSource = File.ReadAllText(fragmentPath);

        Shader shader = new();

        shader.Compile(VertexShaderSource, FragmentShaderSource);

        shaders[name] = shader;
        return shader;
    }

    public static Texture LoadTexture(string filePath) {
        if(textures.TryGetValue(filePath, out Texture cachedTexture)) {
            return cachedTexture;
        }
        
        Texture texture = Texture.LoadFromFile(filePath);
        textures[filePath] = texture;
        return texture;
    }

    public static void Unload() {
        foreach (Shader shader in shaders.Values) {
            GL.DeleteProgram(shader.GetHandle());
        }

        shaders.Clear();
        
        foreach (Texture texture in textures.Values) {
            GL.DeleteTexture(texture.Handle);
        }

        textures.Clear();
    }
}