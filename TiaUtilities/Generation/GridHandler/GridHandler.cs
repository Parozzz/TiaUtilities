﻿using TiaUtilities;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaXmlReader.Generation.GridHandler.CustomColumns;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.Generation.GridHandler.Events;
using TiaXmlReader.GenerationForms;
using TiaXmlReader.Javascript;
using TiaXmlReader.UndoRedo;
using TiaXmlReader.Utility;
using TiaXmlReader.Utility.Extensions;
using static TiaUtilities.Generation.GridHandler.GridFindForm;

namespace TiaXmlReader.Generation.GridHandler
{
    public class GridHandler<C, T> : ICleanable where C : IGenerationConfiguration where T : IGridData<C>
    {
        private class ColumnInfo(DataGridViewColumn column, GridDataColumn dataColumn, int width)
        {
            public DataGridViewColumn Column { get; init; } = column;
            public GridDataColumn DataColumn { get; init; } = dataColumn;
            public int Width { get; init; } = width;
            public bool Visible { get; set; } = true;
        }

        private readonly GridSettings settings;
        private readonly C configuration;

        public GridEvents Events { get; init; }

        private readonly UndoRedoHandler undoRedoHandler;
        private readonly GridExcelDragHandler excelDragHandler;
        private readonly GridSortHandler<C, T> sortHandler;

        public DataGridView DataGridView { get; init; }
        public GridDataHandler<C, T> DataHandler { get; init; }
        public GridDataSource<C, T> DataSource { get; init; }
        public GridScript<C, T> Script { get; init; }
        
        public FindData<C, T>? FindData { get; set; }

        private readonly List<ColumnInfo> columnInfoList;
        private readonly List<IGridCellPainter> cellPainterList;
        private bool init;

        private bool dirty;

        public uint RowCount { get; set; } = 9;
        public bool AddRowIndexToRowHeader { get; set; } = true;
        public bool EnablePasteFromExcel { get; set; } = true;
        public bool EnableRowSelectionFromRowHeaderClick { get; set; } = true;
        public bool ShowJSContextMenuTopLeft { get; set; } = true;

        public GridHandler(JavascriptErrorReportThread jsErrorThread, GridSettings settings, C configuration, GridScriptContainer scriptContainer, IGridRowComparer<C, T> comparer = null)
        {
            this.DataGridView = new MyGrid();

            this.settings = settings;
            this.configuration = configuration;
            this.Events = new GridEvents();

            this.undoRedoHandler = new UndoRedoHandler();
            this.excelDragHandler = new GridExcelDragHandler(this.DataGridView, this.Events, settings);

            this.DataHandler = new GridDataHandler<C, T>(this.DataGridView);
            this.DataSource = new GridDataSource<C, T>(this.DataGridView, this.DataHandler);
            this.sortHandler = new GridSortHandler<C, T>(this, this.undoRedoHandler, comparer);
            this.Script = new GridScript<C, T>(this, jsErrorThread, scriptContainer);
            

            this.columnInfoList = [];
            this.cellPainterList = [];
        }

        public void AddCellPainter(IGridCellPainter cellPainter)
        {
            cellPainterList.Add(cellPainter);
        }

        public void Refresh()
        {
            this.DataGridView.RefreshEdit();
            this.DataGridView.Refresh();
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

            this.DataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable; //Do it myself!
            this.DataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.DataGridView.MultiSelect = true;

            this.DataGridView.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            this.DataGridView.Font = settings.GridFont;
            this.DataGridView.AllowUserToAddRows = false;

            this.DataGridView.DataError += DataErrorEventHandler;

            this.DataSource.InitializeData(this.RowCount);

            InitColumns();

            #region Cell Paiting
            var paintHandler = new GridCellPaintHandler(this.DataGridView);
            paintHandler.AddPainter(this.sortHandler); //ORDER IS IMPORTANT!
            paintHandler.AddPainter(this.excelDragHandler);
            paintHandler.AddPainter(new GridCellPreview<C, T>(this.DataSource, this.configuration, this.settings));
            paintHandler.AddPainterRange(cellPainterList);
            paintHandler.Init();
            #endregion

            #region RowHeaderNumber
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

            #region KeyDown - Paste/Delete/Undo/Redo/Find
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
                        GridUtils.CopyAsExcel(this.DataGridView);
                        break;
                    case Keys.Insert | Keys.Shift:
                    case Keys.V | Keys.Control:
                        ChangeCells(GridUtils.PasteAsExcel(this.DataGridView));
                        break;
                    case Keys.F | Keys.Control:
                        GridFindForm.StartFind(this);
                        break;
                    case Keys.Delete:
                        this.DeleteSelectedCells();
                        break;
                    case Keys.Escape:
                        this.ResetSelectedCell();
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

            #region MouseClick - RowSelection
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
                                this.DataGridView.ClearSelection();

                                var hitRow = this.DataGridView.Rows[hitTest.RowIndex];
                                if (hitRow.Cells.Count > 0)
                                {
                                    //I need to set the current cell, because i use the CurrentRow as a "starting row"
                                    //Do not cancel current cell! It might select the first cell in the grid and mess up selection.
                                    this.DataGridView.CurrentCell = hitRow.Cells[0];
                                    foreach (DataGridViewCell cell in hitRow.Cells)
                                    {
                                        cell.Selected = true;
                                    }
                                }
                            }

                            break;
                        case DataGridViewHitTestType.Cell:
                            break;
                    }
                };
            }
            #endregion

            #region CellEdit Begin-End
            GridCellChange? editCellChange = null;
            DataGridView.CellBeginEdit += (sender, args) =>
            {
                editCellChange = null;

                var cell = this.DataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex];
                if (cell is DataGridViewTextBoxCell || cell is DataGridViewComboBoxCell)
                {
                    editCellChange = new GridCellChange(cell) { OldValue = cell.Value };
                }
            };

            DataGridView.CellEndEdit += (sender, args) =>
            {
                if (editCellChange == null || editCellChange.ColumnIndex != args.ColumnIndex || editCellChange.RowIndex != args.RowIndex)
                {
                    return;
                }

                var cell = this.DataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex];
                editCellChange.NewValue = cell.Value;
                ChangeCell(editCellChange, applyChanges: false);

                editCellChange = null;
            };

            DataGridView.CellContentClick += (sender, args) =>
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
                    var oldValue = (bool)(checkBoxCell.Value ?? false); //In this case the value is still the old one. For checkBox, been boolean value, i can predict the next!
                    var newValue = !oldValue;
                    ChangeCell(new GridCellChange(checkBoxCell) { OldValue = oldValue, NewValue = newValue }, applyChanges: false);

                    DataGridView.EndEdit(); //If a checkbox is clicked, it goes in edit mode. I do not want that, is confusing. This fixes it.
                }
            };
            #endregion

            #region SHOW_JS_CONTEXT_MENU
            this.DataGridView.CellMouseClick += (sender, args) =>
            {
                if (args.RowIndex == -1 && args.ColumnIndex == -1 && args.Button == MouseButtons.Right)
                {
                    var menuItem = new ToolStripMenuItem { Text = "Execute Javascript" };
                    menuItem.Click += (s, a) => this.Script.ShowConfigForm(this.DataGridView);

                    var contextMenu = new ContextMenuStrip();
                    contextMenu.Items.Add(menuItem);

                    contextMenu.Show(this.DataGridView, this.DataGridView.PointToClient(Cursor.Position));
                }
            };
            #endregion

            this.settings.PropertyChanged += (sender, args) => this.DataGridView.Refresh();

            this.excelDragHandler.Init();
            this.sortHandler.Init();

            this.DataGridView.ResumeLayout();

            #region IS_DIRTY
            this.Events.CellChange += (sender, args) => this.dirty = true;
            this.Events.PostSort += (sender, args) => this.dirty = true;
            #endregion

            init = true;
        }

        public bool IsDirty() => this.dirty;
        public void Wash() => this.dirty = false;

        public GridSave<C, T> CreateSave()
        {
            return new()
            {
                RowData = this.DataSource.CreateSave()
            };
        }

        public void LoadSave(GridSave<C, T> gridSave)
        {
            this.DataGridView.SuspendLayout();

            this.DataSource.LoadSave(gridSave.RowData);
            
            this.DataGridView.Refresh();
            this.DataGridView.ResumeLayout();
        }

        private void DataErrorEventHandler(object sender, DataGridViewDataErrorEventArgs args)
        {
            var cell = this.DataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex];
            if (cell is DataGridViewComboBoxCell comboBoxCell)
            {
                comboBoxCell.Value = "";
            }
        }

        public DataGridViewTextBoxColumn AddTextBoxColumn(GridDataColumn dataColumn, int width)
        {
            return AddColumn(new DataGridViewTextBoxColumn(), dataColumn, width);
        }

        public DataGridViewCheckBoxColumn AddCheckBoxColumn(GridDataColumn dataColumn, int width)
        {
            return AddColumn(new DataGridViewCheckBoxColumn(), dataColumn, width);
        }

        public DataGridViewComboBoxColumn AddComboBoxColumn(GridDataColumn dataColumn, int width, string[] items)
        {
            var column = AddColumn(new DataGridViewComboBoxColumn(), dataColumn, width);
            column.Items.AddRange(items);
            column.FlatStyle = FlatStyle.Flat;
            return column;
        }

        public CC AddCustomColumn<CC>(CC customColumn, GridDataColumn dataColumn, int width) where CC : DataGridViewColumn, IGridCustomColumn
        {
            return this.AddColumn(customColumn, dataColumn, width);
        }

        private CL AddColumn<CL>(CL column, GridDataColumn dataColumn, int width) where CL : DataGridViewColumn
        {
            this.columnInfoList.Add(new ColumnInfo(column, dataColumn, width));
            return column;
        }

        public void ChangeColumnVisibility(GridDataColumn dataColumn, bool visible, bool init = false)
        {
            var columnInfo = columnInfoList.Where(i => i.DataColumn == dataColumn).FirstOrDefault();
            if(columnInfo == null)
            {
                return;
            }

            columnInfo.Visible = visible;
            if(init)
            {
                this.InitColumns();
            }
        }

        public void ShowColumn(GridDataColumn dataColumn)
        {
            this.ChangeColumnVisibility(dataColumn, visible: true);
        }

        public void HideColumn(GridDataColumn dataColumn)
        {
            this.ChangeColumnVisibility(dataColumn, visible: false);
        }

        public void InitColumns()
        {
            foreach (var column in this.DataGridView.Columns)
            {
                if (column is IGridCustomColumn customColumn)
                {
                    customColumn.UnregisterEvents(this.DataGridView);
                }
            }

            this.DataGridView.Columns.Clear();

            columnInfoList.Sort((one, two) => one.DataColumn.ColumnIndex.CompareTo(two.DataColumn.ColumnIndex));
            foreach (var columnInfo in columnInfoList)
            {
                var column = columnInfo.Column;
                if (columnInfo.Visible)
                {
                    if (column is IGridCustomColumn customColumn)
                    {
                        customColumn.RegisterEvents(this.DataGridView);
                    }

                    column.Visible = true;

                    column.Name = columnInfo.DataColumn.Name;
                    column.DisplayIndex = columnInfo.DataColumn.ColumnIndex;
                    column.DataPropertyName = columnInfo.DataColumn.DataPropertyName;
                    column.AutoSizeMode = columnInfo.Width <= 0 ? DataGridViewAutoSizeColumnMode.Fill : DataGridViewAutoSizeColumnMode.None;
                    column.Width = columnInfo.Width;
                    column.MinimumWidth = 15;
                    column.SortMode = DataGridViewColumnSortMode.Programmatic;

                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    column.HeaderCell.Style.Padding = new Padding(0);
                    column.HeaderCell.Style.WrapMode = DataGridViewTriState.True;

                    column.DefaultCellStyle.SelectionBackColor = Color.LightGray;
                    column.DefaultCellStyle.BackColor = SystemColors.ControlLightLight;
                    column.DefaultCellStyle.SelectionForeColor = Color.Black;
                    column.DefaultCellStyle.ForeColor = Color.Black;
                }
                else
                {
                    column.Visible = false;
                }

                this.DataGridView.Columns.Add(column);
            }
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            foreach (var column in this.DataGridView.Columns)
            {//This is required for some special actions! (Like arrows for Suggestions!)
                if (column is IGridCustomColumn customColumn && customColumn.ProcessCmdKey(ref msg, keyData))
                {
                    return true;
                }
            }

            return false;
        }

        public void DeleteSelectedCells()
        {
            var deletedCellList = new List<GridCellChange>();
            foreach (DataGridViewCell selectedCell in DataGridView.SelectedCells)
            {
                deletedCellList.Add(new GridCellChange(selectedCell) { NewValue = null }); //Set value to null so it will clear also checkboxes
            }

            this.ChangeCells(deletedCellList);
        }

        public void ChangeRow(int rowIndex, T data)
        {
            this.ChangeCells(this.DataHandler.CreateCellChanges(rowIndex, data));
        }

        public void ChangeMultipleRows(Dictionary<int, T> dataDict)
        {
            this.ChangeCells(this.DataHandler.CreateCellChanges(dataDict));
        }

        public void ChangeCell(GridCellChange cell, bool applyChanges = true)
        {
            ChangeCells(Utils.SingletonList(cell), applyChanges);
        }

        public void ChangeCells(List<GridCellChange>? cellChangeList, bool applyChanges = true)
        {
            if (cellChangeList == null || cellChangeList.Count == 0)
            {
                return;
            }

            this.DataGridView.SuspendLayout();

            try
            {
                if (applyChanges)
                {
                    this.undoRedoHandler.Lock();
                    foreach (var cellChange in cellChangeList) //Accessing DataSource instead of changing value of cell is WAAAAY faster (From 3s to 22ms)
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

                this.Events.CellChangeEvent(this.DataGridView, new() { CellChangeList = cellChangeList });
                undoRedoHandler.AddUndo(() => UndoChangeCells(cellChangeList));
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            this.DataGridView.Refresh();
            this.DataGridView.ResumeLayout();

            undoRedoHandler.Unlock(); //And a locked undoRedo
        }

        private void UndoChangeCells(List<GridCellChange> cellChangeList)
        {
            this.DataGridView.SuspendLayout();


            try
            {
                this.undoRedoHandler.Lock();
                foreach (var cellChange in cellChangeList) //Accessing DataSource instead of changing value of cell is WAAAAY faster (From 3s to 22ms)
                {
                    var data = this.DataSource[cellChange.RowIndex];
                    data.GetColumn(cellChange.ColumnIndex).SetValueTo(data, cellChange.OldValue);
                }
                this.undoRedoHandler.Unlock();

                var lastCellChange = cellChangeList[cellChangeList.Count - 1];
                SelectCell(lastCellChange);  //Setting se current cell already center the grid to it.

                this.Events.CellChangeEvent(this.DataGridView, new() { CellChangeList = cellChangeList, IsUndo = true });

                undoRedoHandler.AddRedo(() =>
                {
                    ChangeCells(cellChangeList);

                    var firstCellChange = cellChangeList[0];
                    SelectCell(firstCellChange);  //Setting se current cell already center the grid to it.
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

        public void AddData(IEnumerable<T> dataEnumerable)
        {
            var dataDict = new Dictionary<int, T>();

            var emptyIndexList = DataSource.GetFirstEmptyRowIndexes(dataEnumerable.Count());

            int i = 0;
            foreach (var data in dataEnumerable)
            {
                if (i >= emptyIndexList.Count)
                {
                    break;
                }

                var index = emptyIndexList[i++];
                dataDict.Compute(index, data);
            }
            this.ChangeMultipleRows(dataDict);
        }

        public void ResetSelectedCell()
        {
            this.SelectCell(cell: null);
        }

        private void SelectCell(GridCellChange cellChange)
        {
            this.SelectCell(cellChange.RowIndex, cellChange.ColumnIndex);
        }

        public void SelectCell(int rowIndex, int columnIndex)
        {
            this.SelectCell(this.DataGridView.Rows[rowIndex].Cells[columnIndex]);
        }

        private void SelectCell(DataGridViewCell? cell)
        {
            DataGridView.RefreshEdit(); //This is required to refresh checkbox otherwise, if the undo is in a selected cell, it will not update visually (DATA IS CHANGED!)
            DataGridView.Refresh();

            DataGridView.CurrentCell = cell; //Setting se current cell already center the grid to it.
            DataGridView.Refresh();
        }

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
