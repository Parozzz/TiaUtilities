using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;
using static TiaUtilities.Utility.GraphicsUtils;

namespace TiaUtilities.CustomControls.tableColorizable
{
    [ProvideProperty("ColumnSpan", typeof(Control))]
    [ProvideProperty("RowSpan", typeof(Control))]
    [ProvideProperty("Row", typeof(Control))]
    [ProvideProperty("Column", typeof(Control))]
    [ProvideProperty("CellPosition", typeof(Control))]
    [DefaultProperty(nameof(ColumnCount))]
    [Docking(DockingBehavior.Never)]
    public class TableLayoutPanelColorizable : TableLayoutPanel, IMessageFilter
    {

        private readonly HashSet<TableCellStyle> cellStyles = [];
        private TableDynamicCellStyle? dynamicCellStyle;

        public TableLayoutPanelColorizable()
        {
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.Selectable |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Application.AddMessageFilter(this);

            VerticalScroll.LargeChange = 30;
            VerticalScroll.SmallChange = 5;
            HorizontalScroll.LargeChange = 30;
            HorizontalScroll.SmallChange = 5;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            Application.RemoveMessageFilter(this);
        }

        public void SetDynamicCellStyle(TableDynamicCellStyle cellStyle) => this.dynamicCellStyle = cellStyle;

        public void AddCellStyle(TableCellStyle cellStyle)
        {
            this.cellStyles.Add(cellStyle);
            cellStyle.TableLayoutPanel = this;

            this.Invalidate();
        }

        public bool HasCellStyle(TableCellStyle cellStyle) => this.cellStyles.Contains(cellStyle);

        public void RemoveCellStyle(TableCellStyle cellStyle)
        {
            this.cellStyles.Remove(cellStyle);
            cellStyle.TableLayoutPanel = null;

            this.Invalidate();
        }

        public void ClearCellStyles(bool includeDynamic = true)
        {
            foreach (var cellStyle in this.cellStyles)
            {
                cellStyle.TableLayoutPanel = null;
            }

            this.cellStyles.Clear();
            this.Invalidate();

            if (includeDynamic)
            {
                this.dynamicCellStyle = null;
            }
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (this.IsDisposed)
            {
                return false;
            }
            else if(!this.Visible)
            {
                this.DynamicStyleOutsideRange();
                return false;
            }

            if (m.Msg == DllImports.WM_MOUSEMOVE)
            {
                Point clientPoint = this.PointToClient(Cursor.Position);

                var cursorInside = this.ClientRectangle.Contains(clientPoint);
                if (!cursorInside)
                {
                    this.DynamicStyleOutsideRange();
                    return false;
                }

                foreach (var style in this.cellStyles)
                {
                    var newMouseInside = style.Bounds.Contains(clientPoint);
                    style.MouseState(newMouseInside);
                }

                this.FilterDynamicCellStyle(clientPoint);
            } 
            else if(m.Msg == DllImports.WM_NCMOUSEMOVE)
            {
                this.DynamicStyleOutsideRange();
                this.cellStyles.ForEach(c => c.MouseState(false));
            }

            return false;
        }

        private void FilterDynamicCellStyle(Point clientPoint)
        {
            if (this.dynamicCellStyle == null)
            {
                return;
            }

            var hoverCell = this.GetCellFromPoint(clientPoint);
            if (this.dynamicCellStyle.CurrentRow == hoverCell.Row)
            {
                return;
            }

            var firstColumn = this.dynamicCellStyle.Column;
            var lastColumn = this.dynamicCellStyle.Column + this.dynamicCellStyle.ColumnSpan - 1;

            if (hoverCell.Column < firstColumn || hoverCell.Column > lastColumn || hoverCell.Row < this.dynamicCellStyle.StartRow)
            {
                this.DynamicStyleOutsideRange();
                return;
            }

            this.dynamicCellStyle.RowChangedCallback?.Invoke((this.dynamicCellStyle, this.dynamicCellStyle.CurrentRow, hoverCell.Row));
            this.dynamicCellStyle.CurrentRow = hoverCell.Row;

            this.Invalidate();
        }

        private void DynamicStyleOutsideRange()
        {
            if (this.dynamicCellStyle != null && this.dynamicCellStyle.CurrentRow >= 0)
            {
                this.dynamicCellStyle.RowChangedCallback?.Invoke((this.dynamicCellStyle, this.dynamicCellStyle.CurrentRow, -1));
                this.dynamicCellStyle.CurrentRow = -1;
                this.Invalidate();
            }
        }

        /*
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if(this.dynamicCellStyle != null && this.dynamicCellStyle.CurrentRow >= 0)
            {
                this.dynamicCellStyle.RowChangedCallback?.Invoke((this.dynamicCellStyle, this.dynamicCellStyle.CurrentRow, -1));
                this.dynamicCellStyle.CurrentRow = -1;
                this.Invalidate();
            }
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            if (this.dynamicCellStyle != null && this.dynamicCellStyle.CurrentRow >= 0)
            {
                this.dynamicCellStyle.RowChangedCallback?.Invoke((this.dynamicCellStyle, this.dynamicCellStyle.CurrentRow, -1));
                this.dynamicCellStyle.CurrentRow = -1;
                this.Invalidate();
            }
        }
        */
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            this.DrawDynamicCellStyle(e.Graphics);
            this.DrawCellStyles(e.Graphics);
        }

        private void DrawDynamicCellStyle(Graphics g)
        {
            if (this.dynamicCellStyle == null || this.dynamicCellStyle.CurrentRow < 0)
            {
                return;
            }

            var style = this.dynamicCellStyle;

            int column = style.Column;
            int row = style.CurrentRow;

            Rectangle targetRect;
            if (style.FitToControls)
            {
                targetRect = GetBoundsFromControls(column, row, style.ColumnSpan, style.RowSpan);
                if (targetRect.IsEmpty)
                {
                    targetRect = GetSpannedCellBounds(column, row, style.ColumnSpan, style.RowSpan);
                }
            }
            else
            {
                targetRect = GetSpannedCellBounds(column, row, style.ColumnSpan, style.RowSpan);
            }

            var p = style.Padding;
            targetRect = Rectangle.FromLTRB(
                targetRect.Left - p.Left,
                targetRect.Top - p.Top,
                targetRect.Right + p.Right,
                targetRect.Bottom + p.Bottom
            );

            style.Bounds = targetRect;

            var noBackColor = style.BackColor.IsEmpty || style.BackColor.A == 0;
            var noBorder = style.BorderColor.IsEmpty || style.BorderColor.A == 0 || style.BorderWidth <= 0;
            if (targetRect.Width <= 0 || targetRect.Height <= 0 || (noBackColor && noBorder))
            {
                return;
            }

            if (!noBackColor)
            {
                using Brush brush = new SolidBrush(style.BackColor);
                g.FillRectangle(brush, targetRect);
            }

            if (!noBorder)
            {
                Rectangle borderRect = new(targetRect.X,
                    targetRect.Y,
                    targetRect.Width - 1,
                    targetRect.Height - 1
                );

                if (borderRect.Width > 0 && borderRect.Height > 0)
                {
                    using Pen pen = new(style.BorderColor, style.BorderWidth) { Alignment = PenAlignment.Inset };
                    GraphicsUtils.DrawRoundedRectangle(g, pen, borderRect, style.BorderRadius);
                }
            }
        }

        private void DrawCellStyles(Graphics g)
        {
            foreach (var style in this.cellStyles)
            {
                int startCol = style.Column;
                int startRow = style.Row;

                Rectangle targetRect;

                if (style.FitToControls)
                {
                    targetRect = GetBoundsFromControls(startCol, startRow, style.ColumnSpan, style.RowSpan);

                    if (targetRect.IsEmpty)
                    {
                        targetRect = GetSpannedCellBounds(startCol, startRow, style.ColumnSpan, style.RowSpan);
                    }
                }
                else
                {
                    targetRect = GetSpannedCellBounds(startCol, startRow, style.ColumnSpan, style.RowSpan);
                }

                var p = style.Padding;
                targetRect = Rectangle.FromLTRB(
                    targetRect.Left - p.Left,
                    targetRect.Top - p.Top,
                    targetRect.Right + p.Right,
                    targetRect.Bottom + p.Bottom
                );

                style.Bounds = targetRect;

                var noBackColor = style.BackColor.IsEmpty || style.BackColor.A == 0;
                var noBorder = style.BorderColor.IsEmpty || style.BorderColor.A == 0 || style.BorderWidth <= 0;
                if (targetRect.Width <= 0 || targetRect.Height <= 0 || (noBackColor && noBorder))
                {
                    continue;
                }


                // 1. Sfondo
                if (!noBackColor)
                {
                    using Brush brush = new SolidBrush(style.BackColor);
                    g.FillRectangle(brush, targetRect);
                }

                // 2. Bordo
                if (!noBorder)
                {
                    // Definisce il rettangolo di bordo corretto considerando lo spessore della penna
                    Rectangle borderRect = new(
                        targetRect.X,
                        targetRect.Y,
                        targetRect.Width - 1,
                        targetRect.Height - 1
                    );

                    if (borderRect.Width > 0 && borderRect.Height > 0)
                    {
                        using Pen pen = new(style.BorderColor, style.BorderWidth) { Alignment = PenAlignment.Inset };
                        GraphicsUtils.DrawRoundedRectangle(g, pen, borderRect, style.BorderRadius);
                    }
                }
            }
        }

        protected override void OnCellPaint(TableLayoutCellPaintEventArgs e)
        {
            base.OnCellPaint(e);
        }

        private Rectangle GetBoundsFromControls(int startCol, int startRow, int colSpan, int rowSpan)
        {
            int minX = int.MaxValue, minY = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue;
            bool hasControls = false;

            int endCol = startCol + colSpan - 1;
            int endRow = startRow + rowSpan - 1;

            // Ispeziona tutti i controlli figli del TableLayoutPanel
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl == null || !ctrl.Visible)
                {
                    continue;
                }

                var pos = this.GetPositionFromControl(ctrl);

                var column = pos.Column;
                var row = pos.Row;
                if (column >= startCol && column <= endCol && row >= startRow && row <= endRow)
                {
                    hasControls = true;

                    var margin = ctrl.Margin;
                    minX = Math.Min(minX, ctrl.Left - margin.Left);
                    minY = Math.Min(minY, ctrl.Top - margin.Top);
                    maxX = Math.Max(maxX, ctrl.Right + margin.Right);
                    maxY = Math.Max(maxY, ctrl.Bottom + margin.Bottom);
                }
            }

            return hasControls ? new Rectangle(minX, minY, maxX - minX, maxY - minY) : Rectangle.Empty;
        }

        private Rectangle GetSpannedCellBounds(int startCol, int startRow, int colSpan, int rowSpan)
        {
            int[] widths = base.GetColumnWidths();
            int[] heights = base.GetRowHeights();

            if (widths.Length == 0 || heights.Length == 0)
            {
                return Rectangle.Empty;
            }

            int x = this.AutoScrollPosition.X;
            for (int i = 0; i < startCol && i < widths.Length; i++)
            {
                x += widths[i];
            }

            int y = this.AutoScrollPosition.Y;
            for (int j = 0; j < startRow && j < heights.Length; j++)
            {
                y += heights[j];
            }

            int w = 0;
            for (int i = startCol; i < startCol + colSpan && i < widths.Length; i++)
            {
                w += widths[i];
            }

            int h = 0;
            for (int j = startRow; j < startRow + rowSpan && j < heights.Length; j++)
            {
                h += heights[j];
            }

            return new Rectangle(x, y, w, h);
        }

        private TableLayoutPanelCellPosition GetCellFromPoint(Point point)
        {
            int[] widths = base.GetColumnWidths();
            int[] heights = base.GetRowHeights();

            int currentX = this.AutoScrollPosition.X;
            int targetCol = -1;

            for (int i = 0; i < widths.Length; i++)
            {
                if (point.X >= currentX && point.X < currentX + widths[i])
                {
                    targetCol = i;
                    break;
                }
                currentX += widths[i];
            }

            int currentY = this.AutoScrollPosition.Y;
            int targetRow = -1;

            for (int j = 0; j < heights.Length; j++)
            {
                if (point.Y >= currentY && point.Y < currentY + heights[j])
                {
                    targetRow = j;
                    break;
                }
                currentY += heights[j];
            }

            return new(targetCol, targetRow);
        }
    }
}