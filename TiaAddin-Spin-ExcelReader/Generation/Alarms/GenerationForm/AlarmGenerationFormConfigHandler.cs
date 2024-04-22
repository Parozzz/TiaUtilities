using System;
using System.Windows.Forms;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Generation.GridHandler;

namespace TiaXmlReader.Generation.Alarms.GenerationForm
{
    public class AlarmGenerationFormConfigHandler
    {
        private readonly AlarmGenerationForm form;
        private readonly AlarmConfiguration config;
        private readonly GridHandler<AlarmConfiguration, DeviceData> deviceDataGridHandler;
        private readonly GridHandler<AlarmConfiguration, AlarmData> alarmDataGridHandler;

        public AlarmGenerationFormConfigHandler(AlarmGenerationForm form, AlarmConfiguration config, 
                                                    GridHandler<AlarmConfiguration, DeviceData> deviceDataGridHandler, 
                                                    GridHandler<AlarmConfiguration, AlarmData> alarmDataGridHandler)
        {
            this.form = form;
            this.config = config;
            this.deviceDataGridHandler = deviceDataGridHandler;
            this.alarmDataGridHandler = alarmDataGridHandler;
        }

        public void Init()
        {
            var comboBox = form.partitionTypeComboBox;
            comboBox.SelectedValue = config.PartitionType;
            comboBox.SelectionChangeCommitted += (sender, args) => { config.PartitionType = (AlarmPartitionType)form.groupingTypeComboBox.SelectedValue; };

            comboBox = form.groupingTypeComboBox;
            comboBox.SelectedValue = config.GroupingType;
            comboBox.SelectionChangeCommitted += (sender, args) => { config.GroupingType = (AlarmGroupingType)form.partitionTypeComboBox.SelectedValue; };

            form.fcConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("FC");
                configForm.AddTextBoxLine("Nome")
                    .ControlText(config.FCBlockName)
                    .TextChanged(v => config.FCBlockName = v);

                configForm.AddTextBoxLine("Numero")
                    .ControlText(config.FCBlockNumber)
                    .UIntChanged(v => config.FCBlockNumber = v);

                configForm.AddCheckBoxLine("Genera prima bobina")
                    .Value(config.CoilFirst)
                    .CheckedChanged(v => config.CoilFirst = v);

                SetupConfigForm(form.fcConfigButton, configForm);
            };

            form.alarmGenerationConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Generazione Allarmi");
                configForm.AddTextBoxLine("Num. Partenza")
                    .ControlText(config.StartingAlarmNum)
                    .UIntChanged(v => config.StartingAlarmNum = v);

                configForm.AddTextBoxLine("Formato")
                    .ControlText(config.AlarmNumFormat)
                    .TextChanged(v => config.AlarmNumFormat = v);

                configForm.AddTextBoxLine("Anti slittamento")
                    .ControlText(config.AntiSlipNumber)
                    .UIntChanged(v => config.AntiSlipNumber = v);

                configForm.AddTextBoxLine("Salta a fine gruppo")
                    .ControlText(config.SkipNumberAfterGroup)
                    .UIntChanged(v => config.SkipNumberAfterGroup = v);

                configForm.AddCheckBoxLine("Genera vuoti alla fine")
                    .Value(config.GenerateEmptyAlarmAntiSlip)
                    .CheckedChanged(v => config.GenerateEmptyAlarmAntiSlip = v);

                configForm.AddTextBoxLine("Vuoti alla fine")
                    .ControlText(config.EmptyAlarmAtEnd)
                    .UIntChanged(v => config.EmptyAlarmAtEnd = v);

                configForm.AddTextBoxLine("Indirizzo vuoti")
                    .ControlText(config.EmptyAlarmContactAddress)
                    .TextChanged(v => config.EmptyAlarmContactAddress = v);

                SetupConfigForm(form.alarmGenerationConfigButton, configForm);
            };

            form.fieldDefaultValueConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Valori default campi");
                configForm.AddTextBoxLine("Bobina")
                    .ControlText(config.DefaultCoilAddress)
                    .TextChanged(v => config.DefaultCoilAddress = v);

                configForm.AddTextBoxLine("Set")
                    .ControlText(config.DefaultSetCoilAddress)
                    .TextChanged(v => config.DefaultSetCoilAddress = v);

                configForm.AddTextBoxLine("Timer")
                    .ControlText(config.DefaultTimerAddress)
                    .TextChanged(v => config.DefaultTimerAddress = v);

                configForm.AddComboBoxLine("Timer tipo")
                    .ControlText(config.DefaultTimerType)
                    .Items(new string[] { "TON", "TOF" })
                    .TextChanged(v => config.DefaultTimerType = v);

                configForm.AddTextBoxLine("Timer valore")
                    .ControlText(config.DefaultTimerValue)
                    .TextChanged(v => config.DefaultTimerValue = v);

                SetupConfigForm(form.fieldDefaultValueConfigButton, configForm);
            };

            form.fieldPrefixConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Prefissi campi");
                configForm.AddTextBoxLine("Indirizzo allarme")
                    .ControlText(config.AlarmAddressPrefix)
                    .TextChanged(v => config.AlarmAddressPrefix = v);

                configForm.AddTextBoxLine("Indirizzo bobina")
                    .ControlText(config.CoilAddressPrefix)
                    .TextChanged(v => config.CoilAddressPrefix = v);

                configForm.AddTextBoxLine("Indirizzo set")
                    .ControlText(config.SetCoilAddressPrefix)
                    .TextChanged(v => config.SetCoilAddressPrefix = v);

                configForm.AddTextBoxLine("Indirizzo timer")
                    .ControlText(config.TimerAddressPrefix)
                    .TextChanged(v => config.TimerAddressPrefix = v);

                SetupConfigForm(form.fieldPrefixConfigButton, configForm);
            };

            form.segmentNameConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Nomi segmenti generati")
                {
                    ControlWidth = 500
                };

                configForm.AddTextBoxLine("Uno per segmento")
                    .ControlText(config.OneEachSegmentName)
                    .TextChanged(v => config.OneEachSegmentName = v);

                configForm.AddTextBoxLine("Uno per segmento (Vuoti)")
                    .ControlText(config.OneEachEmptyAlarmSegmentName)
                    .TextChanged(v => config.OneEachEmptyAlarmSegmentName = v);

                configForm.AddTextBoxLine("Gruppo per segmento")
                    .ControlText(config.GroupSegmentName)
                    .TextChanged(v => config.GroupSegmentName = v);

                configForm.AddTextBoxLine("Gruppo per segmento (Vuoti)")
                    .ControlText(config.GroupEmptyAlarmSegmentName)
                    .TextChanged(v => config.GroupEmptyAlarmSegmentName = v);

                SetupConfigForm(form.segmentNameConfigButton, configForm);
            }; 
        }

        private void SetupConfigForm(Control button, ConfigForm configForm)
        {
            configForm.StartShowingAtControl(button);
            configForm.Init();
            configForm.Show(form);

            this.deviceDataGridHandler.Refresh();
            this.alarmDataGridHandler.Refresh();
        }

    }
}
