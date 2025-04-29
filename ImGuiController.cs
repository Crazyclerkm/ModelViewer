using System.Runtime.CompilerServices;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

public class ImGuiController {
    private int VAO;
    private int VBO;
    private int EBO;

    public Matrix4 Projection;

    private int WindowWidth;
    private int WindowHeight;

    private Renderer WindowRenderer;

    private FileDialog OpenFileDialog = new();

    public ImGuiController(int width, int height, Renderer renderer) {
        WindowWidth = width;
        WindowHeight = height;

        WindowRenderer = renderer;

        Projection = Matrix4.CreateOrthographicOffCenter(
            0.0f, WindowWidth, WindowHeight, 0.0f, -1.0f, 1.0f
        );

        IntPtr context = ImGui.CreateContext();
        ImGui.SetCurrentContext(context);

        var io = ImGui.GetIO();
        io.Fonts.AddFontDefault();

        
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

        VAO = GL.GenVertexArray();
        VBO = GL.GenBuffer();
        EBO = GL.GenBuffer();

        GL.BindVertexArray(VAO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

        int stride = Unsafe.SizeOf<ImDrawVert>();

        int positionLocation = 0;
        GL.EnableVertexAttribArray(positionLocation);
        GL.VertexAttribPointer(positionLocation, 2, VertexAttribPointerType.Float, false, stride, 0);

        int UVLocation = 1;
        GL.EnableVertexAttribArray(UVLocation);
        GL.VertexAttribPointer(UVLocation, 2, VertexAttribPointerType.Float, false, stride, 8);

        int colourLocation = 2;
        GL.EnableVertexAttribArray(colourLocation);
        GL.VertexAttribPointer(colourLocation, 4, VertexAttribPointerType.UnsignedByte, true, stride, 16);

        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        RebuildFontAtlas();
    }

    private void RebuildFontAtlas() {
        var io = ImGui.GetIO();

        io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height);

        int fontTexture = GL.GenTexture();

        GL.BindTexture(TextureTarget.Texture2D, fontTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

        GL.TextureParameter(fontTexture, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TextureParameter(fontTexture, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        io.Fonts.SetTexID((IntPtr)fontTexture);

        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Update(float deltaTime) {
        var io = ImGui.GetIO();

        io.DeltaTime = deltaTime;
        io.DisplaySize = new System.Numerics.Vector2(WindowWidth, WindowHeight);
        ImGui.NewFrame();
    }

    public void Draw(Shader shader) {
        bool showDialog = false;

        if (ImGui.BeginMainMenuBar()) {
            if (ImGui.BeginMenu("File")) {
                if (ImGui.MenuItem("Open Model")) {
                    showDialog = true;
                }
                ImGui.EndMenu();
            }
            ImGui.EndMainMenuBar();
        }

        if (showDialog) {
            ImGui.OpenPopup("Open File");
        }

        OpenFileDialog.Show((filePath) => {
            Model model = ResourceManager.LoadModel((string)filePath);
            WindowRenderer.AddModel(model);
        });

        OpenFileDialog.Render();
        ImGui.Render();

        ImDrawDataPtr drawData = ImGui.GetDrawData();

        if (drawData.CmdListsCount == 0) {
            return;
        }

        ImGuiIOPtr io = ImGui.GetIO();
        drawData.ScaleClipRects(io.DisplayFramebufferScale);

        GL.Enable(EnableCap.Blend);
        GL.BlendEquation(BlendEquationMode.FuncAdd);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Disable(EnableCap.CullFace);
        GL.Disable(EnableCap.DepthTest);
        GL.Enable(EnableCap.ScissorTest);

        GL.Viewport(0, 0, WindowWidth, WindowHeight);

        GL.BindVertexArray(VAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);

        int stride = sizeof(float) * 2 + sizeof(float) * 2 + sizeof(uint);

        shader.Use();
        shader.SetMatrix4("projection", Projection);
        

        for (int i = 0; i < drawData.CmdListsCount; i++) {
            ImDrawListPtr cmdList = drawData.CmdLists[i];
            
            GL.BufferData(BufferTarget.ArrayBuffer, cmdList.VtxBuffer.Size * stride, cmdList.VtxBuffer.Data ,BufferUsageHint.StreamDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, cmdList.IdxBuffer.Size * sizeof(ushort), cmdList.IdxBuffer.Data, BufferUsageHint.StreamDraw);

            for (int cmd_n = 0; cmd_n < cmdList.CmdBuffer.Size; cmd_n++) {
                ImDrawCmdPtr pcmd = cmdList.CmdBuffer[cmd_n];
                
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, (int)pcmd.TextureId);

                var clip = pcmd.ClipRect;
                GL.Scissor((int)clip.X, WindowHeight - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));

                GL.DrawElements(BeginMode.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (int)pcmd.IdxOffset * sizeof(ushort));
            }
        }
        
        // Cleanup
        GL.Disable(EnableCap.ScissorTest);
        GL.Enable(EnableCap.DepthTest);
        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }

    public void WindowResized(int width, int height) {
        WindowWidth = width;
        WindowHeight = height;
        Projection = Matrix4.CreateOrthographicOffCenter(
            0.0f, WindowWidth, WindowHeight, 0.0f, -1.0f, 1.0f
        );
    }

    public void OnMouseMove(float x, float y) {
        ImGui.GetIO().MousePos = new System.Numerics.Vector2(x, y);
    }

    public void OnMouseDown(MouseButtonEventArgs e) {
        ImGui.GetIO().MouseDown[(int) e.Button] = true;
    }

    public void OnMouseUp(MouseButtonEventArgs e) {
        ImGui.GetIO().MouseDown[(int) e.Button] = false;
    }
}