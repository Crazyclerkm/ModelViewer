using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class Renderer {

    private readonly List<Model> Models = [];
    private readonly List<Camera> Cameras = [];

    private Shader DefaultShader = ResourceManager.LoadShader("Default", "Resources/Shaders/basicShader.vs", "Resources/Shaders/basicShader.fs");

    public Camera ActiveCamera {get; private set;}
    
    public void AddModel(Model model) {Models.Add(model);}
    public void AddCamera(Camera camera) {Cameras.Add(camera);}

    public Renderer(int width, int height) {
        GL.ClearColor(0.1f, 0.2f, 0.2f, 1.0f);
        GL.Enable(EnableCap.DepthTest);

        Camera camera = new Camera(Vector3.UnitZ*2, width / (float)height);
        AddCamera(camera);
        ActiveCamera = camera;
        
        Models.Add(ResourceManager.LoadModel("Resources/Models/cube/cube.obj"));
    }
    
    public static void ClearScreen() {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    public void RenderScene() {
        ClearScreen();
        Matrix4 projection_from_view = ActiveCamera.GetProjectionMatrix();
        Matrix4 view_from_world = ActiveCamera.GetViewMatrix();
        Matrix4 world_from_object;
        
        DefaultShader.Use();

        DefaultShader.SetMatrix4("projection_from_view", projection_from_view);
        DefaultShader.SetMatrix4("view_from_world", view_from_world);  

        foreach (Model model in Models) {
            world_from_object = model.GetModelMatrix(); 
            DefaultShader.SetMatrix4("world_from_object", world_from_object);  

            foreach (Mesh mesh in model.Meshes) {
                mesh.Draw();
            }
        }
    }

    public static void Unload() {
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }

    public void ResizeViewport(int x, int y, int width, int height) {
        GL.Viewport(x, y, width, height);

        foreach (var camera in Cameras) {
            camera.AspectRatio = width / (float) height;
        }
    }

    public void ResizeViewport(int width, int height) {
        ResizeViewport(0, 0, width, height);
    }
}