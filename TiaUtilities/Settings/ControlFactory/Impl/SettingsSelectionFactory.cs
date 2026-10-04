using DocumentFormat.OpenXml;
using System.Collections;
using System.ComponentModel;
using TiaUtilities.Configuration;
using TiaUtilities.Settings;
using TiaUtilities.Settings.ControlFactory;
using TiaUtilities.Utility;

namespace TiaUtilities.Settings.ControlFactory.Impl
{
    public class SettingsSelectionFactory(SettingsConfigurationProperty? configurationProperty, string name, string description, SettingsFactoryGeneralOptions options) 
        : SettingsControlFactory(configurationProperty, name, description, options)
    {
        public override (Label, Control, Predicate<PropertyChangedEventArgs>) Create(ObservableConfiguration configuration, SettingsFactoryCreateOptions createOptions)
        {
            Validate.NotNull(this.ConfigurationProperty);

            var nameLabel = SettingsControls.GetNameLabel(this.Name, this.Description, this.GeneralOptions, createOptions, () => $"{this.ConfigurationProperty?.GetFrom(configuration)}");
            
            var comboBox = SettingsControls.GetComboBox(this.GeneralOptions.Selections, this.GeneralOptions, createOptions);

            void startValueSetter(object? sender, EventArgs args)
            {
                comboBox.BindingContextChanged -= startValueSetter; //Unsubscribe first to avoid never clearing it if an exception is thrown.

                var startValue = this.ConfigurationProperty.GetFrom(configuration);
                if (comboBox.DataSource is IEnumerable enumerableSource)
                {                    
                    var selectedItem = enumerableSource
                        .OfType<ControlUtils.ComboBoxDataSourceItem>()
                        .FirstOrDefault(i => i.Text.Equals($"{startValue}", StringComparison.OrdinalIgnoreCase)); //Compare with string because is easier. For numbers would be a lot of work to check for each type.

                    comboBox.SelectedItem = selectedItem;
                }
            }
            comboBox.BindingContextChanged += startValueSetter;

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
                if (!this.ConfigurationProperty.IsThisPropertyChanged(args))
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
