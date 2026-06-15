using Silk.NET.Input;
using Silk.NET.Windowing;
using System;
using Silk.NET.Maths;
using System.Numerics;


namespace Scop
{
    public static class InputUtils
    {
        public static float CamX = 0f;
        public static float CamY = 0f;
        public static float CamZ = 3f;

        public static bool MoveForward  = false;
        public static bool MoveBack     = false;
        public static bool MoveLeft     = false;
        public static bool MoveRight    = false;
        public static bool MoveUp       = false;
        public static bool MoveDown     = false;

        public static void SetupInput(IWindow window, Action<IWindow, Key, int> onKeyDown)
        {
            IInputContext input = window.CreateInput();

            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += (keyboard, key, keyCode) => onKeyDown(window, key, keyCode);
                input.Keyboards[i].KeyUp   += (keyboard, key, keyCode) => KeyUp(key);
            }

            for (int i = 0; i < input.Mice.Count; i++)
            {
                input.Mice[i].MouseMove += MouseMove;
                input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
            }
        }

        public static void KeyDown(IWindow window, Key key, int arg3)
        {
            float speed = 0.1f;
            if (key == Key.Escape)                  window.Close();
            if (key == Key.Left  || key == Key.A)   MoveLeft    = true;
            if (key == Key.Right || key == Key.D)   MoveRight   = true;
            if (key == Key.Up    || key == Key.W)   MoveForward = true;
            if (key == Key.Down  || key == Key.S)   MoveBack    = true;
            if (key == Key.Space)                   MoveUp      = true;
            if (key == Key.ControlLeft)             MoveDown    = true;
        }

        public static void KeyUp(Key key)
        {
            if (key == Key.Left  || key == Key.A)   MoveLeft    = false;
            if (key == Key.Right || key == Key.D)   MoveRight   = false;
            if (key == Key.Up    || key == Key.W)   MoveForward = false;
            if (key == Key.Down  || key == Key.S)   MoveBack    = false;
            if (key == Key.Space)                   MoveUp      = false;
            if (key == Key.ControlLeft)             MoveDown    = false;
        }

        public static float Yaw   = -90f;
        public static float Pitch = 0f;
        private static Vector2 _lastMousePos;
        private static bool _firstMouse = true;

        public static void MouseMove(IMouse mouse, Vector2 pos)
        {
            if (_firstMouse)
            {
                _lastMousePos = pos;
                _firstMouse = false;
                return;
            }

            float sensitivity = 0.1f;
            float dx = (pos.X - _lastMousePos.X) * sensitivity;
            float dy = (_lastMousePos.Y - pos.Y) * sensitivity;

            Yaw   += dx;
            Pitch += dy;

            if (Pitch >  89f) Pitch =  89f;
            if (Pitch < -89f) Pitch = -89f;

            _lastMousePos = pos;
        }

        public static Vector3D<float> CamPos = new(0, 0, 3f);
        public static Vector3D<float> CamFront = new(0, 0, -1f);

        public static void UpdateCamera(double deltaTime)
        {
            float speed = 2f * (float)deltaTime;

            float yawRad   = MathF.PI / 180f * Yaw;
            float pitchRad = MathF.PI / 180f * Pitch;

            CamFront = Vector3D.Normalize(new Vector3D<float>(
                MathF.Cos(yawRad) * MathF.Cos(pitchRad),
                MathF.Sin(pitchRad),
                MathF.Sin(yawRad) * MathF.Cos(pitchRad)
            ));

            Vector3D<float> up    = new(0, 1, 0);
            Vector3D<float> right = Vector3D.Normalize(Vector3D.Cross(CamFront, up));

            if (MoveForward) CamPos += CamFront * speed;
            if (MoveBack)    CamPos -= CamFront * speed;
            if (MoveRight)   CamPos += right    * speed;
            if (MoveLeft)    CamPos -= right    * speed;
            if (MoveUp)      CamPos += up       * speed;
            if (MoveDown)    CamPos -= up       * speed;
        }
    }
}
