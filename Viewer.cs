using ImGuiNET;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ModelViewer.Core {
    public class Viewer : GameWindow {

        private readonly Renderer renderer;
        private ImGuiController GuiController;

        public Viewer(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() {
            ClientSize = (width, height),
            Title = title
        }) { 
            renderer = new Renderer(width, height);
            GuiController = new ImGuiController(width, height, renderer);
        }

        static void Main() {
            using(Viewer viewer = new Viewer(800, 600, "Model Viewer")) {
                viewer.Run();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            
           renderer.RenderScene();
           renderer.RenderUI(GuiController);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

            ProcessInput(e);
            GuiController.Update((float)e.Time);    
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
            ImGuiIOPtr io = ImGui.GetIO();

            if (!io.WantCaptureKeyboard) {
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
            
        }

        protected override void OnMouseMove(MouseMoveEventArgs e) {
            base.OnMouseMove(e);
            GuiController.OnMouseMove(e.X, e.Y);

            ImGuiIOPtr io = ImGui.GetIO();
            
            if(!io.WantCaptureMouse && IsFocused && MouseState.IsButtonDown(MouseButton.Middle)) {
                renderer.ActiveCamera.Rotate(e.DeltaX, e.DeltaY);
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            base.OnMouseWheel(e);
            renderer.ActiveCamera.Zoom(e.OffsetY);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            GuiController.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            GuiController.OnMouseUp(e);
        }
    }
}