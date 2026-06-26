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

		private static double _time = 0;

        private static unsafe void OnRender3D(double obj)
        {
			_time += obj;

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
				0.1f, 10000f
			);

			int modelLoc = Gl.GetUniformLocation(Shader, "uModel");
			int viewLoc  = Gl.GetUniformLocation(Shader, "uView");
			int projLoc  = Gl.GetUniformLocation(Shader, "uProjection");
			int CamMode	 = Gl.GetUniformLocation(Shader, "uCamMode");
			int timeLoc  = Gl.GetUniformLocation(Shader, "uTime");
			int kdLocation = Gl.GetUniformLocation(Shader, "uKd");

			Gl.Uniform1(CamMode, InputUtils.CamMode % 3);
			Gl.Uniform1(timeLoc, (float)_time);

			Gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);
			Gl.UniformMatrix4(viewLoc,  1, false, (float*)&view);
			Gl.UniformMatrix4(projLoc,  1, false, (float*)&proj);

			Gl.ActiveTexture(TextureUnit.Texture0);
			Gl.BindTexture(TextureTarget.Texture2D, _texture);

			if (usemtl.Count == 0)
			{
				if (InputUtils.CamMode % 3 == 2)
				{
					Gl.Uniform1(CamMode, 0);
					Gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
					Gl.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, null);

					Gl.Uniform1(CamMode, 2);
					Gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
				}
                // Console.WriteLine($"count : {count}\nstartIndex : {startIndex}\n");
				Gl.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, null);
			}

			for (int i = 0; i < usemtl.Count; i++)
			{
				var kd = Materials[usemtl[i].Item1].Diffuse;
				Gl.Uniform3(kdLocation, ref kd);

				int startIndex = 0;
				if (i != 0)
					startIndex = usemtl[i - 1].Item2;

				uint count = 0;
				if (i == usemtl.Count - 1)
					count = (uint)(_indexCount - startIndex);
				else
					count = (uint)(usemtl[i].Item2 - startIndex);

				if (InputUtils.CamMode % 3 == 2)
				{
					Gl.Uniform1(CamMode, 0);
					Gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
					Gl.DrawElements(PrimitiveType.Triangles, count, DrawElementsType.UnsignedInt, (void*)(startIndex * sizeof(uint)));

					Gl.Uniform1(CamMode, 2);
					Gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
				}
				// Console.WriteLine($"count : {count}\nstartIndex : {startIndex}");
                // Console.WriteLine($"count : {count}\nstartIndex : {startIndex}\n");
				Gl.DrawElements(PrimitiveType.Triangles, count, DrawElementsType.UnsignedInt, (void*)(startIndex * sizeof(uint)));
			}
		}
	}
}
