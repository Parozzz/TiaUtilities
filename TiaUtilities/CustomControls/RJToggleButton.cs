using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace CustomControls.RJControls
{
    public class RJToggleButton : CheckBox
    {
        //Fields
        private Color onBackColor = Color.MediumSlateBlue;
        private Color onToggleColor = Color.WhiteSmoke;
        private Color offBackColor = Color.Gray;
        private Color offToggleColor = Color.Gainsboro;
        private Color borderColor = Color.Gray;
        private int borderWidth = 2;
        private int toggleWidthPercentage = 15;

        //Properties
        [Category("RJ Code Advance")]
        public Color OnBackColor
        {
            get => onBackColor;
            set
            {
                this.onBackColor = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public Color OnToggleColor
        {
            get => onToggleColor;
            set
            {
                this.onToggleColor = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public Color OffBackColor
        {
            get => offBackColor;
            set
            {
                this.offBackColor = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public Color OffToggleColor
        {
            get => offToggleColor;
            set
            {
                this.offToggleColor = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                this.borderColor = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public int BorderWidth
        {
            get => this.borderWidth;
            set
            {
                this.borderWidth = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public int ToggleWidthPercentage
        {
            get => this.toggleWidthPercentage;
            set
            {
                this.toggleWidthPercentage = value;
                this.Invalidate();
            }
        }

        [Browsable(false)]
        public override string Text
        {
            get => base.Text;
        }

        //Constructor
        public RJToggleButton()
        {
            this.MinimumSize = new Size(45, 22);
        }

        //Methods
        private GraphicsPath GetFigurePath()
        {
            int arcSize = this.Height - 1;
            Rectangle leftArc = new(0, 0, arcSize, arcSize);
            Rectangle rightArc = new(this.Width - arcSize - 2, 0, arcSize, arcSize);

            GraphicsPath path = new();
            path.StartFigure();
            path.AddArc(leftArc, 90, 180);
            path.AddArc(rightArc, 270, 180);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs args)
        {
            int toggleHeight = this.Height - 5;
            int toggleWidth = (int)Math.Floor(this.Width * this.toggleWidthPercentage / 100d);
            args.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            args.Graphics.Clear(this.Parent.BackColor);

            var path = GetFigurePath();

            using var borderPen = new Pen(this.borderColor, this.borderWidth);
            args.Graphics.DrawPath(borderPen, path);

            if (this.Checked) //ON
            {
                using var backgroundBrush = new SolidBrush(this.onBackColor);
                args.Graphics.FillPath(backgroundBrush, path);

                using var toggleBrush = new SolidBrush(this.onToggleColor);
                args.Graphics.FillEllipse(toggleBrush, new Rectangle(this.Width - this.Height - 1, 2, toggleHeight, toggleHeight));
            }
            else //OFF
            {
                using var backgroundBrush = new SolidBrush(this.offBackColor);
                args.Graphics.FillPath(backgroundBrush, path);

                using var toggleBrush = new SolidBrush(this.offToggleColor);
                args.Graphics.FillEllipse(toggleBrush, new Rectangle(2, 2, toggleHeight, toggleHeight));
            }
        }
    }
}
