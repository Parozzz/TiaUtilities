using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridDoDragDropHandler<T> where T : GridData
    {
        public bool Busy;

        private readonly GridDragDownHandler<T> excelDragHandler;
        private bool cursorInsideBorder = false;
        private Point dragDropMouseDownLocation = Point.Empty;

        public GridDoDragDropHandler(GridDragDownHandler<T> excelDragHandler)
        {
            this.excelDragHandler = excelDragHandler;
        }

        public void registerEvents(DataGridView dataGridView)
        {/*
            bool cursorInsideBorder = false;
            dataGridView.CellMouseMove += (sender, args) =>
            {
                dataGridView.Cursor = Cursors.Default;
                cursorInsideBorder = false;

                if (excelDragHandler.IsStarted())
                {
                    return;
                }

                var rowIndex = args.RowIndex;
                var columnIndex = args.ColumnIndex;
                if (!this.AreCellCoordinatesValid(rowIndex, columnIndex))
                {
                    return;
                }

                var currentCell = dataGridView.CurrentCell;
                if (currentCell != null && rowIndex == currentCell.RowIndex && columnIndex == currentCell.ColumnIndex && excelDragHandler.MouseShouldDisplayCursor(args))
                {
                    dataGridView.Cursor = Cursors.Cross;
                    return;
                }

                //Display cursor for drag&drop
                var cell = dataGridView.Rows[rowIndex].Cells[columnIndex];
                if (cell.Selected)
                {
                    const int EXTRA_PX_BORDER_MOUSE_SELECTION = 3;

                    var topBorder = topBorderRect;
                    if (topBorder != RectangleF.Empty)
                    {
                        topBorder.Inflate(EXTRA_PX_BORDER_MOUSE_SELECTION, EXTRA_PX_BORDER_MOUSE_SELECTION);
                    }
                    var bottomBorder = bottomBorderRect;
                    if (bottomBorder != RectangleF.Empty)
                    {
                        bottomBorder.Inflate(EXTRA_PX_BORDER_MOUSE_SELECTION, EXTRA_PX_BORDER_MOUSE_SELECTION);
                    }
                    var leftBorder = leftBorderRect;
                    if (leftBorder != RectangleF.Empty)
                    {
                        leftBorder.Inflate(EXTRA_PX_BORDER_MOUSE_SELECTION, EXTRA_PX_BORDER_MOUSE_SELECTION);
                    }
                    var rightBorder = rightBorderRect;
                    if (rightBorder != RectangleF.Empty)
                    {
                        rightBorder.Inflate(EXTRA_PX_BORDER_MOUSE_SELECTION, EXTRA_PX_BORDER_MOUSE_SELECTION);
                    }

                    var cursorPosition = dataGridView.PointToClient(Cursor.Position);
                    if (topBorder.Contains(cursorPosition) || bottomBorder.Contains(cursorPosition) || leftBorder.Contains(cursorPosition) || rightBorder.Contains(cursorPosition))
                    {
                        dataGridView.Cursor = Cursors.SizeAll;
                        cursorInsideBorder = true;
                    }
                }
            };

            dataGridView.MouseLeave += (sender, args) => cursorInsideBorder = false; //This seems to be called just after the DoDragDrop method.

            dataGridView.MouseDown += (sender, args) =>
            {
                if (!this.excelDragHandler.IsStarted() && cursorInsideBorder && args.Button == MouseButtons.Left)
                {
                    var text = GridUtils.GetCopyAsExcelText(dataGridView, out var _);
                    if (text != null)
                    {
                        cursorInsideBorder = false;
                        this.dragDropMouseDownLocation = args.Location;

                        var mouseDownHitTest = dataGridView.HitTest(this.dragDropMouseDownLocation.X, this.dragDropMouseDownLocation.Y);
                        if (mouseDownHitTest.Type == DataGridViewHitTestType.Cell)
                        {//This allows the cell to show different back color
                            dataGridView.InvalidateCell(mouseDownHitTest.ColumnIndex, mouseDownHitTest.RowIndex);
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

                        this.dragDropMouseDownLocation = Point.Empty;
                    }
                }

            };
            */
            dataGridView.QueryContinueDrag += (sender, args) =>
            {
                /*
                var dataGridCursorPosition = dataGridView.PointToClient(Cursor.Position);
                
                var hitTest = dataGridView.HitTest(dataGridCursorPosition.X, dataGridCursorPosition.Y);
                Debug.WriteLine("Query continue drag. HitTest: " + (hitTest.Type == DataGridViewHitTestType.Cell));

                if(hitTest.Type == DataGridViewHitTestType.Cell)
                {
                    var cell = dataGridView.Rows[hitTest.RowIndex].Cells[hitTest.ColumnIndex];
                    cell.Style.BackColor = SystemColors.ActiveCaptionText;
                }*/
            };
            /*
            dataGridView.DragOver += (sender, args) =>
            {
                var data = args.Data?.GetData(typeof(string));
                args.Effect = data is not string ? DragDropEffects.None : DragDropEffects.Move;
            };

            dataGridView.DragDrop += (sender, args) =>
            {
                try
                {
                    var dataObj = args.Data?.GetData(typeof(string));
                    if (dataObj is string excelText)
                    {
                        var gridPoint = dataGridView.PointToClient(new(args.X, args.Y));

                        var mouseDownHitTest = dataGridView.HitTest(this.dragDropMouseDownLocation.X, this.dragDropMouseDownLocation.Y);
                        var dropHitTest = dataGridView.HitTest(gridPoint.X, gridPoint.Y);

                        var bothAreCells = mouseDownHitTest.Type == DataGridViewHitTestType.Cell && dropHitTest.Type == DataGridViewHitTestType.Cell;
                        var sameCells = mouseDownHitTest.RowIndex == dropHitTest.RowIndex && mouseDownHitTest.ColumnIndex == dropHitTest.ColumnIndex;
                        if (bothAreCells && !sameCells)
                        {
                            //var rowIndex = dropHitTest.RowIndex;
                            //var columnIndex = dropHitTest.ColumnIndex;

                            var minRowIndex = dataGridView.SelectedCells.Cast<DataGridViewCell>().Min(c => c.RowIndex);
                            var minColumnIndex = dataGridView.SelectedCells.Cast<DataGridViewCell>().Min(c => c.ColumnIndex);

                            var pasteRowIndex = minRowIndex + (dropHitTest.RowIndex - mouseDownHitTest.RowIndex);
                            var pasteColumnIndex = minColumnIndex + (dropHitTest.ColumnIndex - mouseDownHitTest.ColumnIndex);

                            this.SuspendLayout();

                            var req = this.DataChangedHandler.Join();
                            GridUtils.PasteAsExcel(dataGridView, excelText, pasteRowIndex, pasteColumnIndex, out var pastedCellsList, ignoreSingleLineMultiplePasting: true);

                            var cellsToDelete = dataGridView.SelectedCells.Cast<DataGridViewCell>().Except(pastedCellsList);
                            foreach (var cell in cellsToDelete)
                            {
                                cell.Value = null;
                            }

                            this.DataChangedHandler.End(req);

                            this.FindForm()?.BeginInvoke(() =>
                            {
                                DllImports.RaiseLeftMouse(Cursor.Position); //This fixes the mouse down stuck after dropping.

                                dataGridView.ClearSelection();
                                dataGridView.CurrentCell = dataGridView.Rows[pasteRowIndex].Cells[pasteColumnIndex];

                                foreach (var cell in pastedCellsList)
                                {
                                    cell.Selected = true;
                                }

                                this.ResumeLayout(refresh: false);
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
                    cursorInsideBorder = false;
                    this.dragDropMouseDownLocation = Point.Empty;
                }

            };*/
        }
    }
}
