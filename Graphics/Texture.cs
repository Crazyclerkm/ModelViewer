using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace ModelViewer.Graphics {
    public enum Texture_Type {
        DIFFUSE,
        SPECULAR_COLOUR,
        SPECULAR_HIGHLIGHT,
        ALPHA,
        BUMP,
        DISPLACEMENT,
        DECAL

    }

    public class Texture {
        public readonly int Handle;

        public readonly Texture_Type Type;

        public static Texture LoadFromFile(string filePath) {
            int handle = GL.GenTexture();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, handle);

            StbImage.stbi_set_flip_vertically_on_load(1);

            using(Stream stream = File.OpenRead(filePath)) {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return new Texture(handle);
        }

        public Texture(int glHandle) {
            Handle = glHandle;
        }

        public void Use(TextureUnit unit) {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }
}