using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Generation;

namespace TiaXmlReader.GenerationForms.IO
{
    public class IOGenerationDataSource
    {
        private readonly DataGridView dataGridView;

        private readonly List<IOData> dataList;
        private readonly BindingList<IOData> bindingList;

        public IOGenerationDataSource(DataGridView dataGridView)
        {
            this.dataGridView = dataGridView;

            dataList = new List<IOData>();
            bindingList = new BindingList<IOData>(dataList);
        }

        public IOData this[int i]
        {
            get { return dataList[i]; }
            set { dataList[i] = value; }
        }

        public void Init()
        {
            this.InitializeData();
            this.dataGridView.DataSource = new BindingSource() { DataSource = bindingList }; ;
        }

        public void InitializeData()
        {
            this.dataList.Clear();
            for (int i = 0; i < IOGenerationForm.TOTAL_ROW_COUNT; i++)
            {
                dataList.Add(new IOData());
            }
        }

        public IOData GetDataAt(int rowIndex)
        {
            return dataList[rowIndex];
        }

        public void AddDataAtEnd(IOData newData)
        {
            AddMultipleDataAtEnd(new List<IOData>() { newData });
        }

        public void AddMultipleDataAtEnd(List<IOData> newDataCollection)
        {
            this.dataGridView.SuspendLayout();

            var newDataAdded = 0;
            for (int i = 0; i < IOGenerationForm.TOTAL_ROW_COUNT; i++)
            {
                var data = this[i];
                if (data.IsEmpty())
                {
                    data.CopyFrom(newDataCollection[newDataAdded]);
                    if(++newDataAdded >= newDataCollection.Count)
                    {
                        break;
                    }
                }
            }

            this.dataGridView.Refresh();
            this.dataGridView.ResumeLayout();
        }

        public void Sort(IComparer<IOData> comparer, SortOrder sortOrder)
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

        public Dictionary<IOData, int> CreateDataListSnapshot()
        {
            var dict = new Dictionary<IOData, int>();
            for (int x = 0; x < dataList.Count; x++)
            {
                dict.Add(dataList[x], x);
            }
            return dict;
        }

        public void RestoreDataListSnapshot(Dictionary<IOData, int> dict)
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
        
        public Dictionary<IOData, int> GetNotEmptyDataDict()
        {
            var notEmptyDict = new Dictionary<IOData, int>();

            for(var x = 0; x < dataList.Count; x++)
            {
                var data = dataList[x];
                if(!data.IsEmpty())
                {
                    notEmptyDict.Add(data, x);
                }
            }
            return notEmptyDict;
        }

    }
}
