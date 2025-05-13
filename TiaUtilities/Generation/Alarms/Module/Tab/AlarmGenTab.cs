using TiaUtilities.Generation.Alarms.Module.Template;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.GridHandler.CustomColumns;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.Placeholders;

namespace TiaUtilities.Generation.Alarms.Module.Tab
{
    public class AlarmGenTab : ICleanable, ISaveable<AlarmGenTabSave>
    {
        private readonly GridBindContainer gridBindContainer;
        private readonly AlarmGenModule module;
        private readonly AlarmMainConfiguration mainConfig;
        private readonly AlarmGenTemplateHandler templateHandler;

        public TabPage TabPage { get; init; }
        public AlarmTabConfiguration TabConfig { get; init; }

        private readonly GridDataPreviewer<DeviceData> deviceDataPreview;
        private readonly GridHandler<DeviceData> deviceGridHandler;

        public AlarmGenTabControl TabControl { get; init; }
        public List<DeviceData> DeviceDataList { get => new(deviceGridHandler.DataSource.GetNotEmptyClonedDataDict().Keys); } //Return CLONED data, otherwise operations on the xml generation will affect the table!

        private bool dirty = false;

        public AlarmGenTab(GridBindContainer bindContainer, AlarmGenModule module, AlarmMainConfiguration mainConfig, AlarmGenTemplateHandler templateHandler, TabPage tabPage)
        {
            this.module = module;
            this.gridBindContainer = bindContainer;
            this.mainConfig = mainConfig;
            this.templateHandler = templateHandler;
            this.TabPage = tabPage;

            this.TabConfig = new();
            GenUtils.CopyJsonFieldsAndProperties(MainForm.Settings.PresetAlarmTabConfiguration, this.TabConfig);

            AlarmGenPlaceholdersHandler placeholdersHandler = new(mainConfig, this.TabConfig);
            this.deviceDataPreview = new();
            this.deviceGridHandler = new(MainForm.Settings.GridSettings, this.gridBindContainer, this.deviceDataPreview, placeholdersHandler) { RowCount = 499 };

            this.TabControl = new(bindContainer.GridScriptHandler.ErrorThread, this.templateHandler, deviceGridHandler.DataGridView);
        }

        public void Init()
        {
            this.TabControl.Init(); //This before configHandler.
            this.TabControl.BindConfig(this.gridBindContainer, this.mainConfig, this.TabConfig, this.module.TabConfigurations);

            #region DEVICE_GRID_SETUP
            this.deviceGridHandler.Events.ExcelDragPreview += (sender, args) => GridUtils.DragPreview(args, deviceGridHandler);
            this.deviceGridHandler.Events.ExcelDragDone += (sender, args) => GridUtils.DragDone(args, deviceGridHandler);

            //Columns before GridHandler.Init()
            this.deviceGridHandler.AddTextBoxColumn(DeviceData.NAME, 125);

            SuggestionTextBoxColumn templateSuggestionColumn = new();
            templateSuggestionColumn.SetGetItemsFunc(templateHandler.GetAllNames);
            this.deviceGridHandler.AddCustomColumn(templateSuggestionColumn, DeviceData.TEMPLATE, 200);

            this.deviceGridHandler.AddTextBoxColumn(DeviceData.DESCRIPTION, 0);

            this.deviceGridHandler.Init();
            this.deviceDataPreview.Function = (column, deviceData) => null;

            this.deviceGridHandler.ScriptVariableList.Add(GridScriptVariable.ReadOnlyValue("tabName", () => this.TabPage.Text));
            #endregion

            this.TabPage.TextChanged += (sender, args) => this.dirty = true;

            this.Translate();
        }

        public void Selected()
        {
            this.gridBindContainer.ChangeBind(this.deviceGridHandler);
        }

        private void Translate()
        {
            TabControl.Translate();
        }

        public bool IsDirty() => this.dirty || this.TabConfig.IsDirty() || this.deviceGridHandler.IsDirty();
        public void Wash()
        {
            this.dirty = false;
            this.TabConfig.Wash();
            this.deviceGridHandler.Wash();
        }

        public AlarmGenTabSave CreateSave()
        {
            AlarmGenTabSave save = new()
            {
                TabConfig = TabConfig,
                Name = TabPage.Text,
                DeviceGrid = deviceGridHandler.CreateSave()
            };

            GenUtils.CopyJsonFieldsAndProperties(TabConfig, save.TabConfig);
            return save;
        }

        public void LoadSave(AlarmGenTabSave save)
        {
            TabPage.Text = save.Name;
            GenUtils.CopyJsonFieldsAndProperties(save.TabConfig, TabConfig);

            deviceGridHandler.LoadSave(save.DeviceGrid);
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return deviceGridHandler.ProcessCmdKey(ref msg, keyData);
        }
    }
}
