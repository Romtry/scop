using Silk.NET.OpenGL;
using System;

namespace Scop
{
    public static class ShaderUtils
    {
        public static uint CreateShaderProgram(GL Gl, string vertexSource, string fragmentSource)
        {
            uint vertexShader = CompileShader(Gl, ShaderType.VertexShader, vertexSource);
            uint fragmentShader = CompileShader(Gl, ShaderType.FragmentShader, fragmentSource);
            return LinkProgram(Gl, vertexShader, fragmentShader);
        }

        public static void CacheUniformLocations(GL Gl, uint Shader)
        {
            Scop._modelLoc          = Gl.GetUniformLocation(Shader, "uModel");
            Scop._viewLoc           = Gl.GetUniformLocation(Shader, "uView");
            Scop._projLoc           = Gl.GetUniformLocation(Shader, "uProjection");
            Scop._camModeLoc        = Gl.GetUniformLocation(Shader, "uCamMode");
            Scop._timeLoc           = Gl.GetUniformLocation(Shader, "uTime");
            Scop._kdLoc             = Gl.GetUniformLocation(Shader, "uKd");
            Scop._kaLoc             = Gl.GetUniformLocation(Shader, "uKa");
            Scop._ksLoc             = Gl.GetUniformLocation(Shader, "uKs");
            Scop._nsLoc             = Gl.GetUniformLocation(Shader, "uNs");
            Scop._lightPosLoc       = Gl.GetUniformLocation(Shader, "uLightPos");
			Scop._viewPosLoc        = Gl.GetUniformLocation(Shader, "uViewPos");
			Scop._lightIntensity    = Gl.GetUniformLocation(Shader, "uLightIntensity");
            Scop._hasTextures       = Gl.GetUniformLocation(Shader, "uHasTexture");
            Scop._hasNsTextures     = Gl.GetUniformLocation(Shader, "uHasNsTexture");
            Scop._hasReflTextures   = Gl.GetUniformLocation(Shader, "uHasReflTexture");
        }

        public static uint CompileShader(GL Gl, ShaderType type, string source)
        {
            uint shader = Gl.CreateShader(type);
            Gl.ShaderSource(shader, source);
            Gl.CompileShader(shader);

            string infoLog = Gl.GetShaderInfoLog(shader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Console.WriteLine($"Error compiling {type} shader: {infoLog}");
            }
            return shader;
        }

        public static uint LinkProgram(GL Gl, uint vertexShader, uint fragmentShader)
        {
            uint program = Gl.CreateProgram();
            Gl.AttachShader(program, vertexShader);
            Gl.AttachShader(program, fragmentShader);
            Gl.LinkProgram(program);

            Gl.GetProgram(program, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader: {Gl.GetProgramInfoLog(program)}");
            }

            Gl.DetachShader(program, vertexShader);
            Gl.DetachShader(program, fragmentShader);
            Gl.DeleteShader(vertexShader);
            Gl.DeleteShader(fragmentShader);

            return program;
        }
    }
}
