using InfoBox;
using MS.WindowsAPICodePack.Internal;
using System.Runtime.InteropServices;
using TiaUtilities.Languages;
using TiaUtilities.Resources;

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
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.Padding = new Point(12, 5);
            this.AllowDrop = true;

            this.BackColor = Color.Yellow;

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

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            //Calling it AFTER the base.WndProc i am ovewriting the default painting!
            if (m.Msg == 0xF) //WM_PAINT
            {
                using Graphics g = this.CreateGraphics();
                //Double buffering stuff...
                BufferedGraphicsContext currentContext;
                BufferedGraphics myBuffer;
                currentContext = BufferedGraphicsManager.Current;
                myBuffer = currentContext.Allocate(g,
                   this.ClientRectangle);

                Rectangle r = ClientRectangle;

                //Painting background
                if (Enabled)
                    myBuffer.Graphics.FillRectangle(new SolidBrush(Color.Yellow), r);
                else
                    myBuffer.Graphics.FillRectangle(Brushes.LightGray, r);

                //Painting border
                r.Height = this.DisplayRectangle.Height + 1; //Using display rectangle hight because it excludes the tab headers already
                r.Y = this.DisplayRectangle.Y - 1; //Same for Y coordinate
                r.Width -= 5;
                r.X += 1;

                if (Enabled)
                    myBuffer.Graphics.DrawRectangle(new Pen(Color.FromArgb(255, 133, 158, 191), 1), r);
                else
                    myBuffer.Graphics.DrawRectangle(Pens.DarkGray, r);

                myBuffer.Render();
                myBuffer.Dispose();


                //Actual painting of items after Background was painted
                /*
                foreach (int index in ItemArgs.Keys)
                {
                    CustomDrawItem(ItemArgs[index]);
                }
                */
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            var tabPage = this.TabPages[e.Index];
            var isNewTagPage = tabPage is EditableNewTabPage;
            var isSelected = this.SelectedTab == tabPage;

            var coloredForeColor = Color.White;
            var baseForeColor = tabPage.ForeColor; ;
            var coloredBackColor = Color.Blue;
            var baseBackColor = tabPage.BackColor;
            /*
            if (!isNewTagPage)
            {
                using Brush backBrush = new SolidBrush(isSelected ? coloredBackColor : baseBackColor);

                var bounds = e.Bounds;
                bounds.Offset(0, 1);
                bounds.Inflate(0, 0);
                e.Graphics.FillRectangle(backBrush, bounds);
            }

            using Pen displayRectPen = new(coloredBackColor, 3f);

            var displayRect = this.DisplayRectangle;
            displayRect.Offset(-1, 0);
            displayRect.Inflate(3, 2);
            e.Graphics.DrawRectangle(displayRectPen, displayRect);
            */
            var tabRect = GetTabRect(e.Index);
            tabRect.Width += this.Padding.X;
            tabRect.Height = this.Padding.Y;

            var textRect = new Rectangle(tabRect.Location, tabRect.Size);
            if (!isNewTagPage)
            { //The cross to close is not drawn, so no need to offset!
                textRect.Offset(4, 2);
            }

            using var textBrush = new SolidBrush(tabPage.ForeColor);

            string title = tabPage.Text;
            var font = tabPage.Font;
            if (isNewTagPage)
            {
                Point textPositionPoint = new(textRect.X + this.Padding.X / 2 + 2, textRect.Y);
                TextRenderer.DrawText(e.Graphics, title, font, textPositionPoint, baseForeColor, baseBackColor, TextFormatFlags.WordEllipsis);
            }
            else
            {
                int selectedTabOffset = 0;
                if (isSelected)
                {//Draw little green sphere to indicate selected tab
                    var selectedRect = this.GetTabRect(e.Index);
                    selectedRect.Offset(SELECTED_TAB_RECT_SIDE_PADDING, selectedRect.Height - (SELECTED_TAB_RECT_HEIGHT + 1));
                    selectedRect.Width -= SELECTED_TAB_RECT_SIDE_PADDING * 2;
                    selectedRect.Height = SELECTED_TAB_RECT_HEIGHT;

                    e.Graphics.FillRectangle(Brushes.DarkGray, selectedRect);

                    selectedTabOffset = SELECTED_TAB_RECT_HEIGHT;
                }

                Point textPositionPoint = new(textRect.X + 9, textRect.Y + textRect.Height / 2 - selectedTabOffset);
                TextRenderer.DrawText(e.Graphics, title, font, textPositionPoint, baseForeColor, baseBackColor, TextFormatFlags.WordEllipsis);

                /*
                    TextRenderer.DrawText(e.Graphics, title, font, textPositionPoint, 
                    isSelected ? coloredForeColor : baseForeColor,
                    isSelected ? coloredBackColor : baseBackColor, 
                    TextFormatFlags.WordEllipsis);
                 */

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
                this.AddTab();
                return;
            }

            if (args.Button == MouseButtons.Left)
            {
                dragMouseDownPoint = new(args.X, args.Y);
            }
            else if (args.Button == MouseButtons.Right)
            {
                this.SelectedTab = tabPage;
                this.HandleContextMenu(tabPage, index);
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
                this.AddTab();
                return;
            }

            this.SelectedTab = tabPage;
            this.HandleContextMenu(tabPage, index);
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
                this.DoDragDrop(tabPage, DragDropEffects.Move, null, Point.Empty, true);
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
            args.Message = "Test Message?";
            args.MessageReplacementToken = "WOW!";

            var tabRectIndex = this.GetMousePointTabRectIndex(args.X, args.Y);
            args.Effect = tabRectIndex == -1 || tabRectIndex >= this.TabCount ? DragDropEffects.None : DragDropEffects.Move;
        }

        protected override void OnDragEnter(DragEventArgs args)
        {
            base.OnDragEnter(args);
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

            var dataObj = args.Data?.GetData(typeof(EditableNormalTabPage));
            if (dataObj is TabPage draggedTabPage && draggedTabPage != droppedTabPage)
            {
                this.SuspendLayout();

                int draggedIndex = this.TabPages.IndexOf(draggedTabPage);
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

        private const string IMAGE_QUICK_EDIT_KEY = "QuickEdit";
        private const string IMAGE_ADD_KEY = "Add";
        private const string IMAGE_EDIT_NAME_KEY = "EditName";
        private const string IMAGE_CLOSE_KEY = "Close";

        private void HandleContextMenu(TabPage tabPage, int index)
        {
            ImageList imageList = new();
            imageList.Images.Add(IMAGE_QUICK_EDIT_KEY, ImageResources.EDIT_562275);
            imageList.Images.Add(IMAGE_ADD_KEY, ImageResources.ADD_501366_007435);
            imageList.Images.Add(IMAGE_EDIT_NAME_KEY, ImageResources.A_TO_Z_72773);
            imageList.Images.Add(IMAGE_CLOSE_KEY, ImageResources.CLOSE_193002_FF001C);

            var contextMenu = new ContextMenuStrip()
            {
                MinimumSize = new(300, 0),
                ImageList = imageList,
            };
            contextMenu.KeyDown += (sender, args) => this.CloseContextMenuOnKeyPress(contextMenu, args);

            ToolStripMenuItem quickEditItem = new()
            {
                Text = Locale.EDITABLE_TAB_CONTROL_CONTEXT_MENU_QUICK_EDIT,
                ImageKey = IMAGE_QUICK_EDIT_KEY,
            };
            quickEditItem.Click += (sender, args) =>
            {
                var quickEditForm = new EditableTabControlQuickEditForm(this);
                var result = quickEditForm.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    var oldSelectedTab = this.SelectedTab;

                    foreach (var rowData in quickEditForm.RowsData)
                    {
                        var loopTabPage = rowData.TabPage;
                        this.RenameTab(loopTabPage, rowData.NameTextBox.Text);

                        if (rowData.Info.NeedsDeletition)
                        {
                            rowData.Info.NeedsDeletition = this.CloseTab(loopTabPage);
                        }
                    }

                    var pages = quickEditForm.RowsData
                                    .OrderBy(rd => rd.Info.Index)
                                    .Where(rd => !rd.Info.NeedsDeletition)
                                    .Select(rd => rd.TabPage)
                                    .ToArray();

                    this.TabPages.Clear();
                    this.TabPages.AddRange(pages);

                    if (oldSelectedTab != null && oldSelectedTab.Parent != null)
                    {
                        this.SelectedTab = oldSelectedTab;
                    }
                }
            };

            ToolStripMenuItem addNewItem = new()
            {
                Text = Locale.EDITABLE_TAB_CONTROL_CONTEXT_MENU_ADD_FIVE_TABS,
                ImageKey = IMAGE_ADD_KEY
            };
            addNewItem.Click += (sender, args) =>
            {
                for (int x = 0; x < 5; x++)
                {
                    this.AddTab();
                }
            };

            ToolStripMenuItem closeItem = new()
            {
                Text = Locale.EDITABLE_TAB_CONTROL_CONTEXT_MENU_CLOSE,
                ImageKey = IMAGE_CLOSE_KEY,
            };
            closeItem.Click += (sender, args) => this.CloseTab(tabPage);

            Label editNameLabel = new()
            {
                Text = Locale.EDITABLE_TAB_CONTROL_CONTEXT_MENU_EDIT_NAME,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new(0),
                Margin = new(0, 0, 8, 0),
            };

            TextBox editNameTextBox = new()
            {
                Text = tabPage.Text,
                TextAlign = HorizontalAlignment.Left,
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill,
                Padding = new(0),
                Margin = new(0),
                MinimumSize = new(200, 0)
            };
            editNameTextBox.KeyDown += (sender, args) => this.CloseContextMenuOnKeyPress(contextMenu, args);

            FlowLayoutPanel panel = new()
            {
                AutoSize = true,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Controls = { editNameLabel, editNameTextBox },
                Padding = new(0),
            };

            ToolStripControlHost editNameHost = new(panel)
            {
                BackColor = Color.Transparent,
                ImageKey = IMAGE_EDIT_NAME_KEY, //Seems to not work :(
            };


            contextMenu.Items.Add(editNameHost);
            contextMenu.Items.Add(closeItem);
            contextMenu.Items.Add(new ToolStripSeparator() { Margin = new(0), Padding = new(0) });
            contextMenu.Items.Add(quickEditItem);
            contextMenu.Items.Add(addNewItem);

            contextMenu.Closed += (sender, args) =>
            {
                RenameTab(tabPage, editNameTextBox.Text);
            };

            contextMenu.Show(Cursor.Position);
        }

        private void CloseContextMenuOnKeyPress(ContextMenuStrip contextMenu, KeyEventArgs args)
        {
            if (args.KeyData == Keys.Escape || args.KeyData == Keys.Enter)
            {
                contextMenu.Close();
            }
        }


        protected override void OnKeyDown(KeyEventArgs args)
        {
            base.OnKeyDown(args);
        }

        public void AddTab()
        {
            var addedTabPage = new EditableNormalTabPage();

            var eventArgs = new EditableTabPreAddEventArgs(addedTabPage);
            TabPreAdded(this, eventArgs);
            if (!eventArgs.Cancel)
            {
                this.TabPages.Add(addedTabPage);
            }
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

        public bool CloseTab(TabPage tabPage, bool useGlobalConfirmBeforeClosing = true)
        {
            if (tabPage is EditableNewTabPage)
            {
                return false;
            }

            bool canceldHalted = true;
            if (useGlobalConfirmBeforeClosing && this.RequireConfirmationBeforeClosing)
            {
                var result = InformationBox.Show($"Are you sure you want to close {tabPage.Text}?", buttons: InformationBoxButtons.YesNo);
                if (result == InformationBoxResult.Yes)
                {
                    canceldHalted = false;
                }
            }

            if (canceldHalted)
            {
                return false;
            }

            var index = this.TabPages.IndexOf(tabPage);
            if (index < 0)
            {
                return false;
            }

            var args = new EditableTabPreRemoveEventArgs(tabPage, index);
            this.TabPreRemoved(this, args);
            if (!args.Cancel)
            {
                this.TabPages.Remove(tabPage);
                return true;
            }

            return false;
        }
    }

    public class EditableNormalTabPage : TabPage
    {
        public EditableNormalTabPage()
        {
            this.DoubleBuffered = true;
            this.BorderStyle = BorderStyle.None;

            this.Padding = this.Margin = Padding.Empty;
        }

        protected override void OnPaint(PaintEventArgs e) { }

        protected override void OnPaintBackground(PaintEventArgs e) { }
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