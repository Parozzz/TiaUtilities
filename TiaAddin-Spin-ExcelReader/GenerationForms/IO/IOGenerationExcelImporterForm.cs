using ClosedXML.Excel;
using Flee.PublicTypes;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TiaXmlReader.GenerationForms.IO
{
    public partial class IOGenerationExcelImporterForm : Form
    {
        public const string ROW_SPECIAL_CHAR = "$";
        public const string EXPRESSION_OPERATORS = "Operators: Bool[<>, =, >, <, >=, <=, AND, OR, XOR, NAND].\nString[.Contains(str), .StartsWith(str), .EndsWith(str)].";

        public const int ADDRESS_COLUMN = 0;
        public const int IONAME_COLUMN = 1;
        public const int COMMENT_COLUMN = 2;

        public class ExcelImportData
        {
            public string Address { get; set; }
            public string IOName { get; set; }
            public string Comment { get; set; }
        }

        private string addressCellConfig = "$A";
        private string ioNameCellConfig = "$A";
        private string commentCellConfig = "$E $F $G $H (P$K - $O)";
        private uint startingRow = 2;
        private string ignoreRowExpressionConfig = "$A <> \"\"";

        private readonly BindingList<ExcelImportData> bindingList;
        public IEnumerable<ExcelImportData> ImportDataEnumerable { get => bindingList; }

        public IOGenerationExcelImporterForm()
        {
            InitializeComponent();

            this.bindingList = new BindingList<ExcelImportData>(new List<ExcelImportData>());

            Init();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Cancel || keyData == Keys.Escape)
            {
                this.Close();
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

            this.dataGridView.DataSource = new BindingSource() { DataSource = bindingList };
            this.dataGridView.AutoGenerateColumns = false;

            this.dataGridView.Dock = DockStyle.Fill;
            this.dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.dataGridView.AutoSize = false;

            this.dataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dataGridView.MultiSelect = true;

            this.dataGridView.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

            this.dataGridView.AllowUserToAddRows = false;

            var addressColumn = InitColumn(dataGridView.Columns[ADDRESS_COLUMN], "Indirizzo", 80);
            var nameIOColumn = InitColumn(dataGridView.Columns[IONAME_COLUMN], "Nome IO", 110);
            var commentColumn = InitColumn(dataGridView.Columns[COMMENT_COLUMN], "Commento", 0);

            this.ConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Configurazione");
                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Indirizzo")
                    .TextBox(addressCellConfig)
                    .TextChanged(str => addressCellConfig = str));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Nome IO")
                    .TextBox(ioNameCellConfig)
                    .TextChanged(str => ioNameCellConfig = str));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Commento")
                    .TextBox(commentCellConfig)
                    .TextChanged(str => commentCellConfig = str));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Riga di partenza")
                    .TextBox(startingRow)
                    .NumericOnly()
                    .UIntChanged(num => startingRow = num));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Espressione validità riga")
                    .TextBox(ignoreRowExpressionConfig)
                    .TextChanged(str => ignoreRowExpressionConfig = str));

                configForm.StartShowingAtControl(this.ConfigButton);
                configForm.Init();
                configForm.ShowDialog(this);

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

        private T InitColumn<T>(T column, string name, int width) where T : DataGridViewColumn
        {
            column.Name = name;
            column.DefaultCellStyle.SelectionBackColor = Color.LightGray;
            column.DefaultCellStyle.BackColor = SystemColors.ControlLightLight;
            column.DefaultCellStyle.SelectionForeColor = Color.Black;
            column.DefaultCellStyle.ForeColor = Color.Black;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column.Width = width;
            column.AutoSizeMode = width <= 0 ? DataGridViewAutoSizeColumnMode.Fill : DataGridViewAutoSizeColumnMode.None;
            column.SortMode = DataGridViewColumnSortMode.Programmatic;
            return column;
        }

        private void ImportExcel(string filePath)
        {
            this.bindingList.Clear();
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var configWorkbook = new XLWorkbook(stream))
                {
                    var worksheet = configWorkbook.Worksheets.Worksheet(1);

                    var excelCellLetterList = AddMatchExpression(new string[] { addressCellConfig, commentCellConfig, ioNameCellConfig, ignoreRowExpressionConfig });

                    uint index = this.startingRow;
                    while (true)
                    {
                        var excelCellValueDict = new Dictionary<string, string>();

                        bool allEmpty = true;
                        foreach (var cellLetter in excelCellLetterList)
                        {
                            var cellValue = worksheet.Cell(cellLetter + index).Value.ToString();
                            excelCellValueDict.Add(cellLetter, cellValue); //There should not be two equals cellLetter. If there are, somethign wrong in AddMatchExpression.

                            allEmpty &= string.IsNullOrEmpty(cellValue);
                        }
                        index++;

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

                        var address = this.addressCellConfig;
                        var ioName = this.ioNameCellConfig;
                        var comment = this.commentCellConfig;
                        foreach (var entry in excelCellValueDict)
                        {
                            address = address.Replace(ROW_SPECIAL_CHAR + entry.Key, entry.Value);
                            ioName = ioName.Replace(ROW_SPECIAL_CHAR + entry.Key, entry.Value);
                            comment = comment.Replace(ROW_SPECIAL_CHAR + entry.Key, entry.Value);
                        }

                        bindingList.Add(new ExcelImportData()
                        {
                            Address = address,
                            IOName = ioName,
                            Comment = comment,
                        });
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

                var dynExp = context.CompileDynamic(ignoreRowExpressionConfig.Replace(ROW_SPECIAL_CHAR, ""));
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