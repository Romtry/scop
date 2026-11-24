using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;

namespace Scop
{
    public struct Vertex3D
    {
        public float X, Y, Z;
        public float NX, NY, NZ;
        public float U, V;
    }

    public struct FaceVertex
    {
        public int PosIndex;
        public int TexIndex;
        public int NormalIndex;
    }

    public struct MeshGroup
    {
        public string MaterialName;
        public int StartIndex;
        public int IndexCount;
    }

    public static class ObjParser
    {

        static (float, float, float) FaceNormal(
            (float, float, float) a, (float, float, float) b, (float, float, float) c)
        {
            var u = (b.Item1 - a.Item1, b.Item2 - a.Item2, b.Item3 - a.Item3);
            var v = (c.Item1 - a.Item1, c.Item2 - a.Item2, c.Item3 - a.Item3);
            var n = (u.Item2 * v.Item3 - u.Item3 * v.Item2,
                    u.Item3 * v.Item1 - u.Item1 * v.Item3,
                    u.Item1 * v.Item2 - u.Item2 * v.Item1);
            float len = MathF.Sqrt(n.Item1 * n.Item1 + n.Item2 * n.Item2 + n.Item3 * n.Item3);
            return len > 1e-8f ? (n.Item1 / len, n.Item2 / len, n.Item3 / len) : (0f, 0f, 1f);
        }

        static (float u, float v) BoxUV(
            (float x, float y, float z) p,
            (float x, float y, float z) n,
            float scale)
        {
            float ax = MathF.Abs(n.x), ay = MathF.Abs(n.y), az = MathF.Abs(n.z);

            if (ax >= ay && ax >= az) return (p.z / scale, p.y / scale);
            if (ay >= az)             return (p.x / scale, p.z / scale);
            return (p.x / scale, p.y / scale);
        }

        public static (Vertex3D[] vertices, uint[] indices, string mtlFile, List<(string, int)>) ParseOBJ(string filePath)
        {
            string mtlFile = null;
            var usemtl = new List<(string, int)>();

            var positions = new List<(float, float, float)>();
            var normals = new List<(float, float, float)>();
            var texCoords = new List<(float, float)>();
            var faceVertices = new List<FaceVertex>();

            try
            {
                foreach (string line in File.ReadLines(filePath))
                {
                    string trimmed = line.Trim();

                    if (trimmed.StartsWith("#") || string.IsNullOrEmpty(trimmed))
                        continue;

                    if (trimmed.StartsWith("v "))
                    {
                        var parts = trimmed.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 4)
                            throw new InvalidDataException($"'v' error : {trimmed}");
                        float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                        float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                        positions.Add((x, y, z));
                    }
                    else if (trimmed.StartsWith("vn "))
                    {
                        var parts = trimmed.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 4)
                            throw new InvalidDataException($"'vn' error : {trimmed}"); 
                        float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                        float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                        normals.Add((x, y, z));
                    }
                    else if (trimmed.StartsWith("vt "))
                    {
                        var parts = trimmed.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 3)
                            throw new InvalidDataException($"'vt' error : {trimmed}");
                        float u = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        float v = float.Parse(parts[2], CultureInfo.InvariantCulture);
                        texCoords.Add((u, v));
                    }
                    else if (trimmed.StartsWith("f "))
                    {
                        var parts = trimmed.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 4)
                            throw new InvalidDataException($"less than 3 Vertex : {trimmed}");

                        for (int i = 2; i < parts.Length - 1; i++)
                        {
                            ParseFaceVertex(parts[1],     positions, normals, texCoords, faceVertices);
                            ParseFaceVertex(parts[i],     positions, normals, texCoords, faceVertices);
                            ParseFaceVertex(parts[i + 1], positions, normals, texCoords, faceVertices);
                        }
                    }
                    else if (trimmed.StartsWith("mtllib "))
                    {
                        var parts = trimmed.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 2)
                            throw new InvalidDataException($"no mtllib : {trimmed}");
                        mtlFile = parts[1];
                    }
                    else if (trimmed.StartsWith("usemtl "))
                    {
                        var parts = trimmed.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 2)
                            throw new InvalidDataException($"no usemtl : {trimmed}");
                        usemtl.Add ((parts[1], faceVertices.Count));
                    }
                }
                if (positions.Count == 0)
                    throw new InvalidDataException("no v Vertex");
                if (faceVertices.Count == 0)
                    throw new InvalidDataException("no f Vertex");
            }

            catch (InvalidDataException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return (null, null, null, null);
            }


            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;
            float minZ = float.MaxValue, maxZ = float.MinValue;
            foreach (var p in positions)
            {

                minX = Math.Min(minX, p.Item1); maxX = Math.Max(maxX, p.Item1);
                minY = Math.Min(minY, p.Item2); maxY = Math.Max(maxY, p.Item2);
                minZ = Math.Min(minZ, p.Item3); maxZ = Math.Max(maxZ, p.Item3);
            }
            float sizeX = (maxX - minX) == 0 ? 1 : (maxX - minX);
            float sizeY = (maxY - minY) == 0 ? 1 : (maxY - minY);
            float sizeZ = (maxZ - minZ) == 0 ? 1 : (maxZ - minZ);

            var vertices = new Vertex3D[faceVertices.Count];
            (float, float, float) faceN = (0f, 0f, 1f);
            Scop.center = (minX + sizeX / 2f, minY + sizeY / 2f, minZ + sizeZ / 2f);
            for (int i = 0; i < faceVertices.Count; i++)
            {
                // no stretching
                // if (i % 3 == 0 && i + 2 < faceVertices.Count)
                // {
                //     var pa = positions[faceVertices[i].PosIndex];
                //     var pb = positions[faceVertices[i + 1].PosIndex];
                //     var pc = positions[faceVertices[i + 2].PosIndex];
                //     faceN = FaceNormal(pa, pb, pc);

                //     var toFace = (pa.Item1 - center.Item1, pa.Item2 - center.Item2, pa.Item3 - center.Item3);
                //     float dot = faceN.Item1 * toFace.Item1 + faceN.Item2 * toFace.Item2 + faceN.Item3 * toFace.Item3;
                //     if (dot < 0f) faceN = (-faceN.Item1, -faceN.Item2, -faceN.Item3);
                // }

                var fv = faceVertices[i];
                var pos = positions[fv.PosIndex];
                var normal = fv.NormalIndex >= 0 ? normals[fv.NormalIndex] : (0, 0, 1);

                (float, float) tex;
                if (fv.TexIndex >= 0 && fv.TexIndex < texCoords.Count)
                    tex = texCoords[fv.TexIndex];
                // else
                // {
                //     float ax = Math.Abs(faceN.Item1);
                //     float ay = Math.Abs(faceN.Item2);
                //     float az = Math.Abs(faceN.Item3);

                //     if (ax >= ay && ax >= az)
                //         tex = ((pos.Item3 - minZ) / sizeZ, (pos.Item2 - minY) / sizeY);
                //     else if (ay >= ax && ay >= az)
                //         tex = ((pos.Item1 - minX) / sizeX, (pos.Item3 - minZ) / sizeZ);
                //     else
                //         tex = ((pos.Item1 - minX) / sizeX, (pos.Item2 - minY) / sizeY);
                // }
                else
                    tex = ((pos.Item3 - minZ) / sizeZ, (pos.Item2 - minY) / sizeY);

                vertices[i] = new Vertex3D
                {
                    X = pos.Item1,
                    Y = pos.Item2,
                    Z = pos.Item3,
                    NX = normal.Item1,
                    NY = normal.Item2,
                    NZ = normal.Item3,
                    U = tex.Item1,
                    V = tex.Item2
                };
            }

            var indices = Enumerable.Range(0, vertices.Length).Select(i => (uint)i).ToArray();

            return (vertices, indices, mtlFile, usemtl);
        }

        private static void ParseFaceVertex(string vertex,
            List<(float, float, float)> positions,
            List<(float, float, float)> normals,
            List<(float, float)> texCoords,
            List<FaceVertex> faceVertices)
        {
            var indices = vertex.Split('/');

            int posIndex = int.Parse(indices[0]);
            if (posIndex < 0)
                posIndex = positions.Count + posIndex;
            else
                posIndex = posIndex - 1;
            int texIndex = indices.Length > 1 && !string.IsNullOrEmpty(indices[1]) 
                ? int.Parse(indices[1]) - 1
                : -1;
            int normalIndex = indices.Length > 2 && !string.IsNullOrEmpty(indices[2]) 
                ? int.Parse(indices[2]) - 1
                : -1;

            faceVertices.Add(new FaceVertex
            {
                PosIndex = posIndex,
                TexIndex = texIndex,
                NormalIndex = normalIndex
            });
        }
    }
}
