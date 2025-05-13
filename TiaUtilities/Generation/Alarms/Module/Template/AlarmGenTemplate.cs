using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.Alarms;

namespace TiaUtilities.Generation.Alarms.Module.Template
{
    public class AlarmGenTemplate(string name)
    {
        public string Name { get; set; } = name;
        public AlarmTemplateConfiguration TemplateConfig { get; init; } = new();
        public GridSave<AlarmData> AlarmGridSave { get; set; } = new();


        public AlarmGenTemplate Clone()
        {
            AlarmGenTemplate newClone = new(this.Name);
            foreach (var item in this.AlarmGridSave.RowData)
            {
                newClone.AlarmGridSave.RowData.Add(item.Key, item.Value.Clone());
            }
            return newClone;
        }

    }

}