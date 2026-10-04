using System.Collections;
using System.Collections.ObjectModel;
using TiaUtilities.Configuration;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.Settings
{
    public class SettingsSequence
    {
        public class PanelContainer(List<SettingsSequencePanel> panelControlsList) : IEnumerable<SettingsSequencePanel>
        {
            public int Count { get => panelControlsList.Count; }

            public int IndexOf(SettingsSequencePanel panelControls) => panelControlsList.IndexOf(panelControls);

            public IEnumerator<SettingsSequencePanel> GetEnumerator() => panelControlsList.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public SettingsSequencePanel? this[int index]
            {
                get => panelControlsList.TryGet(index, out var panelControls) ? panelControls : null;
                set
                {
                    if (panelControlsList.InRange(index))
                    {
                        if (value == null)
                        {
                            panelControlsList.RemoveAt(index);
                        }
                        else
                        {
                            panelControlsList[index] = value;
                        }
                    }
                }
            }
        }

        public ObservableConfiguration Configuration { get; init; }

        public string FullName { get => $"{this.GroupName} - {Name}"; }
        public string GroupName { get; init; }
        public string Name { get; init; }

        public PanelContainer Panels { get; init; }

        public Func<string, string>? PlaceholdersCallback { get; set; } = null;
        public Action<SettingsSequencePanel>? PanelCreationCallback { get; set; } = null;
        public Action<SettingsSequencePanel>? PanelDisposeCallback { get; set; } = null;

        private readonly ObservableCollection<SettingsSequencePanelDescriptor> descriptors = [];
        private readonly List<SettingsSequencePanel> panelControlsList = [];

        public SettingsSequence(ObservableConfiguration configuration, string groupName, string name)
        {
            this.Configuration = configuration;
            this.GroupName = groupName;
            this.Name = name;

            this.descriptors = [];
            this.panelControlsList = [];

            this.Panels = new(this.panelControlsList);
        }

        public void Add(SettingsSequencePanelDescriptor context) => descriptors.Add(context);
        public void AddRange(IEnumerable<SettingsSequencePanelDescriptor> context) => descriptors.AddRange(context);

        public void Init(SettingsControl settingsControl)
        {
            if (this.panelControlsList.Count == 0)
            {
                this.panelControlsList.AddRange(
                    descriptors.Select(d => 
                    {
                        var panel = new SettingsSequencePanel(settingsControl, this, d, this.Configuration);
                        this.PanelCreationCallback?.Invoke(panel);
                        return panel;
                    })
                );
                this.panelControlsList.ForEach(p => p.RegisterListeners());
            }
        }

        public void DisposeAll()
        {
            this.panelControlsList.ForEach(p =>
            {
                this.PanelDisposeCallback?.Invoke(p);

                p.UnregisterListeners();
                p.Lines.ForEach(l => l.DisposeAll());

                this.PanelCreationCallback = null;
                this.PanelDisposeCallback = null;
            });
        }
    }
}
