using InfoBox;
using System.Diagnostics.CodeAnalysis;
using TiaUtilities.Languages;
using TiaUtilities.Styles;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;
using static TiaUtilities.CustomControls.EditableTab.EditableTabControl;
using static TiaUtilities.CustomControls.EditableTab.EditableTabControlEvents;

namespace TiaUtilities.CustomControls.EditableTab
{
    public class EditableTabControl : TabControl, IMessageFilter
    {
        private record DragAndDropData(TabPage TabPage);

        private class TabMetadata
        {
            public required EventHandler TextChanged { get; init; }
            public required string OldName { get; set; }

            public bool RenamingInProgress { get; set; } = false;
        }

        private const int SELECTED_TAB_RECT_SIDE_PADDING = 10;
        private const int SELECTED_TAB_RECT_HEIGHT = 3;

        public event TabRemovedEventHandler TabRemoved = delegate { };
        public event TabAddedEventHandler TabAdded = delegate { };
        public event TabRenamedEventHandler TabRenamed = delegate { };

        public bool RequireConfirmationBeforeClosing { get; set; } = false;

        public EditableTabAddButton AddButton { get; init; }

        private readonly Dictionary<TabPage, TabMetadata> metadataDict;
        private Point? dragMouseDownPoint;

        private bool dragDropActive = false;

        public EditableTabControl() : base()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true); //This is needed for the backgroundPaint event to be called!
            this.DoubleBuffered = true;

            this.AddButton = new(owner: this);

            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.Padding = new(12, 5);
            this.Font = new(Font.SystemFontName, 9f, FontStyle.Italic);
            this.AllowDrop = true;

            this.metadataDict = [];
        }

        protected override void OnHandleCreated(EventArgs args)
        {  //This allows tab to be small 
            base.OnHandleCreated(args);
            Application.AddMessageFilter(this);

            DllImports.SendMessage(this.Handle, DllImports.TCM_SETMINTABWIDTH, IntPtr.Zero, 1); //wParam must be zero. lParam min width.
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            Application.RemoveMessageFilter(this);
        }

        protected override void OnPaint(PaintEventArgs e) { }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Rectangle clientRect = this.ClientRectangle;

            using Brush backBrush = new SolidBrush(SystemColors.Control);
            e.Graphics.FillRectangle(backBrush, clientRect);

            for (int i = 0; i < this.TabPages.Count; i++)
            {
                this.DrawTabCustom(e.Graphics, i);
            }

            this.AddButton.DrawAddButton(e.Graphics);
        }

        private void DrawTabCustom(Graphics g, int index)
        {
            var tabRect = this.GetTabRect(index);
            if (tabRect == Rectangle.Empty)
            {
                return;
            }

            var tabPage = this.TabPages[index];

            var isSelected = this.SelectedTab == tabPage;

            var selectedAndFocused = isSelected && this.Focused;
            var selectedOnly = isSelected && !this.Focused;

            var backColor = selectedAndFocused || selectedOnly ?
                StyleManager.EditableTabControl.FOCUSED_TAB_BACK :
                StyleManager.EditableTabControl.TAB_BACK;

            if (!backColor.IsEmpty && backColor.A > 0)
            {
                if (selectedOnly)
                {
                    using Pen backBorderPen = new(backColor, 2f)
                    {
                        Alignment = System.Drawing.Drawing2D.PenAlignment.Inset
                    };

                    GraphicsUtils.DrawRoundedRectangle(g, backBorderPen, tabRect, new(3, 0));
                }
                else
                {
                    using Brush backBrush = new SolidBrush(backColor);
                    GraphicsUtils.FillRoundedRectangle(g, backBrush, tabRect, new(3, 0));
                }
            }

            var foreColor = selectedAndFocused ?
                StyleManager.EditableTabControl.FOCUSED_TAB_FORE :
                StyleManager.EditableTabControl.TAB_FORE;

            if (!foreColor.IsEmpty && foreColor.A > 0)
            {
                var textRect = tabRect;
                textRect.Offset(0, -2);

                TextRenderer.DrawText(g,
                    tabPage.Text,
                    tabPage.Font,
                    textRect,
                    foreColor,
                    Color.Transparent,
                    TextFormatFlags.TextBoxControl | TextFormatFlags.WordEllipsis | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }


            var panelColor = StyleManager.EditableTabControl.PANEL_BORDER_COLOR;
            if(!panelColor.IsEmpty && panelColor.A > 0)
            {//Whole panel border. Not tab related
                var borderRect = this.DisplayRectangle;
                borderRect.Inflate(3, 2);

                using Pen panelBorderPen = new(StyleManager.EditableTabControl.PANEL_BORDER_COLOR, 3f);
                g.DrawRectangle(panelBorderPen, borderRect);
            }


        }

        protected override void OnDrawItem(DrawItemEventArgs e) { }

        public bool PreFilterMessage(ref Message m)
        {
            if (this.IsDisposed || !this.Visible)
            {
                return false;
            }

            return this.AddButton.PreFilterMessage(m);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            var horizontalScroll = ControlUtils.WncProcHScrollWheel(m);
            if (horizontalScroll != 0)
            {
                SelectNextTab(previous: (horizontalScroll < 0));
            }
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.KeyData == (Keys.Enter | Keys.Control))
            {
                this.HandleContextMenuOnKeyboard();
            }
            else if (e.KeyData == Keys.Delete)
            {
                var tab = this.SelectedTab;
                if (tab != null)
                {
                    this.CloseTabs([tab]);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs args)
        {
            base.OnMouseDown(args);

            var index = GetLocationTabRectIndex(args.Location);
            if (index == -1)
            {
                return;
            }

            var tabPage = this.TabPages[index];
            if (args.Button == MouseButtons.Left)
            {
                dragMouseDownPoint = new(args.X, args.Y);
            }
            else if (args.Button == MouseButtons.Right)
            {
                this.SelectedTab = tabPage;
                EditableTabControlContextMenuFactory.CreateContextMenu(this, tabPage).Show(Cursor.Position);
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

            this.SelectedTab = tabPage;
            EditableTabControlContextMenuFactory.CreateContextMenu(this, tabPage).Show(Cursor.Position);
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

            try
            {
                this.dragDropActive = true;

                var droppedTabPage = this.TabPages[tabRectIndex];

                var dataObj = args.Data?.GetData(typeof(EditableTabControl.DragAndDropData));
                if (dataObj is EditableTabControl.DragAndDropData dragAndDrop && dragAndDrop.TabPage != droppedTabPage)
                {
                    this.SuspendLayout();

                    int dragStartIndex = this.TabPages.IndexOf(dragAndDrop.TabPage);
                    int dragEndIndex = this.TabPages.IndexOf(droppedTabPage);

                    if (dragStartIndex >= 0 && dragStartIndex < this.TabCount &&
                        dragEndIndex >= 0 && dragEndIndex < this.TabCount &&
                        dragStartIndex != dragEndIndex)
                    {
                        var oldSelectedTab = this.SelectedTab;

                        var item = this.TabPages[dragStartIndex];
                        this.TabPages.RemoveAt(dragStartIndex);
                        this.TabPages.Insert(dragEndIndex, item);

                        this.SelectedTab = oldSelectedTab;
                    }

                    this.ResumeLayout(true);
                }

            }
            catch (Exception) { }
            finally
            {
                this.dragDropActive = false;
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

        private void HandleContextMenuOnKeyboard()
        {
            if (this.SelectedTab == null)
            {
                return;
            }

            var contextMenu = EditableTabControlContextMenuFactory.CreateContextMenu(this, this.SelectedTab);

            Point point = new(this.SelectedTab.Left, this.SelectedTab.Top);
            contextMenu.Show(this.SelectedTab.PointToScreen(point));
        }

        private TabPage? removedTabFromEvent;

        protected override void OnControlAdded(ControlEventArgs e)
        {
            if (this.dragDropActive)
            {
                return;
            }

            if (e.Control is TabPage tabPage)
            {
                if (this.removedTabFromEvent == tabPage)
                { //Also don't call ControlAdded event
                    return;
                }

                TabAddedEventArgs addedArgs = new(tabPage);
                TabAdded(this, addedArgs);
                if (addedArgs.Cancel)
                {
                    this.BeginInvoke(() => this.TabPages.Remove(tabPage));
                    return;
                }

                var metadata = this.GetMetadata(tabPage);
                tabPage.TextChanged += metadata.TextChanged;
            }

            base.OnControlAdded(e);
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            if (this.dragDropActive)
            {
                return;
            }

            if (e.Control is TabPage tabPage)
            {
                var indexOf = this.TabPages.IndexOf(tabPage);
                if (indexOf >= 0)
                {
                    TabRemovedEventArgs args = new(tabPage, indexOf);
                    this.TabRemoved(this, args);
                    if (args.Cancel)
                    {
                        this.BeginInvoke(() =>
                        {
                            this.removedTabFromEvent = tabPage;
                            this.TabPages.Insert(indexOf, tabPage);
                            this.removedTabFromEvent = null;
                        });
                        return;
                    }

                    if (this.RemoveMetadata(tabPage, out var metadata)) //In case is been removed before creating metadata, avoid creating new.
                    {
                        tabPage.TextChanged -= metadata.TextChanged;
                    }

                    tabPage.Dispose(); //Ooops was memory leaking before.
                }
            }

            base.OnControlRemoved(e);
        }

        public TabPage AddTab()
        {
            TabPage tabPage = new();

            this.SuspendLayout();
            this.TabPages.Add(tabPage);
            this.ResumeLayout();

            return ControlUtils.SetDoubleBuffered(tabPage);
        }

        public void AddTabs(int count = 1)
        {
            var pages = Enumerable.Range(0, count).Select(i => new TabPage()).Select(ControlUtils.SetDoubleBuffered);

            this.SuspendLayout();
            this.TabPages.AddRange([.. pages]);
            this.ResumeLayout();
        }

        public void InsertTabs(int index, int count = 1)
        {
            if (index < 0 || index >= this.TabCount)
            {
                return;
            }

            for (int x = 0; x < count; x++)
            {
                TabPage tabPage = new();
                ControlUtils.SetDoubleBuffered(tabPage);

                this.TabPages.Insert(index + 1, tabPage);
            }
        }

        public bool CloseTab(TabPage tabPage, bool forceClosing = false)
        {
            return this.CloseTabs([tabPage], forceClosing);
        }

        public bool CloseTabs(IEnumerable<TabPage> tabPages, bool forceClosing = false)
        {
            if (!tabPages.Any())
            {
                return false;
            }

            if (!forceClosing && this.RequireConfirmationBeforeClosing)
            {
                var names = String.Join(", ", tabPages.Select(t => t.Text));

                var result = InformationBox.Show(Locale.EDITABLE_TAB_CONTROL_DELETE_CONFIRM.Replace("{t}", names), buttons: InformationBoxButtons.YesNo);
                if (result != InformationBoxResult.Yes)
                {
                    return false;
                }
            }

            tabPages.ForEach(this.TabPages.Remove);
            return true;
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

        private TabMetadata GetMetadata(TabPage tabPage)
        {
            Validate.IsTrue(tabPage.Parent == this);
            Validate.IsTrue(this.TabPages.Contains(tabPage));

            if (!this.metadataDict.TryGetValue(tabPage, out TabMetadata? metadata))
            {
                metadata = new() { OldName = tabPage.Text, TextChanged = HandleTabTextChanged };
                this.metadataDict.Add(tabPage, metadata);
            }

            return metadata;
        }

        private bool RemoveMetadata(TabPage tabPage, [NotNullWhen(true)] out TabMetadata? metadata) => this.metadataDict.Remove(tabPage, out metadata);

        private void HandleTabTextChanged(object? sender, EventArgs args)
        {
            if (sender is not TabPage tabPage)
            {
                return;
            }

            var metadata = this.GetMetadata(tabPage);
            if (metadata.RenamingInProgress)
            {
                return;
            }

            metadata.RenamingInProgress = true;

            try
            {
                TabRenamedEventArgs renamedArgs = new(tabPage, tabPage.Text, metadata.OldName);
                TabRenamed(this, renamedArgs);
                if (!renamedArgs.Handled && !String.Equals(tabPage.Text, renamedArgs.NewName))
                {
                    tabPage.Text = renamedArgs.NewName;
                }

                metadata.OldName = tabPage.Text;
            }
            catch (Exception) { }

            metadata.RenamingInProgress = false;
        }

    }

}