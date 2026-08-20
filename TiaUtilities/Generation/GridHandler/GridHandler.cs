using System.Collections.Immutable;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.GridHandler.CustomColumns;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.GridHandler.GridImprovements;
using TiaUtilities.Generation.GridHandler.Improvements;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.JSScript;
using TiaUtilities.Languages;
using TiaUtilities.UndoRedo;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridHandler<T> : IGridHandlerEventCalls, ICleanable, ISaveable<GridSave<T>> where T : GridData
    {
        public required uint InitializeRowCount { get; set; } = 9;

        public int RowCount { get => this.DataGridView.RowCount; }
        public int ColumnCount { get => this.DataGridView.ColumnCount; }

        public GridColumnHandler Columns { get; init; }
        public GridDataSource<T> DataSource { get; init; }
        public GridDataChangedHandler DataChangedHandler { get; init; }
        public List<GridScriptVariable> ScriptVariableList { get; init; }
        public GridSelectionBorder SelectionBorder { get; init; }


        public event GridDataChangedEventHandler DataChanged = delegate { };
        public event GridDataLoadedEvent DataLoaded = delegate { };

        public event DataGridViewCellEventHandler RowSelectedChanged
        {
            add => this.DataGridView.RowEnter += value;
            remove => this.DataGridView.RowEnter -= value;
        }

        public event GridPreSortEventHandler PreSort = delegate { };
        public event GridPostSortEventHandler PostSort = delegate { };

        public event GridExcelDragPreviewEventHandler ExcelDragPreview = delegate { };
        public event GridExcelDragDoneEventHandler ExcelDragDone = delegate { };

        public event DataGridViewCellToolTipTextNeededEventHandler CellToolTipTextNeeded
        {
            add => this.DataGridView.CellToolTipTextNeeded += value;
            remove => this.DataGridView.CellToolTipTextNeeded -= value;
        }


        internal GridSettings GridSettings { get; init; }
        internal GridDataPreviewer<T> DataPreviewer { get; init; }
        private ExcelLikeDataGridView DataGridView { get; init; }


        private readonly GridBindContainer gridBindFactory;
        private readonly GridHandlerEventCaller eventCaller;
        private readonly GenPlaceholderHandler placeholderHandler;


        private readonly UndoRedoHandler undoRedoHandler;
        private readonly GridDoDragDropHandler doDragDropHandler;
        private readonly GridDragDownHandler dragDownHandler;
        private readonly GridSortHandler<T> sortHandler;
        private readonly GridQOL qol;

        private bool init;
        private bool dirty;

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

            this.DataGridView = new();

            this.eventCaller = new(this);
            this.undoRedoHandler = new();

            this.Columns = new(this.DataGridView);
            this.DataChangedHandler = new(this.DataGridView, eventCaller, this.undoRedoHandler);
            this.DataSource = new(this.DataGridView, this.DataChangedHandler, eventCaller);

            this.ScriptVariableList = [];
            this.SelectionBorder = new(this.DataGridView, this.GridSettings);

            this.doDragDropHandler = new(this.DataGridView, this.DataChangedHandler, this.GridSettings);
            this.dragDownHandler = new(this.DataGridView, eventCaller, settings);
            this.sortHandler = new(this.DataGridView, this.DataSource, eventCaller, this.undoRedoHandler, comparer);
            this.qol = new(this.DataGridView);
        }

        #region Call Events
        void IGridHandlerEventCalls.CallPreSortEvent(GridPreSortEventArgs args) => this.PreSort(this.DataGridView, args);

        void IGridHandlerEventCalls.CallPostSortEvent(GridPostSortEventArgs args)
        {
            this.dirty = true;
            this.PostSort(this.DataGridView, args);
        }

        void IGridHandlerEventCalls.CallExcelDragPreviewEvent(GridExcelDragEventArgs args) => this.ExcelDragPreview(this.DataGridView, args);

        void IGridHandlerEventCalls.CallExcelDragDoneEvent(GridExcelDragEventArgs args) => this.ExcelDragDone(this.DataGridView, args);

        void IGridHandlerEventCalls.CallDataChangedEvent(List<GridDataChangedCache> cachedChanges)
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

        void IGridHandlerEventCalls.CallLoadDataEvent() => this.DataLoaded(this.DataGridView, new());
        #endregion

        public void RefreshRow(int rowIndex)
        {
            if (GridUtils.IsRowValid(this.DataGridView, rowIndex))
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
            this.DataGridView.DataError += (obj, args) =>
            {
                var cell = this.DataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex];
                if (cell is DataGridViewComboBoxCell comboBoxCell)
                {
                    comboBoxCell.Value = null;
                }
            };

            void LoadDefaultCellStyle()
            {
                //The real selection colors is applied in CellPainting
                this.DataGridView.DefaultCellStyle.SelectionBackColor = SystemColors.ControlLightLight;
                this.DataGridView.DefaultCellStyle.SelectionForeColor = Color.Black;
                this.DataGridView.DefaultCellStyle.BackColor = SystemColors.ControlLightLight;
                this.DataGridView.DefaultCellStyle.ForeColor = Color.Black;
                this.DataGridView.DefaultCellStyle.Padding = new Padding(2);

                this.DataGridView.Refresh();
            }

            LoadDefaultCellStyle();
            this.GridSettings.PropertyChanged += (sender, args) => LoadDefaultCellStyle();

            this.DataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.DataGridView.ColumnHeadersDefaultCellStyle.Padding = new Padding(0);
            this.DataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;

            this.DataGridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            this.DataGridView.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;

            this.Columns.InitializeColumns();
            this.DataSource.InitializeData(this.InitializeRowCount);

            #region EVENTS(CellMouseDown / CellClick / CellMouseDoubleClick / MouseWheel / RowPostPaint) QOL
            this.DataGridView.CellMouseDown += (sender, args) => this.qol.EventCellMouseDown_FullRowSelection(args.RowIndex, args.ColumnIndex);
            this.DataGridView.CellClick += (sender, args) => this.qol.EventCellClick_ImproveComboBox(args.RowIndex, args.ColumnIndex);
            this.DataGridView.CellMouseDoubleClick += (sender, args) => this.qol.EventCellDoubleClick(args.RowIndex, args.ColumnIndex, args.Button);
            this.DataGridView.MouseWheel += (sender, args) => this.qol.EventMouseWheel_UseWheelDuringSelection(args.Delta);
            this.DataGridView.RowPostPaint += (sender, args) => this.qol.EventRowPostPaint_AddNumbers(args.RowBounds, args.InheritedRowStyle, args.Graphics, args.RowIndex);
            #endregion

            #region EVENTS(SelectionChanged / CellPainting / Paint) - Paint stuff on cells / Draw borders on cells
            this.DataGridView.SelectionChanged += (sender, args) =>
            {
                this.dragDownHandler.EventSelectionChanged();
                this.SelectionBorder.EventSelectionChanged();
            };

            this.DataGridView.CellPainting += (sender, args) =>
            {
                args.Handled = true;

                if (sortHandler.IsTopRow(args.ColumnIndex, args.RowIndex))
                {
                    args.PaintBackground(args.ClipBounds, true);

                    sortHandler.EventCellPainting_DrawText(args.CellBounds, args.Graphics, args.CellStyle, args.RowIndex, args.ColumnIndex, args.FormattedValue); //This paints the top columns cell
                    return;
                }

                var cellStyle = args.CellStyle;
                if (cellStyle != null)
                {
                    var currentCellAddress = this.DataGridView.CurrentCellAddress;
                    if (args.ColumnIndex == currentCellAddress.X && args.RowIndex == currentCellAddress.Y)
                    {
                        cellStyle.SelectionBackColor = cellStyle.BackColor;
                        cellStyle.SelectionForeColor = cellStyle.ForeColor;
                    }
                    else if (args.State.HasFlag(DataGridViewElementStates.Selected))
                    {
                        cellStyle.SelectionBackColor = this.GridSettings.SelectedCellBackColor;
                        cellStyle.SelectionForeColor = this.GridSettings.SelectedCellForeColor;
                    }
                }

                this.dragDownHandler.EventCellPainting(args);
                this.doDragDropHandler.EventCellPainting(args.CellBounds, args.CellStyle, args.ColumnIndex, args.RowIndex); //Change cell style background for DoDragDrop start cell.

                args.PaintBackground(args.ClipBounds, true);

                var previewData = GridUtils.CellValuePreviewRequestData(this, args.ColumnIndex, args.RowIndex);
                if (previewData != null)
                {
                    var method = typeof(DataGridViewCell).GetMethod("Paint", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

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

            RectangleF[] oldBorders = [];
            this.DataGridView.Paint += (sender, args) =>
            {
                try
                {
                    this.SelectionBorder.EventPaint(args.Graphics);

                    this.dragDownHandler.PaintTriangle(args.Graphics);
                    if (this.doDragDropHandler.Busy)
                    {
                        if (oldBorders.Length == 4)
                        {
                            GridSelectionBorder.DrawBorders(args.Graphics, oldBorders, Color.Transparent);
                            oldBorders = [];
                        }

                        var offset = this.doDragDropHandler.GetDifferenceFromMouseDown(Cursor.Position);

                        var bordersWithOffset = this.SelectionBorder.CalculateBorders(this.GridSettings.BorderWeight + 2, offset.Y, offset.X);
                        GridSelectionBorder.DrawBorders(args.Graphics, bordersWithOffset, this.GridSettings.SingleSelectedCellBorderColor);

                        oldBorders = bordersWithOffset;

                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex);
                }
            };
            #endregion

            #region EVENTS(MouseMove) - Change cursor
            this.DataGridView.MouseMove += (sender, args) =>
            {
                this.SelectionBorder.EventMouseMove_CursorInsideBorder(args.X, args.Y);

                this.DataGridView.Cursor = Cursors.Default;

                var hitTest = this.DataGridView.HitTest(args.X, args.Y);
                if (hitTest.Type == DataGridViewHitTestType.Cell)
                {
                    var rowIndex = hitTest.RowIndex;
                    var columnIndex = hitTest.ColumnIndex;

                    if (this.dragDownHandler.Started)
                    {
                        this.DataGridView.Cursor = Cursors.Cross;
                        return;
                    }

                    if (this.dragDownHandler.RequiresCursorCross(args.X, args.Y, columnIndex, rowIndex))
                    {
                        this.DataGridView.Cursor = Cursors.Cross;
                        return;
                    }

                    if (this.SelectionBorder.CursorInsideBorder)
                    {//Display cursor for do drag&drop
                        this.DataGridView.Cursor = Cursors.SizeAll;
                    }
                }
            };
            #endregion

            #region EVENTS(MouseLeave / MouseDown / LostFocus / DragOver / DragDrop / QueryContinueDrag) - DragDown / DoDragDrop
            this.DataGridView.MouseDown += (sender, args) =>
            {
                if (args.Button != MouseButtons.Left)
                {
                    return;
                }

                var hitTest = this.DataGridView.HitTest(args.Location.X, args.Location.Y);
                if (hitTest.Type != DataGridViewHitTestType.Cell)
                {
                    return;
                }

                var columnIndex = hitTest.ColumnIndex;
                var rowIndex = hitTest.RowIndex;

                this.dragDownHandler.EventMouseDown(args.Location, columnIndex, rowIndex);

                if (!this.dragDownHandler.Started && this.SelectionBorder.CursorInsideBorder)
                {
                    this.doDragDropHandler.EventMouseDown(columnIndex, rowIndex);
                }
            };

            this.DataGridView.MouseUp += (sender, args) => this.dragDownHandler.EventMouseUp();
            this.DataGridView.MouseLeave += (sender, args) => this.SelectionBorder.EventMouseLeave();
            this.DataGridView.LostFocus += (sender, args) => this.dragDownHandler.EventLostFocus();

            this.DataGridView.QueryContinueDrag += (sender, args) =>
            {
                var gridPoint = this.DataGridView.PointToClient(Cursor.Position);

                var hitTest = this.DataGridView.HitTest(gridPoint.X, gridPoint.Y);
                if (hitTest.Type == DataGridViewHitTestType.Cell)
                {
                    var columnIndex = hitTest.ColumnIndex;
                    var rowIndex = hitTest.RowIndex;

                    var displayedRowCount = this.DataGridView.DisplayedRowCount(includePartialRow: false);

                    if (rowIndex < this.DataGridView.FirstDisplayedScrollingRowIndex + 4)
                    {
                        this.qol.Scroll(-1);
                    }
                    if (rowIndex < this.DataGridView.FirstDisplayedScrollingRowIndex + 2)
                    {
                        this.qol.Scroll(-2);
                    }
                    else if (rowIndex > this.DataGridView.FirstDisplayedScrollingRowIndex + displayedRowCount - 2)
                    {
                        this.qol.Scroll(2);
                    }
                    else if (rowIndex > this.DataGridView.FirstDisplayedScrollingRowIndex + displayedRowCount - 4)
                    {
                        this.qol.Scroll(1);
                    }
                }

                this.Refresh(); //Force refresh also during DoDragDrop!
            };

            this.DataGridView.DragOver += (sender, args) =>
            {
                args.Effect = this.doDragDropHandler.EventDragOver(args.Data);
            };

            this.DataGridView.DragDrop += (sender, args) =>
            {
                this.doDragDropHandler.EventDragDrop(args.Data, args.X, args.Y);
            };
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
                        GridUtils.PasteAsExcelFromClipboard(this.DataGridView);
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

            #region EVENTS(CellFormatting) - Custom formatting for DataGridViewButtonCell
            this.DataGridView.CellFormatting += (sender, args) =>
            {
                int columnIndex = args.ColumnIndex;
                int rowIndex = args.RowIndex;

                if (!GridUtils.AreCoordinatesValid(this.DataGridView, rowIndex, columnIndex))
                {
                    return;
                }

                var cell = this.DataGridView.Rows[rowIndex].Cells[columnIndex];
                if (cell is DataGridViewButtonCell buttonCell)
                {
                    var data = this.DataSource[rowIndex][columnIndex];
                    args.Value = $"{data}";
                }
            };
            #endregion

            #region EVENTS(CellContentClick) - Fix for edit mode for DataGridViewCheckBoxCell
            this.DataGridView.CellContentClick += (sender, args) =>
            {
                var rowIndex = args.RowIndex;
                var columnIndex = args.ColumnIndex;
                if (!GridUtils.AreCoordinatesValid(this.DataGridView, rowIndex, columnIndex))
                {
                    return;
                }

                var cell = this.DataGridView.Rows[rowIndex].Cells[columnIndex];
                if (cell is DataGridViewCheckBoxCell checkBoxCell)
                {
                    this.DataGridView.EndEdit(); //If a checkbox is clicked, it goes in edit mode. I do not want that, is confusing. This fixes it.
                }
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

            #region EVENTS(ColumnHeaderMouseClick) - SortHandler
            this.DataGridView.ColumnHeaderMouseClick += (sender, args) => this.sortHandler.EventColumnHeaderMouseClick(args.Button, args.ColumnIndex);
            #endregion

            this.GridSettings.PropertyChanged += (sender, args) => this.DataGridView.Refresh();
            this.DataGridView.VisibleChanged += (sender, args) => this.DataGridView.AutoResizeColumnHeadersHeight();

            this.DataGridView.ResumeLayout(true);

            init = true;
        }

        public void MarkDirty() => this.dirty = true;

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

            this.eventCaller.CallLoadDataEvent();
        }

        public Form? FindForm() => this.DataGridView.FindForm();

        public void AutoResizeColumns() => this.DataGridView.AutoResizeColumns();

        public void AutoResizeColumnHeadersHeight() => this.DataGridView.AutoResizeColumnHeadersHeight();

        public DataGridViewCell? GetCurrentCell() => this.DataGridView.CurrentCell;

        public void ChangeCurrentCell(int rowIndex, int columnIndex)
        {
            if (GridUtils.AreCoordinatesValid(this.DataGridView, rowIndex, columnIndex))
            {
                this.DataGridView.CurrentCell = this.DataGridView.Rows[rowIndex].Cells[columnIndex];
            }
        }

        public DataGridViewCell? GetCell(int rowIndex, GridDataColumn column) => this.GetCell(rowIndex, column.ColumnIndex);

        public DataGridViewCell? GetCell(int rowIndex, int columnIndex)
        {
            return GridUtils.AreCoordinatesValid(this.DataGridView, rowIndex, columnIndex) ? this.DataGridView.Rows[rowIndex].Cells[columnIndex] : null;
        }

        public DataGridViewColumn GetColumn(int columnIndex) => this.DataGridView.Columns[columnIndex];

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

        public void SelectRow(int rowIndex) => this.qol.SelectRow(rowIndex);

        public Control GetControl() => this.DataGridView;
    }

    public class ExcelLikeDataGridView : DataGridView
    {
        public ImmutableList<DataGridViewColumn> VisibleColumns { get; private set; } = [];
        public ImmutableList<DataGridViewRow> VisibleRows { get; private set; } = [];

        public ExcelLikeDataGridView()
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

        protected override void OnColumnDisplayIndexChanged(DataGridViewColumnEventArgs e)
        {
            base.OnColumnDisplayIndexChanged(e);
            this.UpdateVisibleColumns();
        }

        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            base.OnColumnAdded(e);
            this.UpdateVisibleColumns();
        }

        protected override void OnColumnRemoved(DataGridViewColumnEventArgs e)
        {
            base.OnColumnRemoved(e);
            this.UpdateVisibleColumns();
        }

        protected override void OnColumnStateChanged(DataGridViewColumnStateChangedEventArgs e)
        {
            base.OnColumnStateChanged(e);
            if (e.StateChanged.HasFlag(DataGridViewElementStates.Selected))
            {
                this.UpdateVisibleColumns();
            }
        }
        protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
        {
            base.OnRowsAdded(e);
            this.UpdateVisibleRows();
        }

        protected override void OnRowsRemoved(DataGridViewRowsRemovedEventArgs e)
        {
            base.OnRowsRemoved(e);
            this.UpdateVisibleRows();
        }

        protected override void OnRowStateChanged(int rowIndex, DataGridViewRowStateChangedEventArgs e)
        {
            base.OnRowStateChanged(rowIndex, e);
            if (e.StateChanged.HasFlag(DataGridViewElementStates.Selected))
            {
                this.UpdateVisibleRows();
            }
        }

        private void UpdateVisibleColumns()
        {
            this.VisibleColumns = this.Columns.Cast<DataGridViewColumn>()
                                       .Where(col => col.Visible)
                                       .OrderBy(col => col.DisplayIndex)
                                       .ToImmutableList();
        }

        private void UpdateVisibleRows()
        {
            this.VisibleRows = this.Rows.Cast<DataGridViewRow>()
                                    .Where(row => row.Visible)
                                    .OrderBy(row => row.Index)
                                    .ToImmutableList();
        }

    }
}