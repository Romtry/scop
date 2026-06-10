using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using Silk.NET.Maths;
using StbImageSharp;
using System.IO;

namespace Scop
{
    partial class Program
    {
        private static  string   IMAGE_PATH;
        private static  IWindow  window;
        private static  GL       Gl;
        private static  bool     is3D;


        private static uint Vbo;
        private static uint Ebo;
        private static uint Vao;
        private static uint Shader;
        private static uint _texture;



        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: programme <path>");
                return;
            }

            IMAGE_PATH = args[0];

            if (!File.Exists(IMAGE_PATH))
            {
                Console.WriteLine($"Fichier introuvable : {IMAGE_PATH}");
                return;
            }

            string extension = Path.GetExtension(IMAGE_PATH);

            switch (extension)
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                    is3D = false;
                    break;

                case ".obj":
                case ".fbx":
                case ".gltf":
                    is3D = true;
                    break;

                default:
                    Console.WriteLine($"Format non supporté : {extension}");
                    return;
            }

            InitWindow();

            window.Run();

            window.Dispose();
        }

        private static unsafe void OnRender(double obj)
        {
            Gl.Clear((uint) ClearBufferMask.ColorBufferBit);

            Gl.BindVertexArray(Vao);
            Gl.UseProgram(Shader);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2D, _texture);

            Gl.DrawElements(PrimitiveType.Triangles, (uint) Indices2D.Length, DrawElementsType.UnsignedInt, null);
        }

        private static void OnUpdate(double obj)
        {

        }

        private static void OnFramebufferResize(Vector2D<int> newSize)
        {
            Gl.Viewport(newSize);
        }

        private static void OnClose()
        {
            Gl.DeleteBuffer(Vbo);
            Gl.DeleteBuffer(Ebo);
            Gl.DeleteVertexArray(Vao);
            Gl.DeleteProgram(Shader);
        }

        private static void KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            if (arg2 == Key.Escape)
            {
                window.Close();
            }
        }
    }
}
