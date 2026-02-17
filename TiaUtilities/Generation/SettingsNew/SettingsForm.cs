using System.Diagnostics;
using System.Runtime.InteropServices;
using TiaUtilities.Configuration;
using TiaUtilities.Generation.IO;
using TiaUtilities.Generation.SettingsNew.Editors;
using TiaUtilities.Languages;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.Generation.SettingsNew
{

    public partial class SettingsForm : Form
    {
        private class MacroSection(string name)
        {
            public string Name { get; init; } = name;
            public override string ToString() => Name;
            public List<Section> Sections { get; init; } = [];

            public Label? Label { get; set; } = null;
        }

        private class Section(MacroSection macroSection, string name, string tooltip)
        {
            public MacroSection MacroSection { get; init; } = macroSection;
            public string Name { get; init; } = name;
            public string Description { get; init; } = tooltip;
            public List<SettingsValue> ValueList { get; init; } = [];

            public ListViewItem? ListItem { get; set; } = null;
            public TableLayoutPanel? Panel { get; set; } = null;
            public float VisiblePercentage { get; set; } = 0f;

            public override string ToString() => $"{Name}, {VisiblePercentage}";
        }

        private class ListViewItemTag() { }

        private class ListViewItemSectionTag(Section section) : ListViewItemTag
        {
            public bool IsSectionVisible { get; set; } = false;
            public Section Section { get; init; } = section; //Section or MacroSection
        }

        private class ListViewItemMacroSectionTag(MacroSection macroSection) : ListViewItemTag
        {
            public MacroSection MacroSection { get; init; } = macroSection;
        }

        private readonly TableLayoutPanel mainPanel;
        private readonly ListViewThatKeepsSelection leftSelectSectionListView;
        private readonly TableLayoutPanel rightSettingsPanel;

        private readonly SettingsBindings bindings;
        private readonly List<MacroSection> macroSectionList;


        public SettingsForm(SettingsBindings bindings)
        {
            InitializeComponent();

            var millisStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            this.bindings = bindings;
            this.macroSectionList = [];

            this.mainPanel = new();
            this.leftSelectSectionListView = new();
            this.rightSettingsPanel = new();

            Init();
            ParseBindings();

            this.Shown += (sender, args) =>
            {
                var shownMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                Debug.WriteLine($"Shown Total Time: {shownMillis - millisStart}ms");
                var _ = "";
            };
        }

        private void Init()
        {
            this.mainPanel.Dock = DockStyle.Fill;
            this.mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.mainPanel.AutoSize = true;

            this.mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SettingsConstants.SECTIONS_LIST_VIEW_WIDTH));
            this.mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            this.mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            this.InitLeftListView();
            this.InitRightPanel();

            this.mainPanel.Controls.Add(this.leftSelectSectionListView, 0, 0);
            this.mainPanel.Controls.Add(this.WrapRightPanelInScrollable(this.rightSettingsPanel), 1, 0);

            this.Controls.Add(this.mainPanel);
        }

        private ScrollableControl WrapRightPanelInScrollable(TableLayoutPanel rightPanel)
        {
            //This allow scrollability avoiding the problem with the vertical scrollbar appearing.
            //Caused resizing children causing horizontal scroll to appear

            ScrollableControl scrollableControl = new()
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                AutoScrollMinSize = new Size(100, 100),
                Controls = { rightPanel }
            };

            scrollableControl.Scroll += (sender, args) => UpdateSectionVisiblePercentage(scrollableControl, args.NewValue);
            scrollableControl.MouseWheel += (sender, args) => UpdateSectionVisiblePercentage(scrollableControl);
            scrollableControl.Resize += (sender, args) => UpdateSectionVisiblePercentage(scrollableControl);
            scrollableControl.Layout += (sender, args) => UpdateSectionVisiblePercentage(scrollableControl);

            return scrollableControl;
        }

        private void UpdateSectionVisiblePercentage(ScrollableControl scrollableControl, int scrollPositionNew = -1)
        {
            this.leftSelectSectionListView.SuspendLayout();

            var scrollableVisibleTop = scrollPositionNew >= 0 ? scrollPositionNew : scrollableControl.VerticalScroll.Value; //Effectively the scroll position
            var scrollableVisibleBottom = scrollableVisibleTop + scrollableControl.Height;

            foreach (var macroSection in this.macroSectionList)
            {
                foreach (var section in macroSection.Sections)
                {
                    var panel = section.Panel;
                    if (panel == null)
                    {
                        continue;
                    }

                    var top = panel.Top;
                    var bottom = panel.Bottom;

                    var height = (float)panel.Height;
                    if (bottom <= scrollableVisibleTop || top >= scrollableVisibleBottom)
                    {
                        section.VisiblePercentage = 0;
                    }
                    else
                    {
                        var minBottom = Math.Min(bottom, scrollableVisibleBottom);
                        var maxTop = Math.Max(scrollableVisibleTop, top);

                        section.VisiblePercentage = (minBottom - maxTop) / height;
                    }

                    var listItem = section.ListItem;
                    if (listItem != null && listItem.Tag is ListViewItemSectionTag sectionTag)
                    {
                        var oldVisible = sectionTag.IsSectionVisible;
                        sectionTag.IsSectionVisible = (section.VisiblePercentage > 0.3f);

                        if (oldVisible != sectionTag.IsSectionVisible)
                        {
                            listItem.Text = listItem.Text; //This force a redraw of the item!
                        }
                    }
                }
            }

            this.leftSelectSectionListView.ResumeLayout(true);
        }

        private void InitLeftListView()
        {
            this.leftSelectSectionListView.Font = SettingsConstants.LIST_LEFT_FONT;
            this.leftSelectSectionListView.View = View.Tile; //This view shows group 
            this.leftSelectSectionListView.Dock = DockStyle.Fill;
            this.leftSelectSectionListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.leftSelectSectionListView.Activation = ItemActivation.OneClick;
            this.leftSelectSectionListView.BorderStyle = BorderStyle.None;
            this.leftSelectSectionListView.LabelEdit = false;
            this.leftSelectSectionListView.AllowColumnReorder = false;
            this.leftSelectSectionListView.CheckBoxes = false;
            this.leftSelectSectionListView.FullRowSelect = true;
            this.leftSelectSectionListView.GridLines = false;
            this.leftSelectSectionListView.Sorting = SortOrder.None;
            this.leftSelectSectionListView.Scrollable = false;
            this.leftSelectSectionListView.MaximumSize = new Size(SettingsConstants.SECTIONS_LIST_VIEW_WIDTH, 0);
            this.leftSelectSectionListView.MinimumSize = new Size(30, 80);
            this.leftSelectSectionListView.TileSize = new Size(SettingsConstants.SECTIONS_LIST_VIEW_WIDTH, SettingsConstants.SECTIONS_LIST_TILE_HEIGHT);

            this.leftSelectSectionListView.OwnerDraw = true;
            this.leftSelectSectionListView.DrawItem += this.ListView_DrawItem;
            //this.leftSelectSectionListView.DrawColumnHeader += this.ListView_DrawColumnHeader;
            //this.leftSelectSectionListView.DrawSubItem += this.ListView_DrawSubItem;


            this.leftSelectSectionListView.ItemSelectionChanged += (sender, args) =>
            {
                if (this.rightSettingsPanel.Parent is ScrollableControl scrollableControl)
                {
                    if (args.Item?.Tag is ListViewItemSectionTag sectionTag && sectionTag.Section.Panel != null)
                    {
                        scrollableControl.VerticalScroll.Value = sectionTag.Section.Panel.Top;
                    }
                    else if (args.Item?.Tag is ListViewItemMacroSectionTag macroSectionTag && macroSectionTag.MacroSection.Label != null)
                    {
                        scrollableControl.VerticalScroll.Value = macroSectionTag.MacroSection.Label.Top;
                    }

                    scrollableControl.PerformLayout(); //This immediately updates the control since the Function below uses client side values for calculation!
                    this.UpdateSectionVisiblePercentage(scrollableControl);
                }
            };
        }

        private void InitRightPanel()
        {
            //SystemInformation.VerticalScrollBarWidth
            this.rightSettingsPanel.AutoSize = true;
            this.rightSettingsPanel.AutoScroll = true;
            //Anchors are needed for the ScrollableControl above!
            this.rightSettingsPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            this.rightSettingsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.rightSettingsPanel.Margin = Padding.Empty;
            this.rightSettingsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        }

        private void ParseBindings()
        {

            MacroSection? macroSection = null;
            foreach (var sectionBinding in this.bindings.SectionList)
            {
                if (macroSection == null || macroSection.Name != sectionBinding.MacroSectionBinding.Name)
                {
                    macroSection = new(sectionBinding.MacroSectionBinding.Name);
                    this.macroSectionList.Add(macroSection);

                    ListViewItem macroSectionListViewItem = new()
                    {
                        Text = macroSection.Name,
                        Tag = new ListViewItemMacroSectionTag(macroSection)
                    };
                    this.leftSelectSectionListView.Items.Add(macroSectionListViewItem);
                }

                Section section = new(macroSection, sectionBinding.Name, sectionBinding.Description);
                macroSection.Sections.Add(section);

                section.ValueList.AddRange(sectionBinding.ValueList);

                ListViewItem listViewItem = new()
                {
                    Text = section.Name,
                    ToolTipText = section.Description,
                    Tag = new ListViewItemSectionTag(section)
                };
                this.leftSelectSectionListView.Items.Add(listViewItem);

                section.ListItem = listViewItem;
            }
            
            LoadMacroSections();
        }

        private void LoadMacroSections()
        {
            this.rightSettingsPanel.Controls.Clear();
            foreach (var macroSection in macroSectionList)
            {
                Label macroSectioNameLabel = new()
                {
                    Text = macroSection.Name,
                    Dock = DockStyle.Fill,
                    BorderStyle = BorderStyle.None,
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Form.DefaultBackColor,
                    Padding = Padding.Empty,
                    Margin = new Padding(0, 0, 0, 10),
                    Font = SettingsConstants.MACROSECTION_NAME_LABEL_FONT,
                };

                this.rightSettingsPanel.RowStyles.Add(new(SizeType.AutoSize));
                this.rightSettingsPanel.Controls.Add(macroSectioNameLabel);
                
                macroSection.Label = macroSectioNameLabel;
                
                foreach (var section in macroSection.Sections)
                {
                    TableLayoutPanel sectionPanel = new()
                    {
                        //AutoScroll = true,
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink,
                        Dock = DockStyle.Fill,
                        ColumnStyles = {
                            new(SizeType.Absolute, SettingsConstants.SECTIONS_NAME_COLUMN_SIZE),
                            new(SizeType.Absolute, SettingsConstants.SECTIONS_BORDER_COLUMN_SIZE),
                            new(SizeType.AutoSize)
                        },
                        RowStyles = { new(SizeType.AutoSize) },
                        Padding = Padding.Empty,
                        Margin = new Padding(0, 0, 0, (int)SettingsConstants.SECTIONS_SEPARATION),
                    };

                    Label sectionNameLabel = new()
                    {
                        Text = section.Name,
                        Dock = DockStyle.Fill,
                        BorderStyle = BorderStyle.None,
                        AutoSize = false,
                        TextAlign = ContentAlignment.TopCenter,
                        BackColor = Form.DefaultBackColor,
                        Padding = Padding.Empty,
                        Margin = Padding.Empty,
                        Font = SettingsConstants.SECTION_NAME_LABEL_FONT,
                    };

                    TableLayoutPanel valuesPanel = new()
                    {
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink,
                        Dock = DockStyle.Fill,
                        ColumnStyles = { new ColumnStyle(SizeType.AutoSize) },
                        Padding = Padding.Empty,
                        Margin = Padding.Empty,
                    };

                    int rowCount = 0;
                    foreach (var value in section.ValueList)
                    {
                        this.AppendToValuePanel(valuesPanel, value, ref rowCount);
                    }

                    //This is just to view bettere the division
                    var sectionBorderLabel = new Label()
                    {
                        Text = "",
                        Dock = DockStyle.Fill,
                        BackColor = Color.DarkBlue,
                        Padding = Padding.Empty,
                        Margin = new Padding(0, 0, (int)(SettingsConstants.SECTIONS_BORDER_COLUMN_SIZE / 2f), 0),
                    };

                    sectionPanel.Controls.Add(sectionNameLabel, 0, 0);
                    sectionPanel.Controls.Add(sectionBorderLabel, 1, 0);
                    sectionPanel.Controls.Add(valuesPanel, 2, 0);

                    this.rightSettingsPanel.RowStyles.Add(new(SizeType.AutoSize));
                    this.rightSettingsPanel.Controls.Add(sectionPanel);

                    section.Panel = sectionPanel;
                }
            }
        }
        private void AppendToValuePanel(TableLayoutPanel panel, SettingsValue value, ref int rowCount)
        {
            bool hasName = !string.IsNullOrEmpty(value.Name);
            bool hasDescription = !string.IsNullOrEmpty(value.Description);

            if (hasName)
            {
                Label nameLabel = new()
                {
                    Text = value.Name,
                    AutoSize = true,
                    Dock = DockStyle.Fill,
                    BorderStyle = BorderStyle.None,
                    UseCompatibleTextRendering = true,
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = Color.Transparent,
                    Margin = hasDescription ? Padding.Empty : new(0, 0, 0, 3),
                    Padding = Padding.Empty,
                    Font = SettingsConstants.VALUE_NAME_LABEL_FONT,
                    FlatStyle = FlatStyle.Standard,
                };

                panel.RowStyles.Add(new(SizeType.AutoSize));

                panel.Controls.Add(nameLabel, 0, rowCount);
                rowCount++;
            }

            if (hasDescription)
            {
                Label nameLabel = new()
                {
                    Text = value.Description,
                    AutoSize = false, //This is to allow the text to wrap
                    Dock = DockStyle.Fill,
                    BorderStyle = BorderStyle.None,
                    UseCompatibleTextRendering = true,
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = Color.Transparent,
                    Margin = new Padding(0, 0, 0, 3),
                    Padding = Padding.Empty,
                    Font = SettingsConstants.DESCRIPTION_LABEL_FONT,
                    FlatStyle = FlatStyle.Standard,
                };

                panel.RowStyles.Add(new(SizeType.AutoSize));

                panel.Controls.Add(nameLabel, 0, rowCount);
                rowCount++;
            }

            var editor = SettingsEditor.ObtainFromValue(this, value);

            panel.RowStyles.Add(new(SizeType.AutoSize));
            panel.Controls.Add(editor.GetControl(), 0, rowCount);
            rowCount++;

            panel.RowStyles.Add(new(SizeType.Absolute, SettingsConstants.SECTION_VALUE_SEPERATION));
            rowCount++;
        }

        private void ListView_DrawItem(object? sender, DrawListViewItemEventArgs e)
        {
            var item = e.Item;
            if (item == null)
            {
                return;
            }

            if (item.Tag is ListViewItemTag tag)
            {
                var foreColor = SettingsConstants.SECTIONS_ITEM_FORE_COLOR;
                var backColor = tag is ListViewItemSectionTag sectionTag && sectionTag.IsSectionVisible ? SettingsConstants.SECTIONS_SELECTED_ITEM_BACK_COLOR : Color.Transparent;

                var rectsLeftPadding = 0;//(SECTIONS_LEFT_PADDING * 3) - 2;

                //BACKGROUND
                var backBounds = e.Bounds;
                backBounds = Rectangle.Inflate(backBounds, -rectsLeftPadding / 2, 0);
                backBounds.Offset(rectsLeftPadding / 2, 0);

                e.Graphics.FillRectangle(new SolidBrush(backColor), backBounds);

                //DRAW TEXT
                var textBounds = e.Bounds;
                textBounds = Rectangle.Inflate(textBounds, tag is ListViewItemMacroSectionTag ? 0 : -SettingsConstants.SECTIONS_LEFT_PADDING * 3, 0);
                TextRenderer.DrawText(e.Graphics, item.Text, this.leftSelectSectionListView.Font, textBounds, foreColor, TextFormatFlags.Left);
            }


        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                switch (keyData)
                {
                    case Keys.Escape:
                        this.Close();
                        return true; //Return required otherwise will write the letter.
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

        class ListViewThatKeepsSelection : ListView
        {
            private const int SETCURSOR = 0x0020;
            private const int LBUTTONDOWN = 0x0201;
            private const int MBUTTONDBLCLK = 0x0209;

            public ListViewThatKeepsSelection()
            {
                this.DoubleBuffered = true;
            }

            protected override void WndProc(ref Message m)
            {
                //https://www.pinvoke.net/default.aspx/enums/windowsmessages.html
                // Suppress mouse messages that are OUTSIDE of the items area
                /*
                    LBUTTONDOWN = 0x0201,
                    LBUTTONUP = 0x0202,
                    LBUTTONDBLCLK = 0x0203,
                    RBUTTONDOWN = 0x0204,
                    RBUTTONUP = 0x0205,
                    RBUTTONDBLCLK = 0x0206,
                    MBUTTONDOWN = 0x0207,
                    MBUTTONUP = 0x0208,
                    MBUTTONDBLCLK = 0x0209, 
                */
                if (m.Msg >= LBUTTONDOWN && m.Msg <= MBUTTONDBLCLK)
                {
                    Point pos = new(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);

                    var hit = this.HitTest(pos);
                    switch (hit.Location)
                    {
                        case ListViewHitTestLocations.AboveClientArea:
                        case ListViewHitTestLocations.BelowClientArea:
                        case ListViewHitTestLocations.LeftOfClientArea:
                        case ListViewHitTestLocations.RightOfClientArea:
                        case ListViewHitTestLocations.None:
                            return;

                    }
                }

                base.WndProc(ref m);
            }

            public bool IsVerticalScrollbarVisible()
            {
                var delta = (this.Width - this.ClientSize.Width);
                return this.BorderStyle switch
                {
                    BorderStyle.None => (delta > 0),
                    BorderStyle.FixedSingle => (delta > 2),
                    BorderStyle.Fixed3D => (delta > 4),
                    _ => throw new NotImplementedException(),
                };
            }

        }

    }
}

/*

        private void ListView_DrawColumnHeader(object? sender, DrawListViewColumnHeaderEventArgs e)
        {
            var header = e.Header;

            HorizontalAlignment hAlign = header?.TextAlign ?? HorizontalAlignment.Left;
            TextFormatFlags flags = (hAlign == HorizontalAlignment.Left) ? TextFormatFlags.Left :
                                    ((hAlign == HorizontalAlignment.Center) ? TextFormatFlags.HorizontalCenter :
                                     TextFormatFlags.Right);
            flags |= TextFormatFlags.WordEllipsis;

            Rectangle newBounds = Rectangle.Inflate(e.Bounds, -SettingsConstants.SECTIONS_LEFT_PADDING, 0);
            TextRenderer.DrawText(e.Graphics, header?.Text, this.leftSelectSectionListView.Font, newBounds, Color.Black, flags);
        }

        private void ListView_DrawSubItem(object? sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

*/