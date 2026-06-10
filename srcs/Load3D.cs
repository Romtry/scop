// using Silk.NET.Input;
// using Silk.NET.OpenGL;
// using Silk.NET.Maths;
// using StbImageSharp;
// using System.IO;

// namespace Scop
// {
//     partial class Program
//     {
//         private static void Load3D()
//         {
// 			IInputContext input = window.CreateInput();
//             for (int i = 0; i < input.Keyboards.Count; i++)
//             {
//                 input.Keyboards[i].KeyDown += KeyDown;
//             }

//             Gl = GL.GetApi(window);
//             Gl.Enable(EnableCap.Blend);

//             Vao = Gl.GenVertexArray();
//             Gl.BindVertexArray(Vao);

//             Vbo = Gl.GenBuffer();
//             Gl.BindBuffer(BufferTargetARB.ArrayBuffer, Vbo);
//             fixed (void* v = &Vertices2D[0])
//             {
//                 Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (Vertices2D.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw);
//             }

//             Gl.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);

//             Ebo = Gl.GenBuffer();
//             Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, Ebo);
//             fixed (void* i = &Indices2D[0])
//             {
//                 Gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint) (Indices2D.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw);
//             }

//             uint vertexShader = Gl.CreateShader(ShaderType.VertexShader);
//             Gl.ShaderSource(vertexShader, VertexShaderSource2D);
//             Gl.CompileShader(vertexShader);

//             string infoLog = Gl.GetShaderInfoLog(vertexShader);
//             if (!string.IsNullOrWhiteSpace(infoLog))
//             {
//                 Console.WriteLine($"Error compiling vertex shader {infoLog}");
//             }

//             uint fragmentShader = Gl.CreateShader(ShaderType.FragmentShader);
//             Gl.ShaderSource(fragmentShader, FragmentShaderSource2D);
//             Gl.CompileShader(fragmentShader);

//             infoLog = Gl.GetShaderInfoLog(fragmentShader);
//             if (!string.IsNullOrWhiteSpace(infoLog))
//             {
//                 Console.WriteLine($"Error compiling fragment shader {infoLog}");
//             }

//             Shader = Gl.CreateProgram();
//             Gl.AttachShader(Shader, vertexShader);
//             Gl.AttachShader(Shader, fragmentShader);
//             Gl.LinkProgram(Shader);

//             Gl.GetProgram(Shader, GLEnum.LinkStatus, out var status);
//             if (status == 0)
//             {
//                 Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(Shader)}");
//             }

//             Gl.DetachShader(Shader, vertexShader);
//             Gl.DetachShader(Shader, fragmentShader);
//             Gl.DeleteShader(vertexShader);
//             Gl.DeleteShader(fragmentShader);

//             const uint positionLoc = 0;
//             Gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)0);
//             Gl.EnableVertexAttribArray(0);

//             const uint texCoordLoc = 1;
//             Gl.EnableVertexAttribArray(texCoordLoc);
//             Gl.VertexAttribPointer(texCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));

//             _texture = Gl.GenTexture();
//             Gl.ActiveTexture(TextureUnit.Texture0);
//             Gl.BindTexture(TextureTarget.Texture2D, _texture);
//             ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(IMAGE_PATH), ColorComponents.RedGreenBlueAlpha);
//             fixed (byte* ptr = result.Data)
//             Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)result.Width,
//             (uint)result.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);

//             Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.Repeat);
//             Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.Repeat);
//             Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Nearest);
//             Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Nearest);
//             Gl.BindTexture(TextureTarget.Texture2D, 0);
//             int location = Gl.GetUniformLocation(Shader, "uTexture");
//             Gl.Uniform1(location, 0);

//             Gl.Enable(EnableCap.Blend);
//             Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
// 		}
// 	}
// }
