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

    private Scene ViewScene;

    public Viewer(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() {
        ClientSize = (width, height),
        Title = title
    }) { 
        ViewScene = new Scene();
        ViewScene.ActiveCamera.AspectRatio = width / (float) height;
        ViewRenderer = new Renderer();
        UIController = new ImGuiController(ViewScene, width, height);
    }

    static void Main() {
        using(Viewer viewer = new Viewer(800, 600, "Model Viewer")) {
            viewer.Run();
        }
    }

    protected override void OnRenderFrame(FrameEventArgs e) {
        base.OnRenderFrame(e);
        
        ViewRenderer.RenderScene(ViewScene);
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

        foreach (Camera camera in ViewScene.Cameras) {
            camera.AspectRatio = e.Width / (float) e.Height;
        }
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
            if (input.IsKeyDown(Keys.W)) ViewScene.ActiveCamera.Move(CameraMovement.FORWARD, (float)e.Time);
            if (input.IsKeyDown(Keys.S)) ViewScene.ActiveCamera.Move(CameraMovement.BACKWARD, (float)e.Time);
            if (input.IsKeyDown(Keys.A)) ViewScene.ActiveCamera.Move(CameraMovement.LEFT, (float)e.Time);
            if (input.IsKeyDown(Keys.D)) ViewScene.ActiveCamera.Move(CameraMovement.RIGHT, (float)e.Time);
            if (input.IsKeyDown(Keys.Space)) ViewScene.ActiveCamera.Move(CameraMovement.UP, (float)e.Time);
            if (input.IsKeyDown(Keys.LeftShift)) ViewScene.ActiveCamera.Move(CameraMovement.DOWN, (float)e.Time);

            if (!io.WantCaptureKeyboard) {
                if (input.IsKeyDown(Keys.Right)) ViewScene.ActiveCamera.Rotate(0.1f, 0.0f); 
                if (input.IsKeyDown(Keys.Left)) ViewScene.ActiveCamera.Rotate(-0.1f, 0.0f);
                if (input.IsKeyDown(Keys.Up)) ViewScene.ActiveCamera.Rotate(0.0f, -0.1f); 
                if (input.IsKeyDown(Keys.Down)) ViewScene.ActiveCamera.Rotate(0.0f, 0.1f);
            }  

            if (input.IsKeyDown(Keys.LeftControl) && input.IsKeyDown(Keys.R)) {
                ViewScene.ActiveCamera.Position = new Vector3(0.0f, 0.0f, 2.0f);
                ViewScene.ActiveCamera.Pitch = 0.0f;
                ViewScene.ActiveCamera.Yaw = -MathHelper.PiOver2;

                ViewScene.ActiveCamera.Rotate(0f, 0f);
            }

            if (input.IsKeyDown(Keys.LeftControl) && input.IsKeyPressed(Keys.L)) {
                UIController.ToggleLightingMenu();
            }
        }
        
    }

    protected override void OnMouseMove(MouseMoveEventArgs e) {
        base.OnMouseMove(e);
        UIController.OnMouseMove(e.X, e.Y);

        ImGuiIOPtr io = ImGui.GetIO();
        
        if(!io.WantCaptureMouse && IsFocused && MouseState.IsButtonDown(MouseButton.Middle)) {
            ViewScene.ActiveCamera.Rotate(e.DeltaX, e.DeltaY);
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e) {
        base.OnMouseWheel(e);
        UIController.OnMouseWheel(e);

        ImGuiIOPtr io = ImGui.GetIO();
        if (!io.WantCaptureMouse) {
            ViewScene.ActiveCamera.Zoom(e.OffsetY);
        }
    }

    protected override void OnMouseDown(MouseButtonEventArgs e) {
        base.OnMouseDown(e);
        UIController.OnMouseDown(e);

        ImGuiIOPtr io = ImGui.GetIO();
        if (!io.WantCaptureMouse && IsFocused && e.Button == MouseButton.Button1) {
            Model? selectedModel = ViewScene.SelectModel(MouseState.X, MouseState.Y, ClientSize.X, ClientSize.Y);
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
}