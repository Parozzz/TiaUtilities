using System.Diagnostics;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridDoDragDropHandler<T>(DataGridView dataGridView, GridDataChangedHandler<T> dataChangedHandler, GridSettings gridSettings) where T : GridData
    {
        public bool Busy { get; private set; } = false;
        //public Point MouseDownLocation { get; private set; } = Point.Empty;
        public Point MouseDownCellAddress { get; private set; } = Point.Empty;

        public void EventCellPainting(Rectangle cellBounds, DataGridViewCellStyle? cellStyle, int columnIndex, int rowIndex)
        {
            if (cellStyle == null)
            {
                return;
            }

            if (Busy)
            {
                if (this.MouseDownCellAddress != Point.Empty && columnIndex == MouseDownCellAddress.X && rowIndex == MouseDownCellAddress.Y)
                {
                    cellStyle.SelectionBackColor = gridSettings.DragDropStartCellSelectedBackColor;
                }

                var cursorPoint = dataGridView.PointToClient(Cursor.Position);

                var hitTest = dataGridView.HitTest(cursorPoint.X, cursorPoint.Y);
                if (hitTest.Type == DataGridViewHitTestType.Cell && hitTest.ColumnIndex == columnIndex && hitTest.RowIndex == rowIndex)
                {
                    cellStyle.BackColor = gridSettings.DragDropStartCellSelectedBackColor;
                    cellStyle.SelectionBackColor = gridSettings.DragDropStartCellSelectedBackColor;
                }
            }

        }

        public bool EventMouseDown(Point location, int columnIndex, int rowIndex)
        {
            var text = GridUtils.GetCopyAsExcelText(dataGridView, out var _);
            text = String.IsNullOrEmpty(text) ? "" : text;

            var cell = dataGridView.Rows[rowIndex].Cells[columnIndex];
            if (!cell.Selected)
            {
                var selectedCells = dataGridView.SelectedCells;

                DataGridViewCell? closestCell = null;
                foreach (DataGridViewCell selectedCell in selectedCells)
                {
                    if (closestCell == null)
                    {
                        closestCell = selectedCell;
                    }
                    else
                    {
                        var closestColumnDiff = Math.Abs(closestCell.ColumnIndex - cell.ColumnIndex);
                        var closesRowDiff = Math.Abs(closestCell.RowIndex - cell.RowIndex);

                        var selectedColumnDiff = Math.Abs(selectedCell.ColumnIndex - cell.ColumnIndex);
                        var selectedRowDiff = Math.Abs(selectedCell.RowIndex - cell.RowIndex);

                        if (selectedColumnDiff < closestColumnDiff || selectedRowDiff < closesRowDiff)
                        {
                            closestCell = selectedCell;
                        }
                    }
                }

                if (closestCell != null)
                {
                    this.MouseDownCellAddress = new(closestCell.ColumnIndex, closestCell.RowIndex);
                    dataGridView.InvalidateCell(closestCell);
                }
            }
            else
            {
                this.MouseDownCellAddress = new(cell.ColumnIndex, cell.RowIndex);
                dataGridView.InvalidateCell(columnIndex, rowIndex);
            }

            if (this.MouseDownCellAddress.IsEmpty)
            {
                return false;
            }

            this.Busy = true;

            try
            {//DoDragDrop seems to block operations in the grid. Everything else must be done b4 the method.
                dataGridView.DoDragDrop(text, DragDropEffects.Move, null, Point.Empty, true);
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            this.Busy = false;
            this.MouseDownCellAddress = Point.Empty;

            return true;
        }

        public DragDropEffects EventDragOver(IDataObject? data)
        {
            var dataString = data?.GetData(typeof(string));
            return dataString is not string ? DragDropEffects.None : DragDropEffects.Move;
        }

        public Point GetDifferenceFromMouseDown(Point p, bool useDisplayIndex = false) => GetDifferenceFromMouseDown(p.X, p.Y, useDisplayIndex);

        public Point GetDifferenceFromMouseDown(int x, int y, bool useDisplayIndex = false)
        {
            if (this.MouseDownCellAddress == Point.Empty)
            {
                return Point.Empty;
            }

            var gridPoint = dataGridView.PointToClient(new(x, y));

            var hitTest = dataGridView.HitTest(gridPoint.X, gridPoint.Y);
            if (hitTest.Type != DataGridViewHitTestType.Cell)
            {
                return Point.Empty;
            }

            var hitColumnIndex = hitTest.ColumnIndex;
            if(useDisplayIndex)
            {
                hitColumnIndex = dataGridView.Columns[hitColumnIndex].DisplayIndex;
            }

            var hitRowIndex = hitTest.RowIndex;
            return new() { X = hitColumnIndex - this.MouseDownCellAddress.X, Y = hitRowIndex - this.MouseDownCellAddress.Y };
        }

        public void EventDragDrop(IDataObject? data, int x, int y)
        {
            try
            {
                var dataObj = data?.GetData(typeof(string));
                if (dataObj is string excelText)
                {
                    var coordDifference = this.GetDifferenceFromMouseDown(x, y, useDisplayIndex: false);
                    if (coordDifference != Point.Empty)
                    {
                        var minRowIndex = dataGridView.SelectedCells.Cast<DataGridViewCell>().Min(c => c.RowIndex);
                        var minColumnIndex = dataGridView.SelectedCells.Cast<DataGridViewCell>().Min(c => c.ColumnIndex);

                        //These two CAN BE NEGATIVE!!
                        var pasteColumnIndex = minColumnIndex + coordDifference.X;
                        var pasteRowIndex = minRowIndex + coordDifference.Y;

                        dataGridView.SuspendLayout();

                        var req = dataChangedHandler.Join();
                        var pastedCellsList = GridUtils.PasteAsExcel(dataGridView, excelText, pasteRowIndex, pasteColumnIndex, ignoreSingleLineMultiplePasting: true);

                        dataGridView.SelectedCells.Cast<DataGridViewCell>()
                            .Except(pastedCellsList)
                            .ForEach(c => c.Value = null); //Clear cell value for cell not pasted

                        dataChangedHandler.End(req);

                        dataGridView.FindForm()?.BeginInvoke(() =>
                        {
                            DllImports.RaiseLeftMouse(Cursor.Position); //This fixes the mouse down stuck after dropping.

                            dataGridView.ClearSelection();

                            var gridPoint = dataGridView.PointToClient(Cursor.Position);
                            var dropHitTest = dataGridView.HitTest(gridPoint.X, gridPoint.Y);
                            if (dropHitTest.Type == DataGridViewHitTestType.Cell)
                            {
                                dataGridView.CurrentCell = dataGridView.Rows[dropHitTest.RowIndex].Cells[dropHitTest.ColumnIndex];
                            }

                            foreach (var cell in pastedCellsList)
                            {
                                cell.Selected = true;
                            }

                            dataGridView.ResumeLayout(performLayout: false);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }
            finally
            {
                this.MouseDownCellAddress = Point.Empty;
            }

        }
    }
}

/*
 *         public bool Busy { get; private set; } = false;
        //public Point MouseDownLocation { get; private set; } = Point.Empty;
        public Point MouseDownCellAddress { get; private set; } = Point.Empty;

        public void EventCellPainting(Rectangle cellBounds, DataGridViewCellStyle? cellStyle, int columnIndex, int rowIndex)
        {
            if (cellStyle == null)
            {
                return;
            }

            if (Busy)
            {
                if (this.MouseDownLocation != Point.Empty && cellBounds.Contains(this.MouseDownLocation))
                {
                    cellStyle.SelectionBackColor = gridSettings.DragDropStartCellSelectedBackColor;
                }

                var cursorPoint = dataGridView.PointToClient(Cursor.Position);

                var hitTest = dataGridView.HitTest(cursorPoint.X, cursorPoint.Y);
                if (hitTest.Type == DataGridViewHitTestType.Cell && hitTest.ColumnIndex == columnIndex && hitTest.RowIndex == rowIndex)
                {
                    cellStyle.BackColor = gridSettings.DragDropStartCellSelectedBackColor;
                    cellStyle.SelectionBackColor = gridSettings.DragDropStartCellSelectedBackColor;
                }
            }

        }

        public bool EventMouseDown(Point location, int columnIndex, int rowIndex)
        {
            var text = GridUtils.GetCopyAsExcelText(dataGridView, out var _);
            text = String.IsNullOrEmpty(text) ? "" : text;

            var cell = dataGridView.Rows[rowIndex].Cells[columnIndex];
            if (!cell.Selected)
            {
                var selectedCells = dataGridView.SelectedCells;

                DataGridViewCell? closestCell = null;
                foreach (DataGridViewCell selectedCell in selectedCells)
                {
                    if (closestCell == null)
                    {
                        closestCell = selectedCell;
                    }
                    else
                    {
                        var closestColumnDiff = Math.Abs(closestCell.ColumnIndex - cell.ColumnIndex);
                        var closesRowDiff = Math.Abs(closestCell.RowIndex - cell.RowIndex);

                        var selectedColumnDiff = Math.Abs(selectedCell.ColumnIndex - cell.ColumnIndex);
                        var selectedRowDiff = Math.Abs(selectedCell.RowIndex - cell.RowIndex);

                        if (selectedColumnDiff < closestColumnDiff || selectedRowDiff < closesRowDiff)
                        {
                            closestCell = selectedCell;
                        }
                    }
                }

                if (closestCell != null)
                {
                    var cellDisplayRect = dataGridView.GetCellDisplayRectangle(closestCell.ColumnIndex, closestCell.RowIndex, true);
                    this.MouseDownLocation = new(cellDisplayRect.X, cellDisplayRect.Y);

                    dataGridView.InvalidateCell(closestCell);
                }
            }
            else
            {
                this.MouseDownLocation = location;

                dataGridView.InvalidateCell(columnIndex, rowIndex);
            }

            if (this.MouseDownLocation == Point.Empty)
            {
                return false;
            }

            this.Busy = true;

            try
            {//DoDragDrop seems to block operations in the grid. Everything else must be done b4 the method.
                dataGridView.DoDragDrop(text, DragDropEffects.Move, null, Point.Empty, true);
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            this.Busy = false;
            this.MouseDownLocation = Point.Empty;

            return true;
        }

        public DragDropEffects EventDragOver(IDataObject? data)
        {
            var dataString = data?.GetData(typeof(string));
            return dataString is not string ? DragDropEffects.None : DragDropEffects.Move;
        }

        public Point GetDifferenceFromMouseDown(Point p) => GetDifferenceFromMouseDown(p.X, p.Y);

        public Point GetDifferenceFromMouseDown(int x, int y)
        {
            if (this.MouseDownLocation == Point.Empty)
            {
                return Point.Empty;
            }

            var gridPoint = dataGridView.PointToClient(new(x, y));

            var mouseDownHitTest = dataGridView.HitTest(this.MouseDownLocation.X, this.MouseDownLocation.Y);
            var dropHitTest = dataGridView.HitTest(gridPoint.X, gridPoint.Y);

            if (mouseDownHitTest.Type != DataGridViewHitTestType.Cell || dropHitTest.Type != DataGridViewHitTestType.Cell)
            {
                return Point.Empty;
            }

            return new() { X = dropHitTest.ColumnIndex - mouseDownHitTest.ColumnIndex, Y = dropHitTest.RowIndex - mouseDownHitTest.RowIndex };
        }

        public void EventDragDrop(IDataObject? data, int x, int y)
        {
            try
            {
                var dataObj = data?.GetData(typeof(string));
                if (dataObj is string excelText)
                {
                    var coordDifference = this.GetDifferenceFromMouseDown(x, y);
                    if (coordDifference != Point.Empty)
                    {
                        var minRowIndex = dataGridView.SelectedCells.Cast<DataGridViewCell>().Min(c => c.RowIndex);
                        var minColumnIndex = dataGridView.SelectedCells.Cast<DataGridViewCell>().Min(c => c.ColumnIndex);

                        //These two CAN BE NEGATIVE!!
                        var pasteColumnIndex = minColumnIndex + coordDifference.X;
                        var pasteRowIndex = minRowIndex + coordDifference.Y;

                        dataGridView.SuspendLayout();

                        var req = dataChangedHandler.Join();
                        var pastedCellsList = GridUtils.PasteAsExcel(dataGridView, excelText, pasteRowIndex, pasteColumnIndex, ignoreSingleLineMultiplePasting: true);

                        dataGridView.SelectedCells.Cast<DataGridViewCell>()
                            .Except(pastedCellsList)
                            .ForEach(c => c.Value = null); //Clear cell value for cell not pasted

                        dataChangedHandler.End(req);

                        dataGridView.FindForm()?.BeginInvoke(() =>
                        {
                            DllImports.RaiseLeftMouse(Cursor.Position); //This fixes the mouse down stuck after dropping.

                            dataGridView.ClearSelection();

                            var gridPoint = dataGridView.PointToClient(Cursor.Position);
                            var dropHitTest = dataGridView.HitTest(gridPoint.X, gridPoint.Y);
                            if (dropHitTest.Type == DataGridViewHitTestType.Cell)
                            {
                                dataGridView.CurrentCell = dataGridView.Rows[dropHitTest.RowIndex].Cells[dropHitTest.ColumnIndex];
                            }

                            foreach (var cell in pastedCellsList)
                            {
                                cell.Selected = true;
                            }

                            dataGridView.ResumeLayout(performLayout: false);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }
            finally
            {
                this.MouseDownLocation = Point.Empty;
            }

        }
 */