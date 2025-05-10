using OpenTK.Mathematics;

namespace ModelViewer.Graphics {
    
    public enum LightType {
        DIRECTIONAL,
        POINT
    }

    public class Light {
        public LightType Type;

        public Vector3 Position;
        public Vector3 Direction;
        public Vector3 Ambient;
        public Vector3 Specular;
        public Vector3 Diffuse;

        public float ConstantAttenuation;
        public float LinearAttenuation;
        public float QuadraticAttenuation;

        public float CutOff;
        public float OuterCutOff;

        public bool Enabled = true;

        public static Light DefaultPointLight(Vector3 position) {
            Light light = new()
            {
                Type = LightType.POINT,

                Position = position,

                Ambient = new Vector3(0.2f, 0.2f, 0.2f),
                Diffuse = new Vector3(0.5f, 0.5f, 0.5f),
                Specular = new Vector3(1.0f, 1.0f, 1.0f),

                ConstantAttenuation = 1.0f,
                LinearAttenuation = 0.09f,
                QuadraticAttenuation = 0.032f
            };

            return light;
        }

        public static Light DefaultDirectionalLight(Vector3 direction) {
            Light light = new()
            {
                Type = LightType.DIRECTIONAL,

                Direction = direction,

                Ambient = new Vector3(0.2f, 0.2f, 0.2f),
                Diffuse = new Vector3(0.5f, 0.5f, 0.5f),
                Specular = new Vector3(1.0f, 1.0f, 1.0f),

                ConstantAttenuation = 1.0f,
                LinearAttenuation = 0.09f,
                QuadraticAttenuation = 0.032f
            };

            return light;
        }
    }
}