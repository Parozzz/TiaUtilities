using System.ComponentModel;
using System.Reflection;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Utility.Collections;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridDataSource<T> : IGridDataSource, ISaveable<Dictionary<int, T>> where T : IGridRowData
    {
        public class RowEventArgs(T item, int index) : EventArgs
        {
            public T RowData { get; init; } = item;
            public int RowIndex { get; init; } = index;
        }

        public delegate void RowEvent(GridDataSource<T> source, RowEventArgs args);

        public class RowValueEventArgs(T data, int index) : EventArgs
        {
            public T RowData { get; init; } = data;
            public int RowIndex { get; init; } = index;
            public required string PropertyName { get; init; }
            public required GridDataColumn Column { get; init; }
            public required object? OldValue { get; init; }
            public required object? NewValue { get; init; }
        }

        public delegate void RowValueEvent(GridDataSource<T> source, RowValueEventArgs args);

        public event RowEvent Added = delegate { };
        public event RowEvent Changed = delegate { };

        public event RowValueEvent ValueChanged = delegate { };

        public IReadOnlyList<GridDataColumn> DataColumns { get; init; }

        public int Count => this.dataList.Count;

        private readonly ExcelLikeDataGridView dataGridView;
        private readonly GridDataChangedHandler dataChangedHandler;

        private readonly List<T> dataList;

        private readonly GridDataPropertyChangedEvent dataPropertyChanged;

        public GridDataSource(ExcelLikeDataGridView dataGridView, GridDataChangedHandler dataChangedHandler)
        {
            this.DataColumns = GridDataSource<T>.ValidateColumnList();

            this.dataGridView = dataGridView;
            this.dataChangedHandler = dataChangedHandler;

            this.dataList = [];

            this.dataPropertyChanged = (sender, args) =>
            {
                var data = args.Data;
                if (data is T t)
                {
                    var row = this.dataList.IndexOf(t);
                    if (row >= 0)
                    {
                        this.dataChangedHandler.HandleCellChangeEvent(args, row);
                        ValueChanged(this, new(t, row)
                        {
                            PropertyName = args.PropertyName,
                            Column = args.Column,
                            OldValue = args.OldValue,
                            NewValue = args.NewValue
                        });
                    }
                }
            };
        }

        private static IReadOnlyList<GridDataColumn> ValidateColumnList()
        {
            var type = typeof(T);
            var fieldInfo = type.GetField("COLUMN_LIST", BindingFlags.Static | BindingFlags.Public);
            if (fieldInfo == null)
            {
                throw new MissingFieldException($"IGridData must have a public static IReadOnlyList<GridDataColumn> COLUMN_LIST field for {type.Name}");
            }

            if (fieldInfo.FieldType != typeof(IReadOnlyList<GridDataColumn>))
            {
                throw new MissingFieldException($"IGridData must have a public static IReadOnlyList<GridDataColumn> COLUMN_LIST field for {type.Name}");
            }

            return (IReadOnlyList<GridDataColumn>)fieldInfo.GetValue(null);
        }

        public T CreateInstance()
        {
            var type = typeof(T);
            return (T)type.Assembly.CreateInstance(type.FullName);
        }

        public Dictionary<int, T> CreateSave()
        {
            Dictionary<int, T> saveDict = [];
            foreach (var entry in this.GetNotEmptyDataDict())
            {
                saveDict.Add(entry.Value, entry.Key);
            }
            return saveDict;
        }

        public void LoadSave(Dictionary<int, T> saveDict)
        {
            //Here DO NOT CLEAR the data. Seems like the system binds to the loaded data and changes are directly applied.
            //Only clearing it, it will not unbind from previous loaded data and will corrupt it.
            //this.Clear();
            this.InitializeData((uint)this.Count);

            foreach (var entry in saveDict)
            {
                var rowIndex = entry.Key;
                var data = entry.Value;
                if (rowIndex >= 0 && rowIndex <= this.Count)
                {
                    GridUtils.CopyGridDataValues(data, this[rowIndex]);
                }
            }
        }

        private int ValidateRowIndex(int index) => index >= 0 && index < this.Count ? index : throw new IndexOutOfRangeException($"Index {index} out of range for {this.GetType().FullName}.");

        public T this[int i]
        {
            get => this.dataList[this.ValidateRowIndex(i)];
            set
            {
                var rowIndex = this.ValidateRowIndex(i);
                this.dataList[rowIndex] = value;

                this.Changed(this, new RowEventArgs(value, rowIndex));
            }
        }

        public void Clear()
        {
            foreach (var data in this.dataList)
            {
                data.Clear();
            }
            this.dataGridView.Refresh();
        }

        public void InitializeData(uint dataAmount)
        {
            foreach (var data in this.dataList)
            {
                data.Dispose();
            }

            this.dataList.Clear();
            this.dataList.AddRange(
                Enumerable.Range(0, (int)dataAmount).Select(rowIndex =>
                {
                    var data = this.CreateInstance();
                    data.DataPropertyChanged += this.dataPropertyChanged;

                    this.Added(this, new RowEventArgs(data, rowIndex));
                    return data;
                })
            );

            this.dataGridView.RowCount = this.dataList.Count;
        }

        public List<int> GetFirstEmptyRowIndexes(int num)
        {
            var emptyDataRowIndexList = new List<int>();
            if (num <= 0)
            {
                return emptyDataRowIndexList;
            }

            var emptyDataCounter = 0;
            for (int i = 0; i < this.dataList.Count; i++)
            {
                var data = this[i];
                if (data.IsEmpty())
                {
                    emptyDataRowIndexList.Add(i);
                    if (++emptyDataCounter >= num)
                    {
                        break;
                    }
                }
            }

            return emptyDataRowIndexList;
        }

        public void Sort(IComparer<T> comparer, SortOrder sortOrder)
        {
            if (sortOrder != SortOrder.None)
            {
                this.dataList.Sort(comparer);
                if (sortOrder == SortOrder.Descending)
                {
                    this.dataList.Reverse();
                }

                this.dataGridView.Refresh();
            }
        }

        public Dictionary<T, int> CreateIndexListSnapshot()
        {
            var dict = new Dictionary<T, int>();
            for (int x = 0; x < this.dataList.Count; x++)
            {
                var value = this.dataList[x];
                dict.Add(value, x);
            }
            return dict;
        }

        public void RestoreIndexListSnapshot(Dictionary<T, int> dict)
        {
            this.dataList.Sort((x, y) =>
            {
                if (!dict.TryGetValue(x, out int xValue) || !dict.TryGetValue(y, out int yValue))
                {
                    return 0;
                }

                return xValue.CompareTo(yValue);
            });

            this.dataGridView.Refresh();
        }

        public int GetFirstNotEmptyIndexStartingFrom(int indexStart)
        {
            if (indexStart < 0 || indexStart > this.Count)
            {
                return -1;
            }

            for (var x = indexStart; x < this.dataList.Count; x++)
            {
                var data = this.dataList[x];
                if (!data.IsEmpty())
                {
                    return x;
                }
            }

            return -1;
        }

        public Dictionary<T, int> GetNotEmptyDataDict(int startRow = 0)
        {
            Dictionary<T, int> dict = [];
            if (startRow >= this.dataList.Count)
            {
                return dict;
            }

            for (var x = startRow; x < this.dataList.Count; x++)
            {
                var data = this.dataList[x];
                if (!data.IsEmpty())
                {
                    dict.Add(data, x);
                }
            }
            return dict;
        }

        public IEnumerable<T> GetNotEmptyData(int startRow = 0) => GetNotEmptyDataDict(startRow).Keys;

        public ICollection<int> GetNotEmptyIndexes(int startRow = 0) => GetNotEmptyDataDict(startRow).Values;

        public Dictionary<T, int> GetNotEmptyClonedDataDict()
        {
            var notEmptyDict = new Dictionary<T, int>();

            for (var x = 0; x < this.dataList.Count; x++)
            {
                var data = this.dataList[x];
                if (!data.IsEmpty())
                {
                    var dataClone = this.CreateInstance();
                    GridUtils.CopyGridDataValues(data, dataClone);
                    notEmptyDict.Add(dataClone, x);
                }
            }
            return notEmptyDict;
        }

        public IEnumerable<T> GetNotEmptyClonedData()
        {
            return GetNotEmptyClonedDataDict().Keys;
        }

        public IGridRowData GetGeneric(int index) => this[index];

        Dictionary<IGridRowData, int> IGridDataSource.GetGenericNotEmptyDataDict(int startRow)
        {
            if (startRow >= this.dataList.Count)
            {
                return [];
            }

            Dictionary<IGridRowData, int> dict = [];
            for (var x = startRow; x < this.dataList.Count; x++)
            {
                var data = this.dataList[x];
                if (!data.IsEmpty())
                {
                    dict.Add(data, x);
                }
            }
            return dict;
        }

        IEnumerable<IGridRowData> IGridDataSource.GetGenericNotEmptyData(int startRow) => ((IGridDataSource)this).GetGenericNotEmptyDataDict(startRow).Keys;

        Dictionary<IGridRowData, int> IGridDataSource.GetGenericNotEmptyClonedDataDict()
        {
            var notEmptyDict = new Dictionary<IGridRowData, int>();

            for (var x = 0; x < this.dataList.Count; x++)
            {
                var data = this.dataList[x];
                if (!data.IsEmpty())
                {
                    var dataClone = this.CreateInstance();
                    GridUtils.CopyGridDataValues(data, dataClone);
                    notEmptyDict.Add(dataClone, x);
                }
            }
            return notEmptyDict;
        }

        IEnumerable<IGridRowData> IGridDataSource.GetGenericNotEmptyClonedData() => ((IGridDataSource)this).GetGenericNotEmptyClonedDataDict().Keys;

    }
}