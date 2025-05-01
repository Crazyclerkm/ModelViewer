using OpenTK.Mathematics;

namespace ModelViewer.Graphics {
    public class Model {

        public List<Mesh> Meshes;

        private BoundingBox localAABB;
        public BoundingBox AABB {
            get {
                return localAABB.ApplyTransform(GetModelMatrix());
            }
        }

        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        public string Name {get; private set;}

        public Model(List<Mesh> meshes, string name) {
            Meshes = meshes;
            Name = name;
            localAABB = new BoundingBox([.. Meshes.SelectMany(mesh => mesh.GetVertexPositions())]);
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
    }
}