using System.Collections.Immutable;
using TiaUtilities.Configuration;
using TiaUtilities.SettingsNew;

namespace TiaUtilities.SettingsStep
{
    public class SettingsStepContainer(ObservableConfiguration configuration, string groupName, string name)
    {
        public string GroupName { get; init; } = groupName;
        public string Name { get; init; } = name;
        public ObservableConfiguration Configuration { get; init; } = configuration;

        private readonly List<SettingsStepContext> contexts = [];
        private readonly List<SettingsStepMapper> mappings = [];

        public void Add(SettingsStepContext context) => contexts.Add(context);
        public void AddRange(IEnumerable<SettingsStepContext> context) => contexts.AddRange(context);

        public ImmutableList<SettingsStepMapper> GetMappings(SettingsStepForm form)
        {
            if (this.contexts.Count == this.mappings.Count)
            {
                return [.. this.mappings];
            }

            this.mappings.Clear();
            this.mappings.AddRange(
                contexts.Select(c => new SettingsStepMapper(form, this, c, this.Configuration))
            );

            return [.. this.mappings];
        }
    }
}
