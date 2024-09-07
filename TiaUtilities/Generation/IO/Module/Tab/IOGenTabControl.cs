using TiaXmlReader.Languages;
using TiaUtilities.Generation.IO;
using TiaXmlReader.Generation.Configuration;
using TiaUtilities.Generation.Configuration.Utility;

namespace TiaUtilities.Generation.GenModules.IO.Tab
{
    public partial class IOGenTabControl : UserControl
    {
        private readonly DataGridView dataGridView;

        public IOGenTabControl(DataGridView dataGridView)
        {
            this.dataGridView = dataGridView;
            InitializeComponent();
            //This is a subordinated control. Init is called in the class that add this.
        }

        public void Init()
        {
            this.AutoSize = true; //AutoSize set here otherwise while doing the UI, everything will be shrinked to minimun (So useless)
            this.Dock = DockStyle.Fill;

            this.generationTableLayout.Controls.Add(this.dataGridView);

            Translate();
        }

        private void Translate()
        {
            this.fcConfigButton.Text = Localization.Get("IO_GEN_CONFIG_FC");
            this.segmentNameConfigButton.Text = Localization.Get("IO_GEN_CONFIG_SEGMENT");
        }

        public void BindConfig(IOTabConfiguration tabConfig)
        {
            {
                var button = this.fcConfigButton;
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
                var button = this.segmentNameConfigButton;
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
