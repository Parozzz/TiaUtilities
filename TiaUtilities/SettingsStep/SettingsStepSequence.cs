using System.Collections;
using System.Collections.ObjectModel;
using TiaUtilities.Configuration;
using TiaUtilities.SettingsNew;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.SettingsStep
{
    public class SettingsStepSequence
    {
        public class PanelControlsContainer : IEnumerable<SettingsStepPanelControls>
        {
            public int Count { get => this.panelControlsList.Count; }

            private readonly List<SettingsStepPanelControls> panelControlsList;
            public PanelControlsContainer(List<SettingsStepPanelControls> panelControlsList)
            {
                this.panelControlsList = panelControlsList;
            }

            public int IndexOf(SettingsStepPanelControls panelControls) => panelControlsList.IndexOf(panelControls);

            public IEnumerator<SettingsStepPanelControls> GetEnumerator() => panelControlsList.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public SettingsStepPanelControls? this[int index]
            {
                get => this.panelControlsList.TryGet(index, out var panelControls) ? panelControls : null;
                set
                {
                    if (this.panelControlsList.InRange(index))
                    {
                        if (value == null)
                        {
                            this.panelControlsList.RemoveAt(index);
                        }
                        else
                        {
                            this.panelControlsList[index] = value;
                        }
                    }
                }
            }
        }

        public ObservableConfiguration Configuration { get; init; }

        public string FullName { get => $"{this.GroupName} - {Name}"; }
        public string GroupName { get; init; }
        public string Name { get; init; }

        public PanelControlsContainer PanelControls { get; init; }

        public Func<string, string>? PlaceholdersCallBack { get; set; } = null;

        private readonly ObservableCollection<SettingsStepDescriptor> descriptors = [];
        private readonly List<SettingsStepPanelControls> panelControlsList = [];

        public SettingsStepSequence(ObservableConfiguration configuration, string groupName, string name)
        {
            this.Configuration = configuration;
            this.GroupName = groupName;
            this.Name = name;

            this.descriptors = [];
            this.panelControlsList = [];

            this.PanelControls = new(this.panelControlsList);
        }

        public void Add(SettingsStepDescriptor context) => descriptors.Add(context);
        public void AddRange(IEnumerable<SettingsStepDescriptor> context) => descriptors.AddRange(context);

        public void Init(SettingsStepForm form)
        {
            if (this.panelControlsList.Count == 0)
            {
                this.panelControlsList.AddRange(
                    descriptors.Select(d => new SettingsStepPanelControls(form, this, d, this.Configuration))
                );
                this.panelControlsList.ForEach(p => p.RegisterListeners());
            }
        }
    }
}
