using System.Drawing.Drawing2D;
using TiaUtilities.Utility;

namespace TiaUtilities.SettingsStep.CustomControls
{
    public class LabelColorizable : Label
    {
        public new Color BackColor { get => _backColor; set { _backColor = value; Invalidate(); } }
        public Color MouseHoverBackColor { get => _mouseHoverBackColor; set { _mouseHoverBackColor = value; Invalidate(); } }
        public Color MouseDownBackColor { get => _mouseDownBackColor; set { _mouseDownBackColor = value; Invalidate(); } }

        public new Color ForeColor { get => _foreColor; set { _foreColor = value; Invalidate(); } }
        public Color MouseHoverForeColor { get => _foreColor; set { _mouseHoverForeColor = value; Invalidate(); } }
        public Color MouseDownForeColor { get => _foreColor; set { _mouseDownForeColor = value; Invalidate(); } }

        public Color BorderColor { get => _borderColor; set { _borderColor = value; Invalidate(); } }
        public int BorderWidth { get => _borderWidth; set { _borderWidth = value; Invalidate(); } }
        public int BorderRadius { get => _borderRadius; set {  _borderRadius = value; Invalidate(); }  }


        private Color _backColor = Color.Transparent;
        private Color _mouseHoverBackColor = Color.MediumSeaGreen;
        private Color _mouseDownBackColor = Color.LightGreen;

        private Color _foreColor = Form.DefaultForeColor;
        private Color _mouseHoverForeColor = Form.DefaultForeColor;
        private Color _mouseDownForeColor = Form.DefaultForeColor;

        private Color _borderColor = Color.Black;
        private int _borderWidth = 1;
        private int _borderRadius = 3;

        private Color _activeBackColor = Color.Transparent;

        public LabelColorizable()
        {
            base.DoubleBuffered = true;
            base.BackColor = Color.Transparent; 
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            this.SelectActiveColors(this._backColor, this._foreColor);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.SelectActiveColors(this._mouseHoverBackColor, this._mouseHoverForeColor);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            this.SelectActiveColors(this._mouseDownBackColor, this._mouseDownForeColor);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            this.SelectActiveColors(this._mouseHoverBackColor, this._mouseHoverForeColor);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            this.SelectActiveColors(this._backColor, this._foreColor);
        }

        private void SelectActiveColors(Color backColor, Color foreColor)
        {
            if (this._activeBackColor != backColor)
            {
                this._activeBackColor = backColor;
                this.Invalidate();
            }

            if(base.ForeColor != foreColor)
            {
                base.ForeColor = foreColor;
                this.Invalidate();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);

            if(this._activeBackColor.A > 0 && this.Width > 0 && this.Height > 0)
            {
                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                var backgroundRect = this.ClientRectangle;
                backgroundRect.Inflate(-this._borderWidth, -this._borderWidth);

                if(backgroundRect.Width > 0 && backgroundRect.Height > 0)
                {
                    using SolidBrush brush = new(this._activeBackColor);
                    GraphicsUtils.FillRoundedRectangle(pevent.Graphics, brush, backgroundRect, new(this._borderRadius));
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this._borderColor.A > 0 && this._borderWidth > 0 && this.Width > 0 && this.Height > 0)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle borderRect = new(
                    this._borderWidth,
                    this._borderWidth,
                    this.ClientRectangle.Width - this._borderWidth - 1,
                    this.ClientRectangle.Height - this._borderWidth - 1
                );

                if(borderRect.Width > 0 && borderRect.Height > 0)
                {
                    using Pen pen = new(this._borderColor, this._borderWidth) { Alignment = PenAlignment.Inset };
                    GraphicsUtils.DrawRoundedRectangle(e.Graphics, pen, borderRect, new(this._borderRadius));
                }

            }
        }
    }
}
