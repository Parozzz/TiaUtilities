﻿using System.ComponentModel;
using System.Runtime.InteropServices;

namespace TiaUtilities.CustomControls
{
    public class FlatTextBox : TextBox
    {
        Color borderColor = Color.Gray;

        [DefaultValue(typeof(Color), "Gray")]
        [Category("Appearance")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero,
                    RDW_FRAME | RDW_IUPDATENOW | RDW_INVALIDATE);
            }
        }

        const int WM_NCPAINT = 0x85;
        const uint RDW_INVALIDATE = 0x1;
        const uint RDW_IUPDATENOW = 0x100;
        const uint RDW_FRAME = 0x400;

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprc, IntPtr hrgn, uint flags);

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCPAINT && BorderColor != Color.Transparent) //&& BorderStyle == BorderStyle.Fixed3D)
            {
                var hdc = GetWindowDC(this.Handle);

                using var graphics = Graphics.FromHdcInternal(hdc);

                using var borderPen = new Pen(BorderColor, 2);
                graphics.DrawRectangle(borderPen, new Rectangle(0, 0, Width - 1, Height - 1));

                ReleaseDC(this.Handle, hdc);
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero,
                   RDW_FRAME | RDW_IUPDATENOW | RDW_INVALIDATE);
        }
    }
}
