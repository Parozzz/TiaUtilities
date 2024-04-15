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
            for (int i = 0; i < IOGenerationForm.TOTAL_ROW_COUNT; i++)
            {
                dataList.Add(new IOData());
            }

            bindingList = new BindingList<IOData>(dataList);
        }

        public void Init()
        {
            this.dataGridView.DataSource = new BindingSource() { DataSource = bindingList }; ;
        }

        public IOData GetDataAt(int rowIndex)
        {
            return dataList[rowIndex];
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
