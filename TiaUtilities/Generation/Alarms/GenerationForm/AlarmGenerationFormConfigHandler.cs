using System;
using System.Windows.Forms;
using TiaUtilities.Generation.Configuration.Utility;
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
            comboBox.SelectionChangeCommitted += (sender, args) => config.PartitionType = (AlarmPartitionType)(comboBox.SelectedValue ?? default(AlarmPartitionType));

            comboBox = form.groupingTypeComboBox;
            comboBox.SelectedValue = config.GroupingType;
            comboBox.SelectionChangeCommitted += (sender, args) => config.GroupingType = (AlarmGroupingType)(comboBox.SelectedValue ?? default(AlarmGroupingType));

            form.fcConfigButton.Click += (sender, args) =>
            {
                var configForm = new ConfigForm("FC");

                var mainGroup = configForm.Init();
                mainGroup.AddTextBox().LabelText("Nome")
                    .ControlText(config.FCBlockName)
                    .TextChanged(v => config.FCBlockName = v);

                mainGroup.AddTextBox().LabelText("Numero")
                    .ControlText(config.FCBlockNumber)
                    .UIntChanged(v => config.FCBlockNumber = v);

                mainGroup.AddCheckBox().LabelText("Genera prima bobina")
                    .Value(config.CoilFirst)
                    .CheckedChanged(v => config.CoilFirst = v);

                SetupConfigForm(form.fcConfigButton, configForm);
            };

            form.alarmGenerationConfigButton.Click += (sender, args) =>
            {
                var configForm = new ConfigForm("Generazione Allarmi");

                var mainGroup = configForm.Init();
                mainGroup.AddTextBox().LabelText("Num. Partenza")
                     .ControlText(config.StartingAlarmNum)
                     .UIntChanged(v => config.StartingAlarmNum = v);

                mainGroup.AddTextBox().LabelText("Formato")
                     .ControlText(config.AlarmNumFormat)
                     .TextChanged(v => config.AlarmNumFormat = v);

                mainGroup.AddTextBox().LabelText("Anti-Slittamento")
                     .ControlText(config.AntiSlipNumber)
                     .UIntChanged(v => config.AntiSlipNumber = v);

                mainGroup.AddTextBox().LabelText("Salta a fine gruppo")
                     .ControlText(config.SkipNumberAfterGroup)
                     .UIntChanged(v => config.SkipNumberAfterGroup = v);

                SetupConfigForm(form.alarmGenerationConfigButton, configForm);
            };

            form.emptyAlarmGenerationConfigButton.Click += (sender, args) =>
            {
                var configForm = new ConfigForm("Generazione Allarmi Vuoti");

                var mainGroup = configForm.Init();
                mainGroup.AddCheckBox().LabelText("Genera in anti-slittamento")
                     .Value(config.GenerateEmptyAlarmAntiSlip)
                     .CheckedChanged(v => config.GenerateEmptyAlarmAntiSlip = v);

                mainGroup.AddTextBox().LabelText("Num. alla fine")
                     .ControlText(config.EmptyAlarmAtEnd)
                     .UIntChanged(v => config.EmptyAlarmAtEnd = v);

                mainGroup.AddTextBox().LabelText("Indirizzo allarme")
                     .ControlText(config.EmptyAlarmContactAddress)
                     .TextChanged(v => config.EmptyAlarmContactAddress = v);

                mainGroup.AddTextBox().LabelText("Indirizzo timer")
                     .ControlText(config.EmptyAlarmTimerAddress)
                     .TextChanged(v => config.EmptyAlarmTimerAddress = v);

                mainGroup.AddComboBox().LabelText("Tipo timer")
                     .Items(new string[] { "TON", "TOF" })
                     .ControlText(config.EmptyAlarmTimerType)
                     .TextChanged(v => config.EmptyAlarmTimerType = v);

                mainGroup.AddTextBox().LabelText("Valore timer")
                     .ControlText(config.EmptyAlarmTimerValue)
                     .TextChanged(v => config.EmptyAlarmTimerValue = v);

                SetupConfigForm(form.alarmGenerationConfigButton, configForm);
            };

            form.fieldDefaultValueConfigButton.Click += (sender, args) =>
            {
                var configForm = new ConfigForm("Valori default campi");

                var mainGroup = configForm.Init();
                mainGroup.AddTextBox().LabelText("Bobina")
                     .ControlText(config.DefaultCoilAddress)
                     .TextChanged(v => config.DefaultCoilAddress = v);

                mainGroup.AddTextBox().LabelText("Set")
                     .ControlText(config.DefaultSetCoilAddress)
                     .TextChanged(v => config.DefaultSetCoilAddress = v);

                mainGroup.AddTextBox().LabelText("Timer")
                     .ControlText(config.DefaultTimerAddress)
                     .TextChanged(v => config.DefaultTimerAddress = v);

                mainGroup.AddComboBox().LabelText("Timer tipo")
                     .ControlText(config.DefaultTimerType)
                     .Items(new string[] { "TON", "TOF" })
                     .TextChanged(v => config.DefaultTimerType = v);

                mainGroup.AddTextBox().LabelText("Timer valore")
                     .ControlText(config.DefaultTimerValue)
                     .TextChanged(v => config.DefaultTimerValue = v);

                SetupConfigForm(form.fieldDefaultValueConfigButton, configForm);
            };

            form.fieldPrefixConfigButton.Click += (sender, args) =>
            {
                var configForm = new ConfigForm("Prefissi campi");

                var mainGroup = configForm.Init();
                mainGroup.AddTextBox().LabelText("Indirizzo allarme")
                     .ControlText(config.AlarmAddressPrefix)
                     .TextChanged(v => config.AlarmAddressPrefix = v);

                mainGroup.AddTextBox().LabelText("Indirizzo bobina")
                     .ControlText(config.CoilAddressPrefix)
                     .TextChanged(v => config.CoilAddressPrefix = v);

                mainGroup.AddTextBox().LabelText("Indirizzo set")
                     .ControlText(config.SetCoilAddressPrefix)
                     .TextChanged(v => config.SetCoilAddressPrefix = v);

                mainGroup.AddTextBox().LabelText("Indirizzo timer")
                     .ControlText(config.TimerAddressPrefix)
                     .TextChanged(v => config.TimerAddressPrefix = v);

                SetupConfigForm(form.fieldPrefixConfigButton, configForm);
            };

            form.segmentNameConfigButton.Click += (sender, args) =>
            {
                var configForm = new ConfigForm("Nomi segmenti generati")
                {
                    ControlWidth = 500
                };

                var mainGroup = configForm.Init();
                mainGroup.AddTextBox().LabelText("Uno per segmento")
                     .ControlText(config.OneEachSegmentName)
                     .TextChanged(v => config.OneEachSegmentName = v);

                mainGroup.AddTextBox().LabelText("Uno per segmento (Vuoti)")
                     .ControlText(config.OneEachEmptyAlarmSegmentName)
                     .TextChanged(v => config.OneEachEmptyAlarmSegmentName = v);

                mainGroup.AddTextBox().LabelText("Gruppo per segmento")
                     .ControlText(config.GroupSegmentName)
                     .TextChanged(v => config.GroupSegmentName = v);

                mainGroup.AddTextBox().LabelText("Gruppo per segmento (Vuoti)")
                     .ControlText(config.GroupEmptyAlarmSegmentName)
                     .TextChanged(v => config.GroupEmptyAlarmSegmentName = v);

                SetupConfigForm(form.segmentNameConfigButton, configForm);
            };

            form.textListConfigButton.Click += (sender, args) =>
            {
                var configForm = new ConfigForm("Lista testi allarmi");

                var mainGroup = configForm.Init();
                mainGroup.AddTextBox().LabelText("Testo allarme")
                     .ControlText(config.AlarmTextInList)
                     .TextChanged(v => config.AlarmTextInList = v);

                mainGroup.AddTextBox().LabelText("Testo allarme vuoto")
                     .ControlText(config.EmptyAlarmTextInList)
                     .TextChanged(v => config.EmptyAlarmTextInList = v);

                SetupConfigForm(form.textListConfigButton, configForm);
            };
        }

        private void SetupConfigForm(Control button, ConfigForm configForm)
        {
            configForm.StartShowingAtControl(button);
            //configForm.Init();
            configForm.Show(form);
        }

    }
}
