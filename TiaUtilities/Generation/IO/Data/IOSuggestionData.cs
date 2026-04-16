using Newtonsoft.Json;
using TiaUtilities.Languages;
using TiaUtilities.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.IO.Module
{
    public class IOSuggestionData : GridData
    {
        private readonly static int COLUMN_COUNT = 0;
        //THESE IS THE ORDER IN WHICH THEY APPEAR!
        public static readonly GridDataColumn VALUE;
        public static readonly IReadOnlyList<GridDataColumn> COLUMN_LIST;

        static IOSuggestionData()
        {
            var type = typeof(IOSuggestionData);
            VALUE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(Value), "suggestion");

            var columnList = GridDataColumn.GetStaticColumnList(type);
            columnList.Sort((x, y) => x.ColumnIndex.CompareTo(y.ColumnIndex));
            COLUMN_LIST = columnList.AsReadOnly();
        }

        [JsonProperty][Locale(nameof(Locale.IO_SUGGESTION_DATA_VALUE))] public string? Value { get => this.GetAs<string>(); set => this.Set(value); }

        public override IReadOnlyList<GridDataColumn> GetColumns()
        {
            return COLUMN_LIST;
        }

        public override GridDataColumn GetColumn(int column)
        {
            return COLUMN_LIST[column];
        }

        public override void Clear()
        {
            this.Value = "";
        }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(Value);
        }
    }
}
