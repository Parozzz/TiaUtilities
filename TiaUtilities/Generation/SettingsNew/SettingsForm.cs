using TiaUtilities.Configuration;
using TiaUtilities.Generation.IO;
using TiaUtilities.Generation.SettingsNew.Editors;
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

            public override string ToString()
            {
                return Name;
            }
        }

        private class Group(string name, string description)
        {
            public string Name { get; init; } = name;
            public string Description { get; init; } = description;
            public List<SettingsValue> Values { get; init; } = []; 
            
            public override string ToString()
            {
                return Name;
            }
        }

        private readonly TableLayoutPanel mainPanel;
        private readonly ListViewThatKeepsSelection leftSelectSectionListView;
        private readonly TableLayoutPanel rightSettingsPanel;

        private readonly List<Section> sectionList;

        public SettingsForm()
        {
            InitializeComponent();

            this.mainPanel = new();
            this.leftSelectSectionListView = new();
            this.rightSettingsPanel = new();

            this.sectionList = [];

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

            this.mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SettingsConstants.SECTIONS_WIDTH));
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
            this.leftSelectSectionListView.MaximumSize = new Size((int)SettingsConstants.SECTIONS_WIDTH, 0);
            this.leftSelectSectionListView.MinimumSize = new Size(30, 80);

            this.leftSelectSectionListView.ItemSelectionChanged += (sender, args) =>
            {
                var item = args.Item;
                if(item == null)
                {
                    return; 
                }

                var itemName = item.Text;
                foreach(var section in sectionList)
                {
                    if(section.Name == itemName)
                    {
                        this.LoadSection(section);
                    }
                }
            };

            var columnHeader = this.leftSelectSectionListView.Columns.Add(columnName, -2, HorizontalAlignment.Left); //Width of -2 indicates auto-size.

            this.leftSelectSectionListView.OwnerDraw = true;
            this.leftSelectSectionListView.DrawColumnHeader += this.ListView_DrawColumnHeader;
            //this.listView.DrawItem += this.ListView_DrawItem;
            this.leftSelectSectionListView.DrawSubItem += this.ListView_DrawSubItem;
        }

        const float SECTION_NAME_SIZE = 120f;
        const float SECTION_BORDER_SIZE = 6;
        private void InitRightPanel()
        {
            //SystemInformation.VerticalScrollBarWidth
            this.rightSettingsPanel.AutoSize = true;
            //Anchors are needed for the ScrollableControl above!
            this.rightSettingsPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            this.rightSettingsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.rightSettingsPanel.Margin = Padding.Empty;

            //First column is for the SubSection. Second column for actual values.

            this.rightSettingsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SECTION_NAME_SIZE));
            this.rightSettingsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SECTION_BORDER_SIZE));
            this.rightSettingsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        }

        public void ParseBindings(SettingsBindings bindings)
        {
            foreach (var sectionBinding in bindings.SectionBindings)
            {
                Section section = new(sectionBinding.Name, sectionBinding.Tooltip);
                foreach(var groupBinding in bindings.GroupBindings)
                {
                    if(groupBinding.Section == section.Name)
                    {
                        Group group = new(groupBinding.Name, groupBinding.Description);
                        foreach(var value in bindings.ValueList)
                        {
                            if(value.Binding.Group == group.Name)
                            {
                                group.Values.Add(value);
                            }
                        }

                        section.Groups.Add(group);
                    }
                }

                ListViewItem listViewItem = new()
                {
                    Text = section.Name,
                    ToolTipText = section.Tooltip,
                };
                this.leftSelectSectionListView.Items.Add(listViewItem);

                this.sectionList.Add(section);
            }
        }

        private void LoadSection(Section section)
        {
            this.rightSettingsPanel.Controls.Clear();

            var rowIndex = 0;
            foreach (var group in section.Groups)
            {
                var groupControl = this.CreateGroupControl(group);

                //This is just to view bettere the division
                var groupVisualizationRectLabel = new Label()
                {
                    Text = "",
                    Dock = DockStyle.Fill,
                    BackColor = Color.DarkBlue,
                    Padding = Padding.Empty,
                    Margin = new Padding(0, 0, (int)(SECTION_BORDER_SIZE / 2f), 0),
                };

                TableLayoutPanel settingsPanel = new()
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Dock = DockStyle.Fill,
                    ColumnStyles = { new ColumnStyle(SizeType.AutoSize) },
                    Padding = Padding.Empty,
                    Margin = Padding.Empty,
                };

                this.rightSettingsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                this.rightSettingsPanel.Controls.Add(groupControl, 0, rowIndex); //This fill the just create LEFT column
                this.rightSettingsPanel.Controls.Add(groupVisualizationRectLabel, 1, rowIndex);
                this.rightSettingsPanel.Controls.Add(settingsPanel, 2, rowIndex); //This fill the just create RIGHT column

                foreach (var value in group.Values)
                {
                    var valueControl = CreateValueControl(value);

                    settingsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    settingsPanel.Controls.Add(valueControl);
                }

                this.rightSettingsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 12f)); //Just for separation!
                rowIndex += 2;
            }
        }

        private TableLayoutPanel CreateGroupControl(Group group)
        {
            TableLayoutPanel panel = new()
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                ColumnStyles = { new ColumnStyle(SizeType.AutoSize) },
                RowStyles = { new RowStyle(SizeType.AutoSize), new RowStyle(SizeType.AutoSize) },
                Margin = Padding.Empty,
                Padding = Padding.Empty,
            };

            Label nameLabel = new()
            {
                Text = group.Name,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                AutoSize = true,
                TextAlign = ContentAlignment.TopCenter,
                BackColor = this.BackColor,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                Font = SettingsConstants.SETTINGS_GROUP_NAME_FONT,
            }; 
            
            Label descriptionLabel = new()
            {
                Text = group.Description,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = this.BackColor,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                Font = SettingsConstants.SETTINGS_GROUP_NAME_FONT,
            };

            panel.Controls.Add(nameLabel, 0, 0);
            panel.Controls.Add(descriptionLabel, 0, 1);

            return panel;
        }

        private TableLayoutPanel CreateValueControl(SettingsValue value)
        {
            TableLayoutPanel panel = new()
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                ColumnStyles = { new ColumnStyle(SizeType.AutoSize) },
                RowStyles = { new RowStyle(SizeType.AutoSize), new RowStyle(SizeType.AutoSize) },
                Margin = Padding.Empty,
                Padding = Padding.Empty,
            };

            var valueName = value.Binding.Name;
            if(!string.IsNullOrEmpty(valueName))
            {
                Label nameLabel = new()
                {
                    Text = value.Binding.Name,
                    AutoSize = true,
                    Dock = DockStyle.Fill,
                    BorderStyle = BorderStyle.None,
                    UseCompatibleTextRendering = true,
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = Color.Transparent,
                    Margin = new Padding(0, 0, 0, 3),
                    Padding = Padding.Empty,
                    Font = SettingsConstants.SETTINGS_VALUE_TITLE_FONT,
                    FlatStyle = FlatStyle.Standard,
                };
                panel.Controls.Add(nameLabel, 0, 0);
            }

            var editor = SettingsEditor.ObtainFromValue(this, value);
            panel.Controls.Add(editor.GetControl(), 0, 1);

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
            if(item == null)
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
            if(borderWidth > 0)
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
