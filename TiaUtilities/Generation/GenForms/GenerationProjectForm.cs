using InfoBox;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaUtilities.Generation.GenForms.Alarm;
using TiaXmlReader.AutoSave;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Languages;
using TiaXmlReader.Utility;

namespace TiaUtilities.Generation.GenForms
{
    public partial class GenerationProjectForm : Form
    {
        private readonly IGenerationProject generationProject;
        private readonly TimedSaveHandler autoSaveHandler;
        private readonly GridSettings gridSettings;

        private IGenerationProjectSave? oldProjectSave;
        private string? lastFilePath;

        public GenerationProjectForm(IGenerationProject generationProject, TimedSaveHandler autoSaveHandler, GridSettings gridSettings)
        {
            this.generationProject = generationProject;
            this.autoSaveHandler = autoSaveHandler;
            this.gridSettings = gridSettings;

            InitializeComponent();

            Init();
        }

        public void Init()
        {
            #region FORM_CLOSING_PROJECT_COMPARE
            this.FormClosing += (sender, args) =>
            {
                InformationBoxResult result = InformationBoxResult.None;
                if (this.oldProjectSave == null)
                {
                    result = InformationBox.Show("Do you want to save this project?", title: "Project not saved", buttons: InformationBoxButtons.YesNoCancel);
                }
                else
                {
                    var projectSave = this.generationProject.CreateProjectSave();
                    if (Utils.AreDifferentObject(projectSave, this.oldProjectSave))
                    {
                        result = InformationBox.Show("Do you want to save this project?", title: "Project different from last save", buttons: InformationBoxButtons.YesNoCancel);
                    }
                }

                if (result == InformationBoxResult.Yes)
                {
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

                        this.generationProject.ExportXML(folderName);
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

            generationProject.Init(this);

            this.projectTableLayout.Controls.Add(this.generationProject.GetTopControl());
            this.projectTableLayout.Controls.Add(this.generationProject.GetBottomControl());

            Translate();
        }

        private void Translate()
        {
            this.Text = this.generationProject.GetFormLocalizatedName();

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
                        this.ProjectSave();
                        return true; //Return required otherwise will write the letter.
                    case Keys.L | Keys.Control:
                        this.ProjectLoad();
                        return true; //Return required otherwise will write the letter.
                }

                if(this.generationProject.ProcessCmdKey(ref msg, keyData))
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

        private void ProjectSave(bool saveAs = false)
        {
            var projectSave = this.generationProject.CreateProjectSave();

            var saveOK = projectSave.Save(ref lastFilePath, saveAs || !File.Exists(lastFilePath));
            if (!saveOK)
            {
                return;
            }

            this.Text = this.generationProject.GetFormLocalizatedName() + ". Project File: " + lastFilePath;
            this.oldProjectSave = projectSave;
        }

        private void ProjectLoad()
        {
            var projectSave = this.generationProject.Load(ref lastFilePath);
            if (projectSave != null)
            {
                this.Text = this.generationProject.GetFormLocalizatedName() + ". Project File: " + lastFilePath;
                this.oldProjectSave = projectSave;
            }
        }
    }
}
