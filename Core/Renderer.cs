using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using ModelViewer.Resources;
using ModelViewer.Graphics;

namespace ModelViewer.Core {
    public class Renderer {
        private const int MAX_LIGHTS = 64; 

        private Shader DefaultShader = ResourceManager.LoadShader("Default", "Resources/Shaders/basicShader.vs", "Resources/Shaders/lightingShader.fs");
        private Shader LineShader = ResourceManager.LoadShader("Line", "Resources/Shaders/lineShader.vs", "Resources/Shaders/lineShader.fs");

        public Renderer() {
            GL.ClearColor(0.1f, 0.2f, 0.2f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
        }
        
        public static void ClearScreen() {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        }

        public void RenderScene(Scene scene) {
            ClearScreen();

            Matrix4 projectionFromView = scene.ActiveCamera.GetProjectionMatrix();
            Matrix4 viewFromWorld = scene.ActiveCamera.GetViewMatrix();
            Matrix4 worldFromObject;

            DefaultShader.Use();

            DefaultShader.SetVec3("viewPos", scene.ActiveCamera.Position);
            DefaultShader.SetMatrix4("projectionFromView", projectionFromView);
            DefaultShader.SetMatrix4("viewFromWorld", viewFromWorld);

            IEnumerable<Light> DirLights = scene.Lights.Where(light => light.Type == LightType.DIRECTIONAL);
            IEnumerable<Light> PointLights = scene.Lights.Where(light => light.Type == LightType.POINT);

            for (int i=0; i<DirLights.Count() && i<MAX_LIGHTS; i++) {
                DefaultShader.SetVec3($"dirLights[{i}].direction", DirLights.ElementAt(i).Direction);

                DefaultShader.SetVec3($"dirLights[{i}].ambient", DirLights.ElementAt(i).Ambient);
                DefaultShader.SetVec3($"dirLights[{i}].diffuse", DirLights.ElementAt(i).Diffuse);
                DefaultShader.SetVec3($"dirLights[{i}].specular", DirLights.ElementAt(i).Specular);
            }

            DefaultShader.SetInt("numDirLights", DirLights.Count());

            for (int i=0; i<PointLights.Count() && i<MAX_LIGHTS; i++) {
                DefaultShader.SetVec3($"pointLights[{i}].position", PointLights.ElementAt(i).Position);

                DefaultShader.SetVec3($"pointLights[{i}].ambient", PointLights.ElementAt(i).Ambient);
                DefaultShader.SetVec3($"pointLights[{i}].diffuse", PointLights.ElementAt(i).Diffuse);
                DefaultShader.SetVec3($"pointLights[{i}].specular", PointLights.ElementAt(i).Specular);

                DefaultShader.SetFloat($"pointLights[{i}].constant", PointLights.ElementAt(i).ConstantAttenuation);
                DefaultShader.SetFloat($"pointLights[{i}].linear", PointLights.ElementAt(i).LinearAttenuation);
                DefaultShader.SetFloat($"pointLights[{i}].quadratic", PointLights.ElementAt(i).QuadraticAttenuation); 
            }

            DefaultShader.SetInt("numPointLights", PointLights.Count());

            foreach (Model model in scene.Models) {
                worldFromObject = model.GetModelMatrix(); 
                DefaultShader.SetMatrix4("worldFromObject", worldFromObject); 

                foreach (Mesh mesh in model.Meshes) {
                    if(mesh.Material != null) {
                        DefaultShader.SetFloat("material.shininess", mesh.Material.SpecularExponent);

                        if (mesh.Material.DiffuseTexture != null) {
                            DefaultShader.SetInt("material.diffuseMap", 0);
                            
                        }

                        if (mesh.Material.SpecularHighlightTexture != null) {
                            DefaultShader.SetInt("material.specularMap", 2);
                        }
                        
                    }
                    mesh.Draw();
                }
            }

            LineShader.Use();
            LineShader.SetMatrix4("projectionFromView", projectionFromView);
            LineShader.SetMatrix4("viewFromWorld", viewFromWorld); 
        
            foreach (Raycaster ray in scene.Rays) {
                worldFromObject = ray.GetModelMatrix();
                LineShader.SetMatrix4("worldFromObject", worldFromObject);
                LineShader.SetVec3("lineColour", ray.Colour);
                ray.Draw();
            }
            
            GL.UseProgram(0);
        }

        public static void Unload() {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }

        public void ResizeViewport(int x, int y, int width, int height) {
            GL.Viewport(x, y, width, height);
        }

        public void ResizeViewport(int width, int height) {
            ResizeViewport(0, 0, width, height);
        }
    }
}