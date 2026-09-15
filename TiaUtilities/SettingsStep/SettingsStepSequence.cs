using System.Collections.Immutable;
using TiaUtilities.Configuration;
using TiaUtilities.SettingsNew;

namespace TiaUtilities.SettingsStep
{
    public class SettingsStepSequence(ObservableConfiguration configuration, string groupName, string name)
    {
        public string GroupName { get; init; } = groupName;
        public string Name { get; init; } = name;
        public ObservableConfiguration Configuration { get; init; } = configuration;

        private readonly List<SettingsStepDescriptor> descriptors = [];
        private readonly List<SettingsStepPanelControls> panelControlsList = [];

        public void Add(SettingsStepDescriptor context) => descriptors.Add(context);
        public void AddRange(IEnumerable<SettingsStepDescriptor> context) => descriptors.AddRange(context);

        public ImmutableList<SettingsStepPanelControls> GetPanelControlsList(SettingsStepForm form)
        {
            if (this.descriptors.Count == this.panelControlsList.Count)
            {
                return [.. this.panelControlsList];
            }

            this.panelControlsList.Clear();
            this.panelControlsList.AddRange(
                descriptors.Select(c => new SettingsStepPanelControls(form, this, c, this.Configuration))
            );

            return [.. this.panelControlsList];
        }
    }
}
