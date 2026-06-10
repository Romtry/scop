using Silk.NET.OpenGL;
using Silk.NET.Maths;
using StbImageSharp;
using System.IO;

namespace Scop
{
    partial class Program
    {
        private static unsafe void Load2D()
        {
            // Input
            InputUtils.SetupInput(window, KeyDown);

            // GL Init
            Gl = GL.GetApi(window);
            Gl.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Buffers
            (Vao, Vbo, Ebo) = BufferUtils.CreateBuffers(Gl, Vertices2D, Indices2D);

            // Shader
            Shader = ShaderUtils.CreateShaderProgram(Gl, VertexShaderSource2D, FragmentShaderSource2D);

            // Vertex Attributes
            const uint positionLoc = 0;
            Gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)0);
            Gl.EnableVertexAttribArray(positionLoc);

            const uint texCoordLoc = 1;
            Gl.VertexAttribPointer(texCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));
            Gl.EnableVertexAttribArray(texCoordLoc);

            // Texture
            _texture = Gl.GenTexture();
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2D, _texture);

            ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(IMAGE_PATH), ColorComponents.RedGreenBlueAlpha);
            fixed (byte* ptr = result.Data)
                Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)result.Width,
                    (uint)result.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);

            Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.Repeat);
            Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.Repeat);
            Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Nearest);
            Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Nearest);

            int location = Gl.GetUniformLocation(Shader, "uTexture");
            Gl.Uniform1(location, 0);
        }
    }
}
