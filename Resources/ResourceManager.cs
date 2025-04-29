
using OpenTK.Graphics.OpenGL4;

using ModelViewer.Importers;
using ModelViewer.Graphics;

namespace ModelViewer.Resources {
    public static class ResourceManager {

        private static readonly Dictionary<string, Model> Models = new();
        private static readonly Dictionary<string, Shader> Shaders = new();
        private static readonly Dictionary<string, Texture> Textures = new();

        public static Model LoadModel(string filePath) {
            if(Models.TryGetValue(filePath, out Model? cachedModel)) {
                return cachedModel;
            }
            
            string extension = Path.GetExtension(filePath).ToLower();
            IModelImporter importer = GetImporter(extension);

            Model model = importer.LoadModel(filePath);
            Models[filePath] = model;
            return model;
        }

        public static Shader LoadShader(string name, string vertexPath, string fragmentPath) {
            if(Shaders.TryGetValue(name, out Shader? cachedShader)) {
                return cachedShader;
            }
            
            string VertexShaderSource = File.ReadAllText(vertexPath);
            string FragmentShaderSource = File.ReadAllText(fragmentPath);

            Shader shader = new();

            shader.Compile(VertexShaderSource, FragmentShaderSource);

            Shaders[name] = shader;
            return shader;
        }

        public static Texture LoadTexture(string filePath) {
            if(Textures.TryGetValue(filePath, out Texture? cachedTexture)) {
                return cachedTexture;
            }
            
            Texture texture = Texture.LoadFromFile(filePath);
            Textures[filePath] = texture;
            return texture;
        }

        private static IModelImporter GetImporter(string extension) {
            return extension switch {
                ".obj" => new ObjImporter(),
                _ => throw new NotSupportedException($"Unsupported file format: {extension}")
            };
        }

        public static void Unload() {
            foreach (Shader shader in Shaders.Values) {
                GL.DeleteProgram(shader.GetHandle());
            }

            Shaders.Clear();
            
            foreach (Texture texture in Textures.Values) {
                GL.DeleteTexture(texture.Handle);
            }

            Textures.Clear();

            Models.Clear();

        }
    }
}