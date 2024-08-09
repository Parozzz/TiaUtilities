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
using TiaUtilities.Generation.GenForms.Alarm;

namespace TiaUtilities.Generation.GenForms.Alarm.Controls
{
    public partial class AlarmGenConfigTopControl : UserControl
    {
        public AlarmGenConfigTopControl()
        { //This is a subordinated control. Init is called in the class that add this.
            InitializeComponent();

            this.Dock = DockStyle.Fill;
            Translate();
        }

        private void Translate()
        {
            this.fcConfigButton.Text = Localization.Get("ALARM_CONFIG_FC");
            this.segmentNameConfigButton.Text = Localization.Get("ALARM_CONFIG_SEGMENT_NAME");
            this.textListConfigButton.Text = Localization.Get("ALARM_CONFIG_TEXT_LIST");
        }
    }
}
