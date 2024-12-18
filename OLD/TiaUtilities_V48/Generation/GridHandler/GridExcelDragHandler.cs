﻿using System;
using System.Drawing;
using System.Windows.Forms;
using static TiaXmlReader.Generation.GridHandler.GridCellPaintHandler;
using TiaXmlReader.Generation.GridHandler;

namespace TiaXmlReader.Generation.GridHandler
{
    public class GridExcelDragHandler : IGridCellPainter
    {
        public const int TRIANGLE_SIZE = 13;
        public class DragData
        {
            public DataGridView DataGridView { get; set; }
            public int StartingRow { get; set; }
            public int ActualRow { get; set; }
            public uint SelectedRowCount { get; set; }
            public int DraggedColumn { get; set; }
            public bool DraggingDown { get; set; }
            public int TopSelectedRow { get; set; } //Lowest index number
            public int BottomSelectedRow { get; set; } //Highest index number
            public string TooltipString { get; set; } //Only used for preview
        }

        private readonly DataGridView dataGridView;
        private readonly GridSettings settings;

        private bool started = false;
        private int rowIndexStart = -1;
        private int draggedColumnIndex = -1;

        private int topSelectionRowIndex = -1; //Lowest
        private int bottomSelectionRowIndex = -1; //Highest
        private uint SelectedRows { get => (uint)(bottomSelectionRowIndex - topSelectionRowIndex) + 1; }

        private Action<DragData> previewAction;
        private Action<DragData> mouseUpAction;

        private ToolTip dragToolTip;

        public bool DraggingDown { get => topSelectionRowIndex == rowIndexStart; }

        public GridExcelDragHandler(DataGridView dataGridView, GridSettings settings)
        {
            this.dataGridView = dataGridView;
            this.settings = settings;
        }

        public bool IsStarted()
        {
            return started;
        }

        public void Init()
        {
            dataGridView.MouseMove += (sender, args) =>
            {
                if (started)
                {
                    return;
                }

                dataGridView.Cursor = Cursors.Default;

                if (IsInsideTriangle(args.X, args.Y, dataGridView.CurrentCell))
                {
                    dataGridView.Cursor = Cursors.Cross;
                }
            };

            this.dataGridView.LostFocus += delegate { this.Clear(); };

            this.dataGridView.MouseDown += (sender, args) =>
            {
                var hitTest = dataGridView.HitTest(args.X, args.Y);
                if (hitTest.Type == DataGridViewHitTestType.Cell && args.Button == MouseButtons.Left)
                {
                    var hitCell = dataGridView.Rows[hitTest.RowIndex].Cells[hitTest.ColumnIndex];
                    if (hitCell == dataGridView.CurrentCell && IsInsideTriangle(args.X, args.Y, hitCell))
                    {
                        started = true;
                        bottomSelectionRowIndex = topSelectionRowIndex = rowIndexStart = hitTest.RowIndex;
                        draggedColumnIndex = hitTest.ColumnIndex;
                    }
                }
            };

            this.dataGridView.MouseUp += (sender, args) =>
            {
                if (started)
                {
                    started = false;

                    if (this.mouseUpAction != null)
                    {
                        var data = CreateDragData();
                        mouseUpAction.Invoke(data);
                    }

                    this.Clear();
                }
            };

            dataGridView.SelectionChanged += (sender, args) =>
            {
                if (!started)
                {
                    return;
                }

                var highestRowIndex = int.MinValue;
                var lowestRowIndex = int.MaxValue;

                //When dragging, only allow cell on the column to be selected! I don't care about other ways and want it simple.
                foreach (DataGridViewCell selectedCell in dataGridView.SelectedCells)
                {
                    bool sameColumn = selectedCell.ColumnIndex == draggedColumnIndex;
                    if (!sameColumn)
                    {
                        selectedCell.Selected = false; //Only keep the ones on the same columns of the selected!
                        continue;
                    }

                    highestRowIndex = Math.Max(highestRowIndex, selectedCell.RowIndex);
                    lowestRowIndex = Math.Min(lowestRowIndex, selectedCell.RowIndex);
                }

                topSelectionRowIndex = lowestRowIndex;
                bottomSelectionRowIndex = highestRowIndex;

                if (dragToolTip == null)
                {
                    dragToolTip = new ToolTip
                    {
                        Active = true,
                        BackColor = Color.DarkGray,
                        ForeColor = Color.Black,
                        ShowAlways = true,
                    };
                }

                if (this.previewAction != null)
                {
                    var data = CreateDragData();
                    previewAction.Invoke(data);
                    if (!string.IsNullOrEmpty(data.TooltipString))
                    {
                        dragToolTip.Show(data.TooltipString, dataGridView.FindForm(), dataGridView.FindForm().PointToClient(Cursor.Position));
                    }
                }
            };
        }

        private DragData CreateDragData()
        {
            return new DragData()
            {
                DataGridView = this.dataGridView,
                StartingRow = this.rowIndexStart,
                SelectedRowCount = this.SelectedRows,
                DraggedColumn = this.draggedColumnIndex,
                DraggingDown = this.DraggingDown,
                TopSelectedRow = this.topSelectionRowIndex,
                BottomSelectedRow = this.bottomSelectionRowIndex,
                TooltipString = ""
            };
        }

        public GridExcelDragHandler SetPreviewAction(Action<DragData> action)
        {
            this.previewAction = action;
            return this;
        }

        public GridExcelDragHandler SetMouseUpAction(Action<DragData> action)
        {
            this.mouseUpAction = action;
            return this;
        }

        public PaintRequest PaintCellRequest(DataGridViewCellPaintingEventArgs args)
        {
            var paintRequest = new PaintRequest();

            var columnIndex = args.ColumnIndex;
            var rowIndex = args.RowIndex;

            if (columnIndex >= 0 && rowIndex >= 0)
            {
                if (started && this.draggedColumnIndex == columnIndex)
                {
                    return paintRequest.Background();
                }

                var currentCell = dataGridView.CurrentCell;
                if (currentCell != null && currentCell.RowIndex == rowIndex && currentCell.ColumnIndex == columnIndex && dataGridView.SelectedCells.Count == 1)
                {
                    return paintRequest.Background();
                }
            }

            return paintRequest;
        }

        public void PaintCell(DataGridViewCellPaintingEventArgs args, PaintRequest paintRequest, bool backgroundRequested)
        {
            if (args.Handled)
            {
                return;
            }

            var bounds = args.CellBounds;
            var graphics = args.Graphics;

            var rowIndex = args.RowIndex;
            var columnIndex = args.ColumnIndex;

            var style = args.CellStyle;

            if (rowIndex < 0 || columnIndex < 0)
            {
                return;
            }
            else if (started)
            {
                style.SelectionBackColor = this.settings.DragSelectedCellBorderColor;

                args.PaintBackground(bounds, true);
                return;
            }

            var currentCell = dataGridView.CurrentCell;
            if (currentCell != null && currentCell.RowIndex == rowIndex && currentCell.ColumnIndex == columnIndex && dataGridView.SelectedCells.Count == 1)
            {//I only want to apply the effect when the only selected cell is the current cell.
                args.PaintBackground(bounds, true);

                //args.Paint(bounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                using (var pen = new Pen(this.settings.SingleSelectedCellBorderColor, 2))
                {//Border
                    Rectangle rect = args.CellBounds;
                    rect.Width -= 1;
                    rect.Height -= 1;
                    graphics.DrawRectangle(pen, rect);
                }

                if (currentCell is DataGridViewTextBoxCell)
                {
                    using (var brush = new SolidBrush(this.settings.SelectedCellTriangleColor))
                    {//Little triangle in the lower part only for current cell
                        var point1 = new Point(bounds.Right - 1, bounds.Bottom - TRIANGLE_SIZE);
                        var point2 = new Point(bounds.Right - 1, bounds.Bottom - 1);
                        var point3 = new Point(bounds.Right - TRIANGLE_SIZE, bounds.Bottom - 1);

                        Point[] pt = new Point[] { point1, point2, point3 };
                        graphics.FillPolygon(brush, pt);
                    }
                }
            }
        }

        public bool IsInsideTriangle(int x, int y, DataGridViewCell cell)
        {
            if (cell == null || !(cell is DataGridViewTextBoxCell))
            {
                return false;
            }

            var bounds = dataGridView.GetCellDisplayRectangle(cell.ColumnIndex, cell.RowIndex, false);
            return x >= bounds.Right - TRIANGLE_SIZE && x <= bounds.Right + 2 //Go outside a bit of the cell to avoid misclick that sometime happend
                && y >= bounds.Bottom - TRIANGLE_SIZE && y <= bounds.Bottom + 2;
        }

        private void Clear()
        {
            started = false;
            rowIndexStart = draggedColumnIndex = topSelectionRowIndex = bottomSelectionRowIndex = -1;

            if (dragToolTip != null)
            {
                dragToolTip.Active = false;
                dragToolTip.Dispose();
                dragToolTip = null;
            }
        }
    }
}
