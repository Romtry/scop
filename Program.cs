using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using Silk.NET.Maths;
using StbImageSharp;

namespace Tutorial
{
    class Program
    {
        private static IWindow window;
        private static GL Gl;

        private static uint Vbo;
        private static uint Ebo;
        private static uint Vao;
        private static uint Shader;
        private static uint _texture;

        private static readonly string VertexShaderSource = @"
        #version 330 core

        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec2 aTextureCoord;

        out vec2 frag_texCoords;

        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
            frag_texCoords = aTextureCoord;
        }";

        private static readonly string FragmentShaderSource = @"
        #version 330 core

        uniform sampler2D uTexture;
        in vec2 frag_texCoords;

        out vec4 out_color;

        void main()
        {
            out_color = texture(uTexture, frag_texCoords);
        }";

        private static readonly float[] Vertices =
        {
            0.5f,  0.5f, 0.0f,  1.0f, 1.0f,
            0.5f, -0.5f, 0.0f,  1.0f, 0.0f,
            -0.5f, -0.5f, 0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f, 0.0f,  0.0f, 1.0f
        };

        private static readonly uint[] Indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private static void Main(string[] args)
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(800, 600);
            options.Title = "LearnOpenGL with Silk.NET";
            window = Window.Create(options);

            window.Load += OnLoad;
            window.Render += OnRender;
            window.Update += OnUpdate;
            window.FramebufferResize += OnFramebufferResize;
            window.Closing += OnClose;

            window.Run();

            window.Dispose();
        }


        private static unsafe void OnLoad()
        {
            IInputContext input = window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += KeyDown;
            }

            Gl = GL.GetApi(window);
            Gl.Enable(EnableCap.Blend);

            Vao = Gl.GenVertexArray();
            Gl.BindVertexArray(Vao);

            Vbo = Gl.GenBuffer();
            Gl.BindBuffer(BufferTargetARB.ArrayBuffer, Vbo);
            fixed (void* v = &Vertices[0])
            {
                Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (Vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw);
            }

            Gl.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);

            Ebo = Gl.GenBuffer();
            Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, Ebo);
            fixed (void* i = &Indices[0])
            {
                Gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint) (Indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw);
            }

            uint vertexShader = Gl.CreateShader(ShaderType.VertexShader);
            Gl.ShaderSource(vertexShader, VertexShaderSource);
            Gl.CompileShader(vertexShader);

            string infoLog = Gl.GetShaderInfoLog(vertexShader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Console.WriteLine($"Error compiling vertex shader {infoLog}");
            }

            uint fragmentShader = Gl.CreateShader(ShaderType.FragmentShader);
            Gl.ShaderSource(fragmentShader, FragmentShaderSource);
            Gl.CompileShader(fragmentShader);

            infoLog = Gl.GetShaderInfoLog(fragmentShader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Console.WriteLine($"Error compiling fragment shader {infoLog}");
            }

            Shader = Gl.CreateProgram();
            Gl.AttachShader(Shader, vertexShader);
            Gl.AttachShader(Shader, fragmentShader);
            Gl.LinkProgram(Shader);

            Gl.GetProgram(Shader, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(Shader)}");
            }

            Gl.DetachShader(Shader, vertexShader);
            Gl.DetachShader(Shader, fragmentShader);
            Gl.DeleteShader(vertexShader);
            Gl.DeleteShader(fragmentShader);

            const uint positionLoc = 0;
            Gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)0);
            Gl.EnableVertexAttribArray(0);

            const uint texCoordLoc = 1;
            Gl.EnableVertexAttribArray(texCoordLoc);
            Gl.VertexAttribPointer(texCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));

            _texture = Gl.GenTexture();
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2D, _texture);
            ImageResult result = ImageResult.FromMemory(File.ReadAllBytes("images/silk.png"), ColorComponents.RedGreenBlueAlpha);
            fixed (byte* ptr = result.Data)
            Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)result.Width,
            (uint)result.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);

            Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.Repeat);
            Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.Repeat);
            Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Nearest);
            Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Nearest);
            Gl.BindTexture(TextureTarget.Texture2D, 0);
            int location = Gl.GetUniformLocation(Shader, "uTexture");
            Gl.Uniform1(location, 0);

            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        private static unsafe void OnRender(double obj)
        {
            Gl.Clear((uint) ClearBufferMask.ColorBufferBit);

            Gl.BindVertexArray(Vao);
            Gl.UseProgram(Shader);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2D, _texture);

            Gl.DrawElements(PrimitiveType.Triangles, (uint) Indices.Length, DrawElementsType.UnsignedInt, null);
        }

        private static void OnUpdate(double obj)
        {

        }

        private static void OnFramebufferResize(Vector2D<int> newSize)
        {
            Gl.Viewport(newSize);
        }

        private static void OnClose()
        {
            Gl.DeleteBuffer(Vbo);
            Gl.DeleteBuffer(Ebo);
            Gl.DeleteVertexArray(Vao);
            Gl.DeleteProgram(Shader);
        }

        private static void KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            if (arg2 == Key.Escape)
            {
                window.Close();
            }
        }
    }
}
