using Newtonsoft.Json;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Languages;

namespace TiaUtilities.Generation.TextsEditor
{
    public class TextsEditorData : GridData
    {
        private readonly static int COLUMN_COUNT = 0;
        //THESE IS THE ORDER IN WHICH THEY APPEAR!
        public static readonly GridDataColumn COLUMN_ID1;
        public static readonly GridDataColumn COLUMN_ID2;
        public static readonly GridDataColumn COLUMN_ID3;
        public static readonly GridDataColumn COLUMN_TEXT;
        public static readonly IReadOnlyList<GridDataColumn> COLUMN_LIST;

        static TextsEditorData()
        {
            var type = typeof(TextsEditorData);
            COLUMN_ID1 = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TextsEditorData.ID1));
            COLUMN_ID2 = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TextsEditorData.ID2));
            COLUMN_ID3 = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TextsEditorData.ID3));
            COLUMN_TEXT = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TextsEditorData.Text));

            var columnList = GridDataColumn.GetStaticColumnList(type);
            columnList.Sort((x, y) => x.ColumnIndex.CompareTo(y.ColumnIndex));
            COLUMN_LIST = columnList.AsReadOnly();
        }

        [JsonProperty][Locale("ID1")] public string? ID1 { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale("ID2")] public string? ID2 { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale("ID3")] public string? ID3 { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale("Text")] public string? Text { get => this.GetAs<string>(); set => this.Set(value); }

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
            this.ID1 = this.ID2 = this.ID3 = this.Text = null;
        }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.ID1) && string.IsNullOrEmpty(this.ID2) && string.IsNullOrEmpty(this.ID3); //Text can be empty!
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            var equals = GenUtils.CompareJsonFieldsAndProperties(this, obj, out _);
            return equals;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
