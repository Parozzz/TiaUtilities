using System.ComponentModel;
using TiaUtilities.CustomControls;
using TiaUtilities.Languages;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.Alarms.Module.Template
{
    public delegate void AlarmGenTemplateSelectedChanged(object? sender, AlarmGenTemplateSelectedChangedArgs args);
    public class AlarmGenTemplateSelectedChangedArgs : EventArgs
    {
        public AlarmGenTemplate? OldTemplate { get; set; }
    }

    public class AlarmGenTemplateHandler : ICleanable
    {
        private readonly List<AlarmGenTemplate> templateList;
        public BindingList<AlarmGenTemplate> BindingList { get; init; }

        private AlarmGenTemplate? _selectedTemplate;
        public AlarmGenTemplate? SelectedTemplate
        {
            get => _selectedTemplate;
            set
            {
                var oldTemplate = _selectedTemplate;
                _selectedTemplate = value;

                if (Utils.AreDifferentObject(oldTemplate, _selectedTemplate))
                {
                    this.SelectedTemplateChanged(this, new() { OldTemplate = oldTemplate });
                }
            }
        }

        public event AlarmGenTemplateSelectedChanged SelectedTemplateChanged = delegate { };

        private bool dirty = false;

        public AlarmGenTemplateHandler()
        {
            this.templateList = [];
            this.BindingList = new(templateList);
        }

        public void Init(ICollection<AlarmGenTemplate> templateCollection)
        {
            this.templateList.Clear();
            foreach (var template in templateCollection)
            {
                this.templateList.Add(template);
            }

            if (templateCollection.Count == 0)
            {
                this.AddNewTemplate();
            }
            this.SelectedTemplate = this.templateList[0];
        }

        public IEnumerable<string> GetAllNames()
        {
            return this.templateList.Select(template => template.Name);
        }

        public AlarmGenTemplate? FindTemplate(string? name)
        {
            if (name == null)
            {
                return null;
            }

            foreach (var template in this.templateList)
            {
                if (template.Name == name)
                {
                    return template;
                }
            }

            return null;
        }

        public void AddNewTemplate()
        {
            AlarmGenTemplate newTemplate = new($"New template [{templateList.Count}]");
            this.templateList.Add(newTemplate);
            this.BindingList.ResetBindings();

            this.SelectedTemplate = newTemplate;

            this.dirty = true;
        }

        public void RemoveSelectedTemplate()
        {
            if (this.SelectedTemplate == null)
            {
                return;
            }

            var result = MessageBox.Show(Locale.CONFIRM_DELETE_DIALOG_TEXT.Replace("{delete_item}", this.SelectedTemplate.Name),
                Locale.CONFIRM_DELETE_DIALOG_CAPTION,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var index = this.templateList.IndexOf(this.SelectedTemplate);

                this.templateList.Remove(this.SelectedTemplate);
                this.BindingList.ResetBindings();

                if (this.templateList.Count > 0)
                {//Select the template above the one just deleted.
                    this.SelectedTemplate = this.templateList[index - 1];
                }

                this.dirty = true;
            }
        }

        public void RenameSelectedTemplate(IWin32Window? window)
        {
            if (SelectedTemplate == null)
            {
                return;
            }

            var floatingTextBox = new FloatingTextBox(SelectedTemplate.Name) { Width = 400 };
            if (floatingTextBox.ShowDialogAtCursor(window) == DialogResult.OK)
            {
                SelectedTemplate.Name = floatingTextBox.InputText;
                this.BindingList.ResetBindings();

                this.dirty = true;
            }
        }

        public void CloneSelectedTemplate()
        {
            if (this.SelectedTemplate == null)
            {
                return;
            }

            AlarmGenTemplate newClone = this.SelectedTemplate.Clone();
            newClone.Name += $" [{this.templateList.Count}]";
            this.templateList.Add(newClone);
            this.BindingList.ResetBindings();

            this.SelectedTemplate = newClone;

            this.dirty = true;
        }

        public List<AlarmGenTemplateSave> CreateSave()
        {
            List<AlarmGenTemplateSave> list = [];

            foreach (var template in this.templateList)
            {
                if (template.AlarmGridSave == null)
                {
                    continue;
                }

                list.Add(new()
                {
                    Name = template.Name,
                    AlarmGrid = template.AlarmGridSave,
                    TemplateConfig = template.TemplateConfig,
                });
            }

            return list;
        }

        public void LoadSave(List<AlarmGenTemplateSave> saveList)
        {
            List<AlarmGenTemplate> templateList = [];
            foreach (var save in saveList)
            {
                AlarmGenTemplate template = new(save.Name) { AlarmGridSave = save.AlarmGrid, TemplateConfig = save.TemplateConfig };
                templateList.Add(template);
            }

            this.Init(templateList);
        }

        public bool IsDirty() => this.dirty;

        public void Wash() => this.dirty = false;
    }

}
