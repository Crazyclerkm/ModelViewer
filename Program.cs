using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using ModelViewer.Resources;
using ModelViewer.Graphics;
using ModelViewer.Core;
using ModelViewer.UI;

public class Viewer : GameWindow {

    private readonly Renderer ViewRenderer;
    private ImGuiController UIController;

    public Viewer(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() {
        ClientSize = (width, height),
        Title = title
    }) { 
        ViewRenderer = new Renderer(width, height);
        UIController = new ImGuiController(width, height);

        UIController.OnModelLoaded += AddModel;
    }

    static void Main() {
        using(Viewer viewer = new Viewer(800, 600, "Model Viewer")) {
            viewer.Run();
        }
    }

    protected override void OnRenderFrame(FrameEventArgs e) {
        base.OnRenderFrame(e);
        
        ViewRenderer.RenderScene();
        UIController.RenderUI();

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e) {
        base.OnUpdateFrame(e);

        ProcessInput(e);
        UIController.Update((float)e.Time);    
    }

    protected override void OnUnload() {
        Renderer.Unload();
        ResourceManager.Unload();

        base.OnUnload();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e) {
        base.OnFramebufferResize(e);

        ViewRenderer.ResizeViewport(e.Width, e.Height);
        UIController.WindowResized(e.Width, e.Height);
    }

    private void ProcessInput(FrameEventArgs e) {
        KeyboardState input = KeyboardState;

        UIController.UpdateKeyboardState(input);

        ImGuiIOPtr io = ImGui.GetIO();
        bool isModalOpen = ImGui.IsPopupOpen("Open File", ImGuiPopupFlags.AnyPopupId);

        if (!isModalOpen && !io.WantTextInput) {
            // Window controls
            if (input.IsKeyDown(Keys.Escape) && !io.WantCaptureKeyboard) Close();

            // Movement controls
            if (input.IsKeyDown(Keys.W)) ViewRenderer.ActiveCamera.Move(CameraMovement.FORWARD, (float)e.Time);
            if (input.IsKeyDown(Keys.S)) ViewRenderer.ActiveCamera.Move(CameraMovement.BACKWARD, (float)e.Time);
            if (input.IsKeyDown(Keys.A)) ViewRenderer.ActiveCamera.Move(CameraMovement.LEFT, (float)e.Time);
            if (input.IsKeyDown(Keys.D)) ViewRenderer.ActiveCamera.Move(CameraMovement.RIGHT, (float)e.Time);
            if (input.IsKeyDown(Keys.Space)) ViewRenderer.ActiveCamera.Move(CameraMovement.UP, (float)e.Time);
            if (input.IsKeyDown(Keys.LeftShift)) ViewRenderer.ActiveCamera.Move(CameraMovement.DOWN, (float)e.Time);

            if (!io.WantCaptureKeyboard) {
                if (input.IsKeyDown(Keys.Right)) ViewRenderer.ActiveCamera.Rotate(0.1f, 0.0f); 
                if (input.IsKeyDown(Keys.Left)) ViewRenderer.ActiveCamera.Rotate(-0.1f, 0.0f);
                if (input.IsKeyDown(Keys.Up)) ViewRenderer.ActiveCamera.Rotate(0.0f, -0.1f); 
                if (input.IsKeyDown(Keys.Down)) ViewRenderer.ActiveCamera.Rotate(0.0f, 0.1f);
            }  

            if (input.IsKeyDown(Keys.LeftControl) & input.IsKeyDown(Keys.R)) {
                ViewRenderer.ActiveCamera.Position = new Vector3(0.0f, 0.0f, 2.0f);
                ViewRenderer.ActiveCamera.Pitch = 0.0f;
                ViewRenderer.ActiveCamera.Yaw = -MathHelper.PiOver2;

                ViewRenderer.ActiveCamera.Rotate(0f, 0f);
            }
        }
        
    }

    protected override void OnMouseMove(MouseMoveEventArgs e) {
        base.OnMouseMove(e);
        UIController.OnMouseMove(e.X, e.Y);

        ImGuiIOPtr io = ImGui.GetIO();
        
        if(!io.WantCaptureMouse && IsFocused && MouseState.IsButtonDown(MouseButton.Middle)) {
            ViewRenderer.ActiveCamera.Rotate(e.DeltaX, e.DeltaY);
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e) {
        base.OnMouseWheel(e);
        ViewRenderer.ActiveCamera.Zoom(e.OffsetY);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e) {
        base.OnMouseDown(e);
        UIController.OnMouseDown(e);

        ImGuiIOPtr io = ImGui.GetIO();
        if (!io.WantCaptureMouse && IsFocused && e.Button == MouseButton.Button1) {
            Model? selectedModel = ViewRenderer.SelectModel(MouseState.X, MouseState.Y);
            UIController.SetSelectedModel(selectedModel);
        }
    }

    protected override void OnMouseUp(MouseButtonEventArgs e) {
        base.OnMouseUp(e);
        UIController.OnMouseUp(e);
    }

    protected override void OnTextInput(TextInputEventArgs e) {
        base.OnTextInput(e);

        UIController.PressChar((char)e.Unicode);
    }

    private void AddModel(Model model) {
        ViewRenderer.AddModel(model);
    }
}