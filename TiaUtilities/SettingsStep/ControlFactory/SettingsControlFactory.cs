using System.ComponentModel;
using TiaUtilities.Configuration;

namespace TiaUtilities.SettingsStep.ControlFactory
{
    public abstract class SettingsControlFactory(SettingsConfigurationProperty? configurationProperty, string name, string description, SettingsFactoryOptions? options = null)
    {

        public SettingsConfigurationProperty? ConfigurationProperty { get; init; } = configurationProperty;
        public string Name { get; init; } = name;
        public string Description { get; init; } = description;
        public SettingsFactoryOptions? Options { get; init; } = options;

        public abstract (Label?, Control?, Predicate<PropertyChangedEventArgs>?) Create(ObservableConfiguration configuration);
    }
}
