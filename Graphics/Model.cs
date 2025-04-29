using OpenTK.Mathematics;

namespace ModelViewer.Graphics {
    public class Model {

        public List<Mesh> Meshes;

        public Vector3 AABBmin {get; private set;}
        public Vector3 AABBmax {get; private set;}

        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        public string Name {get; private set;}

        public Model(List<Mesh> meshes, string name) {
            Meshes = meshes;
            Name = name;
            ComputeAABB();
            Scale = Vector3.One;
        }

        public Matrix4 GetModelMatrix() {
            Matrix4 scale = Matrix4.CreateScale(Scale.X, Scale.Y, Scale.Z);
            Matrix4 translation = Matrix4.CreateTranslation(Position);

            Matrix4 rotationX = Matrix4.CreateRotationX(Rotation.X);
            Matrix4 rotationY = Matrix4.CreateRotationY(Rotation.Y);
            Matrix4 rotationZ = Matrix4.CreateRotationZ(Rotation.Z);

            Matrix4 rotation = rotationY * rotationX * rotationZ;

            return  scale * rotation * translation;
        }

        private void ComputeAABB() {
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            foreach (var mesh in Meshes) {
                min = Vector3.ComponentMin(min, mesh.AABBmin);
                max = Vector3.ComponentMax(max, mesh.AABBmax);
            }

            AABBmin = min;
            AABBmax = max;
        }
    }
}