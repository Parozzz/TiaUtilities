using System;
using System.Collections.Generic;
using System.Linq;
using static TiaXmlReader.Generation.GridHandler.GridExcelDragHandler;
using TiaXmlReader.Utility;
using System.Windows.Forms;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;
using System.Text.RegularExpressions;

namespace TiaXmlReader.Generation.GridHandler
{
    public static class GridUtils
    {
        public static void DragPreview<C, T>(DragData data, GridHandler<C, T> gridHandler) where C : IGenerationConfiguration where T : IGridData<C>
        {
            var startCell = data.DataGridView.Rows[data.StartingRow]?.Cells[data.DraggedColumn];
            if (!(startCell is DataGridViewTextBoxCell))
            {
                return;
            }

            var startString = startCell.Value?.ToString();
            if (Utils.SplitStringFromNumberFromRight(startString, out string before, out string numString, out string after) && int.TryParse(numString, out int num))
            {
                var nextNum = num + (data.SelectedRowCount - 1) * (data.DraggingDown ? 1 : -1);

                var nextNumString = nextNum.ToString();
                if (numString.Length > nextNumString.Length)
                {
                    var nextNumLen = nextNumString.Length;
                    for (var x = 0; x < (numString.Length - nextNumLen); x++)
                    {
                        nextNumString = '0' + nextNumString;
                    }
                }
                data.TooltipString = before + nextNumString + after;
            }
            else
            {
                data.TooltipString = startString; //If it does not contains number, i simply copy the starting value!
            }
        }

        public static void DragMouseUp<C, T>(DragData data, GridHandler<C, T> gridHandler) where C : IGenerationConfiguration where T : IGridData<C>
        {
            var startCell = data.DataGridView.Rows[data.StartingRow]?.Cells[data.DraggedColumn];
            if (!(startCell is DataGridViewTextBoxCell))
            {
                return;
            }

            var rowIndexEnumeration = Enumerable.Range(data.TopSelectedRow, (int)data.SelectedRowCount);
            if (!data.DraggingDown)
            {
                rowIndexEnumeration = rowIndexEnumeration.Reverse();
            }

            var cellChangeList = new List<GridCellChange>();

            var startString = startCell.Value?.ToString();
            if (Utils.SplitStringFromNumberFromRight(startString, out string before, out string numString, out string after) && int.TryParse(numString, out int num))
            {
                var x = 0;
                foreach (var rowIndex in rowIndexEnumeration)
                {
                    var nextNum = num + (x++ * (data.DraggingDown ? 1 : -1));

                    var nextNumString = nextNum.ToString();
                    if (numString.Length > nextNumString.Length)
                    {
                        var nextNumLen = nextNumString.Length;
                        for (var z = 0; z < (numString.Length - nextNumLen); z++)
                        {
                            nextNumString = '0' + nextNumString;
                        }
                    }

                    var cellChange = new GridCellChange(data.DraggedColumn, rowIndex) { NewValue = (before + nextNumString + after) };
                    cellChangeList.Add(cellChange);
                }
            }
            else
            {
                foreach (var rowIndex in rowIndexEnumeration)
                {
                    var cellChange = new GridCellChange(data.DraggedColumn, rowIndex) { NewValue = startString };
                    cellChangeList.Add(cellChange);
                }
            }

            gridHandler.ChangeCells(cellChangeList);
        }

        public static void CopyAsExcel(DataGridView dataGridView)
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
            foreach (var cell in selectedCellList.Where(c => dataGridView.Columns[c.ColumnIndex].Visible))
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

            Clipboard.SetText(clipboardText, TextDataFormat.UnicodeText);
        }

        public static List<GridCellChange> PasteAsExcel(DataGridView dataGridView)
        {
            var clipboardDataObject = (DataObject)Clipboard.GetDataObject();
            if (!clipboardDataObject.GetDataPresent(DataFormats.Text))
            {
                return null;
            }

            var pasteString = clipboardDataObject.GetData(DataFormats.Text).ToString();

            bool containsExcelChars = pasteString.Contains("\r\n") || pasteString.Contains('\t');
            if (!containsExcelChars)
            {//If is a normal string, i will paste in ALL the selected cells!
                return dataGridView.SelectedCells
                    .Cast<DataGridViewCell>()
                    .Select(c => new GridCellChange(c) { NewValue = pasteString })
                    .ToList();
            }

            var pastedCellList = new List<GridCellChange>();

            //If contains new lines or tab it needs to handled like an excel file. New line => next row. Tab => next column.
            int startRowIndex = dataGridView.CurrentCell.RowIndex; //The currentCell row index needs to be taken BEFORE adding cells otherwise it will be moved!
            int startColumnIndex = dataGridView.CurrentCell.ColumnIndex;

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
                        pastedCellList.Add(new GridCellChange(cell) { NewValue = pastedValue });
                    }

                    columnCounter++;
                }

                rowIndex++;
                if (rowIndex >= rowCount)
                {
                    break;
                }
            }

            return pastedCellList;
        }
    }
}
