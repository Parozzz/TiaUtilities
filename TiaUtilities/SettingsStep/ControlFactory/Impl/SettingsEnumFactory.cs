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
    public class SettingsEnumFactory : SettingsControlFactory
    {

        public required Type EnumType { get; init; }

        public SettingsEnumFactory(SettingsConfigurationProperty? configurationProperty, string name, string description, SettingsFactoryOptions? options = null) 
            : base(configurationProperty, name, description, options)
        {
        }

        public override (Label, Control, Predicate<PropertyChangedEventArgs>) Create(ObservableConfiguration configuration)
        {
            Validate.NotNull(this.ConfigurationProperty);

            var nameLabel = SettingsControls.GetNameLabel(this.Name, this.Description, this.Options, () => $"{this.ConfigurationProperty?.GetFrom(configuration)}");

            var translatedEnumTexts = Enum.GetValues(this.EnumType).Cast<Enum>().Select(e => e.GetTranslation());

            var comboBox = SettingsControls.GetComboBox(translatedEnumTexts, this.Options);
            ControlUtils.CreateComboBoxEnumDataSource(comboBox, this.EnumType);

            var startValue = this.ConfigurationProperty.GetFrom(configuration);
            comboBox.SelectedValue = startValue;

            var setInProgress = false;
            comboBox.SelectedValueChanged += (sender, args) =>
            {
                var value = comboBox.SelectedValue;
                if(value.GetType() == this.EnumType)
                {
                    setInProgress = true;
                    this.ConfigurationProperty.SetTo(configuration, value);
                    setInProgress = false;
                }
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
