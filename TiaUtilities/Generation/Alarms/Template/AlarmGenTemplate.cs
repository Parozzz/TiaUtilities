using System.ComponentModel;
using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Generation.Alarms.Data;
using TiaUtilities.Generation.GridHandler;

namespace TiaUtilities.Generation.Alarms.Template
{
    public class AlarmGenTemplate(AlarmGenTemplateContainer container, string name) : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        public string Name
        {
            get => this._name;
            set
            {
                var oldName = this._name;
                this._name = value;

                container.TemplateRenamed(this, oldName);
                PropertyChanged?.Invoke(this, new(nameof(Name)));
            }
        }
        public AlarmTemplateConfiguration TemplateConfig { get; init; } = new();
        public GridSave<TemplateData> AlarmGridSave { get; set; } = new(); //This should be TemplateGridSave. But for compatibility, won't be renamed yet

        private string _name = name;

        public AlarmGenTemplate Clone(AlarmGenTemplateContainer container, string? name = null)
        {
            AlarmGenTemplate newClone = new(container, name ?? this.Name);
            foreach (var item in this.AlarmGridSave.RowData)
            {
                newClone.AlarmGridSave.RowData.Add(item.Key, item.Value.Clone());
            }
            return newClone;
        }

        public override string ToString() => this.Name;

    }

}