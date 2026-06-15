using Silk.NET.OpenGL;
using Silk.NET.Maths;

namespace Scop
{
    partial class Scop
    {
        private static unsafe void OnRender2D(double obj)
        {
            Gl.Clear((uint) ClearBufferMask.ColorBufferBit);

            Gl.BindVertexArray(Vao);
            Gl.UseProgram(Shader);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2D, _texture);

            Gl.DrawElements(PrimitiveType.Triangles, (uint) Indices2D.Length, DrawElementsType.UnsignedInt, null);
        }

        private static unsafe void OnRender3D(double obj)
        {
			Gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
			Gl.BindVertexArray(Vao);
			Gl.UseProgram(Shader);

			var model = Matrix4X4<float>.Identity;

			var camPos = new Vector3D<float>(InputUtils.CamX, InputUtils.CamY, InputUtils.CamZ);

			var view = Matrix4X4.CreateLookAt(
				InputUtils.CamPos,
				InputUtils.CamPos + InputUtils.CamFront,
				new Vector3D<float>(0f, 1f, 0f)
			);

			var proj = Matrix4X4.CreatePerspectiveFieldOfView(
				MathF.PI / 4f,
				800f / 600f,
				0.1f, 100f
			);

			int modelLoc = Gl.GetUniformLocation(Shader, "uModel");
			int viewLoc  = Gl.GetUniformLocation(Shader, "uView");
			int projLoc  = Gl.GetUniformLocation(Shader, "uProjection");

			Gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);
			Gl.UniformMatrix4(viewLoc,  1, false, (float*)&view);
			Gl.UniformMatrix4(projLoc,  1, false, (float*)&proj);

			Gl.ActiveTexture(TextureUnit.Texture0);
			Gl.BindTexture(TextureTarget.Texture2D, _texture);

			Gl.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, null);
        }
	}
}
