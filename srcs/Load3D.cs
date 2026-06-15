using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Maths;
using System;

namespace Scop
{
    partial class Scop
    {
        private static uint _indexCount;

        private static unsafe void Load3D()
        {
            // Input
            InputUtils.SetupInput(window, InputUtils.KeyDown);

            // GL Init
            Gl = GL.GetApi(window);
            Gl.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);

			// Depth
            Gl.Enable(EnableCap.DepthTest);
            Gl.DepthFunc(DepthFunction.Less);


            // Parse OBJ
            var (vertices3D, indices3D) = ObjParser.ParseOBJ(IMAGE_PATH);
            _indexCount = (uint)indices3D.Length;

            // Buffers
            (Vao, Vbo, Ebo) = BufferUtils.CreateBuffers(Gl, vertices3D, indices3D);

            // Shader
            Shader = ShaderUtils.CreateShaderProgram(Gl, VertexShaderSource3D, FragmentShaderSource3D);

            // Matrices
            float aspectRatio = (float)window.Size.X / window.Size.Y;
            _projection = MatrixUtils.CreatePerspectiveProjection(
                MathF.PI / 4.0f,
                aspectRatio,
                0.1f,
                100.0f
            );

            _view = MatrixUtils.CreateLookAt(
                InputUtils.CamPos,
                InputUtils.CamPos + InputUtils.CamFront,
                new Vector3D<float>(0, 1, 0)
            );

            _model = MatrixUtils.CreateIdentity();
        }
    }
}
