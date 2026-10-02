using InfoBox;
using Microsoft.WindowsAPICodePack.Dialogs;
using SimaticML.API;
using SimaticML.Blocks;
using System.Globalization;
using TiaUtilities.Configuration;
using TiaUtilities.CustomControls.EditableTab;
using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Generation.Alarms.Data;
using TiaUtilities.Generation.Alarms.Module.Tab;
using TiaUtilities.Generation.Alarms.Template;
using TiaUtilities.Generation.Alarms.Xml;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Generation.TextsEditor;
using TiaUtilities.JSScript;
using TiaUtilities.Languages;
using TiaUtilities.Resources;
using TiaUtilities.SettingsStep;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.Alarms.Module
{
    public class AlarmGenModule : IGenModule
    {
        public const int DEVICE_GRID_ROW_COUNT = 999;
        public const int TEMPLATE_GRID_ROW_COUNT = 999;

        private readonly MultiGridOperationHandler multiGrid;
        private JSScriptHandler JsScriptHandler { get => this.multiGrid.JsScriptHandler; }

        private readonly EditableTabControl tabControl;
        private readonly SettingsControl settingsControl;

        private readonly AlarmMainConfiguration mainConfig;
        private readonly AlarmGenTemplateContainer templateContainer;

        private readonly List<AlarmGenTab> alarmTabList;
        public IEnumerable<AlarmTabConfiguration> TabConfigurations { get => this.alarmTabList.Select(tab => tab.TabConfig); }

        public List<IGenModule.ModuleControl> ModuleControls { get; init; }

        private bool loadingSave = false;

        public AlarmGenModule()
        {
            this.multiGrid = new();

            this.tabControl = new()
            {
                Dock = DockStyle.Fill,
                Padding = new Point(12, 5),
                RequireConfirmationBeforeClosing = true,
                SelectedIndex = 0,
            };
            //this.templateControl = this.CreateTemplateControl();

            this.settingsControl = new()
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
            };
            this.settingsControl.InitControls();

            this.mainConfig = new();
            this.templateContainer = new();
            GenUtils.CopyJsonFieldsAndProperties(MainForm.Settings.PresetAlarmMainConfiguration, this.mainConfig);

            this.alarmTabList = [];

            this.ModuleControls = [
                new() { Name = "Grids", RequestControlCallback = () => this.tabControl },
                new() { Name = Locale.DEVICE_DATA_TEMPLATE, RequestControlCallback = this.CreateTemplateControl },
                new() {
                    Name = Locale.GENERICS_SETTINGS,
                    RequestControlCallback = () => UpdateSettingsControl()
                },
            ];
        }

        public void Init(GenModuleForm form)
        {
            this.multiGrid.Init(form);

            #region TOP_BUTTONS_STRIP
            ToolStripMenuItem importTemplatesFromFb = new("Import templates from FB");
            importTemplatesFromFb.Click += (sender, args) =>
            {
                var savedFilePath = MainForm.Settings.GetSavedFileDialogPath(FileDialogResources.GENERATION_ALARM_IMPORT_TEMPLATES_FROM_FB);

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
                    foreach (var filePath in fileDialog.FileNames)
                    {
                        if (string.IsNullOrEmpty(filePath))
                        {
                            continue;
                        }

                        var xmlNodeConfiguration = SimaticMLAPI.ParseFile(filePath);
                        if (xmlNodeConfiguration is BlockFB blockFB)
                        {
                            var templateName = blockFB.AttributeList.BlockName;
                            templateContainer.Remove(templateName);

                            var template = templateContainer.Add(templateName);
                            foreach (var member in blockFB.AttributeList.STATIC.GetItems())
                            {
                                if (member.MemberName.StartsWith("allarmi", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    var templateRowData = template.AlarmGridSave.RowData;

                                    var nextGridIndex = templateRowData.Count == 0 ? 0 : (templateRowData.Keys.Max() + 1);
                                    foreach (var subMember in member.Members)
                                    {
                                        TemplateData newTemplateData = new()
                                        {
                                            Enable = true,
                                            AlarmVariable = subMember.MemberName,
                                            Description = subMember.Comment[CultureInfo.CurrentCulture]
                                        };

                                        templateRowData.Add(nextGridIndex, newTemplateData);
                                        nextGridIndex++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            InformationBox.Show("The selected block is NOT a BlockFB or file is invalid.", "Invalid imported xml", icon: InformationBoxIcon.Exclamation);
                        }
                    }
                }
            };
            form.importExportMenuItem.DropDownItems.Add(importTemplatesFromFb);
            #endregion

            #region TEMPLATE_HANDLER
            this.templateContainer.Renamed += (sender, args) =>
            {
                foreach (var tab in this.alarmTabList)
                {
                    tab.ParseTemplateRenamed(args.OldName, args.Template.Name);
                }

                this.UpdateSettingsControl(ifVisible: true);
            };

            this.templateContainer.Added += (sender, args) =>  this.UpdateSettingsControl(ifVisible: true);

            #endregion

            #region TAB_CONTROL
            this.tabControl.TabAdded += (sender, args) =>
            {
                if(this.loadingSave)
                {
                    return;
                }

                this.TabCreation(args.TabPage);
                this.UpdateSettingsControl(ifVisible: true);
            };
            this.tabControl.TabRemoved += (sender, args) =>
            {
                if (this.loadingSave)
                {
                    return;
                }

                if (args.TabPage.Tag is AlarmGenTab tab)
                {
                    this.alarmTabList.Remove(tab);
                }

                this.UpdateSettingsControl(ifVisible: true);
            };

            this.tabControl.TabRenamed += (sender, args) =>
            {
                if (this.loadingSave)
                {
                    return;
                }

                var newName = args.NewName;
                foreach (var loopTab in this.alarmTabList)
                {
                    if (newName == loopTab.Name)
                    {
                        var tabNames = this.alarmTabList
                                            .Where(tab => tab.TabPage != args.TabPage)
                                            .Select(tab => tab.Name);

                        var fixedNewName = Utils.CheckEqualityAndAddNumberAtEnd(newName, tabNames);
                        args.NewName = fixedNewName;
                    }
                }

                this.UpdateSettingsControl(ifVisible: true);
            };
            this.tabControl.Selected += (sender, args) =>
            {
                if (args.TabPage?.Tag is AlarmGenTab tab)
                {
                    tab.Selected();
                }
            };
            #endregion

            form.Shown += (sender, args) =>
            {
                if (this.tabControl.TabCount == 0)
                { //Check required because Load could be called before form is shown!
                    this.tabControl.AddTabs();
                }
            };
        }

        public void ShowSettings()
        {
            var sequences = this.GetSettingsSequences();

            SettingsForm form = new();
            form.SetSequences(sequences);
            form.ShowDialog();

            //this.settingsFormCache.ToggleVisibility();
        }

        private void TabCreation(TabPage tabPage, AlarmGenTabSave? save = null)
        {
            AlarmGenTab alarmTab = new(this.multiGrid, this, this.mainConfig, this.templateContainer, tabPage);
            alarmTab.Init();

            if (save == null)
            {
                alarmTab.Name = Utils.CheckEqualityAndAddNumberAtEnd("AlarmTab", this.alarmTabList.Select(tab => tab.Name));
            }
            else
            {
                alarmTab.LoadSave(save);
                alarmTab.Name = Utils.CheckEqualityAndAddNumberAtEnd(alarmTab.Name, this.alarmTabList.Select(tab => tab.Name)); //In case the loaded file has a duplicated name!
            }

            tabPage.Tag = alarmTab;
            tabPage.Controls.Add(alarmTab.GetGridControl());

            alarmTabList.Add(alarmTab); //Do this AFTER. Otherwise the Selected event is called with Tag null.
        }

        public void Clear()
        {
            this.alarmTabList.Clear();
            this.tabControl.TabPages.Clear();
        }

        public bool IsDirty() => this.mainConfig.IsDirty() || this.alarmTabList.Any(x => x.IsDirty()) || this.JsScriptHandler.IsDirty() || this.templateContainer.IsDirty();
        public void Wash()
        {
            this.mainConfig.Wash();
            foreach (var tab in this.alarmTabList)
            {
                tab.Wash();
            }
            this.JsScriptHandler.Wash();
            this.templateContainer.IsDirty();
        }

        public object CreateSave()
        {
            var projectSave = new AlarmGenSaveV1()
            {
                ScriptSave = this.JsScriptHandler.CreateSave(),
                TemplateSaves = this.templateContainer.CreateSave()
            };

            GenUtils.CopyJsonFieldsAndProperties(mainConfig, projectSave.AlarmMainConfig);

            foreach (var tab in alarmTabList)
            {
                var tabSave = tab.CreateSave();
                projectSave.TabSaves.Add(tabSave);
            }

            return projectSave;
        }

        public void LoadSave(object saveObject)
        {
            if (saveObject is not AlarmGenSaveV1 loadedSave)
            {
                return;
            }

            this.loadingSave = true;

            try
            {
                this.Clear();

                GenUtils.CopyJsonFieldsAndProperties(loadedSave.AlarmMainConfig, mainConfig);

                this.JsScriptHandler.LoadSave(loadedSave.ScriptSave);
                this.templateContainer.LoadSave(loadedSave.TemplateSaves);

                foreach (var tabSave in loadedSave.TabSaves)
                {
                    var tabPage = this.tabControl.AddTab();
                    TabCreation(tabPage, tabSave);
                }

                //Seems that the Selected event is not called in this case. Doing it manually.
                if (this.tabControl.SelectedTab?.Tag is AlarmGenTab tab)
                {
                    tab.Selected();
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            this.loadingSave = false;

        }

        public void ExportXML(string folderPath)
        {
            AlarmXmlGenerator ioXmlGenerator = new(mainConfig);
            foreach (var tab in alarmTabList)
            {
                ioXmlGenerator.GenerateAlarms(tab.TabPage.Text, tab.TabConfig, this.templateContainer, tab.DeviceDataList);
            }
            ioXmlGenerator.ExportXML(folderPath);
        }

        public void OpenPlaceholderViewer(IWin32Window? window = null)
        {
            var form = window ?? this.tabControl.FindForm();

            var placeholderForm = new PlaceholderViewerForm(GenPlaceholders.Alarms.PLACEHOLDER_LIST);
            placeholderForm.Show(form);
        }

        public string GetFormLocalizatedName()
        {
            return Locale.ALARM_GEN_FORM;
        }

        private GenPlaceholderHandler? CreateGenericPlaceholderHandler()
        {
            var currentTabName = this.GetCurrentTabName();
            var currentTab = this.GetCurrentTab();

            AlarmGenPlaceholdersHandler? placeholdersHandler = null;
            if (currentTabName != null && currentTab != null)
            {
                placeholdersHandler = new(this.mainConfig, currentTab.TabConfig)
                {
                    TabName = currentTabName
                };

                if (currentTab.DeviceDataList.Count > 0)
                {
                    var firstDeviceData = currentTab.DeviceDataList[0];
                    placeholdersHandler.DeviceData = firstDeviceData;
                }

                if (this.templateContainer.Count > 0)
                {
                    var firstTemplate = this.templateContainer[0];
                    if (firstTemplate.AlarmGridSave.RowData.Count > 0)
                    {
                        var firstTemplateData = firstTemplate.AlarmGridSave.RowData[0];
                        placeholdersHandler.TemplateData = firstTemplateData;
                    }
                }

                placeholdersHandler.LoadJSONObject(currentTab.TabConfig.CustomPlaceholdersJSON);

                placeholdersHandler.SetAlarmNum(0, this.mainConfig.AlarmNumFormat);
                placeholdersHandler.SetStartEndAlarmNum(0, 999, this.mainConfig.AlarmNumFormat);
            }

            return placeholdersHandler;
        }

        private SettingsControl UpdateSettingsControl(bool ifVisible = true)
        {
            if(!this.settingsControl.Visible && ifVisible)
            {
                return this.settingsControl;
            }

            var sequences = this.GetSettingsSequences();
            this.settingsControl.SetSequences(sequences);
            return this.settingsControl;
        }

        private List<SettingsSequence> GetSettingsSequences()
        {
            string ParsePlaceholders(string str)
            {
                var placeholderHandler = this.CreateGenericPlaceholderHandler();
                return placeholderHandler == null ? str : placeholderHandler.ParseNotNull(str);
            }

            List<SettingsSequence> sequenceList = [];

            var globalStepDescriptors = AlarmGenUtils.CreateGlobalSettingsDescriptors();
            var tablStepDescriptors = AlarmGenUtils.CreateTabSettingsStepDescriptors();
            var templateStepDescriptors = AlarmGenUtils.CreateTemplateSettingsStepDescriptor();

            SettingsSequence globalSequence = new(this.mainConfig, "Global", "Settings") { PlaceholdersCallBack = ParsePlaceholders };
            globalSequence.AddRange(globalStepDescriptors);
            sequenceList.Add(globalSequence);

            foreach (var tab in this.alarmTabList)
            {
                SettingsSequence tabSequence = new(tab.TabConfig, "Tab", tab.Name) { PlaceholdersCallBack = ParsePlaceholders };
                tabSequence.AddRange(tablStepDescriptors);
                sequenceList.Add(tabSequence);
            }

            foreach (var template in this.templateContainer)
            {
                SettingsSequence templateSequence = new(template.TemplateConfig, "Template", template.Name) { PlaceholdersCallBack = ParsePlaceholders };
                templateSequence.AddRange(templateStepDescriptors);
                sequenceList.Add(templateSequence);
            }

            return sequenceList;
        }

        private AlarmGenTemplateControl CreateTemplateControl()
        {
            var currentTabConfig = this.GetCurrentTabConfiguration();

            Validate.NotNull(currentTabConfig);

            AlarmGenTemplateControl control = new(this.mainConfig, currentTabConfig, this.multiGrid, this.templateContainer)
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
            };
            control.Init();
            return control;
        }

        private string GetCurrentTabName()
        {
            var tabPage = this.tabControl.SelectedTab;
            return tabPage == null ? "" : tabPage.Text;
        }

        private bool IsAnyTabSelected()
        {
            return this.tabControl.SelectedTab != null;
        }

        private AlarmTabConfiguration? GetCurrentTabConfiguration()
        {
            return this.GetCurrentTab()?.TabConfig;
        }

        private AlarmGenTab? GetCurrentTab() => this.tabControl.SelectedTab?.Tag is AlarmGenTab genTab ? genTab : null;

        private Dictionary<string, ObservableConfiguration> GetTabConfigurationDict()
        {
            Dictionary<string, ObservableConfiguration> dict = [];
            foreach (var tab in this.alarmTabList)
            {
                if (!dict.TryAdd(tab.Name, tab.TabConfig))
                {
                    dict.Add(tab.Name + "*", tab.TabConfig);
                }
            }
            return dict;
        }

        private string GetActiveTemplateName()
        {
            var selectedTemplate = this.templateContainer.SelectedTemplate;
            return selectedTemplate == null ? "" : selectedTemplate.Name;
        }

        private AlarmTemplateConfiguration? GetActiveTemplateConfiguration()
        {
            return this.templateContainer.SelectedTemplate?.TemplateConfig;
        }

        private Dictionary<string, ObservableConfiguration> GetTemplateConfigurationDict()
        {
            Dictionary<string, ObservableConfiguration> dict = [];
            foreach (var template in this.templateContainer)
            {
                if (!dict.TryAdd(template.Name, template.TemplateConfig))
                {
                    dict.Add(template.Name + "*", template.TemplateConfig);
                }
            }
            return dict;
        }

        public List<GenModuleEditableTextReference> GetTextsReferences()
        {
            var splitter = GenModuleTextsEditorForm.REFERENCE_EDITOR_SPLITTER;

            List<GenModuleEditableTextReference> textReferencesList = [];
            foreach (var tab in this.alarmTabList)
            {
                var moduleId = $"TAB{splitter}{tab.Name}";
                AddDataFieldTextReferences(textReferencesList, tab.DeviceDataList, moduleId, d => d.Name ?? "INVALID", nameof(DeviceData.Description));
            }

            foreach (var template in this.templateContainer)
            {
                var moduleId = $"TEMPLATE{splitter}{template.Name}";

                var templateDataEnumerable = template.AlarmGridSave.RowData.Values;
                AddDataFieldTextReferences(textReferencesList, templateDataEnumerable, moduleId, t => t.AlarmVariable ?? "INVALID", nameof(TemplateData.HmiAlarmText));
                AddDataFieldTextReferences(textReferencesList, templateDataEnumerable, moduleId, t => t.AlarmVariable ?? "INVALID", nameof(TemplateData.Description));
            }

            return textReferencesList;
        }

        public void SetTextsReferences(List<GenModuleEditableTextReference> textReferences)
        {
            var splitter = GenModuleTextsEditorForm.REFERENCE_EDITOR_SPLITTER;

            foreach (var tab in this.alarmTabList)
            {
                var tabTextReferences = textReferences.Where(r => this.CheckTextReferenceID1(r, "TAB", tab.Name));
                this.SetTextReferencesToDataField(tabTextReferences, tab.DeviceDataList, d => d.Name);
            }

            foreach (var template in this.templateContainer)
            {
                var templateTextReferenced = textReferences.Where(r => this.CheckTextReferenceID1(r, "TEMPLATE", template.Name));
                this.SetTextReferencesToDataField(templateTextReferenced, template.AlarmGridSave.RowData.Values, d => d.AlarmVariable);
            }
            /*
            foreach (var textReference in textReferences)
            {
                if (textReference.ID1.StartsWith("TAB"))
                {
                    var tabName = textReference.ID1.Split(splitter)[1];
                    var deviceName = textReference.ID2;
                    var dataName = textReference.ID3;

                    var text = textReference.Text;

                    var tab = this.alarmTabList.FirstOrDefault(t => t.Name == tabName);
                    if (tab == null)
                    {
                        continue;
                    }


                    var dataList = tab.DeviceDataList.Where(d => d.Name == deviceName);
                    foreach (var data in dataList)
                    {
                        if (dataName == nameof(data.Description))
                        {
                            data.Description = text;
                        }
                    }
                }
                else if (textReference.ID1.StartsWith("TEMPLATE"))
                {
                    var templateName = textReference.ID1.Split(splitter)[1];
                    var variable = textReference.ID2;
                    var dataName = textReference.ID3;

                    var text = textReference.Text;


                    var template = this.templateHandler.BindingList.FirstOrDefault(t => t.Name == templateName);
                    if (template == null)
                    {
                        continue;
                    }

                    var dataPairEnumerable = template.AlarmGridSave.RowData.Where(p => p.Value.AlarmVariable == variable);
                    foreach (var (rowIndex, templateData) in dataPairEnumerable)
                    {
                        if (dataName == nameof(templateData.Description))
                        {
                            templateData.Description = text;
                        }
                        else if (dataName == nameof(templateData.HmiAlarmText))
                        {
                            templateData.HmiAlarmText = text;
                        }
                    }
                }
            }*/
        }

        private bool CheckTextReferenceID1(GenModuleEditableTextReference textReference, params string[] param)
        {
            var id1SplitArray = textReference.ID1.Split(GenModuleTextsEditorForm.REFERENCE_EDITOR_SPLITTER);
            return id1SplitArray.Length > 0 && id1SplitArray.Length == param.Length && Enumerable.SequenceEqual(id1SplitArray, param);
        }

        private void AddDataFieldTextReferences<T>(List<GenModuleEditableTextReference> textReferenceList,
            IEnumerable<T> dataEnumerable,
            string moduleId, string id2, string propertyName) where T : GridData
        {
            AddDataFieldTextReferences<T>(textReferenceList, dataEnumerable, moduleId, t => id2, propertyName);
        }

        private void AddDataFieldTextReferences<T>(List<GenModuleEditableTextReference> textReferenceList,
            IEnumerable<T> dataEnumerable,
            string moduleId, Func<T, string?> id2Getter, string propertyName) where T : GridData
        {
            foreach (var data in dataEnumerable)
            {
                try
                {
                    var propertyInfo = data.GetType().GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (propertyInfo == null || !propertyInfo.CanRead || propertyInfo.PropertyType != typeof(string))
                    {
                        continue;
                    }

                    var text = (string)propertyInfo.GetValue(data);

                    var textReference = this.CreateEditableTextReference(moduleId, id2Getter(data), propertyName, text);
                    textReferenceList.Add(textReference);
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex);
                }
            }
        }

        private void SetTextReferencesToDataField<T>(IEnumerable<GenModuleEditableTextReference> textReferenceList,
            IEnumerable<T> dataEnumerable,
            Func<T, string?> id2Getter) where T : GridData
        {
            foreach (var textReference in textReferenceList)
            {
                foreach (var data in dataEnumerable)
                {
                    var id2 = id2Getter(data);
                    if (id2 != textReference.ID2)
                    {
                        continue;
                    }

                    var propertyInfo = data.GetType().GetProperty(textReference.ID3, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (propertyInfo == null || !propertyInfo.CanRead || propertyInfo.PropertyType != typeof(string))
                    {
                        continue;
                    }

                    propertyInfo.SetValue(data, textReference.Text);
                }
            }
        }

        private GenModuleEditableTextReference CreateEditableTextReference(string moduleId, string? id, string fieldName, string? fieldText) => new(ID1: moduleId, ID2: id, ID3: fieldName, Text: fieldText);
    }
}
