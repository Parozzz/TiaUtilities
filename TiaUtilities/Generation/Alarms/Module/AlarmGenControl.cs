using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Languages;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Languages;

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
        }

        public void BindConfig(AlarmMainConfiguration mainConfig)
        {
            {
                var button = this.fcConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text);

                    var mainGroup = configForm.Init().ControlWidth(185);
                    mainGroup.AddTextBox().Label(Locale.GENERICS_NAME)
                        .ControlText(mainConfig.FCBlockName)
                        .TextChanged(v => mainConfig.FCBlockName = v);

                    mainGroup.AddTextBox().Label(Locale.GENERICS_NUMBER)
                        .ControlText(mainConfig.FCBlockNumber)
                        .UIntChanged(v => mainConfig.FCBlockNumber = v);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = this.segmentNameConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text) { ControlWidth = 500 };

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_SEGMENT_NAME_ONE_EACH)
                         .ControlText(mainConfig.OneEachSegmentName)
                         .TextChanged(v => mainConfig.OneEachSegmentName = v);

                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_SEGMENT_NAME_ONE_EACH_EMPTY)
                         .ControlText(mainConfig.OneEachEmptyAlarmSegmentName)
                         .TextChanged(v => mainConfig.OneEachEmptyAlarmSegmentName = v);

                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_SEGMENT_NAME_GROUP_EACH)
                         .ControlText(mainConfig.GroupSegmentName)
                         .TextChanged(v => mainConfig.GroupSegmentName = v);

                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_SEGMENT_NAME_GROUP_EACH_EMPTY)
                         .ControlText(mainConfig.GroupEmptyAlarmSegmentName)
                         .TextChanged(v => mainConfig.GroupEmptyAlarmSegmentName = v);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = this.textListConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(Locale.ALARM_CONFIG_TEXT_LIST);

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_TEXT_LIST_FULL)
                         .ControlText(mainConfig.AlarmTextInList)
                         .TextChanged(v => mainConfig.AlarmTextInList = v);

                    mainGroup.AddTextBox().Label(Locale.ALARM_CONFIG_TEXT_LIST_EMPTY)
                         .ControlText(mainConfig.EmptyAlarmTextInList)
                         .TextChanged(v => mainConfig.EmptyAlarmTextInList = v);

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
