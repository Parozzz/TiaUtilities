using System;
using System.Collections.Generic;
using System.Linq;
using static TiaXmlReader.Generation.GridHandler.GridExcelDragHandler;
using TiaXmlReader.Utility;
using System.Windows.Forms;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;

namespace TiaXmlReader.Generation.GridHandler
{
    public static class GridUtils
    {
        public static void DragPreview<C, T>(DragData data, GridHandler<C, T> gridHandler) where C : IGenerationConfiguration where T : IGridData<C>
        {
            var startCell = data.DataGridView.Rows[data.StartingRow]?.Cells[data.DraggedColumn];
            if(!(startCell is DataGridViewTextBoxCell))
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

                    var cellChange = new GridCellChange(data.DataGridView, data.DraggedColumn, rowIndex) { NewValue = (before + nextNumString + after) };
                    cellChangeList.Add(cellChange);
                }
            }
            else
            {
                foreach (var rowIndex in rowIndexEnumeration)
                {
                    var cellChange = new GridCellChange(data.DataGridView, data.DraggedColumn, rowIndex) { NewValue = startString };
                    cellChangeList.Add(cellChange);
                }
            }

            gridHandler.ChangeCells(cellChangeList);
        }


    }
}
