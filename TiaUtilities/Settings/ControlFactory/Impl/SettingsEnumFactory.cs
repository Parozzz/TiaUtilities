using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Configuration;
using TiaUtilities.Languages;
using TiaUtilities.Utility;

namespace TiaUtilities.SettingsStep.ControlFactory.Impl
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
            //ControlUtils.CreateComboBoxEnumDataSource(comboBox, this.EnumType);

            var startValue = this.ConfigurationProperty.GetFrom(configuration);
            comboBox.SelectedValue = startValue;

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
