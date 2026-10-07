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
        public static  string   OBJ_NAME;
        private static  IWindow  window;
        public  static  GL       Gl;
        private static  bool     is3D;


        private static uint Vbo;
        private static uint Ebo;
        private static uint Vao;
        private static uint Shader;
        private static uint _texture;
        public static int _modelLoc;
        public static int _viewLoc;
        public static int _projLoc;
        public static int _camModeLoc;
        public static int _timeLoc;
        public static int _kdLoc;
        public static int _kaLoc;
        public static int _ksLoc;
        public static int _nsLoc;
        public static int _lightPosLoc;
        public static int _viewPosLoc;
        public static int _lightIntensity;

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

            if (extension != null && IMAGE_PATH.Contains('/'))
                OBJ_NAME = IMAGE_PATH.Substring(IMAGE_PATH.LastIndexOf('/') + 1, IMAGE_PATH.Length - IMAGE_PATH.LastIndexOf('/') - extension.Length - 1);

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
