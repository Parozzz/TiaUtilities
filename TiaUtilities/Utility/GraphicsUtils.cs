using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Utility
{
    public static class GraphicsUtils
    {
        public class BorderRadius(int topLeft, int topRight, int bottomLeft, int bottomRight)
        {
            public int TopLeft { get; init; } = topLeft;
            public int TopRight { get; init; } = topRight;
            public int BottomLeft { get; init; } = bottomLeft;
            public int BottomRight { get; init; } = bottomRight;

            public BorderRadius(int all) : this(all, all, all, all) { }
            public BorderRadius(int top, int bottom) : this(top, top, bottom, bottom) { }
        }

        public static void DrawRoundedRectangle(Graphics graphics, Pen pen, Rectangle bounds, BorderRadius cornerRadius)
        {
            Validate.NotNull(graphics);
            Validate.NotNull(pen);

            graphics.SmoothingMode = SmoothingMode.AntiAlias; // Rende i bordi lisci
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using var path = CreateRoundedRectanglePath(bounds, cornerRadius, borderWidth: pen.Width);
            graphics.DrawPath(pen, path);
        }

        public static void FillRoundedRectangle(Graphics graphics, Brush brush, Rectangle bounds, BorderRadius cornerRadius)
        {
            Validate.NotNull(graphics);
            Validate.NotNull(brush);

            graphics.SmoothingMode = SmoothingMode.AntiAlias; // Rende i bordi lisci
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using var path = CreateRoundedRectanglePath(bounds, cornerRadius);
            graphics.FillPath(brush, path);
        }

        public static GraphicsPath CreateRoundedRectanglePath(RectangleF bounds, BorderRadius radii, float borderWidth = 1f)
        {
            GraphicsPath path = new();

            // Rientro per allineare la penna perfettamente all'interno del controllo
            float halfPen = borderWidth / 2f;

            float x = bounds.X + halfPen;
            float y = bounds.Y + halfPen;
            float width = bounds.Width - borderWidth;
            float height = bounds.Height - borderWidth;

            if (width <= 0 || height <= 0)
            {
                return path;
            }

            float right = x + width;
            float bottom = y + height;

            // Limite di sicurezza per evitare sovrapposizione degli archi
            float maxRadius = Math.Min(width, height) / 2f;
            float rTL = Math.Min(radii.TopLeft, maxRadius);
            float rTR = Math.Min(radii.TopRight, maxRadius);
            float rBR = Math.Min(radii.BottomRight, maxRadius);
            float rBL = Math.Min(radii.BottomLeft, maxRadius);

            // 1. TOP-LEFT (Arco o Angolo retto)
            if (rTL > 0)
            {
                path.AddArc(x, y, rTL * 2f, rTL * 2f, 180, 90);
            }
            else
            {
                path.AddLine(x, y, x + rTR, y); // Inizia la linea superiore
            }

            // Linea Superiore (tra TL e TR)
            if (rTL > 0 || rTR > 0)
            {
                path.AddLine(x + rTL, y, right - rTR, y);
            }

            // 2. TOP-RIGHT (Arco o Angolo retto)
            if (rTR > 0)
            {
                path.AddArc(right - (rTR * 2f), y, rTR * 2f, rTR * 2f, 270, 90);
            }

            // Linea Destra (tra TR e BR)
            if (rTR > 0 || rBR > 0)
            {
                path.AddLine(right, y + rTR, right, bottom - rBR);
            }

            // 3. BOTTOM-RIGHT (Arco o Angolo retto)
            if (rBR > 0)
            {
                path.AddArc(right - (rBR * 2f), bottom - (rBR * 2f), rBR * 2f, rBR * 2f, 0, 90);
            }

            // Linea Inferiore (tra BR e BL)
            if (rBR > 0 || rBL > 0)
            {
                path.AddLine(right - rBR, bottom, x + rBL, bottom);
            }

            // 4. BOTTOM-LEFT (Arco o Angolo retto)
            if (rBL > 0)
            {
                path.AddArc(x, bottom - (rBL * 2f), rBL * 2f, rBL * 2f, 90, 90);
            }

            // Linea Sinistra (tra BL e TL) -> Chiude la figura connettendosi al punto di partenza
            path.AddLine(x, bottom - rBL, x, y + rTL);

            // NOTA: NESSUNA chiamata a path.CloseFigure() !

            return path;
        }
    }
}
