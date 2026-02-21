using System.Diagnostics;
using System.Xml;
using TiaUtilities.Generation.SettingsNew.Bindings;
using TiaUtilities.Generation.SettingsNew.Editors;
using TiaUtilities.Utility;

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
            this.DoubleBuffered = true;
            InitializeComponent();

            this.bindings = bindings;
            this.macroSectionList = [];

            this.mainPanel = new();
            this.leftSelectSectionListView = new();
            this.rightSettingsPanel = new();

            Utils.SetDoubleBuffered(this.mainPanel);
            Utils.SetDoubleBuffered(this.leftSelectSectionListView);
            Utils.SetDoubleBuffered(this.rightSettingsPanel);

            Init();
            ParseBindings();
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
            Utils.SetDoubleBuffered(scrollableControl);

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

            var textSize = TextRenderer.MeasureText("AaGg", SettingsConstants.LIST_LEFT_FONT);
            this.leftSelectSectionListView.TileSize = new Size(SettingsConstants.SECTIONS_LIST_VIEW_WIDTH, textSize.Height + 4);

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


            this.rightSettingsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SettingsConstants.SECTIONS_NAME_COLUMN_SIZE));
            this.rightSettingsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SettingsConstants.SECTIONS_BORDER_COLUMN_SIZE));
            this.rightSettingsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        }

        private void ParseBindings()
        {
            foreach (var macroSectionBinding in this.bindings.MacroSectionList)
            {
                var configurationObject = macroSectionBinding.GetConfigurationObject();
                if(configurationObject == null)
                {
                    continue;
                }

                MacroSection macroSection = new(macroSectionBinding.Name);
                this.macroSectionList.Add(macroSection);

                ListViewItem macroSectionListViewItem = new()
                {
                    Text = macroSection.Name,
                    Tag = new ListViewItemMacroSectionTag(macroSection)
                };
                this.leftSelectSectionListView.Items.Add(macroSectionListViewItem);

                foreach(var sectionBinding in macroSectionBinding.SectionsList)
                {
                    Section section = new(macroSection, sectionBinding.Name, sectionBinding.Description);
                    macroSection.Sections.Add(section);

                    var valueList = sectionBinding.ValueList.Select(bindings => new SettingsValue(bindings, configurationObject)).ToArray();
                    section.ValueList.AddRange(valueList);

                    ListViewItem listViewItem = new()
                    {
                        Text = section.Name,
                        ToolTipText = section.Description,
                        Tag = new ListViewItemSectionTag(section)
                    };
                    this.leftSelectSectionListView.Items.Add(listViewItem);

                    section.ListItem = listViewItem;
                }
            }

            LoadMacroSections();
        }

        const int SECTION_NAME_COLUMN = 0;
        const int SECTION_BORDER_COLUMN = 1;
        const int SECTION_VALUES_COLUMN = 2;

        private record ControlPosition(Control Control, int Column, int Row, int ColumnSpan = 0, int RowSpan = 0);

        private void LoadMacroSections()
        {
            //I've tried many things and this seems to be the more efficient!
            this.rightSettingsPanel.Controls.Clear();

            List<ControlPosition> controlPositionList = [];

            int rowCount = 0;
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

                controlPositionList.Add(new(macroSectioNameLabel, 0, rowCount, ColumnSpan: 3));

                //this.rightSettingsPanel.Controls.Add(macroSectioNameLabel, 0, rowCount);
                //this.rightSettingsPanel.SetColumnSpan(macroSectioNameLabel, 3);

                rowCount++;

                macroSection.Label = macroSectioNameLabel;

                foreach (var section in macroSection.Sections)
                {
                    var sectionStartRowCount = rowCount;
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
                    ToolTip toolTip = new()
                    {
                        InitialDelay = 1500,
                        ReshowDelay = 800,
                        AutomaticDelay = 1000,
                        UseFading = false,
                        UseAnimation = false,
                    };
                    toolTip.SetToolTip(sectionNameLabel, section.Description);

                    TableLayoutPanel valuesPanel = new()
                    {
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink,
                        Dock = DockStyle.Fill,
                        ColumnStyles = { new ColumnStyle(SizeType.AutoSize) },
                        Padding = Padding.Empty,
                        Margin = Padding.Empty,
                    };
                    Utils.SetDoubleBuffered(valuesPanel);

                    List<ControlPosition> valuesControlPositionList = [];

                    int valuesRowCount = 0;
                    foreach (var value in section.ValueList)
                    {
                        this.AppendToValuePanel(valuesPanel, valuesControlPositionList, value, ref valuesRowCount);
                    }

                    SettingsForm.ApplyControlPositions(valuesPanel, valuesControlPositionList);

                    //This is just to view bettere the division
                    var sectionBorderLabel = new Label()
                    {
                        Text = "",
                        Dock = DockStyle.Fill,
                        BackColor = Color.DarkBlue,
                        Padding = Padding.Empty,
                        Margin = new Padding(0, 6, (int)(SettingsConstants.SECTIONS_BORDER_COLUMN_SIZE / 2f), 6),
                    };

                    this.rightSettingsPanel.RowStyles.Add(new(SizeType.AutoSize));
                    rowCount++;

                    controlPositionList.Add(new(sectionNameLabel, SECTION_NAME_COLUMN, rowCount - 1));
                    controlPositionList.Add(new(sectionBorderLabel, SECTION_BORDER_COLUMN, rowCount - 1));
                    controlPositionList.Add(new(valuesPanel, SECTION_VALUES_COLUMN, rowCount - 1));

                    this.rightSettingsPanel.RowStyles.Add(new(SizeType.Absolute, SettingsConstants.SECTION_VALUE_SEPERATION));
                    rowCount++;

                    section.Panel = valuesPanel;
                }
            }


            SettingsForm.ApplyControlPositions(this.rightSettingsPanel, controlPositionList);
        }

        private static void ApplyControlPositions(TableLayoutPanel panel, List<ControlPosition> controlPositionList)
        {
            panel.Controls.AddRange(controlPositionList.Select(c => c.Control).ToArray());
            foreach (var controlPosition in controlPositionList)
            {
                panel.SetCellPosition(controlPosition.Control, new(controlPosition.Column, controlPosition.Row));
                if (controlPosition.ColumnSpan > 0)
                {
                    panel.SetColumnSpan(controlPosition.Control, controlPosition.ColumnSpan);
                }

                if (controlPosition.RowSpan > 0)
                {
                    panel.SetRowSpan(controlPosition.Control, controlPosition.RowSpan);
                }
            }
        }

        //DO NOT USE UseCompatibleTextRendering! IT WILL INCREASE TIME A LOT!
        private void AppendToValuePanel(TableLayoutPanel panel, List<ControlPosition> controlPositionList, SettingsValue value, ref int rowCount)
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
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = Color.Transparent,
                    Margin = hasDescription ? Padding.Empty : new(0, 0, 0, 3),
                    Padding = Padding.Empty,
                    Font = SettingsConstants.VALUE_NAME_LABEL_FONT,
                };
                Utils.SetDoubleBuffered(nameLabel);

                panel.RowStyles.Add(new(SizeType.AutoSize));
                rowCount++;

                controlPositionList.Add(new(nameLabel, 0, rowCount-1));
            }

            if (hasDescription)
            {
                Label descriptionLabel = new()
                {
                    Text = value.Description,
                    AutoSize = true,
                    Dock = DockStyle.Fill,
                    BorderStyle = BorderStyle.None,
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = Color.Transparent,
                    Margin = new Padding(0, 0, 0, 3),
                    Padding = Padding.Empty,
                    Font = SettingsConstants.DESCRIPTION_LABEL_FONT,
                    MaximumSize = new Size(450, 0) //Since last columns is to fill the all control, set a maximun size to allow wrapping of text
                };
                Utils.SetDoubleBuffered(descriptionLabel);

                panel.RowStyles.Add(new(SizeType.AutoSize));
                rowCount++;

                controlPositionList.Add(new(descriptionLabel, 0, rowCount-1));
            }

            panel.RowStyles.Add(new(SizeType.AutoSize));
            rowCount++;

            var editor = SettingsEditor.ObtainFromValue(this, value);
            controlPositionList.Add(new(editor.GetControl(), 0, rowCount - 1));

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

            if(e.State == 0)
            {
                return;
            }

            if (item.Tag is ListViewItemTag tag)
            {
                var rectsLeftPadding = 0;//(SECTIONS_LEFT_PADDING * 3) - 2;

                var foreColor = SettingsConstants.SECTIONS_ITEM_FORE_COLOR;
                var backColor = tag is ListViewItemSectionTag sectionTag && sectionTag.IsSectionVisible ? SettingsConstants.SECTIONS_SELECTED_ITEM_BACK_COLOR : Color.Transparent;

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
                private void LoadMacroSections()
        {
            this.rightSettingsPanel.Controls.Clear();

            List<ControlPosition> controlPositionList = [];

            int rowCount = 0;
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

                controlPositionList.Add(new(macroSectioNameLabel, 0, rowCount, ColumnSpan: 3));
               
                //this.rightSettingsPanel.Controls.Add(macroSectioNameLabel, 0, rowCount);
                //this.rightSettingsPanel.SetColumnSpan(macroSectioNameLabel, 3);
                
                rowCount++;

                macroSection.Label = macroSectioNameLabel;
                
                foreach (var section in macroSection.Sections)
                {
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
                    Utils.SetDoubleBuffered(valuesPanel);


                    List<ControlPosition> valuesControlPositionList = [];

                    int valueRowCount = 0;
                    foreach (var value in section.ValueList)
                    {
                        this.AppendToValuePanel(valuesPanel, value, valuesControlPositionList, ref valueRowCount);
                    }

                    SettingsForm.ApplyControlPositions(valuesPanel, valuesControlPositionList);

                    //This is just to view bettere the division
                    var sectionBorderLabel = new Label()
                    {
                        Text = "",
                        Dock = DockStyle.Fill,
                        BackColor = Color.DarkBlue,
                        Padding = Padding.Empty,
                        Margin = new Padding(0, 6, (int)(SettingsConstants.SECTIONS_BORDER_COLUMN_SIZE / 2f), 6),
                    };

                    this.rightSettingsPanel.RowStyles.Add(new(SizeType.AutoSize));

                    
                    controlPositionList.Add(new(sectionNameLabel, 0, rowCount));
                    controlPositionList.Add(new(sectionBorderLabel, 1, rowCount));
                    controlPositionList.Add(new(valuesPanel, 2, rowCount));
                    
                    
                    this.rightSettingsPanel.Controls.Add(sectionNameLabel, 0, rowCount);
                    this.rightSettingsPanel.Controls.Add(sectionBorderLabel, 1, rowCount);
                    this.rightSettingsPanel.Controls.Add(valuesPanel, 2, rowCount);
                    
                    rowCount++;

                    this.rightSettingsPanel.RowStyles.Add(new(SizeType.Absolute, SettingsConstants.SECTION_VALUE_SEPERATION));
                    rowCount++;

                    section.Panel = valuesPanel;
                                    }
                                }


            SettingsForm.ApplyControlPositions(this.rightSettingsPanel, controlPositionList);
        }

        //DO NOT USE UseCompatibleTextRendering! IT WILL INCREASE TIME A LOT!
        private void AppendToValuePanel(TableLayoutPanel panel, SettingsValue value, List<ControlPosition> controlPositionList, ref int rowCount)
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
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = Color.Transparent,
                    Margin = hasDescription ? Padding.Empty : new(0, 0, 0, 3),
                    Padding = Padding.Empty,
                    Font = SettingsConstants.VALUE_NAME_LABEL_FONT,
                };

                panel.RowStyles.Add(new(SizeType.AutoSize));

                controlPositionList.Add(new(nameLabel, 0, rowCount));
                //panel.Controls.Add(nameLabel, 0, rowCount);
                rowCount++;
            }
            
            if (hasDescription)
            {
                Label descriptionLabel = new()
                {
                    Text = value.Description,
                    AutoSize = false, //This is to allow the text to wrap
                    Dock = DockStyle.Fill,
                    BorderStyle = BorderStyle.None,
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = Color.Transparent,
                    Margin = new Padding(0, 0, 0, 3),
                    Padding = Padding.Empty,
                    Font = SettingsConstants.DESCRIPTION_LABEL_FONT,
                };

                panel.RowStyles.Add(new(SizeType.AutoSize));

                controlPositionList.Add(new(descriptionLabel, 0, rowCount));
                //panel.Controls.Add(descriptionLabel, 0, rowCount);
                rowCount++;
            }

            var editor = SettingsEditor.ObtainFromValue(this, value);

            panel.RowStyles.Add(new(SizeType.AutoSize));

            controlPositionList.Add(new(editor.GetControl(), 0, rowCount));
            //panel.Controls.Add(editor.GetControl(), 0, rowCount);
            rowCount++;

            panel.RowStyles.Add(new(SizeType.Absolute, SettingsConstants.SECTION_VALUE_SEPERATION));
            rowCount++;
        }

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