using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class Renderer {

    public Renderer(int width, int height) {
        GL.ClearColor(0.1f, 0.2f, 0.2f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
    }

    public void ClearScreen() {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    public void RenderMesh(Mesh mesh, Shader shader) {
        ClearScreen();
        shader.Use();

        mesh.Draw();
    }

    public static void Unload() {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }

    public void ResizeViewport(int x, int y, int width, int height) {
        GL.Viewport(x, y, width, height);
    }

    public void ResizeViewport(int width, int height) {
        ResizeViewport(0, 0, width, height);
    }
}