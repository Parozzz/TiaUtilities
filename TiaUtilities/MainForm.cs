﻿using InfoBox;
using Jint;
using Microsoft.WindowsAPICodePack.Dialogs;
using SimaticML;
using SimaticML.API;
using SimaticML.Blocks;
using SimaticML.Blocks.FlagNet;
using SimaticML.Enums;
using System.Globalization;
using System.Xml;
using TiaUtilities;
using TiaUtilities.Generation.Alarms.Module;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Generation.GenModules;
using TiaUtilities.Generation.IO.Module;
using TiaUtilities.Languages;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Javascript;
using TiaXmlReader.Languages;
using TiaXmlReader.Utility;
using Timer = System.Windows.Forms.Timer;

namespace TiaXmlReader
{
    public partial class MainForm : Form
    {
        public static ProgramSettings Settings { get; private set; } = new();

        private readonly TimedSaveHandler autoSaveHandler;
        private readonly JavascriptErrorReportThread jsErrorHandlingThread;

        public MainForm()
        {
            InitializeComponent();

            Settings = SavesLoader.LoadWithoutDialog(ProgramSettings.GetFilePath(), "json") is ProgramSettings loadedSave ? loadedSave : new();
            Settings.Save(); //To create file if not exist!

            this.autoSaveHandler = new TimedSaveHandler();
            this.jsErrorHandlingThread = new JavascriptErrorReportThread();

            Init();
        }

        private void Init()
        {
            var ignoreSettingsAtStartup = true;

            LogHandler.INSTANCE.Init();
            LogHandler.INSTANCE.Start();

            jsErrorHandlingThread.Init();
            jsErrorHandlingThread.Start();

            this.tiaVersionComboBox.SelectedIndexChanged += (sender, args) =>
            {
                if (uint.TryParse(tiaVersionComboBox.Text, out var version))
                {
                    SimaticMLAPI.TIA_VERSION = version;

                    if (!ignoreSettingsAtStartup)
                    {
                        Settings.TIAVersion = version;
                        Settings.Save();
                    }
                }
            };
            this.tiaVersionComboBox.Text = "" + Settings.TIAVersion; //After the event so is applied!

            this.languageComboBox.Items.AddRange(["it-IT", "en-US"]);
            this.languageComboBox.TextChanged += (sender, args) =>
            {
                try
                {
                    var culture = CultureInfo.GetCultureInfo(this.languageComboBox.Text);
                    LocaleVariables.LANG = culture.IetfLanguageTag;

                    if (ignoreSettingsAtStartup)
                    {
                        return;
                    }

                    //Without save, after restart it would restore the old (Meaning it would be useless)
                    Settings.IetfLanguage = culture.IetfLanguageTag;
                    Settings.Save();

                    //This should stay always in english. In case someone set an unkown language, this will be neautral.
                    var result = InformationBox.Show("Do you want to restart application?", "Restart to change language", buttons: InformationBoxButtons.YesNo);
                    if (result == InformationBoxResult.Yes)
                    {
                        Application.Restart();
                        Environment.Exit(0);
                    }
                }
                catch (CultureNotFoundException ex)
                {
                    this.languageComboBox.SelectedItem = this.languageComboBox.Items[0];
                    Utils.ShowExceptionMessage(ex);
                }
            };
            this.languageComboBox.Text = Settings.IetfLanguage; //Call this after so the text changed event changes the system lang.

            this.autoSaveTimeTextBox.TextChanged += (sender, args) =>
            {
                if (int.TryParse(autoSaveTimeTextBox.Text, out int time))
                {
                    this.autoSaveHandler.Start(time * 1000);
                    if (!ignoreSettingsAtStartup)
                    {
                        Settings.AutoSaveTime = time;
                        Settings.Save(); //To create file if not exist!
                    }
                }
            };
            this.autoSaveTimeTextBox.Text = "" + Settings.AutoSaveTime; //Call this after so it start auto save

            Timer settingsDirtyTimer = new() { Interval = 1000 };
            settingsDirtyTimer.Tick += (sender, args) =>
            {
                if (MainForm.Settings.IsDirty())
                {
                    MainForm.Settings.Save();
                }
            };
            settingsDirtyTimer.Start();

            Translate();

            ignoreSettingsAtStartup = false;
        }

        private void Translate()
        {
            this.Text = Locale.MAIN_FORM;

            this.fileToolStripMenuItem.Text = Locale.GENERICS_FILE;
            this.loadToolStripMenuItem.Text = Locale.GENERICS_LOAD;
            this.autoSaveMenuItem.Text = Locale.MAIN_FORM_TOP_FILE_AUTO_SAVE;

            this.dbDuplicationMenuItem.Text = Locale.MAIN_FORM_TOP_DB_DUPLICATION;
            this.generateIOMenuItem.Text = Locale.MAIN_FORM_TOP_IO_GENERATION;
            this.generateAlarmsMenuItem.Text = Locale.MAIN_FORM_TOP_ALARM_GENERATOR;

            this.tiaVersionLabel.Text = Locale.MAIN_FORM_TIA_VERSION;
            this.languageLabel.Text = Locale.MAIN_FORM_LANGUAGE;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                switch (keyData)
                {
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

        private void DbDuplicationMenuItem_Click(object sender, EventArgs e) => new DBDuplicationForm(Settings) { ShowInTaskbar = false }.ShowDialog();

        private void GenerateIOMenuItem_Click(object sender, EventArgs e) => OpenIOGenModuleForm();

        private void GenerateAlarmsToolStripMenuItem_Click(object sender, EventArgs e) => OpenAlarmGenModuleForm();

        private GenModuleForm OpenIOGenModuleForm()
        {
            IOGenModule ioGenProject = new(jsErrorHandlingThread);
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
            AlarmGenModule alarmGenProject = new(jsErrorHandlingThread);
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

        private void SampleXMLMenuItem_Click(object sender, EventArgs e)
        {
            var fc = new BlockFC();

            //Add basic block information, like name, number and programming language.
            var attributeList = fc.AttributeList;
            attributeList.BlockName = "FC_TEST";
            attributeList.BlockNumber = 123;
            attributeList.ProgrammingLanguage = SimaticProgrammingLanguage.LADDER;

            //Add two temp variables to the specific section.

            var contactVariables = new List<SimaticVariable>();
            for (int i = 0; i < 10; i++)
            {
                var var = attributeList.TEMP.AddVariable($"tContact{i}", SimaticDataType.BOOLEAN);
                contactVariables.Add(var);
            }
            var coil1Var = attributeList.TEMP.AddVariable("tCoil1", SimaticDataType.BOOLEAN);
            var coil2Var = attributeList.TEMP.AddVariable("tCoil2", SimaticDataType.BOOLEAN);

            var segment = new SimaticLADSegment();
            segment.Title[LocaleVariables.CULTURE] = "Segment Title!";
            segment.Comment[LocaleVariables.CULTURE] = "Segment Comment! Much information here ...";

            var contactParts = new ContactPart[10];
            for (int i = 0; i < 10; i++)
            {
                contactParts[i] = new ContactPart() { Operand = contactVariables[i] };
            }
            var coil1 = new CoilPart() { Operand = coil1Var };
            var coil2 = new CoilPart() { Operand = coil2Var };

            //Brackets are important! C# will prioritize & to |, so the logic might break if not using them!
            var _ = segment.Powerrail & (contactParts[0] & (((contactParts[1] | contactParts[2]) & (contactParts[3] | contactParts[4])) | (contactParts[5] & contactParts[6])) & (contactParts[7] | contactParts[8]) | contactParts[9]) & coil1 & coil2;
            segment.Create(fc);
            /*
            var compileUnit = fc.AddCompileUnit();

            //Create the parts that will form the compileUnit (Segment). Also add the operand of the part to the local variable created before.
            //A Part is everything that does something inside a segment (Contact, Coil, Block) that is not an FC/FB.
            var contact = new ContactPart(compileUnit) { Operand = new SimaticLocalVariable("tVar1") };
            var coil = new CoilPart(compileUnit) { Operand = new SimaticLocalVariable("tVar2") };

            //Create the connections between the parts
            // |
            // | --- || --- () 
            // |
            var _ = compileUnit.Powerrail & contact & coil; //Create a AND connection between all the parts.
            */

            //Create skeleton for the XML Document and add the FC to it.
            var xmlDocument = SimaticMLAPI.CreateDocument(fc);

            var fileDialog = GenUtils.CreateFileDialog(false, null, "xml");
            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok && !string.IsNullOrEmpty(fileDialog.FileName))
            {
                xmlDocument.Save(fileDialog.FileName);
            }

            //Save the file.
            //xmlDocument.Save(Directory.GetCurrentDirectory() + "/fc.xml");
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