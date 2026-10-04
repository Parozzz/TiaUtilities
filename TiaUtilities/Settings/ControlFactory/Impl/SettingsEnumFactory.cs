using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Configuration;
using TiaUtilities.Languages;
using TiaUtilities.Settings;
using TiaUtilities.Settings.ControlFactory;
using TiaUtilities.Utility;

namespace TiaUtilities.Settings.ControlFactory.Impl
{
    public class SettingsEnumFactory(SettingsConfigurationProperty? configurationProperty, string name, string description, SettingsFactoryGeneralOptions options) 
        : SettingsControlFactory(configurationProperty, name, description, options)
    {

        public required Type EnumType { get; init; }

        public override (Label, Control, Predicate<PropertyChangedEventArgs>) Create(ObservableConfiguration configuration, SettingsFactoryCreateOptions createOptions)
        {
            Validate.NotNull(this.ConfigurationProperty);

            var nameLabel = SettingsControls.GetNameLabel(this.Name, this.Description, this.GeneralOptions, createOptions, () => $"{this.ConfigurationProperty?.GetFrom(configuration)}");

            var comboBox = SettingsControls.GetEnumComboBox(this.EnumType, this.GeneralOptions, createOptions);

            void startValueSetter(object? sender, EventArgs args)
            {
                comboBox.BindingContextChanged -= startValueSetter; //Unsubscribe first to avoid never clearing it if an exception is thrown.

                var startValue = this.ConfigurationProperty.GetFrom(configuration);
                if (comboBox.DataSource is IEnumerable enumerableSource)
                {
                    var selectedItem = enumerableSource
                        .OfType<ControlUtils.ComboBoxDataSourceItem>()
                        .FirstOrDefault(i => Enum.Equals(i.Value, startValue));
                    
                    comboBox.SelectedItem = selectedItem;
                }
            }
            comboBox.BindingContextChanged += startValueSetter;

            var setInProgress = false;
            comboBox.SelectionChangeCommitted += (sender, args) =>
            {
                if(comboBox.SelectedItem is ControlUtils.ComboBoxDataSourceItem item &&  this.EnumType == item.Value.GetType())
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
                if(value != null && value.GetType() == this.EnumType)
                {
                    comboBox.SelectedValue = value;
                }
                return true;
            }

            return (nameLabel, comboBox, propertyChangedPredicate);
        }
    }
}
