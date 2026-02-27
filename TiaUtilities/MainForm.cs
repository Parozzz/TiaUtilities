using InfoBox;
using Jint;
using Microsoft.WindowsAPICodePack.Dialogs;
using SimaticML;
using SimaticML.API;
using System.Globalization;
using System.Xml;
using TiaUtilities.DbVisualization;
using TiaUtilities.Editors.ErrorReporting;
using TiaUtilities.Generation;
using TiaUtilities.Generation.Alarms.Module;
using TiaUtilities.Generation.Configuration;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.IO.Module;
using TiaUtilities.Generation.SettingsNew;
using TiaUtilities.Languages;
using TiaUtilities.SettingsNew;
using TiaUtilities.SettingsNew.Bindings;
using TiaUtilities.Utility;
using Timer = System.Windows.Forms.Timer;

namespace TiaUtilities
{
    public partial class MainForm : Form
    {
        public static ProgramSettings Settings { get; private set; } = new();
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

            Settings = SavesLoader.LoadWithoutDialog(ProgramSettings.GetFilePath(), "json") is ProgramSettings loadedSave ? loadedSave : new();
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
                .MacroSection(() => Locale.GENERICS_PROGRAM, () => MainForm.Settings)

                .Section(Locale.PROGRAM_SETTINGS_AUTO_SAVE)
                .AddInt(nameof(ProgramSettings.AutoSaveTime))

                .Section(Locale.PROGRAM_SETTINGS_LANGUAGE)
                .AddStringList(nameof(ProgramSettings.IetfLanguage), ["it-IT", "en-US"])

                .Section(Locale.PROGRAM_SETTINGS_TIA_VERSION)
                .AddUnsignedNumberList(nameof(ProgramSettings.TIAVersion), [16, 17, 18, 19])

                .MacroSection(() => Locale.PROGRAM_SETTINGS_GRID_PREFERENCES, () => MainForm.Settings.GridSettings)

                .Section(Locale.PROGRAM_SETTINGS_GRID_PREFERENCES_COLORS)
                .AddColor(nameof(GridSettings.SingleSelectedCellBorderColor), Locale.PROGRAM_SETTINGS_GRID_PREFERENCES_COLORS_SELECTED_CELL_BORDER)
                .AddColor(nameof(GridSettings.DragSelectedCellBorderColor), Locale.PROGRAM_SETTINGS_GRID_PREFERENCES_COLORS_SELECTED_CELL_BORDER)
                .AddColor(nameof(GridSettings.SelectedCellTriangleColor), Locale.PROGRAM_SETTINGS_GRID_PREFERENCES_COLORS_DRAGGED_CELL_BACK)
                .AddColor(nameof(GridSettings.PreviewColor), Locale.PROGRAM_SETTINGS_GRID_PREFERENCES_COLORS_PREVIEW_FORE);

            MainForm.Settings.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ProgramSettings.AutoSaveTime))
                {
                    this.autoSaveHandler.Start(MainForm.Settings.AutoSaveTime * 1000);
                }
                else if (args.PropertyName == nameof(ProgramSettings.TIAVersion))
                {
                    MainForm.LoadTIAVersion();
                }
                else if (args.PropertyName == nameof(ProgramSettings.IetfLanguage))
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

            this.ioGenButton.BackgroundImage = Image.FromFile("Resources/Images/AliasGenerator.png");
            this.ioGenButton.BackgroundImageLayout = ImageLayout.Zoom;
            this.ioGenButton.Click += (sender, args) => OpenIOGenModuleForm();

            this.alarmGenButton.BackgroundImage = Image.FromFile("Resources/Images/AlarmGenerator.png");
            this.alarmGenButton.BackgroundImageLayout = ImageLayout.Zoom;
            this.alarmGenButton.Click += (sender, args) => OpenAlarmGenModuleForm();

            this.duplicateDBButton.BackgroundImage = Image.FromFile("Resources/Images/DuplicateDB.png");
            this.duplicateDBButton.BackgroundImageLayout = ImageLayout.Zoom;
            this.duplicateDBButton.Click += (sender, args) => new DBDuplicationForm(Settings) { ShowInTaskbar = false }.ShowDialog();

            Translate();
        }

        private void Translate()
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.Text = $"{Locale.MAIN_FORM} V{Program.VERSION}";

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

            var saveObject = SavesLoader.LoadWithDialog(ref filePath, Constants.SAVE_FILE_EXTENSION);

            GenModuleForm genForm;
            if (saveObject is IOGenSave)
            {
                genForm = OpenIOGenModuleForm();
            }
            else if (saveObject is AlarmGenSave)
            {
                genForm = OpenAlarmGenModuleForm();
            }
            else
            {
                return;
            }


            genForm.SetLastFilePath(filePath);
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

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsParser.CreateTestForm().Show(this);
        }
    }
}

/*
 


        private void generateButton_Click(object sender, EventArgs e)
        {
            GlobalIDGenerator.ResetID();

            var fc = new BlockFC();
            fc.Init();

            //BLOCK ATTRIBUTES
            var inputSection = fc.GetBlockAttributes().ComputeSection(SectionTypeEnum.INPUT);

            var variableInput = inputSection.AddMember("VariableInput", "Int");
            //BLOCK ATTRIBUTES

            //COMPILE UNITS
            var compileUnit = fc.AddCompileUnit();
            compileUnit.Init();

            var contactPart = compileUnit.AddPart(Part.Type.CONTACT).SetNegated();
            var coilPart = compileUnit.AddPart(Part.Type.COIL);

            compileUnit.AddPowerrail(new Dictionary<Part, string> {
                    { contactPart, "in" }
            });
            compileUnit.AddIdentWire(Access.Type.GLOBAL_VARIABLE, "IO.IN_01", contactPart, "operand");
            compileUnit.AddIdentWire(Access.Type.GLOBAL_VARIABLE, "IO.IN_02", coilPart, "operand");
            compileUnit.AddBoolANDWire(contactPart, "out", coilPart, "in");

            //COMPILE UNITS

            var xmlDocument = SiemensMLParser.CreateDocument();
            xmlDocument.DocumentElement.AppendChild(fc.Generate(xmlDocument));
            if (!string.IsNullOrEmpty(xmlPathTextBlock.Text))
            {
                xmlDocument.Save(xmlPathTextBlock.Text);
            }
        }

        private void generateTagTableButton_Click(object sender, EventArgs e)
        {
            var tagTable = SiemensMLParser.CreateEmptyTagTable();

            var tag = tagTable.AddTag();
            tag.SetLogicalAddress("%M40.0");
            tag.SetTagName("TagName?!");
            tag.SetDataTypeName("bool");

            var xmlDocument = SiemensMLParser.CreateDocument();
            xmlDocument.DocumentElement.AppendChild(tagTable.Generate(xmlDocument));
            xmlDocument.Save(excelPathTextBox.Text);
        }
        

        private void GenerateXMLExportFiles_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                //Allow opening file while having excel open. TIMESAVER!
                using (var stream = new FileStream(configExcelPathTextBox.Text,
                                 FileMode.Open,
                                 FileAccess.Read,
                                 FileShare.ReadWrite))
                {

                    using (var configWorkbook = new XLWorkbook(stream))
                    {
                        var configWorksheet = configWorkbook.Worksheets.Worksheet(1);

                        var configTypeValue = configWorksheet.Cell("A2").Value;
                        if (!configTypeValue.IsText || string.IsNullOrEmpty(configTypeValue.GetText()))
                        {
                            throw new ApplicationException("Configuration excel file invalid");
                        }

                        switch (configTypeValue.GetText().ToLower())
                        {
                            case "type1":
                                var alarmExcelImporter = new AlarmExcelImporter();
                                alarmExcelImporter.ImportExcelConfig(configWorksheet);

                                var alarmXmlGenerator = new AlarmXmlGenerator(alarmExcelImporter.GetConfiguration(), alarmExcelImporter.GetAlarmDataList(), alarmExcelImporter.GetDeviceDataList());
                                alarmXmlGenerator.GenerateBlocks();
                                alarmXmlGenerator.ExportXML(exportPathTextBlock.Text);
                                break;
                            case "type3":
                                var ioExcelImporter = new IOExcelImporter();
                                ioExcelImporter.ImportExcelConfig(configWorksheet);

                                var ioXmlGenerator = new IOXmlGenerator(ioExcelImporter.GetConfiguration(), ioExcelImporter.GetDataList());
                                ioXmlGenerator.GenerateBlocks();
                                ioXmlGenerator.ExportXML(exportPathTextBlock.Text);
                                break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }
        }
        
*/