using InfoBox;
using System.Runtime.InteropServices;
using TiaUtilities.Languages;
using TiaUtilities.Styles;
using TiaUtilities.Utility;

namespace TiaUtilities.CustomControls.EditableTab
{
    public delegate void EditableTabPreRemovedEventHandler(object? sender, EditableTabPreRemoveEventArgs args);
    public class EditableTabPreRemoveEventArgs(TabPage tabPage, int tabIndex) : EventArgs
    {
        public TabPage TabPage { get; init; } = tabPage;
        public int TabIndex { get; init; } = tabIndex;
        public bool Cancel { get; set; }
    }

    public delegate void EditableTabPreAddEventHandler(object? sender, EditableTabPreAddEventArgs args);
    public class EditableTabPreAddEventArgs(TabPage tabPage) : EventArgs
    {
        public TabPage TabPage { get; init; } = tabPage;
        public bool Cancel { get; set; }
    }

    public delegate void EditableTabNameChangedEventHandler(object? sender, EditableTabNameChangedEventArgs args);
    public class EditableTabNameChangedEventArgs(TabPage tabPage, string newName, string oldName) : EventArgs
    {
        public TabPage TabPage { get; init; } = tabPage;
        public string NewName { get; set; } = newName;
        public string OldName { get; init; } = oldName;
        public bool Handled { get; set; } = false;
    }

    public class EditableTabControl : TabControl
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        private const int TCM_SETMINTABWIDTH = 0x1300 + 49;

        private const int SELECTED_TAB_RECT_SIDE_PADDING = 4;
        private const int SELECTED_TAB_RECT_HEIGHT = 2;

        public event EditableTabPreRemovedEventHandler TabPreRemoved = delegate { };
        public event EditableTabPreAddEventHandler TabPreAdded = delegate { };
        public event EditableTabNameChangedEventHandler TabNameUserChanged = delegate { };

        public bool RequireConfirmationBeforeClosing { get; set; } = false;

        private readonly EditableNewTabPage newTabPage;

        private bool tabClosed;

        public new int TabCount
        {
            get
            {//This is to avoid keeping count of the InteractableNewTabPage
                return base.TabCount > 0 ? base.TabCount - 1 : 0;
            }
        }

        public EditableTabControl() : base()
        {
            SetStyle(ControlStyles.UserPaint, true); //This is needed for the backgroundPaint event to be called!

            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.DoubleBuffered = true;
            this.Padding = new(12, 5);
            this.Font = new Font(Font.SystemFontName, 9f, FontStyle.Italic);
            this.AllowDrop = true;

            //This allows tab to be small 
            this.HandleCreated += (sender, args) => SendMessage(this.Handle, TCM_SETMINTABWIDTH, IntPtr.Zero, (IntPtr)16);
            this.Selecting += (sender, args) =>
            {
                if (tabClosed || args.TabPage is EditableNewTabPage)
                { //If i remove a tab while mouse clicking, seems that selecting is done twice! This will avoid clicking the wrong page while closing another (Like closing the first one!) and fixes the flicker.
                    tabClosed = false;
                    args.Cancel = true;
                }
            };

            this.newTabPage = new();
            this.TabPages.Add(this.newTabPage);

            this.ControlAdded += (sender, args) =>
            {
                if (this.TabPages.IndexOf(this.newTabPage) != base.TabCount - 1)
                {
                    this.TabPages.Remove(this.newTabPage);
                    this.TabPages.Add(this.newTabPage);
                }
            };
        }

        protected override void OnPaint(PaintEventArgs e) { }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Rectangle clientRect = this.ClientRectangle;

            //Painting Background
            using Brush backBrush = new SolidBrush(SystemColors.Control);
            e.Graphics.FillRectangle(backBrush, clientRect);

            for (int i = 0; i < this.TabPages.Count; i++)
            {
                this.DrawCustomItem(e.Graphics, i);
            }
        }

        private void DrawCustomItem(Graphics g, int index)
        {
            var selectedForeColor = StyleManager.EditableTabControl.SELECTED_TAB_FORE_COLOR;
            var selectedBackColor = StyleManager.EditableTabControl.SELECTED_TAB_BACK_COLOR;

            var baseForeColor = StyleManager.EditableTabControl.TAB_FORE_COLOR;
            var baseBackColor = StyleManager.EditableTabControl.TAB_BACK_COLOR;


            var tabPage = this.TabPages[index];
            var isNewTagPage = tabPage is EditableNewTabPage;
            var isSelected = this.SelectedTab == tabPage;

            var tabRect = this.GetTabRect(index);

            //Draw background
            if (!isNewTagPage)
            {
                using Brush backBrush = new SolidBrush(isSelected ? selectedBackColor : baseBackColor);
                g.FillRectangle(backBrush, tabRect);
            }

            using Pen displayRectPen = new(selectedBackColor, 3f);

            var borderRect = this.DisplayRectangle;
            borderRect.Offset(-1, 0);
            borderRect.Inflate(3, 2);
            g.DrawRectangle(displayRectPen, borderRect);

            if (isNewTagPage)
            {
                var textRect = tabRect;
                textRect.Offset(0, -1);
                TextRenderer.DrawText(g,
                    tabPage.Text,
                    tabPage.Font,
                    textRect,
                    StyleManager.EditableTabControl.ADD_TAB_FORE_COLOR,
                    Color.Transparent,
                    TextFormatFlags.Top);
            }
            else
            {
                int textHeightOffset = -2;

                //Draw selected tab bottom line
                if (isSelected)
                {
                    var selectedRect = tabRect;
                    selectedRect.Offset(SELECTED_TAB_RECT_SIDE_PADDING, selectedRect.Height - (SELECTED_TAB_RECT_HEIGHT + 1));
                    selectedRect.Width -= SELECTED_TAB_RECT_SIDE_PADDING * 2;
                    selectedRect.Height = SELECTED_TAB_RECT_HEIGHT;

                    using Brush bottomRectBrush = new SolidBrush(StyleManager.EditableTabControl.SELECTED_TAB_BOTTOM_LINE_COLOR);
                    g.FillRectangle(bottomRectBrush, selectedRect);

                    textHeightOffset += SELECTED_TAB_RECT_HEIGHT;
                }

                var textRect = tabRect;
                textRect.Offset(0, 3);
                TextRenderer.DrawText(g,
                    tabPage.Text,
                    tabPage.Font,
                    textRect,
                    isSelected ? selectedForeColor : baseForeColor,
                    Color.Transparent,
                    TextFormatFlags.TextBoxControl | TextFormatFlags.WordEllipsis | TextFormatFlags.HorizontalCenter);
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e) { }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            var horizontalScroll = ControlUtils.WncProcHorizontalScrollWheel(m);
            if (horizontalScroll != 0)
            {
                SelectNextTab(previous: (horizontalScroll < 0));
            }
        }

        private Point? dragMouseDownPoint;
        protected override void OnMouseDown(MouseEventArgs args)
        {
            base.OnMouseDown(args);

            var index = GetLocationTabRectIndex(args.Location);
            if (index == -1)
            {
                return;
            }

            var tabPage = this.TabPages[index];
            if (tabPage is EditableNewTabPage)
            {
                this.AddTabs(count: args.Button == MouseButtons.Right ? 5 : 1);
                return;
            }

            if (args.Button == MouseButtons.Left)
            {
                dragMouseDownPoint = new(args.X, args.Y);
            }
            else if (args.Button == MouseButtons.Right)
            {
                this.SelectedTab = tabPage;
                this.HandleContextMenu(tabPage);
            }
        }

        protected override void OnMouseUp(MouseEventArgs args)
        {
            base.OnMouseUp(args);

            this.dragMouseDownPoint = null;
        }

        protected override void OnMouseDoubleClick(MouseEventArgs args)
        {
            base.OnMouseDoubleClick(args);

            var index = GetLocationTabRectIndex(args.Location);
            if (index == -1)
            {
                return;
            }

            var tabPage = this.TabPages[index];
            if (tabPage is EditableNewTabPage)
            {
                this.AddTabs(count: args.Button == MouseButtons.Right ? 5 : 1);
                return;
            }

            this.SelectedTab = tabPage;
            this.HandleContextMenu(tabPage);
        }

        protected override void OnMouseMove(MouseEventArgs args)
        {
            base.OnMouseMove(args);

            var index = this.GetLocationTabRectIndex(args.Location);
            if (index == -1)
            {
                return;
            }

            var tabPage = this.TabPages[index];
            if (tabPage is EditableNewTabPage)
            {
                return;
            }

            if (!this.dragMouseDownPoint.HasValue || Math.Abs(this.dragMouseDownPoint.Value.X - args.X) < 5)
            {
                return;
            }

            if (args.Button == MouseButtons.Left)
            {
                EditableTabControl.DragAndDropData dragAndDrop = new(tabPage);
                this.DoDragDrop(dragAndDrop, DragDropEffects.Move, null, Point.Empty, true);
            }
        }

        protected override void OnDragOver(DragEventArgs args)
        {
            base.OnDragOver(args);

            var data = args.Data;
            if (data == null || this.TabCount == 0)
            {
                args.Effect = DragDropEffects.None;
                return;
            }

            var tabRectIndex = this.GetMousePointTabRectIndex(args.X, args.Y);
            args.Effect = tabRectIndex == -1 || tabRectIndex >= this.TabCount ? DragDropEffects.None : DragDropEffects.Move;
        }

        protected override void OnDragDrop(DragEventArgs args)
        {
            base.OnDragDrop(args);

            var tabRectIndex = this.GetMousePointTabRectIndex(args.X, args.Y);
            if (tabRectIndex == -1)
            {
                return;
            }

            var droppedTabPage = this.TabPages[tabRectIndex];

            var dataObj = args.Data?.GetData(typeof(EditableTabControl.DragAndDropData));
            if (dataObj is EditableTabControl.DragAndDropData dragAndDrop && dragAndDrop.TabPage != droppedTabPage)
            {
                this.SuspendLayout();

                int draggedIndex = this.TabPages.IndexOf(dragAndDrop.TabPage);
                int droppedIndex = this.TabPages.IndexOf(droppedTabPage);

                if (draggedIndex >= 0 && draggedIndex < this.TabCount &&
                    droppedIndex >= 0 && droppedIndex < this.TabCount &&
                    draggedIndex != droppedIndex)
                {
                    var oldSelectedTab = this.SelectedTab;

                    (this.TabPages[droppedIndex], this.TabPages[draggedIndex]) = (this.TabPages[draggedIndex], this.TabPages[droppedIndex]);

                    this.SelectedTab = oldSelectedTab;
                }

                this.ResumeLayout(true);
            }
        }

        private int GetMousePointTabRectIndex(int x, int y)
        {
            Point mousePoint = new(x, y);
            return GetLocationTabRectIndex(this.PointToClient(mousePoint));
        }

        private int GetLocationTabRectIndex(Point p)
        {
            for (int i = 0; i < base.TabCount; i++)
            {
                var tabRect = this.GetTabRect(i);
                if (tabRect.Contains(p))
                {
                    return i;
                }
            }

            return -1;
        }

        private void HandleContextMenu(TabPage tabPage)
        {
            var contextMenu = EditableTabControlContextMenuFactory.CreateContextMenu(this, tabPage);
            contextMenu.Show(Cursor.Position);
        }

        public void AddTabs(int count = 1)
        {
            for (int x = 0; x < count; x++)
            {
                var tabPage = this.CreateTabPage();
                if (tabPage != null)
                {
                    this.TabPages.Add(tabPage);
                }
            }
        }

        public void InsertTabs(int index, int count = 1)
        {
            if (index < 0 || index >= this.TabCount)
            {
                return;
            }

            for (int x = 0; x < count; x++)
            {
                var tabPage = this.CreateTabPage();
                if (tabPage != null)
                {
                    this.TabPages.Insert(index + 1, tabPage);
                }
            }
        }

        private TabPage? CreateTabPage()
        {
            TabPage newTabPage = new();

            var eventArgs = new EditableTabPreAddEventArgs(newTabPage);
            TabPreAdded(this, eventArgs);
            return eventArgs.Cancel ? null : newTabPage;
        }

        public void RenameTab(TabPage tabPage, string newName)
        {
            var oldName = tabPage.Text;
            if (oldName == newName)
            {
                return;
            }

            EditableTabNameChangedEventArgs args = new(tabPage, newName, oldName);
            TabNameUserChanged(this, args);
            if (!args.Handled)
            {
                tabPage.Text = args.NewName;
            }
        }

        public bool CloseTab(TabPage tabPage, bool forceClosing = false)
        {
            var closeRequest = new CloseRequest() { TabPage = tabPage };
            this.CloseTabs([closeRequest], forceClosing);
            return closeRequest.Closed;
        }

        public void CloseTabs(IEnumerable<CloseRequest> closeRequests, bool forceClosing = false)
        {
            var validCloseRequests = closeRequests.Where(cr => cr.TabPage is not EditableNewTabPage);

            if (!forceClosing && this.RequireConfirmationBeforeClosing)
            {
                var names = String.Join(", ", validCloseRequests.Select(cr => cr.TabPage.Text));

                var result = InformationBox.Show(Locale.EDITABLE_TAB_CONTROL_DELETE_CONFIRM.Replace("{t}", names), buttons: InformationBoxButtons.YesNo);
                if (result != InformationBoxResult.Yes)
                {
                    return;
                }
            }

            foreach (var closeRequest in validCloseRequests)
            {
                var tabPage = closeRequest.TabPage;

                var index = this.TabPages.IndexOf(tabPage);
                if (index < 0)
                {
                    continue;
                }

                var args = new EditableTabPreRemoveEventArgs(tabPage, index);
                this.TabPreRemoved(this, args);
                if (args.Cancel)
                {
                    continue;
                }

                this.TabPages.Remove(tabPage);
                closeRequest.Closed = true;
            }
        }

        public void SelectNextTab(bool previous)
        {
            var selectedTab = this.SelectedTab;
            if (selectedTab == null)
            {
                return;
            }

            var selectedIndex = this.TabPages.IndexOf(selectedTab);
            if (previous)
            {
                this.SelectedIndex = Math.Max(0, selectedIndex - 1);
            }
            else
            {
                this.SelectedIndex = Math.Min(this.TabCount - 1, selectedIndex + 1);
            }
        }

        private record DragAndDropData(TabPage TabPage);

        public class CloseRequest()
        {
            public required TabPage TabPage { get; init; }
            public object? Tag { get; set; }
            public bool Closed { get; set; } = false;

            public override string ToString()
            {
                return $"TabPage: {this.TabPage.Text}, Closed: {this.Closed}";
            }
        }

    }

    public class EditableNewTabPage : TabPage
    {
        public EditableNewTabPage() : base()
        {
            this.DoubleBuffered = true;

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


/*
if (this.SelectedTab == tabPage)
{//Draw little green sphere to indicate selected tab
    var ellipseRect = this.GetTabRect(e.Index);
    ellipseRect.Offset(2, 2);
    ellipseRect.Width = 7;
    ellipseRect.Height = 7;

    e.Graphics.FillEllipse(Brushes.DarkSeaGreen, ellipseRect);

    using var activePageEllipseBorderPen = new Pen(Brushes.Gray, 1f);
    e.Graphics.DrawEllipse(activePageEllipseBorderPen, ellipseRect);


}
protected override void OnMouseDoubleClick(MouseEventArgs e)
{
    var index = GetLocationTabRectIndex(e.Location);
    if (index == -1)
    {
        return;
    }

    var tabPage = this.TabPages[index];
    if (tabPage is not EditableNewTabPage)
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
            RenameTab(tabPage, floatingTextBox.InputText);
        }
    }
}
*/