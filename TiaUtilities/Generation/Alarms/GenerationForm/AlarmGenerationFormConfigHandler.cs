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
            comboBox.SelectionChangeCommitted += (sender, args) => config.PartitionType = (AlarmPartitionType)form.partitionTypeComboBox.SelectedValue;

            comboBox = form.groupingTypeComboBox;
            comboBox.SelectedValue = config.GroupingType;
            comboBox.SelectionChangeCommitted += (sender, args) => config.GroupingType = (AlarmGroupingType)form.groupingTypeComboBox.SelectedValue;

            form.fcConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("FC");
                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Nome")
                    .ControlText(config.FCBlockName)
                    .TextChanged(v => config.FCBlockName = v);

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Numero")
                    .ControlText(config.FCBlockNumber)
                    .UIntChanged(v => config.FCBlockNumber = v);

                configForm.AddLine(ConfigFormLineTypes.CHECK_BOX).LabelText("Genera prima bobina")
                    .Value(config.CoilFirst)
                    .CheckedChanged(v => config.CoilFirst = v);

                SetupConfigForm(form.fcConfigButton, configForm);
            };

            form.alarmGenerationConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Generazione Allarmi");
                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Num. Partenza")
                    .ControlText(config.StartingAlarmNum)
                    .UIntChanged(v => config.StartingAlarmNum = v);

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Formato")
                    .ControlText(config.AlarmNumFormat)
                    .TextChanged(v => config.AlarmNumFormat = v);

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Anti-Slittamento")
                    .ControlText(config.AntiSlipNumber)
                    .UIntChanged(v => config.AntiSlipNumber = v);

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Salta a fine gruppo")
                    .ControlText(config.SkipNumberAfterGroup)
                    .UIntChanged(v => config.SkipNumberAfterGroup = v);

                SetupConfigForm(form.alarmGenerationConfigButton, configForm);
            };

            form.emptyAlarmGenerationConfigButton.Click += (sender, args) =>
            {
                var configForm = new ConfigForm("Generazione Allarmi Vuoti");
                configForm.AddLine(ConfigFormLineTypes.CHECK_BOX).LabelText("Genera in anti-slittamento")
                    .Value(config.GenerateEmptyAlarmAntiSlip)
                    .CheckedChanged(v => config.GenerateEmptyAlarmAntiSlip = v);

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Num. alla fine")
                    .ControlText(config.EmptyAlarmAtEnd)
                    .UIntChanged(v => config.EmptyAlarmAtEnd = v);

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Indirizzo allarme")
                    .ControlText(config.EmptyAlarmContactAddress)
                    .TextChanged(v => config.EmptyAlarmContactAddress = v);

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Indirizzo timer")
                    .ControlText(config.EmptyAlarmTimerAddress)
                    .TextChanged(v => config.EmptyAlarmTimerAddress = v);

                configForm.AddLine(ConfigFormLineTypes.COMBO_BOX).LabelText("Tipo timer")
                    .Items(new string[] { "TON", "TOF" })
                    .ControlText(config.EmptyAlarmTimerType)
                    .TextChanged(v => config.EmptyAlarmTimerType = v);

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Valore timer")
                    .ControlText(config.EmptyAlarmTimerValue)
                    .TextChanged(v => config.EmptyAlarmTimerValue = v);

                SetupConfigForm(form.alarmGenerationConfigButton, configForm);
            };

            form.fieldDefaultValueConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Valori default campi");
                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Bobina")
                    .ControlText(config.DefaultCoilAddress)
                    .TextChanged(v => config.DefaultCoilAddress = v);

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Set")
                    .ControlText(config.DefaultSetCoilAddress)
                    .TextChanged(v => config.DefaultSetCoilAddress = v);

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Timer")
                    .ControlText(config.DefaultTimerAddress)
                    .TextChanged(v => config.DefaultTimerAddress = v);

                configForm.AddLine(ConfigFormLineTypes.COMBO_BOX).LabelText("Timer tipo")
                    .ControlText(config.DefaultTimerType)
                    .Items(new string[] { "TON", "TOF" })
                    .TextChanged(v => config.DefaultTimerType = v);

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Timer valore")
                    .ControlText(config.DefaultTimerValue)
                    .TextChanged(v => config.DefaultTimerValue = v);

                SetupConfigForm(form.fieldDefaultValueConfigButton, configForm);
            };

            form.fieldPrefixConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Prefissi campi");
                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Indirizzo allarme")
                    .ControlText(config.AlarmAddressPrefix)
                    .TextChanged(v => config.AlarmAddressPrefix = v);

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Indirizzo bobina")
                    .ControlText(config.CoilAddressPrefix)
                    .TextChanged(v => config.CoilAddressPrefix = v);

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Indirizzo set")
                    .ControlText(config.SetCoilAddressPrefix)
                    .TextChanged(v => config.SetCoilAddressPrefix = v);

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Indirizzo timer")
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

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Uno per segmento")
                    .ControlText(config.OneEachSegmentName)
                    .TextChanged(v => config.OneEachSegmentName = v);

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Uno per segmento (Vuoti)")
                    .ControlText(config.OneEachEmptyAlarmSegmentName)
                    .TextChanged(v => config.OneEachEmptyAlarmSegmentName = v);

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Gruppo per segmento")
                    .ControlText(config.GroupSegmentName)
                    .TextChanged(v => config.GroupSegmentName = v);

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Gruppo per segmento (Vuoti)")
                    .ControlText(config.GroupEmptyAlarmSegmentName)
                    .TextChanged(v => config.GroupEmptyAlarmSegmentName = v);

                SetupConfigForm(form.segmentNameConfigButton, configForm);
            };

            form.textListConfigButton.Click += (sender, args) =>
            {
                var configForm = new ConfigForm("Lista testi allarmi");
                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Testo allarme")
                    .ControlText(config.AlarmTextInList)
                    .TextChanged(v => config.AlarmTextInList = v);

                configForm.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Testo allarme vuoto")
                    .ControlText(config.EmptyAlarmTextInList)
                    .TextChanged(v => config.EmptyAlarmTextInList = v);

                SetupConfigForm(form.textListConfigButton, configForm);
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
