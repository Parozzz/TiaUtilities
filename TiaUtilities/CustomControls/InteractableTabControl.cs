
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TiaUtilities.CustomControls
{
    public class InteractableTabControl : TabControl
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        private const int TCM_SETMINTABWIDTH = 0x1300 + 49;

        public event TabPreRemovedEventHandler TabPreRemoved = delegate { };
        public event TabPreAddEventHandler TabPreAdd = delegate { };
        private readonly NewTabPage newTabPage;

        private bool tabClosed;

        public InteractableTabControl() : base()
        {
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.Padding = new Point(12, 5);

            this.HandleCreated += (sender, args) =>
            {//This allows tab to be small
                SendMessage(this.Handle, TCM_SETMINTABWIDTH, IntPtr.Zero, (IntPtr)16);
            };

            this.Selecting += (sender, args) =>
            {
                if(tabClosed)
                {//If i remove a tab while mouse clicking, seems that selecting is done twice! This will avoid clicking the wrong page while closing another (Like closing the first one!) and fixes the flicker.
                    tabClosed = false;
                    args.Cancel = true;

                    return;
                }

                Debug.WriteLine("SELECTING!");
                if (args.TabPage is NewTabPage)
                {
                    var page = new TabPage();

                    var eventArgs = new TabPreAddEventArgs(page);
                    TabPreAdd(this, eventArgs);

                    if(!eventArgs.Cancel)
                    {
                        this.TabPages.Add(page);
                    }

                    args.Cancel = true;
                }
            };

            this.newTabPage = new();
            this.TabPages.Add(this.newTabPage);

            this.ControlAdded += (sender, args) =>
            {
                if (this.TabPages.IndexOf(this.newTabPage) != this.TabCount - 1)
                {
                    this.TabPages.Remove(this.newTabPage);
                    this.TabPages.Add(this.newTabPage);
                }
            };
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            var tab = this.TabPages[e.Index];
            var isNewTagPage = tab is NewTabPage;

            var tabRect = GetTabRect(e.Index);
            tabRect.Width += this.Padding.X;
            tabRect.Height = this.Padding.Y;

            var textRect = new Rectangle(tabRect.Location, tabRect.Size);
            if(!isNewTagPage)
            { //The cross to close is not drawn, so no need to offset!
                textRect.Offset(4, 2);
            }

            using var textBrush = new SolidBrush(tab.ForeColor);
            string title = tab.Text;
            var font = tab.Font;
            if (!isNewTagPage)
            {
                e.Graphics.DrawString(title, font, textBrush, new PointF(textRect.X + 9, textRect.Y + textRect.Height / 2));

                var crossRect = GetCloseCrossRect(e.Index);

                using var closingCrossPen = new Pen(new SolidBrush(Color.Red), 2);
                e.Graphics.DrawLine(closingCrossPen, crossRect.X, crossRect.Y, crossRect.X + crossRect.Width, crossRect.Y + crossRect.Height); //Draw \
                e.Graphics.DrawLine(closingCrossPen, crossRect.X + crossRect.Width, crossRect.Y, crossRect.X, crossRect.Y + crossRect.Height); //Draw /
            }
            else
            {
                e.Graphics.DrawString(title, font, textBrush, new PointF(textRect.X + this.Padding.X / 2 + 2, textRect.Y));
            }
            /*
            if (e.Index == TabPages.Count - 1)
            {
                var plusRect = GetTabRect(e.Index);
                plusRect.Offset(plusRect.Width + 5, plusRect.Height / 2);
                plusRect.Height = plusRect.Width = 8;

                using var plusPen = new Pen(new SolidBrush(Color.Green), 2);

                e.Graphics.DrawLine(plusPen, plusRect.X, plusRect.Y, plusRect.X + plusRect.Width, plusRect.Y);
                e.Graphics.DrawLine(plusPen, plusRect.X + plusRect.Width / 2, plusRect.Y - plusRect.Height / 2, plusRect.X + plusRect.Width / 2, plusRect.Y + plusRect.Height / 2);
            }*/
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Debug.WriteLine("MOUSE DOWN!");
            Point p = e.Location;
            for (int i = 0; i < TabCount; i++)
            {
                var tabRect = GetCloseCrossRect(i);
                tabRect.Inflate(2, 2); //Make a little bit bigger so is easier to click.
                if (tabRect.Contains(p))
                {
                    CloseTab(i);

                    tabClosed = true;
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            base.OnKeyDown(args);
        }

        private Rectangle GetCloseCrossRect(int index)
        {
            var crossRect = GetTabRect(index);
            crossRect.Offset(2, 2);
            crossRect.Width = 7;
            crossRect.Height = 7;
            return crossRect;
        }

        private void CloseTab(int i)
        {
            if (TabPages[i] is NewTabPage)
            {
                return;
            }

            var args = new TabPreRemoveEventArgs(i);
            TabPreRemoved(this, args);

            if (!args.Handled)
            {
                TabPages.Remove(TabPages[i]);
            }
        }
    }

    public delegate void TabPreRemovedEventHandler(object? sender, TabPreRemoveEventArgs args);

    public class TabPreRemoveEventArgs(int tabIndex) : EventArgs
    {
        public int TabIndex { get; set; } = tabIndex;
        public bool Handled { get; set; }
    }

    public delegate void TabPreAddEventHandler(object? sender, TabPreAddEventArgs args);

    public class TabPreAddEventArgs(TabPage tabPage) : EventArgs
    {
        public TabPage Page { get; set; } = tabPage;
        public bool Cancel { get; set; }
    }

    public class NewTabPage : TabPage
    {
        public NewTabPage() : base()
        {
            this.Text = "+";
            this.Width = 5;

            this.Padding = this.Margin = Padding.Empty;

            this.BorderStyle = BorderStyle.None;
            this.ForeColor = Color.Green;
            this.Font = new Font(Font.SystemFontName, 15f, FontStyle.Regular);

        }

        protected override void OnPaint(PaintEventArgs e) { }

        protected override void OnPaintBackground(PaintEventArgs e) { }
    }
}
