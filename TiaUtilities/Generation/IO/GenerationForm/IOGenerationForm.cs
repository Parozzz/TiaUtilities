
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TiaXmlReader.Localization;
using TiaXmlReader.Utility;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.IO.GenerationForm.ExcelImporter;
using TiaXmlReader.AutoSave;
using TiaXmlReader.Javascript;
using TiaXmlReader.Generation.GridHandler.CustomColumns;
using TiaXmlReader.SimaticML;
using TiaXmlReader.SimaticML.Blocks;
using InfoBox;
using System.Diagnostics;
using TiaXmlReader.SimaticML.TagTable;
using System.IO;

namespace TiaXmlReader.Generation.IO.GenerationForm
{
    public partial class IOGenerationForm : Form
    {
        private const int MERKER_ADDRESS_COLUMN_SIZE = 80;

        private readonly JavascriptScriptErrorReportingThread jsErrorHandlingThread;
        private readonly TimedSaveHandler autoSaveHandler;
        private readonly IOGenerationSettings settings;
        private readonly GridSettings gridSettings;
        private readonly GridHandler<IOConfiguration, IOData> gridHandler;
        private readonly IOGenerationFormConfigHandler configHandler;

        private readonly List<string> variableSuggestionList;
        private readonly SuggestionTextBoxColumn variableAddressColumn;

        private IOGenerationExcelImportSettings ExcelImportConfig { get => settings.ExcelImportConfiguration; }
        private IOConfiguration IOConfig { get => settings.IOConfiguration; }


        private string lastFilePath;

        public IOGenerationForm(JavascriptScriptErrorReportingThread jsErrorHandlingThread, TimedSaveHandler autoSaveHandler, IOGenerationSettings settings, GridSettings gridSettings)
        {
            InitializeComponent();

            this.jsErrorHandlingThread = jsErrorHandlingThread;
            this.autoSaveHandler = autoSaveHandler;
            this.settings = settings;
            this.gridSettings = gridSettings;

            this.gridHandler = new GridHandler<IOConfiguration, IOData>(jsErrorHandlingThread, this.dataGridView, gridSettings, IOConfig, IOData.COLUMN_LIST, new IOGenerationComparer())
            {
                RowCount = 2999
            };

            this.configHandler = new IOGenerationFormConfigHandler(this, this.IOConfig, this.gridHandler);

            this.variableSuggestionList = new List<string>();
            this.variableAddressColumn = new SuggestionTextBoxColumn(() =>
            {
                var list = new List<string>(variableSuggestionList);
                foreach(var data in this.gridHandler.DataSource.GetNotEmptyDataDict().Keys)
                {
                    list.Remove(data.Variable);
                }
                return list.ToArray();
            });

            Init();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.S | Keys.Control:
                    this.ProjectSave();
                    return true; //Return required otherwise will write the letter.
                case Keys.L | Keys.Control:
                    this.ProjectLoad();
                    return true; //Return required otherwise will write the letter.
            }

            if(this.gridHandler.ProcessCmdKey(ref msg, keyData))
            {
                return true;
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void Init()
        {
            #region TopMenu
            this.saveToolStripMenuItem.Click += (object sender, EventArgs args) => { this.ProjectSave(); };
            this.saveAsToolStripMenuItem.Click += (object sender, EventArgs args) => { this.ProjectSave(true); };
            this.loadToolStripMenuItem.Click += (object sender, EventArgs args) => { this.ProjectLoad(); };
            this.exportXMLToolStripMenuItem.Click += (object sender, EventArgs args) =>
            {
                try
                {
                    var fileDialog = new CommonOpenFileDialog
                    {
                        IsFolderPicker = true,
                        EnsurePathExists = true,
                        EnsureValidNames = true,
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    };

                    if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        var ioDataList = new List<IOData>(this.gridHandler.DataSource.GetNotEmptyClonedDataDict().Keys); //Return CLONED data, otherwise operations on the xml generation will affect the table!

                        var ioXmlGenerator = new IOXmlGenerator(this.IOConfig, ioDataList);
                        ioXmlGenerator.GenerateBlocks();
                        ioXmlGenerator.ExportXML(fileDialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex);
                }
            };
            this.importExcelToolStripMenuItem.Click += (object sender, EventArgs args) =>
            {
                var excelImportForm = new IOGenerationExcelImportForm(this.jsErrorHandlingThread, this.ExcelImportConfig, this.gridSettings);

                var dialogResult = excelImportForm.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    var ioDataList = new List<IOData>();
                    foreach (var importData in excelImportForm.ImportDataEnumerable)
                    {
                        ioDataList.Add(new IOData()
                        {
                            Address = importData.Address,
                            IOName = importData.IOName,
                            Comment = importData.Comment
                        });
                    }

                    var firstEmptyIndexList = this.gridHandler.DataSource.GetFirstEmptyRowIndexes(ioDataList.Count);

                    var dataDict = new Dictionary<int, IOData>();
                    for (int i = 0; i < firstEmptyIndexList.Count; i++)
                    {
                        dataDict.Add(firstEmptyIndexList[i], ioDataList[i]);
                    }
                    this.gridHandler.ChangeMultipleRows(dataDict);
                }
            };

            this.importSuggestionsToolStripMenuItem.Click += (sender, args) =>
            {
                var fileDialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = false,
                    EnsurePathExists = true,
                    EnsureValidNames = true,
                    Multiselect = true,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    DefaultExtension = ".xml",
                    Filters = { new CommonFileDialogFilter("XML Files", "*.xml") }
                };

                if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    this.variableSuggestionList.Clear();

                    foreach(var filePath in fileDialog.FileNames)
                    {
                        var extension = Path.GetExtension(filePath);

                        var xmlNodeConfiguration = SimaticMLParser.ParseFile(filePath);
                        if (xmlNodeConfiguration is BlockGlobalDB globalDB)
                        {
                            var list = globalDB.GetAllMemberAddress();
                            this.variableSuggestionList.AddRange(list);
                        }
                        else if(xmlNodeConfiguration is XMLTagTable tagTable)
                        {
                            this.variableSuggestionList.AddRange(tagTable.GetTags().Values.Select(t => t.GetTagName()));
                        }
                        else
                        {
                            InformationBox.Show("The selected block is NOT a GlobalDB or file is invalid.", "Invalid imported xml", icon: InformationBoxIcon.Exclamation);
                        }
                    }
                }

            };

            this.preferencesToolStripMenuItem.Click += (object sender, EventArgs args) => this.gridSettings.ShowConfigForm(this);
            #endregion

            #region MemoryType ComboBox
            this.memoryTypeComboBox.DisplayMember = "Text";
            this.memoryTypeComboBox.ValueMember = "Value";

            var memoryTypeItems = new List<object>();
            foreach (IOMemoryTypeEnum memoryType in Enum.GetValues(typeof(IOMemoryTypeEnum)))
            {
                memoryTypeItems.Add(new { Text = memoryType.GetTranslation(), Value = memoryType });
            }
            this.memoryTypeComboBox.DataSource = memoryTypeItems;

            this.memoryTypeComboBox.SelectionChangeCommitted += (object sender, EventArgs args) => this.UpdateConfigPanel();
            #endregion

            #region GroupingType ComboBox
            this.groupingTypeComboBox.DisplayMember = "Text";
            this.groupingTypeComboBox.ValueMember = "Value";

            var gropingTypeItems = new List<object>();
            foreach (IOGroupingTypeEnum groupingType in Enum.GetValues(typeof(IOGroupingTypeEnum)))
            {
                gropingTypeItems.Add(new { Text = groupingType.GetTranslation(), Value = groupingType });
            }
            this.groupingTypeComboBox.DataSource = gropingTypeItems;
            #endregion

            #region DRAG
            this.gridHandler.SetDragPreviewAction(data => { IOGenerationUtils.DragPreview(data, this.gridHandler); });
            this.gridHandler.SetDragMouseUpAction(data => { IOGenerationUtils.DragMouseUp(data, this.gridHandler); });
            #endregion
            //Column initialization before gridHandler.Init()
            #region COLUMNS
            var addressColumn = this.gridHandler.AddTextBoxColumn(IOData.ADDRESS, 65);
            addressColumn.MaxInputLength = 10;

            this.gridHandler.AddTextBoxColumn(IOData.IO_NAME, 110);
            this.gridHandler.AddCustomColumn(this.variableAddressColumn, IOData.VARIABLE, 200);
            this.gridHandler.AddTextBoxColumn(IOData.MERKER_ADDRESS, MERKER_ADDRESS_COLUMN_SIZE);
            this.gridHandler.AddTextBoxColumn(IOData.COMMENT, 0);
            #endregion

            this.gridHandler?.Init();
            this.configHandler?.Init();

            #region JS_SCRIPT
            this.gridHandler.TableScript.SetReadScriptFunc(() => settings.JSScript);
            this.gridHandler.TableScript.SetWriteScriptAction((str) => settings.JSScript = str);
            #endregion

            #region AUTO_SAVE
            void eventHandler(object sender, EventArgs args)
            {
                if (File.Exists(this.lastFilePath))
                {
                    this.ProjectSave();
                }
            }
            this.Shown += (sender, args) => autoSaveHandler.AddTickEventHandler(eventHandler);
            this.FormClosed += (sender, args) => autoSaveHandler.RemoveTickEventHandler(eventHandler);
            #endregion

            UpdateConfigPanel();
        }

        public void ProjectSave(bool saveAs = false)
        {
            var projectSave = new IOGenerationProjectSave();
            foreach (var entry in gridHandler.DataSource.GetNotEmptyDataDict())
            {
                projectSave.RowDict.Add(entry.Value, entry.Key);
            }
            
            var saveOK = projectSave.Save(ref lastFilePath, saveAs || !File.Exists(lastFilePath));
            if (!saveOK)
            {
                this.Text = this.Name;
                return;
            }

            this.Text = this.Name + ". Project File: " + lastFilePath;
        }

        public void ProjectLoad()
        {
            var loadedProjectSave = IOGenerationProjectSave.Load(ref lastFilePath);
            if (loadedProjectSave != null)
            {
                this.dataGridView.SuspendLayout();
                this.gridHandler.DataSource.InitializeData(this.gridHandler.RowCount);

                foreach (var entry in loadedProjectSave.RowDict)
                {
                    var rowIndex = entry.Key;
                    var data = entry.Value;
                    if (rowIndex >= 0 && rowIndex <= this.gridHandler.RowCount)
                    {
                        this.gridHandler.DataHandler.CopyValues(data, this.gridHandler.DataSource[rowIndex]);
                    }
                }

                this.dataGridView.Refresh();
                this.dataGridView.ResumeLayout();

                this.Text = this.Name + ". Project File: " + lastFilePath;
            }
        }
        private void UpdateConfigPanel()
        {
            this.configButtonPanel.SuspendLayout();

            this.configButtonPanel.Controls.Clear();

            this.configButtonPanel.Controls.Add(fcConfigButton);

            var merkerAddressColumnInfo = this.gridHandler.GetColumnInfo(IOData.MERKER_ADDRESS);
            merkerAddressColumnInfo.Visible = false;
            if (IOMemoryTypeEnum.DB.Equals(memoryTypeComboBox.SelectedValue))
            {
                this.configButtonPanel.Controls.Add(dbConfigButton);
            }
            else if (IOMemoryTypeEnum.MERKER.Equals(memoryTypeComboBox.SelectedValue))
            {
                this.configButtonPanel.Controls.Add(variableTableConfigButton);
                merkerAddressColumnInfo.Visible = true;
            }
            this.configButtonPanel.Controls.Add(ioTableConfigButton);
            this.configButtonPanel.Controls.Add(segmentNameConfigButton);

            this.gridHandler.InitColumns();

            this.configButtonPanel.ResumeLayout();
        }
    }
}

/*
this.dataGridView.SortCompare += (sender, args) =>
{
    if (args.Column.Index == 0)
    {
        //This is required to avoid the values to go bottom and top when sorting. I want the empty lines always at the bottom!.
        var sortOrderMultiplier = dataGridView.SortOrder == SortOrder.Ascending ? -1 : 1;

        var cell1Address = SimaticTagAddress.FromAddress(args.CellValue1?.ToString());
        var cell2Address = SimaticTagAddress.FromAddress(args.CellValue2?.ToString());
        if (cell1Address == null)
        {
            args.SortResult = -1 * sortOrderMultiplier;
        }
        else if (cell2Address == null)
        {
            args.SortResult = 1 * sortOrderMultiplier;
        }
        else
        {
            args.SortResult = cell1Address.CompareTo(cell2Address);
        }

        args.Handled = true;
    }
};

dataGridView.MouseDoubleClick += (sender, args) =>
{
    var hitTest = dataGridView.HitTest(args.X, args.Y);
    if (hitTest.Type == DataGridViewHitTestType.Cell)
    {
        var cell = dataGridView.Rows[hitTest.RowIndex].Cells[hitTest.ColumnIndex];
        dataGridView.CurrentCell = cell;
        dataGridView.BeginEdit(true);
    }
};

dataGridView.CellPainting += (sender, args) =>
{
    if (args.RowIndex >= 0 && args.ColumnIndex >= 0)
    {
        if (dataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex].Selected == true)
        {
            args.Paint(args.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
            using (Pen p = new Pen(Color.Red, 3))
            {
                Rectangle rect = args.CellBounds;
                rect.Width -= 2;
                rect.Height -= 2;
                args.Graphics.DrawRectangle(p, rect);
            }
            args.Handled = true;
        }
    }

    e.PaintBackground(e.CellBounds, true);  
    e.PaintContent(e.CellBounds);  
    using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 0, 0)))  
    {  
        Point[] pt = new Point[] { new Point(e.CellBounds.Right - 1, e.CellBounds.Bottom - 10), new Point(e.CellBounds.Right - 1, e.CellBounds.Bottom - 1), new Point(e.CellBounds.Right - 10, e.CellBounds.Bottom - 1) };  
        e.Graphics.FillPolygon(brush, pt);  
    }  
    e.Handled = true;  
};
*/