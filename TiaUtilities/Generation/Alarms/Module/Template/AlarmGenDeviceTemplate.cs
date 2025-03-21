using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Generation.GridHandler;
using TiaXmlReader.Generation.Alarms;

namespace TiaUtilities.Generation.Alarms.Module.Template
{
    public class AlarmGenDeviceTemplate(string name) : INotifyPropertyChanged
    {
        public string Name { get; set; } = name;

        public GridSave<AlarmData>? AlarmGridSave { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));

    }

}