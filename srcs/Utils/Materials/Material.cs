using System.Numerics;

namespace Scop
{
	public class Material
	{
		public string Name { get; set; }
		public Vector3 Diffuse { get; set; } = new Vector3(1, 1, 1);
	}
}
