using System.ComponentModel;
using TiaUtilities.Configuration;
using TiaUtilities.Utility;

namespace TiaUtilities.SettingsStep.ControlFactory.Impl
{
    public class SettingsBoolFactory : SettingsControlFactory
    {

        public SettingsBoolFactory(SettingsConfigurationProperty configurationProperty, string name, string description, SettingsFactoryGeneralOptions options = null) 
            : base(configurationProperty, name, description, options)
        {
            this.ConfigurationProperty = configurationProperty;
            this.Name = name;
            this.Description = description;
            this.GeneralOptions = options;
        }

        public override (Label, Control, Predicate<PropertyChangedEventArgs>) Create(ObservableConfiguration configuration, SettingsFactoryCreateOptions createOptions)
        {
            Validate.NotNull(this.ConfigurationProperty);

            var nameLabel = SettingsControls.GetNameLabel(this.Name, this.Description, this.GeneralOptions, createOptions);

            var checkBox = SettingsControls.GetCheckBox(this.GeneralOptions, createOptions);

            var startValue = this.ConfigurationProperty.GetFrom(configuration);
            this.UpdateChecked(checkBox, startValue);

            var setInProgress = false;
            checkBox.CheckedChanged += (sender, args) =>
            {
                setInProgress = true;
                this.ConfigurationProperty.SetTo(configuration, checkBox.Checked);
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
                this.UpdateChecked(checkBox, value);

                return true;
            }

            return (nameLabel, checkBox, propertyChangedPredicate);
        }


        private void UpdateChecked(CheckBox checkBox, object? value)
        {
            if (value is bool boolValue)
            {
                checkBox.Checked = boolValue;
            }
            else if (bool.TryParse($"{value}", out bool parsedBoolValue))
            {
                checkBox.Checked = parsedBoolValue;
            }
        }
    }
}
