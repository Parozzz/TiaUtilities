using System.ComponentModel;
using TiaUtilities.Configuration;
using TiaUtilities.CustomControls;
using TiaUtilities.SettingsNew;
using TiaUtilities.Utility;

namespace TiaUtilities.SettingsStep.ControlFactory.Impl
{
    public class SettingsStringSelectionFactory : SettingsControlFactory
    {
        public required IEnumerable<string> Selections { get; init; }

        public SettingsStringSelectionFactory(SettingsConfigurationProperty? configurationProperty, string name, string description, SettingsFactoryOptions? options = null) :
            base(configurationProperty, name, description, options)
        {
        }

        public override (Label, Control, Predicate<PropertyChangedEventArgs>) Create(ObservableConfiguration configuration)
        {
            Validate.NotNull(this.ConfigurationProperty);

            var nameLabel = SettingsControls.GetNameLabel(this.Name, this.Description, this.Options, () => $"{this.ConfigurationProperty?.GetFrom(configuration)}");

            var comboBox = SettingsControls.GetComboBox(Selections, this.Options);
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

                this.Options?.PropertyChangedCallback?.Invoke(); //This way also handles property changed from the control!
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
