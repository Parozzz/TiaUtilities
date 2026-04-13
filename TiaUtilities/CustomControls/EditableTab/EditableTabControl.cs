
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using InfoBox;
using System.Runtime.InteropServices;
using TiaUtilities.Languages;
using TiaUtilities.Resources;
using static TiaUtilities.CustomControls.EditableTab.EditableTabControlQuickEditForm;

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

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            var tabPage = this.TabPages[e.Index];
            var isNewTagPage = tabPage is EditableNewTabPage;

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
            if (!isNewTagPage)
            {
                e.Graphics.DrawString(title, font, textBrush, new PointF(textRect.X + 9, textRect.Y + textRect.Height / 2));

                if (this.SelectedTab == tabPage)
                {
                    var ellipseRect = this.GetTabRect(e.Index);
                    ellipseRect.Offset(2, 2);
                    ellipseRect.Width = 7;
                    ellipseRect.Height = 7;

                    e.Graphics.FillEllipse(Brushes.DarkSeaGreen, ellipseRect);

                    using var activePageEllipseBorderPen = new Pen(Brushes.Gray, 1f);
                    e.Graphics.DrawEllipse(activePageEllipseBorderPen, ellipseRect);
                }
            }
            else
            {
                e.Graphics.DrawString(title, font, textBrush, new PointF(textRect.X + this.Padding.X / 2 + 2, textRect.Y));
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
            this.SelectedTab = tabPage;

            if (tabPage is EditableNewTabPage)
            {
                AddTab();
            }

            if (e.Button == MouseButtons.Right)
            {
                this.HandleContextMenu(tabPage, index);
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
            this.SelectedTab = tabPage;

            this.HandleContextMenu(tabPage, index);
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

        private void HandleContextMenu(TabPage tabPage, int index)
        {
            var contextMenu = new ContextMenuStrip()
            {
                MinimumSize = new(300, 0)
            };
            contextMenu.KeyDown += (sender, args) => this.CloseContextMenuOnKeyPress(contextMenu, args);

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

            ToolStripMenuItem closeItem = new()
            {
                Text = Locale.EDITABLE_TAB_CONTROL_CONTEXT_MENU_CLOSE,
                Image = ImageResources.CLOSE_193002_FF001C,
            };
            closeItem.Click += (sender, args) => this.CloseTab(tabPage);

            ToolStripMenuItem quickEditItem = new()
            {
                Text = Locale.EDITABLE_TAB_CONTROL_CONTEXT_MENU_QUICK_EDIT,
                Image = ImageResources.EDIT_562275,
            };
            quickEditItem.Click += (sender, args) => 
            {
                var quickEditForm = new EditableTabControlQuickEditForm(this);
                var result = quickEditForm.ShowDialog(this); 
                if(result == DialogResult.OK)
                {
                    var oldSelectedTab = this.SelectedTab;

                    foreach(var rowData in quickEditForm.RowsData)
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

                    if(oldSelectedTab != null && oldSelectedTab.Parent != null)
                    {
                        this.SelectedTab = oldSelectedTab;
                    }
                }
            };

            contextMenu.Items.Add(quickEditItem);
            contextMenu.Items.Add(new ToolStripSeparator() { Margin = new(0), Padding = new(0) });
            contextMenu.Items.Add(new ToolStripControlHost(panel) { BackColor = Color.Transparent });
            contextMenu.Items.Add(closeItem);

            contextMenu.Show(Cursor.Position);

            contextMenu.Closed += (sender, args) =>
            {
                RenameTab(tabPage, editNameTextBox.Text);
            };
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
            var addedTabPage = new TabPage();

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
            if (useGlobalConfirmBeforeClosing &&  this.RequireConfirmationBeforeClosing)
            {
                var result = InformationBox.Show($"Are you sure you want to close {tabPage.Text}?", buttons: InformationBoxButtons.YesNo);
                if (result == InformationBoxResult.Yes)
                {
                    canceldHalted = false;
                }
            }

            if(canceldHalted)
            {
                return false;
            }

            var index = this.TabPages.IndexOf(tabPage);
            if(index < 0)
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

    public class EditableNewTabPage : TabPage
    {
        public EditableNewTabPage() : base()
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

/*
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