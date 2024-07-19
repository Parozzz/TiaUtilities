using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Languages;

namespace TiaUtilities.Generation.GenForms.Alarm
{
    public partial class AlarmConfigControl : UserControl
    {
        public AlarmConfigControl()
        {
            InitializeComponent();

            //This is a subordinated control. Init is called in the class that add this.
        }

        public void Init()
        {
            this.Dock = DockStyle.Fill;

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

            Translate();
        }

        private void Translate()
        {
            this.fcConfigButton.Text = Localization.Get("ALARM_CONFIG_FC");
            this.alarmGenerationConfigButton.Text = Localization.Get("ALARM_CONFIG_GENERATION");
            this.fieldDefaultValueConfigButton.Text = Localization.Get("ALARM_CONFIG_DEFAULTS");
            this.fieldPrefixConfigButton.Text = Localization.Get("ALARM_CONFIG_PREFIX");
            this.segmentNameConfigButton.Text = Localization.Get("ALARM_CONFIG_SEGMENT_NAME");
            this.textListConfigButton.Text = Localization.Get("ALARM_CONFIG_TEXT_LIST");
        }
    }
}
