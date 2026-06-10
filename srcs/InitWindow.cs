using Silk.NET.Windowing;
using Silk.NET.Maths;

namespace Scop
{
    partial class Program
    {
        private static void InitWindow()
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(800, 600);
            options.Title = "Window";

            window = Window.Create(options);

            // window.Load += is3D ? Load3D : Load2D;
            window.Load += Load2D;
            window.Render += OnRender;
            window.Update += OnUpdate;
            window.FramebufferResize += OnFramebufferResize;
            window.Closing += OnClose;
        }
    }
}
