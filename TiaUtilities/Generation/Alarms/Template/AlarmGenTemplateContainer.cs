using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
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
    public class AlarmTemplateRenamedEventArgs(AlarmGenTemplate template, string oldName) : EventArgs
    {
        public AlarmGenTemplate Template { get; init; } = template;
        public string OldName { get; init; } = oldName;
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
            this.templateList.ListChanged += (sender, args) => this.dirty = true;

            this.Add(silent: true);
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

        public AlarmGenTemplate Add(string name = "") => this.Add(name, silent: false);
        private AlarmGenTemplate Add(string name = "", bool silent = false)
        {
            var templateName = string.IsNullOrEmpty(name) ? this.GetDefaultTemplateName() : name;

            AlarmGenTemplate newTemplate = new(this, templateName);

            if(!silent)
            {
                this.Added(this, new(newTemplate));
            }

            this.templateList.Add(newTemplate);
            this.SelectedTemplate = newTemplate;
            return newTemplate;
        }

        private string GetDefaultTemplateName() => $"TEMPLATE [{templateList.Count}]";

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
                        if (index >= 0)
                        {
                            this.SelectedTemplate = this.templateList[index - 1];
                        }
                    }
                }

                this.templateList.Remove(template);
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
                var newName = floatingTextBox.InputText;
                this.SelectedTemplate.Name = newName;
            }
        }

        public void CloneSelectedTemplate()
        {
            if (this.SelectedTemplate == null)
            {
                return;
            }

            var newName = $"{this.SelectedTemplate.Name} ({this.templateList.Count})";

            AlarmGenTemplate newClone = this.SelectedTemplate.Clone(this, newName);
            this.templateList.Add(newClone);

            this.SelectedTemplate = newClone;
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
            this.templateList.Clear();
            this.templateList.AddRange(
                saveList.Select(save =>
                    new AlarmGenTemplate(this, save.Name)
                    {
                        AlarmGridSave = save.AlarmGrid,
                        TemplateConfig = save.TemplateConfig
                    }
                )
            );

            this.Wash();
        }

        internal void TemplateRenamed(AlarmGenTemplate template, string oldName)
        {
            this.dirty = true;
            this.Renamed(this, new(template, oldName));
        }

        public bool IsDirty() => this.dirty;

        public void Wash() => this.dirty = false;

        public IEnumerator<AlarmGenTemplate> GetEnumerator() => this.templateList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

}
