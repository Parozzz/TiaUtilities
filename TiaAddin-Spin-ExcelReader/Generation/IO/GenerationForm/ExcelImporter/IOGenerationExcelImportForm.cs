using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Generation.GridHandler;
using Jint;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace TiaXmlReader.Generation.IO.GenerationForm.ExcelImporter
{
    public partial class IOGenerationExcelImportForm : Form
    {
        public const string ROW_SPECIAL_CHAR = "$";

        public const int ADDRESS_COLUMN = 0;
        public const int IONAME_COLUMN = 1;
        public const int COMMENT_COLUMN = 2;

        private readonly IOGenerationExcelImportSettings importSettings;
        private readonly GridHandler<IOGenerationExcelImportSettings, IOGenerationExcelImportData> gridHandler;

        public IEnumerable<IOGenerationExcelImportData> ImportDataEnumerable { get => gridHandler.DataSource.GetNotEmptyDataDict().Keys; }

        public IOGenerationExcelImportForm(IOGenerationExcelImportSettings importSettings, GridSettings gridSettings)
        {
            InitializeComponent();

            this.importSettings = importSettings;
            this.gridHandler = new GridHandler<IOGenerationExcelImportSettings, IOGenerationExcelImportData>
                (this.dataGridView, gridSettings, importSettings, IOGenerationExcelImportData.COLUMN_LIST)
            {
                RowCount = 1999
            };

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
            this.gridHandler.SetDragPreviewAction(data => { IOGenerationUtils.DragPreview(data, this.gridHandler); });
            this.gridHandler.SetDragMouseUpAction(data => { IOGenerationUtils.DragMouseUp(data, this.gridHandler); });
            #endregion

            #region COLUMNS
            this.gridHandler.AddTextBoxColumn(IOGenerationExcelImportData.ADDRESS, 80);
            this.gridHandler.AddTextBoxColumn(IOGenerationExcelImportData.IO_NAME, 110);
            this.gridHandler.AddTextBoxColumn(IOGenerationExcelImportData.COMMENT, 0);
            #endregion

            gridHandler?.Init();


            this.ConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Configurazione")
                {
                    ControlWidth = 600
                };

                configForm.AddJavascriptTextBoxLine("JS", height: 500)
                    .ControlText(importSettings.IgnoreRowExpressionConfig)
                    .TextChanged(str => importSettings.IgnoreRowExpressionConfig = str);

                configForm.FormClosed += (s, e) =>
                {

                };

                configForm.StartShowingAtCursor();
                configForm.Init();
                configForm.Show(this);

                dataGridView.Refresh();
            };

            this.ImportExcelButton.Click += (object sender, EventArgs args) =>
            {
                var fileDialog = new CommonOpenFileDialog
                {
                    EnsurePathExists = true,
                    EnsureFileExists = true,
                    DefaultExtension = ".xlsx",
                    Filters = { new CommonFileDialogFilter("Excel Files", "*.xlsx") }
                };

                if (fileDialog.ShowDialog(this.Handle) == CommonFileDialogResult.Ok)
                {
                    this.ImportExcel(fileDialog.FileName);
                }
            };
        }

        private void ImportExcel(string filePath)
        {
            this.gridHandler.DataSource.Clear();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var configWorkbook = new XLWorkbook(stream))
                {
                    var worksheet = configWorkbook.Worksheets.Worksheet(1);

                    //var t1 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

                    var excelCellLetterList = AddMatchExpression(new string[] {
                        importSettings.AddressCellConfig, importSettings.CommentCellConfig, importSettings.IONameCellConfig, importSettings.IgnoreRowExpressionConfig
                    });

                    var importDataList = new List<IOGenerationExcelImportData>();

                    uint rowIndex = importSettings.StartingRow;
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

                        if (!EvaluteRowExpression(excelCellValueDict, out bool expressionResult))
                        {
                            break;
                        }
                        else if (!expressionResult)
                        {
                            continue;
                        }

                        var address = importSettings.AddressCellConfig;
                        var ioName = importSettings.IONameCellConfig;
                        var comment = importSettings.CommentCellConfig;
                        foreach (var entry in excelCellValueDict)
                        {
                            address = address.Replace(entry.Key, entry.Value);
                            ioName = ioName.Replace(entry.Key, entry.Value);
                            comment = comment.Replace(entry.Key, entry.Value);
                        }

                        importDataList.Add(new IOGenerationExcelImportData()
                        {
                            Address = address,
                            IOName = ioName,
                            Comment = comment,
                        });
                    }
                    //Splitted this way to increase performance. Changing cell one at the time for 20-30 values takes 400ms, this way 10ms
                    var freeIndexList = this.gridHandler.DataSource.GetFirstEmptyRowIndexes(importDataList.Count);

                    var dataDict = new Dictionary<int, IOGenerationExcelImportData>();
                    for(int i = 0; i < freeIndexList.Count; i++)
                    {
                        dataDict.Add(freeIndexList[i], importDataList[i]);
                    }
                    this.gridHandler.ChangeMultipleRows(dataDict);

                    //var t2 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                    //Console.WriteLine("Time: " + (t2 - t1) + " ms");
                }
            }
        }

        private bool EvaluteRowExpression(Dictionary<string, string> dict, out bool result)
        {
            result = false;

            try
            {
                using(var engine = new Engine())
                {
                    foreach (var entry in dict)
                    {
                        engine.SetValue(entry.Key, entry.Value);
                    }

                    var eval = engine.Evaluate(importSettings.IgnoreRowExpressionConfig);
                    if (!eval.IsBoolean())
                    {
                        MessageBox.Show("Return must be boolean.", "Invalid ignore row operation");
                        return false;
                    }

                    result = eval.AsBoolean();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show("Error while compiling javascript.\n" + ex.Message, "Invalid ignore row operation");
                return false;
            }

        }

        private List<string> AddMatchExpression(string[] strArray)
        {
            var matchCollection = new List<string>();
            foreach (var str in strArray)
            {
                var matches = Regex.Matches(str, "[" + ROW_SPECIAL_CHAR + "]+\\w"); //Matches all the string with $+LETTER
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        var matchString = match.Value.ToUpper();
                        if (!matchCollection.Contains(matchString))
                        {
                            matchCollection.Add(matchString);
                        }
                    }
                }
            }
            return matchCollection;
        }
    }
}
