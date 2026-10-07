using System.Numerics;

namespace Scop
{
	public class Material
	{
		public string Name { get; set; }
		public uint TextureId { get; set; }
		public uint NsTextureId { get; set; }
		public Vector3 Diffuse { get; set; } = new Vector3(1, 1, 1);
		public Vector3 Ambient { get; set; } = new Vector3(1, 1, 1);
		public Vector3 SpecularColor { get; set; } = new Vector3(1, 1, 1);
		public float SpecularExp { get; set; } = 32f;
	}
}
