using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.Placeholders;

namespace TiaUtilities.Generation.TextsEditor
{
    public partial class GenModuleTextsEditorForm : Form
    {
        public const string REFERENCE_EDITOR_SPLITTER = "//";

        private readonly ITextsEditorExporter exporter;

        private readonly MultiGridOperationHandler multiGrid;
        private readonly GridDataPreviewer<TextsEditorData> previewer;
        private readonly GridHandler<TextsEditorData> gridHandler;

        public GenModuleTextsEditorForm(ITextsEditorExporter exporter)
        {
            InitializeComponent();

            this.exporter = exporter;

            this.multiGrid = new();
            this.previewer = new();

            GenPlaceholderHandler placeholderHandler = new();
            this.gridHandler = new(MainForm.Settings.GridSettings, this.multiGrid, this.previewer, placeholderHandler, new TextsEditorDataComparer()) { InitializeRowCount = 1 };

            Init();
        }

        private void Init()
        {
            #region GRID_COLUMNS
            var id1Column = this.gridHandler.Columns.AddTextBox(TextsEditorData.COLUMN_ID1, 200);
            id1Column.ReadOnly = true;

            var id2Column = this.gridHandler.Columns.AddTextBox(TextsEditorData.COLUMN_ID2, 200);
            id2Column.ReadOnly = true;

            var id3Column = this.gridHandler.Columns.AddTextBox(TextsEditorData.COLUMN_ID3, 200);
            id3Column.ReadOnly = true;

            this.gridHandler.Columns.AddTextBox(TextsEditorData.COLUMN_TEXT, -1);
            #endregion

            this.saveButton.Click += (sender, args) =>
            {
                List<GenModuleEditableTextReference> editableTextReferenceList = [];

                var dataEnumerable = this.gridHandler.DataSource.GetNotEmptyData();
                foreach(var data in dataEnumerable)
                {
                    if(string.IsNullOrEmpty(data.ID1) || string.IsNullOrEmpty(data.ID2) || string.IsNullOrEmpty(data.ID3) )
                    {
                        continue;
                    }

                    GenModuleEditableTextReference reference = new(data.ID1, data.ID2, data.ID3, data.Text);
                    editableTextReferenceList.Add(reference);

                    this.Close();
                }

                exporter.SetTextsReferences(editableTextReferenceList);
            };

            this.gridHandler.Init();
            LoadReferences();

            this.mainPanel.Controls.Add(this.gridHandler.GetControl(), 0, 0);

        }

        private void LoadReferences()
        {
            var referencesList = exporter.GetTextsReferences();

            this.gridHandler.DataSource.InitializeData((uint)referencesList.Count);

            List<TextsEditorData> dataList = [];
            foreach (var reference in referencesList)
            {
                TextsEditorData data = new()
                {
                    ID1 = reference.ID1,
                    ID2 = reference.ID2,
                    ID3 = reference.ID3,
                    Text = reference.Text,
                };
                dataList.Add(data);
            }
            this.gridHandler.AppendData(dataList);
        }
    }

    public record GenModuleEditableTextReference(string ID1, string ID2, string ID3, string? Text);
}
