using System;
using System.IO;
using System.IO.Compression;

public static class PngLoader
{
    public static (byte[] data, int width, int height) Load(string path, bool flipVertically = true)
    {
        byte[] file = File.ReadAllBytes(path);

        byte[] sig = { 137, 80, 78, 71, 13, 10, 26, 10 };
        for (int i = 0; i < 8; i++)
            if (file[i] != sig[i])
                throw new InvalidDataException($"{path} n'est pas un fichier PNG");

        int width = 0, height = 0, bitDepth = 0, colorType = 0, interlace = 0;
        byte[]? palette = null;
        byte[]? paletteAlpha = null;
        using var idat = new MemoryStream();

        int pos = 8;
        while (pos < file.Length)
        {
            int length = ReadInt(file, pos);
            string type = System.Text.Encoding.ASCII.GetString(file, pos + 4, 4);
            int dataStart = pos + 8;

            switch (type)
            {
                case "IHDR":
                    width     = ReadInt(file, dataStart);
                    height    = ReadInt(file, dataStart + 4);
                    bitDepth  = file[dataStart + 8];
                    colorType = file[dataStart + 9];
                    interlace = file[dataStart + 12];
                    break;
                case "PLTE":
                    palette = file[dataStart..(dataStart + length)];
                    break;
                case "tRNS":
                    paletteAlpha = file[dataStart..(dataStart + length)];
                    break;
                case "IDAT":
                    idat.Write(file, dataStart, length);
                    break;
            }
            if (type == "IEND") break;
            pos += 12 + length;
        }

        if (bitDepth != 8)
            throw new NotSupportedException($"{path} : seule la profondeur 8 bits est supportée");
        if (interlace != 0)
            throw new NotSupportedException($"{path} : PNG entrelacé non supporté");

        int bpp = colorType switch
        {
            0 => 1,
            2 => 3,
            3 => 1,
            4 => 2,
            6 => 4,
            _ => throw new NotSupportedException($"colorType {colorType} non supporté")
        };

        idat.Position = 0;
        using var z = new ZLibStream(idat, CompressionMode.Decompress);
        using var raw = new MemoryStream();
        z.CopyTo(raw);
        byte[] src = raw.ToArray();

        int stride = width * bpp;
        byte[] pixels = new byte[stride * height];
        byte[] prev = new byte[stride];
        int s = 0;

        for (int y = 0; y < height; y++)
        {
            byte filter = src[s++];
            int rowStart = y * stride;

            for (int x = 0; x < stride; x++)
            {
                byte cur  = src[s++];
                byte a    = x >= bpp ? pixels[rowStart + x - bpp] : (byte)0;
                byte b    = prev[x];
                byte c    = x >= bpp ? prev[x - bpp] : (byte)0;

                pixels[rowStart + x] = filter switch
                {
                    0 => cur,
                    1 => (byte)(cur + a),
                    2 => (byte)(cur + b),
                    3 => (byte)(cur + ((a + b) >> 1)),
                    4 => (byte)(cur + Paeth(a, b, c)),
                    _ => throw new InvalidDataException($"Filtre PNG inconnu : {filter}")
                };
            }
            Array.Copy(pixels, rowStart, prev, 0, stride);
        }

        byte[] rgba = new byte[width * height * 4];
        for (int y = 0; y < height; y++)
        {
            int dstY = flipVertically ? height - 1 - y : y;
            for (int x = 0; x < width; x++)
            {
                int i = y * stride + x * bpp;
                int o = (dstY * width + x) * 4;

                switch (colorType)
                {
                    case 0:
                        rgba[o] = rgba[o + 1] = rgba[o + 2] = pixels[i];
                        rgba[o + 3] = 255;
                        break;
                    case 2:
                        rgba[o] = pixels[i]; rgba[o + 1] = pixels[i + 1]; rgba[o + 2] = pixels[i + 2];
                        rgba[o + 3] = 255;
                        break;
                    case 3:
                        int idx = pixels[i];
                        rgba[o]     = palette![idx * 3];
                        rgba[o + 1] = palette[idx * 3 + 1];
                        rgba[o + 2] = palette[idx * 3 + 2];
                        rgba[o + 3] = paletteAlpha != null && idx < paletteAlpha.Length ? paletteAlpha[idx] : (byte)255;
                        break;
                    case 4:
                        rgba[o] = rgba[o + 1] = rgba[o + 2] = pixels[i];
                        rgba[o + 3] = pixels[i + 1];
                        break;
                    case 6:
                        rgba[o] = pixels[i]; rgba[o + 1] = pixels[i + 1];
                        rgba[o + 2] = pixels[i + 2]; rgba[o + 3] = pixels[i + 3];
                        break;
                }
            }
        }

        return (rgba, width, height);
    }

    private static int ReadInt(byte[] b, int p) =>
        (b[p] << 24) | (b[p + 1] << 16) | (b[p + 2] << 8) | b[p + 3];

    private static byte Paeth(byte a, byte b, byte c)
    {
        int p = a + b - c;
        int pa = Math.Abs(p - a), pb = Math.Abs(p - b), pc = Math.Abs(p - c);
        if (pa <= pb && pa <= pc) return a;
        return pb <= pc ? b : c;
    }
}
