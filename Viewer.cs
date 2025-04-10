using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ModelViewer.Core {
    public class Viewer : GameWindow {

        private Renderer renderer;
        private Shader shader;

        private List<Vertex> RectangleVertices = new List<Vertex> {
            new Vertex(new Vector3( 0.5f,  0.5f, 0.0f), Vector3.Zero, new Vector2(1.0f, 1.0f)),
            new Vertex(new Vector3( 0.5f, -0.5f, 0.0f), Vector3.Zero, new Vector2(1.0f, 0.0f)),
            new Vertex(new Vector3(-0.5f, -0.5f, 0.0f), Vector3.Zero, new Vector2(0.0f, 0.0f)),
            new Vertex(new Vector3(-0.5f,  0.5f, 0.0f), Vector3.Zero, new Vector2(0.0f, 1.0f))
        };

        private List<uint> RectangleIndices = new List<uint> {
            0, 1, 3,
            1, 2, 3
        };

        private List<Texture> RectangleTextures = new List<Texture>();
        private List<Mesh> meshes = [];
        public Viewer(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() {
            ClientSize = (width, height),
            Title = title
        }) { }

        static void Main() {
            using(Viewer viewer = new Viewer(800, 600, "Model Viewer")) {
                viewer.Run();
            }
        }

        protected override void OnLoad() {
            base.OnLoad();

            renderer = new Renderer(Size.X, Size.Y);
            
            Mesh rectangle = new Mesh(RectangleVertices, RectangleIndices, RectangleTextures);
            meshes.Add(rectangle);
           
            shader = ResourceManager.LoadShader("basic", "Resources/Shaders/basicShader.vs", "Resources/Shaders/basicShader.fs");
            
            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            
            foreach(var mesh in meshes) {
                renderer.RenderMesh(mesh, shader);
            }

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

            if (!IsFocused) return; 

            ProcessInput(e);
        }

        protected override void OnUnload() {
            Renderer.Unload();
            ResourceManager.Unload();

            base.OnUnload();
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            renderer.ResizeViewport(e.Width, e.Height);
        }

        private void ProcessInput(FrameEventArgs e) {
            KeyboardState input = KeyboardState;

            // Window controls
            if (input.IsKeyDown(Keys.Escape)) Close();

            // Movement controls
            if (input.IsKeyDown(Keys.W)) renderer.ActiveCamera.Move(CameraMovement.FORWARD, (float)e.Time);
            if (input.IsKeyDown(Keys.S)) renderer.ActiveCamera.Move(CameraMovement.BACKWARD, (float)e.Time);
            if (input.IsKeyDown(Keys.A)) renderer.ActiveCamera.Move(CameraMovement.LEFT, (float)e.Time);
            if (input.IsKeyDown(Keys.D)) renderer.ActiveCamera.Move(CameraMovement.RIGHT, (float)e.Time);
            if (input.IsKeyDown(Keys.Space)) renderer.ActiveCamera.Move(CameraMovement.UP, (float)e.Time);
            if (input.IsKeyDown(Keys.LeftShift)) renderer.ActiveCamera.Move(CameraMovement.DOWN, (float)e.Time);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e) {
            base.OnMouseMove(e);

            if(IsFocused) {
                renderer.ActiveCamera.Rotate(e.DeltaX, e.DeltaY);
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            base.OnMouseWheel(e);
            renderer.ActiveCamera.Zoom(e.OffsetY);
        }
    }
}