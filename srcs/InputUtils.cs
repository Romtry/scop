using Silk.NET.Input;
using Silk.NET.Windowing;
using System;

namespace Scop
{
    public static class InputUtils
    {
        public static void SetupInput(IWindow window, Action<IKeyboard, Key, int> onKeyDown)
        {
            IInputContext input = window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += (keyboard, key, keyCode) => onKeyDown(keyboard, key, keyCode);
            }
        }
    }
}
