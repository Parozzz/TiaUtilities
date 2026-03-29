namespace TiaUtilities.Generation.GridHandler.CustomColumns
{
    public delegate void DataGridViewDataGridViewEventableButtonColumnPressedEvent(object? sender, DataGridViewDataGridViewEventableButtonColumnPressedEventArgs args);
    public record DataGridViewDataGridViewEventableButtonColumnPressedEventArgs(object? Data, DataGridViewButtonCell Cell);

    public class DataGridViewEventableButtonColumn : DataGridViewButtonColumn
    {

        public event DataGridViewDataGridViewEventableButtonColumnPressedEvent ButtonPressed = delegate { };
        public event DataGridViewDataGridViewEventableButtonColumnPressedEvent ButtonDoublePressed = delegate { };

        public void ButtonPressedEvent(object? data, DataGridViewButtonCell cell)
        {
            ButtonPressed.Invoke(this.DataGridView, new(data, cell));
        }

        public void ButtonDoublePressedEvent(object? data, DataGridViewButtonCell cell)
        {
            ButtonDoublePressed.Invoke(this.DataGridView, new(data, cell));
        }
    }
}
