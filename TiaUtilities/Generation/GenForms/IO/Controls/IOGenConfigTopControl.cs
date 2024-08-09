using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Languages;
using TiaUtilities.Generation.GenForms.Alarm;
using TiaUtilities.Generation.GenForms.Alarm.Controls;
using TiaXmlReader.Generation.IO;

namespace TiaUtilities.Generation.GenForms.IO
{
    public partial class IOGenConfigTopControl : UserControl
    {
        public delegate void MemoryTypeChangedEventHandler(object? sender, MemoryTypeChangedEventArgs eventArgs);
        public record MemoryTypeChangedEventArgs(IOMemoryTypeEnum MemoryType);

        public event MemoryTypeChangedEventHandler MemoryTypeChanged = delegate { };

        public IOGenConfigTopControl()
        {//This is a subordinated control. Init is called in the class that add this.
            InitializeComponent();

            this.Dock = DockStyle.Fill;

            #region MemoryType ComboBox
            this.memoryTypeComboBox.DisplayMember = "Text";
            this.memoryTypeComboBox.ValueMember = "Value";

            var memoryTypeItems = new List<object>();
            foreach (IOMemoryTypeEnum memoryType in Enum.GetValues(typeof(IOMemoryTypeEnum)))
            {
                memoryTypeItems.Add(new { Text = memoryType.GetTranslation(), Value = memoryType });
            }
            this.memoryTypeComboBox.DataSource = memoryTypeItems;
            #endregion

            #region GroupingType ComboBox
            this.groupingTypeComboBox.DisplayMember = "Text";
            this.groupingTypeComboBox.ValueMember = "Value";

            var groupingTypeItems = new List<object>();
            foreach (IOGroupingTypeEnum groupingType in Enum.GetValues(typeof(IOGroupingTypeEnum)))
            {
                groupingTypeItems.Add(new { Text = groupingType.GetTranslation(), Value = groupingType });
            }
            this.groupingTypeComboBox.DataSource = groupingTypeItems;
            #endregion

            this.memoryTypeComboBox.SelectionChangeCommitted += (sender, args) =>
            {
                if (this.memoryTypeComboBox.SelectedValue is IOMemoryTypeEnum memoryType)
                {
                    MemoryTypeChanged(this, new(memoryType));
                    this.UpdateConfigPanel(memoryType);
                }
            };

            //this.UpdateConfigPanel(this.memoryTypeComboBox.SelectedValue);

            Translate();
        }

        private void Translate()
        {
            this.memoryTypeLabel.Text = Localization.Get("IO_CONFIG_MEMORY_TYPE");
            this.groupingTypeLabel.Text = Localization.Get("IO_CONFIG_GROUPING_TYPE");

            this.dbConfigButton.Text = Localization.Get("IO_GEN_CONFIG_ALIAS_DB");
            this.variableTableConfigButton.Text = Localization.Get("IO_GEN_CONFIG_ALIAS_TABLE");
            this.ioTableConfigButton.Text = Localization.Get("IO_GEN_CONFIG_IO_TABLE");
        }

        private void UpdateConfigPanel(IOMemoryTypeEnum memoryType)
        {
            this.configButtonPanel.SuspendLayout();

            this.configButtonPanel.Controls.Remove(dbConfigButton);
            this.configButtonPanel.Controls.Remove(variableTableConfigButton);
            if (memoryType == IOMemoryTypeEnum.DB)
            {
                this.configButtonPanel.Controls.Add(dbConfigButton);
            }
            else if (memoryType == IOMemoryTypeEnum.MERKER)
            {
                this.configButtonPanel.Controls.Add(variableTableConfigButton);
            }

            this.configButtonPanel.ResumeLayout();
        }
    }
}
