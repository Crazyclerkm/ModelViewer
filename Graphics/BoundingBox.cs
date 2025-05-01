using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ModelViewer.Graphics {
    public class BoundingBox {
        public Vector3 Min {get; private set;}
        public Vector3 Max {get; private set;}

        private Vector3[] Corners;

        private static readonly uint[] Indices = [
            0, 1, 1, 2, 2, 3, 3, 0, // Bottom face
            4, 5, 5, 6, 6, 7, 7, 4, // Top face
            0, 4, 1, 5, 2, 6, 3, 7  // Vertical edges
        ];

        private int VAO, VBO, EBO;

        public BoundingBox(Vector3 min, Vector3 max) {
            Min = min;
            Max = max;

            Corners = [
                new Vector3(Min.X, Min.Y, Min.Z),
                new Vector3(Max.X, Min.Y, Min.Z),
                new Vector3(Max.X, Max.Y, Min.Z),
                new Vector3(Min.X, Max.Y, Min.Z),
                new Vector3(Min.X, Min.Y, Max.Z),
                new Vector3(Max.X, Min.Y, Max.Z),
                new Vector3(Max.X, Max.Y, Max.Z),
                new Vector3(Min.X, Max.Y, Max.Z)
            ];

            SetupBuffers();
        }

        public BoundingBox(List<Vector3> positions) : this(
            positions.Aggregate(new Vector3(float.MaxValue), Vector3.ComponentMin),
            positions.Aggregate(new Vector3(float.MinValue), Vector3.ComponentMax)
        ) {}

        public BoundingBox ApplyTransform(Matrix4 transform) {
            var transformedCorners = Corners.Select( corner => {
                Vector4 transformedCorner = Vector4.TransformRow(new Vector4(corner, 1.0f), transform);
                return transformedCorner.Xyz;
            });

            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            foreach (var corner in transformedCorners) {
                min = Vector3.ComponentMin(min, corner);
                max = Vector3.ComponentMax(max, corner);
            }

            return new BoundingBox(min, max);
        }

        private void SetupBuffers() {
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            // Setup VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Corners.Length * Vector3.SizeInBytes, Corners, BufferUsageHint.StaticDraw);

            // Setup EBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);

            int vertexLocation = 0;
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.BindVertexArray(0);
        }
        

        public void Draw() {
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Lines, Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }
}