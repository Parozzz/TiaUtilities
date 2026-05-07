using InfoBox;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Reflection;
using TiaUtilities.Constants;
using TiaUtilities.Generation.SettingsNew;
using TiaUtilities.Languages;
using TiaUtilities.Resources;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation
{
    public partial class GenModuleForm : Form
    {
        private readonly IGenModule module;
        private readonly TimedSaveHandler autoSaveHandler;

        private bool projectLoading = false;

        protected string? openProjectFilePath;

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
                if (string.IsNullOrEmpty(this.openProjectFilePath))
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

            #region TOP_MENU_FILE
            this.saveMenuItem.Click += (sender, args) => { this.ModuleSave(); };
            this.saveAsMenuItem.Click += (sender, args) => { this.ModuleSave(saveAs: true); };
            this.loadMenuItem.Click += (sender, args) => { this.ModuleLoad(); };
            #endregion

            #region TOP_MENU_TOOLS
            this.toolsPlaceholderViewerMenuItem.Click += (sender, args) => this.module.OpenPlaceholderViewer(this);
            #endregion

            #region TOP_MENU_PROGRAM
            this.programSettingsMenuItem.Click += (sender, args) => new SettingsForm(MainForm.SettingsBindings).Show(this);
            this.programModuleSetupMenuItem.Click += (sender, args) => this.module.ToggleSettingsFormVisibility();
            #endregion

            #region TOP_MENU_IMPORT_EXPORT
            this.exportXMLMenuItem.Click += (sender, args) =>
            {
                try
                {
                    var filePath = MainForm.Settings.GetSavedFileDialogPath(FileDialogResources.GENERATION_EXPORT_XML);

                    var folderDialog = new CommonOpenFileDialog
                    {
                        IsFolderPicker = true,
                        EnsurePathExists = true,
                        EnsureValidNames = true,
                        InitialDirectory = filePath
                    };

                    if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        var folderName = folderDialog.FileName;
                        if (string.IsNullOrEmpty(folderName) || !Directory.Exists(folderName))
                        {
                            return;
                        }

                        this.module.ExportXML(folderName);

                        MainForm.Settings.SetSavedFileDialogPath(FileDialogResources.GENERATION_EXPORT_XML, folderName);
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex);
                }
            };
            #endregion

            #region AUTO_SAVE
            void eventHandler(object? sender, EventArgs args)
            {
                if (File.Exists(this.openProjectFilePath))
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
            this.saveMenuItem.Text = Locale.GENERICS_SAVE + " (CTRL+S)"; ;
            this.saveAsMenuItem.Text = Locale.GENERICS_SAVE_AS;
            this.loadMenuItem.Text = Locale.GENERICS_LOAD + " (CTRL+L)";

            this.programMenuItem.Text = Locale.GENERICS_PROGRAM;
            this.programSettingsMenuItem.Text = Locale.GENERICS_SETTINGS + " (CTRL+P)";
            this.programModuleSetupMenuItem.Text = Locale.GENERICS_SETUP + " (CTRL+W)";

            this.toolsMenuItem.Text = Locale.GEN_FORM_TOOLS;
            this.toolsPlaceholderViewerMenuItem.Text = Locale.GEN_FORM_TOOLS_PLACEHOLDER_VIEWER + " (CTRL+Q)";

            this.importExportMenuItem.Text = Locale.GEN_FORM_IMPORT_EXPORT;
            this.exportXMLMenuItem.Text = Locale.GEN_FORM_IMPORT_EXPORT_EXPORT_XML;


        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                switch (keyData)
                {
                    case Keys.P | Keys.Control:
                        this.programSettingsMenuItem.PerformClick();
                        return true;
                    case Keys.W | Keys.Control:
                        this.programModuleSetupMenuItem.PerformClick();
                        return true;
                    case Keys.S | Keys.Control:
                        this.ModuleSave(force: true);
                        return true; //Return required otherwise will write the letter.
                    case Keys.L | Keys.Control:
                        this.ModuleLoad();
                        return true; //Return required otherwise will write the letter.
                    case Keys.I | Keys.Control:
                        new SettingsForm(this.module.SettingsBindings).Show(this);
                        return true;
                    case Keys.Q | Keys.Control:
                        this.module.OpenPlaceholderViewer(this);
                        return true;
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
            if (projectSave == null)
            {
                return;
            }

            var version = GetProjectSaveVersion(projectSave);



            var filePath = this.openProjectFilePath;
            if(string.IsNullOrEmpty(filePath))
            {
                filePath = MainForm.Settings.GetSavedFileDialogPath(FileDialogResources.GENERATION_SAVE);
            }

            var requireFileDialog = string.IsNullOrEmpty(this.openProjectFilePath) || saveAs || !File.Exists(filePath);

            var saveOK = SavesLoader.Save(projectSave, version, ref filePath, ProgramConstants.SAVE_FILE_EXTENSION, requireFileDialog);
            if (saveOK)
            {
                this.openProjectFilePath = filePath;

                this.module.Wash();
                this.SetLocalizedFormText(filePath ?? "");
            }

            MainForm.Settings.SetSavedFileDialogPath(FileDialogResources.GENERATION_SAVE, filePath);
        }

        public void ModuleLoad(object? saveObject = null)
        {
            projectLoading = true;

            var filePath = this.openProjectFilePath;
            if(string.IsNullOrEmpty(filePath))
            {
                filePath = MainForm.Settings.GetSavedFileDialogPath(FileDialogResources.GENERATION_LOAD);
            }

            saveObject ??= SavesLoader.LoadWithDialog(ref filePath, ProgramConstants.SAVE_FILE_EXTENSION);
            if (saveObject != null)
            {
                this.openProjectFilePath = filePath;
                this.SetLocalizedFormText(filePath ?? "");

                this.module.LoadSave(saveObject);
                this.module.Wash();

            }

            MainForm.Settings.SetSavedFileDialogPath(FileDialogResources.GENERATION_LOAD, filePath);

            projectLoading = false;
        }

        private void SetLocalizedFormText(string filePath)
        {
            this.Text = this.module.GetFormLocalizatedName().Replace("{file_path}", filePath);
        }

        public void SetOpenProjectFilePath(string? filePath)
        {
            this.openProjectFilePath = filePath;
        }

        private int GetProjectSaveVersion(Object obj)
        {
            const int LEGACY_VERSION = 1;

            var type = obj.GetType();

            var versionPropertyType = type.GetProperty("VERSION", BindingFlags.Static);
            if (versionPropertyType == null)
            {
                return LEGACY_VERSION; //If no version is found, i consider it a legacy V1.
            }

            var version = versionPropertyType.GetValue(null);
            return version is int intVersion ? intVersion : LEGACY_VERSION;
        }
    }
}
