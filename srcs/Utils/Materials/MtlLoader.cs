using System.Numerics;
using System.Globalization;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace Scop
{
	public static class MtlLoader
	{
		private static unsafe uint LoadTexture(GL Gl, string path, uint Shader)
		{
			StbImage.stbi_set_flip_vertically_on_load(1);

			uint id = Gl.GenTexture();
			Gl.BindTexture(TextureTarget.Texture2D, id);

			ImageResult img = ImageResult.FromMemory(File.ReadAllBytes(path), ColorComponents.RedGreenBlueAlpha);
			fixed (byte* ptr = img.Data)
				Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)img.Width, (uint)img.Height,
							0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);

			Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.Repeat);
			Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.Repeat);
			Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Linear);
			Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Linear);
			return id;
		}

		public static Dictionary<string, Material> Load(string path, GL Gl, uint Shader)
		{
			var materials = new Dictionary<string, Material>();
			Material current = null;
			string baseDir = Path.GetDirectoryName(path);

			foreach (var line in File.ReadLines(path))
			{
				var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length == 0) continue;

				switch (parts[0])
				{
					case "newmtl":
						current = new Material { Name = parts[1] };
						materials[parts[1]] = current;
						break;

					case "Kd":
						current.Diffuse = new Vector3(
							float.Parse(parts[1], CultureInfo.InvariantCulture),
							float.Parse(parts[2], CultureInfo.InvariantCulture),
							float.Parse(parts[3], CultureInfo.InvariantCulture)
						);
						break;

					case "Ka":
						current.Ambient = new Vector3(
							float.Parse(parts[1], CultureInfo.InvariantCulture),
							float.Parse(parts[2], CultureInfo.InvariantCulture),
							float.Parse(parts[3], CultureInfo.InvariantCulture)
						);
						break;

					case "Ks":
						current.SpecularColor = new Vector3(
							float.Parse(parts[1], CultureInfo.InvariantCulture),
							float.Parse(parts[2], CultureInfo.InvariantCulture),
							float.Parse(parts[3], CultureInfo.InvariantCulture)
						);
						break;

					case "Ns":
						current.SpecularExp = float.Parse(parts[1], CultureInfo.InvariantCulture);
						break;
					case "map_Kd":
						string texPath = Path.Combine(baseDir, "Textures", Scop.OBJ_NAME, parts[1]);
						if (File.Exists(texPath))
							current.TextureId = LoadTexture(Gl, texPath, Shader);
						else
							Console.WriteLine($"Texture introuvable : {texPath}");
						break;
				}
			}
			return materials;
		}
	}
}
