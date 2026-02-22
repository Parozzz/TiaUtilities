using TiaUtilities.Generation.Configuration;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Generation.SettingsNew;
using TiaUtilities.Languages;
using TiaUtilities.Utility;

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
            this.settingsButton.Text = "Settings";
            Utils.CreateStandardToolTip().SetToolTip(this.settingsButton, "CTRL + I");

            this.templateButton.Text = Locale.ALARM_SETTINGS_EDIT_TEMPLATE;
        }
    }
}
