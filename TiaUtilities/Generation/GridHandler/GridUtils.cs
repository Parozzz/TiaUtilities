using InfoBox;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.GridHandler
{
    public static class GridUtils
    {
        public static void CopyGridDataValues(GridData copyFrom, GridData moveTo)
        {
            if (copyFrom.GetType() == moveTo.GetType())
            {
                var columns = copyFrom.GetColumns();

                foreach (var dataColumn in columns)
                {
                    var copeFromValue = dataColumn.GetValueFrom<object>(copyFrom);
                    dataColumn.SetValueTo(moveTo, copeFromValue);
                }
            }

        }

        public static void DragPreview<T>(GridExcelDragEventArgs eventArgs, GridHandler<T> gridHandler) where T : GridData
        {
            var gridColumn = gridHandler.DataSource[eventArgs.StartingRow].GetColumn(eventArgs.DraggedColumn);
            if (gridColumn.PropertyInfo.PropertyType != typeof(string))
            {
                return;
            }

            var startString = "" + gridHandler.DataSource[eventArgs.StartingRow][eventArgs.DraggedColumn];
            if (Utils.SplitStringFromNumberFromRight(startString, out string before, out string numString, out string after) && int.TryParse(numString, out int num))
            {
                var nextNum = num + (eventArgs.SelectedRowCount - 1) * (eventArgs.DraggingDown ? 1 : -1);

                var nextNumString = nextNum.ToString();
                if (numString.Length > nextNumString.Length)
                {
                    var nextNumLen = nextNumString.Length;
                    for (var x = 0; x < (numString.Length - nextNumLen); x++)
                    {
                        nextNumString = '0' + nextNumString;
                    }
                }
                eventArgs.TooltipString = before + nextNumString + after;
            }
            else
            {
                eventArgs.TooltipString = startString; //If it does not contains number, i simply copy the starting value!
            }
        }

        public static void DragDone<T>(GridExcelDragEventArgs eventArgs, GridHandler<T> gridHandler) where T : GridData
        {
            var gridColumn = gridHandler.DataSource[eventArgs.StartingRow].GetColumn(eventArgs.DraggedColumn);
            if (gridColumn.PropertyInfo.PropertyType != typeof(string))
            {
                return;
            }

            var rowIndexEnumeration = Enumerable.Range(eventArgs.TopSelectedRow, (int)eventArgs.SelectedRowCount);
            if (!eventArgs.DraggingDown)
            {
                rowIndexEnumeration = rowIndexEnumeration.Reverse();
            }

            var req = gridHandler.DataChangedHandler.Join();

            var startString = "" + gridHandler.DataSource[eventArgs.StartingRow][eventArgs.DraggedColumn];
            if (Utils.SplitStringFromNumberFromRight(startString, out string before, out string numString, out string after) && int.TryParse(numString, out int num))
            {
                var x = 0;
                foreach (var rowIndex in rowIndexEnumeration)
                {
                    var nextNum = num + (x++ * (eventArgs.DraggingDown ? 1 : -1));

                    var nextNumString = nextNum.ToString();
                    if (numString.Length > nextNumString.Length)
                    {
                        var nextNumLen = nextNumString.Length;
                        for (var z = 0; z < (numString.Length - nextNumLen); z++)
                        {
                            nextNumString = '0' + nextNumString;
                        }
                    }

                    var newValue = (before + nextNumString + after);
                    gridHandler.DataSource[rowIndex][eventArgs.DraggedColumn] = newValue;
                }
            }
            else
            {
                foreach (var rowIndex in rowIndexEnumeration)
                {
                    var newValue = startString;
                    gridHandler.DataSource[rowIndex][eventArgs.DraggedColumn] = newValue;
                }
            }

            gridHandler.DataChangedHandler.End(req);
        }

        public static string? GetCopyAsExcelText(DataGridView dataGridView)
        {
            try
            {
                var selectedCellList = dataGridView.SelectedCells.Cast<DataGridViewCell>().ToList();
                selectedCellList.Sort((a, b) =>
                {
                    if (a.RowIndex == b.RowIndex)
                    {
                        return a.ColumnIndex.CompareTo(b.ColumnIndex);
                    }

                    return a.RowIndex.CompareTo(b.RowIndex);
                }); //Sort the list so is ready to be added sequencially to the text! First by row. If row is same, then by column.

                var clipboardText = "";

                int startingRowIndex = -999;

                var visibleSelectedCells = selectedCellList.Where(c => dataGridView.Columns[c.ColumnIndex].Visible);
                foreach (var cell in visibleSelectedCells)
                {
                    if (startingRowIndex == -999)
                    {
                        startingRowIndex = cell.RowIndex;
                    }
                    else if (startingRowIndex != cell.RowIndex)
                    {
                        clipboardText += "\r\n";
                        startingRowIndex = cell.RowIndex;
                    }
                    else
                    {
                        clipboardText += '\t';
                    }

                    var stringValue = cell.Value == null ? "" : cell.Value.ToString();
                    clipboardText += stringValue;
                }

                if (visibleSelectedCells.Count() > 1)
                {
                    if (clipboardText.Contains('\t')) //If is a multiple column copy, there needs to be a tab at the end required by some software
                    {
                        clipboardText += '\t';
                    }

                    clipboardText += "\r\n"; //Add since some software requires it for multiple rows.
                }

                return clipboardText;
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            return null;
        }

        public static void CopyAsExcelToClipboard(DataGridView dataGridView)
        {
            try
            {
                var clipboardText = GridUtils.GetCopyAsExcelText(dataGridView);

                //The clipboard cannot have an empty string as text
                if (string.IsNullOrEmpty(clipboardText))
                {
                    Clipboard.Clear();
                }
                else
                {
                    Clipboard.SetText(clipboardText, TextDataFormat.UnicodeText);
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

        }

        public static void PasteAsExcel(DataGridView dataGridView, string pasteString, int row, int column, bool ignoreSingleLineMultiplePasting = false)
        {
            try
            {
                if (!ignoreSingleLineMultiplePasting)
                {
                    var returnCount = pasteString.Count(c => c == '\t' || c == '\n');
                    //If the text has only one return or tab AND it does not end with a special char, i will treat it as a single line/column
                    if (returnCount == 0 || (returnCount == 1 && (pasteString.EndsWith('\t') || pasteString.EndsWith('\n'))))
                    {//If is a normal string, i will paste in ALL the selected cells!
                        var strippedPasteString = pasteString.Replace("\t", "").Replace("\r", "").Replace("\n", "");

                        foreach (DataGridViewCell cell in dataGridView.SelectedCells)
                        {
                            cell.Value = strippedPasteString;
                        }

                        return;
                    }
                }

                //If contains new lines or tab it needs to handled like an excel file. New line => next row. Tab => next column.
                int startRowIndex = row; //The currentCell row index needs to be taken BEFORE adding cells otherwise it will be moved!
                int startColumnIndex = column;

                var validColumnIndexes = dataGridView.Columns.Cast<DataGridViewColumn>()
                    .Where(x => x.Visible)
                    .Select(x => x.Index)
                    .Where(x => x >= startColumnIndex)
                    .ToArray();

                var rowCount = dataGridView.RowCount;
                var columnCount = validColumnIndexes.Length;

                string[] pastedRowArray = Regex.Split(pasteString.TrimEnd("\r\n".ToCharArray()), "\r\n");

                var rowIndex = startRowIndex;
                foreach (var pastedRow in pastedRowArray)
                {
                    var pastedValueArray = pastedRow.Split('\t');

                    var columnCounter = 0;
                    for (int i = 0; i < pastedValueArray.Length && columnCounter < columnCount; i++)
                    {
                        var pastedValue = pastedValueArray[i];
                        var columnIndex = validColumnIndexes[columnCounter];

                        var cell = dataGridView.Rows[rowIndex]?.Cells[columnIndex];
                        if (cell != null)
                        {
                            cell.Value = pastedValue;
                        }

                        columnCounter++;
                    }

                    rowIndex++;
                    if (rowIndex >= rowCount)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }
        }

        public static void PasteAsExcelFromClipboard(DataGridView dataGridView)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    var clipboardText = Clipboard.GetText();

                    var currentCell = dataGridView.CurrentCell;
                    GridUtils.PasteAsExcel(dataGridView, clipboardText, currentCell.RowIndex, currentCell.ColumnIndex);
                }
                else
                {
                    foreach (DataGridViewCell cell in dataGridView.SelectedCells)
                    {
                        cell.Value = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }
        }

        public static bool AreSelectedCellsPlanar(DataGridView dgv)
        {//Can you see this is vibe-coded?
         // 1. Prendi solo le celle effettivamente visibili (evita ghost selection su righe/colonne nascoste)
            var selectedCellsList = dgv.SelectedCells.Cast<DataGridViewCell>()
                .Where(c => c.Visible && c.OwningColumn.Visible && c.OwningRow.Visible)
                .ToList();

            if (!selectedCellsList.Any())
            {
                return false;
            }

            // 2. Identifica colonne e righe uniche coinvolte
            var distinctColumnsList = selectedCellsList.Select(c => c.OwningColumn).Distinct().ToList();
            var distinctRowsList = selectedCellsList.Select(c => c.OwningRow).Distinct().ToList();

            // --- CONTROLLO COLONNE CONSECUTIVE ---
            var columnsMapList = dgv.Columns.Cast<DataGridViewColumn>()
                .Where(c => c.Visible)
                .OrderBy(c => c.DisplayIndex)
                .ToList();

            var columnsIndexList = distinctColumnsList
                .Select(c => columnsMapList.IndexOf(c))
                .OrderBy(i => i)
                .ToList();

            bool columnsOK = (columnsIndexList.Last() - columnsIndexList.First()) == (distinctColumnsList.Count - 1);

            if (!columnsOK)
            {
                return false;
            }

            // --- CONTROLLO RIGHE CONSECUTIVE ---
            var rowsMapList = dgv.Rows.Cast<DataGridViewRow>()
                .Where(r => r.Visible)
                .OrderBy(r => r.Index)
                .ToList();

            var rowsIndexList = distinctRowsList
                .Select(r => rowsMapList.IndexOf(r))
                .OrderBy(i => i)
                .ToList();

            bool rowsOK = (rowsIndexList.Last() - rowsIndexList.First()) == (distinctRowsList.Count - 1);
            if (!rowsOK)
            {
                return false;
            }

            // --- CONTROLLO AREA (Il "Rettangolo Pieno") ---
            // Se è un rettangolo senza buchi, il numero di celle selezionate 
            // deve corrispondere esattamente all'area (Base x Altezza)
            int gridCalculatedArea = distinctColumnsList.Count * distinctRowsList.Count;
            return selectedCellsList.Count == gridCalculatedArea;
        }

        public class SelectedCellsBorderCoordinates
        {
            public required int Top { get; init; } //Lowest Row Index
            public required int Bottom { get; init; } //Highest Row Index
            public required int Left { get; init; } //Lowest Column Index
            public required int Right { get; init; } //Highest Column Index

            public bool IsTop(DataGridViewCell cell) => cell.RowIndex == this.Top;
            public bool IsBottom(DataGridViewCell cell) => cell.RowIndex == this.Bottom;
            public bool IsLeft(DataGridViewCell cell) => cell.ColumnIndex == this.Left;
            public bool IsRight(DataGridViewCell cell) => cell.ColumnIndex == this.Right;
        }

        public static SelectedCellsBorderCoordinates GetSelectedCellsBorderCoordinates(DataGridView dataGridView)
        {
            int lowestRowIndex = -1;
            int highestRowIndex = -1;

            int lowestColumnIndex = -1;
            int highestColumnIndex = -1;

            foreach (DataGridViewCell cell in dataGridView.SelectedCells)
            {
                lowestRowIndex = (lowestRowIndex == -1 || cell.RowIndex < lowestRowIndex) ? cell.RowIndex : lowestRowIndex;
                highestRowIndex = (highestRowIndex == -1 || cell.RowIndex > highestRowIndex) ? cell.RowIndex : highestRowIndex;

                lowestColumnIndex = (lowestColumnIndex == -1 || cell.ColumnIndex < lowestColumnIndex) ? cell.ColumnIndex : lowestColumnIndex;
                highestColumnIndex = (highestColumnIndex == -1 || cell.ColumnIndex > highestColumnIndex) ? cell.ColumnIndex : highestColumnIndex;
            }

            return new() { Top = lowestRowIndex, Bottom = highestRowIndex, Left = lowestColumnIndex, Right = highestColumnIndex };
            //return [lowestRowIndex, highestRowIndex, lowestColumnIndex, highestColumnIndex]; //top, bottom, left, right
        }

        public static void PaintCellBorder<T>(GridHandler<T> gridHandler, Graphics graphics, SelectedCellsBorderCoordinates borders, DataGridViewCell cell, Color color, float width) where T : GridData
        {
            var cellBounds = gridHandler.InternalDataGridView.GetCellDisplayRectangle(cell.ColumnIndex, cell.RowIndex, false);

            var cellTag = gridHandler.GetCellTag(cell);

            RectangleF topBorderRect = RectangleF.Empty;
            RectangleF bottomBorderRect = RectangleF.Empty;
            RectangleF leftBorderRect = RectangleF.Empty;
            RectangleF rightBorderRect = RectangleF.Empty;

            using var brush = new SolidBrush(color);

            var top = borders.IsTop(cell);
            var bottom = borders.IsBottom(cell);
            var left = borders.IsLeft(cell);
            var right = borders.IsRight(cell);

            cellTag.HasBorder = top | bottom | left | right;

            if (top)
            {
                topBorderRect = new(cellBounds.Left - 1, cellBounds.Top, cellBounds.Right - cellBounds.Left - 1, width);
                graphics.FillRectangle(brush, topBorderRect);
            }

            if (bottom)
            {
                bottomBorderRect = new(cellBounds.Left - 1, cellBounds.Bottom - width - 1, cellBounds.Right - cellBounds.Left - 1, width);
                graphics.FillRectangle(brush, bottomBorderRect);
            }

            if (left)
            {
                leftBorderRect = new(cellBounds.Left, cellBounds.Top + 1, width, cellBounds.Bottom - cellBounds.Top - 1);
                graphics.FillRectangle(brush, leftBorderRect);
            }

            if (right)
            {
                rightBorderRect = new(cellBounds.Right - width - 1, cellBounds.Top - 1, width, cellBounds.Bottom - cellBounds.Top - 1);
                graphics.FillRectangle(brush, rightBorderRect);
            }

            cellTag.BordersWidth = width;
            cellTag.Borders = [topBorderRect, bottomBorderRect, leftBorderRect, rightBorderRect];
        }
    }
}
