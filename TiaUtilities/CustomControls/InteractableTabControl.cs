
using InfoBox;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TiaUtilities.Utility;

namespace TiaUtilities.CustomControls
{
    public class InteractableTabControl : TabControl
    {

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        private const int TCM_SETMINTABWIDTH = 0x1300 + 49;

        public event InteractableTabPreRemovedEventHandler TabPreRemoved = delegate { };
        public event InteractableTabPreAddEventHandler TabPreAdded = delegate { };
        public event InteractableTabNameChangedEventHandler TabNameUserChanged = delegate { };

        public bool RequireConfirmationBeforeClosing { get; set; } = false;

        private readonly InteractableNewTabPage newTabPage;

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
                if (tabClosed || args.TabPage is InteractableNewTabPage)
                { //If i remove a tab while mouse clicking, seems that selecting is done twice! This will avoid clicking the wrong page while closing another (Like closing the first one!) and fixes the flicker.
                    tabClosed = false;
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
            var isNewTagPage = tab is InteractableNewTabPage;

            var tabRect = GetTabRect(e.Index);
            tabRect.Width += this.Padding.X;
            tabRect.Height = this.Padding.Y;

            var textRect = new Rectangle(tabRect.Location, tabRect.Size);
            if (!isNewTagPage)
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
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            var index = GetLocationTabRectIndex(e.Location);
            if (index == -1)
            {
                return;
            }

            var tabPage = this.TabPages[index];
            if (tabPage is not InteractableNewTabPage)
            {
                var floatingTextBox = new FloatingTextBox()
                {
                    StartPosition = FormStartPosition.Manual,
                    Location = tabPage.PointToScreen(e.Location),
                    InputText = tabPage.Text,
                };
                floatingTextBox.Size = floatingTextBox.Size with { Width = 250 };
                if (floatingTextBox.ShowDialog(this) == DialogResult.OK)
                {
                    InteractableTabNameChangedEventArgs args = new(tabPage, tabPage.Text, floatingTextBox.InputText);
                    TabNameUserChanged(this, args);
                    if (!args.Cancel)
                    {
                        tabPage.Text = args.NewName;
                    }
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            var index = GetLocationTabRectIndex(e.Location);
            if (index == -1)
            {
                return;
            }

            var tabPage = this.TabPages[index];
            if (tabPage is InteractableNewTabPage)
            {
                var addedTabPage = new TabPage();

                var eventArgs = new InteractableTabPreAddEventArgs(addedTabPage);
                TabPreAdded(this, eventArgs);
                if (!eventArgs.Cancel)
                {
                    this.TabPages.Add(addedTabPage);
                }
            }
            else
            {
                var tabRect = GetCloseCrossRect(index);
                tabRect.Inflate(2, 2); //Make a little bit bigger so is easier to click.
                if (tabRect.Contains(e.Location))
                {
                    CloseTab(tabPage, index);
                    tabClosed = true;
                }
            }
        }

        private int GetLocationTabRectIndex(Point p)
        {
            for (int i = 0; i < TabCount; i++)
            {
                var tabRect = this.GetTabRect(i);
                if (tabRect.Contains(p))
                {
                    return i;
                }
            }

            return -1;
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            base.OnKeyDown(args);
        }

        private Rectangle GetCloseCrossRect(int index)
        {
            var crossRect = this.GetTabRect(index);
            crossRect.Offset(2, 2);
            crossRect.Width = 7;
            crossRect.Height = 7;
            return crossRect;
        }

        private void CloseTab(TabPage tabPage, int index)
        {
            if (tabPage is InteractableNewTabPage)
            {
                return;
            }

            if(this.RequireConfirmationBeforeClosing)
            {
                var result = InformationBox.Show($"Are you sure you want to close {tabPage.Text}?", buttons: InformationBoxButtons.YesNo);
                if (result == InformationBoxResult.No)
                {
                    return;
                }
            }

            var args = new InteractableTabPreRemoveEventArgs(tabPage, index);
            TabPreRemoved(this, args);
            if (!args.Cancel)
            {
                TabPages.Remove(tabPage);
            }
        }
    }

    public delegate void InteractableTabPreRemovedEventHandler(object? sender, InteractableTabPreRemoveEventArgs args);
    public class InteractableTabPreRemoveEventArgs(TabPage tabPage, int tabIndex) : EventArgs
    {
        public TabPage TabPage { get; init; } = tabPage;
        public int TabIndex { get; init; } = tabIndex;
        public bool Cancel { get; set; }
    }

    public delegate void InteractableTabPreAddEventHandler(object? sender, InteractableTabPreAddEventArgs args);
    public class InteractableTabPreAddEventArgs(TabPage tabPage) : EventArgs
    {
        public TabPage TabPage { get; init; } = tabPage;
        public bool Cancel { get; set; }
    }

    public delegate void InteractableTabNameChangedEventHandler(object? sender, InteractableTabNameChangedEventArgs args);
    public class InteractableTabNameChangedEventArgs(TabPage tabPage, string oldName, string newName) : EventArgs
    {
        public TabPage TabPage { get; init; } = tabPage;
        public string OldName { get; init; } = oldName;
        public string NewName { get; set; } = newName;
        public bool Cancel { get; set; }
    }

    public class InteractableNewTabPage : TabPage
    {
        public InteractableNewTabPage() : base()
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
