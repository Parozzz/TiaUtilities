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
using TiaXmlReader.Utility;
using InfoBox;
using TiaXmlReader.Javascript;

namespace TiaXmlReader.Generation.IO.GenerationForm.ExcelImporter
{
    public partial class IOGenerationExcelImportForm : Form
    {
        public const string ROW_SPECIAL_CHAR = "$";

        public const int ADDRESS_COLUMN = 0;
        public const int IONAME_COLUMN = 1;
        public const int COMMENT_COLUMN = 2;

        private readonly IOGenerationExcelImportSettings settings;
        private readonly GridHandler<IOGenerationExcelImportSettings, IOGenerationExcelImportData> gridHandler;

        public IEnumerable<IOGenerationExcelImportData> ImportDataEnumerable { get => gridHandler.DataSource.GetNotEmptyDataDict().Keys; }

        public IOGenerationExcelImportForm(JavascriptScriptErrorReportingThread jsErrorHandlingThread, IOGenerationExcelImportSettings settings, GridSettings gridSettings)
        {
            InitializeComponent();

            this.settings = settings;
            this.gridHandler = new GridHandler<IOGenerationExcelImportSettings, IOGenerationExcelImportData>
                (jsErrorHandlingThread, this.dataGridView, gridSettings, settings, IOGenerationExcelImportData.COLUMN_LIST)
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

            #region JS_SCRIPT
            this.gridHandler.TableScript.SetReadScriptFunc(() => settings.JSScript);
            this.gridHandler.TableScript.SetWriteScriptAction((str) => settings.JSScript = str);
            #endregion

            this.ConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Configurazione")
                {
                    ControlWidth = 500
                };

                configForm.AddTextBoxLine("Indirizzo")
                    .ControlText(settings.AddressCellConfig)
                    .TextChanged(str => settings.AddressCellConfig = str);

                configForm.AddTextBoxLine("Nome IO")
                    .ControlText(settings.IONameCellConfig)
                    .TextChanged(str => settings.IONameCellConfig = str);

                configForm.AddTextBoxLine("Commento")
                    .ControlText(settings.CommentCellConfig)
                    .TextChanged(str => settings.CommentCellConfig = str);

                configForm.AddTextBoxLine("Riga di partenza")
                    .ControlText(settings.StartingRow)
                    .UIntChanged(num => settings.StartingRow = num);

                configForm.AddJavascriptTextBoxLine("Espressione validità riga", height: 200)
                    .ControlText(settings.IgnoreRowExpressionConfig)
                    .TextChanged(str => settings.IgnoreRowExpressionConfig = str);

                configForm.StartShowingAtControl(this.ConfigButton);
                configForm.Init();
                configForm.Show(this);
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

            var scriptTimer = new ScriptTimer();
            try
            {
                using (Engine engine = new Engine())
                {
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var configWorkbook = new XLWorkbook(stream))
                        {
                            var worksheet = configWorkbook.Worksheets.Worksheet(1);

                            var excelCellLetterList = AddMatchExpression(new string[] {
                                settings.AddressCellConfig, settings.CommentCellConfig, settings.IONameCellConfig, settings.IgnoreRowExpressionConfig
                            });

                            var importDataList = new List<IOGenerationExcelImportData>();

                            uint rowIndex = settings.StartingRow;
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

                                if (!EvaluteRowExpressionTimed(scriptTimer, engine, excelCellValueDict, out bool expressionResult))
                                {
                                    break;
                                }
                                else if (!expressionResult)
                                {
                                    continue;
                                }

                                var address = settings.AddressCellConfig;
                                var ioName = settings.IONameCellConfig;
                                var comment = settings.CommentCellConfig;
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
                            for (int i = 0; i < freeIndexList.Count; i++)
                            {
                                dataDict.Add(freeIndexList[i], importDataList[i]);
                            }
                            this.gridHandler.ChangeMultipleRows(dataDict);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }
            finally
            {
                scriptTimer.StopAndSave();
                scriptTimer.Log(settings.IgnoreRowExpressionConfig, "IOGenerationExcelImportForm");
            }

        }

        private bool EvaluteRowExpressionTimed(ScriptTimer scriptTimer, Engine engine, Dictionary<string, string> dict, out bool result)
        {
            scriptTimer.Restart();
            var ret = EvaluteRowExpression(engine, dict, out result);
            scriptTimer.StopAndSave();

            return ret;
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

                var eval = engine.Evaluate(settings.IgnoreRowExpressionConfig);
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
