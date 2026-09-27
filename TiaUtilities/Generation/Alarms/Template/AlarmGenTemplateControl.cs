using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Languages;

namespace TiaUtilities.Generation.Alarms.Template
{
    public partial class AlarmGenTemplateControl : UserControl
    {
        private readonly AlarmMainConfiguration mainConfig;
        private readonly AlarmTabConfiguration tabConfig;
        private readonly TemplateAlarmGridWrapper templateDataGridWrapper;

        private readonly AlarmGenTemplateHandler templateHandler;
        private AlarmGenTemplate? SelectedTemplate { get => this.templateHandler.SelectedTemplate; set => this.templateHandler.SelectedTemplate = value; }

        public AlarmGenTemplateControl(AlarmMainConfiguration mainConfig, AlarmTabConfiguration tabConfig,
            MultiGridOperationHandler multiGrid, AlarmGenTemplateHandler templateHandler)
        {
            InitializeComponent();

            this.mainConfig = mainConfig;
            this.tabConfig = tabConfig;

            AlarmGenPlaceholdersHandler placeholdersHandler = new(mainConfig, tabConfig);
            this.templateDataGridWrapper = new(placeholdersHandler, multiGrid);

            this.templateHandler = templateHandler;

        }

        public void Init()
        {
            this.templateDataGridWrapper.Init(this.mainConfig, this.tabConfig, () => this.SelectedTemplate?.TemplateConfig ?? new());

            this.mainPanel.Controls.Add(this.templateDataGridWrapper.GetGridControl());

            this.templateHandler.SelectedTemplateChanged += (sender, args) => this.HandleTemplateChanged(args.OldTemplate);

            this.addButton.Click += (sender, args) => this.templateHandler.Add();
            this.removeButton.Click += (sender, args) => this.templateHandler.RemoveSelectedTemplate();
            this.renameButton.Click += (sender, args) => this.templateHandler.RenameSelectedTemplate(this);
            this.cloneButton.Click += (sender, args) => this.templateHandler.CloneSelectedTemplate();

            this.selectComboBox.DataSource = new BindingSource() { DataSource = this.templateHandler.BindingList };
            this.selectComboBox.DisplayMember = "Name";
            this.selectComboBox.ValueMember = "Name";
            this.selectComboBox.SelectedIndexChanged += (sender, args) =>
            {
                var selectedItem = this.selectComboBox.SelectedItem;
                if (selectedItem is AlarmGenTemplate template)
                {
                    this.templateHandler.SelectedTemplate = template;
                }
            };

            //Load Current after init and save current when form is closed.
            this.HandleTemplateChanged(null);
            this.Translate();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if(!this.Visible)
            {
                HandleTemplateChanged(this.SelectedTemplate);
            }
        }

        private void HandleTemplateChanged(AlarmGenTemplate? oldTemplate)
        {
            if (oldTemplate != null)
            {
                oldTemplate.AlarmGridSave = this.templateDataGridWrapper.CreateSave();
            }

            if (this.SelectedTemplate == null || oldTemplate == this.SelectedTemplate)
            {
                return;
            }

            this.templateDataGridWrapper.LoadSave(this.SelectedTemplate.AlarmGridSave);
            this.templateDataGridWrapper.Wash();

            this.selectComboBox.SelectedItem = this.SelectedTemplate;
        }

        private void Translate()
        {
            this.Text = Locale.ALARM_TEMPLATE_FORM;
            this.selectLabel.Text = Locale.ALARM_TEMPLATE_SELECT_TEMPLATE;
        }
    }
}
