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

            this.addButton.Click += (sender, args) => this.templateHandler.AddNewTemplate();
            this.removeButton.Click += (sender, args) => this.templateHandler.RemoveSelectedTemplate();
            this.renameButton.Click += (sender, args) => this.templateHandler.RenameSelectedTemplate(this);

            this.selectComboBox.DataSource = new BindingSource() { DataSource = this.templateHandler.BindingList };
            this.selectComboBox.DisplayMember = "Name";
            this.selectComboBox.ValueMember = "Name";
            this.selectComboBox.SelectedIndexChanged += (sender, args) =>
            {
                var selectedItem = this.selectComboBox.SelectedItem;
                if (selectedItem is AlarmGenTemplate template)
                {
                    SelectTemplate(template);
                }
            };

            this.SelectTemplate(this.templateHandler.SelectedTemplate);

            System.Windows.Forms.Timer timer = new() { Interval = 300 };
            timer.Tick += (sender, args) =>
            {
                var selectedTemplate = this.templateHandler.SelectedTemplate;
                if (selectedTemplate == null || Selecting)
                {
                    return;
                }

                if (this.alarmDataGridWrapper.IsDirty())
                {
                    selectedTemplate.AlarmGridSave = this.alarmDataGridWrapper.CreateSave();
                    this.alarmDataGridWrapper.Wash();
                }
            };
            timer.Start();
            this.FormClosed += (sender, args) => timer.Stop();

            this.Translate();
        }

        private void Translate()
        {
            this.Text = Locale.ALARM_TEMPLATE_FORM;
            this.selectLabel.Text = Locale.ALARM_TEMPLATE_SELECT_TEMPLATE;
        }

        private bool Selecting = false;
        private void SelectTemplate(AlarmGenTemplate? template)
        {
            if (template == null)
            {
                return;
            }

            Selecting = true;

            this.templateHandler.SetSelectedTemplate(template);

            this.alarmDataGridWrapper.LoadSave(template.AlarmGridSave);
            this.alarmDataGridWrapper.Wash();

            Selecting = false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return this.alarmDataGridWrapper.ProcessCmdKey(ref msg, keyData) || base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
