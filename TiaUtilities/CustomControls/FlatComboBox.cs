﻿using System.ComponentModel;
using System.Runtime.InteropServices;
using TiaUtilities.CustomControls;

namespace TiaUtilities.CustomControls
{
    public class FlatComboBox : ComboBox
    {
        private Color borderColor = Color.Gray;
        private Color buttonColor = Color.LightGray;

        [DefaultValue(typeof(Color), "Gray")]
        [Category("Appearance")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                if (borderColor != value)
                {
                    borderColor = value;
                    Invalidate();
                }
            }
        }

        [DefaultValue(typeof(Color), "LightGray")]
        [Category("Appearance")]
        public Color ButtonColor
        {
            get { return buttonColor; }
            set
            {
                if (buttonColor != value)
                {
                    buttonColor = value;
                    Invalidate();
                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_PAINT && DropDownStyle != ComboBoxStyle.Simple)
            {
                var clientRect = ClientRectangle;

                var dropDownButtonWidth = SystemInformation.HorizontalScrollBarArrowWidth;
                var outerBorder = new Rectangle(clientRect.Location, new Size(clientRect.Width - 1, clientRect.Height - 1));
                var innerBorder = new Rectangle(outerBorder.X + 1, outerBorder.Y + 1, outerBorder.Width - dropDownButtonWidth - 2, outerBorder.Height - 2);
                var innerInnerBorder = new Rectangle(innerBorder.X + 1, innerBorder.Y + 1, innerBorder.Width - 2, innerBorder.Height - 2);
                var dropDownRect = new Rectangle(innerBorder.Right + 1, innerBorder.Y, dropDownButtonWidth, innerBorder.Height + 1);
                if (RightToLeft == RightToLeft.Yes)
                {
                    innerBorder.X = clientRect.Width - innerBorder.Right;
                    innerInnerBorder.X = clientRect.Width - innerInnerBorder.Right;
                    dropDownRect.X = clientRect.Width - dropDownRect.Right;
                    dropDownRect.Width += 1;
                }
                var innerBorderColor = BackColor; //Enabled ? BackColor : SystemColors.Control;
                var outerBorderColor = BorderColor; //Enabled ? BorderColor : SystemColors.ControlDark;
                var buttonColor = ButtonColor; //Enabled ? ButtonColor : SystemColors.Control;
                var middle = new Point(dropDownRect.Left + dropDownRect.Width / 2, dropDownRect.Top + dropDownRect.Height / 2);
                var arrow = new Point[] { new(middle.X - 3, middle.Y - 2), new(middle.X + 4, middle.Y - 2), new(middle.X, middle.Y + 2) };

                var paintStruct = new PAINTSTRUCT();
                bool shoulEndPaint = false;
                IntPtr dc;
                if (m.WParam == IntPtr.Zero)
                {
                    dc = BeginPaint(Handle, ref paintStruct);
                    m.WParam = dc;
                    shoulEndPaint = true;
                }
                else
                {
                    dc = m.WParam;
                }
                var rgn = CreateRectRgn(innerInnerBorder.Left, innerInnerBorder.Top,
                    innerInnerBorder.Right, innerInnerBorder.Bottom);
                SelectClipRgn(dc, rgn);
                DefWndProc(ref m);
                DeleteObject(rgn);
                rgn = CreateRectRgn(clientRect.Left, clientRect.Top,
                    clientRect.Right, clientRect.Bottom);
                SelectClipRgn(dc, rgn);

                using var graphics = Graphics.FromHdc(dc);

                using var buttonBrush = new SolidBrush(buttonColor);
                graphics.FillRectangle(buttonBrush, dropDownRect);

                using var outerBorderBrush = new SolidBrush(outerBorderColor);
                graphics.FillPolygon(outerBorderBrush, arrow);

                using var innerBorderPen = new Pen(innerBorderColor);
                graphics.DrawRectangle(innerBorderPen, innerBorder);
                graphics.DrawRectangle(innerBorderPen, innerInnerBorder);

                using var outerBorderPen = new Pen(outerBorderColor);
                graphics.DrawRectangle(outerBorderPen, outerBorder);

                if (shoulEndPaint)
                {
                    EndPaint(Handle, ref paintStruct);
                }
                DeleteObject(rgn);
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        private const int WM_PAINT = 0xF;
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int L, T, R, B;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public int rcPaint_left;
            public int rcPaint_top;
            public int rcPaint_right;
            public int rcPaint_bottom;
            public bool fRestore;
            public bool fIncUpdate;
            public int reserved1;
            public int reserved2;
            public int reserved3;
            public int reserved4;
            public int reserved5;
            public int reserved6;
            public int reserved7;
            public int reserved8;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr BeginPaint(IntPtr hWnd,
            [In, Out] ref PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        private static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport("gdi32.dll")]
        public static extern int SelectClipRgn(IntPtr hDC, IntPtr hRgn);

        [DllImport("user32.dll")]
        public static extern int GetUpdateRgn(IntPtr hwnd, IntPtr hrgn, bool fErase);
        public enum RegionFlags
        {
            ERROR = 0,
            NULLREGION = 1,
            SIMPLEREGION = 2,
            COMPLEXREGION = 3,
        }

        [DllImport("gdi32.dll")]
        internal static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);
    }
}
