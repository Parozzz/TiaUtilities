using TiaUtilities.Utility;

namespace TiaUtilities.SettingsNew
{
    public class SettingsValueNameLabel : Label
    {

        private const int DOT_SIZE = 7;
        private const int DOT_BORDER_SIZE = 2;
        private const int DOT_DISTANCE = 5;

        public class DotInfo
        {
            public required Color Color { get; init; }
            public ToolTip ToolTip { get; init; } = Utils.CreateStandardToolTip();
            public required string ToolTipText { get; init; }

            public Rectangle rect = Rectangle.Empty;

        }

        public List<DotInfo> DotList { get; init; } = [];


        public SettingsValueNameLabel()
        {
            this.DoubleBuffered = true;
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            var point = this.PointToClient(Cursor.Position);
            foreach(var dotInfo in this.DotList)
            {
                
                if(dotInfo.rect == Rectangle.Empty)
                {
                    continue;
                }

                var rect = dotInfo.rect;
                rect.Offset(-DOT_DISTANCE, -DOT_DISTANCE);
                rect.Inflate(DOT_DISTANCE*2, DOT_DISTANCE*2);
                if (rect.Contains(point))
                {
                    dotInfo.ToolTip.Show(dotInfo.ToolTipText, this);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle face = SettingsValueNameLabel.DeflateRect(this.ClientRectangle, this.Padding);
            var measuredText = TextRenderer.MeasureText(this.Text, this.Font, face.Size);

            var sphereX = face.X + measuredText.Width;
            var sphereY = face.Y + measuredText.Height / 2 - DOT_SIZE / 2;
            foreach(var dotInfo in this.DotList)
            {
                var rect = new Rectangle() { X = sphereX, Y = sphereY, Height = DOT_SIZE, Width = DOT_SIZE };

                using var fillingBrush = new SolidBrush(dotInfo.Color);
                e.Graphics.FillEllipse(fillingBrush, rect);

                using Brush borderSolidBrush = new SolidBrush(SettingsFormConstants.MARKER_DOT_BORDER_COLOR);
                using Pen borderPen = new(borderSolidBrush, DOT_BORDER_SIZE);
                e.Graphics.DrawEllipse(borderPen, rect);

                sphereX += DOT_SIZE + DOT_DISTANCE;

                dotInfo.rect = rect;
            }
        }

        private static Rectangle DeflateRect(Rectangle rect, Padding padding)
        {
            rect.X += padding.Left;
            rect.Y += padding.Top;
            rect.Width -= padding.Horizontal;
            rect.Height -= padding.Vertical;
            return rect;
        }
    }
}
