using TiaUtilities.Generation.SettingsNew;
using TiaUtilities.SettingsNew.Bindings;

namespace TiaUtilities.SettingsNew
{
    public class SettingsFormCache(SettingsBindings settingsBindings)
    {
        private readonly SettingsBindings settingsBindings = settingsBindings;
        private SettingsForm? settingsForm;

        public void Show(IWin32Window? window)
        {
            if (this.settingsForm != null)
            {
                if (window is Control control)
                {
                    this.settingsForm.Owner = control.FindForm();
                }
                this.settingsForm.ShowInTaskbar = true;
                this.settingsForm.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.settingsForm = new(settingsBindings);
                this.settingsForm.FormClosing += (sender, args) =>
                {
                    if(args.CloseReason == CloseReason.UserClosing)
                    {
                        this.settingsForm.WindowState = FormWindowState.Minimized;
                        this.settingsForm.ShowInTaskbar = false;

                        args.Cancel = true;
                    }
                };
                this.settingsForm.Show(window);
            }
        }

    }
}
