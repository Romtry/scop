using System.Numerics;
using System.Globalization;

namespace Scop
{
	public static class MtlLoader
	{
		public static Dictionary<string, Material> Load(string path)
		{
			var materials = new Dictionary<string, Material>();
			Material current = null;

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
				}
			}
			return materials;
		}
	}
}
