using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Generation.Alarms.Data;
using TiaUtilities.Generation.GridHandler;

namespace TiaUtilities.Generation.Alarms.Template
{
    public class AlarmGenTemplate(string name)
    {
        public string Name { get; set; } = name;
        public AlarmTemplateConfiguration TemplateConfig { get; init; } = new();
        public GridSave<TemplateData> AlarmGridSave { get; set; } = new(); //This should be TemplateGridSave. But for compatibility, won't be renamed yet


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