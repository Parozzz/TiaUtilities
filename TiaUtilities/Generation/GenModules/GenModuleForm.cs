using InfoBox;
using Microsoft.WindowsAPICodePack.Dialogs;
using TiaUtilities.Configuration;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Languages;
using TiaXmlReader.Utility;
using TiaUtilities.Generation.GenModules;

namespace TiaUtilities.Generation.GenModules
{
    public partial class GenModuleForm : Form
    {
        private readonly IGenModule module;
        private readonly TimedSaveHandler autoSaveHandler;
        private readonly GridSettings gridSettings;

        private bool projectLoading = false;
        private string? lastFilePath;

        public GenModuleForm(IGenModule generationProject, TimedSaveHandler autoSaveHandler, GridSettings gridSettings)
        {
            this.module = generationProject;
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
                else if (this.module.IsDirty())
                {
                    result = InformationBox.Show("Do you want to save this project?", title: "Project different from last save", buttons: InformationBoxButtons.YesNoCancel);
                }

                if (result == InformationBoxResult.Yes)
                {
                    this.module.Wash();
                    this.ModuleSave();
                }
                else if (result == InformationBoxResult.Cancel)
                {
                    args.Cancel = true;
                }
            };
            #endregion

            #region TOP_MENU
            this.saveMenuItem.Click += (sender, args) => { this.ModuleSave(); };
            this.saveAsMenuItem.Click += (sender, args) => { this.ModuleSave(true); };
            this.loadMenuItem.Click += (sender, args) => { this.ModuleLoad(); };
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

                        this.module.ExportXML(folderName);
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
                    this.ModuleSave();
                }
            }
            this.Shown += (sender, args) => this.autoSaveHandler.AddTickEventHandler(eventHandler);
            this.FormClosed += (sender, args) => this.autoSaveHandler.RemoveTickEventHandler(eventHandler);
            #endregion

            module.Init(this);

            this.projectTableLayout.Controls.Add(this.module.GetTopControl());
            this.projectTableLayout.Controls.Add(this.module.GetBottomControl());

            Translate();
        }

        private void Translate()
        {
            this.Text = this.module.GetFormLocalizatedName();

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
                        this.ModuleSave(force: true);
                        return true; //Return required otherwise will write the letter.
                    case Keys.L | Keys.Control:
                        this.ModuleLoad();
                        return true; //Return required otherwise will write the letter.
                }

                if (this.module.ProcessCmdKey(ref msg, keyData))
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

        private void ModuleSave(bool force = false, bool saveAs = false)
        {
            if(projectLoading)
            { //This is to avoid that autosave will save and empty file while selected file with dialog
                return;
            }

            var isDirty = this.module.IsDirty();
            if(!isDirty && !force && !saveAs)
            {
                return;
            }

            var projectSave = this.module.CreateSave();

            var saveOK = SavesLoader.Save(projectSave, ref lastFilePath, Constants.SAVE_FILE_EXTENSION, saveAs || !File.Exists(lastFilePath));
            if (!saveOK)
            {
                return;
            }

            this.module.Wash();

            this.Text = this.module.GetFormLocalizatedName() + ". Project File: " + lastFilePath;
        }

        private void ModuleLoad()
        {
            projectLoading = true;

            var saveObject = SavesLoader.LoadWithDialog(ref lastFilePath, Constants.SAVE_FILE_EXTENSION);
            if (saveObject != null)
            {
                this.module.LoadSave(saveObject);
                this.module.Wash();

                this.Text = this.module.GetFormLocalizatedName() + ". Project File: " + lastFilePath;
            }

            projectLoading = false;
        }
    }
}
