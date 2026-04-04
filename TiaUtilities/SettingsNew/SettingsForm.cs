using FastColoredTextBoxNS;
using TiaUtilities.CustomControls;
using TiaUtilities.SettingsNew;
using TiaUtilities.SettingsNew.Bindings;
using TiaUtilities.SettingsNew.FormHelpers;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.SettingsNew
{
    public partial class SettingsForm : Form
    {
        private readonly TableLayoutPanel mainPanel;
        private readonly SettingsFormSectionListView leftSectionListView;
        private readonly TableLayoutPanel rightSettingsPanel;

        private readonly SettingsFormBindingControlLoader bindingsControlLoader;

        public SettingsForm(SettingsBindings bindings)
        {
            this.DoubleBuffered = true;
            InitializeComponent();

            this.mainPanel = new();
            Utils.SetDoubleBuffered(this.mainPanel);

            this.leftSectionListView = new();
            Utils.SetDoubleBuffered(this.leftSectionListView);

            this.rightSettingsPanel = new();
            Utils.SetDoubleBuffered(this.rightSettingsPanel);

            this.bindingsControlLoader = new(bindings, this);

            Init();
        }

        private void Init()
        {
            this.mainPanel.Dock = DockStyle.Fill;
            this.mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.mainPanel.AutoSize = true;

            this.mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SettingsFormConstants.SECTIONS_LIST_VIEW_WIDTH));
            this.mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            this.mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            this.InitLeftListView();
            this.InitRightPanel();

            this.mainPanel.Controls.Add(this.leftSectionListView, 0, 0);
            this.mainPanel.Controls.Add(this.WrapRightPanelInScrollable(this.rightSettingsPanel), 1, 0);
            this.Controls.Add(this.mainPanel);

            this.bindingsControlLoader.Load();
            this.bindingsControlLoader.Add(this.leftSectionListView, this.rightSettingsPanel);

            void UpdateRequestEvent(object? sender, EventArgs args) => this.bindingsControlLoader.UpdateValues();
            this.bindingsControlLoader.Bindings.UpdateEvent += UpdateRequestEvent;

            void ReloadEvent(object? sender, EventArgs args) => this.ReloadBindings();
            this.bindingsControlLoader.Bindings.ReloadEvent += ReloadEvent;

            this.FormClosed += (sender, args) =>
            {
                this.bindingsControlLoader.Bindings.UpdateEvent -= UpdateRequestEvent;
                this.bindingsControlLoader.Bindings.ReloadEvent -= ReloadEvent;
            };
        }

        private void InitLeftListView()
        {
            this.leftSectionListView.ItemSelectionChanged += (sender, args) =>
            {
                if (this.rightSettingsPanel.Parent is ScrollableControl scrollableControl)
                {
                    int scrollValue = scrollableControl.VerticalScroll.Minimum;
                    if (args.Item?.Tag is SettingsFormSectionListView.ItemSectionTag sectionTag && sectionTag.Section.Panel != null)
                    {
                        scrollValue = sectionTag.Section.Panel.Top;
                    }
                    else if (args.Item?.Tag is SettingsFormSectionListView.ItemMacroSectionTag macroSectionTag && macroSectionTag.MacroSection.Label != null)
                    {
                        scrollValue = macroSectionTag.MacroSection.Label.Top;
                    }

                    if(scrollValue >= scrollableControl.VerticalScroll.Minimum && scrollValue <= scrollableControl.VerticalScroll.Maximum)
                    {
                        scrollableControl.VerticalScroll.Value = scrollValue; 

                        scrollableControl.PerformLayout(); //This immediately updates the control since the Function below uses client side values for calculation!
                        this.UpdateSectionVisiblePercentage(scrollableControl);
                    }
                }
            };
        }

        private void InitRightPanel()
        {
            //SystemInformation.VerticalScrollBarWidth
            this.rightSettingsPanel.AutoSize = true;
            this.rightSettingsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            //Anchors are needed for the ScrollableControl above!
            this.rightSettingsPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            this.rightSettingsPanel.Margin = Padding.Empty;

            this.rightSettingsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SettingsFormConstants.SECTIONS_NAME_COLUMN_SIZE));
            this.rightSettingsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SettingsFormConstants.SECTIONS_BORDER_COLUMN_SIZE));
            this.rightSettingsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
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
            this.leftSectionListView.SuspendLayout();

            var scrollableVisibleTop = scrollPositionNew >= 0 ? scrollPositionNew : scrollableControl.VerticalScroll.Value; //Effectively the scroll position
            var scrollableVisibleBottom = scrollableVisibleTop + scrollableControl.Height;

            this.bindingsControlLoader.UpdateVisiblePercentages(scrollableVisibleTop, scrollableVisibleBottom);

            this.leftSectionListView.ResumeLayout(true);
        }

        private void ReloadBindings()
        {
            this.SuspendAll();

            this.leftSectionListView.Items.Clear();
            this.rightSettingsPanel.Controls.Clear();

            this.bindingsControlLoader.Load();
            this.bindingsControlLoader.Add(this.leftSectionListView, this.rightSettingsPanel);

            this.ResumeAll();
        }

        public void SuspendAll()
        {
            this.leftSectionListView.SuspendLayout();
            this.rightSettingsPanel.SuspendLayout();
            this.rightSettingsPanel.Parent?.SuspendLayout();
        }

        public void ResumeAll()
        {
            this.leftSectionListView.ResumeLayout(true);
            this.rightSettingsPanel.ResumeLayout(true);
            this.rightSettingsPanel.Parent?.ResumeLayout(true);
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
                    case Keys.Enter | Keys.Control:
                        var activeControl = this.ActiveControl;
                        if (activeControl is IButtonControl button)
                        {
                            button.PerformClick();
                            return true;
                        }
                        else if (activeControl is CheckBox checkBox)
                        {
                            checkBox.Checked = !checkBox.Checked;
                            return true;
                        }
                        else if (activeControl is ComboBox comboBox)
                        {
                            comboBox.DroppedDown = true;
                            return true;
                        }
                        else if (activeControl is RJComboBox rjComboBox)
                        {
                            rjComboBox.DroppedDrown = true;
                        }
                        break;
                    case Keys.Enter:
                        if(this.ActiveControl is not FastColoredTextBox)
                        {
                            this.rightSettingsPanel.SelectNextControl(ActiveControl, true, true, true, false);
                            return true;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}