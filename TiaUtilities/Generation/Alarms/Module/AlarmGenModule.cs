using ClosedXML.Excel;
using InfoBox;
using Microsoft.WindowsAPICodePack.Dialogs;
using SimaticML.API;
using SimaticML.Blocks;
using System.Globalization;
using TiaUtilities.Configuration;
using TiaUtilities.Editors.ErrorReporting;
using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Generation.Alarms.Data;
using TiaUtilities.Generation.Alarms.Module.Tab;
using TiaUtilities.Generation.Alarms.Module.Template;
using TiaUtilities.Generation.Alarms.Template;
using TiaUtilities.Generation.Alarms.Xml;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Generation.TextsEditor;
using TiaUtilities.Languages;
using TiaUtilities.Resources;
using TiaUtilities.SettingsNew;
using TiaUtilities.SettingsNew.Bindings;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.Alarms.Module
{
    public class AlarmGenModule : IGenModule
    {
        public const int DEVICE_GRID_ROW_COUNT = 199;
        public const int TEMPLATE_GRID_ROW_COUNT = 499;

        private readonly GridBindContainer gridBindContainer;
        private GridScriptHandler GridScriptHandler { get => this.gridBindContainer.GridScriptHandler; }

        private readonly AlarmGenControl control;
        private readonly AlarmMainConfiguration mainConfig;
        private readonly AlarmGenTemplateHandler templateHandler;

        private readonly List<AlarmGenTab> alarmTabList;
        public IEnumerable<AlarmTabConfiguration> TabConfigurations { get => this.alarmTabList.Select(tab => tab.TabConfig); }

        public SettingsBindings SettingsBindings { get; init; }
        private readonly SettingsFormCache settingsFormCache;

        private AlarmGenTemplateForm? shownTemplateForm = null;

        public AlarmGenModule(ErrorReportThread errorThread)
        {
            this.gridBindContainer = new(errorThread);

            this.control = new();
            this.mainConfig = new();
            this.templateHandler = new();
            GenUtils.CopyJsonFieldsAndProperties(MainForm.Settings.PresetAlarmMainConfiguration, this.mainConfig);

            this.alarmTabList = [];
            this.SettingsBindings = new();
            this.settingsFormCache = new(this.SettingsBindings, this.control);
        }

        public void Init(GenModuleForm form)
        {
            #region TOP_BUTTONS_STRIP
            this.control.setupButton.Click += (sender, args) => this.ToggleSettingsFormVisibility();
            this.control.changeTemplateButton.Click += (sender, args) =>
            {
                var currentTabConfig = GetCurrentTabConfiguration();
                if (currentTabConfig != null)
                {
                    shownTemplateForm = new AlarmGenTemplateForm(mainConfig, currentTabConfig, this.gridBindContainer, templateHandler);
                    shownTemplateForm.Init();
                    shownTemplateForm.Show(this.control);
                    shownTemplateForm.FormClosed += (sender, args) =>
                    {
                        shownTemplateForm = null;
                        this.SettingsBindings.Reload();
                    };

                    this.SettingsBindings.Reload();
                }
            };

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
                        var xmlNodeConfiguration = SimaticMLAPI.ParseFile(filePath);
                        if (xmlNodeConfiguration is BlockFB blockFB)
                        {
                            foreach (var member in blockFB.AttributeList.STATIC.GetItems())
                            {
                                if (member.MemberName.ToLower().Equals("allarmi"))
                                {
                                    var template = templateHandler.AddNewTemplate();
                                    template.Name = blockFB.AttributeList.BlockName;

                                    var nextGridIndex = template.AlarmGridSave.RowData.Count == 0 ? 0 : (template.AlarmGridSave.RowData.Keys.Max() + 1);
                                    foreach (var subMember in member.GetItems())
                                    {
                                        TemplateData newTemplateData = new() { AlarmVariable = subMember.MemberName, Description = subMember.Comment[CultureInfo.CurrentCulture] };

                                        template.AlarmGridSave.RowData.Add(nextGridIndex, newTemplateData);
                                        nextGridIndex++;
                                    }

                                    break;
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

            this.gridBindContainer.Init(form);

            #region TEMPLATE_HANDLER
            this.templateHandler.Init([]);
            this.templateHandler.TemplateRenamed += (sender, args) =>
            {
                foreach (var tab in this.alarmTabList)
                {
                    tab.ParseTemplateRenamed(args.OldName, args.NewName);
                }

                this.SettingsBindings.Update();
            };
            this.templateHandler.SelectedTemplateChanged += (sender, args) => this.SettingsBindings.Update();
            #endregion

            #region TAB_CONTROL
            this.control.tabControl.TabPreAdded += (sender, args) => TabCreation(args.TabPage);
            this.control.tabControl.TabPreRemoved += (sender, args) =>
            {
                if (args.TabPage.Tag is AlarmGenTab tab)
                {
                    this.alarmTabList.Remove(tab);
                }
            };

            this.control.tabControl.TabNameUserChanged += (sender, args) =>
            {
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

                this.SettingsBindings.Update();
            };
            this.control.tabControl.Selected += (sender, args) =>
            {
                if (args.TabPage?.Tag is AlarmGenTab tab)
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
                if (this.control.tabControl.TabCount == 0)
                { //Check required because Load could be called before form is shown!
                    this.control.tabControl.AddTabs();
                }
            };

            this.AddConfigurationBindings(this.SettingsBindings);
        }

        public void ToggleSettingsFormVisibility() => this.settingsFormCache.ToggleVisibility();

        private void TabCreation(TabPage tabPage, AlarmGenTabSave? save = null)
        {
            AlarmGenTab alarmTab = new(this.gridBindContainer, this, this.mainConfig, this.templateHandler, tabPage);
            alarmTab.Init();

            if (save == null)
            {
                alarmTab.Name = Utils.CheckEqualityAndAddNumberAtEnd("AlarmTab", this.alarmTabList.Select(tab => tab.Name));
            }
            if (save != null)
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
            this.SettingsBindings.Clear();

            this.alarmTabList.Clear();
            this.control.tabControl.TabPages.Clear();
        }

        public bool IsDirty() => this.mainConfig.IsDirty() || this.alarmTabList.Any(x => x.IsDirty()) || this.GridScriptHandler.IsDirty() || this.templateHandler.IsDirty();
        public void Wash()
        {
            this.mainConfig.Wash();
            foreach (var tab in this.alarmTabList)
            {
                tab.Wash();
            }
            this.GridScriptHandler.Wash();
            this.templateHandler.IsDirty();
        }

        public object CreateSave()
        {
            var projectSave = new AlarmGenSaveV1()
            {
                ScriptSave = this.GridScriptHandler.CreateSave(),
                TemplateSaves = this.templateHandler.CreateSave()
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

            this.Clear();

            this.GridScriptHandler.LoadSave(loadedSave.ScriptSave);
            this.templateHandler.LoadSave(loadedSave.TemplateSaves);
            GenUtils.CopyJsonFieldsAndProperties(loadedSave.AlarmMainConfig, mainConfig);

            foreach (var tabSave in loadedSave.TabSaves)
            {
                TabPage tabPage = new();
                TabCreation(tabPage, tabSave);
                this.control.tabControl.TabPages.Add(tabPage);
            }

            this.AddConfigurationBindings(this.SettingsBindings);

            //Seems that the Selected event is not called in this case. Doing it manually.
            if (this.control.tabControl.SelectedTab?.Tag is AlarmGenTab tab)
            {
                tab.Selected();
            }
        }

        public void ExportXML(string folderPath)
        {
            AlarmXmlGenerator ioXmlGenerator = new(mainConfig);
            foreach (var tab in alarmTabList)
            {
                ioXmlGenerator.GenerateAlarms(tab.TabPage.Text, tab.TabConfig, this.templateHandler, tab.DeviceDataList);
            }
            ioXmlGenerator.ExportXML(folderPath);
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

            var placeholderForm = new PlaceholderViewerForm(GenPlaceholders.Alarms.PLACEHOLDER_LIST);
            placeholderForm.Show(form);
        }

        public string GetFormLocalizatedName()
        {
            return Locale.ALARM_GEN_FORM;
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var selectedTab = this.control.tabControl.SelectedTab;
            if (selectedTab != null && selectedTab.Tag is AlarmGenTab alarmGenTab)
            {
                return alarmGenTab.ProcessCmdKey(ref msg, keyData);
            }

            return false;
        }

        private void AddConfigurationBindings(SettingsBindings settingsBindings)
        {
            AlarmGenUtils.AddMainConfigBindings(settingsBindings, this.mainConfig);

            AlarmGenUtils.AddTabConfigSettings(settingsBindings,
                this.GetCurrentTabName,
                this.IsAnyTabSelected,
                this.GetCurrentTabConfiguration,
                this.GetTabConfigurationDict);

            AlarmGenUtils.AddTemplateConfigSettings(settingsBindings,
                this.GetActiveTemplateName,
                this.IsTemplateVisible,
                this.GetActiveTemplateConfiguration,
                this.GetTemplateConfigurationDict);
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

        private AlarmTabConfiguration? GetCurrentTabConfiguration()
        {
            return this.control.tabControl.SelectedTab?.Tag is AlarmGenTab genTab ? genTab.TabConfig : null;
        }

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
            var selectedTemplate = this.templateHandler.SelectedTemplate;
            return selectedTemplate == null ? "" : selectedTemplate.Name;
        }

        private bool IsTemplateVisible()
        {
            return shownTemplateForm != null && shownTemplateForm.Visible;
        }

        private AlarmTemplateConfiguration? GetActiveTemplateConfiguration()
        {
            return this.templateHandler.SelectedTemplate?.TemplateConfig;
        }

        private Dictionary<string, ObservableConfiguration> GetTemplateConfigurationDict()
        {
            Dictionary<string, ObservableConfiguration> dict = [];
            foreach (var template in this.templateHandler.BindingList)
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

                foreach (var template in this.templateHandler.BindingList)
                {
                    moduleId = $"TEMPLATE{splitter}{template.Name}";

                    var templateDataEnumerable = template.AlarmGridSave.RowData.Values;
                    AddDataFieldTextReferences(textReferencesList, templateDataEnumerable, moduleId, t => t.AlarmVariable ?? "INVALID", nameof(TemplateData.HmiAlarmText));
                    AddDataFieldTextReferences(textReferencesList, templateDataEnumerable, moduleId, t => t.AlarmVariable ?? "INVALID", nameof(TemplateData.Description));
                }
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

            foreach (var template in this.templateHandler.BindingList)
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
