using static TiaXmlReader.Generation.GridHandler.GridExcelDragHandler;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;
using SimaticML;
using SimaticML.Enums;
using TiaXmlReader.Generation.IO;
using TiaUtilities.Generation.GridHandler.Events;

namespace TiaUtilities.Generation.IO.Module
{
    public static class IOGenUtils
    {
        public static void DragPreview<T>(GridExcelDragEventArgs eventArgs, GridHandler<T> gridHandler) where T : IGridData
        {
            var dataGridView = gridHandler.DataGridView;

            var startingCellValue = dataGridView.Rows[eventArgs.StartingRow]?.Cells[eventArgs.DraggedColumn].Value;
            if (eventArgs.DraggedColumn == IOData.ADDRESS)
            {
                var tagAddress = SimaticTagAddress.FromAddress(startingCellValue?.ToString());
                if (tagAddress != null)
                {
                    eventArgs.TooltipString = (eventArgs.DraggingDown
                                ? tagAddress.NextBit(SimaticDataType.BYTE, eventArgs.SelectedRowCount - 1)
                                : tagAddress.PreviousBit(SimaticDataType.BYTE, eventArgs.SelectedRowCount - 1)).GetAddress();
                }
            }
            else
            {
                GridUtils.DragPreview(eventArgs, gridHandler);
            }
        }

        public static void DragDone<T>(GridExcelDragEventArgs eventArgs, GridHandler<T> gridHandler) where T : IGridData
        {
            var dataGridView = gridHandler.DataGridView;
            if (eventArgs.DraggedColumn == IOData.ADDRESS)
            {
                var startString = dataGridView.Rows[eventArgs.StartingRow]?.Cells[eventArgs.DraggedColumn].Value?.ToString();
                if (string.IsNullOrEmpty(startString))
                {
                    return;
                }

                var tagAddress = SimaticTagAddress.FromAddress(startString);
                if (tagAddress == null)  //If is not a valid address, i won't care about doing any stuff.
                {
                    return;
                }

                var cellChangeList = new List<GridCellChange>();

                var rowIndexEnumeration = Enumerable.Range(eventArgs.TopSelectedRow, (int)eventArgs.SelectedRowCount);
                if (!eventArgs.DraggingDown)
                {
                    rowIndexEnumeration = rowIndexEnumeration.Reverse();
                }

                foreach (var rowIndex in rowIndexEnumeration)
                {
                    var cellChange = new GridCellChange(0, rowIndex) { NewValue = tagAddress.GetAddress() };
                    cellChangeList.Add(cellChange);

                    var _ = eventArgs.DraggingDown ? tagAddress.NextBit(SimaticDataType.BYTE) : tagAddress.PreviousBit(SimaticDataType.BYTE); //Increase at the end. The first value is valid!
                }

                gridHandler.ChangeCells(cellChangeList);
            }
            else
            {
                GridUtils.DragDone(eventArgs, gridHandler);
            }
        }
    }
}
