using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace TiaUtilities.CustomControls
{
    [DefaultEvent("TextChanged")]
    public partial class RJTextBox : UserControl
    {
        [Category("RJ Code Advance")]
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
                this.Invalidate();
            }
        }
        private Color borderColor = Color.MediumSlateBlue;

        [Category("RJ Code Advance")]
        public Color BorderFocusColor
        {
            get => borderFocusColor;
            set => borderFocusColor = value;
        }
        private Color borderFocusColor = Color.HotPink;

        [Category("RJ Code Advance")]
        public int BorderSize
        {
            get => borderSize;
            set
            {
                if (value >= 1)
                {
                    borderSize = value;
                    this.Invalidate();
                }
            }
        }
        private int borderSize = 2;

        [Category("RJ Code Advance")]
        public bool Underlined
        {
            get => underlined;
            set
            {
                underlined = value;
                this.Invalidate();
            }
        }
        private bool underlined = false;

        [Category("RJ Code Advance")]
        public Color UnderlineColor
        {
            get => underlineColor;
            set
            {
                underlineColor = value;
                this.Invalidate();
            }
        }
        private Color underlineColor = Color.HotPink;

        [Category("RJ Code Advance")]
        public bool PasswordChar
        {
            get => isPasswordChar;
            set => isPasswordChar = value;
        }
        private bool isPasswordChar = false;

        [Category("RJ Code Advance")]
        public bool Multiline
        {
            get => textBox.Multiline;
            set => textBox.Multiline = value;
        }

        [Category("RJ Code Advance")]
        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                textBox.BackColor = value;
            }
        }

        [Category("RJ Code Advance")]
        public override Color ForeColor
        {
            get => base.ForeColor;
            set
            {
                base.ForeColor = value;
                textBox.ForeColor = value;
            }
        }

        [Category("RJ Code Advance")]
        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                textBox.Font = value;
                if (this.DesignMode)
                {
                    UpdateControlHeight();
                }
            }
        }

        [Category("RJ Code Advance")]
        public override string Text
        {
            get => textBox.Text;
            set => textBox.Text = value;
        }

        [Category("RJ Code Advance")]
        public int BorderRadius
        {
            get => borderRadius;
            set
            {
                if (value >= 0)
                {
                    borderRadius = value;
                    this.Invalidate();//Redraw control
                }
            }
        }
        private int borderRadius = 0;

        [Category("RJ Code Advance")]
        public bool ReadOnly
        {
            get => readOnly;
            set
            {
                readOnly = value;
                this.textBox.ReadOnly = value;
            }
        }
        private bool readOnly = false;

        [Category("RJ Code Advance")]
        public int TextLeftPadding
        {
            get => base.Padding.Left;
            set
            {
                var padding = base.Padding;
                base.Padding = new Padding(value, padding.Top, padding.Right, padding.Bottom);
            }
        }

        [Category("RJ Code Advance")]
        public int TextTopBottomPadding
        {
            get => base.Padding.Left;
            set
            {
                var padding = base.Padding;
                base.Padding = new Padding(padding.Left, value, padding.Right, value);
            }
        }

        public override DockStyle Dock
        {
            get => base.Dock;
            set
            {
                base.Dock = value;
                this.textBox.Dock = value;
            }
        }
        public override AnchorStyles Anchor
        {
            get => base.Anchor;
            set
            {
                base.Anchor = value;
                this.textBox.Anchor = value;
            }
        }
        public HorizontalAlignment TextAlign
        {
            get => this.textBox.TextAlign;
            set => this.textBox.TextAlign = value;
        }
        public ScrollBars ScrollBars
        {
            get => this.textBox.ScrollBars;
            set => this.textBox.ScrollBars = value;
        }
        public override ContextMenuStrip? ContextMenuStrip
        {
            get => this.textBox.ContextMenuStrip;
            set => this.textBox.ContextMenuStrip = value;
        }

        private bool isFocused = false;

        public RJTextBox()
        {
            //Created by designer
            InitializeComponent();
            this.textBox.Click += (sender, args) => this.OnClick(args);
            this.textBox.TextChanged += (sender, args) => this.OnTextChanged(args);
            this.textBox.Enter += (sender, args) =>
            {
                isFocused = true;
                this.Invalidate();
            };
            this.textBox.Leave += (sender, args) =>
            {
                isFocused = false;
                this.Invalidate();
            };
            this.textBox.KeyPress += (sender, args) => this.OnKeyPress(args);
            this.textBox.MouseEnter += (sender, args) => this.OnMouseEnter(args);
            this.textBox.MouseLeave += (sender, args) => this.OnMouseLeave(args);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.DesignMode)
            {
                UpdateControlHeight();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateControlHeight();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;

            if (borderRadius > 1)//Rounded TextBox
            {
                //-Fields
                var rectBorderSmooth = this.ClientRectangle;
                var rectBorder = Rectangle.Inflate(rectBorderSmooth, -borderSize, -borderSize);
                int smoothSize = borderSize > 0 ? borderSize : 1;

                using var pathBorderSmooth = GetFigurePath(rectBorderSmooth, borderRadius);
                using Pen penBorderSmooth = new(this.Parent.BackColor, smoothSize);

                //-Drawing
                this.Region = new Region(pathBorderSmooth);//Set the rounded region of UserControl
                if (borderRadius > 15)
                {
                    SetTextBoxRoundedRegion();//Set the rounded region of TextBox component
                }

                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.DrawPath(penBorderSmooth, pathBorderSmooth); //Draw border smoothing

                if (underlined) //Line Style
                {
                    //Draw border
                    graphics.SmoothingMode = SmoothingMode.None;

                    using var underlinePen = new Pen(underlineColor, borderSize);
                    graphics.DrawLine(underlinePen, 0, this.Height - 1, this.Width, this.Height - 1);
                }
                else //Normal Style
                {
                    using var pathBorder = GetFigurePath(rectBorder, borderRadius - borderSize);

                    using var penBorder = new Pen(isFocused ? borderFocusColor : borderColor, borderSize);
                    penBorder.Alignment = PenAlignment.Center;
                    graphics.DrawPath(penBorder, pathBorder); //Draw border
                }

            }
            else //Square/Normal TextBox
            {
                //Draw border
                this.Region = new Region(this.ClientRectangle);

                if (underlined) //Line Style
                {
                    using var underlinePen = new Pen(underlineColor, borderSize);
                    graphics.DrawLine(underlinePen, 0, this.Height - 1, this.Width, this.Height - 1);
                }
                else //Normal Style
                {
                    using var penBorder = new Pen(isFocused ? borderFocusColor : borderColor, borderSize);
                    penBorder.Alignment = PenAlignment.Inset;
                    graphics.DrawRectangle(penBorder, 0, 0, this.Width - 0.5F, this.Height - 0.5F);
                }
            }
        }
        private static GraphicsPath GetFigurePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new();
            float curveSize = radius * 2F;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void SetTextBoxRoundedRegion()
        {
            GraphicsPath pathTxt;
            if (Multiline)
            {
                pathTxt = GetFigurePath(textBox.ClientRectangle, borderRadius - borderSize);
                textBox.Region = new Region(pathTxt);
            }
            else
            {
                pathTxt = GetFigurePath(textBox.ClientRectangle, borderSize * 2);
                textBox.Region = new Region(pathTxt);
            }
            pathTxt.Dispose();
        }

        private void UpdateControlHeight()
        {
            if (textBox.Multiline == false)
            {
                int txtHeight = TextRenderer.MeasureText("Text", this.Font).Height + 1;
                textBox.Multiline = true;
                textBox.MinimumSize = new Size(0, txtHeight);
                textBox.Multiline = false;

                this.Height = textBox.Height + this.Padding.Top + this.Padding.Bottom;
            }
        }
    }
}
