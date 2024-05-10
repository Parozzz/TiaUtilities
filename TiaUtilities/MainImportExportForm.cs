using Microsoft.WindowsAPICodePack.Dialogs;
using System.Globalization;
using TiaXmlReader.Utility;
using TiaXmlReader.Generation.Alarms.GenerationForm;
using TiaXmlReader.Generation.IO.GenerationForm;
using TiaXmlReader.AutoSave;
using TiaXmlReader.SimaticML;
using System.Xml;
using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.Generation.Configuration;
using Jint;
using TiaXmlReader.Javascript;
using Timer = System.Windows.Forms.Timer;

namespace TiaXmlReader
{
    public partial class MainImportExportForm : Form
    {
        private readonly ProgramSettings programSettings;
        private readonly TimedSaveHandler autoSaveHandler;
        private readonly JavascriptErrorReportThread jsErrorHandlingThread;

        public MainImportExportForm()
        {
            InitializeComponent();

            this.programSettings = ProgramSettings.Load();
            this.programSettings.Save(); //To create file if not exist!

            this.autoSaveHandler = new TimedSaveHandler(programSettings, this.autoSaveComboBox.ComboBox);
            this.jsErrorHandlingThread = new JavascriptErrorReportThread();

            Init();
        }

        private void Init()
        {
            LogHandler.INSTANCE.Init();
            LogHandler.INSTANCE.Start();

            jsErrorHandlingThread.Init();
            jsErrorHandlingThread.Start();

            tiaVersionComboBox.Text = "" + programSettings.lastTIAVersion;

            this.languageComboBox.Items.AddRange(new string[] { "it-IT", "en-US" });
            this.languageComboBox.TextChanged += (object sender, EventArgs args) =>
            {
                try
                {
                    var culture = CultureInfo.GetCultureInfo(this.languageComboBox.Text);
                    LocalizationVariables.LANG = programSettings.ietfLanguage = culture.IetfLanguageTag;
                }
                catch (CultureNotFoundException ex)
                {
                    this.languageComboBox.SelectedItem = this.languageComboBox.Items[0];
                    Utils.ShowExceptionMessage(ex);
                }
            };
            this.languageComboBox.Text = programSettings.ietfLanguage; //Call this after so the text changed event changes the system lang.

            #region SETTINGS_SAVE_TICK + AUTO_SAVE
            this.autoSaveHandler.Start();

            var settingsWrapper = new AutoSaveSettingsWrapper(this.programSettings);
            settingsWrapper.Scan();

            var timer = new Timer { Interval = 1000 };
            timer.Start();
            timer.Tick += (sender, e) =>
            {
                if (!settingsWrapper.CompareSnapshot())
                {
                    this.programSettings.Save();
                }
            };
            #endregion
        }

        private void TiaVersionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (uint.TryParse(tiaVersionComboBox.Text, out var version))
            {
                Constants.VERSION = programSettings.lastTIAVersion = version;
            }
        }

        private void DbDuplicationMenuItem_Click(object sender, EventArgs e)
        {
            var dbDuplicationForm = new DBDuplicationForm(programSettings);
            dbDuplicationForm.ShowInTaskbar = false;
            dbDuplicationForm.ShowDialog();
        }

        private void GenerateIOMenuItem_Click(object sender, EventArgs e)
        {
            new IOGenerationForm(this.jsErrorHandlingThread, this.autoSaveHandler, this.programSettings.IOSettings, this.programSettings.GridSettings).Show(this);
        }

        private void GenerateAlarmsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AlarmGenerationForm(this.jsErrorHandlingThread, this.autoSaveHandler, this.programSettings.AlarmSettings, this.programSettings.GridSettings).Show(this);
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

                var xmlDocument = new XmlDocument();
                xmlDocument.Load(filePath);
                var xmlNodeConfiguration = SimaticMLParser.ParseXML(xmlDocument);


                xmlNodeConfiguration.UpdateID_UId(new IDGenerator());
                var document = SimaticMLParser.CreateDocument();
                var generatedXML = xmlNodeConfiguration.Generate(document);

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
                    var __ = SimaticDataType.BOOLEAN;

                    filePath = fileDialog.FileName;
                    document.DocumentElement.AppendChild(generatedXML);
                    document.Save(filePath);
                }
                var _debug = "" + "";
            }
        }

        private string JS;
        private void JSToolStripMenuItem_Click(object sender, EventArgs args)
        {
            var configForm = new ConfigForm("TEST JS")
            {
                ControlWidth = 500
            };

            var mainGroup = configForm.Init();
            mainGroup.AddLine(ConfigFormLineTypes.JAVASCRIPT)
                  .LabelText("Espressione").Height(300)
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