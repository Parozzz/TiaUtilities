using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Configuration;

namespace TiaUtilities.SettingsStep.ControlFactory.Impl
{
    public class SettingsTextFactory : SettingsControlFactory
    {
        public SettingsTextFactory(string name, string description, SettingsFactoryOptions? options = null) 
            : base(null, name, description, options)
        {
        }

        public override (Label, Control?, Predicate<PropertyChangedEventArgs>?) Create(ObservableConfiguration configuration)
        {
            var label = SettingsControls.GetText(this.Name, this.Options);
            return (label, null, null);
        }
    }
}
