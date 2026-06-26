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
        public static (Vertex3D[] vertices, uint[] indices, string mtlFile, List<(string, int)>) ParseOBJ(string filePath)
        {
            string mtlFile = null;
            var usemtl = new List<(string, int)>();

            // string currentMaterial = null;
            // int currentStart = 0;
            // var groups = new List<MeshGroup>();

            var positions = new List<(float, float, float)>();
            var normals = new List<(float, float, float)>();
            var texCoords = new List<(float, float)>();
            var faceVertices = new List<FaceVertex>();


            foreach (string line in File.ReadLines(filePath))
            {
                string trimmed = line.Trim();

                if (trimmed.StartsWith("#") || string.IsNullOrEmpty(trimmed))
                    continue;

                if (trimmed.StartsWith("v "))
                {
                    var parts = trimmed.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                    float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                    float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                    float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                    positions.Add((x, y, z));
                }
                else if (trimmed.StartsWith("vn "))
                {
                    var parts = trimmed.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                    float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                    float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                    float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                    normals.Add((x, y, z));
                }
                else if (trimmed.StartsWith("vt "))
                {
                    var parts = trimmed.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                    float u = float.Parse(parts[1], CultureInfo.InvariantCulture);
                    float v = float.Parse(parts[2], CultureInfo.InvariantCulture);
                    texCoords.Add((u, v));
                }
                else if (trimmed.StartsWith("f "))
                {
                    var parts = trimmed.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

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
                    mtlFile = parts[1];
                }
                else if (trimmed.StartsWith("usemtl "))
                {
                    var parts = trimmed.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                    usemtl.Add ((parts[1], faceVertices.Count));
                }
            }


            var vertices = new Vertex3D[faceVertices.Count];
            for (int i = 0; i < faceVertices.Count; i++)
            {
                // Console.WriteLine($"faceVertices.Count : {faceVertices.Count}\ni : {i}\nfv : {faceVertices[i].PosIndex}");
                var fv = faceVertices[i];
                // Console.WriteLine($"positions.Count : {positions.Count}\nfv.PosIndex : {fv.PosIndex}");
                var pos = positions[fv.PosIndex];
                var normal = fv.NormalIndex >= 0 ? normals[fv.NormalIndex] : (0, 0, 1);
                var tex = fv.TexIndex >= 0 ? texCoords[fv.TexIndex] : (0, 0);

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
