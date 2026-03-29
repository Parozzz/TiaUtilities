using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Languages;
using TiaUtilities.Generation.Alarms.Configurations;

namespace TiaUtilities.Generation.Alarms.Module.Template
{
    public partial class AlarmGenTemplateForm : Form
    {
        private readonly AlarmMainConfiguration mainConfig;
        private readonly AlarmTabConfiguration tabConfig;
        private readonly TemplateAlarmGridWrapper templateDataGridWrapper;

        private readonly AlarmGenTemplateHandler templateHandler;
        private AlarmGenTemplate? SelectedTemplate { get => this.templateHandler.SelectedTemplate; set => this.templateHandler.SelectedTemplate = value; }

        public AlarmGenTemplateForm(AlarmMainConfiguration mainConfig, AlarmTabConfiguration tabConfig,
            GridBindContainer bindContainer, AlarmGenTemplateHandler templateHandler)
        {
            this.mainConfig = mainConfig;
            this.tabConfig = tabConfig;

            AlarmGenPlaceholdersHandler placeholdersHandler = new(mainConfig, tabConfig);
            this.templateDataGridWrapper = new(placeholdersHandler, bindContainer);

            this.templateHandler = templateHandler;

            InitializeComponent();
        }

        public void Init()
        {
            this.templateDataGridWrapper.Init(this.mainConfig, this.tabConfig, () => this.SelectedTemplate?.TemplateConfig ?? new());

            this.mainPanel.Controls.Add(this.templateDataGridWrapper.GetDataGridView());

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
            this.FormClosing += (sender, args) => HandleTemplateChanged(this.SelectedTemplate); //Save the selected template b4 closing.

            this.Translate();
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return this.templateDataGridWrapper.ProcessCmdKey(ref msg, keyData) || base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
