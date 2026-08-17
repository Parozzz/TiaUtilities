namespace TiaUtilities.Generation.GridHandler.CustomColumns.ButtonColumn
{
    public delegate void DataGridViewDataGridViewEventableButtonColumnPressedEvent(object? sender, DataGridViewDataGridViewEventableButtonColumnPressedEventArgs args);
    public record DataGridViewDataGridViewEventableButtonColumnPressedEventArgs(DataGridViewButtonCell Cell);

    public class DataGridViewEventableButtonColumn : DataGridViewButtonColumn
    {

        public event DataGridViewDataGridViewEventableButtonColumnPressedEvent ButtonPressed = delegate { };
        public event DataGridViewDataGridViewEventableButtonColumnPressedEvent ButtonDoublePressed = delegate { };

        public DataGridViewEventableButtonColumn()
        {
            CellTemplate = new DataGridViewEventableButtonCell();
        }

        public void ButtonPressedEvent(DataGridViewButtonCell cell)
        {
            ButtonPressed.Invoke(this.DataGridView, new(cell));
        }

        public void ButtonDoublePressedEvent(DataGridViewButtonCell cell)
        {
            ButtonDoublePressed.Invoke(this.DataGridView, new(cell));
        }
    }
}
