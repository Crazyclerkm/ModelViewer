using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;

public struct Vertex {
    public Vector3 Position;
    public Vector3 Normal;
    public Vector2 TexCoords;

    public static int SizeInBytes => (sizeof(float) * 3) + (sizeof(float) * 3) + (sizeof(float) * 2);

    public Vertex(Vector3 position, Vector3 normal, Vector2 texCoords) {
        Position = position;
        Normal = normal;
        TexCoords = texCoords;
    }   
}

public class Mesh {

    private List<Vertex> Vertices;
    private List<uint> Indices;

    public Material? Material;

    private int VAO;
    private int VBO;
    private int EBO;

    public Mesh(List<Vertex> vertices, List<uint> indices, Material? material) {
        Vertices = vertices;
        Indices = indices;
        Material = material;

        SetupMesh();
    }

    public Mesh() {
        Vertices = [];
        Indices = [];
    }

    public void Draw() {
        Material?.DiffuseTexture?.Use(TextureUnit.Texture0);
        Material?.SpecularColourTexture?.Use(TextureUnit.Texture1);
        Material?.SpecularHighlightTexture?.Use(TextureUnit.Texture2);

        GL.BindVertexArray(VAO);
        GL.DrawElements(PrimitiveType.Triangles, Indices.Count, DrawElementsType.UnsignedInt, 0);
        
        GL.BindVertexArray(0);
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.ActiveTexture(0);
    }

    public void SetupMesh() {
        VAO = GL.GenVertexArray();
        VBO = GL.GenBuffer();
        EBO = GL.GenBuffer();

        GL.BindVertexArray(VAO);

        // Setup VBO
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Count * Vertex.SizeInBytes, Vertices.ToArray(), BufferUsageHint.StaticDraw);

        // Setup EBO
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Count * sizeof(uint), Indices.ToArray(), BufferUsageHint.StaticDraw);

        int vertexLocation = 0;
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 0);

        int normalLocation = 1;
        GL.EnableVertexAttribArray(normalLocation);
        GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Marshal.OffsetOf<Vertex>(nameof(Vertex.Normal)));

        int textureLocation = 2;
        GL.EnableVertexAttribArray(textureLocation);
        GL.VertexAttribPointer(textureLocation, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Marshal.OffsetOf<Vertex>(nameof(Vertex.TexCoords)));

        GL.BindVertexArray(0);
    }

    ~Mesh() {
        GL.DeleteVertexArray(VAO);
        GL.DeleteBuffer(VBO);
        GL.DeleteBuffer(EBO);
    }
}