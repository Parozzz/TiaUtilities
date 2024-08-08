using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaUtilities.Generation.GenForms.Alarm;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Languages;

namespace TiaUtilities.Generation.GenForms.Alarm.Controls
{
    public partial class DeviceAlarmTabControl : UserControl
    {
        private readonly DataGridView dataGridViewLeft;
        private readonly DataGridView dataGridViewRight;

        public DeviceAlarmTabControl(DataGridView dataGridViewLeft, DataGridView dataGridViewRight)
        {
            this.dataGridViewLeft = dataGridViewLeft;
            this.dataGridViewRight = dataGridViewRight;

            InitializeComponent();

            //This is a subordinated control. Init is called in the class that add this.
        }

        public void Init()
        {
            this.AutoSize = true; //AutoSize set here otherwise while doing the UI, everything will be shrinked to minimun (So useless)
            this.Dock = DockStyle.Fill;

            this.gridSplitContainer.Panel1.Controls.Add(this.dataGridViewLeft);
            this.gridSplitContainer.Panel2.Controls.Add(this.dataGridViewRight);

            #region PartitionType ComboBox
            this.partitionTypeComboBox.DisplayMember = "Text";
            this.partitionTypeComboBox.ValueMember = "Value";

            var partitionTypeItems = new List<object>();
            foreach (AlarmPartitionType partitionType in Enum.GetValues(typeof(AlarmPartitionType)))
            {
                partitionTypeItems.Add(new { Text = partitionType.GetTranslation(), Value = partitionType });
            }
            this.partitionTypeComboBox.DataSource = partitionTypeItems;
            #endregion

            #region GroupingType ComboBox
            this.groupingTypeComboBox.DisplayMember = "Text";
            this.groupingTypeComboBox.ValueMember = "Value";

            var gropingTypeItems = new List<object>();
            foreach (AlarmGroupingType groupingType in Enum.GetValues(typeof(AlarmGroupingType)))
            {
                gropingTypeItems.Add(new { Text = groupingType.GetTranslation(), Value = groupingType });
            }
            this.groupingTypeComboBox.DataSource = gropingTypeItems;
            #endregion
        }

        public void Translate()
        {
            this.generationConfigButton.Text = Localization.Get("ALARM_CONFIG_GENERATION");
            this.defaultValuesConfigButton.Text = Localization.Get("ALARM_CONFIG_DEFAULTS");
            this.valuesPrefixesConfigButton.Text = Localization.Get("ALARM_CONFIG_PREFIX");
        }
    }
}
