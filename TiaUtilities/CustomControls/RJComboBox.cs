using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Drawing2D;

namespace TiaUtilities.CustomControls
{
    [DefaultEvent("OnSelectedIndexChanged")]
    class RJComboBox : UserControl
    {
        [Category("RJ Code - Appearance")]
        public override Color BackColor
        {
            get => backColor;
            set
            {
                backColor = value;
                this.textBox.BackColor = backColor;
                this.button.BackColor = backColor;
            }
        }
        private Color backColor = Color.WhiteSmoke;

        [Category("RJ Code - Appearance")]
        public Color IconColor
        {
            get => iconColor;
            set
            {
                iconColor = value;
                button.Invalidate();//Redraw icon
            }
        }
        private Color iconColor = Color.MediumSlateBlue;

        public Color IconBackColor
        {
            get => iconBackColor;
            set
            {
                this.button.BackColor = value;
                iconBackColor = value;
                button.Invalidate();//Redraw icon
            }
        }
        private Color iconBackColor = Color.AntiqueWhite;

        [Category("RJ Code - Appearance")]
        public Color ListBackColor
        {
            get => listBackColor;
            set
            {
                listBackColor = value;
                comboBox.BackColor = listBackColor;
            }
        }
        private Color listBackColor = SystemColors.Control;

        [Category("RJ Code - Appearance")]
        public Color ListTextColor
        {
            get => listTextColor;
            set
            {
                listTextColor = value;
                comboBox.ForeColor = listTextColor;
            }
        }
        private Color listTextColor = SystemColors.ControlText;

        [Category("RJ Code - Appearance")]
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
                base.BackColor = borderColor; //Border Color
            }
        }
        private Color borderColor = Color.MediumSlateBlue;

        [Category("RJ Code - Appearance")]
        public int BorderSize
        {
            get => borderSize;
            set
            {
                borderSize = value;
                this.Padding = new Padding(borderSize);//Border Size
                AdjustComboBoxDimensions();
            }
        }
        private int borderSize = 1;

        [Category("RJ Code - Appearance")]
        public override Color ForeColor
        {
            get => base.ForeColor;
            set
            {
                base.ForeColor = value;
                this.textBox.ForeColor = value;
            }
        }

        [Category("RJ Code - Appearance")]
        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                this.textBox.Font = value;
                this.comboBox.Font = value; //Optional
                AdjustComboBoxDimensions();
            }
        }

        [Category("RJ Code - Appearance")]
        public override string Text
        {
            get => comboBox.Text;
            set
            {
                this.textBox.Text = value;
                this.comboBox.SelectedItem = value;
            }
        }

        [Category("RJ Code - Appearance")]
        public bool Underlined
        {
            get => this.textBox.Underlined;
            set => this.textBox.Underlined = value;
        }

        [Category("RJ Code - Appearance")]
        public Color UnderlineColor
        {
            get => this.textBox.UnderlineColor;
            set => this.textBox.UnderlineColor = value;
        }

        [Category("RJ Code - Appearance")]
        public ComboBoxStyle DropDownStyle
        {
            get => this.comboBox.DropDownStyle;
            set
            {
                this.textBox.ReadOnly = (value != ComboBoxStyle.Simple);
                comboBox.DropDownStyle = value;
            }
        }

        #region COMBO_BOX_PROPERTIES
        [Category("RJ Code - Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(true)]
        [MergableProperty(false)]
        public ComboBox.ObjectCollection Items => this.comboBox.Items;

        [Category("RJ Code - Data")]
        [AttributeProvider(typeof(IListSource))]
        [DefaultValue(null)]
        public object? DataSource
        {
            get => this.comboBox.DataSource;
            set => this.comboBox.DataSource = value;
        }

        [Category("RJ Code - Data")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Localizable(true)]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get => this.comboBox.AutoCompleteCustomSource;
            set => this.comboBox.AutoCompleteCustomSource = value;
        }

        [Category("RJ Code - Data")]
        [Browsable(true)]
        [DefaultValue(AutoCompleteSource.None)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteSource AutoCompleteSource
        {
            get => this.comboBox.AutoCompleteSource;
            set => this.comboBox.AutoCompleteSource = value;
        }

        [Category("RJ Code - Data")]
        [Browsable(true)]
        [DefaultValue(AutoCompleteMode.None)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteMode AutoCompleteMode
        {
            get => this.comboBox.AutoCompleteMode;
            set => this.comboBox.AutoCompleteMode = value;
        }

        [Category("RJ Code - Data")]
        [Bindable(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object? SelectedValue
        {
            get => this.comboBox.SelectedValue;
            set => this.comboBox.SelectedValue = value;
        }

        [Category("RJ Code - Data")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get => this.comboBox.SelectedIndex;
            set => this.comboBox.SelectedIndex = value;
        }

        [Category("RJ Code - Data")]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string DisplayMember
        {
            get => this.comboBox.DisplayMember;
            set => this.comboBox.DisplayMember = value;
        }

        [Category("RJ Code - Data")]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string ValueMember
        {
            get => this.comboBox.ValueMember;
            set => this.comboBox.ValueMember = value;
        }
        #endregion

        private readonly TableLayoutPanel textBoxPanel;
        private readonly ComboBox comboBox;
        private readonly RJTextBox textBox;
        private readonly Button button;

        //Events
        public event EventHandler OnSelectedIndexChanged = delegate { }; //Default event

        public RJComboBox()
        {
            this.SuspendLayout();

            //Label: Text
            this.textBox = new()
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                ForeColor = this.ForeColor,
                BackColor = this.BackColor,
                BorderColor = Color.Transparent,
                BorderFocusColor = Color.Transparent,
                ReadOnly = true,
                TextLeftPadding = 2,
                TextTopBottomPadding = 5,
            };
            this.textBox.MouseEnter += (sender, args) => this.OnMouseLeave(args);
            this.textBox.MouseLeave += (sender, args) => this.OnMouseLeave(args);
            this.textBox.TextChanged += (sender, args) => this.OnTextChanged(args);
            this.textBox.Click += (sender, args) =>
            {
                this.OnClick(args);
                if (this.comboBox.DropDownStyle != ComboBoxStyle.Simple)
                {
                    this.comboBox.Select();
                    this.comboBox.DroppedDown = true;//Open dropdown list
                }
            };

            this.button = new()
            {
                Dock = DockStyle.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = this.iconBackColor,
                Size = new Size(30, 30),
                Cursor = Cursors.Hand,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
            };
            this.button.FlatAppearance.BorderSize = 0;
            this.button.Click += (sender, args) =>
            {//Open dropdown list
                comboBox.Select();
                comboBox.DroppedDown = true;
            };
            this.button.Paint += Button_Paint;//Draw icon

            this.comboBox = new()
            {
                ForeColor = listTextColor,
                BackColor = listBackColor,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
            };
            this.comboBox.SelectedIndexChanged += (sender, args) =>
            { //Default event
                this.OnSelectedIndexChanged.Invoke(sender, args);
                this.textBox.Text = this.comboBox.Text;
            };
            this.comboBox.TextChanged += (sender, args) => this.textBox.Text = this.comboBox.Text;   //Refresh text

            this.Controls.Add(textBox);//2
            this.Controls.Add(button);//0
            this.Controls.Add(comboBox);//1

            this.ResumeLayout();
            AdjustComboBoxDimensions();

            InitializeComponent();
        }

        // 
        // RJComboBox
        // 
        private void InitializeComponent()
        {
            SuspendLayout();
            this.Name = "RJComboBox";
            this.MinimumSize = new Size(200, 30);
            this.Size = new Size(200, 30);
            this.ForeColor = Color.DimGray;
            this.Margin = new Padding(0);
            this.Padding = new Padding(borderSize);//Border Size
            this.Font = new Font(this.Font.Name, 10F);
            base.BackColor = borderColor; //Border Color
            this.Load += new EventHandler(this.RJComboBox_Load);
            ResumeLayout(false);
        }

        private void AdjustComboBoxDimensions()
        {
            comboBox.Width = textBox.Width;
            comboBox.Location = new Point()
            {
                X = this.Width - this.Padding.Right - comboBox.Width,
                Y = textBox.Bottom - comboBox.Height
            };
            if (comboBox.Height >= this.Height)
            {
                this.Height = comboBox.Height + (this.borderSize * 2);
            }
        }

        private void Button_Paint(object? sender, PaintEventArgs e)
        {
            //Fields
            int iconWidht = 14;
            int iconHeight = 6;
            var rectIcon = new Rectangle((this.button.Width - iconWidht) / 2, (button.Height - iconHeight) / 2, iconWidht, iconHeight);
            var graphics = e.Graphics;

            //Draw arrow down icon
            using GraphicsPath iconPath = new();
            using Pen iconPen = new(iconColor, 2);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            iconPath.AddLine(rectIcon.X, rectIcon.Y, rectIcon.X + (iconWidht / 2), rectIcon.Bottom);
            iconPath.AddLine(rectIcon.X + (iconWidht / 2), rectIcon.Bottom, rectIcon.Right, rectIcon.Y);
            graphics.DrawPath(iconPen, iconPath);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (DesignMode)
            {
                AdjustComboBoxDimensions();
            }
        }

        private void RJComboBox_Load(object? sender, EventArgs e)
        {
            AdjustComboBoxDimensions();
        }
    }
}
