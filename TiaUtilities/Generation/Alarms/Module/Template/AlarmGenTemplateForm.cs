using TiaUtilities.Generation.Configuration;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Languages;

namespace TiaUtilities.Generation.Alarms.Module.Template
{
    public partial class AlarmGenTemplateForm : Form
    {
        private readonly AlarmMainConfiguration mainConfig;
        private readonly AlarmTabConfiguration tabConfig;
        private readonly TemplateAlarmGridWrapper alarmDataGridWrapper;

        private readonly AlarmGenTemplateHandler templateHandler;
        private AlarmGenTemplate? SelectedTemplate { get => this.templateHandler.SelectedTemplate; set => this.templateHandler.SelectedTemplate = value; }

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
            this.alarmDataGridWrapper.Init(this.mainConfig, this.tabConfig, () => this.SelectedTemplate?.TemplateConfig ?? new());

            this.mainPanel.Controls.Add(this.alarmDataGridWrapper.GetDataGridView());

            this.templateHandler.SelectedTemplateChanged += (sender, args) => this.HandleTemplateChanged(args.OldTemplate);

            this.addButton.Click += (sender, args) => this.templateHandler.AddNewTemplate();
            this.removeButton.Click += (sender, args) => this.templateHandler.RemoveSelectedTemplate();
            this.renameButton.Click += (sender, args) => this.templateHandler.RenameSelectedTemplate(this);
            this.cloneButton.Click += (sender, args) => this.templateHandler.CloneSelectedTemplate();
            this.configurationButton.Click += (sender, args) =>
            {
                var templateConfig = this.SelectedTemplate?.TemplateConfig;
                if (templateConfig == null)
                {
                    return;
                }

                var configForm = new ConfigForm("Configuration") { ControlWidth = 300 };
                configForm.SetConfiguration(templateConfig, MainForm.Settings.PresetTemplateConfiguration);

                var mainGroup = configForm.Init().ControlWidth(150);
                mainGroup.AddCheckBox().Label("Allarmi Indipendenti").BindChecked(() => templateConfig.StandaloneAlarms);

                configForm.StartShowingAtControl(this.configurationButton);
                configForm.Show(this);

                configForm.FormClosed += (sender, args) => this.alarmDataGridWrapper.Refresh();
            };

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
                oldTemplate.AlarmGridSave = this.alarmDataGridWrapper.CreateSave();
            }

            if (this.SelectedTemplate == null || oldTemplate == this.SelectedTemplate)
            {
                return;
            }

            this.alarmDataGridWrapper.LoadSave(this.SelectedTemplate.AlarmGridSave);
            this.alarmDataGridWrapper.Wash();

            this.selectComboBox.SelectedItem = this.SelectedTemplate;
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
