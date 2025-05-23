﻿using TiaUtilities.Languages;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.Configuration
{
    public partial class ConfigForm : Form
    {
        private readonly string title;

        public object? Configuration { get; private set; }
        public object? PresetConfiguration { get; private set; }
        public IEnumerable<object> OtherConfigurations { get; private set; } = [];

        public Font LabelFont { get; set; } = ConfigStyle.LABEL_FONT;
        public Font ControlFont { get; set; } = ConfigStyle.CONTROL_FONT;
        public int ControlWidth { get; set; } = 300;
        public int ControlHeight { get; set; } = 30;
        public bool CloseOnOutsideClick { get; set; } = true;
        public bool CloseOnEnter { get; set; } = true;
        public bool ShowControlBox { get; set; } = false;

        public ConfigForm(string title)
        {
            InitializeComponent();

            this.title = title;
        }

        public void SetConfiguration<T>(T configuration, T? presetConfiguration = default, IEnumerable<T>? otherConfigurations = null)
        {
            this.Configuration = configuration;
            this.PresetConfiguration = presetConfiguration;
            this.OtherConfigurations = otherConfigurations == null ? [] : otherConfigurations.Cast<object>();
        }

        public void StartShowingAtCursor()
        {
            this.StartShowingAtLocation(Cursor.Position);
        }

        public void StartShowingAtControl(Control control)
        {
            var loc = control.PointToScreen(Point.Empty);
            //loc.X += fcConfigButton.Width;
            loc.Y += control.Height;
            this.StartShowingAtLocation(loc);
        }

        public void StartShowingAtLocation(Point loc)
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Location = loc;
        }

        public ConfigGroup Init()
        {
            this.ControlBox = this.ShowControlBox;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.titleLabel.Text = title;

            ToolTip savePresetTooltip = new() { InitialDelay = 500, AutoPopDelay = 4000, ReshowDelay = 500 };
            savePresetTooltip.SetToolTip(this.savePresetButton, Locale.CONFIG_LINE_SAVE_DEFAULT_TOOLTIP);

            if(this.PresetConfiguration == null)
            {
                this.topPanel.Controls.Remove(this.savePresetButton);
            }

            var mainGroup = new ConfigGroup(this);

            this.Load += (sender, args) =>
            {
                var control = mainGroup.GetControl();
                this.mainPanel.Controls.Add(control);
            };
            this.Shown += (sender, args) => formReadyToClose = true;
            this.FormClosed += (sender, args) => this.Dispose();

            return mainGroup;
        }

        private bool formReadyToClose = false;
        protected override void WndProc(ref Message m)
        {
            try
            {
                // if click outside dialog -> Close Dlg
                //CanFocus is false if there is a modal window open to avoid closing for cliking it (Like color dialog or file dialog)
                if (CloseOnOutsideClick && formReadyToClose && m.Msg == 0x86 && this.CanFocus && !this.RectangleToScreen(this.DisplayRectangle).Contains(Cursor.Position)) //0x86 WM_NCACTIVATE
                {
                    this.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            base.WndProc(ref m);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                if (keyData == Keys.Cancel || keyData == Keys.Escape || (keyData == Keys.Enter && this.CloseOnEnter))
                {
                    this.Close();
                    return true;    // indicate that you handled this keystroke
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SavePresetButton_Click(object sender, EventArgs e)
        {
            if (this.Configuration != null && this.PresetConfiguration != null)
            {
                GenUtils.CopySamePublicFieldsAndProperties(this.Configuration, this.PresetConfiguration);
            }
        }
    }

}
