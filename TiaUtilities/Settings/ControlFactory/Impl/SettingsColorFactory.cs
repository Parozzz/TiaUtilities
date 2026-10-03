using Cyotek.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Configuration;
using TiaUtilities.SettingsStep;
using TiaUtilities.SettingsStep.ControlFactory;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.Settings.ControlFactory.Impl
{
    public class SettingsColorFactory : SettingsControlFactory
    {
        public SettingsColorFactory(SettingsConfigurationProperty? configurationProperty, string name, string description, SettingsFactoryGeneralOptions generalOptions) 
            : base(configurationProperty, name, description, generalOptions)
        {
        }

        public override (Label?, Control?, Predicate<PropertyChangedEventArgs>?) Create(ObservableConfiguration configuration, SettingsFactoryCreateOptions createOptions)
        {
            Validate.NotNull(this.ConfigurationProperty);

            var nameLabel = SettingsControls.GetNameLabel(this.Name, this.Description, this.GeneralOptions, createOptions);

            var button = SettingsControls.GetButton(this.GeneralOptions, createOptions);
            button.FlatAppearance.BorderSize = 4;

            var startValue = this.ConfigurationProperty.GetFrom(configuration);
            this.ColorChanged(button, startValue);

            var setInProgress = false;
            button.Click += (sender, args) =>
            {
                var color = button.FlatAppearance.BorderColor;

                var colorDialog = new ColorPickerDialog() { Color = color, ShowAlphaChannel = false };
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    var colorPicked = colorDialog.Color;

                    button.FlatAppearance.BorderColor = colorPicked;
                    button.Text = colorPicked.ToHexString();

                    setInProgress = true;
                    this.ConfigurationProperty.SetTo(configuration, colorPicked);
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
                this.ColorChanged(button, value);

                return true;
            }

            return (nameLabel, button, propertyChangedPredicate);
        }

        private void ColorChanged(Button button, object? value)
        {
            if(value is Color color)
            {
                button.FlatAppearance.BorderColor = color;
                button.Text = color.ToHexString();
            }
        }
    }
}
