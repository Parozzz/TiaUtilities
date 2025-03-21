using TiaUtilities.Generation.GenModules.Alarm.Tab;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.Placeholders;
using TiaXmlReader;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler;

namespace TiaUtilities.Generation.Alarms.Module.Tab
{
    public class AlarmGenTab : ICleanable, ISaveable<AlarmGenTabSave>
    {
        private readonly GridBindContainer gridBindContainer;
        private readonly AlarmGenModule module;
        private readonly AlarmMainConfiguration mainConfig;

        public TabPage TabPage { get; init; }
        public AlarmTabConfiguration TabConfig { get; init; }

        private readonly GridDataPreviewer<DeviceData> deviceDataPreview;
        private readonly GridHandler<DeviceData> deviceGridHandler;
        private readonly AlarmDataGridWrapper alarmDataGridWrapper;

        public AlarmGenTabControl TabControl { get; init; }

        public List<DeviceData> DeviceDataList { get => new(deviceGridHandler.DataSource.GetNotEmptyClonedDataDict().Keys); } //Return CLONED data, otherwise operations on the xml generation will affect the table!
        public List<AlarmData> AlarmDataList { get => this.alarmDataGridWrapper.AlarmDataList; } //Return CLONED data, otherwise operations on the xml generation will affect the table!

        private bool dirty = false;

        public AlarmGenTab(GridBindContainer bindContainer, AlarmGenModule module, AlarmMainConfiguration mainConfig, TabPage tabPage)
        {
            this.module = module;
            this.gridBindContainer = bindContainer;
            this.mainConfig = mainConfig;
            this.TabPage = tabPage;

            this.TabConfig = new();
            GenUtils.CopyJsonFieldsAndProperties(MainForm.Settings.PresetAlarmTabConfiguration, this.TabConfig);
            
            AlarmGenPlaceholdersHandler placeholdersHandler = new(mainConfig, this.TabConfig);
            this.deviceDataPreview = new();
            this.deviceGridHandler = new(MainForm.Settings.GridSettings, this.gridBindContainer, this.deviceDataPreview, placeholdersHandler) { RowCount = 499 };
            
            this.alarmDataGridWrapper = new(placeholdersHandler, this.gridBindContainer);

            TabControl = new(deviceGridHandler.DataGridView, this.alarmDataGridWrapper.GetDataGridView());
        }

        public void Init()
        {
            TabControl.Init(); //This before configHandler.
            TabControl.BindConfig(this.gridBindContainer, this.mainConfig, this.TabConfig);

            #region DEVICE_GRID_SETUP
            this.deviceGridHandler.Events.ExcelDragPreview += (sender, args) => GridUtils.DragPreview(args, deviceGridHandler);
            this.deviceGridHandler.Events.ExcelDragDone += (sender, args) => GridUtils.DragDone(args, deviceGridHandler);

            //Columns before GridHandler.Init()
            this.deviceGridHandler.AddTextBoxColumn(DeviceData.NAME, 125);
            this.deviceGridHandler.AddTextBoxColumn(DeviceData.DESCRIPTION, 0);

            this.deviceGridHandler.Init();
            this.deviceDataPreview.Function = (column, deviceData) => null;

            this.deviceGridHandler.ScriptVariableList.Add(GridScriptVariable.ReadOnlyValue("tabName", () => this.TabPage.Text));
            #endregion

            #region ALARM_GRID_WRAPPER_SETUP
            this.alarmDataGridWrapper.Init(this.mainConfig, this.TabConfig);
            this.alarmDataGridWrapper.AddScriptVariable(GridScriptVariable.ReadOnlyValue("tabName", () => this.TabPage.Text));
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

        public bool IsDirty() => this.dirty || this.TabConfig.IsDirty() || this.deviceGridHandler.IsDirty() || this.alarmDataGridWrapper.IsDirty();
        public void Wash()
        {
            this.dirty = false;
            this.TabConfig.Wash();
            this.deviceGridHandler.Wash();
            this.alarmDataGridWrapper.Wash();
        }

        public AlarmGenTabSave CreateSave()
        {
            AlarmGenTabSave save = new()
            {
                TabConfig = TabConfig,
                Name = TabPage.Text,
                DeviceGrid = deviceGridHandler.CreateSave(),
                AlarmGrid = this.alarmDataGridWrapper.CreateSave()
            };

            GenUtils.CopyJsonFieldsAndProperties(TabConfig, save.TabConfig);
            return save;
        }

        public void LoadSave(AlarmGenTabSave save)
        {
            TabPage.Text = save.Name;
            GenUtils.CopyJsonFieldsAndProperties(save.TabConfig, TabConfig);

            this.alarmDataGridWrapper.LoadSave(save.AlarmGrid);
            deviceGridHandler.LoadSave(save.DeviceGrid);
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return deviceGridHandler.ProcessCmdKey(ref msg, keyData) || this.alarmDataGridWrapper.ProcessCmdKey(ref msg, keyData);
        }
    }
}
