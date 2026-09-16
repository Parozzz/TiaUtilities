using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Configuration;

namespace TiaUtilities.SettingsStep.ControlFactory.Impl
{
    public class SettingsTextFactory(string name, string description, SettingsFactoryGeneralOptions options) 
        : SettingsControlFactory(null, name, description, options)
    {
        public override (Label, Control?, Predicate<PropertyChangedEventArgs>?) Create(ObservableConfiguration configuration, SettingsFactoryCreateOptions createOptions)
        {
            var label = SettingsControls.GetText(this.Name, this.GeneralOptions, createOptions);
            return (label, null, null);
        }
    }
}
