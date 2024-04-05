using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.CustomControls;

namespace TiaXmlReader.GenerationForms.IO
{
    public partial class ConfigForm : Form
    {
        private static readonly Font LABEL_FONT = new Font("Microsoft Sans Serif", 12.5f, FontStyle.Bold);
        private static readonly Font CONTROL_FONT = new Font("Microsoft Sans Serif", 9f);

        private readonly string title;
        private readonly List<ConfigFormLine> lineList;
        public ConfigForm(string title)
        {
            InitializeComponent();

            this.title = title;
            this.lineList = new List<ConfigFormLine>();
        }

        public ConfigForm AddConfigLine(ConfigFormLine line)
        {
            lineList.Add(line);
            return this;
        }

        private bool formReadyToClose = false;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // if click outside dialog -> Close Dlg
            if (formReadyToClose && m.Msg == 134) //0x86 WM_NCACTIVATE
            {
                if (!this.RectangleToScreen(this.DisplayRectangle).Contains(Cursor.Position))
                {
                    this.Close();
                }
            }
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
                    ColumnCount = 2,
                    ColumnStyles = { new ColumnStyle(SizeType.AutoSize), new ColumnStyle(SizeType.AutoSize) },
                    RowCount = 1,
                    RowStyles = { new RowStyle(SizeType.Percent, 50f) }
                };
                linesTables.Add(panel);

                var label = new Label
                {
                    Font = LABEL_FONT,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Text = line.GetTitle(),
                    AutoSize = true,
                    Padding = new Padding(0),
                    Margin = new Padding(2)
                };
                panel.Controls.Add(label);

                var size = TextRenderer.MeasureText(line.GetTitle(), LABEL_FONT);
                size.Width += 4; //Padding
                if (size.Width > biggestTitleLength)
                {
                    biggestTitleLength = size.Width;
                }

                var control = line.GetControl();
                control.Width = 300;
                control.Height = 30;
                control.Dock = DockStyle.Fill;
                control.Font = CONTROL_FONT;

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


    public class ConfigFormLine
    {
        private string title;

        private Control control;

        private bool numericOnly;
        private EventHandler textChangedEvent;
        private EventHandler uintChangedEvent;
        private KeyPressEventHandler keyPressEvent;

        public ConfigFormLine()
        {
        }

        public ConfigFormLine Title(string title)
        {
            this.title = title;
            return this;
        }

        public ConfigFormLine TextBox(IConvertible value)
        {
            this.control = new FlatTextBox();
            this.control.Text = value.ToString();

            HandleEvents();
            return this;
        }

        public ConfigFormLine ComboBox(string[] valuesArray)
        {
            this.control = new ComboBox();
            if (valuesArray.Length > 0)
            {
                this.control.Text = valuesArray[0];
                ((ComboBox)this.control).Items.AddRange(valuesArray);
            }

            HandleEvents();
            return this;
        }

        public ConfigFormLine TextChanged(Action<string> action)
        {
            HandleEvents(action);
            return this;
        }

        public ConfigFormLine UIntChanged(Action<uint> action)
        {
            HandleEvents(null, str =>
            {
                if (uint.TryParse(str, out uint result))
                {
                    action.Invoke(result);
                }
            });
            return this;
        }

        public ConfigFormLine NumericOnly()
        {
            numericOnly = true;
            return this;
        }

        public string GetTitle()
        {
            return title;
        }

        public Control GetControl()
        {
            return control;
        }

        private void HandleEvents(Action<string> textChangedAction = null, Action<string> uintChangedAction = null)
        {
            if (control != null)
            {
                HandleTextChanged(ref textChangedEvent, textChangedAction);
                HandleTextChanged(ref uintChangedEvent, uintChangedAction);

                if (keyPressEvent != null)
                {
                    control.KeyPress -= keyPressEvent;
                    keyPressEvent = null;
                }

                if (numericOnly)
                {
                    keyPressEvent = (object sender, KeyPressEventArgs args) => { args.Handled = Char.IsLetter(args.KeyChar); };
                    control.KeyPress += keyPressEvent;
                }
            }
        }

        private void HandleTextChanged(ref EventHandler handler, Action<string> action = null)
        {
            if (action != null)
            {//if action is defined, i will removed the old one and add the new. Otherwise it will add the old.
                if (handler != null)
                {
                    control.TextChanged -= handler;
                    handler = null;
                }

                handler = (object sender, EventArgs args) => { action.Invoke(control.Text); };
            }

            if (handler != null)
            {
                control.TextChanged += handler;
            }
        }
    }

}
