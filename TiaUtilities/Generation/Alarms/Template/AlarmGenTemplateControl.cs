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
using TiaUtilities.Resources;

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

            this.templateHandler.SelectedChanged += (sender, args) => this.HandleTemplateChanged(oldTemplate: args.OldTemplate);

            ImageList buttonsImages = new()
            {
                Images = { ImageResources.ADD_501366_007435, ImageResources.DELETE, ImageResources.RENAME, ImageResources.DUPLICATE },
                ImageSize = new(18, 18),
            };

            this.addButton.ImageList = buttonsImages;
            this.addButton.ImageIndex = 0;
            this.addButton.ImageAlign = ContentAlignment.MiddleLeft;
            this.addButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.addButton.Click += (sender, args) => this.templateHandler.Add();

            this.removeButton.ImageList = buttonsImages;
            this.removeButton.ImageIndex = 1;
            this.removeButton.ImageAlign = ContentAlignment.MiddleLeft;
            this.removeButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.removeButton.Click += (sender, args) => this.templateHandler.RemoveSelectedTemplate();

            this.renameButton.ImageList = buttonsImages;
            this.renameButton.ImageIndex = 2;
            this.renameButton.ImageAlign = ContentAlignment.MiddleLeft;
            this.renameButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.renameButton.Click += (sender, args) => this.templateHandler.RenameSelectedTemplate(this);

            this.cloneButton.ImageList = buttonsImages;
            this.cloneButton.ImageIndex = 3;
            this.cloneButton.ImageAlign = ContentAlignment.MiddleLeft;
            this.cloneButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.cloneButton.Click += (sender, args) => this.templateHandler.CloneSelectedTemplate();

            this.selectComboBox.AutoWidthFromItems = true;
            this.selectComboBox.SetFilterableSource(this.templateHandler.BindingList, "Name", "Name");
            this.selectComboBox.SelectionChangeCommitted += (sender, args) =>
            {
                var selectedItem = this.selectComboBox.SelectedItem;
                if (selectedItem is AlarmGenTemplate template)
                {
                    this.templateHandler.SelectedTemplate = template;
                }
            };

            //Load Current after init and save current when form is closed.
            this.HandleTemplateChanged();
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


        private void HandleTemplateChanged(AlarmGenTemplate? oldTemplate = null)
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

            this.addButton.Text = "Add";
            this.removeButton.Text = "Remove";
            this.renameButton.Text = "Rename";
            this.cloneButton.Text = "Clone";
        }
    }
}
