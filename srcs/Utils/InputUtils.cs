using Silk.NET.Input;
using Silk.NET.Windowing;
using System;
using Silk.NET.Maths;
using System.Numerics;
using Silk.NET.OpenGL;
using System.Diagnostics;


namespace Scop
{
    public static class InputUtils
    {
        public static readonly Stopwatch _clock = Stopwatch.StartNew();

        public static double _transitionStart = 0;
        public static float _transitionSave = 0;

        public static float CamX  = 0f;
        public static float CamY  = 0f;
        public static float CamZ  = 3f;

        public static int   CamMode = 0;
        public static int   Sprint  = 1;

        public static bool TranslateMode = false;
        public static bool MoveForward  = false;
        public static bool MoveBack     = false;
        public static bool MoveLeft     = false;
        public static bool MoveRight    = false;
        public static bool MoveUp       = false;
        public static bool MoveDown     = false;
        public static bool RotateLeft     = false;
        public static bool RotateRight    = false;
        public static bool RotateUp       = false;
        public static bool RotateDown     = false;

        public static int LightLvl = 3;

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
            if (key == Key.E)
            {
                ++CamMode;
                if (CamMode % 3 != 0)
                    _transitionStart = _clock.Elapsed.TotalSeconds;
                if (CamMode % 3 == 0)
                {
                    if (Scop._transitionRatio >= 1)
                        _transitionSave = 0;
                }
                else
                {
                    if (Scop._transitionRatio >= 1)
                        _transitionSave = 1f;
                    else
                        _transitionSave = Scop._transitionRatio;
                }
            }
            if (key == Key.Tab)
            {
                if (TranslateMode == true)
                    TranslateMode = false;
                else
                    TranslateMode = true;
            }
            if (key == Key.L)                       ++LightLvl;
            if (key == Key.Escape)                  window.Close();
            if (key == Key.Left)                    RotateLeft  = true;
            if (key == Key.Right)                   RotateRight = true;
            if (key == Key.Up)                      RotateUp    = true;
            if (key == Key.Down)                    RotateDown  = true;
            if (key == Key.A)                       MoveLeft    = true;
            if (key == Key.D)                       MoveRight   = true;
            if (key == Key.W)                       MoveForward = true;
            if (key == Key.S)                       MoveBack    = true;
            if (key == Key.Space)                   MoveUp      = true;
            if (key == Key.ControlLeft)             MoveDown    = true;
            if (key == Key.ShiftLeft)               Sprint      = Config.SprintMultiplier;
        }

        public static void KeyUp(Key key)
        {
            if (key == Key.Left)                    RotateLeft  = false;
            if (key == Key.Right)                   RotateRight = false;
            if (key == Key.Up)                      RotateUp    = false;
            if (key == Key.Down)                    RotateDown  = false;
            if (key == Key.A)                       MoveLeft    = false;
            if (key == Key.D)                       MoveRight   = false;
            if (key == Key.W)                       MoveForward = false;
            if (key == Key.S)                       MoveBack    = false;
            if (key == Key.Space)                   MoveUp      = false;
            if (key == Key.ControlLeft)             MoveDown    = false;
            if (key == Key.ShiftLeft)               Sprint      = 1;
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

            float sensitivity = Config.Sensitivity;
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
            float speed = Config.MoveSpeed * Sprint;

            float yawRad   = MathF.PI / 180f * Yaw;
            float pitchRad = MathF.PI / 180f * Pitch;

            CamFront = Vector3D.Normalize(new Vector3D<float>(
                MathF.Cos(yawRad) * MathF.Cos(pitchRad),
                MathF.Sin(pitchRad),
                MathF.Sin(yawRad) * MathF.Cos(pitchRad)
            ));

            Vector3D<float> up    = new(0, 1, 0);
            Vector3D<float> right = Vector3D.Normalize(Vector3D.Cross(CamFront, up));

            if (MoveForward)    CamPos += CamFront * speed;
            if (MoveBack)       CamPos -= CamFront * speed;
            if (MoveRight)      CamPos += right    * speed;
            if (MoveLeft)       CamPos -= right    * speed;
            if (MoveUp)         CamPos += up       * speed;
            if (MoveDown)       CamPos -= up       * speed;
            if (RotateUp)
            {
                if (TranslateMode == true)
                {
                    if (Sprint == 1)
                        Scop._objPos.Y += speed;
                    else
                        Scop._objPos.Z -= speed;
                }
                else
                    Scop._rotX += Config.RotSpeed * (float)deltaTime;
            }
            if (RotateDown)
            {
                if (TranslateMode == true)
                {
                    if (Sprint == 1)
                        Scop._objPos.Y -= speed;
                    else
                        Scop._objPos.Z += speed;
                }
                else
                    Scop._rotX -= Config.RotSpeed * (float)deltaTime;
            }
            if (RotateLeft)
            {
                if (TranslateMode == true)
                    Scop._objPos.X -= speed;
                else if (Sprint != 1)
                    Scop._rotZ -= Config.RotSpeed * (float)deltaTime;
                else
                    Scop._rotY -= Config.RotSpeed * (float)deltaTime;
            }
            if (RotateRight)
            {
                if (TranslateMode == true)
                    Scop._objPos.X += speed;
                else if (Sprint != 1)
                    Scop._rotZ += Config.RotSpeed * (float)deltaTime;
                else
                    Scop._rotY += Config.RotSpeed * (float)deltaTime;
            }
        }
    }
}
