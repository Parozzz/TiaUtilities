using System.ComponentModel;
using System.Runtime.CompilerServices;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.GridHandler.Data
{
    public interface IGridData : INotifyPropertyChanged
    {
        public event GridDataPropertyChangedEvent DataPropertyChanged;


        public void Set(object? newValue, [CallerMemberName] string propertyName = "");

        public object? Get([CallerMemberName] string key = "");
        public T? GetAs<T>([CallerMemberName] string key = "");

        public object? this[GridDataColumn c] { get; set; }

        public object? this[int c] { get; set; }

        public void Clear();

        public bool IsEmpty(); 
        
        public IReadOnlyList<GridDataColumn> GetColumns();

        public GridDataColumn GetColumn(int column);

        public void Dispose();
    }
}
