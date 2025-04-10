using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class Renderer {

    private readonly List<Camera> Cameras = new();

    public Camera ActiveCamera {get; private set;}

    public Renderer(int width, int height) {
        GL.ClearColor(0.1f, 0.2f, 0.2f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        Camera camera = new Camera(Vector3.UnitZ, width / (float)height);
        AddCamera(camera);
    }

    public void AddCamera(Camera camera) {
        Cameras.Add(camera);

        if (ActiveCamera == null) {
            ActiveCamera = camera;
        }
    }

    public void ClearScreen() {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    public void RenderMesh(Mesh mesh, Shader shader) {
        ClearScreen();
        shader.Use();

        shader.SetMatrix4("projection_from_view", ActiveCamera.GetProjectionMatrix());
        shader.SetMatrix4("view_from_world", ActiveCamera.GetViewMatrix());        
        shader.SetMatrix4("world_from_object", Matrix4.Identity);

        mesh.Draw();
    }

    public static void Unload() {
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