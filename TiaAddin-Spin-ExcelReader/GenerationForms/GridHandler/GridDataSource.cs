using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.GenerationForms.GridHandler;

namespace TiaXmlReader.GenerationForms.GridHandler
{
    public class GridDataSource<T> where T : IGridData
    {
        private readonly DataGridView dataGridView;
        private readonly Func<T> newObjectFunction;
        private readonly Action<T, T> trasferDataAction; //-> 1° = old 2° = new

        private readonly List<T> dataList;
        private readonly BindingList<T> bindingList;

        public GridDataSource(DataGridView dataGridView, Func<T> newObjectFunction, Action<T, T> trasferDataAction)
        {
            this.dataGridView = dataGridView;
            this.newObjectFunction = newObjectFunction;
            this.trasferDataAction = trasferDataAction;

            dataList = new List<T>();
            bindingList = new BindingList<T>(dataList);
        }

        public T this[int i]
        {
            get { return dataList[i]; }
            set { dataList[i] = value; }
        }

        public void Clear()
        {
            foreach(var data in this.dataList)
            {
                data.Clear();
            }
        }

        public void InitializeData(uint dataAmount)
        {
            this.dataList.Clear();
            for (int i = 0; i < dataAmount; i++)
            {
                dataList.Add(newObjectFunction.Invoke());
            }

            this.dataGridView.DataSource = new BindingSource() { DataSource = bindingList };
        }

        public List<int> GetFirstEmptyRowIndexes(int num)
        {
            var emptyDataRowIndexList = new List<int>();

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
                if (!dict.ContainsKey(x) || !dict.ContainsKey(y))
                {
                    return 0;
                }

                return dict[x].CompareTo(dict[y]);
            });
            dataGridView.Refresh();
        }

        public Dictionary<T, int> GetNotEmptyDataDict()
        {
            var notEmptyDict = new Dictionary<T, int>();

            for (var x = 0; x < dataList.Count; x++)
            {
                var data = dataList[x];
                if (!data.IsEmpty())
                {
                    notEmptyDict.Add(data, x);
                }
            }
            return notEmptyDict;
        }

    }

    public interface IGridData
    {
        void Clear();
        bool IsEmpty();
    }

}
