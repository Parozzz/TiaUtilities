﻿
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
using System.Drawing;
using System.ComponentModel;

namespace TiaXmlReader.Generation.IO.GenerationForm
{
    public partial class IOGenerationForm : Form
    {
        private const int MERKER_ADDRESS_COLUMN_SIZE = 80;

        private readonly JavascriptErrorReportThread jsErrorHandlingThread;
        private readonly TimedSaveHandler autoSaveHandler;
        private readonly IOGenerationSettings settings;

        private readonly GridSettings gridSettings;
        private readonly GridHandler<IOConfiguration, IOData> ioGridHandler;
        private readonly GridHandler<IOConfiguration, IOSuggestion> suggestionGridHandler;

        private readonly IOGenerationFormConfigHandler configHandler;

        //private readonly List<string> variableSuggestionList;
        private readonly SuggestionTextBoxColumn variableAddressColumn;

        private IOGenerationExcelImportSettings ExcelImportConfig { get => settings.ExcelImportConfiguration; }
        private IOConfiguration IOConfig { get => settings.IOConfiguration; }


        private string lastFilePath;

        public IOGenerationForm(JavascriptErrorReportThread jsErrorHandlingThread, TimedSaveHandler autoSaveHandler, IOGenerationSettings settings, GridSettings gridSettings)
        {
            InitializeComponent();

            this.jsErrorHandlingThread = jsErrorHandlingThread;
            this.autoSaveHandler = autoSaveHandler;
            this.settings = settings;
            this.gridSettings = gridSettings;

            this.ioGridHandler = new GridHandler<IOConfiguration, IOData>(jsErrorHandlingThread, gridSettings, IOConfig, new IOGenerationComparer())
            {
                RowCount = 2999
            };

            this.suggestionGridHandler = new GridHandler<IOConfiguration, IOSuggestion>(jsErrorHandlingThread, gridSettings, IOConfig)
            {
                RowCount = 1999,
            };

            this.configHandler = new IOGenerationFormConfigHandler(this, this.IOConfig, this.ioGridHandler);
            this.variableAddressColumn = new SuggestionTextBoxColumn();

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

            if (this.ioGridHandler.ProcessCmdKey(ref msg, keyData) || this.suggestionGridHandler.ProcessCmdKey(ref msg, keyData))
            {
                return true;
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void Init()
        {
            this.GridsSplitContainer.Panel1.Controls.Add(this.suggestionGridHandler.DataGridView);
            this.GridsSplitContainer.Panel2.Controls.Add(this.ioGridHandler.DataGridView);

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
                        var ioDataList = new List<IOData>(this.ioGridHandler.DataSource.GetNotEmptyClonedDataDict().Keys); //Return CLONED data, otherwise operations on the xml generation will affect the table!

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

                    var firstEmptyIndexList = this.ioGridHandler.DataSource.GetFirstEmptyRowIndexes(ioDataList.Count);

                    var dataDict = new Dictionary<int, IOData>();
                    for (int i = 0; i < firstEmptyIndexList.Count; i++)
                    {
                        dataDict.Add(firstEmptyIndexList[i], ioDataList[i]);
                    }
                    this.ioGridHandler.ChangeMultipleRows(dataDict);
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
                    this.suggestionGridHandler.DataSource.InitializeData(this.suggestionGridHandler.RowCount);

                    foreach (var filePath in fileDialog.FileNames)
                    {
                        var extension = Path.GetExtension(filePath);

                        var xmlNodeConfiguration = SimaticMLParser.ParseFile(filePath);
                        if (xmlNodeConfiguration is BlockGlobalDB globalDB)
                        {
                            var suggestionEnumerable = globalDB.GetAllMemberAddress().Select(v => new IOSuggestion() { Value = v });
                            this.suggestionGridHandler.AddData(suggestionEnumerable);

                        }
                        else if (xmlNodeConfiguration is XMLTagTable tagTable)
                        {
                            var suggestionEnumerable = tagTable.GetTags().Values.Select(t => t.GetTagName())
                                                                                .Select(n => new IOSuggestion() { Value = n });
                            this.suggestionGridHandler.AddData(suggestionEnumerable);
                        }
                        else
                        {
                            InformationBox.Show("The selected block is NOT a GlobalDB or file is invalid.", "Invalid imported xml", icon: InformationBoxIcon.Exclamation);
                        }

                        UpdateSuggestionColors();
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
            this.ioGridHandler.SetDragPreviewAction(data => { IOGenerationUtils.DragPreview(data, this.ioGridHandler); });
            this.ioGridHandler.SetDragMouseUpAction(data => { IOGenerationUtils.DragMouseUp(data, this.ioGridHandler); });

            this.suggestionGridHandler.SetDragPreviewAction(data => { GridUtils.DragPreview(data, this.suggestionGridHandler); });
            this.suggestionGridHandler.SetDragMouseUpAction(data => { GridUtils.DragMouseUp(data, this.suggestionGridHandler); });
            #endregion
            //Column initialization before gridHandler.Init()
            #region COLUMNS
            var addressColumn = this.ioGridHandler.AddTextBoxColumn(IOData.ADDRESS, 65);
            addressColumn.MaxInputLength = 10;

            this.ioGridHandler.AddTextBoxColumn(IOData.IO_NAME, 110);
            this.ioGridHandler.AddCustomColumn(this.variableAddressColumn, IOData.VARIABLE, 200);
            this.variableAddressColumn.SetGetItemsFunc(() =>
            {
                var ioVariableEnumerable = this.ioGridHandler.DataSource.GetNotEmptyDataDict().Keys.Select(i => i.Variable);
                var suggestionEnumerable = this.suggestionGridHandler.DataSource.GetNotEmptyDataDict();

                var notAddedSuggestionEnumerable = suggestionEnumerable.Select(k => k.Key.Value).Except(ioVariableEnumerable).ToArray();
                return notAddedSuggestionEnumerable;
            });

            this.ioGridHandler.AddTextBoxColumn(IOData.MERKER_ADDRESS, MERKER_ADDRESS_COLUMN_SIZE);
            this.ioGridHandler.AddTextBoxColumn(IOData.COMMENT, 0);

            this.suggestionGridHandler.AddTextBoxColumn(IOSuggestion.VALUE, 0);
            #endregion


            this.ioGridHandler?.Init();
            this.suggestionGridHandler?.Init();
            this.configHandler?.Init();

            this.ioGridHandler.Events.CellChange += (sender, args) =>
            {
                if (args.CellChangeList.Where(c => c.ColumnIndex == IOData.VARIABLE).Any())
                {
                    UpdateSuggestionColors();
                }

                if (args.CellChangeList.Where(c => c.ColumnIndex == IOData.ADDRESS || c.ColumnIndex == IOData.IO_NAME).Any())
                {
                    UpdateDuplicatedIOValues();
                }
            };
            this.suggestionGridHandler.Events.CellChange += (sender, args) => UpdateSuggestionColors();

            this.ioGridHandler.Events.ScriptShowVariable += (sender, args) => args.VariableList.Add("suggestions [array]");
            this.ioGridHandler.Events.ScriptAddVariables += (sender, args) => args.VariableDict.Add("suggestions", this.suggestionGridHandler.DataSource.GetNotEmptyData().Select(s => s.Value).ToArray());

            #region JS_SCRIPT
            this.ioGridHandler.Script.SetReadScriptFunc(() => settings.JSScript);
            this.ioGridHandler.Script.SetWriteScriptAction(str => settings.JSScript = str);

            this.suggestionGridHandler.Script.SetReadScriptFunc(() => settings.SuggestionJSScript);
            this.suggestionGridHandler.Script.SetWriteScriptAction(str => settings.SuggestionJSScript = str);
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
            foreach (var entry in suggestionGridHandler.DataSource.GetNotEmptyDataDict())
            {
                projectSave.SuggestionRowDict.Add(entry.Value, entry.Key);
            }

            foreach (var entry in ioGridHandler.DataSource.GetNotEmptyDataDict())
            {
                projectSave.RowDict.Add(entry.Value, entry.Key);
            }

            var saveOK = projectSave.Save(ref lastFilePath, saveAs || !File.Exists(lastFilePath));
            this.Text = this.Name + (saveOK ? ". Project File: " + lastFilePath : "");
        }

        public void ProjectLoad()
        {
            var loadedProjectSave = IOGenerationProjectSave.Load(ref lastFilePath);
            if (loadedProjectSave != null)
            {
                this.ioGridHandler.DataGridView.SuspendLayout();
                this.suggestionGridHandler.DataGridView.SuspendLayout();

                this.ioGridHandler.DataSource.Clear();
                this.suggestionGridHandler.DataSource.Clear();

                foreach (var entry in loadedProjectSave.RowDict)
                {
                    var rowIndex = entry.Key;
                    var data = entry.Value;
                    if (rowIndex >= 0 && rowIndex <= this.ioGridHandler.RowCount)
                    {
                        this.ioGridHandler.DataHandler.CopyValues(data, this.ioGridHandler.DataSource[rowIndex]);
                    }
                }

                foreach (var entry in loadedProjectSave.SuggestionRowDict)
                {
                    var rowIndex = entry.Key;
                    var data = entry.Value;
                    if (rowIndex >= 0 && rowIndex <= this.suggestionGridHandler.RowCount)
                    {
                        this.suggestionGridHandler.DataHandler.CopyValues(data, this.suggestionGridHandler.DataSource[rowIndex]);
                    }
                }

                this.UpdateDuplicatedIOValues();
                this.UpdateSuggestionColors();

                this.ioGridHandler.DataGridView.Refresh();
                this.suggestionGridHandler.DataGridView.Refresh();

                this.ioGridHandler.DataGridView.ResumeLayout();
                this.suggestionGridHandler.DataGridView.ResumeLayout();

                this.Text = this.Name + ". Project File: " + lastFilePath;
            }
        }

        private void UpdateDuplicatedIOValues()
        {
            this.ioGridHandler.DataGridView.SuspendLayout();

            foreach (DataGridViewRow row in this.ioGridHandler.DataGridView.Rows)
            {
                var addressCell = row.Cells[IOData.ADDRESS.ColumnIndex];
                addressCell.ToolTipText = string.Empty;
                addressCell.Style.BackColor = SystemColors.ControlLightLight;
                addressCell.Style.SelectionBackColor = Color.LightGray;

                var ioNameCell = row.Cells[IOData.IO_NAME.ColumnIndex];
                ioNameCell.ToolTipText = string.Empty;
                ioNameCell.Style.BackColor = SystemColors.ControlLightLight;
                ioNameCell.Style.SelectionBackColor = Color.LightGray;
            }

            var dataDict = this.ioGridHandler.DataSource.GetNotEmptyDataDict();

            var multipleIONameGroupingList = dataDict.Where(x => !string.IsNullOrEmpty(x.Key.IOName)).GroupBy(x => x.Key.IOName).Where(g => g.Count() > 1).ToList();
            foreach (var grouping in multipleIONameGroupingList)
            {
                //+1 to row because rows index start from 0.
                var tooltipText = grouping
                    .Select(x => (x.Value + 1) + ") " + x.Key.Address + " - " + x.Key.Comment)
                    .Aggregate((a, b) => a + '\n' + b);

                foreach (var entry in grouping)
                {
                    var rowIndex = entry.Value;

                    var ioNameCell = this.ioGridHandler.DataGridView.Rows[rowIndex].Cells[IOData.IO_NAME.ColumnIndex];
                    ioNameCell.ToolTipText = tooltipText;
                    ioNameCell.Style.BackColor = ControlPaint.LightLight(Color.Orange);
                }
            }

            var multipleAddressGroupingList = dataDict.Where(x => !string.IsNullOrEmpty(x.Key.Address)).GroupBy(x => x.Key.Address).Where(g => g.Count() > 1).ToList();
            foreach (var grouping in multipleAddressGroupingList)
            {
                //+1 to row because rows index start from 0.
                var tooltipText = grouping
                    .Select(x => (x.Value + 1) + ") " + x.Key.Address + " - " + x.Key.Comment)
                    .Aggregate((a, b) => a + '\n' + b);

                foreach (var entry in grouping)
                {
                    var rowIndex = entry.Value;

                    var addressCell = this.ioGridHandler.DataGridView.Rows[rowIndex].Cells[IOData.ADDRESS.ColumnIndex];
                    addressCell.ToolTipText = tooltipText;
                    addressCell.Style.BackColor = ControlPaint.LightLight(Color.Orange);
                }
            }

            this.ioGridHandler.DataGridView.ResumeLayout();
        }

        private void UpdateSuggestionColors()
        {
            this.suggestionGridHandler.DataGridView.SuspendLayout();

            foreach (DataGridViewRow row in this.suggestionGridHandler.DataGridView.Rows)
            {
                var cell = row.Cells[IOSuggestion.VALUE.ColumnIndex];
                cell.ToolTipText = "";
                cell.Style.BackColor = SystemColors.ControlLightLight;
                cell.Style.SelectionBackColor = Color.LightGray;
            }

            var ioDataDict = ioGridHandler.DataSource.GetNotEmptyDataDict();

            var suggestionDict = suggestionGridHandler.DataSource.GetNotEmptyDataDict();
            foreach (var entry in suggestionDict)
            {
                var suggestion = entry.Key;
                var row = entry.Value;

                var foundDataEnumerable = ioDataDict.Where(d => d.Key.Variable == suggestion.Value);
                if (foundDataEnumerable.Any())
                {
                    //The suggestion could be used multiple times! So i will aggregate all the found data.
                    var tooltipText = foundDataEnumerable
                        .Select(x => (x.Value + 1) + ") " + x.Key.Address + " - " + x.Key.IOName + " - " + x.Key.Comment)
                        .Aggregate((a, b) => a + '\n' + b);

                    var cell = this.suggestionGridHandler.DataGridView.Rows[row].Cells[IOSuggestion.VALUE.ColumnIndex];
                    cell.ToolTipText = tooltipText;
                    cell.Style.BackColor = cell.Style.SelectionBackColor = Color.LightGreen;
                }
            }

            this.suggestionGridHandler.DataGridView.ResumeLayout();
        }

        private void UpdateConfigPanel()
        {
            this.configButtonPanel.SuspendLayout();

            this.configButtonPanel.Controls.Clear();

            this.configButtonPanel.Controls.Add(fcConfigButton);
            if (IOMemoryTypeEnum.DB.Equals(memoryTypeComboBox.SelectedValue))
            {
                this.configButtonPanel.Controls.Add(dbConfigButton);
                this.ioGridHandler.HideColumn(IOData.MERKER_ADDRESS);
            }
            else if (IOMemoryTypeEnum.MERKER.Equals(memoryTypeComboBox.SelectedValue))
            {
                this.configButtonPanel.Controls.Add(variableTableConfigButton);
                this.ioGridHandler.ShowColumn(IOData.MERKER_ADDRESS);
            }
            this.configButtonPanel.Controls.Add(ioTableConfigButton);
            this.configButtonPanel.Controls.Add(segmentNameConfigButton);

            this.ioGridHandler.InitColumns();
            this.configButtonPanel.ResumeLayout();

            this.UpdateSuggestionColors();
            this.UpdateDuplicatedIOValues();
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