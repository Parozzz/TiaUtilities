using DocumentFormat.OpenXml.Drawing.Charts;
using System.Collections;
using System.ComponentModel;
using TiaUtilities.CustomControls;
using TiaUtilities.Languages;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Collections;

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

    public delegate void AlarmTemplateAddedEvent(object? sender, AlarmTemplateAddedEventArgs args);
    public class AlarmTemplateAddedEventArgs(AlarmGenTemplate added) : EventArgs
    {
        public AlarmGenTemplate Added { get; init; } = added;
    }

    public class AlarmGenTemplateContainer : IEnumerable<AlarmGenTemplate>, IReadOnlyList<AlarmGenTemplate>, ICleanable
    {
        public AlarmGenTemplate? SelectedTemplate
        {
            get => selectedTemplate;
            set
            {
                var oldTemplate = selectedTemplate;
                selectedTemplate = value;

                if (Utils.AreDifferentObject(oldTemplate, selectedTemplate))
                {
                    this.SelectedChanged(this, new() { OldTemplate = oldTemplate });
                }
            }
        }

        public int Count => this.templateList.Count;

        public ReadOnlyBindingListWrapper<AlarmGenTemplate> ReadOnlyBindingList => new(this.templateList);

        public event AlarmTemplateSelectedChanged SelectedChanged = delegate { };
        public event AlarmTemplateRenamedEvent Renamed = delegate { };
        public event AlarmTemplateAddedEvent Added = delegate { };

        private readonly BindingListSmart<AlarmGenTemplate> templateList;
        private AlarmGenTemplate? selectedTemplate;

        private bool dirty = false;

        public AlarmGenTemplateContainer()
        {
            this.templateList = [];
        }

        public void Init(ICollection<AlarmGenTemplate> templateCollection)
        {
            this.templateList.Clear();
            this.templateList.AddRange(templateCollection);

            if (templateCollection.Count == 0)
            {
                this.Add();
            }

            this.SelectedTemplate = this.templateList[0];
        }

        public AlarmGenTemplate this[int index] => this.templateList[index];

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
            var templateName = string.IsNullOrEmpty(name) ? $"TEMPLATE [{templateList.Count}]" : name;

            AlarmGenTemplate newTemplate = new(templateName);

            AlarmTemplateAddedEventArgs args = new(newTemplate);
            this.Added(this, args);
            this.templateList.Add(newTemplate);

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
                    if (this.templateList.Count > 0)
                    {//Select the template above the one just deleted.
                        var index = this.templateList.IndexOf(template);
                        if(index >= 0)
                        {
                            this.SelectedTemplate = this.templateList[index - 1];
                        }
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

                Renamed(this, new(oldName, newName));

                this.SelectedTemplate.Name = newName;

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
                AlarmGenTemplate template = new(save.Name) 
                { 
                    AlarmGridSave = save.AlarmGrid, 
                    TemplateConfig = save.TemplateConfig 
                };
                templateList.Add(template);
            }

            this.Init(templateList);
        }

        public bool IsDirty() => this.dirty;

        public void Wash() => this.dirty = false;

        public IEnumerator<AlarmGenTemplate> GetEnumerator() => this.templateList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

}
