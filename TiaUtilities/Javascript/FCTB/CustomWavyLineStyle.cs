using System.Collections.Generic;
using System.Drawing;
using FastColoredTextBoxNS;

namespace TiaXmlReader.Javascript.FCTB
{
    internal class CustomWavyLineStyle : Style
    {
        private const int WAVE_DISTANCE_FROM_TEXT = -2; // POSITIVE = UP, NEGATIVE = DOWN
        private const int WAVE_MIN_SIZE = 10;
        private const int WAVE_PEN_SIZE = 2;
        private const int WAVE_HEIGHT = 1;
        private const int WAVE_WIDTH = 3;

        private readonly Pen wavePen;
        private readonly Brush shortRangeBrush;

        public CustomWavyLineStyle(Color waveColor, Color shortRangeBackgroundColor)
        {
            wavePen = new Pen(waveColor, WAVE_PEN_SIZE);
            shortRangeBrush = new SolidBrush(shortRangeBackgroundColor);
        }

        public override void Draw(Graphics graphics, Point position, Range range)
        {
            Size sizeOfRange = Style.GetSizeOfRange(range);
            Point start = new Point(position.X, position.Y + sizeOfRange.Height - WAVE_DISTANCE_FROM_TEXT);
            Point end = new Point(position.X + sizeOfRange.Width, position.Y + sizeOfRange.Height - WAVE_DISTANCE_FROM_TEXT);

            if (end.X - start.X < WAVE_MIN_SIZE) //If the text is too short, it will also change color of it to be more evident.
            {
                var rect = new Rectangle(position.X, position.Y, (range.End.iChar - range.Start.iChar) * range.tb.CharWidth, range.tb.CharHeight);
                if (rect.Width != 0)
                {
                    graphics.FillRectangle(shortRangeBrush, rect);
                }

                graphics.DrawLine(wavePen, start, end);
                return;
            }

            DrawWavyLine(graphics, start, end);
        }

        private void DrawWavyLine(Graphics graphics, Point start, Point end)
        {
            int num = -WAVE_HEIGHT;

            var list = new List<Point>();
            for (int i = start.X; i <= end.X; i += WAVE_WIDTH)
            {
                list.Add(new Point(i, start.Y + num));
                num = -num;
            }

            graphics.DrawLines(wavePen, list.ToArray());
        }

        public override void Dispose()
        {
            base.Dispose();
            wavePen?.Dispose();
        }
    }
}

