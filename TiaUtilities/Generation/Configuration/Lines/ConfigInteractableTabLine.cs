using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaUtilities.CustomControls;
using TiaXmlReader.Generation.Configuration;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigInteractableTabLine : ConfigLine<ConfigInteractableTabLine>
    {
        private readonly InteractableTabControl tabControl;

        private Action<TabPage>? tabAddedAction;
        private Action<TabPage>? tabRemovedAction;
        private Action<TabPage?>? tabSelectedChangedAction;
        private Action<TabPage, string>? tabChangedNameAction;

        public ReadOnlyCollection<TabPage> TabPages 
        { 
            get
            {
                List<TabPage> list = [];
                foreach (TabPage page in this.tabControl.TabPages)
                {
                    list.Add(page);
                }
                return list.AsReadOnly();
            }
        }

        public ConfigInteractableTabLine()
        {
            this.tabControl = new();

            this.tabControl.TabPreAdded += (sender, args) =>
            {
                var tabPage = args.TabPage;
                this.tabAddedAction?.Invoke(tabPage);
            };

            this.tabControl.TabPreRemoved += (sender, args) =>
            {
                var tabPage = args.TabPage;
                this.tabRemovedAction?.Invoke(tabPage);
            };

            this.tabControl.SelectedIndexChanged += (sender, args) =>
            {
                var tabPage = this.tabControl.SelectedTab;
                this.tabSelectedChangedAction?.Invoke(tabPage);
            };

            this.tabControl.TabNameUserChanged += (sender, args) =>
            {
                var tabPage = args.TabPage;
                tabChangedNameAction?.Invoke(tabPage, args.NewName);
            };

            this.tabControl.VisibleChanged += (sender, args) =>
            {
                var tabPage = this.tabControl.SelectedTab;
                if(tabPage != null)
                {
                    this.tabSelectedChangedAction?.Invoke(tabPage);
                }
            };
        }

        public ConfigInteractableTabLine RequireConfirmationBeforeClosing()
        {
            this.tabControl.RequireConfirmationBeforeClosing = true;
            return this;
        }

        public ConfigInteractableTabLine TabAdded(Action<TabPage> action)
        {
            this.tabAddedAction = action;
            return this;
        }

        public ConfigInteractableTabLine TabRemoved(Action<TabPage> action)
        {
            this.tabRemovedAction = action;
            return this;
        }

        public ConfigInteractableTabLine TabChanged(Action<TabPage?> action)
        {
            this.tabSelectedChangedAction = action;
            return this;
        }

        public ConfigInteractableTabLine TabNameUserChanged(Action<TabPage, string> action)
        {
            this.tabChangedNameAction = action;
            return this;
        }

        public TabPage AddTabPage()
        {
            TabPage tabPage = new();
            this.tabControl.TabPages.Add(tabPage);
            return tabPage;
        }

        public TabPage? GetVisibileTabPage()
        {
            return this.tabControl.SelectedTab;
        }

        public override Control? GetControl()
        {
            return tabControl;
        }
    }
}
