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
        }

        private class Section(MacroSection macroSection, string name, string tooltip)
        {
            public MacroSection MacroSection { get; init;  } = macroSection;
            public string Name { get; init; } = name;
            public string Description { get; init; } = tooltip;
            public List<SettingsValue> ValueList { get; init; } = [];

            public TableLayoutPanel? Panel { get; set; } = null;
            public int PanelTop { get; set; } = 0;

            public override string ToString() => Name;
        }

        private class ListViewItemTag
        {
            public bool IsMacroSection { get; set; } = false;
            public object? Value { get; set; } = null;
        }

        private readonly TableLayoutPanel mainPanel;
        private readonly ListViewThatKeepsSelection leftSelectSectionListView;
        private readonly TableLayoutPanel rightSettingsPanel;

        private readonly SettingsBindings bindings;
        private readonly List<MacroSection> macroSectionList;
        private readonly List<Section> sectionList;

        private MacroSection? loadedMacroSection;

        public SettingsForm(SettingsBindings bindings)
        {
            InitializeComponent();

            this.bindings = bindings;
            this.macroSectionList = [];
            this.sectionList = [];

            this.mainPanel = new();
            this.leftSelectSectionListView = new();
            this.rightSettingsPanel = new();


            Utils.SetDoubleBuffered(this.mainPanel);
            Utils.SetDoubleBuffered(this.leftSelectSectionListView);
            Utils.SetDoubleBuffered(this.rightSettingsPanel);

            Init();
        }

        private void Init()
        {
            this.mainPanel.Dock = DockStyle.Fill;
            this.mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.mainPanel.AutoSize = true;

            this.mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SettingsConstants.SECTIONS_LIST_VIEW_WIDTH));
            this.mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            this.mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            this.InitLeftListView("Sections");
            this.InitRightPanel();

            this.mainPanel.Controls.Add(this.leftSelectSectionListView, 0, 0);

            //This allow scrollability avoiding the problem with the vertical scrollbar appearing.
            //Caused resizing children causing horizontal scroll to appear
            ScrollableControl scrollableControl = new()
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                AutoScrollMinSize = new Size(100, 100),
                Controls = { this.rightSettingsPanel }
            };
            Utils.SetDoubleBuffered(scrollableControl);
            this.mainPanel.Controls.Add(scrollableControl, 1, 0);

            this.Controls.Add(this.mainPanel);
        }

        private void InitLeftListView(string columnName)
        {
            this.leftSelectSectionListView.Font = SettingsConstants.SECTIONS_LEFT_FONT;
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
            this.leftSelectSectionListView.DrawColumnHeader += this.ListView_DrawColumnHeader;
            this.leftSelectSectionListView.DrawItem += this.ListView_DrawItem;
            //this.leftSelectSectionListView.DrawSubItem += this.ListView_DrawSubItem;

            this.leftSelectSectionListView.ItemSelectionChanged += (sender, args) => LoadFromItem(args.Item);
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

        public void ParseBindings()
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
                        Tag = new ListViewItemTag() { IsMacroSection = true, Value = macroSection }
                    };
                    this.leftSelectSectionListView.Items.Add(macroSectionListViewItem);
                }

                Section section = new(macroSection, sectionBinding.Name, sectionBinding.Description);
                section.ValueList.AddRange(sectionBinding.ValueList);
                this.sectionList.Add(section);

                ListViewItem listViewItem = new()
                {
                    Text = section.Name,
                    ToolTipText = section.Description,
                    Tag = new ListViewItemTag() { IsMacroSection = false, Value = section }
                };
                this.leftSelectSectionListView.Items.Add(listViewItem);
 
                macroSection.Sections.Add(section);
            }

            this.LoadFromItem(this.leftSelectSectionListView.Items[0]);
        }

        private void LoadFromItem(ListViewItem? item)
        {
            if (item?.Tag is ListViewItemTag tag)
            {
                if (tag.Value is Section section)
                {
                    if(section.MacroSection != this.loadedMacroSection)
                    {
                        this.LoadMacroSection(section.MacroSection);
                    }

                    if(section.Panel != null)
                    {
                        this.rightSettingsPanel.ScrollControlIntoView(section.Panel);
                        //this.rightSettingsPanel.AutoScrollPosition = new Point(0, section.PanelTop);
                    }
                }
                else if (tag.Value is MacroSection macroSection)
                {
                    this.LoadMacroSection(macroSection);
                }
            }
        }

        private void LoadMacroSection(MacroSection macroSection)
        {
            this.loadedMacroSection = macroSection;

            this.rightSettingsPanel.Controls.Clear();

            foreach (var section in macroSection.Sections)
            {
                TableLayoutPanel panel = new()
                {
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
                Utils.SetDoubleBuffered(panel);

                TableLayoutPanel valuesPanel = new()
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Dock = DockStyle.Fill,
                    ColumnStyles = { new ColumnStyle(SizeType.AutoSize) },
                    Padding = Padding.Empty,
                    Margin = Padding.Empty,
                };

                foreach (var value in section.ValueList)
                {
                    var valueControl = this.CreateValuePanel(value);

                    valuesPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    valuesPanel.Controls.Add(valueControl);
                }

                var sectionPanel = this.CreateSectionLeftPanel(section);
                //This is just to view bettere the division
                var sectionBorderLabel = new Label()
                {
                    Text = "",
                    Dock = DockStyle.Fill,
                    BackColor = Color.DarkBlue,
                    Padding = Padding.Empty,
                    Margin = new Padding(0, 0, (int)(SettingsConstants.SECTIONS_BORDER_COLUMN_SIZE / 2f), 0),
                };

                panel.Controls.Add(sectionPanel, 0, 0);
                panel.Controls.Add(sectionBorderLabel, 1, 0);
                panel.Controls.Add(valuesPanel, 2, 0);

                this.rightSettingsPanel.RowStyles.Add(new(SizeType.AutoSize));
                this.rightSettingsPanel.Controls.Add(panel);

                section.Panel = panel;

                panel.VisibleChanged += (sender, args) =>
                {
                    panel.AutoScrollPosition = new Point(0, panel.Top);
                    section.PanelTop = panel.Top;
                };
            }
        }

        private TableLayoutPanel CreateSectionLeftPanel(Section section)
        {
            var groupNameTextSize = TextRenderer.MeasureText(section.Name, SettingsConstants.SECTIONS_RIGHT_NAME_FONT, new Size((int)SettingsConstants.SECTIONS_NAME_COLUMN_SIZE, 0), TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak);
            var groupDescriptionTextSize = TextRenderer.MeasureText(section.Description, SettingsConstants.DESCRIPTIONS_FONT, new Size((int)SettingsConstants.SECTIONS_NAME_COLUMN_SIZE, 0), TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak);

            TableLayoutPanel panel = new()
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                ColumnStyles = { new ColumnStyle(SizeType.AutoSize) },
                RowStyles = { new RowStyle(SizeType.Absolute, groupNameTextSize.Height + 4), new RowStyle(SizeType.Absolute, groupDescriptionTextSize.Height + 4) },
                Margin = new Padding(0, 0, 5, 0),
                Padding = Padding.Empty,
            };

            Label nameLabel = new()
            {
                Text = section.Name,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Form.DefaultBackColor,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                Font = SettingsConstants.SECTIONS_RIGHT_NAME_FONT,
            };

            Label descriptionLabel = new()
            {
                Text = section.Description,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                AutoSize = false, //This is to allow the text to wrap
                TextAlign = ContentAlignment.TopLeft,
                BackColor = Form.DefaultBackColor,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                Font = SettingsConstants.DESCRIPTIONS_FONT,
            };

            panel.Controls.Add(nameLabel, 0, 0);
            panel.Controls.Add(descriptionLabel, 0, 1);

            return panel;
        }

        private TableLayoutPanel CreateValuePanel(SettingsValue value)
        {
            TableLayoutPanel panel = new()
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                ColumnStyles = { new ColumnStyle(SizeType.AutoSize) },
                RowStyles = { new RowStyle(SizeType.AutoSize) },
                Margin = Padding.Empty,
                Padding = Padding.Empty,
            };

            bool hasName = !string.IsNullOrEmpty(value.Name);
            bool hasDescription = !string.IsNullOrEmpty(value.Description);

            var rowCount = 0;
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
                    Font = SettingsConstants.VALUE_TITLE_FONT,
                    FlatStyle = FlatStyle.Standard,
                };

                panel.Controls.Add(nameLabel, 0, rowCount);
                rowCount++;

                panel.RowStyles.Add(new(SizeType.AutoSize));
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
                    Font = SettingsConstants.DESCRIPTIONS_FONT,
                    FlatStyle = FlatStyle.Standard,
                };

                panel.Controls.Add(nameLabel, 0, rowCount);
                rowCount++;

                panel.RowStyles.Add(new(SizeType.AutoSize));
            }

            var editor = SettingsEditor.ObtainFromValue(this, value);
            panel.Controls.Add(editor.GetControl(), 0, rowCount);

            return panel;
        }

        private void ListView_DrawColumnHeader(object? sender, DrawListViewColumnHeaderEventArgs e)
        {
            //e.DrawBackground();

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
                var backColor = item.Focused ? SettingsConstants.SECTIONS_SELECTED_ITEM_BACK_COLOR : Color.Transparent;

                var borderColor = item.Focused ? SettingsConstants.SECTIONS_SELECTED_ITEM_BORDER_COLOR : Color.Transparent;
                var borderWidth = SettingsConstants.SECTIONS_SELECTED_ITEM_BORDER_WIDTH;

                var rectsLeftPadding = 0;//(SECTIONS_LEFT_PADDING * 3) - 2;

                //BACKGROUND
                var backBounds = e.Bounds;
                backBounds = Rectangle.Inflate(backBounds, -rectsLeftPadding / 2, 0);
                backBounds.Offset(rectsLeftPadding / 2, 0);

                e.Graphics.FillRectangle(new SolidBrush(backColor), backBounds);

                //BORDER
                if (borderWidth > 0)
                {
                    var borderBound = Rectangle.Inflate(e.Bounds, -borderWidth - rectsLeftPadding / 2, -borderWidth);
                    borderBound.Offset(rectsLeftPadding / 2, 0);

                    var borderPen = new Pen(new SolidBrush(borderColor), borderWidth);

                    e.Graphics.DrawRectangle(borderPen, borderBound);
                }

                //DRAW TEXT

                var textBounds = e.Bounds;
                textBounds = Rectangle.Inflate(textBounds, tag.IsMacroSection ? 0 : -SettingsConstants.SECTIONS_LEFT_PADDING * 3, 0);
                TextRenderer.DrawText(e.Graphics, item.Text, this.leftSelectSectionListView.Font, textBounds, foreColor, TextFormatFlags.Left);
            }


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
