using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Javascript;
using TiaXmlReader.Languages;
using TiaXmlReader.Generation;
using InfoBox;
using Microsoft.WindowsAPICodePack.Dialogs;
using SimaticML.API;
using SimaticML.Blocks;
using SimaticML.TagTable;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.GenModules;
using TiaUtilities.Generation.GenModules.IO;
using TiaUtilities.Generation.GenModules.IO.Controls;
using TiaUtilities.Generation.GenModules.IO.ExcelImporter;
using TiaUtilities.Generation.GenModules.IO.Tab;

namespace TiaUtilities.Generation.GenModules.IO
{
    public class IOGenModule : IGenModule
    {
        private record GenTabRowRecord(IOGenTab GenTab, IOData IOData, int Row);

        public ICollection<IOSuggestionData> Suggestions { get => this.suggestionGridHandler.DataSource.GetNotEmptyDataDict().Keys; }

        private readonly JavascriptErrorReportThread jsErrorHandlingThread;
        private readonly GridSettings gridSettings;

        private readonly GridScriptContainer scriptContainer;

        private readonly IOMainConfiguration mainConfig;
        private readonly IOGenerationExcelImportSettings excelImportConfig;

        private readonly GridHandler<IOMainConfiguration, IOSuggestionData> suggestionGridHandler;

        private readonly IOGenConfigTopControl configControlTop;
        private readonly IOGenBottomControl tabControlBottom;

        private readonly List<IOGenTab> genTabList;

        public IOGenModule(JavascriptErrorReportThread jsErrorHandlingThread, GridSettings gridSettings)
        {
            this.jsErrorHandlingThread = jsErrorHandlingThread;
            this.gridSettings = gridSettings;

            this.scriptContainer = new();

            this.mainConfig = new();
            this.excelImportConfig = new();

            this.suggestionGridHandler = new GridHandler<IOMainConfiguration, IOSuggestionData>(jsErrorHandlingThread, gridSettings, mainConfig, scriptContainer) { RowCount = 1999 };

            this.configControlTop = new();
            this.tabControlBottom = new(this.suggestionGridHandler.DataGridView);

            this.genTabList = [];
        }

        public void Init(GenModuleForm form)
        {
            #region IMPORT_EXPORT_MENU_ITEMS
            ToolStripMenuItem importExcelMenuItem = new(Localization.Get("IO_GEN_FORM_IMPEXP_IMPORT_EXCEL"));
            importExcelMenuItem.Click += (sender, args) =>
            {
                var excelImportForm = new IOGenerationExcelImportForm(this.jsErrorHandlingThread, this.excelImportConfig, this.gridSettings);

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

                    var selectedTab = this.tabControlBottom.gridsTabControl.SelectedTab;
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

            ToolStripMenuItem importSuggestionsMenuItem = new(Localization.Get("IO_GEN_FORM_IMPEXP_IMPORT_SUGGESTION"));
            importSuggestionsMenuItem.Click += (sender, args) =>
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

                        var xmlNodeConfiguration = SimaticMLAPI.ParseFile(filePath);
                        if (xmlNodeConfiguration is BlockGlobalDB globalDB)
                        {
                            var suggestionEnumerable = globalDB.GetAllMemberAddress().Select(v => new IOSuggestionData() { Value = v });
                            this.suggestionGridHandler.AddData(suggestionEnumerable);

                        }
                        else if (xmlNodeConfiguration is XMLTagTable tagTable)
                        {
                            var suggestionEnumerable = tagTable.GetTags().Values.Select(t => t.TagName)
                                                                                .Select(n => new IOSuggestionData() { Value = n });
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
            form.importExportMenuItem.DropDownItems.Add(importSuggestionsMenuItem);
            #endregion

            #region DRAG
            this.suggestionGridHandler.Events.ExcelDragPreview += (sender, args) => GridUtils.DragPreview(args, this.suggestionGridHandler);
            this.suggestionGridHandler.Events.ExcelDragDone += (sender, args) => GridUtils.DragDone(args, this.suggestionGridHandler);
            #endregion
            //Column initialization before gridHandler.Init()
            #region COLUMNS
            this.suggestionGridHandler.AddTextBoxColumn(IOSuggestionData.VALUE, 0);
            #endregion

            this.suggestionGridHandler.Init();
            this.configControlTop.BindConfig(this.mainConfig);

            #region SUGGESTION_GRIDS_EVENTS
            this.suggestionGridHandler.DataGridView.CellToolTipTextNeeded += (sender, args) =>
            {
                if (args.RowIndex < 0 || args.RowIndex > this.suggestionGridHandler.RowCount)
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

                var suggestion = this.suggestionGridHandler.DataSource[args.RowIndex];
                var foundData = rows.Where(d => d.IOData.Variable == suggestion.Value);
                if (foundData.Any())
                {
                    //The suggestion could be used multiple times! So i will aggregate all the found data.
                    args.ToolTipText += foundData
                        .Select(x => $"{x.GenTab.TabPage.Text} {x.Row + 1}) {x.IOData.Address} - {x.IOData.IOName} - {x.IOData.Comment}")
                        .Aggregate((a, b) => a + Environment.NewLine + b); ;
                }
            };

            this.suggestionGridHandler.Events.CellChange += (sender, args) => UpdateSuggestionColors();
            #endregion

            this.tabControlBottom.gridsTabControl.TabPreAdded += (sender, args) =>
            {
                var tabPage = args.TabPage;
                tabPage.Text = "IOGen";

                IOGenTab ioGenTab = new(this, tabPage, this.mainConfig, this.jsErrorHandlingThread, this.gridSettings, this.scriptContainer);
                ioGenTab.Init();
                this.genTabList.Add(ioGenTab);

                tabPage.Controls.Add(ioGenTab.TabControl);
                tabPage.Tag = ioGenTab;
            };

            this.tabControlBottom.gridsTabControl.TabPreRemoved += (sender, args) =>
            {
                if (args.TabPage.Tag is IOGenTab ioGenTab)
                {
                    this.genTabList.Remove(ioGenTab);
                }
            };


            form.Shown += (sender, args) =>
            {
                this.suggestionGridHandler.DataGridView.AutoResizeColumnHeadersHeight();
            };
        }

        public bool IsDirty()
        {
            var dirty = this.mainConfig.IsDirty();
            dirty |= this.suggestionGridHandler.IsDirty();
            foreach (var genTab in genTabList)
            {
                dirty |= genTab.IsDirty();
            }
            return dirty;
        }

        public void Wash()
        {
            this.mainConfig.Wash();
            this.suggestionGridHandler.Wash();
            foreach (var genTab in genTabList)
            {
                genTab.Wash();
            }
        }

        public Control? GetTopControl()
        {
            return this.configControlTop;
        }

        public Control? GetBottomControl()
        {
            return this.tabControlBottom;
        }

        public void ExportXML(string folderPath)
        {
            var ioXmlGenerator = new IOXmlGenerator(this.mainConfig);
            ioXmlGenerator.Init();

            foreach (var genTab in genTabList)
            {
                var ioDataList = new List<IOData>(genTab.GridHandler.DataSource.GetNotEmptyClonedDataDict().Keys); //Return CLONED data, otherwise operations on the xml generation will affect the table!
                ioXmlGenerator.GenerateAlias(genTab.TabPage.Text, genTab.TabConfig, ioDataList);
            }

            ioXmlGenerator.ExportXML(folderPath);
        }

        public object CreateSave()
        {
            IOGenSave save = new()
            {
                SuggestionGrid = this.suggestionGridHandler.CreateSave(),
                ScriptContainer = this.scriptContainer.CreateSave()
            };

            GenUtils.CopyJsonFieldsAndProperties(this.mainConfig, save.MainConfig);
            GenUtils.CopyJsonFieldsAndProperties(this.excelImportConfig, save.ExcelImportConfiguration);

            foreach (var genTab in genTabList)
            {
                var tabSave = genTab.CreateSave();
                save.TabSaves.Add(tabSave);
            }

            return save;
        }

        public void LoadSave(object? saveObject)
        {
            if (saveObject is not IOGenSave loadedSave)
            {
                return;
            }

            this.scriptContainer.LoadSave(loadedSave.ScriptContainer);
            this.suggestionGridHandler.LoadSave(loadedSave.SuggestionGrid);

            GenUtils.CopyJsonFieldsAndProperties(loadedSave.MainConfig, this.mainConfig);
            GenUtils.CopyJsonFieldsAndProperties(loadedSave.ExcelImportConfiguration, this.excelImportConfig);

            foreach (var tabSave in loadedSave.TabSaves)
            {
                var tabPage = new TabPage()
                {
                    Text = "IOGen"
                };
                this.tabControlBottom.gridsTabControl.TabPages.Add(tabPage);

                IOGenTab ioGenTab = new(this, tabPage, this.mainConfig, this.jsErrorHandlingThread, this.gridSettings, this.scriptContainer);
                ioGenTab.Init();
                ioGenTab.LoadSave(tabSave);
                this.genTabList.Add(ioGenTab);

                tabPage.Tag = ioGenTab;
                tabPage.Controls.Add(ioGenTab.TabControl);
            }

            this.UpdateSuggestionColors();
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var selectedTab = this.tabControlBottom.gridsTabControl.SelectedTab;
            if (selectedTab != null && selectedTab.Tag is IOGenTab ioGenTab)
            {
                return ioGenTab.ProcessCmdKey(ref msg, keyData);
            }

            return false;
        }

        public string GetFormLocalizatedName()
        {
            return Localization.Get("IO_GEN_FORM");
        }

        public void UpdateSuggestionColors()
        {
            this.suggestionGridHandler.DataGridView.SuspendLayout();

            foreach (DataGridViewRow row in this.suggestionGridHandler.DataGridView.Rows)
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
                    var cell = this.suggestionGridHandler.DataGridView.Rows[row].Cells[IOSuggestionData.VALUE.ColumnIndex];
                    cell.Style.BackColor = cell.Style.SelectionBackColor = Color.LightGreen;
                }
            }

            this.suggestionGridHandler.DataGridView.ResumeLayout();
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