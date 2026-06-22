using ClosedXML.Excel;
using InfoBox;
using Jint;
using Microsoft.WindowsAPICodePack.Dialogs;
using SimaticML;
using SimaticML.API;
using SimaticML.Blocks;
using SimaticML.nBlockAttributeList;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using TiaUtilities.Constants;
using TiaUtilities.DbVisualization;
using TiaUtilities.Editors.ErrorReporting;
using TiaUtilities.Generation;
using TiaUtilities.Generation.Alarms;
using TiaUtilities.Generation.Alarms.Data;
using TiaUtilities.Generation.Alarms.Module;
using TiaUtilities.Generation.Configuration;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.IO;
using TiaUtilities.Generation.IO.Module;
using TiaUtilities.Generation.SettingsNew;
using TiaUtilities.Languages;
using TiaUtilities.Resources;
using TiaUtilities.SettingsNew.Bindings;
using TiaUtilities.Utility;
using Timer = System.Windows.Forms.Timer;

namespace TiaUtilities
{
    public partial class MainForm : Form
    {
        public static ProgramSettingsV1 Settings { get; private set; } = new();
        public static SettingsBindings SettingsBindings { get; private set; } = new();

        private static void LoadLanguage()
        {
            try
            {
                var culture = CultureInfo.GetCultureInfo(MainForm.Settings.IetfLanguage);
                LocaleVariables.LANG = culture.IetfLanguageTag;
            }
            catch (CultureNotFoundException ex)
            {
                Utils.ShowExceptionMessage(ex);
            }
        }

        private static void LoadTIAVersion()
        {
            SimaticMLAPI.TIA_VERSION = MainForm.Settings.TIAVersion;
        }

        private readonly TimedSaveHandler autoSaveHandler;
        private readonly ErrorReportThread errorThread;

        public static readonly ErrorReportThread JavascriptErrorThread = new();

        public MainForm()
        {
            InitializeComponent();

            Settings = SavesLoader.LoadWithoutDialog(ProgramSettingsV1.GetFilePath(), "json") is ProgramSettingsV1 loadedSave ? loadedSave : new();
            Settings.Save(); //To create file if not exist!

            this.autoSaveHandler = new TimedSaveHandler();
            this.errorThread = new();

            Init();
        }

        private void Init()
        {
            this.saveMenuItem.Click += (sender, args) => MainForm.Settings.Save();
            this.programSettingsMenuItem.Click += (sender, args) => new SettingsForm(MainForm.SettingsBindings).ShowDialog(this);

            this.sampleXMLMenuItem.Click += (sender, args) =>
            {
                SimaticMLExamples.CreateFCExample();
                SimaticMLExamples.CreateGlobalDBExample();
            };

            LogHandler.INSTANCE.Init();
            LogHandler.INSTANCE.Start();

            JavascriptErrorThread.Init();
            JavascriptErrorThread.Start();

            this.errorThread.Init();
            this.errorThread.Start();

            MainForm.LoadLanguage();
            MainForm.LoadTIAVersion();
            this.autoSaveHandler.Start(MainForm.Settings.AutoSaveTime * 1000);

            MainForm.SettingsBindings
                .MacroSection(() => Locale.GENERICS_PROGRAM, () => true, () => MainForm.Settings)

                .Section(Locale.PROGRAM_SETTINGS_AUTO_SAVE)
                .AddInt(nameof(ProgramSettingsV1.AutoSaveTime))

                .Section(Locale.PROGRAM_SETTINGS_LANGUAGE)
                .AddStringList(nameof(ProgramSettingsV1.IetfLanguage), ["it-IT", "en-US"])

                .Section(Locale.PROGRAM_SETTINGS_TIA_VERSION)
                .AddUnsignedNumberList(nameof(ProgramSettingsV1.TIAVersion), [16, 17, 18, 19])

                .MacroSection(() => Locale.GRID_SETTINGS, () => true, () => MainForm.Settings.GridSettings)

                .Section(Locale.GRID_SETTINGS_SELECTED_CELL)
                .AddColor(nameof(GridSettings.SelectedCellBackColor), Locale.GRID_SETTINGS_SELECTED_CELL_BACK_COLOR)
                .AddColor(nameof(GridSettings.SelectedCellForeColor), Locale.GRID_SETTINGS_SELECTED_CELL_FORE_COLOR)

                .Section(Locale.GRID_SETTINGS_BORDERS)
                .AddInt(nameof(GridSettings.BorderWeight), Locale.GRID_SETTINGS_BORDERS_WEIGHT)
                .AddColor(nameof(GridSettings.SingleSelectedCellBorderColor), Locale.GRID_SETTINGS_BORDERS_SELECTED_CELL_COLOR)
                .AddColor(nameof(GridSettings.DragDropStartCellSelectedBackColor), Locale.GRID_SETTINGS_DRAG_DROP_CELL_BACK)

                .Section(Locale.GRID_SETTINGS_DRAG_DOWN)
                .AddColor(nameof(GridSettings.SelectedCellTriangleColor), Locale.GRID_SETTINGS_DRAG_DOWN_TRIANGLE_COLOR)
                .AddColor(nameof(GridSettings.DragSelectedCellBorderColor), Locale.GRID_SETTINGS_DRAG_DOWN_CELL_BACK)

                .Section(Locale.GRID_SETTINGS_PREVIEW)
                .AddColor(nameof(GridSettings.PreviewColor), Locale.GRID_SETTINGS_PREVIEW_FORE_COLOR);

            MainForm.Settings.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ProgramSettingsV1.AutoSaveTime))
                {
                    this.autoSaveHandler.Start(MainForm.Settings.AutoSaveTime * 1000);
                }
                else if (args.PropertyName == nameof(ProgramSettingsV1.TIAVersion))
                {
                    MainForm.LoadTIAVersion();
                }
                else if (args.PropertyName == nameof(ProgramSettingsV1.IetfLanguage))
                {
                    //This should stay always in english. In case someone set an unkown language, this will be neautral.
                    var result = InformationBox.Show("Do you want to restart application?", "Restart to change language", buttons: InformationBoxButtons.YesNo);
                    if (result == InformationBoxResult.Yes)
                    {
                        MainForm.Settings.Save();

                        Application.Restart();
                        Environment.Exit(0);
                        return;
                    }
                }

                MainForm.Settings.Save();
            };

            Timer settingsDirtyTimer = new() { Interval = 1000 };
            settingsDirtyTimer.Tick += (sender, args) =>
            {
                if (MainForm.Settings.IsDirty())
                {
                    MainForm.Settings.Save();
                }
            };
            settingsDirtyTimer.Start();

            this.ioGenButton.BackgroundImage = ImageResources.ALIAS_GENERATOR;
            this.ioGenButton.BackgroundImageLayout = ImageLayout.Zoom;
            this.ioGenButton.Click += (sender, args) => OpenIOGenModuleForm();

            this.alarmGenButton.BackgroundImage = ImageResources.ALARM_GENERATOR;
            this.alarmGenButton.BackgroundImageLayout = ImageLayout.Zoom;
            this.alarmGenButton.Click += (sender, args) => OpenAlarmGenModuleForm();

            this.duplicateDBButton.BackgroundImage = ImageResources.DUPLICATE_DB;
            this.duplicateDBButton.BackgroundImageLayout = ImageLayout.Zoom;
            this.duplicateDBButton.Click += (sender, args) => new DBDuplicationForm(Settings) { ShowInTaskbar = false }.ShowDialog();

            Translate();
        }

        private void Translate()
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.Text = $"{Locale.MAIN_FORM}";

            this.fileToolStripMenuItem.Text = Locale.GENERICS_FILE;
            this.saveMenuItem.Text = Locale.GENERICS_SAVE + " (CTRL+S)";
            this.loadToolStripMenuItem.Text = Locale.GENERICS_LOAD + " (CTRL+L)";

            this.programMenuItem.Text = Locale.GENERICS_PROGRAM;
            this.programSettingsMenuItem.Text = Locale.GENERICS_SETTINGS + " (CTRL+P)";


            this.ioGenButton.Text = Locale.MAIN_FORM_TOP_IO_GENERATION;
            this.alarmGenButton.Text = Locale.MAIN_FORM_TOP_ALARM_GENERATOR;
            this.duplicateDBButton.Text = Locale.MAIN_FORM_TOP_DB_DUPLICATION;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                switch (keyData)
                {
                    case Keys.P | Keys.Control:
                        new SettingsForm(MainForm.SettingsBindings).ShowDialog(this);
                        return true;
                    case Keys.S | Keys.Control:
                        MainForm.Settings.Save();
                        return true;
                    case Keys.L | Keys.Control:
                        this.loadToolStripMenuItem.PerformClick();
                        return true; //Return required otherwise will write the letter.
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var filePath = "";

            var saveObject = SavesLoader.LoadWithDialog(ref filePath, ProgramConstants.SAVE_FILE_EXTENSION);

            GenModuleForm genForm;
            if (saveObject is IOGenSaveV1)
            {
                genForm = OpenIOGenModuleForm();
            }
            else if (saveObject is AlarmGenSaveV1)
            {
                genForm = OpenAlarmGenModuleForm();
            }
            else
            {
                return;
            }


            genForm.SetOpenProjectFilePath(filePath);
            genForm.ModuleLoad(saveObject);
        }

        private GenModuleForm OpenIOGenModuleForm()
        {
            IOGenModule ioGenProject = new(this.errorThread);
            GenModuleForm projectForm = new(ioGenProject, autoSaveHandler)
            {
                Width = 1400,
                Height = 850
            };
            projectForm.Show(this);
            return projectForm;
        }

        private GenModuleForm OpenAlarmGenModuleForm()
        {
            AlarmGenModule alarmGenProject = new(this.errorThread);
            GenModuleForm projectForm = new(alarmGenProject, autoSaveHandler)
            {
                Width = 1400,
                Height = 850
            };
            projectForm.Show(this);
            return projectForm;
        }

        private void ImportXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = false,
                EnsurePathExists = true,
                EnsureFileExists = true,
                DefaultExtension = ".xml",
                Filters = { new CommonFileDialogFilter("XML Files (*.xml)", "*.xml") }
            };

            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var filePath = fileDialog.FileName;
                if (filePath == null)
                {
                    return;
                }

                var xmlDocument = new XmlDocument();
                xmlDocument.Load(filePath);

                var xmlNodeConfiguration = SimaticMLAPI.ParseXML(xmlDocument);
                if (xmlNodeConfiguration == null)
                {
                    return;
                }

                xmlNodeConfiguration.UpdateID_UId(new IDGenerator());

                fileDialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = false,
                    EnsurePathExists = true,
                    EnsureFileExists = false,
                    DefaultExtension = ".xml",
                    Filters = { new CommonFileDialogFilter("XML Files (*.xml)", "*.xml") }
                };

                if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    filePath = fileDialog.FileName;
                    if (filePath == null)
                    {
                        return;
                    }

                    var document = SimaticMLAPI.CreateDocument(xmlNodeConfiguration);
                    document.Save(filePath);
                }
                var _debug = "" + "";
            }
        }

        private string? JS;
        private void JSToolStripMenuItem_Click(object sender, EventArgs args)
        {
            var configForm = new ConfigForm("TEST JS")
            {
                ControlWidth = 500
            };

            var mainGroup = configForm.Init();
            mainGroup.AddJavascript().Label("Espressione").Height(300)
                  .ControlText(JS)
                  .TextChanged(str => JS = str);

            configForm.FormClosed += (s, e) =>
            {
                try
                {
                    if (JS == null)
                    {
                        return;
                    }

                    using (var engine = new Engine())
                    {
                        engine.SetValue("nome", "cacca");

                        var eval = engine.Evaluate(JS);

                        var nome = engine.GetValue("nome");
                        var _ = "";
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex);
                }
            };

            configForm.StartShowingAtCursor();
            configForm.Init();
            configForm.Show(this);

        }

        private void SvgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SvgTestForm svgTestForm = new();
            svgTestForm.Show(this);
        }

        private void dbVisualizationMenuItem_Click(object sender, EventArgs e)
        {
            TreeViewDBVisualization dbVisualizationForm = new();
            dbVisualizationForm.Show(this);
        }

        private void exportAllMembersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = false,
                EnsurePathExists = true,
                EnsureFileExists = true,
                DefaultExtension = ".xml",
                Filters = { new CommonFileDialogFilter("XML Files (*.xml)", "*.xml") }
            };

            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var fileName = fileDialog.FileName;
                if (fileName == null)
                {
                    return;
                }

                List<string> members = [];

                var xml = SimaticMLAPI.ParseFile(fileName);
                if (xml is BlockGlobalDB globalDB)
                {
                    members.AddRange(globalDB.GetAllMemberAddress());
                }
                else if (xml is BlockInstanceDB instanceDB)
                {
                    members.AddRange(instanceDB.GetAllMemberAddress());
                }
                else if (xml is BlockUDT udt)
                {
                    members.AddRange(udt.GetAllMemberAddress());
                }

                string tempFilePath = System.IO.Path.GetTempPath() + "tempPaths.txt";
                File.WriteAllText(tempFilePath, String.Join("\n", members));

                Process.Start("notepad.exe", tempFilePath);
            }
        }

        private void QuestionMarkMenuItem_Click(object sender, EventArgs e)
        {
            new QuestionMarkForm().Show();
        }

        private void CreateTextListsExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateParametersExcel();
        }

        private void CreateParametersExcel()
        {
            const int COLUMN_IDS = 1;
            const int COLUMN_TEXTS = 2;

            const int START_TIMES = 0;
            const int START_VAR = 100;
            const int START_FILTERS = 200;
            const int START_BOOLS = 0;

            var fileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = false,
                EnsurePathExists = true,
                EnsureValidNames = true,
                Multiselect = true,
                DefaultExtension = ".xml",
                Filters = { new CommonFileDialogFilter("XML Files", "*.xml") }
            };

            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Dictionary<string, Pair<int, string>> paramDict = [];

                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("DiscreteAlarms");

                int row = 1;

                var directory = "";
                foreach (var filePath in fileDialog.FileNames)
                {
                    if(directory == "")
                    {
                        directory = Path.GetDirectoryName(filePath);
                    }

                    var xmlNodeConfiguration = SimaticMLAPI.ParseFile(filePath);
                    if (xmlNodeConfiguration is BlockFB blockFB)
                    {
                        worksheet.Cell(row, COLUMN_IDS).Value = "TEXT_LIST";
                        worksheet.Cell(row, COLUMN_TEXTS).Value = blockFB.AttributeList.BlockName;
                        row++;

                        worksheet.Cell(row, COLUMN_IDS).Value = -1;
                        worksheet.Cell(row, COLUMN_TEXTS).Value = "M{module_name}";
                        row++;

                        var parameterMember = blockFB.AttributeList.INOUT.GetItems().FirstOrDefault(m => m.MemberName.ToLower().Equals("parametri"));
                        if (parameterMember == null)
                        {
                            return;
                        }


                        void parseMember(Member? member, int startId)
                        {
                            if (member == null)
                            {
                                return;
                            }

                            foreach (var subElement in member.SubElements)
                            {
                                if(int.TryParse(subElement.Path, out int path))
                                {
                                    string text = ""; 
                                    
                                    var comment = subElement.Comment;
                                    if (comment != null && comment.GetItems().Count > 0)
                                    {
                                        text = comment.GetItems()[0].LangText;
                                    }

                                    worksheet.Cell(row, COLUMN_IDS).Value = path + startId;
                                    worksheet.Cell(row, COLUMN_TEXTS).Value = text;

                                    row++;
                                }
                            }
                        }

                        var subSection = parameterMember.SubSection;
                        if(subSection != null)
                        {
                            var subSectionMembers = subSection.GetItems();
                            if(subSectionMembers != null)
                            {
                                var tempiMember = subSectionMembers.FirstOrDefault(m => m.MemberName.ToLower().Equals("tempi"));
                                var varMember = subSectionMembers.FirstOrDefault(m => m.MemberName.ToLower().Equals("variabili"));
                                var filtersMember = subSectionMembers.FirstOrDefault(m => m.MemberName.ToLower().Equals("filtrisensori"));
                                var boolMember = subSectionMembers.FirstOrDefault(m => m.MemberName.ToLower().Equals("bool"));

                                parseMember(tempiMember, START_TIMES);
                                parseMember(varMember, START_VAR);
                                parseMember(filtersMember, START_FILTERS);
                                parseMember(boolMember, START_BOOLS);
                            }
                        }
                    }
                    else
                    {
                        InformationBox.Show("The selected block is NOT a BlockFB or file is invalid.", "Invalid imported xml", icon: InformationBoxIcon.Exclamation);
                    }
                }


                workbook.SaveAs($"{directory}/textsExcel.xlsx");
            }
        }
    }
}