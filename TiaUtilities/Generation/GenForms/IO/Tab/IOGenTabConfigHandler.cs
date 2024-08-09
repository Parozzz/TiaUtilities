using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Generation.GenForms.Alarm.Controls;
using TiaUtilities.Generation.IO;
using TiaXmlReader.Generation.Configuration;
using TiaUtilities.Generation.GenForms.IO.Tab;

namespace TiaUtilities.Generation.GenForms.IO.Tab
{
    public class IOGenTabConfigHandler(IOGenTabControl tabControl, IOTabConfiguration tabConfig)
    {
        public void Init()
        {
            {
                var button = tabControl.fcConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text) { ControlWidth = 150 };

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().LocalizedLabel("GENERICS_NAME")
                         .ControlText(tabConfig.FCBlockName)
                         .TextChanged(str => tabConfig.FCBlockName = str);

                    mainGroup.AddTextBox().LocalizedLabel("GENERICS_NUMBER")
                         .ControlText(tabConfig.FCBlockNumber)
                         .UIntChanged(num => tabConfig.FCBlockNumber = num);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = tabControl.segmentNameConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text) { ControlWidth = 400 };

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_SEGMENT_BIT_DIVISION")
                         .ControlText(tabConfig.SegmentNameBitGrouping)
                         .TextChanged(str => tabConfig.SegmentNameBitGrouping = str);

                    mainGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_SEGMENT_BYTE_DIVISION")
                         .ControlText(tabConfig.SegmentNameByteGrouping)
                         .TextChanged(str => tabConfig.SegmentNameByteGrouping = str);

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
