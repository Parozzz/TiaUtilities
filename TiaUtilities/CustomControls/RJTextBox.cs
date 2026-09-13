using System.ComponentModel;
using System.Drawing.Drawing2D;
using TiaUtilities.Utility;

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
                borderColor = value; this.Invalidate();
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
            get => _borderSize;
            set
            {
                if (value >= 1)
                {
                    _borderSize = value;
                    this.Invalidate();
                }
            }
        }


        [Category("Underline")]
        public bool Underlined
        {
            get => _underlined;
            set { _underlined = value; this.Invalidate(); }
        }

        [Category("Underline")]
        public Color UnderlineColor
        {
            get => _underlineColor;
            set { _underlineColor = value; this.Invalidate(); }
        }

        [Category("Underline")]
        public Color UnderlineFocusColor
        {
            get => _underlineFocusColor;
            set { _underlineFocusColor = value; this.Invalidate(); }
        }

        [Category("Underline")]
        public int UnderlineBottomPadding
        {
            get => _underlineBottomPadding;
            set { _underlineBottomPadding = value; this.Invalidate(); }
        }


        [Category("RJ Code Advance")]
        public bool PasswordChar
        {
            get => _isPasswordChar;
            set => _isPasswordChar = value;
        }

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
            set { base.BackColor = value; textBox.BackColor = value; }
        }

        [Category("RJ Code Advance")]
        public override Color ForeColor
        {
            get => base.ForeColor;
            set { base.ForeColor = value; textBox.ForeColor = value; }
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
        public override string? Text
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
                    this.RicalculatePadding();
                }
            }
        }
        private int borderRadius = 0;

        [Category("RJ Code Advance")]
        public bool ReadOnly
        {
            get => _readOnly;
            set { _readOnly = value; this.textBox.ReadOnly = value; }
        }

        [Category("RJ Code Advance")]
        public int TextLeftPadding
        {
            get => _leftTextPadding;
            set { _leftTextPadding = value; this.RicalculatePadding(); }
        }

        [Category("RJ Code Advance")]
        public int TextTopBottomPadding
        {
            get => base.Padding.Top;
            set { _topBottomTextPadding = value; this.RicalculatePadding(); } 
        }

        public override DockStyle Dock
        {
            get => base.Dock;
            set { base.Dock = value; this.textBox.Dock = value; }
        }
        public override AnchorStyles Anchor
        {
            get => base.Anchor;
            set { base.Anchor = value; this.textBox.Anchor = value; }
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

        private int _borderSize = 2;
        private bool _isPasswordChar = false;
        private bool _readOnly = false;

        private bool _underlined = false;
        private Color _underlineColor = Color.FromArgb(127, Color.HotPink);
        private Color _underlineFocusColor = Color.HotPink;
        private int _underlineBottomPadding = 0;

        private int _leftTextPadding = 3;
        private int _topBottomTextPadding = 3;

        public RJTextBox()
        {
            //Created by designer
            InitializeComponent();
            base.DoubleBuffered = true;

            ControlUtils.SetDoubleBuffered(this.textBox);
            ControlUtils.SetStyle(this.textBox, ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint, true);

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

        private void RicalculatePadding()
        {
            var padding = base.Padding;
            base.Padding = new Padding(_leftTextPadding, 
                _topBottomTextPadding + _borderSize, 
                padding.Right, 
                _topBottomTextPadding + _borderSize);

            this.Invalidate();//Redraw control
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
                var rectBorder = Rectangle.Inflate(rectBorderSmooth, -_borderSize, -_borderSize);
                int smoothSize = _borderSize > 0 ? _borderSize : 1;

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

                if (_underlined) //Line Style
                {
                    //Draw border
                    graphics.SmoothingMode = SmoothingMode.None;

                    var color = isFocused ? _underlineFocusColor : _underlineColor;
                    var y = this.Height - _underlineBottomPadding - 1;

                    using var underlinePen = new Pen(color, _borderSize);
                    graphics.DrawLine(underlinePen, 0, y, this.Width, y);
                }
                else //Normal Style
                {
                    using var pathBorder = GetFigurePath(rectBorder, borderRadius - _borderSize);

                    using var penBorder = new Pen(isFocused ? borderFocusColor : borderColor, _borderSize);
                    penBorder.Alignment = PenAlignment.Center;
                    graphics.DrawPath(penBorder, pathBorder); //Draw border
                }

            }
            else //Square/Normal TextBox
            {
                //Draw border
                this.Region = new Region(this.ClientRectangle);

                if (_underlined) //Line Style
                {
                    var color = isFocused ? _underlineFocusColor : _underlineColor;
                    var y = this.Height - _underlineBottomPadding - 1;

                    using var underlinePen = new Pen(color, _borderSize);
                    graphics.DrawLine(underlinePen, 0, y, this.Width, y);
                }
                else //Normal Style
                {
                    using var penBorder = new Pen(isFocused ? borderFocusColor : borderColor, _borderSize);
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
                pathTxt = GetFigurePath(textBox.ClientRectangle, borderRadius - _borderSize);
                textBox.Region = new Region(pathTxt);
            }
            else
            {
                pathTxt = GetFigurePath(textBox.ClientRectangle, _borderSize * 2);
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
