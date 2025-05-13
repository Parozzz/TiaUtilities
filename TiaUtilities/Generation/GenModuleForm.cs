using InfoBox;
using Microsoft.WindowsAPICodePack.Dialogs;
using TiaUtilities.Languages;
using TiaXmlReader;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Utility;

namespace TiaUtilities.Generation.GenModules
{
    public partial class GenModuleForm : Form
    {
        private readonly IGenModule module;
        private readonly TimedSaveHandler autoSaveHandler;

        private bool projectLoading = false;
        private string? lastFilePath;

        public GenModuleForm(IGenModule generationProject, TimedSaveHandler autoSaveHandler)
        {
            this.module = generationProject;
            this.autoSaveHandler = autoSaveHandler;

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
                    this.ModuleSave(force: true);
                    this.module.Wash();
                }
                else if (result == InformationBoxResult.Cancel)
                {
                    args.Cancel = true;
                }
            };
            #endregion

            #region TOP_MENU
            this.saveMenuItem.Click += (sender, args) => { this.ModuleSave(); };
            this.saveAsMenuItem.Click += (sender, args) => { this.ModuleSave(saveAs: true); };
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
            this.preferencesMenuItem.Click += (sender, args) => MainForm.Settings.GridSettings.ShowConfigForm(this);
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

            this.formTableLayout.Controls.Add(this.module.GetControl());

            Translate();
        }

        private void Translate()
        {
            this.SetLocalizedFormText("");

            this.fileMenuItem.Text = Locale.GENERICS_FILE;
            this.saveMenuItem.Text = Locale.GENERICS_SAVE;
            this.saveAsMenuItem.Text = Locale.GENERICS_SAVE_AS;
            this.loadMenuItem.Text = Locale.GENERICS_LOAD;

            this.importExportMenuItem.Text = Locale.IO_GEN_FORM_IMPEXP;
            this.exportXMLMenuItem.Text = Locale.IO_GEN_FORM_IMPEXP_EXPORT_XML;

            this.preferencesMenuItem.Text = Locale.GRID_PREFERENCES;
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
            if (projectLoading)
            { //This is to avoid that autosave will save an empty file while selecting file with dialog
                return;
            }

            var isDirty = this.module.IsDirty();
            if (!isDirty && !force && !saveAs)
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

            this.SetLocalizedFormText(lastFilePath ?? "");
        }

        public void ModuleLoad(object? saveObject = null)
        {
            projectLoading = true;

            saveObject ??= SavesLoader.LoadWithDialog(ref lastFilePath, Constants.SAVE_FILE_EXTENSION);
            if (saveObject != null)
            {
                this.module.LoadSave(saveObject);
                this.module.Wash();

                this.SetLocalizedFormText(lastFilePath ?? "");
            }

            projectLoading = false;
        }

        private void SetLocalizedFormText(string filePath)
        {
            this.Text = this.module.GetFormLocalizatedName().Replace("{file_path}", filePath);
        }

        public void SetLastFilePath(string? filePath)
        {
            this.lastFilePath = filePath;
        }
    }
}
