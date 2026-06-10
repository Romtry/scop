using Silk.NET.OpenGL;
using System;

namespace Scop
{
    public static class ShaderUtils
    {
        public static uint CompileShader(GL gl, ShaderType type, string source)
        {
            uint shader = gl.CreateShader(type);
            gl.ShaderSource(shader, source);
            gl.CompileShader(shader);

            string infoLog = gl.GetShaderInfoLog(shader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Console.WriteLine($"Error compiling {type} shader: {infoLog}");
            }
            return shader;
        }

        public static uint LinkProgram(GL gl, uint vertexShader, uint fragmentShader)
        {
            uint program = gl.CreateProgram();
            gl.AttachShader(program, vertexShader);
            gl.AttachShader(program, fragmentShader);
            gl.LinkProgram(program);

            gl.GetProgram(program, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader: {gl.GetProgramInfoLog(program)}");
            }

            gl.DetachShader(program, vertexShader);
            gl.DetachShader(program, fragmentShader);
            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);

            return program;
        }

        public static uint CreateShaderProgram(GL gl, string vertexSource, string fragmentSource)
        {
            uint vertexShader = CompileShader(gl, ShaderType.VertexShader, vertexSource);
            uint fragmentShader = CompileShader(gl, ShaderType.FragmentShader, fragmentSource);
            return LinkProgram(gl, vertexShader, fragmentShader);
        }
    }
}
