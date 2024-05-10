using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.SimaticML;
using static TiaXmlReader.Generation.GridHandler.GridExcelDragHandler;
using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.Utility;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;

namespace TiaXmlReader.Generation.IO.GenerationForm
{
    public static class IOGenerationUtils
    {
        public static void DragPreview<C, T>(DragData data, GridHandler<C, T> gridHandler) where C : IGenerationConfiguration where T : IGridData<C>
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
                GridUtils.DragPreview(data, gridHandler);
            }
        }

        public static void DragMouseUp<C, T>(DragData data, GridHandler<C, T> gridHandler) where C : IGenerationConfiguration where T : IGridData<C>
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
                    var cellChange = new GridCellChange(0, rowIndex) { NewValue = tagAddress.GetAddress() };
                    cellChangeList.Add(cellChange);

                    var _ = data.DraggingDown ? tagAddress.NextBit(SimaticDataType.BYTE) : tagAddress.PreviousBit(SimaticDataType.BYTE); //Increase at the end. The first value is valid!
                }

                gridHandler.ChangeCells(cellChangeList);
            }
            else
            {
                GridUtils.DragMouseUp(data, gridHandler);
            }

            
        }
    }
}
