using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TiaXmlReader.Generation.Configuration
{
    public partial class ConfigForm : Form
    {
        public static readonly Font LABEL_FONT = new Font("Microsoft Sans Serif", 12.5f, FontStyle.Bold);
        public static readonly Font CONTROL_FONT = new Font("Microsoft Sans Serif", 9f);

        private readonly string title;
        private readonly List<IConfigFormLine> lineList;

        public Font LabelFont { get; set; } = LABEL_FONT;
        public Font ControlFont { get; set; } = CONTROL_FONT;
        public int ControlWidth { get; set; } = 300;
        public int ControlHeight { get; set; } = 30;
        public bool CloseOnOutsideClick { get; set; } = true;
        public bool CloseOnEnter {  get; set; } = true;
        public bool ShowControlBox { get; set; } = false;

        public ConfigForm(string title)
        {
            InitializeComponent();

            this.title = title;
            this.lineList = new List<IConfigFormLine>();
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

        public C AddLine<C>(ConfigFormLineType<C> type) where C : ConfigFormLine<C>
        {
            IConfigFormLine line;
            if (type.Equals(ConfigFormLineTypes.LABEL))
            {
                line = new ConfigFormLabelLine();
            }
            else if (type.Equals(ConfigFormLineTypes.TEXT_BOX))
            {
                line = new ConfigFormTextBoxLine();
            }
            else if (type.Equals(ConfigFormLineTypes.COMBO_BOX))
            {
                line = new ConfigFormComboBoxLine();
            }
            else if (type.Equals(ConfigFormLineTypes.CHECK_BOX))
            {
                line = new ConfigFormCheckBoxLine();
            }
            else if (type.Equals(ConfigFormLineTypes.BUTTON_PANEL))
            {
                line = new ConfigFormButtonPanelLine();
            }
            else if (type.Equals(ConfigFormLineTypes.COLOR_PICKER))
            {
                line = new ConfigFormColorPickerLine();
            }
            else if (type.Equals(ConfigFormLineTypes.JAVASCRIPT))
            {
                line = new ConfigFormJavascriptLine();
            }
            else
            {
                throw new Exception("Invalid ConfigForm.AddLine ConfigFormLineType for" + type);
            }

            lineList.Add(line);
            return (C) line;
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

        public void Init()
        {
            this.ControlBox = this.ShowControlBox;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.Shown += (object sender, EventArgs args) => { formReadyToClose = true; };
            this.FormClosed += (object sender, FormClosedEventArgs args) => { this.Dispose(); };

            var linePanelList = new List<TableLayoutPanel>();

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
                    RowCount = 1,
                    RowStyles = { new RowStyle(SizeType.Percent, 50f) },
                    ColumnCount = 0,
                    ColumnStyles = { new ColumnStyle(SizeType.AutoSize) },
                };
                linePanelList.Add(panel);

                var labelText = line.GetLabelText();
                if (labelText != null)
                {
                    panel.ColumnCount++;
                    panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

                    var label = new Label
                    {
                        Font = this.LabelFont,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill,
                        Text = line.GetLabelText(),
                        AutoSize = true,
                        Padding = new Padding(0),
                        Margin = new Padding(0)
                    };
                    label.Font = line.GetLabelFont() ?? this.LabelFont;
                    panel.Controls.Add(label);

                    var size = TextRenderer.MeasureText(label.Text, this.LabelFont);
                    size.Width += 4; //Padding
                    if (size.Width > biggestTitleLength)
                    {
                        biggestTitleLength = size.Width;
                    }
                }

                var control = line.GetControl();
                if (control != null)
                {
                    if (line.IsLabelOnTop())
                    {
                        panel.RowCount++;
                        panel.RowStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
                    }
                    else
                    {
                        panel.ColumnCount++;
                        panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                    }

                    control.Width = this.ControlWidth;
                    control.Height = line.GetHeight() == 0 ? this.ControlHeight : line.GetHeight();
                    control.Dock = DockStyle.Fill;
                    control.Font = this.ControlFont;
                    panel.Controls.Add(control);
                }

               
                this.mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                this.mainPanel.Controls.Add(panel);
            }

            foreach (var panel in linePanelList)
            {
                if (panel.ColumnCount == 2) //Only if there are both label AND control.
                {
                    panel.ColumnStyles[0].SizeType = SizeType.Absolute;
                    panel.ColumnStyles[0].Width = biggestTitleLength;
                }
            }
        }
    }

}
