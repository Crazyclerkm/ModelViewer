using ModelViewer.Graphics;
using OpenTK.Mathematics;

namespace ModelViewer.Core {
    public class Scene {
        public List<Model> Models {get;} = [];
        public List<Raycaster> Rays {get;} = [];
        public List<Camera> Cameras {get;} = [];
        public List<Light> Lights {get;} = [];

        public Camera ActiveCamera {get; set;}

        public Scene() {
            Camera camera = new Camera(Vector3.UnitZ*2);
            AddCamera(camera);
            ActiveCamera = camera;

            Light defaultLight = Light.DefaultPointLight(new Vector3(1.0f, 1.0f, 1.0f));
            Lights.Add(defaultLight);
        }

        public void AddModel(Model model) {Models.Add(model);}
        public void AddCamera(Camera camera) {Cameras.Add(camera);}

        public void AddLight(Light light) {Lights.Add(light);}

        public Model? SelectModel(float x, float y, int windowWidth, int windowHeight) {
            Raycaster ray = WorldRayFromScreenPoint(x, y, windowWidth, windowHeight);

            Model? closestModel = FindClosestModelFromRay(ray);

            return closestModel;
        }

        private Raycaster WorldRayFromScreenPoint(float x, float y, int windowWidth, int windowHeight) {
            Matrix4 viewFromProjection = ActiveCamera.GetProjectionMatrix().Transposed().Inverted();
            Matrix4 worldFromView = ActiveCamera.GetViewMatrix().Transposed().Inverted();

            float ndcX = (2.0f * x / windowWidth) - 1.0f;
            float ndcY = 1.0f - 2.0f * y / windowHeight;
            float ndcZ = 1.0f;

            Vector4 clipCoords = new Vector4(ndcX, ndcY, ndcZ, 1.0f);

            Vector4 viewCoords = viewFromProjection * clipCoords;
            viewCoords.W = 0.0f;

            Vector4 worldCoords = worldFromView * viewCoords;

            Vector3 rayDirection = Vector3.Normalize(worldCoords.Xyz);

            return new Raycaster(ActiveCamera.Position, rayDirection);
        }

        public Model? FindClosestModelFromRay(Raycaster ray) {
            Model? closestModel = null;
            float? closestDistance = null;

            foreach (Model model in Models) {
                Vector3 min = model.AABB.Min;
                Vector3 max = model.AABB.Max;

                bool intersects = ray.IntersectsBoundingBox(model.AABB.Min, model.AABB.Max, out float distance);

                if (!intersects) continue;

                if (closestDistance == null || distance < closestDistance) {
                    closestDistance = distance;
                    closestModel = model;
                }
            }
            return closestModel;
        }
    }
}