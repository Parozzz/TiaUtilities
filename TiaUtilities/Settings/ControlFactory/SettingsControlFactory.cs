using System.ComponentModel;
using TiaUtilities.Configuration;

namespace TiaUtilities.SettingsStep.ControlFactory
{
    public abstract class SettingsControlFactory(SettingsConfigurationProperty? configurationProperty, string name, string description, SettingsFactoryGeneralOptions generalOptions)
    {

        public SettingsConfigurationProperty? ConfigurationProperty { get; init; } = configurationProperty;
        public string Name { get; init; } = name;
        public string Description { get; init; } = description;
        public SettingsFactoryGeneralOptions GeneralOptions { get; init; } = generalOptions;

        public abstract (Label?, Control?, Predicate<PropertyChangedEventArgs>?) Create(ObservableConfiguration configuration, SettingsFactoryCreateOptions createOptions);
    }
}
