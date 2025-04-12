using OpenTK.Mathematics;

public enum MaterialType {
        DIFFUSE_ONLY,
        DIFFUSE_SPECULAR
}

public class Material {
        public string Name;

        public Texture? DiffuseTexture;
        public Texture? SpecularColourTexture;
        public Texture? SpecularHighlightTexture;

        public Vector3 AmbientColour;
        public Vector3 DiffuseColour;
        public Vector3 SpecularColour;

        public float SpecularExponent;
        public float OpticalDensity;

        public Material(string name) {
            Name = name;
        }

        public MaterialType GetMaterialType() {
            bool hasSpecular = !((SpecularColourTexture == null) && (SpecularHighlightTexture == null));

            if (hasSpecular) return MaterialType.DIFFUSE_SPECULAR;

            return MaterialType.DIFFUSE_ONLY;
        }
}