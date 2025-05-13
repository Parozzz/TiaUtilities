using InfoBox;
using Microsoft.WindowsAPICodePack.Dialogs;
using SimaticML.API;
using SimaticML.Blocks;
using SimaticML.TagTable;
using TiaUtilities.Generation.GenModules;
using TiaUtilities.Generation.GenModules.IO.ExcelImporter;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.IO.Module.ExcelImporter;
using TiaUtilities.Generation.IO.Module.Tab;
using TiaUtilities.Javascript.ErrorReporting;
using TiaUtilities.Languages;
using TiaXmlReader;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Languages;
using TiaXmlReader.Utility.Extensions;

namespace TiaUtilities.Generation.IO.Module
{
    public class IOGenModule : IGenModule
    {
        private record GenTabRowRecord(IOGenTab GenTab, IOData IOData, int Row);

        private readonly GridBindContainer gridBindContainer;

        private readonly IOMainConfiguration mainConfig;
        private readonly IOExcelImportConfiguration excelImportConfig;

        private readonly GridDataPreviewer<IOSuggestionData> suggestionPreviewer;
        private readonly GridHandler<IOSuggestionData> suggestionGridHandler;

        private readonly IOGenControl control;

        private readonly List<IOGenTab> genTabList;

        public IOGenModule(ErrorReportThread errorThread)
        {
            this.gridBindContainer = new(errorThread);

            this.mainConfig = new();
            GenUtils.CopyJsonFieldsAndProperties(MainForm.Settings.PresetIOMainConfiguration, this.mainConfig);

            this.excelImportConfig = new();
            GenUtils.CopyJsonFieldsAndProperties(MainForm.Settings.PresetIOExcelImportConfiguration, this.excelImportConfig);

            this.suggestionPreviewer = new();
            this.suggestionGridHandler = new(MainForm.Settings.GridSettings, this.gridBindContainer, suggestionPreviewer, new()) { RowCount = 1999 };

            this.control = new(suggestionGridHandler.DataGridView);

            this.genTabList = [];
        }

        public void Init(GenModuleForm form)
        {
            #region IMPORT_EXPORT_MENU_ITEMS
            ToolStripMenuItem importExcelMenuItem = new(Locale.IO_GEN_FORM_IMPEXP_IMPORT_EXCEL);
            importExcelMenuItem.Click += (sender, args) =>
            {
                IOGenerationExcelImportForm excelImportForm = new(MainForm.Settings.GridSettings, this.gridBindContainer, this.excelImportConfig);

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

                    var selectedTab = this.control.gridsTabControl.SelectedTab;
                    if (selectedTab != null && selectedTab.Tag is IOGenTab genTab)
                    {
                        var gridHandler = genTab.GridHandler;

                        var firstEmptyIndexList = gridHandler.DataSource.GetFirstEmptyRowIndexes(ioDataList.Count);

                        var dataDict = new Dictionary<int, IOData>();
                        for (int i = 0; i < firstEmptyIndexList.Count; i++)
                        {
                            dataDict.Add(firstEmptyIndexList[i], ioDataList[i]);
                        }
                        gridHandler.ChangeMultipleRows(dataDict);
                    }
                }
            };
            form.importExportMenuItem.DropDownItems.Add(importExcelMenuItem);

            ToolStripMenuItem importSuggestionsMenuItem = new(Locale.IO_GEN_FORM_IMPEXP_IMPORT_SUGGESTION);
            importSuggestionsMenuItem.Click += (sender, args) =>
            {
                var fileDialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = false,
                    EnsurePathExists = true,
                    EnsureValidNames = true,
                    Multiselect = true,
                    DefaultExtension = ".xml",
                    Filters = { new CommonFileDialogFilter("XML Files", "*.xml") }
                };

                if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    suggestionGridHandler.DataSource.InitializeData(suggestionGridHandler.RowCount);

                    foreach (var filePath in fileDialog.FileNames)
                    {
                        var xmlNodeConfiguration = SimaticMLAPI.ParseFile(filePath);
                        if (xmlNodeConfiguration is BlockGlobalDB globalDB)
                        {
                            var suggestionEnumerable = globalDB.GetAllMemberAddress().Select(v => new IOSuggestionData() { Value = v });
                            suggestionGridHandler.AddData(suggestionEnumerable);

                        }
                        else if (xmlNodeConfiguration is XMLTagTable tagTable)
                        {
                            var suggestionEnumerable = tagTable.GetTags().Values.Select(t => t.TagName)
                                                                                .Select(n => new IOSuggestionData() { Value = n });
                            suggestionGridHandler.AddData(suggestionEnumerable);
                        }
                        else
                        {
                            InformationBox.Show("The selected block is NOT a GlobalDB or file is invalid.", "Invalid imported xml", icon: InformationBoxIcon.Exclamation);
                        }
                    }

                    this.UpdateSuggestionColors();
                }
            };
            form.importExportMenuItem.DropDownItems.Add(importSuggestionsMenuItem);

            ToolStripMenuItem importAddressMenuItem = new(Locale.IO_GEN_FORM_IMPEXP_IMPORT_IO);
            importAddressMenuItem.Click += (sender, args) =>
            {
                if (this.control.gridsTabControl.SelectedTab?.Tag is not IOGenTab ioTab)
                {
                    return;
                }

                var fileDialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = false,
                    EnsurePathExists = true,
                    EnsureValidNames = true,
                    Multiselect = true,
                    DefaultExtension = ".xml",
                    Filters = { new CommonFileDialogFilter("XML Files", "*.xml") }
                };

                if (fileDialog.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                foreach (var filePath in fileDialog.FileNames)
                {
                    var xmlNodeConfiguration = SimaticMLAPI.ParseFile(filePath);
                    if (xmlNodeConfiguration is XMLTagTable tagTable)
                    {
                        var tags = tagTable.GetTags().Values;

                        var emptyIndexList = ioTab.GridHandler.DataSource.GetFirstEmptyRowIndexes(tags.Count);

                        List<GridCellChange> cellChangeList = [];

                        int i = 0;
                        foreach (var tag in tagTable.GetTags().Values)
                        {
                            if (i >= emptyIndexList.Count)
                            {
                                break;
                            }

                            var index = emptyIndexList[i++];

                            var address = tag.GetLogicalAddress().Replace("%", "");
                            var ioName = tag.TagName;
                            var comment = tag.Comment[LocaleVariables.CULTURE] ?? tag.Comment.GetDictionary().Values.FirstOrElse(() => "");

                            cellChangeList.AddRange(
                                ioTab.GridHandler.DataHandler.CreateCellChanges(index, new() { Address = address, IOName = ioName, Comment = comment })
                            );
                        }

                        ioTab.GridHandler.ChangeCells(cellChangeList);
                    }
                    else
                    {
                        InformationBox.Show("The selected block is NOT a Tag Table or file is invalid.", "Invalid imported xml", icon: InformationBoxIcon.Exclamation);
                    }
                }

                this.UpdateSuggestionColors();
            };
            form.importExportMenuItem.DropDownItems.Add(importAddressMenuItem);
            #endregion

            #region DRAG
            suggestionGridHandler.Events.ExcelDragPreview += (sender, args) => GridUtils.DragPreview(args, suggestionGridHandler);
            suggestionGridHandler.Events.ExcelDragDone += (sender, args) => GridUtils.DragDone(args, suggestionGridHandler);
            #endregion
            //Column initialization before gridHandler.Init()
            #region COLUMNS
            suggestionGridHandler.AddTextBoxColumn(IOSuggestionData.VALUE, 0);
            #endregion

            this.gridBindContainer.Init(form);

            this.suggestionGridHandler.Init();
            this.control.BindConfig(mainConfig);

            #region SUGGESTION_GRIDS_EVENTS
            this.suggestionGridHandler.DataGridView.CellToolTipTextNeeded += (sender, args) =>
            {
                if (args.RowIndex < 0 || args.RowIndex > suggestionGridHandler.RowCount)
                {
                    return;
                }

                List<GenTabRowRecord> rows = [];

                args.ToolTipText = "";

                foreach (var genTab in genTabList)
                {
                    var dict = genTab.GridHandler.DataSource.GetNotEmptyDataDict();
                    foreach (var entry in dict)
                    {
                        rows.Add(new(genTab, entry.Key, entry.Value));
                    }
                }

                var suggestion = suggestionGridHandler.DataSource[args.RowIndex];
                var foundData = rows.Where(d => d.IOData.Variable == suggestion.Value);
                if (foundData.Any())
                {
                    //The suggestion could be used multiple times! So i will aggregate all the found data.
                    args.ToolTipText += foundData
                        .Select(x => $"{x.GenTab.TabPage.Text} {x.Row + 1}) {x.IOData.Address} - {x.IOData.IOName} - {x.IOData.Comment}")
                        .Aggregate((a, b) => a + Environment.NewLine + b); ;
                }
            };

            suggestionGridHandler.Events.CellChange += (sender, args) => UpdateSuggestionColors();
            #endregion

            #region PREVIEW
            this.suggestionPreviewer.Function = (column, ioData) => null;
            #endregion

            this.control.gridsTabControl.TabPreAdded += (sender, args) => TabCreation(args.TabPage);
            this.control.gridsTabControl.TabPreRemoved += (sender, args) =>
            {
                if (args.TabPage.Tag is IOGenTab ioGenTab)
                {
                    genTabList.Remove(ioGenTab);
                }
            };
            this.control.gridsTabControl.Selected += (sender, args) =>
            {
                if (args.TabPage?.Tag is IOGenTab tab)
                {
                    tab.Selected();
                }
            };

            form.Shown += (sender, args) =>
            {
                suggestionGridHandler.DataGridView.AutoResizeColumnHeadersHeight();
                if (this.control.gridsTabControl.TabCount == 0)
                { //Check required because Load could be called before form is shown!
                    TabPage tabPage = new();
                    TabCreation(tabPage);
                    this.control.gridsTabControl.TabPages.Add(tabPage);
                }
            };
        }

        private void TabCreation(TabPage tabPage, IOGenTabSave? save = null)
        {
            tabPage.Text = save?.Name ?? "IOGen";

            IOGenTab ioGenTab = new(MainForm.Settings.GridSettings, this.gridBindContainer, this, tabPage, this.mainConfig);
            genTabList.Add(ioGenTab);

            ioGenTab.Init();
            if (save != null)
            {
                ioGenTab.LoadSave(save);
            }
            tabPage.Tag = ioGenTab;

            tabPage.Controls.Add(ioGenTab.TabControl);
        }

        public void Clear()
        {
            this.genTabList.Clear();
            this.control.gridsTabControl.TabPages.Clear();
        }

        public bool IsDirty() => mainConfig.IsDirty() || suggestionGridHandler.IsDirty() || genTabList.Any(x => x.IsDirty()) || this.gridBindContainer.IsDirty();
        public void Wash()
        {
            this.mainConfig.Wash();
            this.suggestionGridHandler.Wash();
            foreach (var tab in genTabList)
            {
                tab.Wash();
            }
            this.gridBindContainer.Wash();
        }

        public Control? GetControl()
        {
            return this.control;
        }

        public void ExportXML(string folderPath)
        {
            var ioXmlGenerator = new IOXmlGenerator(mainConfig);
            ioXmlGenerator.Init();

            foreach (var tab in genTabList)
            {
                var ioDataList = new List<IOData>(tab.GridHandler.DataSource.GetNotEmptyClonedDataDict().Keys); //Return CLONED data, otherwise operations on the xml generation will affect the table!
                ioXmlGenerator.GenerateAlias(tab.TabPage.Text, tab.Previewer, tab.TabConfig, ioDataList);
            }

            ioXmlGenerator.ExportXML(folderPath);
        }

        public object CreateSave()
        {
            IOGenSave save = new()
            {
                SuggestionGrid = this.suggestionGridHandler.CreateSave(),
                ScriptSave = this.gridBindContainer.GridScriptHandler.CreateSave()
            };

            GenUtils.CopyJsonFieldsAndProperties(mainConfig, save.MainConfig);
            GenUtils.CopyJsonFieldsAndProperties(excelImportConfig, save.ExcelImportConfiguration);

            foreach (var tab in genTabList)
            {
                var tabSave = tab.CreateSave();
                save.TabSaves.Add(tabSave);
            }

            return save;
        }

        public void LoadSave(object saveObject)
        {
            if (saveObject is not IOGenSave loadedSave)
            {
                return;
            }

            this.Clear();

            this.gridBindContainer.GridScriptHandler.LoadSave(loadedSave.ScriptSave);
            this.suggestionGridHandler.LoadSave(loadedSave.SuggestionGrid);

            GenUtils.CopyJsonFieldsAndProperties(loadedSave.MainConfig, mainConfig);
            GenUtils.CopyJsonFieldsAndProperties(loadedSave.ExcelImportConfiguration, excelImportConfig);

            foreach (var tabSave in loadedSave.TabSaves)
            {
                TabPage tabPage = new();
                TabCreation(tabPage, tabSave);
                this.control.gridsTabControl.TabPages.Add(tabPage);
            }

            UpdateSuggestionColors();
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var selectedTab = this.control.gridsTabControl.SelectedTab;
            if (selectedTab != null && selectedTab.Tag is IOGenTab ioGenTab)
            {
                return ioGenTab.ProcessCmdKey(ref msg, keyData);
            }

            return false;
        }

        public string GetFormLocalizatedName()
        {
            return Locale.IO_GEN_FORM_NAME;
        }

        public IEnumerable<string> GetSuggestions(bool filterAlreadyUsed = false)
        {
            IEnumerable<string> suggestions = suggestionGridHandler.DataSource.GetNotEmptyDataDict().Keys.Select(k => k.Value ?? "");
            if(filterAlreadyUsed)
            {
                foreach (var ioTab in this.genTabList)
                {
                    var tabVariables = ioTab.GridHandler.DataSource.GetNotEmptyData().Select(i => i.Variable?.ToLowerInvariant())
                                                                                     .Where(i => i != null);
                    suggestions = suggestions.Where(v => !tabVariables.Contains(v.ToLowerInvariant()));
                }
            }
            return suggestions;
        }

        public void UpdateSuggestionColors()
        {
            suggestionGridHandler.DataGridView.SuspendLayout();

            foreach (DataGridViewRow row in suggestionGridHandler.DataGridView.Rows)
            {
                var cell = row.Cells[IOSuggestionData.VALUE.ColumnIndex];
                cell.Style.BackColor = SystemColors.ControlLightLight;
                cell.Style.SelectionBackColor = Color.LightGray;
            }

            List<GenTabRowRecord> rows = [];

            foreach (var genTab in genTabList)
            {
                var dict = genTab.GridHandler.DataSource.GetNotEmptyDataDict();
                foreach (var entry in dict)
                {
                    rows.Add(new(genTab, entry.Key, entry.Value));
                }
            }

            var suggestionDict = suggestionGridHandler.DataSource.GetNotEmptyDataDict();
            foreach (var suggestionEntry in suggestionDict)
            {
                var suggestion = suggestionEntry.Key;
                var row = suggestionEntry.Value;

                var foundData = rows.Where(d => d.IOData.Variable == suggestion.Value);
                if (foundData.Any())
                {
                    var cell = suggestionGridHandler.DataGridView.Rows[row].Cells[IOSuggestionData.VALUE.ColumnIndex];
                    cell.Style.BackColor = cell.Style.SelectionBackColor = Color.LightGreen;
                }
            }

            suggestionGridHandler.DataGridView.ResumeLayout();
        }
    }
}

/*
        public void UpdateSuggestionColors()
        {
            this.suggestionGridHandler.DataGridView.SuspendLayout();

            foreach (DataGridViewRow row in this.suggestionGridHandler.DataGridView.Rows)
            {
                var cell = row.Cells[IOSuggestionData.VALUE.ColumnIndex];
                cell.ToolTipText = "";
                cell.Style.BackColor = SystemColors.ControlLightLight;
                cell.Style.SelectionBackColor = Color.LightGray;
            }

            List<UpdateSuggestionColorData> dataList = [];

            foreach (var genTab in genTabList)
            {
                var dict = genTab.GridHandler.DataSource.GetNotEmptyDataDict();
                foreach (var entry in dict)
                {
                    dataList.Add(new(genTab, entry.Key, entry.Value));
                }
            }

            var suggestionDict = suggestionGridHandler.DataSource.GetNotEmptyDataDict();
            foreach (var suggestionEntry in suggestionDict)
            {
                var suggestion = suggestionEntry.Key;
                var row = suggestionEntry.Value;

                var foundData = dataList.Where(d => d.IOData.Variable == suggestion.Value);
                if (foundData.Any())
                {
                    //The suggestion could be used multiple times! So i will aggregate all the found data.
                    var tooltipText = foundData
                        .Select(x => $"{x.GenTab.TabPage.Text} {x.Row + 1}) {x.IOData.Address}-{x.IOData.IOName}-{x.IOData.Comment}")
                        .Aggregate((a, b) => a + Environment.NewLine + b);

                    var cell = this.suggestionGridHandler.DataGridView.Rows[row].Cells[IOSuggestionData.VALUE.ColumnIndex];
                    cell.ToolTipText = tooltipText;
                    cell.Style.BackColor = cell.Style.SelectionBackColor = Color.LightGreen;
                }
            }

            this.suggestionGridHandler.DataGridView.ResumeLayout();
        } 
*/