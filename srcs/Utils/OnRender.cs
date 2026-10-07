using Silk.NET.OpenGL;
using Silk.NET.Maths;
using System.Numerics;

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
			Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture3D, _texture);

			var model = Matrix4X4<float>.Identity;

			var camPos = new Vector3D<float>(InputUtils.CamX, InputUtils.CamY, InputUtils.CamZ);

			var view = Matrix4X4.CreateLookAt(
				InputUtils.CamPos,
				InputUtils.CamPos + InputUtils.CamFront,
				new Vector3D<float>(0f, 1f, 0f)
			);

			var proj = Matrix4X4.CreatePerspectiveFieldOfView(
				Config.Fov * MathF.PI / 180f,
				800f / 600f,
				0.1f,
				10000f
			);

			Gl.Uniform1(_camModeLoc, InputUtils.CamMode % 3);
			Gl.Uniform1(_timeLoc, (float)_time);

			Gl.Uniform1(_lightIntensity, (float)(((InputUtils.LightLvl % 5) / 4.0f) * 2.0f));
			Vector3 lightPosition = new Vector3(
				5.0f,
				3.0f,
				5.0f
			);
			Gl.Uniform3(_lightPosLoc, lightPosition);
			Gl.Uniform3(_viewPosLoc, InputUtils.CamX, InputUtils.CamY, InputUtils.CamZ);

			Gl.UniformMatrix4(_modelLoc, 1, false, (float*)&model);
			Gl.UniformMatrix4(_viewLoc,  1, false, (float*)&view);
			Gl.UniformMatrix4(_projLoc,  1, false, (float*)&proj);

			if (usemtl.Count == 0)
			{
				Vector3 kd = new Vector3(1.0f, 1.0f, 1.0f);
				Gl.Uniform3(_kdLoc, ref kd);
				Vector3 ks = new Vector3(0f, 0f, 0f);
				Gl.Uniform3(_ksLoc, ref ks);
				Gl.Uniform1(_nsLoc, 6f);
				Vector3 ka = new Vector3(0.2f, 0.2f, 0.2f);
				Gl.Uniform3(_kaLoc, ref ka);
				if (InputUtils.CamMode % 3 == 2)
				{
					Gl.Uniform1(_camModeLoc, 2);
					Gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
					Gl.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, null);

					Gl.Uniform1(_camModeLoc, 0);
					Gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
				}
				Gl.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, null);
			}

			for (int i = 0; i < usemtl.Count; i++)
			{
				var kd = Materials[usemtl[i].Item1].Diffuse;
				Gl.Uniform3(_kdLoc, ref kd);
				var ks = Materials[usemtl[i].Item1].SpecularColor;
				Gl.Uniform3(_ksLoc, ref ks);
				Gl.Uniform1(_nsLoc, Materials[usemtl[i].Item1].SpecularExp);
				Gl.Uniform3(_kaLoc, Materials[usemtl[i].Item1].Ambient);

				int startIndex = usemtl[i].Item2;
				int endIndex   = (i + 1 < usemtl.Count) ? usemtl[i + 1].Item2 : (int)_indexCount;
				uint count     = (uint)(endIndex - startIndex);
				if (count == 0) continue;

				if (InputUtils.CamMode % 3 == 2)
				{
					Gl.Uniform1(_camModeLoc, 2);
					Gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
					Gl.DrawElements(PrimitiveType.Triangles, count, DrawElementsType.UnsignedInt, (void*)(startIndex * sizeof(uint)));

					Gl.Uniform1(_camModeLoc, 0);
					Gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
				}
				if (Materials[usemtl[i].Item1].TextureId != null)
				{
					Gl.Uniform1(Scop._hasTextures, 1);
					Gl.ActiveTexture(TextureUnit.Texture0);
					Gl.BindTexture(TextureTarget.Texture2D, Materials[usemtl[i].Item1].TextureId);
					Gl.DrawElements(PrimitiveType.Triangles, count, DrawElementsType.UnsignedInt, (void*)(startIndex * sizeof(uint)));
				}
				if (Materials[usemtl[i].Item1].NsTextureId != null)
				{
					Gl.Uniform1(Scop._hasNsTextures, 1);
					Gl.ActiveTexture(TextureUnit.Texture1);
					Gl.BindTexture(TextureTarget.Texture2D, Materials[usemtl[i].Item1].NsTextureId);
					Gl.DrawElements(PrimitiveType.Triangles, count, DrawElementsType.UnsignedInt, (void*)(startIndex * sizeof(uint)));
				}
				if (Materials[usemtl[i].Item1].reflTextureId != null)
				{
					Gl.Uniform1(Scop._hasReflTextures, 1);
					Gl.ActiveTexture(TextureUnit.Texture2);
					Gl.BindTexture(TextureTarget.Texture2D, Materials[usemtl[i].Item1].reflTextureId);
					Gl.DrawElements(PrimitiveType.Triangles, count, DrawElementsType.UnsignedInt, (void*)(startIndex * sizeof(uint)));
				}
			}
		}
	}
}
