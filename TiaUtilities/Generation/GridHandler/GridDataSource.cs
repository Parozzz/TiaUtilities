using System.ComponentModel;
using System.Reflection;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.UndoRedo;
using TiaUtilities.Utility.Collections;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridDataSource<T> : IGridDataSource, ISaveable<Dictionary<int, T>> where T : IGridData
    {
        public event ListChangedEventHandler ListChanged { add => this.bindingList.ListChanged += value; remove => this.bindingList.ListChanged -= value; }

        public IReadOnlyList<GridDataColumn> DataColumns { get; init; }

        public int Count { get => this.bindingList.Count; }

        private readonly ExcelLikeDataGridView dataGridView;
        private readonly GridDataChangedHandler dataChangedHandler;

        private readonly List<T> baseDataList;
        private readonly BindingListSmart<T> bindingList;

        private readonly GridDataPropertyChangedEvent dataPropertyChanged;

        public GridDataSource(ExcelLikeDataGridView dataGridView, GridDataChangedHandler dataChangedHandler)
        {
            this.DataColumns = GridDataSource<T>.ValidateColumnList();

            this.dataGridView = dataGridView;
            this.dataChangedHandler = dataChangedHandler;

            this.baseDataList = [];
            this.bindingList = new(this.baseDataList)
            {
                RaiseListChangedEvents = true,

            };

            this.dataPropertyChanged = (sender, args) =>
            {
                var data = args.Data;
                if (data is T t)
                {
                    var row = this.bindingList.IndexOf(t);
                    this.dataChangedHandler.HandleCellChangeEvent(args, row);
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

        public ReadOnlyBindingListWrapper<T> CreateDataListWrapper() => new(this.bindingList);

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
            get => this.bindingList[this.ValidateRowIndex(i)];
            set => this.bindingList[this.ValidateRowIndex(i)] = value;
        }

        public void Clear()
        {
            foreach (var data in this.bindingList)
            {
                data.Clear();
            }
        }

        public void InitializeData(uint dataAmount)
        {
            foreach (var data in this.bindingList)
            {
                data.Dispose();
            }

            this.bindingList.Clear();
            this.bindingList.AddRange(
                Enumerable.Range(0, (int)dataAmount).Select(r =>
                {
                    var data = this.CreateInstance();
                    data.DataPropertyChanged += this.dataPropertyChanged;
                    return data;
                })
            );

            this.dataGridView.RowCount = this.bindingList.Count;
        }

        public List<int> GetFirstEmptyRowIndexes(int num)
        {
            var emptyDataRowIndexList = new List<int>();
            if (num <= 0)
            {
                return emptyDataRowIndexList;
            }

            var emptyDataCounter = 0;
            for (int i = 0; i < this.bindingList.Count; i++)
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
                this.baseDataList.Sort(comparer);
                if (sortOrder == SortOrder.Descending)
                {
                    this.baseDataList.Reverse();
                }

                this.bindingList.ResetBindings();
                this.dataGridView.Refresh();
            }
        }

        public Dictionary<T, int> CreateIndexListSnapshot()
        {
            var dict = new Dictionary<T, int>();
            for (int x = 0; x < this.bindingList.Count; x++)
            {
                var value = this.bindingList[x];
                dict.Add(value, x);
            }
            return dict;
        }

        public void RestoreIndexListSnapshot(Dictionary<T, int> dict)
        {
            this.baseDataList.Sort((x, y) =>
            {
                if (!dict.TryGetValue(x, out int xValue) || !dict.TryGetValue(y, out int yValue))
                {
                    return 0;
                }

                return xValue.CompareTo(yValue);
            });

            this.bindingList.ResetBindings();
            this.dataGridView.Refresh();
        }

        public int GetFirstNotEmptyIndexStartingFrom(int indexStart)
        {
            if (indexStart < 0 || indexStart > this.Count)
            {
                return -1;
            }

            for (var x = indexStart; x < this.bindingList.Count; x++)
            {
                var data = this.bindingList[x];
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
            if (startRow >= this.bindingList.Count)
            {
                return dict;
            }

            for (var x = startRow; x < this.bindingList.Count; x++)
            {
                var data = this.bindingList[x];
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

            for (var x = 0; x < this.bindingList.Count; x++)
            {
                var data = this.bindingList[x];
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

        public IGridData GetGeneric(int index) => this[index];

        Dictionary<IGridData, int> IGridDataSource.GetGenericNotEmptyDataDict(int startRow)
        {
            if (startRow >= this.bindingList.Count)
            {
                return [];
            }

            Dictionary<IGridData, int> dict = [];
            for (var x = startRow; x < this.bindingList.Count; x++)
            {
                var data = this.bindingList[x];
                if (!data.IsEmpty())
                {
                    dict.Add(data, x);
                }
            }
            return dict;
        }

        IEnumerable<IGridData> IGridDataSource.GetGenericNotEmptyData(int startRow) => ((IGridDataSource)this).GetGenericNotEmptyDataDict(startRow).Keys;

        Dictionary<IGridData, int> IGridDataSource.GetGenericNotEmptyClonedDataDict()
        {
            var notEmptyDict = new Dictionary<IGridData, int>();

            for (var x = 0; x < this.bindingList.Count; x++)
            {
                var data = this.bindingList[x];
                if (!data.IsEmpty())
                {
                    var dataClone = this.CreateInstance();
                    GridUtils.CopyGridDataValues(data, dataClone);
                    notEmptyDict.Add(dataClone, x);
                }
            }
            return notEmptyDict;
        }

        IEnumerable<IGridData> IGridDataSource.GetGenericNotEmptyClonedData() => ((IGridDataSource)this).GetGenericNotEmptyClonedDataDict().Keys;

    }
}