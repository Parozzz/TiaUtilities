using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Languages;
using TiaUtilities.Generation.Configuration;
using TiaUtilities.Generation.SettingsNew;

namespace TiaUtilities.Generation.Alarms.Module
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
            this.formattingButton.Text = Locale.ALARM_CONFIG_FORMATTING;

            this.enableCustomVarLabel.Text = Locale.ALARM_CONFIG_ENABLE_CUSTOM_VAR;
            this.enableTimerLabel.Text = Locale.ALARM_CONFIG_ENABLE_TIMER;
        }
    }
}
