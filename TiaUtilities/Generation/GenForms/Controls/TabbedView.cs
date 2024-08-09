using TiaUtilities.CustomControls;
using TiaUtilities.Generation.GenForms.Alarm.Tab;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Javascript;

namespace TiaUtilities.Generation.GenForms.Alarm.Controls
{
    public partial class TabbedView : UserControl
    {
        public delegate void TabRemovedEventHandler(object sender, TabRemovedEventArgs args);
        public class TabRemovedEventArgs(TabPage tabPage) : EventArgs
        {
            public TabPage TabPage { get; set; } = tabPage;
            public bool Handled { get; set; } = false;
        }

        public delegate void TabAddedEventHandler(object sender, TabAddedEventArgs args);
        public class TabAddedEventArgs(TabPage tabPage) : EventArgs
        {
            public TabPage TabPage { get; set; } = tabPage;
            public bool Cancel { get; set; } = false;
        }

        public event TabRemovedEventHandler TabRemoved = delegate { };
        public event TabAddedEventHandler TabAdded = delegate { };

        public TabbedView()
        {
            InitializeComponent();

            this.AutoSize = true; //AutoSize set here otherwise while doing the UI, everything will be shrinked to minimun (So useless)
            this.Dock = DockStyle.Fill;

            this.gridsTabControl.TabPreRemoved += (sender, args) =>
            {
                var tabPage = this.gridsTabControl.TabPages[args.TabIndex];

                var tabRemovedArgs = new TabRemovedEventArgs(tabPage);
                TabRemoved(this, tabRemovedArgs);
                if (tabRemovedArgs.Handled)
                {
                    args.Cancel = true;
                    return;
                }
            };

            this.gridsTabControl.TabPreAdded += (sender, args) =>
            {
                var tabAddedArgs = new TabAddedEventArgs(args.TabPage);
                TabAdded(this, tabAddedArgs);
                if (tabAddedArgs.Cancel)
                {
                    args.Cancel = true;
                    return;
                }
            };
        }

        public TabPage AddTab()
        {
            TabPage tabPage = new();
            this.gridsTabControl.TabPages.Add(tabPage);
            return tabPage;
        }

        public TabPage? GetVisibleTab()
        {
            if(this.gridsTabControl.TabCount == 0)
            {
                return null;
            }

            var tabPage = this.gridsTabControl.TabPages[this.gridsTabControl.TabIndex];
            if(tabPage is InteractableNewTabPage)
            {
                return null;
            }

            return tabPage;
        }

        public void ClearAllTabs()
        {
            this.gridsTabControl.TabPages.Clear();
        }
    }
}
