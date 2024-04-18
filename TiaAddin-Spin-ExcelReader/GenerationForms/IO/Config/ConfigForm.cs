using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TiaXmlReader.CustomControls;

namespace TiaXmlReader.GenerationForms.IO.Config
{
    public partial class ConfigForm : Form
    {
        private static readonly Font LABEL_FONT = new Font("Microsoft Sans Serif", 12.5f, FontStyle.Bold);
        private static readonly Font CONTROL_FONT = new Font("Microsoft Sans Serif", 9f);

        private readonly string title;
        private readonly List<IConfigFormLine> lineList;

        public Font LabelFont { get; set; } = LABEL_FONT;
        public Font ControlFont { get; set; } = CONTROL_FONT;
        public int ControlWidth { get; set; } = 300;
        public int ControlHeight { get; set; } = 30;
        
        public ConfigForm(string title)
        {
            InitializeComponent();

            this.title = title;
            this.lineList = new List<IConfigFormLine>();
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

        public ConfigFormTextBoxLine AddTextBoxLine(string labelText)
        {
            var line = new ConfigFormTextBoxLine(labelText);
            this.AddConfigLine(line);
            return line;
        }

        public ConfigFormComboBoxLine AddComboBoxLine(string labelText)
        {
            var line = new ConfigFormComboBoxLine(labelText);
            this.AddConfigLine(line);
            return line;
        }

        public ConfigFormColorPickerButtonLine AddColorPickerLine(string labelText)
        {
            var line = new ConfigFormColorPickerButtonLine(labelText);
            this.AddConfigLine(line);
            return line;
        }

        public void AddConfigLine(IConfigFormLine line)
        {
            lineList.Add(line);
        }

        private bool formReadyToClose = false;
        protected override void WndProc(ref Message m)
        {
            // if click outside dialog -> Close Dlg
            //CanFocus is false if there is a modal window open to avoid closing for cliking it (Like color dialog or file dialog)
            if (formReadyToClose && m.Msg == 0x86 && this.CanFocus && !this.RectangleToScreen(this.DisplayRectangle).Contains(Cursor.Position)) //0x86 WM_NCACTIVATE
            {
                this.Close();
                return;
            }

            base.WndProc(ref m);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Cancel || keyData == Keys.Escape)
            {
                this.Close();
                return true;    // indicate that you handled this keystroke
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void Init()
        {
            this.Shown += (object sender, EventArgs args) => { formReadyToClose = true; };
            this.FormClosed += (object sender, FormClosedEventArgs args) => { this.Dispose(); };
                
            List<TableLayoutPanel> linesTables = new List<TableLayoutPanel>();

            var biggestTitleLength = 0;

            this.titleLabel.Text = title;
            foreach (var line in lineList)
            {
                var panel = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    Margin = new Padding(2),
                    Padding = new Padding(0),
                    ColumnCount = 2,
                    ColumnStyles = { new ColumnStyle(SizeType.AutoSize), new ColumnStyle(SizeType.AutoSize) },
                    RowCount = 1,
                    RowStyles = { new RowStyle(SizeType.Percent, 50f) }
                };
                linesTables.Add(panel);

                var label = new Label
                {
                    Font = this.LabelFont,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Text = line.GetLabelText(),
                    AutoSize = true,
                    Padding = new Padding(0),
                    Margin = new Padding(2)
                };
                panel.Controls.Add(label);

                var size = TextRenderer.MeasureText(label.Text, this.LabelFont);
                size.Width += 4; //Padding
                if (size.Width > biggestTitleLength)
                {
                    biggestTitleLength = size.Width;
                }

                var control = line.GetControl();
                control.Width = this.ControlWidth;
                control.Height = this.ControlHeight;
                control.Dock = DockStyle.Fill;
                control.Font = this.ControlFont;
                control.Padding = new Padding(0);
                control.Margin = new Padding(2);
                panel.Controls.Add(control);

                this.mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                this.mainPanel.Controls.Add(panel);
            }

            foreach (var panel in linesTables)
            {
                panel.ColumnStyles[0].SizeType = SizeType.Absolute;
                panel.ColumnStyles[0].Width = biggestTitleLength;
            }
        }
    }

}
