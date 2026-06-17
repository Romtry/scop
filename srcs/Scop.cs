using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using Silk.NET.Maths;
using StbImageSharp;
using System.IO;

namespace Scop
{
    partial class Scop
    {
        private static  string   IMAGE_PATH;
        private static  IWindow  window;
        public  static  GL       Gl;
        private static  bool     is3D;


        private static uint Vbo;
        private static uint Ebo;
        private static uint Vao;
        private static uint Shader;
        private static uint _texture;

        private static Matrix4X4<float> _projection;
        private static Matrix4X4<float> _view;
        private static Matrix4X4<float> _model;



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
                    is3D = false;
                    break;

                case ".obj":
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

        private static void OnUpdate(double obj)
        {
            InputUtils.UpdateCamera(obj);
        }

        private static void OnFramebufferResize(Vector2D<int> newSize)
        {
            Gl.Viewport(newSize);
        }

    }
}
