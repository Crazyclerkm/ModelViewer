using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class Renderer {

    private readonly List<Model> Models = [];
    private readonly List<Raycaster> Rays = [];
    private readonly List<Camera> Cameras = [];

    private Shader DefaultShader = ResourceManager.LoadShader("Default", "Resources/Shaders/basicShader.vs", "Resources/Shaders/basicShader.fs");
    private Shader UIShader = ResourceManager.LoadShader("UI", "Resources/Shaders/UIShader.vs", "Resources/Shaders/UIShader.fs");

    private Shader LineShader = ResourceManager.LoadShader("Line", "Resources/Shaders/lineShader.vs", "Resources/Shaders/lineShader.fs");

    public Camera ActiveCamera {get; private set;}
    
    public void AddModel(Model model) {Models.Add(model);}
    public void AddCamera(Camera camera) {Cameras.Add(camera);}

    private int WindowWidth;
    private int WindowHeight;

    public Renderer(int width, int height) {
        WindowWidth = width;
        WindowHeight = height;
        GL.ClearColor(0.1f, 0.2f, 0.2f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
       
        Camera camera = new Camera(Vector3.UnitZ*2, width / (float)height);
        AddCamera(camera);
        ActiveCamera = camera;

        AddModel(ResourceManager.LoadModel("Resources/Models/cube/cube.obj"));
    }
    
    public static void ClearScreen() {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
    }

    public void RenderUI(ImGuiController imGuiController) {
        imGuiController.Draw(UIShader);
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

        LineShader.Use();
        LineShader.SetMatrix4("projection_from_view", projection_from_view);
        LineShader.SetMatrix4("view_from_world", view_from_world); 
       
        foreach (Raycaster ray in Rays) {
            world_from_object = ray.GetModelMatrix();
            LineShader.SetMatrix4("world_from_object", world_from_object);
            LineShader.SetVec3("lineColour", ray.Colour);
            ray.Draw();
        }
        
        GL.UseProgram(0);
    }

    public static void Unload() {
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }

    public void ResizeViewport(int x, int y, int width, int height) {
        GL.Viewport(x, y, width, height);
        WindowWidth = width;
        WindowHeight = height;

        foreach (var camera in Cameras) {
            camera.AspectRatio = width / (float) height;
        }
    }

    public void ResizeViewport(int width, int height) {
        ResizeViewport(0, 0, width, height);
    }

    public Model? SelectModel(float x, float y) {
        Raycaster ray = WorldRayFromScreenPoint(x, y);

        Model? closestModel = FindClosestModelFromRay(ray);

        return closestModel;
    }

    private Raycaster WorldRayFromScreenPoint(float x, float y) {
        Matrix4 view_from_projection = ActiveCamera.GetProjectionMatrix().Transposed().Inverted();
        Matrix4 world_from_view = ActiveCamera.GetViewMatrix().Transposed().Inverted();

        float ndcX = (2.0f * x / WindowWidth) - 1.0f;
        float ndcY = 1.0f - 2.0f * y / WindowHeight;
        float ndcZ = 1.0f;

        Vector4 clipCoords = new Vector4(ndcX, ndcY, ndcZ, 1.0f);

        Vector4 viewCoords = view_from_projection * clipCoords;
        viewCoords.W = 0.0f;

        Vector4 worldCoords = world_from_view * viewCoords;

        Vector3 rayDirection = Vector3.Normalize(worldCoords.Xyz);

        return new Raycaster(ActiveCamera.Position, rayDirection);
    }

    public Model? FindClosestModelFromRay(Raycaster ray) {
        Model? closestModel = null;
        float? closestDistance = null;

        foreach (Model model in Models) {
            Vector3 min = model.AABBmin;
            Vector3 max = model.AABBmax;

            bool intersects = ray.IntersectsBoundingBox(model.AABBmin, model.AABBmax, out float distance);

            if (!intersects) continue;

            if (closestDistance == null || distance < closestDistance) {
                closestDistance = distance;
                closestModel = model;
            }
        }
        return closestModel;
    }

}