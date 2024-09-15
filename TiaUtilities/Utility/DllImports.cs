using System.Runtime.InteropServices;

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
