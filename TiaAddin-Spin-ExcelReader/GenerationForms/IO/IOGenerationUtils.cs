using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.SimaticML;
using static TiaXmlReader.GenerationForms.GridHandler.GridExcelDragHandler;
using TiaXmlReader.GenerationForms.GridHandler;
using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.Utility;

namespace TiaXmlReader.GenerationForms.IO
{
    public static class IOGenerationUtils
    {
        public static void DragPreview<T>(DragData data, GridHandler<T> gridHandler) where T : IGridData
        {
            var dataGridView = data.DataGridView;

            var startingCellValue = dataGridView.Rows[data.StartingRow]?.Cells[data.DraggedColumn].Value;
            if (data.DraggedColumn == IOGenerationForm.ADDRESS_COLUMN)
            {
                var tagAddress = SimaticTagAddress.FromAddress(startingCellValue?.ToString());
                if (tagAddress != null)
                {
                    data.TooltipString = (data.DraggingDown
                                ? tagAddress.NextBit(SimaticDataType.BYTE, data.SelectedRowCount - 1)
                                : tagAddress.PreviousBit(SimaticDataType.BYTE, data.SelectedRowCount - 1)).GetAddress();
                }
            }
            else
            {
                var startString = startingCellValue?.ToString();
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
            }
        }

        public static void DragMouseUp<T>(DragData data, GridHandler<T> gridHandler) where T : IGridData
        {
            var dataGridView = data.DataGridView;

            var cellChangeList = new List<GridCellChange>();

            var rowIndexEnumeration = Enumerable.Range(data.TopSelectedRow, (int)data.SelectedRowCount);
            if (!data.DraggingDown)
            {
                rowIndexEnumeration = rowIndexEnumeration.Reverse();
            }

            var startingCellValue = dataGridView.Rows[data.StartingRow]?.Cells[data.DraggedColumn].Value;
            if (data.DraggedColumn == IOGenerationForm.ADDRESS_COLUMN)
            {
                var tagAddress = SimaticTagAddress.FromAddress(startingCellValue?.ToString());
                if (tagAddress == null)  //If is not a valid address, i won't care about doing any stuff.
                {
                    return;
                }

                foreach (var rowIndex in rowIndexEnumeration)
                {
                    var cellChange = new GridCellChange(dataGridView, 0, rowIndex) { NewValue = tagAddress.GetAddress() };
                    cellChangeList.Add(cellChange);

                    var _ = data.DraggingDown ? tagAddress.NextBit(SimaticDataType.BYTE) : tagAddress.PreviousBit(SimaticDataType.BYTE); //Increase at the end. The first value is valid!
                }
            }
            else
            {
                var startString = startingCellValue?.ToString();
                if (!(Utils.SplitStringFromNumberFromRight(startString, out string before, out string numString, out string after) && int.TryParse(numString, out int num)))
                {
                    return;
                }

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

                    var cellChange = new GridCellChange(dataGridView, data.DraggedColumn, rowIndex) { NewValue = (before + nextNumString + after) };
                    cellChangeList.Add(cellChange);
                }
            }

            gridHandler.ChangeCells(cellChangeList);
        }
    }
}
