using DocumentFormat.OpenXml;
using System.ComponentModel;
using TiaUtilities.Configuration;
using TiaUtilities.Utility;

namespace TiaUtilities.SettingsStep.ControlFactory.Impl
{
    public class SettingsSelectionFactory(SettingsConfigurationProperty? configurationProperty, string name, string description, SettingsFactoryGeneralOptions options) 
        : SettingsControlFactory(configurationProperty, name, description, options)
    {
        public override (Label, Control, Predicate<PropertyChangedEventArgs>) Create(ObservableConfiguration configuration, SettingsFactoryCreateOptions createOptions)
        {
            Validate.NotNull(this.ConfigurationProperty);

            var nameLabel = SettingsControls.GetNameLabel(this.Name, this.Description, this.GeneralOptions, createOptions, () => $"{this.ConfigurationProperty?.GetFrom(configuration)}");
            
            var comboBox = SettingsControls.GetComboBox(this.GeneralOptions.Selections, this.GeneralOptions, createOptions);

            var startValue = this.ConfigurationProperty.GetFrom(configuration);
            comboBox.Text = $"{startValue}";

            var setInProgress = false;
            comboBox.SelectionChangeCommitted += (sender, args) =>
            {
                if(comboBox.SelectedItem is ControlUtils.ComboBoxDataSourceItem item)
                {
                    setInProgress = true;
                    this.ConfigurationProperty.SetTo(configuration, item.Value);
                    setInProgress = false;
                }
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
