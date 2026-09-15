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

            using var path = CreateRoundedRectanglePath(bounds, cornerRadius);
            graphics.SmoothingMode = SmoothingMode.AntiAlias; // Rende i bordi lisci
            graphics.DrawPath(pen, path);
        }

        public static void FillRoundedRectangle(Graphics graphics, Brush brush, Rectangle bounds, BorderRadius cornerRadius)
        {
            Validate.NotNull(graphics);
            Validate.NotNull(brush);

            using var path = CreateRoundedRectanglePath(bounds, cornerRadius);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.FillPath(brush, path);
        }

        private static GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, BorderRadius radii)
        {
            GraphicsPath path = new();

            if (radii.TopLeft == 0 && radii.TopRight == 0 && radii.BottomLeft == 0 && radii.BottomRight == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            if (radii.TopLeft > 0)
            {
                int diameter = radii.TopLeft * 2;
                Rectangle arc = new(bounds.Left, bounds.Top, diameter, diameter);
                path.AddArc(arc, 180, 90);
            }
            else
            {
                path.AddLine(bounds.Left, bounds.Top, bounds.Left, bounds.Top);
            }

            if (radii.TopRight > 0)
            {
                int diameter = radii.TopRight * 2;
                Rectangle arc = new(bounds.Right - diameter, bounds.Top, diameter, diameter);
                path.AddArc(arc, 270, 90);
            }
            else
            {
                path.AddLine(bounds.Right, bounds.Top, bounds.Right, bounds.Top);
            }

            if (radii.BottomRight > 0)
            {
                int diameter = radii.BottomRight * 2;
                Rectangle arc = new(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter);
                path.AddArc(arc, 0, 90);
            }
            else
            {
                path.AddLine(bounds.Right, bounds.Bottom, bounds.Right, bounds.Bottom);
            }

            if (radii.BottomLeft > 0)
            {
                int diameter = radii.BottomLeft * 2;
                Rectangle arc = new(bounds.Left, bounds.Bottom - diameter, diameter, diameter);
                path.AddArc(arc, 90, 90);
            }
            else
            {
                path.AddLine(bounds.Left, bounds.Bottom, bounds.Left, bounds.Bottom);
            }

            path.CloseFigure();
            return path;
        }
    }
}
