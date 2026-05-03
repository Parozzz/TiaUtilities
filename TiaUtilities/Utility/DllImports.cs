using System.Runtime.InteropServices;

namespace TiaUtilities.Utility
{
    public static class DllImports
    {
        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        //This is a replacement for Cursor.Position in WinForms
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_MOUSEMOVE = 0x0200;
        public const uint WM_MOUSEWHEEL = 0x020A;

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

    }
}
