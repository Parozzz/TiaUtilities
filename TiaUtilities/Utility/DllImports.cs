using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Utility
{
    public static class DllImports
    {
        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_MOUSEMOVE = 0x0200;
        public const uint WM_MOUSEWHEEL = 0x020A;
    }
}
