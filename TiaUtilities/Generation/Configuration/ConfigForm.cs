namespace TiaXmlReader.Generation.Configuration
{
    public partial class ConfigForm : Form
    {
        public static readonly Font LABEL_FONT = new(FontFamily.GenericMonospace, 12.5f, FontStyle.Bold);
        public static readonly Font CONTROL_FONT = new(FontFamily.GenericMonospace, 9f);

        private readonly string title;

        public Font LabelFont { get; set; } = LABEL_FONT;
        public Font ControlFont { get; set; } = CONTROL_FONT;
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

        public ConfigFormGroup Init()
        {
            this.ControlBox = this.ShowControlBox;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.titleLabel.Text = title;

            var mainGroup = new ConfigFormGroup(this);

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
            // if click outside dialog -> Close Dlg
            //CanFocus is false if there is a modal window open to avoid closing for cliking it (Like color dialog or file dialog)
            if (CloseOnOutsideClick && formReadyToClose && m.Msg == 0x86 && this.CanFocus && !this.RectangleToScreen(this.DisplayRectangle).Contains(Cursor.Position)) //0x86 WM_NCACTIVATE
            {
                this.Close();
                return;
            }

            base.WndProc(ref m);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Cancel || keyData == Keys.Escape || (keyData == Keys.Enter && this.CloseOnEnter))
            {
                this.Close();
                return true;    // indicate that you handled this keystroke
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

}
