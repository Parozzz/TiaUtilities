using ClosedXML.Excel;
using InfoBox;
using Jint;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Text.RegularExpressions;
using TiaUtilities.Editors.ErrorReporting;
using TiaUtilities.Generation.Configuration;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.IO.Configurations;
using TiaUtilities.Generation.IO.Data;
using TiaUtilities.Generation.SettingsNew;
using TiaUtilities.Languages;
using TiaUtilities.Resources;
using TiaUtilities.SettingsNew.Bindings;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.IO.Module.ExcelImporter
{
    public partial class IOGenerationExcelImportForm : Form
    {
        public const string ROW_SPECIAL_CHAR = "$";

        private readonly IOExcelImportConfiguration excelImportConfig;
        private readonly GridHandler<IOGenExcelImportData> gridHandler;

        private readonly SettingsBindings settingsBindings;
        public IEnumerable<IOGenExcelImportData> ImportDataEnumerable { get => gridHandler.DataSource.GetNotEmptyDataDict().Keys; }

        public IOGenerationExcelImportForm(GridSettings gridSettings, GridBindContainer gridBindContainer, IOExcelImportConfiguration configuration)
        {
            InitializeComponent();

            this.excelImportConfig = configuration;
            this.gridHandler = new(gridSettings, gridBindContainer, new(), new()) { InitializeRowCount = 1999 };

            this.settingsBindings = new();

            Init();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Cancel || keyData == Keys.Escape)
            {
                this.cancelButton.PerformClick();
                return true;    // indicate that you handled this keystroke
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Init()
        {
            this.MainTableLayoutPanel.Controls.Add(this.gridHandler.GetControl());

            #region FORM
            this.acceptButton.Click += (object sender, EventArgs args) =>
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.cancelButton.Click += (object sender, EventArgs args) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            #endregion

            #region DRAG
            this.gridHandler.ExcelDragPreview += (sender, args) => IOGenUtils.DragPreview(args, this.gridHandler);
            this.gridHandler.ExcelDragDone += (sender, args) => IOGenUtils.DragDone(args, this.gridHandler);
            #endregion

            #region COLUMNS
            this.gridHandler.Columns.AddTextBox(IOGenExcelImportData.ADDRESS, 80);
            this.gridHandler.Columns.AddTextBox(IOGenExcelImportData.IO_NAME, 110);
            this.gridHandler.Columns.AddTextBox(IOGenExcelImportData.COMMENT, 0);
            #endregion

            gridHandler.Init();

            IOGenUtils.AddExcelImporterSettingsBindings(this.settingsBindings, this.excelImportConfig);
            this.setupButton.Click += (sender, args) => new SettingsForm(this.settingsBindings).ShowDialog(this);

            this.importExcelButton.Click += (sender, args) =>
            {
                var savedFilePath = MainForm.Settings.GetSavedFileDialogPath(FileDialogResources.GENERATION_IO_IMPORT_EXCEL);

                var fileDialog = new CommonOpenFileDialog
                {
                    EnsurePathExists = true,
                    EnsureFileExists = true,
                    Filters = { new CommonFileDialogFilter("Excel Files", "*.xlsx,*.xls") },
                    InitialDirectory = savedFilePath,
                };

                if (fileDialog.ShowDialog(ownerWindowHandle: this.Handle) == CommonFileDialogResult.Ok)
                {
                    var fileName = fileDialog.FileName;
                    if (fileName != null)
                    {
                        this.ImportExcel(fileName);

                        MainForm.Settings.SetSavedFileDialogPath(FileDialogResources.GENERATION_IO_IMPORT_EXCEL, Path.GetDirectoryName(fileName));
                    }
                }
            };

            this.Translate();
        }

        private void Translate()
        {
            this.importExcelButton.Text = Locale.IO_GEN_FORM_IMPEXP_IMPORT_EXCEL;
            this.setupButton.Text = Locale.GENERICS_SETUP;
            this.acceptButton.Text = Locale.GENERICS_ACCEPT;
            this.cancelButton.Text = Locale.GENERICS_CANCEL;
        }

        private void ImportExcel(string filePath)
        {
            this.gridHandler.DataSource.Clear();

            try
            {
                using var engine = new Engine(options =>
                {
                    options.LimitMemory(20_000_000); // Limit memory allocations to MB
                    options.TimeoutInterval(TimeSpan.FromMilliseconds(1000)); // Set a timeout to 500 ms.
                    options.MaxStatements(int.MaxValue);
                    options.LimitRecursion(1);
                    options.Strict = true;
                });
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var configWorkbook = new XLWorkbook(stream);

                var worksheet = configWorkbook.Worksheets.Worksheet(1);

                var excelColumnsLetterList = AddMatchExpression([
                                excelImportConfig.AddressCellConfig, excelImportConfig.CommentCellConfig, excelImportConfig.IONameCellConfig, excelImportConfig.IgnoreRowExpressionConfig
                            ]);

                var importDataList = new List<IOGenExcelImportData>();

                var usedRows = worksheet.RowsUsed();
                foreach (var row in usedRows)
                {
                    var rowNumber = row.RowNumber();
                    if (rowNumber < this.excelImportConfig.StartingRow)
                    {
                        continue;
                    }

                    var excelCellValueDict = new Dictionary<string, string>();

                    bool allEmpty = true;
                    foreach (var columnLetter in excelColumnsLetterList)
                    {
                        var cellValue = row.Cell(columnLetter.Replace("$", "")).Value.ToString();
                        excelCellValueDict.Add(columnLetter, cellValue); //There should not be two equals cellLetter. If there are, somethign wrong in AddMatchExpression.

                        allEmpty &= string.IsNullOrEmpty(cellValue);
                    }

                    if (allEmpty || !this.EvaluateRowExpression(engine, excelCellValueDict, out bool expressionResult) || !expressionResult)
                    {
                        continue;
                    }

                    var ioNameEvaluateOk = EvaluateIONameExpression(engine, excelCellValueDict, out string ioName);
                    if(!ioNameEvaluateOk)
                    {
                        continue;
                    }

                    var address = excelImportConfig.AddressCellConfig;
                    var comment = excelImportConfig.CommentCellConfig;
                    foreach (var entry in excelCellValueDict)
                    {
                        address = address.Replace(entry.Key, entry.Value);
                        ioName = ioName.Replace(entry.Key, entry.Value);
                        comment = comment.Replace(entry.Key, entry.Value);
                    }

                    importDataList.Add(new() { Address = address, IOName = ioName, Comment = comment });
                }

                var req = this.gridHandler.DataChangedHandler.Join();

                //Splitted this way to increase performance. Changing cell one at the time for 20-30 values takes 400ms, this way 10ms
                var emptyIndexList = this.gridHandler.DataSource.GetFirstEmptyRowIndexes(importDataList.Count);

                var dataDict = new Dictionary<int, IOGenExcelImportData>();
                for (int i = 0; i < emptyIndexList.Count; i++)
                {
                    var emptyIndex = emptyIndexList[i];
                    var importData = importDataList[i];

                    var emptyImportData = this.gridHandler.DataSource[emptyIndex];
                    GridUtils.CopyGridDataValues(importData, emptyImportData);
                }

                this.gridHandler.DataChangedHandler.End(req);
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

        }

        private bool EvaluateRowExpression(Engine engine, Dictionary<string, string> dict, out bool result)
        {
            result = false;
            try
            {
                foreach (var entry in dict)
                {
                    engine.SetValue(entry.Key, entry.Value);
                }

                var eval = engine.Evaluate(excelImportConfig.IgnoreRowExpressionConfig);
                if (!eval.IsBoolean())
                {
                    InformationBox.Show("Return must be boolean.", "Invalid ignore row operation", icon: InformationBoxIcon.Exclamation);
                    return false;
                }

                result = eval.AsBoolean();
                return true;
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
                return false;
            }
        }

        private bool EvaluateIONameExpression(Engine engine, Dictionary<string, string> dict, out string result)
        {
            result = "";

            foreach (var entry in dict)
            {
                engine.SetValue(entry.Key, entry.Value);
            }

            var eval = engine.Evaluate(excelImportConfig.IONameCellConfig);
            if (!eval.IsString())
            {
                InformationBox.Show("Return must be boolean.", "Invalid ignore row operation", icon: InformationBoxIcon.Exclamation);
                return false;
            }

            result = eval.AsString();
            return true;
        }

        [GeneratedRegex(@"[$]+\w")]
        private static partial Regex RowRegex();

        private static List<string> AddMatchExpression(string[] strArray)
        {
            var matchCollection = new List<string>();
            foreach (var str in strArray)
            {
                var matches = RowRegex().Matches(str); //Matches all the string with $+LETTER
                matchCollection.AddRange(
                    matches.Where(m => m.Success).Select(m => m.Value.ToUpper()).Where(s => !matchCollection.Contains(s))
                );
            }
            return matchCollection;
        }
    }
}
