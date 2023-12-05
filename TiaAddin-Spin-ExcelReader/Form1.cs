using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using SpinXmlReader.Block;
using SpinXmlReader.TagTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using TiaXmlReader;
using TiaXmlReader.AlarmGeneration;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.Cad;

namespace SpinXmlReader
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            tiaVersionComboBox.Text = "" + Constants.VERSION;
        }

        private void tiaVersionComboBox_TextUpdate(object sender, EventArgs e)
        {
            if (uint.TryParse(tiaVersionComboBox.Text, out var version))
            {
                Constants.VERSION = version;
            }
        }

        private void ConfigExcelPathTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                CheckFileExists = true
            };

            var result = fileDialog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                configExcelPathTextBox.Text = fileDialog.FileName;
            }
        }

        private void ExportPathTextBlock_MouseClick(object sender, MouseEventArgs e)
        {
            var fileDialog = new FolderBrowserDialog();

            var result = fileDialog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                exportPathTextBlock.Text = fileDialog.SelectedPath;
            }
        }

        private void GenerateXMLExportFiles_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                using (var configWorkbook = new XLWorkbook(configExcelPathTextBox.Text))
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
                            var importConsumerAlarms = new GenerationConsumerAlarms();
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
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
                Console.WriteLine("Exception: {0}", ex.ToString());
            }
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