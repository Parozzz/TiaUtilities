using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaUtilities.CustomControls;
using TiaUtilities.Generation.Alarms.Module.Tab;
using TiaUtilities.Generation.Alarms.Module.Template;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.Placeholders;
using TiaXmlReader;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.Placeholders;

namespace TiaUtilities.Generation.Alarms.Module
{
    public partial class AlarmGenDeviceTemplateForm : Form
    {
        private readonly List<AlarmGenDeviceTemplate> templateList;
        private readonly BindingList<AlarmGenDeviceTemplate> templateBindingList;

        private readonly AlarmMainConfiguration mainConfig;
        private readonly AlarmTabConfiguration tabConfig;
        private readonly AlarmDataGridWrapper alarmDataGridWrapper;

        private AlarmGenDeviceTemplate selectedTemplate = null;

        public AlarmGenDeviceTemplateForm(AlarmMainConfiguration mainConfig, AlarmTabConfiguration tabConfig, GridBindContainer bindContainer)
        {
            this.templateList = [];
            this.templateBindingList = new();

            this.mainConfig = mainConfig;
            this.tabConfig = tabConfig;

            AlarmGenPlaceholdersHandler placeholdersHandler = new(mainConfig, tabConfig);
            this.alarmDataGridWrapper = new(placeholdersHandler, bindContainer);

            InitializeComponent();
        }

        public void Init(ICollection<AlarmGenDeviceTemplate> templateCollection)
        {
            this.alarmDataGridWrapper.Init(this.mainConfig, this.tabConfig);
            this.alarmDataGridWrapper.GetDataGridView().ReadOnly = true;

            this.mainPanel.Controls.Add(this.alarmDataGridWrapper.GetDataGridView());

            this.templateList.Clear();
            //this.templateBindingList.AddRange(templateCollection);
            foreach (var template in templateCollection)
            {
                this.templateList.Add(template);
            }


            this.addButton.Click += (sender, args) =>
            {
                var newTemplate = new AlarmGenDeviceTemplate("New Template");
                try
                {
                    this.templateList.Add(newTemplate);
                }
                catch (Exception)
                {
                }

                this.templateBindingList.ResetBindings();
            };

            this.removeButton.Click += (sender, args) =>
            {
                if (this.selectedTemplate != null)
                {
                    this.templateList.Remove(this.selectedTemplate);
                }
            };

            this.renameButton.Click += (sender, args) =>
            {
                if (selectedTemplate == null)
                {
                    return;
                }

                var floatingTextBox = new FloatingTextBox()
                {
                    InputText = selectedTemplate.Name
                };
                floatingTextBox.ShowDialog(this);
                selectedTemplate.Name = floatingTextBox.InputText;
            };


            this.selectComboBox.DataSource = this.templateBindingList;
            this.selectComboBox.DisplayMember = "Name";
            this.selectComboBox.ValueMember = "Name";

            this.selectComboBox.SelectedIndexChanged += (sender, args) =>
            {
                var selectedItem = this.selectComboBox.SelectedItem;
                if (selectedItem is AlarmGenDeviceTemplate template)
                {
                    selectedTemplate = template;
                }
            };
        }
    }
}
