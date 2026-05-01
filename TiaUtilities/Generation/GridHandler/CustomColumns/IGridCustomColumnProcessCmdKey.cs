
namespace TiaUtilities.Generation.GridHandler.CustomColumns
{
    public interface IGridCustomColumnProcessCmdKey
    {
        public bool ProcessCmdKey(ref Message msg, Keys keyData);
    }
}
