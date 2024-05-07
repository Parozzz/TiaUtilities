using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TiaXmlReader.UndoRedo;
using static TiaXmlReader.Generation.GridHandler.GridExcelDragHandler;
using TiaXmlReader.Utility;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;
using TiaXmlReader.Javascript;
using TiaXmlReader.Generation.GridHandler.CustomColumns;
using TiaXmlReader.Utility.Extensions;
using System.Diagnostics;

namespace TiaXmlReader.Generation.GridHandler
{
    public class GridHandler<C, T> where C : IGenerationConfiguration where T : IGridData<C>
    {
        public class ColumnInfo
        {
            public DataGridViewColumn Column { get; internal set; }
            public GridDataColumn DataColumn { get; internal set; }
            public int Width { get; set; }
            public bool Visible { get; set; } = true;
        }

        private readonly GridSettings settings;
        private readonly C configuration;
        private readonly UndoRedoHandler undoRedoHandler;
        private readonly GridExcelDragHandler excelDragHandler;
        private readonly GridSortHandler<C, T> sortHandler;

        public DataGridView DataGridView { get; private set; }
        public GridDataHandler<C, T> DataHandler { get; private set; }
        public GridDataSource<C, T> DataSource { get; private set; }
        public GridTableScript<C, T> TableScript { get; private set; }

        private readonly List<ColumnInfo> columnInfoList;
        private readonly List<IGridCellPainter> cellPainterList;

        private bool init;

        public uint RowCount { get; set; } = 9;
        public bool AddRowIndexToRowHeader { get; set; } = true;
        public bool EnablePasteFromExcel { get; set; } = true;
        public bool EnableRowSelectionFromRowHeaderClick { get; set; } = true;
        public bool ShowJSContextMenuTopLeft { get; set; } = true;

        public GridHandler(JavascriptErrorReportThread jsErrorThread, GridSettings settings, C configuration, IGridRowComparer<C, T> comparer = null)
        {
            this.DataGridView = new MyGrid();

            this.settings = settings;
            this.configuration = configuration;
            this.undoRedoHandler = new UndoRedoHandler();
            this.excelDragHandler = new GridExcelDragHandler(this.DataGridView, settings);

            this.DataHandler = new GridDataHandler<C, T>(this.DataGridView);
            this.DataSource = new GridDataSource<C, T>(this.DataGridView, this.DataHandler);
            this.sortHandler = new GridSortHandler<C, T>(this.DataGridView, this.DataSource, this.undoRedoHandler, comparer);
            this.TableScript = new GridTableScript<C, T>(this, jsErrorThread);

            this.columnInfoList = new List<ColumnInfo>();
            this.cellPainterList = new List<IGridCellPainter>();
        }

        public void AddCellPainter(IGridCellPainter cellPainter)
        {
            cellPainterList.Add(cellPainter);
        }

        public void SetDragPreviewAction(Action<DragData> action)
        {
            this.excelDragHandler.SetPreviewAction(action);
        }

        public void SetDragMouseUpAction(Action<DragData> action)
        {
            this.excelDragHandler.SetMouseUpAction(action);
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

            #region KeyDown - Paste/Delete/Undo/Redo
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
                    case Keys.Delete:
                        this.DeleteSelectedCells();
                        break;
                    case Keys.Escape:
                        this.ShowCell(null);
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
            GridCellChange textBoxCellEdit = null;
            DataGridView.CellBeginEdit += (sender, args) =>
            {
                textBoxCellEdit = null;

                var cell = this.DataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex];
                if (cell is DataGridViewTextBoxCell || cell is DataGridViewComboBoxCell)
                {
                    textBoxCellEdit = new GridCellChange(cell);
                }
            };

            DataGridView.CellEndEdit += (sender, args) =>
            {
                if (textBoxCellEdit?.cell == null)
                {
                    return;
                }

                var cell = this.DataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex];
                if (cell == textBoxCellEdit.cell)
                {
                    textBoxCellEdit.NewValue = DataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex].Value;
                    ChangeCell(textBoxCellEdit, applyChanges: false);

                    textBoxCellEdit = null;
                }
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
                }
            };
            #endregion

            #region SHOW_JS_CONTEXT_MENU
            this.DataGridView.CellMouseClick += (sender, args) =>
            {
                if (args.RowIndex == -1 && args.ColumnIndex == -1 && args.Button == MouseButtons.Right && this.TableScript.Valid)
                {
                    var menuItem = new MenuItem();
                    menuItem.Text = "Execute Javascript";
                    menuItem.Click += (s, a) => this.TableScript.ShowConfigForm(this.DataGridView);

                    var contextMenu = new ContextMenu();
                    contextMenu.MenuItems.Add(menuItem);

                    contextMenu.Show(this.DataGridView, this.DataGridView.PointToClient(Cursor.Position));
                }
            };
            #endregion

            this.excelDragHandler.Init();
            this.sortHandler.Init();

            this.DataGridView.ResumeLayout();

            init = true;
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
            this.columnInfoList.Add(new ColumnInfo()
            {
                Column = column,
                DataColumn = dataColumn,
                Width = width
            });
            return column;
        }

        public ColumnInfo GetColumnInfo(GridDataColumn dataColumn)
        {
            return columnInfoList.Where(i => i.DataColumn == dataColumn).FirstOrDefault();
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

        public void ChangeCells(List<GridCellChange> cellChangeList, bool applyChanges = true)
        {
            if (cellChangeList == null || cellChangeList.Count == 0)
            {
                return;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            this.DataGridView.SuspendLayout();
            
            try
            {
                if (applyChanges)
                {
                    this.undoRedoHandler.Lock();
                    foreach (var cellChange in cellChangeList)
                    {//Accessing DataSource instead of changing value of cell is WAAAAY faster (From 3s to 22ms)
                        var data = this.DataSource[cellChange.RowIndex];
                        data.GetColumn(cellChange.ColumnIndex).SetValueTo(data, cellChange.NewValue);
                    }
                    this.undoRedoHandler.Unlock();
                }

                undoRedoHandler.AddUndo(() => UndoChangeCells(cellChangeList));
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            this.DataGridView.Refresh();
            this.DataGridView.ResumeLayout();
            undoRedoHandler.Unlock(); //And a locked undoRedo

            stopwatch.Stop();
            Console.WriteLine("Stopwatch [ms]: " + stopwatch.ElapsedMilliseconds);
        }

        private void UndoChangeCells(List<GridCellChange> cellChangeList)
        {
            this.DataGridView.SuspendLayout();
            

            try
            {
                this.undoRedoHandler.Lock();
                foreach (var cellChange in cellChangeList)
                {//Accessing DataSource instead of changing value of cell is WAAAAY faster (From 3s to 22ms)
                    var data = this.DataSource[cellChange.RowIndex];
                    data.GetColumn(cellChange.ColumnIndex).SetValueTo(data, cellChange.OldValue);
                }
                this.undoRedoHandler.Unlock();

                ShowCell(cellChangeList[cellChangeList.Count - 1].cell);  //Setting se current cell already center the grid to it.
                undoRedoHandler.AddRedo(() =>
                {
                    ChangeCells(cellChangeList);
                    ShowCell(cellChangeList[0].cell);  //Setting se current cell already center the grid to it.
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

        private void ShowCell(DataGridViewCell cell)
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
