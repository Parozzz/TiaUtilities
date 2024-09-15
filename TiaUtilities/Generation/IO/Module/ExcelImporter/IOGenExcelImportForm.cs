using ClosedXML.Excel;
using InfoBox;
using Jint;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Text.RegularExpressions;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.IO.Module;
using TiaUtilities.Generation.IO.Module.ExcelImporter;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Javascript;
using TiaXmlReader.Utility;

namespace TiaUtilities.Generation.GenModules.IO.ExcelImporter
{
    public partial class IOGenerationExcelImportForm : Form
    {
        public const string ROW_SPECIAL_CHAR = "$";

        private readonly IOGenExcelImportConfiguration importConfig;
        private readonly GridHandler<IOGenExcelImportData> gridHandler;

        public IEnumerable<IOGenExcelImportData> ImportDataEnumerable { get => gridHandler.DataSource.GetNotEmptyDataDict().Keys; }

        public IOGenerationExcelImportForm(JavascriptErrorReportThread jsErrorHandlingThread, GridSettings gridSettings, GridScriptContainer scriptContainer,
                    IOGenExcelImportConfiguration configuration)
        {
            InitializeComponent();

            this.importConfig = configuration;
            this.gridHandler = new(jsErrorHandlingThread, gridSettings, scriptContainer, new(), new()) { RowCount = 1999 };

            Init();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Cancel || keyData == Keys.Escape)
            {
                this.CancelButton.PerformClick();
                return true;    // indicate that you handled this keystroke
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Init()
        {
            this.MainTableLayoutPanel.Controls.Add(this.gridHandler.DataGridView);

            #region FORM
            this.AcceptButton.Click += (object sender, EventArgs args) =>
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.CancelButton.Click += (object sender, EventArgs args) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            #endregion

            #region DRAG
            this.gridHandler.Events.ExcelDragPreview += (sender, args) => IOGenUtils.DragPreview(args, this.gridHandler);
            this.gridHandler.Events.ExcelDragDone += (sender, args) => IOGenUtils.DragDone(args, this.gridHandler);
            #endregion

            #region COLUMNS
            this.gridHandler.AddTextBoxColumn(IOGenExcelImportData.ADDRESS, 80);
            this.gridHandler.AddTextBoxColumn(IOGenExcelImportData.IO_NAME, 110);
            this.gridHandler.AddTextBoxColumn(IOGenExcelImportData.COMMENT, 0);
            #endregion

            gridHandler.Init();

            this.ConfigButton.Click += (sender, args) =>
            {
                var configForm = new ConfigForm("Configurazione")
                {
                    ControlWidth = 500
                };

                var mainGroup = configForm.Init();
                mainGroup.AddTextBox().Label("Indirizzo")
                    .ControlText(importConfig.AddressCellConfig)
                    .TextChanged(str => importConfig.AddressCellConfig = str);

                mainGroup.AddTextBox().Label("Nome IO")
                     .ControlText(importConfig.IONameCellConfig)
                     .TextChanged(str => importConfig.IONameCellConfig = str);

                mainGroup.AddTextBox().Label("Commento")
                     .ControlText(importConfig.CommentCellConfig)
                     .TextChanged(str => importConfig.CommentCellConfig = str);

                mainGroup.AddTextBox().Label("Riga di partenza")
                     .ControlText(importConfig.StartingRow)
                     .UIntChanged(num => importConfig.StartingRow = num);

                mainGroup.AddJavascript().Label("Espressione\nvalidità riga").Height(200)
                     .ControlText(importConfig.IgnoreRowExpressionConfig)
                     .TextChanged(str => importConfig.IgnoreRowExpressionConfig = str);

                configForm.StartShowingAtControl(this.ConfigButton);
                configForm.Init();
                configForm.Show(this);
            };

            this.ImportExcelButton.Click += (sender, args) =>
            {
                var fileDialog = new CommonOpenFileDialog
                {
                    EnsurePathExists = true,
                    EnsureFileExists = true,
                    Filters = { new CommonFileDialogFilter("Excel Files", "*.xlsx,*.xls") }
                };

                if (fileDialog.ShowDialog(ownerWindowHandle: this.Handle) == CommonFileDialogResult.Ok)
                {
                    var fileName = fileDialog.FileName;
                    if (fileName != null)
                    {
                        this.ImportExcel(fileName);
                    }
                }
            };
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

                var excelCellLetterList = AddMatchExpression([
                                importConfig.AddressCellConfig, importConfig.CommentCellConfig, importConfig.IONameCellConfig, importConfig.IgnoreRowExpressionConfig
                            ]);

                var importDataList = new List<IOGenExcelImportData>();

                uint rowIndex = importConfig.StartingRow;
                while (true)
                {
                    var excelCellValueDict = new Dictionary<string, string>();

                    bool allEmpty = true;
                    foreach (var cellLetter in excelCellLetterList)
                    {
                        var cellValue = worksheet.Cell(cellLetter + rowIndex).Value.ToString();
                        excelCellValueDict.Add(cellLetter, cellValue); //There should not be two equals cellLetter. If there are, somethign wrong in AddMatchExpression.

                        allEmpty &= string.IsNullOrEmpty(cellValue);
                    }
                    rowIndex++;

                    if (allEmpty)
                    {
                        break;
                    }

                    if (!EvaluteRowExpression(engine, excelCellValueDict, out bool expressionResult))
                    {
                        break;
                    }
                    else if (!expressionResult)
                    {
                        continue;
                    }

                    var address = importConfig.AddressCellConfig;
                    var ioName = importConfig.IONameCellConfig;
                    var comment = importConfig.CommentCellConfig;
                    foreach (var entry in excelCellValueDict)
                    {
                        address = address.Replace(entry.Key, entry.Value);
                        ioName = ioName.Replace(entry.Key, entry.Value);
                        comment = comment.Replace(entry.Key, entry.Value);
                    }

                    importDataList.Add(new() { Address = address, IOName = ioName, Comment = comment });
                }
                //Splitted this way to increase performance. Changing cell one at the time for 20-30 values takes 400ms, this way 10ms
                var freeIndexList = this.gridHandler.DataSource.GetFirstEmptyRowIndexes(importDataList.Count);

                var dataDict = new Dictionary<int, IOGenExcelImportData>();
                for (int i = 0; i < freeIndexList.Count; i++)
                {
                    dataDict.Add(freeIndexList[i], importDataList[i]);
                }
                this.gridHandler.ChangeMultipleRows(dataDict);
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

        }

        private bool EvaluteRowExpression(Engine engine, Dictionary<string, string> dict, out bool result)
        {
            result = false;
            try
            {
                foreach (var entry in dict)
                {
                    engine.SetValue(entry.Key, entry.Value);
                }

                var eval = engine.Evaluate(importConfig.IgnoreRowExpressionConfig);
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
