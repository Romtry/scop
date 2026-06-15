using Silk.NET.Windowing;
using Silk.NET.Maths;

namespace Scop
{
    partial class Scop
    {
        private static void InitWindow()
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(800, 600);
            options.Title = "Window";

            window = Window.Create(options);

            window.Load += is3D ? Load3D : Load2D;
            window.Render += is3D ? OnRender3D : OnRender2D;
            window.Update += OnUpdate;
            window.FramebufferResize += OnFramebufferResize;
            window.Closing += OnClose;
        }

        private static void OnClose()
        {

        }
    }
}
