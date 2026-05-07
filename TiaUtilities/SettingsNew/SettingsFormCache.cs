using TiaUtilities.Generation.SettingsNew;
using TiaUtilities.SettingsNew.Bindings;

namespace TiaUtilities.SettingsNew
{
    public class SettingsFormCache(SettingsBindings settingsBindings, IWin32Window window)
    {
        private readonly SettingsBindings settingsBindings = settingsBindings;
        private readonly IWin32Window window = window;
        private SettingsForm? settingsForm;

        public void Show()
        {
            if (this.settingsForm != null)
            {
                if (this.window is Control control)
                {
                    this.settingsForm.Owner = control.FindForm();
                }
                this.settingsForm.ToggleVisibility(forceOpen: true);
            }
            else
            {
                this.settingsForm = new(settingsBindings);
                this.settingsForm.FormClosing += (sender, args) =>
                {
                    if(args.CloseReason == CloseReason.UserClosing)
                    {
                        this.settingsForm.ToggleVisibility(forceClosed: true);
                        args.Cancel = true;
                    }
                };
                this.settingsForm.Show(this.window);
            }
        }

        public void ToggleVisibility()
        {
            if(settingsForm == null)
            {
                this.Show();
                return;
            }

            this.settingsForm.ToggleVisibility();
        }

    }
}
