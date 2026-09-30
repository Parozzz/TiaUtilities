using InfoBox;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Reflection;
using TiaUtilities.Configuration;
using TiaUtilities.Constants;
using TiaUtilities.Generation.TextsEditor;
using TiaUtilities.Languages;
using TiaUtilities.Resources;
using TiaUtilities.SettingsStep.CustomControls;
using TiaUtilities.Styles;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.Generation
{
    public partial class GenModuleForm : Form
    {
        private enum SplitMode { NO_SPLIT, VERTICAL, HORIZONTAL }

        private const int CONTROL_ROW = 2;

        private readonly IGenModule module;
        private readonly TimedSaveHandler autoSaveHandler;

        private bool projectLoading = false;
        protected string? openProjectFilePath;

        private readonly ObservableObject<SplitMode> splitMode;
        private readonly ObservableObject<IGenModule.ModuleControl?> panel1ModuleControl;
        private readonly ObservableObject<IGenModule.ModuleControl?> panel2ModuleControl;
        private readonly Dictionary<IGenModule.ModuleControl, LabelColorizable> moduleControlLabelDict;

        public GenModuleForm(IGenModule generationProject, TimedSaveHandler autoSaveHandler)
        {
            InitializeComponent();

            this.module = generationProject;
            this.autoSaveHandler = autoSaveHandler;

            this.splitMode = new(SplitMode.NO_SPLIT);

            this.panel1ModuleControl = new(null);
            this.panel2ModuleControl = new(null);

            this.moduleControlLabelDict = [];

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
            this.toolsTextsEditorMenuItem.Click += (sender, args) =>
            {
                GenModuleTextsEditorForm textsEditorForm = new(this.module);
                textsEditorForm.ShowDialog(this);
            };
            #endregion

            #region TOP_MENU_PROGRAM
            this.programSettingsMenuItem.Click += (sender, args) => MainForm.ShowSettingsForm();
            this.programModuleSetupMenuItem.Click += (sender, args) => this.module.ShowSettings();
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

            #region TOP_MENU_VIEW
            this.viewSingleMenuItem.Click += (sender, args) => this.splitMode.Value = SplitMode.NO_SPLIT;
            this.viewSplitVerticalMenuItem.Click += (sender, args) => this.splitMode.Value = SplitMode.VERTICAL;
            this.viewSplitHorizontalMenuItem.Click += (sender, args) => this.splitMode.Value = SplitMode.HORIZONTAL;
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

            this.module.Init(this);

            this.splitMode.Changed += (sender, args) =>
            {
                var newSplitMode = args.NewValue;

                if(newSplitMode == SplitMode.VERTICAL || newSplitMode == SplitMode.HORIZONTAL)
                {
                    this.bottomSplitContainer.Panel2Collapsed = false;
                }
                else
                {
                    this.bottomSplitContainer.Panel2Collapsed = true;
                    this.panel2ModuleControl.Value = null;
                }

                this.bottomSplitContainer.Orientation = newSplitMode == SplitMode.VERTICAL ? Orientation.Vertical : Orientation.Horizontal;

                this.UpdateControlsLabels();
            };

            var labels = module.ModuleControls.Select(m =>
            {
                LabelColorizable label = new()
                {
                    AutoSize = true,

                    BackColor = Color.Transparent,
                    MouseHoverBackColor = Color.FromArgb(40, Color.Cyan),
                    MouseDownBackColor = Color.FromArgb(120, Color.Cyan),

                    BorderRadius = 4,
                    BorderWidth = 0,
                    BorderColor = Color.DimGray,

                    Padding = new(6),
                    Margin = new(4),
                    Text = m.Name,
                    TextAlign = ContentAlignment.MiddleCenter,

                    Font = StyleManager.Fonts.NORMAL_BOLD,
                };
                label.MouseClick += (sender, args) =>
                {
                    if (args.Button == MouseButtons.Left || splitMode.Value == SplitMode.NO_SPLIT)
                    {
                        this.panel1ModuleControl.Value = m;
                    }
                    else if (args.Button == MouseButtons.Right)
                    {
                        this.panel2ModuleControl.Value = m;
                    }
                };

                this.moduleControlLabelDict.Add(m, label);
                return label;
            });
            this.selectControlButtonPanel.Controls.AddRange([.. labels]);

            this.panel1ModuleControl.Changed += (sender, args) =>
            {
                var oldModuleControl = args.OldValue;
                var newModuleControl = args.NewValue;

                if(newModuleControl == panel2ModuleControl.Value)
                {
                    panel2ModuleControl.Value = null;
                }

                this.bottomSplitContainer.SuspendLayout();
                DllImports.SuspendDrawing(this.bottomSplitContainer);

                this.bottomSplitContainer.Panel1.Controls.Clear();

                if (newModuleControl != null)
                {
                    var control = newModuleControl.RequestControlCallback();
                    this.bottomSplitContainer.Panel1.Controls.Add(control);
                }

                this.UpdateControlsLabels();

                DllImports.ResumeDrawing(this.bottomSplitContainer);
                this.bottomSplitContainer.ResumeLayout();
            }; 
            
            this.panel2ModuleControl.Changed += (sender, args) =>
            {
                var oldModuleControl = args.OldValue;
                var newModuleControl = args.NewValue;

                if (newModuleControl == panel1ModuleControl.Value)
                {
                    panel1ModuleControl.Value = null;
                }

                this.bottomSplitContainer.SuspendLayout();
                DllImports.SuspendDrawing(this.bottomSplitContainer);

                this.bottomSplitContainer.Panel2.Controls.Clear();

                if (newModuleControl != null)
                {
                    var control = newModuleControl.RequestControlCallback();
                    this.bottomSplitContainer.Panel2.Controls.Add(control);
                }

                this.UpdateControlsLabels();

                DllImports.ResumeDrawing(this.bottomSplitContainer);
                this.bottomSplitContainer.ResumeLayout();
            };

            this.bottomSplitContainer.Panel1Collapsed = false;
            this.bottomSplitContainer.Panel2Collapsed = true;

            this.panel1ModuleControl.Value = this.module.ModuleControls.FirstOrDefault();

            Translate();
        }

        private void UpdateControlsLabels()
        {
            this.moduleControlLabelDict.Values.ForEach(l =>
            {
                l.BorderWidth = 0;
            });

            var pos1Control = this.panel1ModuleControl.Value;
            if(pos1Control != null)
            {
                if (this.moduleControlLabelDict.TryGetValue(pos1Control, out var label))
                {
                    label.BorderWidth = 2;
                    label.BorderColor = Color.DimGray;
                }
            }

            var pos2Control = this.panel2ModuleControl.Value;
            if (pos2Control != null)
            {
                if (this.moduleControlLabelDict.TryGetValue(pos2Control, out var label))
                {
                    label.BorderWidth = 2;
                    label.BorderColor = Color.CadetBlue;
                }
            }
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
                        this.module.ShowSettings();
                        return true;
                    case Keys.Q | Keys.Control:
                        this.module.OpenPlaceholderViewer(this);
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
            if (string.IsNullOrEmpty(filePath))
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
            if (string.IsNullOrEmpty(filePath))
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
