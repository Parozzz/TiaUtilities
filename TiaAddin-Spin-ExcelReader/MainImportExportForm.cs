using ClosedXML.Excel;
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TiaXmlReader;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Generation.IO_Cad;
using TiaXmlReader.GenerationForms.IO;
using TiaXmlReader.SimaticML;
using TiaXmlReader.Utility;

namespace SpinXmlReader
{
    public partial class MainImportExportForm : Form
    {
        private readonly SaveData saveData;
        public MainImportExportForm()
        {
            InitializeComponent();

            this.saveData = SaveData.Load();
            Init();
        }

        private void Init()
        {
            configExcelPathTextBox.Text = saveData.lastExcelFileName;
            exportPathTextBlock.Text = saveData.lastXMLExportPath;
            tiaVersionComboBox.Text = "" + saveData.lastTIAVersion;

            this.languageComboBox.Items.AddRange(new string[]{ "it-IT", "en-US"});
            this.languageComboBox.TextChanged += (object sender, EventArgs args) =>
            {
                try
                {
                    var culture = CultureInfo.GetCultureInfo(this.languageComboBox.Text);
                    SystemVariables.LANG = saveData.ietfLanguage = culture.IetfLanguageTag;
                    saveData.Save();
                }
                catch (CultureNotFoundException)
                {
                    this.languageComboBox.SelectedItem = this.languageComboBox.Items[0];
                }
            };
            this.languageComboBox.Text = saveData.ietfLanguage; //Call this after so the text changed event changes the system lang.
        }

        private void TiaVersionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (uint.TryParse(tiaVersionComboBox.Text, out var version))
            {
                saveData.lastTIAVersion = version;
                saveData.Save();

                Constants.VERSION = saveData.lastTIAVersion;
            }
        }

        private void ConfigExcelPathTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                CheckFileExists = true
            };

            try
            {
                fileDialog.InitialDirectory = string.IsNullOrEmpty(saveData.lastExcelFileName) ? "" : Path.GetDirectoryName(saveData.lastExcelFileName);
            }
            catch  {  }

            var result = fileDialog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                saveData.lastExcelFileName = fileDialog.FileName;
                saveData.Save();

                configExcelPathTextBox.Text = saveData.lastExcelFileName;
            }
        }

        private void ExportPathTextBlock_MouseClick(object sender, MouseEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                CheckFileExists = false
            };

            try
            {
                fileDialog.InitialDirectory = saveData.lastXMLExportPath;
            }
            catch { }

            var result = fileDialog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                saveData.lastXMLExportPath = Path.GetDirectoryName(fileDialog.FileName);
                saveData.Save();

                exportPathTextBlock.Text = saveData.lastXMLExportPath;
            }
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
                                var importConsumerAlarms = new GenerationUserAlarms();
                                importConsumerAlarms.ImportExcelConfig(configWorksheet);
                                importConsumerAlarms.GenerateBlocks();
                                importConsumerAlarms.ExportXML(exportPathTextBlock.Text);
                                break;
                            case "type2":
                                var generationIO_Cad = new GenerationIO_CAD();
                                generationIO_Cad.ImportExcelConfig(configWorksheet);
                                generationIO_Cad.GenerateBlocks();
                                generationIO_Cad.ExportXML(exportPathTextBlock.Text);
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
                Console.WriteLine("Exception: {0}", ex.ToString());
            }
        }

        private void DbDuplicationMenuItem_Click(object sender, EventArgs e)
        {
            var dbDuplicationForm = new DBDuplicationForm(saveData);
            dbDuplicationForm.ShowInTaskbar = false;
            dbDuplicationForm.ShowDialog();
        }

        private void GenerateIOMenuItem_Click(object sender, EventArgs e)
        {
            new IOGenerationForm().Show();
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
*/