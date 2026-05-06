using System.Diagnostics;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.GridHandler.CustomColumns;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Languages;
using TiaUtilities.UndoRedo;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridHandler<T> : ICleanable, ISaveable<GridSave<T>> where T : GridData
    {
        internal class CellTag
        {

        }

        private readonly GridBindContainer gridBindFactory;

        private readonly UndoRedoHandler undoRedoHandler;
        private readonly GridExcelDragHandler<T> excelDragHandler;
        private readonly GridSortHandler<T> sortHandler;
        private readonly GenPlaceholderHandler placeholderHandler;

        internal GridSettings GridSettings { get; init; }
        internal GridDataPreviewer<T> DataPreviewer { get; init; }
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
        private bool dragAndDropBusy;

        private RectangleF topBorderRect = RectangleF.Empty;
        private RectangleF bottomBorderRect = RectangleF.Empty;
        private RectangleF leftBorderRect = RectangleF.Empty;
        private RectangleF rightBorderRect = RectangleF.Empty;

        private readonly List<DataGridViewCell> oldSelectedCellsList = [];
        private bool selectedCellsArePlanar = false;

        public GridHandler(GridSettings settings,
            GridBindContainer gridBindFactory,
            GridDataPreviewer<T> previewer,
            GenPlaceholderHandler placeholderHandler,
            IGridRowComparer<T>? comparer = null)
        {
            this.GridSettings = settings;
            this.gridBindFactory = gridBindFactory;
            this.DataPreviewer = previewer;
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

        internal void CallPreSortEvent(GridPreSortEventArgs args) => this.PreSort(this.DataGridView, args);

        internal void CallPostSortEvent(GridPostSortEventArgs args)
        {
            this.dirty = true;
            this.PostSort(this.DataGridView, args);
        }

        internal void CallExcelDragPreviewEvent(GridExcelDragEventArgs args) => this.ExcelDragPreview(this.DataGridView, args);


        internal void CallExcelDragDoneEvent(GridExcelDragEventArgs args) => this.ExcelDragDone(this.DataGridView, args);


        internal void CallDataChangedEvent(List<GridDataChangedCache> cachedChanges)
        {
            if (cachedChanges.Count > 0)
            {
                this.dirty = true;

                GridDataChangedEventArgs args = new();
                args.ChangedCellDataList.AddRange(
                    cachedChanges.Select(c => new GridChangedData(c.Args, c.Row))
                );

                this.DataChanged(this.DataGridView, args);
            }
        }

        private void CallRowSelectedChangedEvent(GridSelectedRowChangedArgs args) => this.RowSelectedChanged(this.DataGridView, args);

        private void CallLoadDataEvent() => this.DataLoaded(this.DataGridView, new());


        internal void SetDataSource(object dataSource) => this.DataGridView.DataSource = dataSource;

        public void RefreshRow(int rowIndex)
        {
            if (this.IsCellRowValid(rowIndex))
            {
                var row = this.DataGridView.Rows[rowIndex];
                foreach (DataGridViewCell cell in row.Cells)
                {
                    this.RefreshCell(cell);
                }
            }
        }

        public void RefreshCell(DataGridViewCell cell) => this.DataGridView.InvalidateCell(cell);

        public void RefreshCell(int columnIndex, int rowIndex) => this.DataGridView.InvalidateCell(columnIndex, rowIndex);

        public void Refresh()
        {
            this.DataGridView.RefreshEdit();
            this.DataGridView.Refresh();
        }

        public void SuspendLayout() => this.DataGridView.SuspendLayout();

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
            this.DataGridView.Font = GridSettings.GridFont;
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

            #region EVENTS(MouseDown/CellClick/CellMouseDoubleClick) QOL - Quality of life

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
                if (Control.ModifierKeys == Keys.Shift || Control.ModifierKeys == Keys.Control || args.Button != MouseButtons.Left || args.RowIndex < 0 || args.ColumnIndex < 0)
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

            #region EVENTS(CellPainting/SelectionChanged/Paint) - Paint stuff on cells / Draw borders on cells

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

                var previewData = GridUtils.CellValuePreviewRequestData(this, args.ColumnIndex, args.RowIndex);
                if (previewData != null)
                {
                    /* protected virtual void Paint(Graphics graphics,
                           Rectangle clipBounds,
                           Rectangle cellBounds,
                           int rowIndex,
                           DataGridViewElementStates cellState,
                           object value,
                           object formattedValue,
                           string errorText,
                           DataGridViewCellStyle cellStyle,
                           DataGridViewAdvancedBorderStyle advancedBorderStyle,
                           DataGridViewPaintParts paintParts) */
                    var method = typeof(DataGridViewCell).GetMethod("Paint", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                    //This methods is inside DataGridViewCell.
                    //This allows to draw stuff, like drop down arrow on combo boxes
                    var cell = this.DataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex];
                    method?.Invoke(cell,
                        [args.Graphics,
                        args.ClipBounds,
                        args.CellBounds,
                        args.RowIndex,
                        args.State,
                        args.Value,
                        args.FormattedValue,
                        args.ErrorText,
                        args.CellStyle,
                        args.AdvancedBorderStyle,
                        DataGridViewPaintParts.ContentBackground | DataGridViewPaintParts.ErrorIcon ]);

                    GridUtils.CellValuePreviewContentPaint(this, previewData, this.placeholderHandler, args);
                }
                else
                {
                    args.PaintContent(args.ClipBounds);
                }
            };

            this.DataGridView.SelectionChanged += (sender, args) =>
            {
                var selectedCells = this.DataGridView.SelectedCells;
                if (selectedCells == null || selectedCells.Count == 0)
                {
                    oldSelectedCellsList.Clear();
                    return;
                }

                selectedCellsArePlanar = GridUtils.AreSelectedCellsPlanar(this.DataGridView);

                if (oldSelectedCellsList.Count > 0)
                {
                    foreach (var cell in selectedCells.Cast<DataGridViewCell>().Intersect(oldSelectedCellsList))
                    {
                        if (cell.DataGridView == this.DataGridView)
                        {//It could happen
                            this.DataGridView.InvalidateCell(cell);
                        }
                    }

                    oldSelectedCellsList.Clear();
                }

                oldSelectedCellsList.AddRange(this.DataGridView.SelectedCells.Cast<DataGridViewCell>());
            };

            this.DataGridView.Paint += (sender, args) =>
            {
                if (dragAndDropBusy)
                {
                    return;
                }

                try
                {
                    topBorderRect = bottomBorderRect = leftBorderRect = rightBorderRect = RectangleF.Empty;
                    if (!this.DataGridView.AreAllCellsSelected(false) && selectedCellsArePlanar && this.DataGridView.SelectedCells.Cast<DataGridViewCell>().Any(c => c.Displayed))
                    {
                        var borders = GridUtils.GetSelectedCellsBorderCoordinates(this.DataGridView);

                        var topLeft = oldSelectedCellsList.FirstOrDefault(c => c.ColumnIndex == borders.Left && c.RowIndex == borders.Top);
                        var topRight = oldSelectedCellsList.FirstOrDefault(c => c.ColumnIndex == borders.Right && c.RowIndex == borders.Top);
                        var bottomLeft = oldSelectedCellsList.FirstOrDefault(c => c.ColumnIndex == borders.Left && c.RowIndex == borders.Bottom);
                        var bottomRight = oldSelectedCellsList.FirstOrDefault(c => c.ColumnIndex == borders.Right && c.RowIndex == borders.Bottom);

                        if (topLeft != null && topRight != null && bottomLeft != null && bottomRight != null)
                        {
                            var borderSize = 2f;
                            using Pen borderPen = new(this.GridSettings.SingleSelectedCellBorderColor, borderSize);
                            using Brush borderBrush = new SolidBrush(this.GridSettings.SingleSelectedCellBorderColor);

                            Rectangle topLeftBounds;
                            Rectangle topRightBounds;
                            if (topLeft.Displayed && topRight.Displayed)
                            {
                                topLeftBounds = this.DataGridView.GetCellDisplayRectangle(topLeft.ColumnIndex, topLeft.RowIndex, true);
                                topRightBounds = this.DataGridView.GetCellDisplayRectangle(topRight.ColumnIndex, topRight.RowIndex, true);

                                topBorderRect = new()
                                {
                                    X = topLeftBounds.X,
                                    Y = topLeftBounds.Y,
                                    Width = (topRightBounds.X + topRightBounds.Width) - (topLeftBounds.X + 1),
                                    Height = borderSize
                                };
                                args.Graphics.FillRectangle(borderBrush, topBorderRect);
                            }
                            else
                            {
                                var topDisplayRow = this.DataGridView.FirstDisplayedScrollingRowIndex;
                                topLeftBounds = this.DataGridView.GetCellDisplayRectangle(topLeft.ColumnIndex, topDisplayRow, true);
                                topRightBounds = this.DataGridView.GetCellDisplayRectangle(topRight.ColumnIndex, topDisplayRow, true);
                            }

                            var rowHeight = this.DataGridView.RowTemplate.Height;

                            Rectangle bottomLeftBounds = this.DataGridView.GetCellDisplayRectangle(bottomLeft.ColumnIndex, bottomLeft.RowIndex, true);
                            Rectangle bottomRightBounds = this.DataGridView.GetCellDisplayRectangle(bottomRight.ColumnIndex, bottomRight.RowIndex, true);
                            if (bottomLeft.Displayed && bottomLeftBounds.Height == rowHeight && bottomRight.Displayed && bottomRightBounds.Height == rowHeight)
                            {
                                bottomBorderRect = new()
                                {
                                    X = bottomLeftBounds.X,
                                    Y = bottomLeftBounds.Y + bottomLeftBounds.Height - 1 - borderSize,
                                    Width = (bottomRightBounds.X + bottomRightBounds.Width) - (bottomLeftBounds.X + 1),
                                    Height = borderSize
                                };
                                args.Graphics.FillRectangle(borderBrush, bottomBorderRect);
                            }
                            else
                            {
                                var lastDisplayedRow = this.DataGridView.FirstDisplayedScrollingRowIndex + this.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Displayed | DataGridViewElementStates.Visible) - 1;

                                bottomLeftBounds = this.DataGridView.GetCellDisplayRectangle(bottomLeft.ColumnIndex, lastDisplayedRow, true);
                                bottomRightBounds = this.DataGridView.GetCellDisplayRectangle(bottomRight.ColumnIndex, lastDisplayedRow, true);
                            }

                            leftBorderRect = new()
                            {
                                X = topLeftBounds.X,
                                Y = topLeftBounds.Y,
                                Width = borderSize,
                                Height = (bottomLeftBounds.Y + bottomLeftBounds.Height) - topLeftBounds.Y
                            };
                            args.Graphics.FillRectangle(borderBrush, leftBorderRect);

                            rightBorderRect = new()
                            {
                                X = topRightBounds.X + topRightBounds.Width - borderSize - 1,
                                Y = topRightBounds.Y,
                                Width = borderSize,
                                Height = (bottomRightBounds.Y + bottomRightBounds.Height) - topRightBounds.Y
                            };
                            args.Graphics.FillRectangle(borderBrush, rightBorderRect);
                        }
                    }

                    this.excelDragHandler.PaintTriangle(args.Graphics);
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex);
                }
            };
            #endregion

            #region EVENTS(CellMouseMove/MouseLeave/MouseDown/DragOver/DragDrop) - Drag&Drop / Custom cursor on cell mouse

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
                if (!this.AreCellCoordinatesValid(rowIndex, columnIndex))
                {
                    return;
                }

                var currentCell = this.DataGridView.CurrentCell;
                if (currentCell != null && rowIndex == currentCell.RowIndex && columnIndex == currentCell.ColumnIndex && excelDragHandler.MouseShouldDisplayCursor(args))
                {
                    this.DataGridView.Cursor = Cursors.Cross;
                    return;
                }

                var cell = this.DataGridView.Rows[rowIndex].Cells[columnIndex];
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

                    var cursorPosition = this.DataGridView.PointToClient(Cursor.Position);
                    if (topBorder.Contains(cursorPosition) || bottomBorder.Contains(cursorPosition) || leftBorder.Contains(cursorPosition) || rightBorder.Contains(cursorPosition))
                    {
                        this.DataGridView.Cursor = Cursors.SizeAll;
                        cursorInsideBorder = true;
                    }
                }
            };

            this.DataGridView.MouseLeave += (sender, args) => cursorInsideBorder = false;

            this.DataGridView.MouseDown += (sender, args) =>
            {
                if (!this.excelDragHandler.IsStarted() && cursorInsideBorder && args.Button == MouseButtons.Left)
                {
                    var text = GridUtils.GetCopyAsExcelText(this.DataGridView, out var _);
                    if (text != null)
                    {
                        this.DataGridView.DoDragDrop(text, DragDropEffects.Move, null, Point.Empty, true);
                        cursorInsideBorder = false;
                    }
                }

            };

            this.DataGridView.DragOver += (sender, args) =>
            {
                var data = args.Data?.GetData(typeof(string));
                args.Effect = data is not string ? DragDropEffects.None : DragDropEffects.Move;
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

                            dragAndDropBusy = true;

                            this.SuspendLayout();

                            var req = this.DataChangedHandler.Join();
                            foreach (DataGridViewCell selectedCell in this.DataGridView.SelectedCells)
                            {
                                selectedCell.Value = default;
                            }

                            GridUtils.PasteAsExcel(this.DataGridView, excelText, rowIndex, columnIndex, out var pastedCellsList, ignoreSingleLineMultiplePasting: true);
                            this.DataChangedHandler.End(req);

                            this.FindForm()?.BeginInvoke(() =>
                            {
                                DllImports.RaiseLeftMouse(Cursor.Position); //This fixes the mouse down stuck after dropping.

                                this.DataGridView.ClearSelection();
                                this.DataGridView.CurrentCell = this.DataGridView.Rows[rowIndex].Cells[columnIndex];

                                foreach (var cell in pastedCellsList)
                                {
                                    cell.Selected = true;
                                }

                                dragAndDropBusy = false;
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
                }

            };
            #endregion

            #region EVENTS(RowPostPaint) - Add row header numbers
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

            #region EVENTS(KeyDown) - SelectAll/Cut/Copy/Paste/Delete/Undo/Redo/Find
            this.DataGridView.KeyDown += (sender, args) =>
            {
                var handled = true;
                switch (args.KeyData)
                {
                    case Keys.A | Keys.Control:
                        this.DataGridView.SelectAll(); //KISS
                        break;
                    case Keys.Z | Keys.Control:
                        undoRedoHandler.Undo();
                        break;
                    case Keys.Y | Keys.Control:
                        undoRedoHandler.Redo();
                        break;
                    case Keys.X | Keys.Control:
                        this.SuspendLayout();

                        GridUtils.CopyAsExcelToClipboard(this.DataGridView, out var copiedCellsList);

                        var ctrlXReq = this.DataChangedHandler.Join();
                        foreach (var cell in copiedCellsList)
                        {
                            cell.Value = default;
                        }
                        this.DataChangedHandler.End(ctrlXReq);

                        this.ResumeLayout();
                        break;
                    case Keys.C | Keys.Control:
                        GridUtils.CopyAsExcelToClipboard(this.DataGridView, out var _);
                        break;
                    case Keys.Insert | Keys.Shift:
                    case Keys.V | Keys.Control:

                        var ctrlVReq = this.DataChangedHandler.Join();
                        GridUtils.PasteAsExcelFromClipboard(this.DataGridView, out var _);
                        this.DataChangedHandler.End(ctrlVReq);

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
                        this.DataGridView.RefreshEdit(); //This is required to refresh checkbox otherwise, if the undo is in a selected cell, it will not update visually (DATA IS CHANGED!)
                        this.DataGridView.Refresh();

                        this.DataGridView.ClearSelection();
                        this.DataGridView.CurrentCell = null; //Setting se current cell already center the grid to it.

                        this.DataGridView.Refresh();
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

            #region EVENTS(MouseWheel) - Allow scroll while selecting
            this.DataGridView.MouseWheel += (sender, args) =>
            {
                if ((Control.MouseButtons & MouseButtons.Left) != 0)
                {
                    var rowCount = this.DataGridView.RowCount;
                    var firstDisplayedRowIndex = this.DataGridView.FirstDisplayedScrollingRowIndex;

                    var rowToShow = firstDisplayedRowIndex - args.Delta / 40;

                    var displayedRowCount = this.DataGridView.DisplayedRowCount(false);
                    rowToShow = Math.Max(rowToShow, 0);
                    rowToShow = Math.Min(rowToShow, rowCount - displayedRowCount);

                    this.DataGridView.FirstDisplayedScrollingRowIndex = rowToShow;

                }
            };
            #endregion

            #region EVENTS(CellFormatting) - Custom formatting for DataGridViewButtonCell
            this.DataGridView.CellFormatting += (sender, args) =>
            {
                if (this.DataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex] is DataGridViewButtonCell buttonCell)
                {
                    var data = this.DataSource[args.RowIndex];
                    args.Value = $"{data[args.ColumnIndex]}";
                }
            };
            #endregion

            #region EVENTS(CellContentClick/CellContentDoubleClick) - Logics for DataGridViewCheckBoxCell / DataGridViewEventableButtonColumn
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

            #region EVENTS(RowEnter) - CallRowSelectedChangedEvent
            this.DataGridView.RowEnter += (sender, args) =>
            {
                this.CallRowSelectedChangedEvent(new() { RowIndex = args.RowIndex, ColumnIndex = args.ColumnIndex });
            };
            #endregion

            #region EVENTS(MouseClick/CellMouseClick) Javascripts - Add context menu to top left cell
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

            this.GridSettings.PropertyChanged += (sender, args) => this.DataGridView.Refresh();
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

        public Form? FindForm() => this.DataGridView.FindForm();

        public void AutoResizeColumns() => this.DataGridView.AutoResizeColumns();

        public void AutoResizeColumnHeadersHeight() => this.DataGridView.AutoResizeColumnHeadersHeight();

        private bool IsCellRowValid(int row) => row >= 0 && row < this.RowCount;

        private bool IsCellColumnValid(int column) => column >= 0 && column < this.ColumnCount;

        private bool AreCellCoordinatesValid(int row, int column) => this.IsCellRowValid(row) && this.IsCellColumnValid(column);

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

        internal void AddColumn(DataGridViewColumn column) => this.DataGridView.Columns.Add(column);

        internal void ClearColumns() => this.DataGridView.Columns.Clear();

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