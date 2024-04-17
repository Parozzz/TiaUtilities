using ClosedXML.Excel;
using Flee.PublicTypes;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TiaXmlReader.GenerationForms.GridHandler;

namespace TiaXmlReader.GenerationForms.IO.ExcelImporter
{
    public partial class IOGenerationExcelImportForm : Form
    {
        public const string ROW_SPECIAL_CHAR = "$";
        public const string EXPRESSION_OPERATORS = "Operators: Bool[<>, =, >, <, >=, <=, AND, OR, XOR, NAND].\nString[.Contains(str), .StartsWith(str), .EndsWith(str)].";

        public const int ADDRESS_COLUMN = 0;
        public const int IONAME_COLUMN = 1;
        public const int COMMENT_COLUMN = 2;

        public class ExcelImportData : IGridData
        {
            public string Address { get; set; }
            public string IOName { get; set; }
            public string Comment { get; set; }

            public void Clear()
            {
                this.Address = this.IOName = this.Comment = "";
            }

            public void CopyFrom(ExcelImportData excelImport)
            {
                this.Address = excelImport.Address;
                this.IOName = excelImport.IOName;
                this.Comment = excelImport.Comment;
            }

            public bool IsEmpty()
            {
                return string.IsNullOrEmpty(Address) && string.IsNullOrEmpty(IOName) && string.IsNullOrEmpty(Comment);
            }
        }

        private readonly IOGenerationExcelImportConfiguration config;
        private readonly GridHandler<ExcelImportData> gridHandler;

        public IEnumerable<ExcelImportData> ImportDataEnumerable { get => gridHandler.DataSource.GetNotEmptyDataDict().Keys; }

        public IOGenerationExcelImportForm(IOGenerationExcelImportConfiguration config)
        {
            InitializeComponent();

            this.config = config;
            this.gridHandler = new GridHandler<ExcelImportData>(this.dataGridView, () => new ExcelImportData(), (oldData, newData) => oldData.CopyFrom(newData));

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

            #region DATA_ASSOCIATION
            this.gridHandler.SetDataAssociation(ADDRESS_COLUMN, importData => importData.Address);
            this.gridHandler.SetDataAssociation(IONAME_COLUMN, importData => importData.IOName);
            this.gridHandler.SetDataAssociation(COMMENT_COLUMN, importData => importData.Comment);
            #endregion

            gridHandler.Init();

            #region COLUMNS
            var addressColumn = this.gridHandler.InitColumn(ADDRESS_COLUMN, "Indirizzo", 80);
            var nameIOColumn = this.gridHandler.InitColumn(IONAME_COLUMN, "Nome IO", 110);
            var commentColumn = this.gridHandler.InitColumn(COMMENT_COLUMN, "Commento", 0);
            #endregion

            this.ConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Configurazione");
                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Indirizzo")
                    .TextBox(config.AddressCellConfig)
                    .TextChanged(str => config.AddressCellConfig = str));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Nome IO")
                    .TextBox(config.IONameCellConfig)
                    .TextChanged(str => config.IONameCellConfig = str));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Commento")
                    .TextBox(config.CommentCellConfig)
                    .TextChanged(str => config.CommentCellConfig = str));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Riga di partenza")
                    .TextBox(config.StartingRow)
                    .NumericOnly()
                    .UIntChanged(num => config.StartingRow = num));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Espressione validità riga")
                    .TextBox(config.IgnoreRowExpressionConfig)
                    .TextChanged(str => config.IgnoreRowExpressionConfig = str));

                configForm.StartShowingAtControl(this.ConfigButton);
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

                    var excelCellLetterList = AddMatchExpression(new string[] {
                        config.AddressCellConfig, config.CommentCellConfig, config.IONameCellConfig, config.IgnoreRowExpressionConfig
                    });

                    uint rowIndex = config.StartingRow;
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

                        var address = config.AddressCellConfig;
                        var ioName = config.IONameCellConfig;
                        var comment = config.CommentCellConfig;
                        foreach (var entry in excelCellValueDict)
                        {
                            address = address.Replace(ROW_SPECIAL_CHAR + entry.Key, entry.Value);
                            ioName = ioName.Replace(ROW_SPECIAL_CHAR + entry.Key, entry.Value);
                            comment = comment.Replace(ROW_SPECIAL_CHAR + entry.Key, entry.Value);
                        }

                        var freeIndexList = this.gridHandler.DataSource.GetFirstEmptyRowIndexes(1);
                        if (freeIndexList.Count == 1)
                        {
                            var freeRowIndex = freeIndexList[0];
                            this.gridHandler.ChangeRow(freeRowIndex, new ExcelImportData()
                            {
                                Address = address,
                                IOName = ioName,
                                Comment = comment,
                            });
                        }
                    }
                }
            }
        }

        private bool EvaluteRowExpression(Dictionary<string, string> dict, out bool result)
        {
            result = false;

            try
            {
                ExpressionContext context = new ExpressionContext();
                context.Options.StringComparison = StringComparison.OrdinalIgnoreCase;
                context.ParserOptions.DecimalSeparator = '.';
                context.ParserOptions.FunctionArgumentSeparator = ','; //Seperator 
                foreach (var entry in dict)
                {
                    context.Variables.Add(entry.Key, entry.Value);
                }

                var dynExp = context.CompileDynamic(config.IgnoreRowExpressionConfig.Replace(ROW_SPECIAL_CHAR, ""));
                if (!(dynExp.Evaluate() is bool eval))
                {
                    MessageBox.Show("Return must be boolean.\n" + EXPRESSION_OPERATORS, "Invalid ignore row operation");
                    return false;
                }

                result = eval;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show("Error while compiling.\n" + EXPRESSION_OPERATORS + "\n" + ex.Message, "Invalid ignore row operation");
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
                        var matchString = match.Value.Replace(ROW_SPECIAL_CHAR, "").ToUpper();
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