using System.Diagnostics;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.GridHandler.CellPainters;
using TiaUtilities.Generation.GridHandler.CustomColumns;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Languages;
using TiaUtilities.UndoRedo;
using TiaUtilities.Utility;
using static TiaUtilities.Generation.GridHandler.GridUtils;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridHandler<T> : ICleanable, ISaveable<GridSave<T>> where T : GridData
    {
        internal class CellTag
        {
            public bool HasBorder { get; set; }

            public SelectedCellsBorderCoordinates? CellsBorderCoordinates { get; set; }

            public float BordersWidth { get; set; }
            public RectangleF[] Borders { get; set; } = [];
        }

        private readonly GridSettings settings;
        private readonly GridBindContainer gridBindFactory;

        private readonly UndoRedoHandler undoRedoHandler;
        private readonly GridExcelDragHandler<T> excelDragHandler;
        private readonly GridSortHandler<T> sortHandler;
        private readonly GridDataPreviewer<T> previewer;
        private readonly GenPlaceholderHandler placeholderHandler;

        private DataGridView DataGridView { get; init; }
        internal DataGridView InternalDataGridView { get => this.DataGridView; }

        public GridDataSource<T> DataSource { get; init; }
        public GridDataChangedHandler<T> DataChangedHandler { get; init; }
        public GridColumnHandler<T> Columns { get; init; }
        public List<GridScriptVariable> ScriptVariableList { get; init; }


        public required uint InitializeRowCount { get; set; } = 9;

        public int RowCount { get => this.DataGridView.RowCount; }
        public int ColumnCount { get => this.DataGridView.ColumnCount; }

        public bool AddRowIndexToRowHeader { get; set; } = true;
        public bool EnablePasteFromExcel { get; set; } = true;
        public bool EnableRowSelectionFromRowHeaderClick { get; set; } = true;
        public bool ShowJSContextMenuTopLeft { get; set; } = true;

        public event GridDataChangedEventHandler DataChanged = delegate { };
        public event GridDataLoadedEvent DataLoaded = delegate { };

        public event GridSelectedRowChangedEventHandler RowSelectedChanged = delegate { };

        public event GridPreSortEventHandler PreSort = delegate { };
        public event GridPostSortEventHandler PostSort = delegate { };

        public event GridExcelDragPreviewEventHandler ExcelDragPreview = delegate { };
        public event GridExcelDragDoneEventHandler ExcelDragDone = delegate { };

        public event DataGridViewCellToolTipTextNeededEventHandler CellToolTipTextNeeded
        {
            add => this.DataGridView.CellToolTipTextNeeded += value;
            remove => this.DataGridView.CellToolTipTextNeeded -= value;
        }

        private bool init;
        private bool dirty;

        public GridHandler(GridSettings settings,
            GridBindContainer gridBindFactory,
            GridDataPreviewer<T> previewer,
            GenPlaceholderHandler placeholderHandler,
            IGridRowComparer<T>? comparer = null)
        {
            this.settings = settings;
            this.gridBindFactory = gridBindFactory;
            this.previewer = previewer;
            this.placeholderHandler = placeholderHandler;

            this.ScriptVariableList = [];

            this.DataGridView = new MyGrid();
            this.DataSource = new(this);

            this.undoRedoHandler = new();
            this.excelDragHandler = new(this, settings);

            this.sortHandler = new(this, this.undoRedoHandler, comparer);

            this.DataChangedHandler = new(this, this.undoRedoHandler);
            this.Columns = new(this);
        }

        internal void CallPreSortEvent(GridPreSortEventArgs args)
        {
            this.PreSort(this.DataGridView, args);
        }

        internal void CallPostSortEvent(GridPostSortEventArgs args)
        {
            this.dirty = true;
            this.PostSort(this.DataGridView, args);
        }

        internal void CallExcelDragPreviewEvent(GridExcelDragEventArgs args)
        {
            this.ExcelDragPreview(this.DataGridView, args);
        }

        internal void CallExcelDragDoneEvent(GridExcelDragEventArgs args)
        {
            this.ExcelDragDone(this.DataGridView, args);
        }

        internal void CallDataChangedEvent(List<GridDataChangedCache> cachedChanges)
        {
            if (cachedChanges.Count > 0)
            {
                GridDataChangedEventArgs args = new();
                args.ChangedCellDataList.AddRange(
                    cachedChanges.Select(c => new GridChangedData(c.Args, c.Row))
                );

                this.DataChanged(this.DataGridView, args);
            }
        }

        private void CallRowSelectedChangedEvent(GridSelectedRowChangedArgs args)
        {
            this.RowSelectedChanged(this.DataGridView, args);
        }

        private void CallLoadDataEvent()
        {
            this.DataLoaded(this.DataGridView, new());
        }

        internal void SetDataSource(object dataSource)
        {
            this.DataGridView.DataSource = dataSource;
        }

        public void Refresh()
        {
            this.DataGridView.RefreshEdit();
            this.DataGridView.Refresh();
        }

        public void SuspendLayout()
        {
            this.DataGridView.SuspendLayout();
        }

        public void ResumeLayout(bool refresh = false, bool performLayout = true)
        {
            if (refresh)
            {
                this.Refresh();
            }

            this.DataGridView.ResumeLayout(performLayout);
        }

        public void Init()
        {
            if (init)
            {
                return;
            }

            this.DataGridView.SuspendLayout();

            this.DataGridView.Name = "MyGrid_" + Guid.NewGuid().ToString();
            this.DataGridView.AutoGenerateColumns = false;

            this.DataGridView.Dock = DockStyle.Fill;
            this.DataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None; //Is faster?
            this.DataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.DataGridView.AllowUserToResizeRows = false;
            this.DataGridView.Margin = Padding.Empty;

            this.DataGridView.BorderStyle = BorderStyle.None;

            this.DataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable; //Do it myself!
            this.DataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.DataGridView.MultiSelect = true;

            this.DataGridView.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            this.DataGridView.Font = settings.GridFont;
            this.DataGridView.AllowUserToAddRows = false;

            this.DataGridView.AllowDrop = true;
            this.DataGridView.DataError += DataErrorEventHandler;

            this.DataGridView.DefaultCellStyle.SelectionBackColor = Color.LightGray;
            this.DataGridView.DefaultCellStyle.BackColor = SystemColors.ControlLightLight;
            this.DataGridView.DefaultCellStyle.SelectionForeColor = Color.Black;
            this.DataGridView.DefaultCellStyle.ForeColor = Color.Black;
            this.DataGridView.DefaultCellStyle.Padding = new Padding(2);
            this.DataGridView.DefaultCellStyle.Tag = new CellTag();

            this.DataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.DataGridView.ColumnHeadersDefaultCellStyle.Padding = new Padding(0);
            this.DataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;

            this.DataGridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            this.DataGridView.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;

            this.Columns.Init();
            this.DataSource.InitializeData(this.InitializeRowCount);

            #region QOL - Quality of life

            #region Full Row Selection
            if (EnableRowSelectionFromRowHeaderClick)
            {
                this.DataGridView.MouseDown += (sender, args) =>
                {
                    var hitTest = this.DataGridView.HitTest(args.X, args.Y);
                    switch (hitTest.Type)
                    {
                        case DataGridViewHitTestType.None: //I want that to clear the selection, you do a simple click in an empty area!
                            this.DataGridView.ClearSelection();
                            this.DataGridView.CurrentCell = null; //This avoid the situation where if you click the old cell again, it start editing immediately! 
                            break;
                        case DataGridViewHitTestType.RowHeader: //If i click a row head, i want the whole row to be selected!
                            var currentRow = this.DataGridView.CurrentRow;
                            if (Control.ModifierKeys == Keys.Shift && currentRow != null)
                            {
                                if (hitTest.RowIndex < 0)
                                {
                                    break;
                                }

                                var startRowIndex = currentRow.Index;
                                var endRowIndex = hitTest.RowIndex;

                                this.DataGridView.ClearSelection();

                                var biggestIndex = Math.Max(startRowIndex, endRowIndex);
                                var lowestIndex = Math.Min(startRowIndex, endRowIndex);
                                for (int x = lowestIndex; x < biggestIndex + 1; x++)
                                {
                                    foreach (DataGridViewCell cell in this.DataGridView.Rows[x].Cells)
                                    {
                                        cell.Selected = true;
                                    }
                                }
                            }
                            else
                            {
                                SelectRow(hitTest.RowIndex);
                            }

                            break;
                        case DataGridViewHitTestType.Cell:
                            break;
                    }
                };
            }
            #endregion

            #region Better Editing Control Show
            this.DataGridView.CellClick += (sender, args) =>
            {
                if (Control.ModifierKeys == Keys.Shift || Control.ModifierKeys == Keys.Control || args.RowIndex < 0 || args.ColumnIndex < 0)
                {
                    return;
                }

                var cell = this.DataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex];
                if (cell == null)
                {
                    return;
                }

                //This is to have a better user experience while dealing with combobox. When finishing the edit, it will revert back to a simple selected cell instead of selected text!
                if (cell is DataGridViewComboBoxCell comboBoxCell)
                {
                    this.DataGridView.CurrentCell = cell;
                    this.DataGridView.BeginEdit(false);
                    if (this.DataGridView.EditingControl is DataGridViewComboBoxEditingControl comboBoxEditingControl)
                    {
                        comboBoxEditingControl.DroppedDown = true;
                        comboBoxEditingControl.DropDownClosed += (sender, args) => this.DataGridView.EndEdit();
                    }
                }
            };

            this.DataGridView.CellMouseDoubleClick += (sender, args) =>
            {
                if (Control.ModifierKeys == Keys.Shift || Control.ModifierKeys == Keys.Control || args.RowIndex < 0 || args.ColumnIndex < 0)
                {
                    return;
                }

                var cell = this.DataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex];
                if (cell == null)
                {
                    return;
                }

                if (cell is DataGridViewTextBoxCell textBoxCell)
                {
                    this.DataGridView.BeginEdit(true);
                }
                else if (cell is DataGridViewCheckBoxCell checkboxCell)
                {
                    checkboxCell.Value = (bool)(checkboxCell.Value ?? false) == false;
                }
            };
            #endregion

            #endregion

            #region EVENT - CELL PAINTING

            var previewPainter = new GridCellPreviewPainter<T>(this.placeholderHandler, this.previewer, this.DataSource, this.settings);
            this.DataGridView.CellPainting += (sender, args) =>
            {
                if (sortHandler.ShouldCellContentPaint(args))
                {
                    args.PaintBackground(args.ClipBounds, true);
                    sortHandler.CellContentPaint(args); //This paints the top columns cell
                    return;
                }

                this.excelDragHandler.CellPaiting(args);

                args.Handled = true;
                args.PaintBackground(args.ClipBounds, true);

                var paintData = previewPainter.ShouldCellContentPaint(args);
                if (paintData != null)
                {
                    previewPainter.CellContentPaint(args, paintData);
                }
                else
                {
                    args.PaintContent(args.ClipBounds);
                }
            };

            List<DataGridViewCell> selectedCellsList = [];
            this.DataGridView.Paint += (sender, args) =>
            {
                try
                {
                    if (selectedCellsList.Count > 0)
                    {
                        var same = selectedCellsList.All(c => this.DataGridView.SelectedCells.Contains(c)) && selectedCellsList.Count == this.DataGridView.SelectedCells.Count;
                        if (!same)
                        {
                            foreach (var cell in selectedCellsList)
                            {
                                if(cell.DataGridView != this.DataGridView)
                                {//It could happen
                                    return;
                                }

                                var cellTag = this.GetCellTag(cell);
                                cellTag.HasBorder = false;
                                cellTag.Borders = [RectangleF.Empty, RectangleF.Empty, RectangleF.Empty, RectangleF.Empty];

                                this.DataGridView.InvalidateCell(cell);
                            }
                        }

                        selectedCellsList.Clear();
                    }

                    if (!this.DataGridView.AreAllCellsSelected(false))
                    {
                        selectedCellsList.AddRange(this.DataGridView.SelectedCells.Cast<DataGridViewCell>());

                        var continous = GridUtils.AreSelectedCellsPlanar(this.DataGridView);
                        if (continous)
                        {
                            var borders = GridUtils.GetSelectedCellsBorderCoordinates(this.DataGridView);
                            foreach (DataGridViewCell cell in this.DataGridView.SelectedCells)
                            {
                                GridUtils.CreateCellBorders(this, borders, cell, width: 2f);
                            }

                            var cellsWithBorder = selectedCellsList.Where(c =>
                            {
                                var rowIndex = c.RowIndex;
                                var columnIndex = c.ColumnIndex;
                                return columnIndex == borders.Left || columnIndex == borders.Right || rowIndex == borders.Top || rowIndex == borders.Bottom;
                            });

                            var topLeft = selectedCellsList.FirstOrDefault(c => c.ColumnIndex == borders.Left && c.RowIndex == borders.Top);
                            var topRight = selectedCellsList.FirstOrDefault(c => c.ColumnIndex == borders.Right && c.RowIndex == borders.Top);
                            var bottomLeft = selectedCellsList.FirstOrDefault(c => c.ColumnIndex == borders.Left && c.RowIndex == borders.Bottom);
                            var bottomRight = selectedCellsList.FirstOrDefault(c => c.ColumnIndex == borders.Right && c.RowIndex == borders.Bottom);

                            if(topLeft != null && topRight != null && bottomLeft != null && bottomRight != null)
                            {
                                var topLeftBounds = this.DataGridView.GetCellDisplayRectangle(topLeft.ColumnIndex, topLeft.RowIndex, true);
                                var topRightBounds = this.DataGridView.GetCellDisplayRectangle(topRight.ColumnIndex, topRight.RowIndex, true);
                                var bottomLeftBounds = this.DataGridView.GetCellDisplayRectangle(bottomLeft.ColumnIndex, bottomLeft.RowIndex, true);
                                var bottomRightBounds = this.DataGridView.GetCellDisplayRectangle(bottomRight.ColumnIndex, bottomRight.RowIndex, true);

                                var borderWidth = 2f;
                                RectangleF selectionBorderRect = new() {
                                    X = topLeftBounds.X + 1,
                                    Y = topLeftBounds.Y + 1,
                                    Width = (topRightBounds.X + topRightBounds.Width) - topLeftBounds.X - borderWidth - (borderWidth <= 2f ? 1 : 0),
                                    Height = (bottomLeftBounds.Y + bottomLeftBounds.Height) - topLeftBounds.Y - borderWidth - (borderWidth <= 2f ? 1 : 0)
                                };
                                using Pen borderPen = new(this.settings.SingleSelectedCellBorderColor, borderWidth);
                                args.Graphics.DrawRectangle(borderPen, selectionBorderRect);
                            }


                            var firstRowIndex = this.DataGridView.FirstDisplayedScrollingRowIndex + 1;

                            var visibleRows = this.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Displayed | DataGridViewElementStates.Visible);

                            Debug.WriteLine($"First: {firstRowIndex}, Amount: {visibleRows}, Last: {firstRowIndex + visibleRows - 1}");

                            /*
                            foreach (var cell in cellsWithBorder)
                            {
                                GridUtils.PaintCellBorder(this, args.Graphics, cell, this.settings.SingleSelectedCellBorderColor, width: 2f);
                            }*/
                        }



                        this.excelDragHandler.PaintTriangle(args.Graphics);
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex);
                }
            };
            #endregion

            #region DRAG&DROP + CURSOR

            bool cursorInsideBorder = false;
            this.DataGridView.CellMouseMove += (sender, args) =>
            {
                this.DataGridView.Cursor = Cursors.Default;
                cursorInsideBorder = false;

                if (excelDragHandler.IsStarted())
                {
                    return;
                }

                var rowIndex = args.RowIndex;
                var columnIndex = args.ColumnIndex;

                var currentCell = this.DataGridView.CurrentCell;
                if(currentCell == null)
                {
                    return;
                }

                if (rowIndex == currentCell.RowIndex && columnIndex == currentCell.ColumnIndex && excelDragHandler.CellMouseMoveShouldDisplayCursor(args))
                {
                    this.DataGridView.Cursor = Cursors.Cross;
                    return;
                }

                if (rowIndex < 0 || columnIndex < 0)
                {
                    return;
                }

                var cell = this.DataGridView.Rows[rowIndex].Cells[columnIndex];
                if (cell != null)
                {
                    var tag = this.GetCellTag(cell);
                    if (tag.HasBorder)
                    {
                        const int EXTRA_PX_BORDER_MOUSE_SELECTION = 3;

                        var cursorPosition = this.DataGridView.PointToClient(Cursor.Position);
                        if (tag.Borders.Any(r =>
                        {
                            var rect = r;
                            //rect.Offset(-EXTRA_PX_BORDER_MOUSE_SELECTION, -EXTRA_PX_BORDER_MOUSE_SELECTION);
                            rect.Inflate(EXTRA_PX_BORDER_MOUSE_SELECTION, EXTRA_PX_BORDER_MOUSE_SELECTION);
                            return rect.Contains(cursorPosition);
                        }))
                        {
                            this.DataGridView.Cursor = Cursors.SizeAll;
                            cursorInsideBorder = true;
                        }
                    }
                }
            };

            this.DataGridView.MouseLeave += (sender, args) => cursorInsideBorder = false;

            this.DataGridView.MouseDown += (sender, args) =>
            {
                if (!this.excelDragHandler.IsStarted() && cursorInsideBorder && args.Button == MouseButtons.Left)
                {
                    var text = GridUtils.GetCopyAsExcelText(this.DataGridView);
                    if (text != null)
                    {
                        this.DataGridView.DoDragDrop(text, DragDropEffects.Move, null, Point.Empty, true);
                        cursorInsideBorder = false;
                    }
                }
            };

            this.DataGridView.DragOver += (sender, args) =>
            {
                var data = args.Data;
                args.Effect = data?.GetData(typeof(string)) is string text && string.IsNullOrEmpty(text) ? DragDropEffects.None : DragDropEffects.Move;
            };

            this.DataGridView.DragDrop += (sender, args) =>
            {
                try
                {
                    var dataObj = args.Data?.GetData(typeof(string));
                    if (dataObj is string excelText)
                    {
                        var gridPoint = this.DataGridView.PointToClient(new(args.X, args.Y));

                        var hitTest = this.DataGridView.HitTest(gridPoint.X, gridPoint.Y);
                        if (hitTest.Type == DataGridViewHitTestType.Cell)
                        {
                            var rowIndex = hitTest.RowIndex;
                            var columnIndex = hitTest.ColumnIndex;

                            var selectionRowOffset = rowIndex - this.DataGridView.SelectedCells.Cast<DataGridViewCell>().Min(c => c.RowIndex);
                            var selectionColumnOffset = columnIndex - this.DataGridView.SelectedCells.Cast<DataGridViewCell>().Min(c => c.ColumnIndex);

                            this.SuspendLayout();

                            List<Point> cellsToSelect = [];

                            var req = this.DataChangedHandler.Join();
                            foreach (DataGridViewCell selectedCell in this.DataGridView.SelectedCells)
                            {
                                selectedCell.Value = null;
                                //Needs to create MAP for considering invisible columns and rows.
                                /*
                                var mappaCols = dgv.Columns.Cast<DataGridViewColumn>()
                                    .Where(c => c.Visible)
                                    .OrderBy(c => c.DisplayIndex)
                                    .ToList();

                                var mappaRows = dgv.Rows.Cast<DataGridViewRow>()
                                    .Where(r => r.Visible)
                                    .OrderBy(r => r.Index)
                                    .ToList();*/
                                cellsToSelect.Add(new() { X = selectedCell.RowIndex + selectionRowOffset, Y = selectedCell.ColumnIndex + selectionColumnOffset });
                            }

                            GridUtils.PasteAsExcel(this.DataGridView, excelText, rowIndex, columnIndex, ignoreSingleLineMultiplePasting: true);
                            this.DataChangedHandler.End(req);

                            this.ResumeLayout(refresh: false);

                            this.FindForm()?.BeginInvoke(() =>
                            {//This fixes the mouse down stuck after dropping.
                                var cursorPos = Cursor.Position;
                                DllImports.mouse_event(DllImports.MOUSEEVENTF_LEFTUP, cursorPos.X, cursorPos.Y, 0, 0);

                                this.DataGridView.ClearSelection();
                                this.DataGridView.CurrentCell = this.DataGridView.Rows[rowIndex].Cells[columnIndex];

                                foreach (var point in cellsToSelect)
                                {
                                    var rowIndex = point.X;
                                    var columnIndex = point.Y;
                                    if (this.AreCellCoordinatesValid(rowIndex, columnIndex))
                                    {
                                        var cell = this.DataGridView.Rows[rowIndex].Cells[columnIndex];
                                        cell.Selected = true;

                                        this.DataGridView.InvalidateCell(cell);
                                    }
                                }
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
                }

            };

            #endregion

            #region ROW HEADER NUMBER
            if (AddRowIndexToRowHeader)
            {
                this.DataGridView.RowPostPaint += (sender, args) =>
                {
                    var style = args.InheritedRowStyle;

                    var rowIdx = (args.RowIndex + 1).ToString();

                    var centerFormat = new StringFormat()
                    {
                        // right alignment might actually make more sense for numbers
                        Alignment = StringAlignment.Far,
                        LineAlignment = StringAlignment.Far
                    };

                    var textSize = TextRenderer.MeasureText(rowIdx, style.Font); //get the size of the string
                    DataGridView.RowHeadersWidth = Math.Max(DataGridView.RowHeadersWidth, textSize.Width + 15); //if header width lower then string width then resize

                    var headerBounds = new Rectangle(args.RowBounds.Left, args.RowBounds.Top, DataGridView.RowHeadersWidth, args.RowBounds.Height);
                    args.Graphics.DrawString(rowIdx, style.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
                };
            }
            #endregion

            #region EVENT - KEYDOWN - CopyAsExcel/PasteAsExcel/Delete/Undo/Redo/Find
            this.DataGridView.KeyDown += (sender, args) =>
            {
                var handled = true;
                switch (args.KeyData)
                {
                    case Keys.Z | Keys.Control:
                        undoRedoHandler.Undo();
                        break;
                    case Keys.Y | Keys.Control:
                        undoRedoHandler.Redo();
                        break;
                    case Keys.C | Keys.Control:
                        GridUtils.CopyAsExcelToClipboard(this.DataGridView);
                        break;
                    case Keys.Insert | Keys.Shift:
                    case Keys.V | Keys.Control:

                        var req = this.DataChangedHandler.Join();
                        GridUtils.PasteAsExcelFromClipboard(this.DataGridView);
                        this.DataChangedHandler.End(req);

                        this.Refresh(); //This is required since for some special column type (Like checkbox) is needed.
                        break;
                    case Keys.F | Keys.Control:
                        this.gridBindFactory.ShowFindForm(this);
                        break;
                    case Keys.J | Keys.Control:
                        this.gridBindFactory.ShowGridScript(this);
                        break;
                    case Keys.Delete:
                        this.DeleteSelectedCells();
                        break;
                    case Keys.Escape:
                        this.SelectCell(cell: null);
                        break;
                    case Keys.F5:
                        this.DataGridView.Refresh();
                        break;
                    default:
                        handled = false;
                        break;
                }

                args.Handled = handled;
            };
            #endregion

            #region EVENT - CELL FORMATTING
            this.DataGridView.CellFormatting += (sender, args) =>
            {
                if (this.DataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex] is DataGridViewButtonCell buttonCell)
                {
                    var data = this.DataSource[args.RowIndex];
                    args.Value = $"{data[args.ColumnIndex]}";
                }
            };
            #endregion

            #region EVENT - CELL CONTEXT CLICK / DOUBLE CLICK
            this.DataGridView.CellContentClick += (sender, args) =>
            {
                var rowIndex = args.RowIndex;
                var columnIndex = args.ColumnIndex;
                if (rowIndex < 0 || rowIndex >= this.DataGridView.RowCount || columnIndex < 0 || columnIndex >= this.DataGridView.ColumnCount)
                {
                    return;
                }

                var cell = this.DataGridView.Rows[rowIndex].Cells[columnIndex];
                if (cell is DataGridViewCheckBoxCell checkBoxCell)
                {
                    this.DataGridView.EndEdit(); //If a checkbox is clicked, it goes in edit mode. I do not want that, is confusing. This fixes it.
                }
                else if (cell is DataGridViewButtonCell buttonCell && cell.OwningColumn is DataGridViewEventableButtonColumn eventableButtonColumn)
                {
                    var data = this.DataSource[rowIndex][columnIndex];
                    eventableButtonColumn.ButtonPressedEvent(data, buttonCell);
                }
            };

            this.DataGridView.CellContentDoubleClick += (sender, args) =>
            {
                var rowIndex = args.RowIndex;
                var columnIndex = args.ColumnIndex;
                if (rowIndex < 0 || rowIndex >= this.DataGridView.RowCount || columnIndex < 0 || columnIndex >= this.DataGridView.ColumnCount)
                {
                    return;
                }

                var cell = this.DataGridView.Rows[rowIndex].Cells[columnIndex];
                if (cell is DataGridViewButtonCell buttonCell && cell.OwningColumn is DataGridViewEventableButtonColumn eventableButtonColumn)
                {
                    var data = this.DataSource[rowIndex][columnIndex];
                    eventableButtonColumn.ButtonDoublePressedEvent(data, buttonCell);
                }
            };
            #endregion

            #region EVENT - ROW ENTER (ROW CHANGED)
            this.DataGridView.RowEnter += (sender, args) =>
            {
                this.CallRowSelectedChangedEvent(new() { RowIndex = args.RowIndex, ColumnIndex = args.ColumnIndex });
            };
            #endregion

            #region SCRIPT
            foreach (var column in this.DataSource.DataColumns)
            {
                var scriptVariable = GridScriptVariable.CreateFromGrid(column, this);
                this.ScriptVariableList.Add(scriptVariable);
            }

            this.DataGridView.MouseClick += (sender, args) => this.gridBindFactory.ChangeBind(this);
            this.DataGridView.CellMouseClick += (sender, args) =>
            {
                if (args.RowIndex == -1 && args.ColumnIndex == -1 && args.Button == MouseButtons.Right)
                {
                    var menuItem = new ToolStripMenuItem { Text = Locale.GRID_SCRIPT_OPEN_JAVASCRIPT_CONTEXT };
                    menuItem.Click += (sender, args) => { this.gridBindFactory.ShowGridScript(); };

                    var contextMenu = new ContextMenuStrip();
                    contextMenu.Items.Add(menuItem);
                    contextMenu.Show(this.DataGridView, this.DataGridView.PointToClient(Cursor.Position));
                }
            };
            #endregion

            this.settings.PropertyChanged += (sender, args) => this.DataGridView.Refresh();

            this.DataGridView.VisibleChanged += (sender, argz) => this.DataGridView.AutoResizeColumnHeadersHeight();

            this.excelDragHandler.Init();
            this.sortHandler.Init();

            this.DataGridView.ResumeLayout();

            init = true;
        }

        internal CellTag GetCellTag(DataGridViewCell cell)
        {
            if (cell.Tag is not CellTag)
            {
                cell.Tag = new CellTag();
            }

            return (CellTag)cell.Tag;
        }

        public bool IsDirty() => this.dirty;
        public void Wash() => this.dirty = false;

        public GridSave<T> CreateSave()
        {
            return new()
            {
                RowData = this.DataSource.CreateSave()
            };
        }

        public void LoadSave(GridSave<T> gridSave)
        {
            var req = this.DataChangedHandler.Suspend();

            this.DataGridView.SuspendLayout();
            this.DataGridView.Enabled = false;

            this.undoRedoHandler.Clear();
            this.DataSource.LoadSave(gridSave.RowData);

            this.DataGridView.Enabled = true;
            this.DataGridView.Refresh();
            this.DataGridView.ResumeLayout();

            this.DataChangedHandler.End(req);

            this.CallLoadDataEvent();
        }

        private void DataErrorEventHandler(object? sender, DataGridViewDataErrorEventArgs args)
        {
            var cell = this.DataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex];
            if (cell is DataGridViewComboBoxCell comboBoxCell)
            {
                comboBoxCell.Value = "";
            }
        }

        public Form? FindForm()
        {
            return this.DataGridView.FindForm();
        }

        public void AutoResizeColumns() => this.DataGridView.AutoResizeColumns();

        public void AutoResizeColumnHeadersHeight() => this.DataGridView.AutoResizeColumnHeadersHeight();

        private bool AreCellCoordinatesValid(int row, int column) => row >= 0 && column >= 0 && row < this.RowCount && column < this.ColumnCount;

        public DataGridViewCell? GetCurrentCell() => this.DataGridView.CurrentCell;

        public void ChangeCurrentCell(int rowIndex, int columnIndex)
        {
            if (AreCellCoordinatesValid(rowIndex, columnIndex))
            {
                this.DataGridView.CurrentCell = this.DataGridView.Rows[rowIndex].Cells[columnIndex];
            }
        }

        public DataGridViewCell? GetCell(int rowIndex, GridDataColumn column) => this.GetCell(rowIndex, column.ColumnIndex);

        public DataGridViewCell? GetCell(int rowIndex, int columnIndex)
        {
            return AreCellCoordinatesValid(rowIndex, columnIndex) ? this.DataGridView.Rows[rowIndex].Cells[columnIndex] : null;
        }

        public DataGridViewColumn GetColumn(int columnIndex) => this.DataGridView.Columns[columnIndex];

        internal void AddColumn(DataGridViewColumn column)
        {
            this.DataGridView.Columns.Add(column);
        }

        internal void ClearColumns()
        {
            this.DataGridView.Columns.Clear();
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //For custom columns, focused must not be checked (eg. for columns with dropdown this would kill the interaction).
            if (this.DataGridView.Focused)
            {
                if (keyData == Keys.Enter)
                {
                    var currentCell = this.DataGridView.CurrentCell;
                    if (currentCell is DataGridViewCheckBoxCell checkBoxCell && checkBoxCell.Value is bool boolValue)
                    {
                        checkBoxCell.Value = !boolValue;
                        //this.ChangeCell(new GridCellChange(currentCell) { OldValue = boolValue, NewValue = !boolValue });
                        this.Refresh(); //Needed for checkbox cell

                        if (currentCell.RowIndex < this.DataGridView.RowCount)
                        {//Move the cursor to the next cell below, same as default behaviour that i am overriding
                            this.DataGridView.CurrentCell = this.DataGridView.Rows[currentCell.RowIndex + 1].Cells[currentCell.ColumnIndex];
                        }

                        return true; //Stop all other cmd to be executed
                    }
                }
            }

            foreach (var column in this.DataGridView.Columns)
            {//This is required for some special actions! (Like arrows for Suggestions!)
                if (column is IGridCustomColumnProcessCmdKey columnProcessCmdKey && columnProcessCmdKey.ProcessCmdKey(ref msg, keyData))
                {
                    return true;
                }
            }

            return false;
        }

        public void HandleDataChangedEvent(GridDataPropertyChangedEventArgs args, int row)
        {
            this.dirty = true;
            this.DataChangedHandler.HandleCellChangeEvent(args, row);
        }

        public void DeleteSelectedCells()
        {
            var req = this.DataChangedHandler.Join();

            foreach (DataGridViewCell selectedCell in DataGridView.SelectedCells)
            {
                selectedCell.Value = null; //Set value to null so it will clear also checkboxes
            }

            this.DataChangedHandler.End(req);
        }

        public void AppendData(IEnumerable<T> dataEnumerable)
        {
            var req = this.DataChangedHandler.Join();

            var emptyIndexList = DataSource.GetFirstEmptyRowIndexes(dataEnumerable.Count());

            int i = 0;
            foreach (var data in dataEnumerable)
            {
                if (i >= emptyIndexList.Count)
                {
                    break;
                }

                var emptyIndex = emptyIndexList[i++];

                var emptyData = this.DataSource[emptyIndex];
                GridUtils.CopyGridDataValues(data, emptyData);
            }

            this.DataChangedHandler.End(req);
        }

        public void SelectRow(int rowIndex)
        {
            this.DataGridView.ClearSelection();

            var row = this.DataGridView.Rows[rowIndex];
            if (row.Cells.Count > 0)
            {
                //I need to set the current cell, because i use the CurrentRow as a "starting row"
                //Do not cancel current cell! It might select the first cell in the grid and mess up selection.
                this.DataGridView.CurrentCell = row.Cells[0];
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Selected = true;
                }
            }
        }

        private void SelectCell(DataGridViewCell? cell)
        {
            this.DataGridView.RefreshEdit(); //This is required to refresh checkbox otherwise, if the undo is in a selected cell, it will not update visually (DATA IS CHANGED!)
            this.DataGridView.Refresh();

            this.DataGridView.ClearSelection();

            this.DataGridView.CurrentCell = cell; //Setting se current cell already center the grid to it.
            this.DataGridView.Refresh();
        }

        public Control GetControl() => this.DataGridView;

    }

    internal class MyGrid : DataGridView
    {
        public MyGrid()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true); //this is the key
            this.DoubleBuffered = true;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            // BUG => System.InvalidOperationException: 'L'operazione non può essere eseguita mentre è in corso il ridimensionamento di una colonna con riempimento automatico.'
            // FIX = https://stackoverflow.com/questions/34344499/invalidoperationexception-this-operation-cannot-be-performed-while-an-auto-fill

            // Touching the TopLeftHeaderCell here prevents
            // System.InvalidOperationException:
            // This operation cannot be performed while
            // an auto-filled column is being resized.

            var topLeftHeaderCell = TopLeftHeaderCell;
            base.OnHandleCreated(e);
        }
    }
}

/*
            bool VerificaColonneConsecutive(DataGridView dgv)
            {//Can you see this is vibe-coded?
                if (dgv.SelectedCells.Count == 0)
                {
                    return false;
                }

                // 1. Otteniamo l'elenco delle colonne coinvolte nella selezione (senza duplicati)
                var colonneSelezionate = dgv.SelectedCells
                    .Cast<DataGridViewCell>()
                    .Select(c => c.OwningColumn)
                    .Distinct()
                    .ToList();

                // 2. Creiamo la sequenza REALE delle colonne visibili a schermo
                // Usiamo DisplayIndex solo per l'ordinamento iniziale, poi ce ne dimentichiamo.
                var mappaVisiva = dgv.Columns
                    .Cast<DataGridViewColumn>()
                    .Where(c => c.Visible)
                    .OrderBy(c => c.DisplayIndex)
                    .ToList();

                // 3. Troviamo la posizione (indice) nella nostra mappa per ogni colonna selezionata
                var posizioni = colonneSelezionate
                    .Select(col => mappaVisiva.IndexOf(col))
                    .Where(idx => idx >= 0) // Esclude eventuali colonne selezionate ma diventate invisibili
                    .OrderBy(idx => idx)
                    .ToList();

                if (posizioni.Count <= 1)
                {
                    return true;
                }

                // 4. Controllo di continuità: la differenza tra l'ultimo e il primo indice 
                // deve essere uguale al numero di colonne selezionate - 1.
                // Esempio: Indici [3, 4, 5] -> 5 - 3 = 2. (3 colonne, 3-1=2). OK!
                // Esempio: Indici [3, 4, 6] -> 6 - 3 = 3. (3 colonne, 3-1=2). NO!
                int min = posizioni.First();
                int max = posizioni.Last();

                return (max - min) == (posizioni.Count - 1);
            }

            bool VerificaRigheConsecutive(DataGridView dgv)
            {//Can you see this is vibe-coded?
                if (dgv.SelectedCells.Count == 0) return false;

                // 1. Otteniamo l'elenco delle righe coinvolte nella selezione (senza duplicati)
                var righeSelezionate = dgv.SelectedCells
                    .Cast<DataGridViewCell>()
                    .Select(c => c.OwningRow)
                    .Distinct()
                    .Where(r => r.Visible) // Consideriamo solo righe effettivamente visibili
                    .ToList();

                if (righeSelezionate.Count <= 1)
                {
                    return true;
                }

                // 2. Creiamo la mappa delle righe visibili nell'ordine corrente della UI
                // Usiamo l'indice della collezione Rows che riflette l'ordine di visualizzazione/ordinamento
                var mappaVisiva = dgv.Rows
                    .Cast<DataGridViewRow>()
                    .Where(r => r.Visible)
                    .OrderBy(r => r.Index)
                    .ToList();

                // 3. Troviamo la posizione relativa nella nostra mappa "filtrata"
                var posizioni = righeSelezionate
                    .Select(r => mappaVisiva.IndexOf(r))
                    .OrderBy(idx => idx)
                    .ToList();

                // 4. Verifica di continuità
                int min = posizioni.First();
                int max = posizioni.Last();

                return (max - min) == (posizioni.Count - 1);
            }
 
*/

/*
          public void ChangeRow(int rowIndex, T data)
        {
            this.ChangeCells(this.DataHandler.CreateCellChanges(rowIndex, data));
        }

        public void ChangeMultipleRows(Dictionary<int, T> dataDict)
        {
            this.ChangeCells(this.DataHandler.CreateCellChanges(dataDict));
        }

        public void AddCachedCellChange(GridCellChange cellChange)
        {
            this.cachedCellChangeList.Add(cellChange);
        }

        public void ExecuteCachedCellChange(bool applyChanges = true)
        {
            this.ChangeCells(this.cachedCellChangeList, applyChanges: applyChanges);
            this.ClearCachedCellChange();
        }

        public void ClearCachedCellChange()
        {
            this.cachedCellChangeList.Clear();
        }

        public void ChangeCell(GridCellChange cellChange, bool applyChanges = true)
        {
            ChangeCells(Utils.SingletonList(cellChange), applyChanges);
        }

        public void ChangeCells(List<GridCellChange>? cellChangeList, bool applyChanges = true)
        {
            if (cellChangeList == null || cellChangeList.Count == 0)
            {
                return;
            }

            //This is to avoid having a list that is reused passed here and would cause problems with undo / redo
            var localCellChangeList = new List<GridCellChange>(cellChangeList);

            if (applyChanges)
            {
                this.DataGridView.SuspendLayout();
            }

            try
            {
                if (applyChanges)
                {
                    this.undoRedoHandler.Lock();
                    foreach (var cellChange in localCellChangeList) //Accessing DataSource instead of changing value of cell is WAAAAY faster (From 3s to 22ms)
                    {
                        var data = this.DataSource[cellChange.RowIndex];
                        if (data == null)
                        {
                            continue;
                        }

                        var dataColumn = data.GetColumn(cellChange.ColumnIndex);

                        var oldValue = dataColumn.GetValueFrom<object>(data);
                        cellChange.OldValue = oldValue;

                        //For CheckBox is needed since passing "True" as string does not count as a valid object!
                        var newValue = cellChange.NewValue;
                        if (newValue is string newValueStr)
                        {
                            var columnType = dataColumn.PropertyInfo.PropertyType;
                            if (columnType == typeof(bool) && bool.TryParse(newValueStr, out bool result))
                            {
                                newValue = result;
                            }
                        }
                        dataColumn.SetValueTo(data, newValue);
                    }
                    this.undoRedoHandler.Unlock();
                }

                this.Events.CellChangeEvent(this.DataGridView, new() { CellChangeList = localCellChangeList });
                undoRedoHandler.AddUndo(() => UndoChangeCells(localCellChangeList));
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }
            finally
            { //This is just in case something goes wrong and i won't leave a lock undoRedo
                undoRedoHandler.Unlock();
            }

            if (applyChanges)
            {
                this.DataGridView.Refresh();
                this.DataGridView.ResumeLayout();
            }
        }

        private void UndoChangeCells(List<GridCellChange> cellChangeList)
        {
            this.DataGridView.SuspendLayout();


            try
            {
                this.undoRedoHandler.Lock();

                List<GridCellChange> invertedCellChangeList = [];

                foreach (var cellChange in cellChangeList) //Accessing DataSource instead of changing value of cell is WAAAAY faster (From 3s to 22ms)
                {
                    var data = this.DataSource[cellChange.RowIndex];
                    var column = data.GetColumn(cellChange.ColumnIndex);

                    var actualValue = column.GetValueFrom<object>(data);
                    var restoreValue = cellChange.OldValue;

                    column.SetValueTo(data, cellChange.OldValue);

                    invertedCellChangeList.Add(new(cellChange.ColumnIndex, cellChange.RowIndex) { OldValue = actualValue, NewValue = restoreValue });
                }
                this.undoRedoHandler.Unlock();

                SelectCell(cellChangeList[^1]);  //Setting se current cell already center the grid to it.
                this.Events.CellChangeEvent(this.DataGridView, new() { CellChangeList = invertedCellChangeList, IsUndo = true });

                undoRedoHandler.AddRedo(() =>
                {
                    ChangeCells(cellChangeList);
                    SelectCell(cellChangeList[0]);  //Setting se current cell already center the grid to it.
                });
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            this.DataGridView.Refresh();
            this.DataGridView.ResumeLayout();

            undoRedoHandler.Unlock(); //And a locked undoRedo
        }
 */ 