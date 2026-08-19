using DocumentFormat.OpenXml.Drawing.Charts;
using System.ComponentModel;
using TiaUtilities.CustomControls;
using TiaUtilities.Languages;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.Alarms.Template
{
    public delegate void AlarmTemplateSelectedChanged(object? sender, AlarmTemplateSelectedChangedArgs args);
    public class AlarmTemplateSelectedChangedArgs : EventArgs
    {
        public AlarmGenTemplate? OldTemplate { get; set; }
    }

    public delegate void AlarmTemplateRenamedEvent(object? sender, AlarmTemplateRenamedEventArgs args);
    public class AlarmTemplateRenamedEventArgs(string oldName, string newName) : EventArgs
    {
        public string OldName { get; init; } = oldName;
        public string NewName { get; init; } = newName;
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

        public event AlarmTemplateSelectedChanged SelectedTemplateChanged = delegate { };
        public event AlarmTemplateRenamedEvent TemplateRenamed = delegate { };

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
                this.Add();
            }
            this.SelectedTemplate = this.templateList[0];
        }

        public IEnumerable<string> GetAllNames()
        {
            return this.templateList.Select(template => template.Name);
        }

        public AlarmGenTemplate? Find(string? name)
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

        public AlarmGenTemplate Add(string name = "")
        {
            var templateName = string.IsNullOrEmpty(name) ? $"New template [{templateList.Count}]" : name;

            AlarmGenTemplate newTemplate = new(templateName);
            this.templateList.Add(newTemplate);
            this.BindingList.ResetBindings();

            this.SelectedTemplate = newTemplate;

            this.dirty = true;

            return newTemplate;
        }

        public void Remove(string name)
        {
            var template = this.Find(name);
            this.Remove(template);
        }

        public void Remove(AlarmGenTemplate? template)
        {
            if (template != null)
            {
                if (template == this.SelectedTemplate)
                {
                    this.BindingList.ResetBindings();

                    var index = this.templateList.IndexOf(template);
                    if (this.templateList.Count > 0)
                    {//Select the template above the one just deleted.
                        this.SelectedTemplate = this.templateList[index - 1];
                    }
                }

                this.templateList.Remove(template);
                this.dirty = true;
            }
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
                this.Remove(this.SelectedTemplate);
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
                var oldName = this.SelectedTemplate.Name;
                var newName = floatingTextBox.InputText;

                TemplateRenamed(this, new(oldName, newName));

                this.SelectedTemplate.Name = newName;
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
