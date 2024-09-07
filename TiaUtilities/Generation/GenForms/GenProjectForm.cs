using InfoBox;
using Microsoft.WindowsAPICodePack.Dialogs;
using TiaUtilities.Configuration;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Languages;
using TiaXmlReader.Utility;

namespace TiaUtilities.Generation.GenForms
{
    public partial class GenProjectForm : Form
    {
        private readonly IGenProject project;
        private readonly TimedSaveHandler autoSaveHandler;
        private readonly GridSettings gridSettings;

        private bool projectLoading = false;
        private string? lastFilePath;

        public GenProjectForm(IGenProject generationProject, TimedSaveHandler autoSaveHandler, GridSettings gridSettings)
        {
            this.project = generationProject;
            this.autoSaveHandler = autoSaveHandler;
            this.gridSettings = gridSettings;

            InitializeComponent();

            Init();
        }

        public void Init()
        {
            #region FORM_CLOSING_INFO_BOX_PROJECT_DIRTY
            this.FormClosing += (sender, args) =>
            {
                InformationBoxResult result = InformationBoxResult.None;
                if (string.IsNullOrEmpty(lastFilePath))
                {
                    result = InformationBox.Show("Do you want to save this project?", title: "Project not saved", buttons: InformationBoxButtons.YesNoCancel);
                }
                else if (this.project.IsDirty())
                {
                    result = InformationBox.Show("Do you want to save this project?", title: "Project different from last save", buttons: InformationBoxButtons.YesNoCancel);
                }

                if (result == InformationBoxResult.Yes)
                {
                    this.project.Wash();
                    this.ProjectSave();
                }
                else if (result == InformationBoxResult.Cancel)
                {
                    args.Cancel = true;
                }
            };
            #endregion

            #region TOP_MENU
            this.saveMenuItem.Click += (sender, args) => { this.ProjectSave(); };
            this.saveAsMenuItem.Click += (sender, args) => { this.ProjectSave(true); };
            this.loadMenuItem.Click += (sender, args) => { this.ProjectLoad(); };
            this.exportXMLMenuItem.Click += (sender, args) =>
            {
                try
                {
                    var folderDialog = new CommonOpenFileDialog
                    {
                        IsFolderPicker = true,
                        EnsurePathExists = true,
                        EnsureValidNames = true
                    };

                    if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        var folderName = folderDialog.FileName;
                        if (string.IsNullOrEmpty(folderName) || !Directory.Exists(folderName))
                        {
                            return;
                        }

                        this.project.ExportXML(folderName);
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex);
                }
            };
            this.preferencesMenuItem.Click += (sender, args) => this.gridSettings.ShowConfigForm(this);
            #endregion

            #region AUTO_SAVE
            void eventHandler(object? sender, EventArgs args)
            {
                if (File.Exists(this.lastFilePath))
                {
                    this.ProjectSave();
                }
            }
            this.Shown += (sender, args) => this.autoSaveHandler.AddTickEventHandler(eventHandler);
            this.FormClosed += (sender, args) => this.autoSaveHandler.RemoveTickEventHandler(eventHandler);
            #endregion

            project.Init(this);

            this.projectTableLayout.Controls.Add(this.project.GetTopControl());
            this.projectTableLayout.Controls.Add(this.project.GetBottomControl());

            Translate();
        }

        private void Translate()
        {
            this.Text = this.project.GetFormLocalizatedName();

            this.fileMenuItem.Text = Localization.Get("GENERICS_FILE");
            this.saveMenuItem.Text = Localization.Get("GENERICS_SAVE");
            this.saveAsMenuItem.Text = Localization.Get("GENERICS_SAVE_AS");
            this.loadMenuItem.Text = Localization.Get("GENERICS_LOAD");

            this.importExportMenuItem.Text = Localization.Get("IO_GEN_FORM_IMPEXP");
            this.exportXMLMenuItem.Text = Localization.Get("IO_GEN_FORM_IMPEXP_EXPORT_XML");

            this.preferencesMenuItem.Text = Localization.Get("GRID_PREFERENCES");
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                switch (keyData)
                {
                    case Keys.S | Keys.Control:
                        this.ProjectSave(force: true);
                        return true; //Return required otherwise will write the letter.
                    case Keys.L | Keys.Control:
                        this.ProjectLoad();
                        return true; //Return required otherwise will write the letter.
                }

                if (this.project.ProcessCmdKey(ref msg, keyData))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ProjectSave(bool force = false, bool saveAs = false)
        {
            if(projectLoading)
            { //This is to avoid that autosave will save and empty file while selected file with dialog
                return;
            }

            var isDirty = this.project.IsDirty();
            if(!isDirty && !force && !saveAs)
            {
                return;
            }

            var projectSave = this.project.CreateSave();

            var saveOK = SavesLoader.Save(projectSave, ref lastFilePath, "json", saveAs || !File.Exists(lastFilePath));
            if (!saveOK)
            {
                return;
            }

            this.project.Wash();

            this.Text = this.project.GetFormLocalizatedName() + ". Project File: " + lastFilePath;
        }

        private void ProjectLoad()
        {
            projectLoading = true;

            var saveObject = SavesLoader.LoadWithDialog(ref lastFilePath, "json");
            if (saveObject != null)
            {
                this.project.LoadSave(saveObject);
                this.project.Wash();

                this.Text = this.project.GetFormLocalizatedName() + ". Project File: " + lastFilePath;
            }

            projectLoading = false;
        }
    }
}
