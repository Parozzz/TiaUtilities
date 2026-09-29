using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Styles;
using TiaUtilities.Utility;

namespace TiaUtilities.CustomControls.EditableTab
{
    public class EditableTabAddButton(EditableTabControl owner)
    {
        public class ButtonStyle
        {
            public int Width { get; set; } = 28;

            public Color ForeColor { get; set; } = StyleManager.EditableTabControl.ADD_TAB_FORE_COLOR;
            public Color BackColor { get; set; } = Color.Transparent;
            public Color HoverColor { get; set; } = Color.FromArgb(100, Color.Cyan);

            public Color BorderColor { get; set; } = Color.FromArgb(127, Color.Gray);
            public int BorderWidth { get; set; } = 1;
            public int BorderRadius { get; set; } = 3;

            public Padding Margin { get; set; } = new(3, 1, 3, 5);

            public string Symbol { get; set; } = "➕";
        }

        public ButtonStyle Style { get; init; } = new();

        private Rectangle bounds = Rectangle.Empty;
        private bool isHovered = false;

        internal void DrawAddButton(Graphics g)
        {
            this.bounds = GetAddButtonRect();

            Color backColor = this.isHovered ?
                this.Style.HoverColor :
                this.Style.BackColor;

            var noBack = backColor.IsEmpty || backColor.A == 0;
            var noBorder = this.Style.BorderWidth <= 0 || this.Style.BorderColor.IsEmpty || this.Style.BorderColor.A == 0;
            if (!noBack || !noBorder)
            {
                using var path = GraphicsUtils.CreateRoundedRectanglePath(this.bounds, new(this.Style.BorderRadius));
                if (!noBack)
                {
                    using Brush backBrush = new SolidBrush(backColor);
                    g.FillPath(backBrush, path);
                }

                if (!noBorder)
                {
                    using Pen borderPen = new(this.Style.BorderColor, this.Style.BorderWidth) { Alignment = PenAlignment.Inset };
                    g.DrawPath(borderPen, path);
                }
            }

            using Font font = new(owner.Font.FontFamily, 11f, FontStyle.Bold);
            TextRenderer.DrawText(
                g,
                this.Style.Symbol,
                font,
                this.bounds,
                this.Style.ForeColor,
                Color.Transparent,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.NoClipping
            );
        }

        private Rectangle GetAddButtonRect()
        {
            var margin = this.Style.Margin;
            if (owner.TabCount == 0)
            {
                return new Rectangle(
                    margin.Left,
                    margin.Top,
                    Style.Width - margin.Right,
                    (owner.ItemSize.Height > 0 ? owner.ItemSize.Height : 24) - margin.Bottom
                );
            }

            Rectangle lastTabRect = owner.GetTabRect(owner.TabCount - 1);
            return new Rectangle(
                lastTabRect.Right + margin.Left,
                lastTabRect.Top + margin.Top,
                Style.Width - margin.Right,
                lastTabRect.Height - margin.Bottom
            );
        }


        internal bool PreFilterMessage(Message m)
        {
            if (m.Msg == DllImports.WM_LBUTTONDOWN || m.Msg == DllImports.WM_RBUTTONDOWN)
            {
                var clientPoint = owner.PointToClient(Cursor.Position);

                if (this.bounds.Contains(clientPoint))
                {
                    bool isRightClick = (m.Msg == DllImports.WM_RBUTTONDOWN);
                    owner.AddTabs(isRightClick ? 5 : 1);
                    return true;
                }
            }
            else if (m.Msg == DllImports.WM_MOUSEMOVE)
            {
                var clientPoint = owner.PointToClient(Cursor.Position);

                var contains = this.bounds.Contains(clientPoint);
                if (this.isHovered != contains)
                {
                    this.isHovered = contains;
                    owner.Invalidate();
                    return true;
                }
            }

            return false;
        }
    }
}
