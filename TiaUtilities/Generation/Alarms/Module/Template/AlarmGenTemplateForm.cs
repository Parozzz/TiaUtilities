using System.ComponentModel;
using TiaUtilities.CustomControls;
using TiaUtilities.Generation.Alarms.Module.Tab;
using TiaUtilities.Generation.Alarms.Module.Template;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Languages;
using TiaXmlReader.Generation.Alarms;

namespace TiaUtilities.Generation.Alarms.Module
{
    public partial class AlarmGenTemplateForm : Form
    {

        private readonly AlarmMainConfiguration mainConfig;
        private readonly AlarmTabConfiguration tabConfig;
        private readonly AlarmDataGridWrapper alarmDataGridWrapper;

        private readonly AlarmGenTemplateHandler templateHandler;

        public AlarmGenTemplateForm(AlarmMainConfiguration mainConfig, AlarmTabConfiguration tabConfig,
            GridBindContainer bindContainer, AlarmGenTemplateHandler templateHandler)
        {
            this.mainConfig = mainConfig;
            this.tabConfig = tabConfig;

            AlarmGenPlaceholdersHandler placeholdersHandler = new(mainConfig, tabConfig);
            this.alarmDataGridWrapper = new(placeholdersHandler, bindContainer);

            this.templateHandler = templateHandler;

            InitializeComponent();
        }

        public void Init()
        {
            this.alarmDataGridWrapper.Init(this.mainConfig, this.tabConfig);

            this.mainPanel.Controls.Add(this.alarmDataGridWrapper.GetDataGridView());

            this.templateHandler.SelectedTemplateChanged += (sender, args) => this.HandleTemplateChanged(args.OldTemplate);

            this.addButton.Click += (sender, args) => this.templateHandler.AddNewTemplate();
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
            this.FormClosing += (sender, args) => HandleTemplateChanged(null);

            this.Translate();
        }

        private void HandleTemplateChanged(AlarmGenTemplate? oldTemplate)
        {
            if (oldTemplate != null)
            {
                oldTemplate.AlarmGridSave = this.alarmDataGridWrapper.CreateSave();
            }

            if (this.templateHandler.SelectedTemplate == null)
            {
                return;
            }

            this.alarmDataGridWrapper.LoadSave(this.templateHandler.SelectedTemplate.AlarmGridSave);
            this.alarmDataGridWrapper.Wash();

            this.selectComboBox.SelectedItem = this.templateHandler.SelectedTemplate;
        }

        private void Translate()
        {
            this.Text = Locale.ALARM_TEMPLATE_FORM;
            this.selectLabel.Text = Locale.ALARM_TEMPLATE_SELECT_TEMPLATE;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return this.alarmDataGridWrapper.ProcessCmdKey(ref msg, keyData) || base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
