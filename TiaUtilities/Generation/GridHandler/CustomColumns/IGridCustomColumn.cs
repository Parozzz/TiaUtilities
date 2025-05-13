namespace TiaXmlReader.Generation.GridHandler.CustomColumns
{
    public interface IGridCustomColumn
    {
        void RegisterEvents(DataGridView dataGridView);

        void UnregisterEvents(DataGridView dataGridView);

        bool ProcessCmdKey(ref Message msg, Keys keyData);
    }
}
