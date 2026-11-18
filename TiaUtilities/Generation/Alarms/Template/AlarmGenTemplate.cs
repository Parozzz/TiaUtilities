using System.ComponentModel;
using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Generation.Alarms.Data;
using TiaUtilities.Generation.GridHandler;

namespace TiaUtilities.Generation.Alarms.Template
{
    public class AlarmGenTemplate(AlarmGenTemplateContainer container, string name) : INotifyPropertyChanged
    {
        public static AlarmGenTemplate CreateFromSave(AlarmGenTemplateContainer container, AlarmGenTemplateSave save)
        {
            return new AlarmGenTemplate(container, save.Name)
            {
                GridSave = save.AlarmGrid,
                Config = save.TemplateConfig
            };
        }

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

        public AlarmTemplateConfiguration Config { get; init; } = new();
        public GridSave<TemplateRowData> GridSave { get; set; } = new(); 

        private string _name = name;

        public AlarmGenTemplateSave CreateSave()
        {
            return new()
            {
                Name = this.Name,
                AlarmGrid = this.GridSave,
                TemplateConfig = this.Config,
            };
        }

        public AlarmGenTemplate Clone(AlarmGenTemplateContainer container, string? name = null)
        {
            AlarmGenTemplate newClone = new(container, name ?? this.Name);
            foreach (var item in this.GridSave.RowData)
            {
                newClone.GridSave.RowData.Add(item.Key, item.Value.Clone());
            }
            return newClone;
        }

        public override string ToString() => this.Name;

    }

}