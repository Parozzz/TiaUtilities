using System.ComponentModel;
using TiaUtilities;
using TiaUtilities.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridDataSource<T> : ISaveable<Dictionary<int, T>> where T : IGridData
    {
        private readonly DataGridView dataGridView;
        private readonly GridDataHandler<T> dataHandler;

        private readonly List<T> dataList;

        public int Count { get => dataList.Count; }

        public GridDataSource(DataGridView dataGridView, GridDataHandler<T> dataHandler)
        {
            this.dataGridView = dataGridView;
            this.dataHandler = dataHandler;

            this.dataList = [];
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
            this.InitializeData((uint) this.Count);
            
            foreach (var entry in saveDict)
            {
                var rowIndex = entry.Key;
                var data = entry.Value;
                if (rowIndex >= 0 && rowIndex <= this.Count)
                {
                    this.dataHandler.CopyValues(data, this[rowIndex]);
                }
            }
        }

        public T this[int i]
        {
            get { return dataList[i]; }
            set { dataList[i] = value; }
        }

        public void Clear()
        {
            foreach (var data in this.dataList)
            {
                data.Clear();
            }
        }

        public void InitializeData(uint dataAmount)
        {
            this.dataList.Clear();
            for (int i = 0; i < dataAmount; i++)
            {
                dataList.Add(dataHandler.CreateInstance());
            }

            this.dataGridView.DataSource = new BindingSource() { DataSource = new BindingList<T>(this.dataList) };
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
                dataList.Sort(comparer);
                if (sortOrder == SortOrder.Descending)
                {
                    dataList.Reverse();
                }

                dataGridView.Refresh();
            }
        }

        public Dictionary<T, int> CreateIndexListSnapshot()
        {
            var dict = new Dictionary<T, int>();
            for (int x = 0; x < dataList.Count; x++)
            {
                dict.Add(dataList[x], x);
            }
            return dict;
        }

        public void RestoreIndexListSnapshot(Dictionary<T, int> dict)
        {
            dataList.Sort((x, y) =>
            {
                if (!dict.TryGetValue(x, out int xValue) || !dict.TryGetValue(y, out int yValue))
                {
                    return 0;
                }

                return xValue.CompareTo(yValue);
            });
            dataGridView.Refresh();
        }

        public int GetFirstNotEmptyIndexStartingFrom(int indexStart)
        {
            if (indexStart < 0 || indexStart > this.Count)
            {
                return -1;
            }

            for (var x = indexStart; x < dataList.Count; x++)
            {
                var data = dataList[x];
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
            if(startRow >= dataList.Count)
            {
                return dict;
            }

            for (var x = startRow; x < dataList.Count; x++)
            {
                var data = dataList[x];
                if (!data.IsEmpty())
                {
                    dict.Add(data, x);
                }
            }
            return dict;
        }

        public IEnumerable<T> GetNotEmptyData(int startRow = 0)
        {
            return GetNotEmptyDataDict(startRow).Keys;
        }

        public ICollection<int> GetNotEmptyIndexes(int startRow = 0)
        {
            return GetNotEmptyDataDict(startRow).Values;
        }

        public Dictionary<T, int> GetNotEmptyClonedDataDict()
        {
            var notEmptyDict = new Dictionary<T, int>();

            for (var x = 0; x < dataList.Count; x++)
            {
                var data = dataList[x];
                if (!data.IsEmpty())
                {
                    var dataClone = dataHandler.CreateInstance();
                    dataHandler.CopyValues(data, dataClone);
                    notEmptyDict.Add(dataClone, x);
                }
            }
            return notEmptyDict;
        }

        public IEnumerable<T> GetNotEmptyClonedData()
        {
            return GetNotEmptyClonedDataDict().Keys;
        }
    }

}
