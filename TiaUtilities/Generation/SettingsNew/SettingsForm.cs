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
        private class Section(string name, string tooltip)
        {
            public string Name { get; init; } = name;
            public string Tooltip { get; init; } = tooltip;
            public List<Group> Groups { get; init; } = [];

            public Section(SettingsSectionBinding binding) : this(binding.Name, binding.Tooltip)
            {

            }

            public override string ToString()
            {
                return Name;
            }
        }

        private class Group(string name, string description)
        {
            public static Group Empty { get => new("", ""); }

            public string Name { get; init; } = name;
            public string Description { get; init; } = description;
            public List<SettingsValue> Values { get; init; } = [];

            public Group(SettingsGroupBinding binding) : this(binding.Name, binding.Description)
            {

            }

            public bool IsEmpty()
            {
                return string.IsNullOrEmpty(this.Name);
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private readonly TableLayoutPanel mainPanel;
        private readonly ListViewThatKeepsSelection leftSelectSectionListView;
        private readonly TableLayoutPanel rightSettingsPanel;

        private readonly SettingsBindings bindings;
        private readonly List<Section> sectionList;

        public SettingsForm(SettingsBindings bindings)
        {
            InitializeComponent();

            this.bindings = bindings;
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
            this.leftSelectSectionListView.Font = SettingsConstants.SECTIONS_FONT;
            this.leftSelectSectionListView.View = View.Details; //This view shows group 
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
            this.leftSelectSectionListView.MaximumSize = new Size((int)SettingsConstants.SECTIONS_LIST_VIEW_WIDTH, 0);
            this.leftSelectSectionListView.MinimumSize = new Size(30, 80);

            this.leftSelectSectionListView.ItemSelectionChanged += (sender, args) => LoadSection(args.Item?.Text);

            var columnHeader = this.leftSelectSectionListView.Columns.Add(columnName, -2, HorizontalAlignment.Left); //Width of -2 indicates auto-size.

            this.leftSelectSectionListView.OwnerDraw = true;
            this.leftSelectSectionListView.DrawColumnHeader += this.ListView_DrawColumnHeader;
            //this.listView.DrawItem += this.ListView_DrawItem;
            this.leftSelectSectionListView.DrawSubItem += this.ListView_DrawSubItem;
        }

        private void InitRightPanel()
        {
            //SystemInformation.VerticalScrollBarWidth
            this.rightSettingsPanel.AutoSize = true;
            //Anchors are needed for the ScrollableControl above!
            this.rightSettingsPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            this.rightSettingsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.rightSettingsPanel.Margin = Padding.Empty;
            this.rightSettingsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        }

        public void ParseBindings()
        {
            foreach (var sectionBinding in this.bindings.SectionBindings)
            {
                bool firstGroup = true;
                Group previousGroup = Group.Empty;

                Section section = new(sectionBinding.Name, sectionBinding.Tooltip);

                foreach (var value in this.bindings.ValueList)
                {
                    if (value.SectionBinding.Name != sectionBinding.Name)
                    {
                        continue;
                    }

                    Group nextGroup = previousGroup;

                    var groupBinding = value.GroupBinding;
                    if (groupBinding == null && !previousGroup.IsEmpty())
                    {
                        nextGroup = Group.Empty;
                    }
                    else if (groupBinding != null && groupBinding.Name != previousGroup.Name)
                    {
                        nextGroup = new(groupBinding);
                    }

                    if (firstGroup || previousGroup.Name != nextGroup.Name)
                    {
                        firstGroup = false;

                        previousGroup = nextGroup;
                        section.Groups.Add(nextGroup);
                    }

                    nextGroup.Values.Add(value);
                }


                ListViewItem listViewItem = new()
                {
                    Text = section.Name,
                    ToolTipText = section.Tooltip,
                };
                this.leftSelectSectionListView.Items.Add(listViewItem);

                this.sectionList.Add(section);
            }

            var items = this.leftSelectSectionListView.Items;

            var firstItem = items.Count > 0 ? items[0] : null;
            this.LoadSection(firstItem?.Text);
        }

        private void LoadSection(string? sectionName)
        {
            if (sectionName != null)
            {
                foreach (var section in sectionList)
                {
                    if (section.Name == sectionName)
                    {
                        this.LoadSection(section);
                    }
                }
            }
        }

        private void LoadSection(Section section)
        {
            this.rightSettingsPanel.Controls.Clear();
            foreach (var group in section.Groups)
            {
                TableLayoutPanel panel = new()
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Dock = DockStyle.Fill,
                    RowStyles = { new RowStyle(SizeType.AutoSize) },
                    Padding = Padding.Empty,
                    Margin = new Padding(0, 0, 0, (int) SettingsConstants.GROUP_SEPARATION),
                };
                Utils.SetDoubleBuffered(panel);

                TableLayoutPanel settingsRightPanel = new()
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Dock = DockStyle.Fill,
                    ColumnStyles = { new ColumnStyle(SizeType.AutoSize) },
                    Padding = Padding.Empty,
                    Margin = Padding.Empty,
                };

                foreach (var value in group.Values)
                {
                    var valueControl = this.CreateValuePanel(value);

                    settingsRightPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    settingsRightPanel.Controls.Add(valueControl);
                }

                int columIndex = 0;

                var groupLeftPanel = this.CreateGroupLeftPanel(group);
                if(groupLeftPanel != null)
                {
                    panel.ColumnStyles.Add(new(SizeType.Absolute, SettingsConstants.GROUP_NAME_COLUMN_SIZE));
                    panel.ColumnStyles.Add(new(SizeType.Absolute, SettingsConstants.GROUP_BORDER_COLUMN_SIZE));

                    //This is just to view bettere the division
                    var groupBorderRectLabel = new Label()
                    {
                        Text = "",
                        Dock = DockStyle.Fill,
                        BackColor = Color.DarkBlue,
                        Padding = Padding.Empty,
                        Margin = new Padding(0, 0, (int)(SettingsConstants.GROUP_BORDER_COLUMN_SIZE / 2f), 0),
                    };

                    panel.Controls.Add(groupLeftPanel, 0, 0);
                    panel.Controls.Add(groupBorderRectLabel, 1, 0);

                    columIndex = 2;
                }

                panel.ColumnStyles.Add(new(SizeType.AutoSize));
                panel.Controls.Add(settingsRightPanel, columIndex, 0);

                this.rightSettingsPanel.RowStyles.Add(new(SizeType.AutoSize));
                this.rightSettingsPanel.Controls.Add(panel);
            }
        }

        private TableLayoutPanel? CreateGroupLeftPanel(Group group)
        {
            if (group.IsEmpty())
            {
                return null;
            }

            var groupNameTextSize = TextRenderer.MeasureText(group.Name, SettingsConstants.SETTINGS_GROUP_NAME_FONT, new Size((int)SettingsConstants.GROUP_NAME_COLUMN_SIZE, 0), TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak);
            var groupDescriptionTextSize = TextRenderer.MeasureText(group.Description, SettingsConstants.SETTINGS_DESCRIPTIONS_FONT, new Size((int)SettingsConstants.GROUP_NAME_COLUMN_SIZE, 0), TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak);

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
                Text = group.Name,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Form.DefaultBackColor,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                Font = SettingsConstants.SETTINGS_GROUP_NAME_FONT,
            };

            Label descriptionLabel = new()
            {
                Text = group.Description,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                AutoSize = false, //This is to allow the text to wrap
                TextAlign = ContentAlignment.TopLeft,
                BackColor = Form.DefaultBackColor,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                Font = SettingsConstants.SETTINGS_DESCRIPTIONS_FONT,
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
                    Font = SettingsConstants.SETTINGS_VALUE_TITLE_FONT,
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
                    Font = SettingsConstants.SETTINGS_DESCRIPTIONS_FONT,
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

        private void MainSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            this.leftSelectSectionListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
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

        private void ListView_DrawItem(object? sender, DrawListViewItemEventArgs e)
        {
        }

        private void ListView_DrawSubItem(object? sender, DrawListViewSubItemEventArgs e)
        {
            var item = e.Item;
            if (item == null)
            {
                return;
            }

            var foreColor = SettingsConstants.SECTIONS_ITEM_FORE_COLOR;
            var backColor = item.Focused ? SettingsConstants.SECTIONS_SELECTED_ITEM_BACK_COLOR : Color.Transparent;

            var borderColor = item.Focused ? SettingsConstants.SECTIONS_SELECTED_ITEM_BORDER_COLOR : Color.Transparent;
            var borderWidth = SettingsConstants.SECTIONS_SELECTED_ITEM_BORDER_WIDTH;

            var rectsLeftPadding = 0;//(SECTIONS_LEFT_PADDING * 3) - 2;

            //BACKGROUND
            var backBounds = Rectangle.Inflate(e.Bounds, -rectsLeftPadding / 2, 0);
            backBounds.Offset(rectsLeftPadding / 2, 0);

            e.Graphics.FillRectangle(new SolidBrush(backColor), e.Bounds);

            //BORDER
            if (borderWidth > 0)
            {
                var borderBound = Rectangle.Inflate(e.Bounds, -borderWidth - rectsLeftPadding / 2, -borderWidth);
                borderBound.Offset(rectsLeftPadding / 2, 0);

                var borderPen = new Pen(new SolidBrush(borderColor), borderWidth);

                e.Graphics.DrawRectangle(borderPen, borderBound);
            }

            //DRAW TEXT
            var textBounds = Rectangle.Inflate(e.Bounds, -SettingsConstants.SECTIONS_LEFT_PADDING * 3, 0);
            TextRenderer.DrawText(e.Graphics, item.Text, this.leftSelectSectionListView.Font, textBounds, foreColor, TextFormatFlags.Left);
        }

        class ListViewThatKeepsSelection : ListView
        {
            public ListViewThatKeepsSelection()
            {
                this.DoubleBuffered = true;
            }

            protected override void WndProc(ref Message m)
            {
                // Suppress mouse messages that are OUTSIDE of the items area
                if (m.Msg >= 0x201 && m.Msg <= 0x209)
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
