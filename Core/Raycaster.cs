using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ModelViewer.Core {
    public class Raycaster {
        public Vector3 Origin {get; set;}
        public Vector3 Direction {get; set;}

        public Vector3 Colour;

        private int VAO;
        private int VBO;

        public Raycaster(Vector3 origin, Vector3 direction) {
            Origin = origin;
            Direction = direction;

            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, 2 * Vector3.SizeInBytes, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.BindVertexArray(0);

            Colour = new Vector3(0.0f, 0.0f, 0.0f);
        }
        
        public Raycaster(Vector3 origin, Vector3 direction, Vector3 colour) : this(origin, direction) {
            Colour = colour;
        }

        public Matrix4 GetModelMatrix() {
            return Matrix4.Identity;
        }

        public bool IntersectsBoundingBox(Vector3 boxMin, Vector3 boxMax, out float distance) {
            distance = -1;
            float tmin = float.NegativeInfinity;
            float tmax = float.PositiveInfinity;
            
            for (int dim = 0; dim < 3; dim++) {
            if (Direction[dim] != 0.0f) {
                
                float t1 = (boxMin[dim] - Origin[dim]) / Direction[dim];
                float t2 = (boxMax[dim] - Origin[dim]) / Direction[dim];

                tmin = Math.Max(tmin, Math.Min(t1, t2));
                tmax = Math.Min(tmax, Math.Max(t1, t2));

            } else if (Origin[dim] < boxMin[dim] || Origin[dim] > boxMax[dim]) {
                    return false;
            }
            }

            if (tmax < 0 || tmin > tmax) {
                return false;
            }

            distance = tmin >= 0 ? tmin : tmax;
            
            return tmin < tmax;
        }

        public void Draw() {
            Vector3[] lineVerts = [Origin, Origin+Direction*100f];
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, 2*Vector3.SizeInBytes, lineVerts);

            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
            GL.BindVertexArray(0);
        }

    }
}