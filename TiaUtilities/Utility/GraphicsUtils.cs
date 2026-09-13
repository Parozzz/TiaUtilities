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
        public static void DrawRoundedRectangle(Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
        {
            Validate.NotNull(graphics);
            Validate.NotNull(pen);

            using var path = CreateRoundedRectanglePath(bounds, cornerRadius);
            graphics.SmoothingMode = SmoothingMode.AntiAlias; // Rende i bordi lisci
            graphics.DrawPath(pen, path);
        }

        public static void FillRoundedRectangle(Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
        {
            Validate.NotNull(graphics);
            Validate.NotNull(brush);

            using var path = CreateRoundedRectanglePath(bounds, cornerRadius);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.FillPath(brush, path);
        }

        private static GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new(diameter, diameter);
            Rectangle arc = new(bounds.Location, size);

            GraphicsPath path = new();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // Angolo in alto a sinistra
            path.AddArc(arc, 180, 90);

            // Angolo in alto a destra
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Angolo in basso a destra
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Angolo in basso a sinistra
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}
