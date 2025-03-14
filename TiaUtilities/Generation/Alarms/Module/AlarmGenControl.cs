﻿using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Languages;
using TiaXmlReader;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.Configuration;

namespace TiaUtilities.Generation.GenModules.Alarm
{
    public partial class AlarmGenControl : UserControl
    {
        public AlarmGenControl()
        { //This is a subordinated control. Init is called in the class that add this.
            InitializeComponent();

            this.Dock = DockStyle.Fill;
            this.tabControl.RequireConfirmationBeforeClosing = true;

            Translate();
        }

        private void Translate()
        {
            this.fcConfigButton.Text = Locale.ALARM_CONFIG_FC;
            this.segmentNameConfigButton.Text = Locale.ALARM_CONFIG_SEGMENT_NAME;
            this.textListConfigButton.Text = Locale.ALARM_CONFIG_TEXT_LIST;

            this.enableCustomVarLabel.Text = Locale.ALARM_CONFIG_ENABLE_CUSTOM_VAR;
            this.enableTimerLabel.Text = Locale.ALARM_CONFIG_ENABLE_TIMER;
        }

        public void BindConfig(AlarmMainConfiguration mainConfig)
        {
            this.enableCustomVarToggleButton.Checked = mainConfig.EnableCustomVariable;
            this.enableCustomVarToggleButton.CheckedChanged += (sender, args) =>
            {
                mainConfig.EnableCustomVariable = this.enableCustomVarToggleButton.Checked;
            };

            this.enableTimerToggleButton.Checked = mainConfig.EnableTimer;
            this.enableTimerToggleButton.CheckedChanged += (sender, args) =>
            {
                mainConfig.EnableTimer = this.enableTimerToggleButton.Checked;
            };

            {
                var button = this.fcConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text);
                    configForm.SetConfiguration(mainConfig, MainForm.Settings.PresetAlarmMainConfiguration);

                    var mainGroup = configForm.Init().ControlWidth(185);
                    mainGroup.AddTextBox().Label(Locale.GENERICS_NAME)
                        .BindText(() => mainConfig.FCBlockName);

                    mainGroup.AddTextBox().Label(Locale.GENERICS_NUMBER)
                        .BindUInt(() => mainConfig.FCBlockNumber);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = this.segmentNameConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text) { ControlWidth = 500 };
                    configForm.SetConfiguration(mainConfig, MainForm.Settings.PresetAlarmMainConfiguration);

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_SEGMENT_NAME_ONE_EACH)
                         .BindText(() => mainConfig.OneEachSegmentName);

                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_SEGMENT_NAME_ONE_EACH_EMPTY)
                         .BindText(() => mainConfig.OneEachEmptyAlarmSegmentName);

                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_SEGMENT_NAME_GROUP_EACH)
                         .BindText(() => mainConfig.GroupSegmentName);

                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_SEGMENT_NAME_GROUP_EACH_EMPTY)
                         .BindText(() => mainConfig.GroupEmptyAlarmSegmentName);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = this.textListConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text);
                    configForm.SetConfiguration(mainConfig, MainForm.Settings.PresetAlarmMainConfiguration);

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_TEXT_LIST_FULL)
                        .BindText(() => mainConfig.AlarmTextInList);
                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_TEXT_LIST_EMPTY)
                        .BindText(() => mainConfig.EmptyAlarmTextInList);

                    SetupConfigForm(button, configForm);
                };
            }
        }

        private static void SetupConfigForm(Control button, ConfigForm configForm)
        {
            configForm.StartShowingAtControl(button);
            //configForm.Init();
            configForm.Show(button.FindForm());
        }
    }
}
