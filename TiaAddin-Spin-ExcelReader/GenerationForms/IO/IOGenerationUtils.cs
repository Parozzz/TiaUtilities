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
using TiaXmlReader.Generation.IO;

namespace TiaXmlReader.GenerationForms.IO
{
    public static class IOGenerationUtils
    {
        public static void DragPreview<T>(DragData data, GridHandler<T> gridHandler) where T : IGridData
        {
            var dataGridView = data.DataGridView;

            var startingCellValue = dataGridView.Rows[data.StartingRow]?.Cells[data.DraggedColumn].Value;
            if (data.DraggedColumn == IOData.ADDRESS)
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
                GenerationUtils.DragPreview(data, gridHandler);
            }
        }

        public static void DragMouseUp<T>(DragData data, GridHandler<T> gridHandler) where T : IGridData
        {
            var dataGridView = data.DataGridView;
            if (data.DraggedColumn == IOData.ADDRESS)
            {
                var startString = dataGridView.Rows[data.StartingRow]?.Cells[data.DraggedColumn].Value?.ToString();
                if(string.IsNullOrEmpty(startString))
                {
                    return;
                }

                var tagAddress = SimaticTagAddress.FromAddress(startString);
                if (tagAddress == null)  //If is not a valid address, i won't care about doing any stuff.
                {
                    return;
                }

                var cellChangeList = new List<GridCellChange>();

                var rowIndexEnumeration = Enumerable.Range(data.TopSelectedRow, (int)data.SelectedRowCount);
                if (!data.DraggingDown)
                {
                    rowIndexEnumeration = rowIndexEnumeration.Reverse();
                }

                foreach (var rowIndex in rowIndexEnumeration)
                {
                    var cellChange = new GridCellChange(dataGridView, 0, rowIndex) { NewValue = tagAddress.GetAddress() };
                    cellChangeList.Add(cellChange);

                    var _ = data.DraggingDown ? tagAddress.NextBit(SimaticDataType.BYTE) : tagAddress.PreviousBit(SimaticDataType.BYTE); //Increase at the end. The first value is valid!
                }

                gridHandler.ChangeCells(cellChangeList);
            }
            else
            {
                GenerationUtils.DragMouseUp(data, gridHandler);
            }

            
        }
    }
}
