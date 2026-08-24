using InfoBox;
using Microsoft.WindowsAPICodePack.Dialogs;
using SimaticML.API;
using SimaticML.Blocks;
using SimaticML.TagTable;
using TiaUtilities.Configuration;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.IO.Configurations;
using TiaUtilities.Generation.IO.Data;
using TiaUtilities.Generation.IO.Module.ExcelImporter;
using TiaUtilities.Generation.IO.Module.Tab;
using TiaUtilities.Generation.IO.Xml;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Generation.TextsEditor;
using TiaUtilities.Languages;
using TiaUtilities.Resources;
using TiaUtilities.SettingsNew;
using TiaUtilities.SettingsNew.Bindings;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.Generation.IO.Module
{
    public class IOSuggesttionRowComparare : IGridRowComparer<IOSuggestionData>
    {
        public bool CanSortColumn(int column) => true;

        public int Compare(IOSuggestionData? x, IOSuggestionData? y)
        {
            if (String.IsNullOrWhiteSpace(x?.Value) && !String.IsNullOrWhiteSpace(y?.Value))
            {
                return 1;
            }
            else if (String.IsNullOrWhiteSpace(x?.Value) && String.IsNullOrWhiteSpace(y?.Value))
            {
                return 0;
            }
            else if (!String.IsNullOrWhiteSpace(x?.Value) && String.IsNullOrWhiteSpace(y?.Value))
            {
                return -1;
            }
            else
            {
                return string.Compare(x?.Value ?? "", y?.Value ?? "");
            }
        }

        public SortOrder GetSortOrder() => SortOrder.Ascending;

        public void SetSortedColumn(int column) { }

        public void SetSortOrder(SortOrder sortOrder) { }
    }

    public class IOGenModule : IGenModule
    {
        private record GenTabRowRecord(IOGenTab GenTab, IOData IOData, int Row);

        private readonly MultiGridOperationHandler multiGrid;

        private readonly IOMainConfiguration mainConfig;
        private readonly IOExcelImportConfiguration excelImportConfig;

        private readonly GridDataPreviewer<IOSuggestionData> suggestionPreviewer;
        private readonly GridHandler<IOSuggestionData> suggestionGridHandler;

        private readonly IOGenControl control;

        private readonly List<IOGenTab> ioTabList;

        public SettingsBindings SettingsBindings { get; init; }
        private readonly SettingsFormCache settingsFormCache;

        public IOGenModule()
        {
            this.multiGrid = new();

            this.mainConfig = new();
            GenUtils.CopyJsonFieldsAndProperties(MainForm.Settings.PresetIOMainConfiguration, this.mainConfig);

            this.excelImportConfig = new();
            GenUtils.CopyJsonFieldsAndProperties(MainForm.Settings.PresetIOExcelImportConfiguration, this.excelImportConfig);

            this.suggestionPreviewer = new();
            this.suggestionGridHandler = new(MainForm.Settings.GridSettings, this.multiGrid, suggestionPreviewer, new(), new IOSuggesttionRowComparare()) { InitializeRowCount = 9999 };

            this.control = new(suggestionGridHandler.GetControl());

            this.ioTabList = [];
            this.SettingsBindings = new();
            this.settingsFormCache = new(this.SettingsBindings, this.control);
        }

        public void Init(GenModuleForm form)
        {
            #region TOP_BUTTONS_STRIP
            this.control.setupButton.Click += (sender, args) => this.ToggleSettingsFormVisibility();
            #endregion

            #region IMPORT_EXPORT_MENU_ITEMS
            ToolStripMenuItem importExcelMenuItem = new(Locale.IO_GEN_FORM_IMPEXP_IMPORT_EXCEL);
            importExcelMenuItem.Click += (sender, args) =>
            {
                IOGenerationExcelImportForm excelImportForm = new(MainForm.Settings.GridSettings, this.multiGrid, this.excelImportConfig);

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

                    var selectedTab = this.control.tabControl.SelectedTab;
                    if (selectedTab != null && selectedTab.Tag is IOGenTab genTab)
                    {
                        var gridHandler = genTab.GridHandler;

                        var firstEmptyIndexList = gridHandler.DataSource.GetFirstEmptyRowIndexes(ioDataList.Count);

                        var dataDict = new Dictionary<int, IOData>();
                        for (int i = 0; i < firstEmptyIndexList.Count; i++)
                        {
                            var emptyIndex = firstEmptyIndexList[i];
                            var ioData = ioDataList[i];

                            var emptyIoData = gridHandler.DataSource[emptyIndex];
                            GridUtils.CopyGridDataValues(ioData, emptyIoData);
                        }
                    }
                }
            };
            form.importExportMenuItem.DropDownItems.Add(importExcelMenuItem);

            ToolStripMenuItem importSuggestionsMenuItem = new(Locale.IO_GEN_FORM_IMPEXP_IMPORT_SUGGESTION);
            importSuggestionsMenuItem.Click += (sender, args) =>
            {
                var savedFilePath = MainForm.Settings.GetSavedFileDialogPath(FileDialogResources.GENERATION_IO_IMPORT_SUGGESTIONS);

                var fileDialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = false,
                    EnsurePathExists = true,
                    EnsureValidNames = true,
                    Multiselect = true,
                    DefaultExtension = ".xml",
                    Filters = { new CommonFileDialogFilter("XML Files", "*.xml") },
                    InitialDirectory = savedFilePath,
                };

                if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    suggestionGridHandler.DataSource.InitializeData(suggestionGridHandler.InitializeRowCount);

                    foreach (var filePath in fileDialog.FileNames)
                    {
                        var xmlNodeConfiguration = SimaticMLAPI.ParseFile(filePath);
                        if (xmlNodeConfiguration is BlockGlobalDB globalDB)
                        {
                            var suggestionEnumerable = globalDB.GetAllMemberAddress().Select(v => new IOSuggestionData() { Value = v });
                            suggestionGridHandler.AppendData(suggestionEnumerable);

                        }
                        else if (xmlNodeConfiguration is BlockInstanceDB instanceDB)
                        {
                            var suggestionEnumerable = instanceDB.GetAllMemberAddress().Select(v => new IOSuggestionData() { Value = v });
                            suggestionGridHandler.AppendData(suggestionEnumerable);
                        }
                        else if (xmlNodeConfiguration is XMLTagTable tagTable)
                        {
                            var suggestionEnumerable = tagTable.GetTags().Values.Select(t => t.TagName)
                                                                                .Select(n => new IOSuggestionData() { Value = n });
                            suggestionGridHandler.AppendData(suggestionEnumerable);
                        }
                        else
                        {
                            InformationBox.Show("The selected block is NOT a GlobalDB, BlockInstanceDB or file is invalid.", "Invalid imported xml", icon: InformationBoxIcon.Exclamation);
                        }
                    }

                    this.UpdateSuggestionColors();

                    var fileName = fileDialog.FileName;
                    if(fileName != null)
                    {
                        MainForm.Settings.SetSavedFileDialogPath(FileDialogResources.GENERATION_IO_IMPORT_SUGGESTIONS, Path.GetDirectoryName(fileName));
                    }
                }
            };
            form.importExportMenuItem.DropDownItems.Add(importSuggestionsMenuItem);

            ToolStripMenuItem importAddressMenuItem = new(Locale.IO_GEN_FORM_IMPEXP_IMPORT_IO);
            importAddressMenuItem.Click += (sender, args) =>
            {
                if (this.control.tabControl.SelectedTab?.Tag is not IOGenTab ioTab)
                {
                    return;
                }

                var savedFilePath = MainForm.Settings.GetSavedFileDialogPath(FileDialogResources.GENERATION_IO_IMPORT_FROM_TABLE);

                var fileDialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = false,
                    EnsurePathExists = true,
                    EnsureValidNames = true,
                    Multiselect = true,
                    DefaultExtension = ".xml",
                    Filters = { new CommonFileDialogFilter("XML Files", "*.xml") },
                    InitialDirectory = savedFilePath,
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
                        var req = ioTab.GridHandler.DataChangedHandler.Join();

                        var tags = tagTable.GetTags().Values;

                        var emptyIndexList = ioTab.GridHandler.DataSource.GetFirstEmptyRowIndexes(tags.Count);

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

                            var ioData = ioTab.GridHandler.DataSource[index];
                            ioData.Address = address;
                            ioData.IOName = ioName;
                            ioData.Comment = comment;
                        }

                        ioTab.GridHandler.DataChangedHandler.End(req);
                    }
                    else
                    {
                        InformationBox.Show("The selected block is NOT a Tag Table or file is invalid.", "Invalid imported xml", icon: InformationBoxIcon.Exclamation);
                    }
                }

                this.UpdateSuggestionColors();

                var fileName = fileDialog.FileName;
                if (fileName != null)
                {
                    MainForm.Settings.SetSavedFileDialogPath(FileDialogResources.GENERATION_IO_IMPORT_FROM_TABLE, Path.GetDirectoryName(fileName));
                }
            };
            form.importExportMenuItem.DropDownItems.Add(importAddressMenuItem);
            #endregion

            #region DRAG
            suggestionGridHandler.ExcelDragPreview += (sender, args) => GridUtils.DragPreview(args, suggestionGridHandler);
            suggestionGridHandler.ExcelDragDone += (sender, args) => GridUtils.DragDone(args, suggestionGridHandler);
            #endregion
            //Column initialization before gridHandler.Init()
            #region COLUMNS
            suggestionGridHandler.Columns.AddTextBox(IOSuggestionData.VALUE, 0);
            #endregion

            this.multiGrid.Init(form);
            this.suggestionGridHandler.Init();

            #region SUGGESTIONS GRID - EVENTS - TOOL TIP / CELL CHANGE
            this.suggestionGridHandler.CellToolTipTextNeeded += (sender, args) =>
            {
                if (args.RowIndex < 0 || args.RowIndex > suggestionGridHandler.InitializeRowCount)
                {
                    return;
                }

                List<GenTabRowRecord> rows = [];

                args.ToolTipText = "";

                foreach (var genTab in ioTabList)
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

            suggestionGridHandler.DataChanged += (sender, args) => UpdateSuggestionColors();
            #endregion

            #region PREVIEW
            this.suggestionPreviewer.Function = (column, ioData) => null;
            #endregion

            #region TAB CONTROL
            this.control.tabControl.TabPreAdded += (sender, args) => TabCreation(args.TabPage);
            this.control.tabControl.TabPreRemoved += (sender, args) =>
            {
                if (args.TabPage.Tag is IOGenTab ioGenTab)
                {
                    ioTabList.Remove(ioGenTab);
                }
            };

            this.control.tabControl.TabNameUserChanged += (sender, args) =>
            {
                var newName = args.NewName;
                foreach (var loopTab in this.ioTabList)
                {
                    if (newName == loopTab.Name)
                    {
                        var tabNames = this.ioTabList
                                            .Where(tab => tab.TabPage != args.TabPage)
                                            .Select(tab => tab.Name);

                        var fixedNewName = Utils.CheckEqualityAndAddNumberAtEnd(newName, tabNames);
                        args.NewName = fixedNewName;
                    }
                }

                this.SettingsBindings.Update();
            };

            this.control.tabControl.Selected += (sender, args) =>
            {
                if (args.TabPage?.Tag is IOGenTab tab)
                {
                    tab.Selected();
                    this.SettingsBindings.Update();
                }
            };
            #endregion

            #region SETTINGS_BINDINGS
            void placeholderRequestEvent(object? sender, PlaceholderViewRequestEventArgs args) => this.OpenPlaceholderViewer(args.Form);
            this.SettingsBindings.PlaceholderViewerRequestEvent += placeholderRequestEvent;

            form.FormClosed += (sender, args) =>
            {
                this.SettingsBindings.PlaceholderViewerRequestEvent -= placeholderRequestEvent;
            };
            #endregion

            form.Shown += (sender, args) =>
            {
                this.suggestionGridHandler.ViewManipulator.AutoResizeColumnHeadersHeight();
                if (this.control.tabControl.TabCount == 0)
                { //Check required because Load could be called before form is shown!
                    this.control.tabControl.AddTabs();
                }
            };

            this.AddConfigurationBindings(this.SettingsBindings);
        }

        public void ToggleSettingsFormVisibility() => this.settingsFormCache.ToggleVisibility();

        private void TabCreation(TabPage tabPage, IOGenTabSave? save = null)
        {
            IOGenTab ioGenTab = new(MainForm.Settings.GridSettings, this.multiGrid, this, tabPage, this.mainConfig);
            ioGenTab.Init();
            
            if (save == null)
            {
                ioGenTab.Name = Utils.CheckEqualityAndAddNumberAtEnd("IoTab", this.ioTabList.Select(tab => tab.Name));
            }
            if (save != null)
            {
                ioGenTab.LoadSave(save);
                ioGenTab.Name = Utils.CheckEqualityAndAddNumberAtEnd(ioGenTab.Name, this.ioTabList.Select(tab => tab.Name)); //In case the loaded file has a duplicated name!
            }

            tabPage.Tag = ioGenTab;
            tabPage.Controls.Add(ioGenTab.GridHandler.GetControl());

            ioTabList.Add(ioGenTab);
        }

        public void Clear()
        {
            this.SettingsBindings.Clear();

            this.ioTabList.Clear();
            this.control.tabControl.TabPages.Clear();
        }

        public bool IsDirty() => mainConfig.IsDirty() || suggestionGridHandler.IsDirty() || ioTabList.Any(x => x.IsDirty()) || this.multiGrid.IsDirty();
        public void Wash()
        {
            this.mainConfig.Wash();
            this.suggestionGridHandler.Wash();
            foreach (var tab in ioTabList)
            {
                tab.Wash();
            }
            this.multiGrid.Wash();
        }

        public Control? GetControl()
        {
            return this.control;
        }

        public void OpenPlaceholderViewer(IWin32Window? window = null)
        {
            var form = window ?? this.control.FindForm();
            if (form == null)
            {
                return;
            }

            var placeholderForm = new PlaceholderViewerForm(GenPlaceholders.IO.PLACEHOLDER_LIST);
            placeholderForm.Show(form);
        }

        public void ExportXML(string folderPath)
        {
            var ioXmlGenerator = new IOXmlGenerator(mainConfig);
            ioXmlGenerator.Init();

            foreach (var tab in ioTabList)
            {
                var ioDataList = new List<IOData>(tab.GridHandler.DataSource.GetNotEmptyClonedDataDict().Keys); //Return CLONED data, otherwise operations on the xml generation will affect the table!
                ioXmlGenerator.GenerateAlias(tab.TabPage.Text, tab.Previewer, tab.TabConfig, ioDataList);
            }

            ioXmlGenerator.ExportXML(folderPath);
        }

        public object CreateSave()
        {
            IOGenSaveV1 save = new()
            {
                SuggestionGrid = this.suggestionGridHandler.CreateSave(),
                ScriptSave = this.multiGrid.JsScriptHandler.CreateSave()
            };

            GenUtils.CopyJsonFieldsAndProperties(mainConfig, save.MainConfig);
            GenUtils.CopyJsonFieldsAndProperties(excelImportConfig, save.ExcelImportConfiguration);

            foreach (var tab in ioTabList)
            {
                var tabSave = tab.CreateSave();
                save.TabSaves.Add(tabSave);
            }

            return save;
        }

        public void LoadSave(object saveObject)
        {
            if (saveObject is not IOGenSaveV1 loadedSave)
            {
                return;
            }

            this.Clear();

            this.multiGrid.JsScriptHandler.LoadSave(loadedSave.ScriptSave);
            this.suggestionGridHandler.LoadSave(loadedSave.SuggestionGrid);

            GenUtils.CopyJsonFieldsAndProperties(loadedSave.MainConfig, mainConfig);
            GenUtils.CopyJsonFieldsAndProperties(loadedSave.ExcelImportConfiguration, excelImportConfig);

            foreach (var tabSave in loadedSave.TabSaves)
            {
                TabPage tabPage = new();
                TabCreation(tabPage, tabSave);
                this.control.tabControl.TabPages.Add(tabPage);
            }

            this.UpdateSuggestionColors();

            this.AddConfigurationBindings(this.SettingsBindings);
            //Seems that the Selected event is not called in this case. Doing it manually.
            if (this.control.tabControl.SelectedTab?.Tag is IOGenTab tab)
            {
                tab.Selected();
            }
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var selectedTab = this.control.tabControl.SelectedTab;
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
            if (filterAlreadyUsed)
            {
                foreach (var ioTab in this.ioTabList)
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
            this.suggestionGridHandler.ViewManipulator.SuspendLayout();

            foreach (var rowIndex in this.suggestionGridHandler.DataSource.GetNotEmptyIndexes())
            {
                var cell = this.suggestionGridHandler.ViewManipulator.GetCell(rowIndex, IOSuggestionData.VALUE);
                if(cell != null)
                {
                    cell.Style.BackColor = SystemColors.ControlLightLight;
                    cell.Style.SelectionBackColor = Color.LightGray;
                }
            }

            List<GenTabRowRecord> rows = [];

            foreach (var genTab in ioTabList)
            {
                var dict = genTab.GridHandler.DataSource.GetNotEmptyDataDict();
                foreach (var entry in dict)
                {
                    rows.Add(new(genTab, entry.Key, entry.Value));
                }
            }

            var suggestionDict = this.suggestionGridHandler.DataSource.GetNotEmptyDataDict();
            foreach (var suggestionEntry in suggestionDict)
            {
                var suggestion = suggestionEntry.Key;
                var row = suggestionEntry.Value;

                var foundData = rows.Where(d => d.IOData.Variable == suggestion.Value);
                if (foundData.Any())
                {
                    var cell = this.suggestionGridHandler.ViewManipulator.GetCell(row, IOSuggestionData.VALUE);
                    if(cell != null)
                    {
                        cell.Style.BackColor = cell.Style.SelectionBackColor = Color.LightGreen;
                    }
                }
            }

            this.suggestionGridHandler.ViewManipulator.ResumeLayout(refresh: true);
        }

        private void AddConfigurationBindings(SettingsBindings settingsBindings)
        {
            IOGenUtils.AddMainConfigBindings(settingsBindings, this.mainConfig);
            IOGenUtils.AddTabConfigSettings(settingsBindings, 
                this.GetCurrentTabName,
                this.IsAnyTabSelected,
                this.GetCurrentTabConfiguration, 
                this.GetTabConfigurationDict);
        }

        private string GetCurrentTabName()
        {
            var tabPage = this.control.tabControl.SelectedTab;
            return tabPage == null ? "" : tabPage.Text;
        }

        private bool IsAnyTabSelected()
        {
            return this.control.tabControl.SelectedTab != null;
        }

        private IOTabConfiguration? GetCurrentTabConfiguration()
        {
            return this.control.tabControl.SelectedTab?.Tag is IOGenTab genTab ? genTab.TabConfig : null;
        }

        private Dictionary<string, ObservableConfiguration> GetTabConfigurationDict()
        {
            Dictionary<string, ObservableConfiguration> dict = [];
            foreach (var tab in this.ioTabList)
            {
                if(!dict.TryAdd(tab.Name, tab.TabConfig))
                {
                    dict.Add(tab.Name + "*", tab.TabConfig);
                }
            }
            return dict;
        }

        public List<GenModuleEditableTextReference> GetTextsReferences()
        {
            throw new NotImplementedException();
        }

        public void SetTextsReferences(List<GenModuleEditableTextReference> textReferences)
        {
            throw new NotImplementedException();
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