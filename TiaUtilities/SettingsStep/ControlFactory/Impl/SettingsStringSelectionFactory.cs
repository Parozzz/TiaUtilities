using System.ComponentModel;
using TiaUtilities.Configuration;
using TiaUtilities.CustomControls;
using TiaUtilities.SettingsNew;
using TiaUtilities.Utility;

namespace TiaUtilities.SettingsStep.ControlFactory.Impl
{
    public class SettingsStringSelectionFactory(SettingsConfigurationProperty? configurationProperty, string name, string description, SettingsFactoryGeneralOptions options) 
        : SettingsControlFactory(configurationProperty, name, description, options)
    {
        public required IEnumerable<string> Selections { get; init; }

        public override (Label, Control, Predicate<PropertyChangedEventArgs>) Create(ObservableConfiguration configuration, SettingsFactoryCreateOptions createOptions)
        {
            Validate.NotNull(this.ConfigurationProperty);

            var nameLabel = SettingsControls.GetNameLabel(this.Name, this.Description, this.GeneralOptions, createOptions, () => $"{this.ConfigurationProperty?.GetFrom(configuration)}");

            var comboBox = SettingsControls.GetComboBox(Selections, this.GeneralOptions, createOptions);
            ControlUtils.CreateComboBoxObjectDataSource(comboBox, Selections.ToList(), editable: false);

            var startValue = this.ConfigurationProperty.GetFrom(configuration);
            comboBox.SelectedValue = startValue;

            var setInProgress = false;
            comboBox.SelectedValueChanged += (sender, args) =>
            {
                setInProgress = true;

                var value = comboBox.SelectedValue;
                if (value is string stringValue)
                {
                    this.ConfigurationProperty.SetTo(configuration, stringValue);
                }

                setInProgress = false;
            };

            bool propertyChangedPredicate(PropertyChangedEventArgs args)
            {
                if (!this.ConfigurationProperty.IsPropertyChanged(args))
                {
                    return false;
                }

                this.GeneralOptions?.PropertyChangedCallback?.Invoke(); //This way also handles property changed from the control!
                if (setInProgress)
                {
                    return false;
                }

                var value = this.ConfigurationProperty.GetFrom(configuration);
                if (value is string stringValue)
                {
                    this.ConfigurationProperty.SetTo(configuration, stringValue);
                }

                return true;
            }

            return (nameLabel, comboBox, propertyChangedPredicate);
        }
    }
}
