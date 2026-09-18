using System.ComponentModel;
using System.Drawing.Drawing2D;
using TiaUtilities.Utility;
using static TiaUtilities.Utility.GraphicsUtils;

namespace TiaUtilities.CustomControls
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
        public class CellStyle
        {
            public required int Column { get; init; }
            public required int Row { get; init; }
            public int ColumnSpan { get => _columnSpan; set { _columnSpan = value; this.TableLayoutPanel?.Invalidate(); } }
            public int RowSpan { get => _rowSpan; set { _rowSpan = value; this.TableLayoutPanel?.Invalidate(); } }

            public Color BackColor { get => _backColor; set { _backColor = value; this.TableLayoutPanel?.Invalidate(); } }

            public Color BorderColor { get => _borderColor; set { _borderColor = value; this.TableLayoutPanel?.Invalidate(); } }
            public int BorderWidth { get => _borderWidth; set { _borderWidth = value; this.TableLayoutPanel?.Invalidate(); } }
            public BorderRadius BorderRadius { get => _borderRadius; set { _borderRadius = value; this.TableLayoutPanel?.Invalidate(); } }

            public Padding Padding { get => _padding; set { _padding = value; this.TableLayoutPanel?.Invalidate(); } }

            public bool FitToControls { get => _fitToControls; set { _fitToControls = value; this.TableLayoutPanel?.Invalidate(); } }

            public Rectangle Bounds { get; internal set; } = Rectangle.Empty;
            public bool MouseInside { get; internal set; } = false;
            public Action<CellStyle>? MouseEnterCallback { get; set; }
            public Action<CellStyle>? MouseLeaveCallback { get; set; }

            private Color _backColor = Color.Transparent;
            private Color _borderColor = Color.Transparent;
            private int _borderWidth = 1;
            private BorderRadius _borderRadius = new(3);
            private int _columnSpan = 1;
            private int _rowSpan = 1;
            private bool _fitToControls = true;
            private Padding _padding = new(0);

            internal TableLayoutPanelColorizable? TableLayoutPanel { get; set; }

            internal void MouseState(bool newState)
            {
                if (newState && !this.MouseInside)
                {
                    this.MouseEnterCallback?.Invoke(this);
                }
                else if (!newState && this.MouseInside)
                {
                    this.MouseLeaveCallback?.Invoke(this);
                }

                this.MouseInside = newState;
            }
        }


        private readonly HashSet<CellStyle> cellStyles = [];

        public TableLayoutPanelColorizable()
        {
            SetStyle(ControlStyles.Selectable | 
                ControlStyles.OptimizedDoubleBuffer | 
                ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.ResizeRedraw, true);
            this.DoubleBuffered = true;
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

        public void AddCellStyle(CellStyle cellStyle)
        {
            this.cellStyles.Add(cellStyle);
            cellStyle.TableLayoutPanel = this;

            this.Invalidate();
        }

        public bool HasCellStyle(CellStyle cellStyle) => this.cellStyles.Contains(cellStyle);

        public void RemoveCellStyle(CellStyle cellStyle)
        {
            this.cellStyles.Remove(cellStyle);
            cellStyle.TableLayoutPanel = null;

            this.Invalidate();
        }

        public void ClearCellStyles()
        {
            foreach(var cellStyle in this.cellStyles)
            {
                cellStyle.TableLayoutPanel = null;
            }

            this.cellStyles.Clear();
            this.Invalidate();
        }

        public bool PreFilterMessage(ref Message m)
        {
            if(this.IsDisposed)
            {
                return false;
            }

            if (m.Msg == DllImports.WM_MOUSEMOVE)
            {
                Point clientPoint = this.PointToClient(Cursor.Position);

                foreach (var style in this.cellStyles)
                {
                    var newMouseInside = style.Bounds.Contains(clientPoint);
                    style.MouseState(newMouseInside);
                }
            }

            return false;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            this.DrawCellStyles(e.Graphics);
        }

        private void DrawCellStyles(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

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

                    if(borderRect.Width > 0 && borderRect.Height > 0)
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

    }
}

/*
 
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            /*
            switch (m.Msg)
            {
                case DllImports.WM_PAINT:
                case DllImports.WM_ERASEBKGND:
                case DllImports.WM_NCCALCSIZE:
                    if (DesignMode || !AutoScroll)
                    {
                        break;
                    }

                    //DllImports.ShowScrollBar(this.Handle, DllImports.SB_SHOW_BOTH, false);
                    break;
                case DllImports.WM_MOUSEWHEEL:
                    // Handle Mouse Wheel for other specific cases
                    int delta = (int)(m.WParam.ToInt64() >> 16);
                    int direction = Math.Sign(delta);
                    //DllImports.ShowScrollBar(this.Handle, DllImports.SB_SHOW_BOTH, false);
                    break;
            }
        }

        public bool PreFilterMessage(ref Message m)
        {
                    var horScroll = ControlUtils.WncProcHorizontalScrollWheel(m);
                    if (horScroll != 0)
                    {
                        DllImports.SendMessage(this.Handle, DllImports.WM_MOUSEWHEEL, m.WParam, m.LParam);
                        return true;
                    }

                    var mousePosition = Control.MousePosition;
                    switch (m.Msg)
                    {
                        case DllImports.WM_MOUSEWHEEL:
                        case DllImports.WM_MOUSEHWHEEL:
                            if (base.DesignMode || !base.AutoScroll)
                            {
                                return false;
                            }

                            if (base.VerticalScroll.Maximum <= ClientSize.Height)
                            {
                                return false;
                            }

                            // Should also check whether the ForegroundWindow matches the parent Form.
                            if (base.RectangleToScreen(ClientRectangle).Contains(mousePosition))
                            {
                                DllImports.SendMessage(this.Handle, DllImports.WM_MOUSEWHEEL, m.WParam, m.LParam);
                                return true;
                            }
                            break;
                        case DllImports.WM_LBUTTONDOWN:
                            // Pre-handle Left Mouse clicks for all child Controls
                            if (RectangleToScreen(ClientRectangle).Contains(mousePosition))
                            {
                                // Inside our bounds but it's not our window
                                if (base.TopLevelControl == null || DllImports.GetForegroundWindow() != base.TopLevelControl.Handle)
                                {
                                    return false;
                                }

                                // The hosted Control that contains the mouse pointer 
                                var ctrl = FromHandle(DllImports.ChildWindowFromPoint(this.Handle, PointToClient(mousePosition)));
                                // A child Control of the hosted Control that will be clicked 
                                // If no child Controls at that position the Parent's handle
                                var child = FromHandle(DllImports.WindowFromPoint(mousePosition));
                            }
                            return false;
                            // Eventually, if you don't want the message to reach the child Control
                            // return true; 
                    }
            return false;
        }

*/