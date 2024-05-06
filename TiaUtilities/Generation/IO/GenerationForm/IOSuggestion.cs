using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.Localization;

namespace TiaXmlReader.Generation.IO.GenerationForm
{
    public class IOSuggestion : IGridData<IOConfiguration>
    {
        public static int COLUMN_COUNT = 0;
        //THESE IS THE ORDER IN WHICH THEY APPEAR!
        public static readonly GridDataColumn VALUE;
        public static readonly List<GridDataColumn> COLUMN_LIST;

        static IOSuggestion()
        {
            var type = typeof(IOSuggestion);
            VALUE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOSuggestion.Value));
            COLUMN_LIST = GridDataColumn.GetStaticColumnList(type);
            COLUMN_LIST.Sort((x, y) => x.ColumnIndex.CompareTo(y.ColumnIndex));
        }

        [JsonProperty][Localization("IO_SUGGESTION_VALUE")] public string Value { get; set; }

        public GridDataPreview GetPreview(int column, IOConfiguration config)
        {
            return null;
        }

        public void Clear()
        {
            this.Value = "";
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Value);
        }
    }
}
