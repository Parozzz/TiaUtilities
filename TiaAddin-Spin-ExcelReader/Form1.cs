using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using SpinXmlReader.Block;
using SpinXmlReader.TagTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using TiaXmlReader;
using TiaXmlReader.AlarmGeneration;
using TiaXmlReader.Generation;

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

        private void ExcelPathTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                CheckFileExists = true
            };

            var result = fileDialog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                excelPathTextBox.Text = fileDialog.FileName;
            }
        }

        private void XMLPathTextBlock_MouseClick(object sender, MouseEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml",
                CheckFileExists = false
            };

            var result = fileDialog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                xmlPathTextBlock.Text = fileDialog.FileName;
            }
        }

        private void GenerateAlarmsFCButton_MouseClick(object sender, MouseEventArgs e)
        {
            GlobalIDGenerator.ResetID();

            var fc = new BlockFC();
            fc.Init();
            try
            {
                using (var workbook = new XLWorkbook(excelPathTextBox.Text))
                {
                    var worksheet = workbook.Worksheets.Worksheet(1);

                    var blockName = worksheet.Cell("C4").Value.GetText();
                    var blockNumber = (uint)worksheet.Cell("C5").Value.GetNumber();
                    var startingAlarmNum = (uint)worksheet.Cell("C7").Value.GetNumber();
                    var alarmNumFormat = worksheet.Cell("C8").Value.GetText();
                    var divisionType = worksheet.Cell("C9").Value.GetText();
                    var groupingType = worksheet.Cell("C10").Value.GetText();
                    var skipNumberAfterGroup = (uint)worksheet.Cell("C11").Value.GetNumber();

                    fc.GetBlockAttributes().SetBlockName(blockName);
                    fc.GetBlockAttributes().SetBlockNumber(blockNumber);

                    var generationDataList = new List<AlarmGenerationData>();

                    uint variablesCellIndex = 4;
                    while (true)
                    {
                        var consumerAddressValue = worksheet.Cell("H" + variablesCellIndex).Value;
                        var coil1AddressValue = worksheet.Cell("I" + variablesCellIndex).Value;
                        var coil2AddressValue = worksheet.Cell("J" + variablesCellIndex).Value;
                        var descriptionValue = worksheet.Cell("K" + variablesCellIndex).Value;
                        var enableValue = worksheet.Cell("L" + variablesCellIndex).Value;

                        variablesCellIndex++;

                        if (!consumerAddressValue.IsText || !coil1AddressValue.IsText || !coil2AddressValue.IsText || !descriptionValue.IsText || !enableValue.IsText)
                        {
                            break;
                        }

                        var generationData = new AlarmGenerationData(consumerAddressValue.GetText(), coil1AddressValue.GetText(), coil2AddressValue.GetText(), descriptionValue.GetText(), bool.Parse(enableValue.GetText())); ;
                        generationDataList.Add(generationData);
                    }

                    var consumerDataList = new List<ConsumerData>();

                    var consumerCellIndex = 4;
                    while (true)
                    {
                        var consumerNameValue = worksheet.Cell("E" + consumerCellIndex).Value;
                        var dbNameValue = worksheet.Cell("F" + consumerCellIndex).Value;
                        consumerCellIndex++;

                        if (!consumerNameValue.IsText || !dbNameValue.IsText)
                        {
                            break;
                        }

                        consumerDataList.Add(new ConsumerData(consumerNameValue.GetText(), dbNameValue.GetText()));
                    }

                    var nextAlarmNum = startingAlarmNum;

                    CompileUnit compileUnit = null;
                    switch (groupingType)
                    {
                        case "PerUtenza":
                            foreach (var consumerData in consumerDataList)
                            {
                                if (divisionType == "GruppoPerSegmento")
                                {
                                    compileUnit = fc.AddCompileUnit();
                                    compileUnit.Init();
                                }

                                foreach (var generationData in generationDataList)
                                {
                                    if (!generationData.GetEnable())
                                    {
                                        continue;
                                    }

                                    if (divisionType == "UnoPerSegmento")
                                    {
                                        compileUnit = fc.AddCompileUnit();
                                        compileUnit.Init();
                                    }

                                    var placeholders = new GenerationPlaceholders()
                                            .SetConsumerData(consumerData)
                                            .SetAlarmNumFormat(alarmNumFormat);
                                    FillAlarmCompileUnit(compileUnit, placeholders, generationData, ref nextAlarmNum);
                                }

                                nextAlarmNum += (skipNumberAfterGroup - 1); //Decrease by one since this is already the NEXT. If i add without decrementing it will loose one alarm.
                            }
                            break;
                        case "PerTipoAllarme":
                            foreach (var generationData in generationDataList)
                            {
                                if (!generationData.GetEnable())
                                {
                                    continue;
                                }

                                if (divisionType == "GruppoPerSegmento")
                                {
                                    compileUnit = fc.AddCompileUnit();
                                    compileUnit.Init();
                                }

                                foreach (var consumerData in consumerDataList)
                                {

                                    if (divisionType == "UnoPerSegmento")
                                    {
                                        compileUnit = fc.AddCompileUnit();
                                        compileUnit.Init();
                                    }

                                    var placeholders = new GenerationPlaceholders()
                                            .SetConsumerData(consumerData)
                                            .SetAlarmNumFormat(alarmNumFormat);
                                    FillAlarmCompileUnit(compileUnit, placeholders, generationData, ref nextAlarmNum);
                                }

                                nextAlarmNum += (skipNumberAfterGroup - 1); //Decrease by one since this is already the NEXT. If i add without decrementing it will loose one alarm.
                            }
                            break;
                    }
                }

                var xmlDocument = SiemensMLParser.CreateDocument();
                xmlDocument.DocumentElement.AppendChild(fc.Generate(xmlDocument));
                if (!string.IsNullOrEmpty(xmlPathTextBlock.Text))
                {
                    xmlDocument.Save(xmlPathTextBlock.Text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.ToString());
            }
        }

        private void FillAlarmCompileUnit(CompileUnit compileUnit, GenerationPlaceholders placeholders, AlarmGenerationData alarmGenerationData, ref uint nextAlarmNum)
        {
            placeholders.SetAlarmNum(nextAlarmNum); //PLACEHOLDERS ALWAYS FIRST!.
            nextAlarmNum++;

            compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, alarmGenerationData.GetDescription(placeholders.Parse));

            var contact = compileUnit.AddPart(Part.Type.CONTACT);
            var coil1 = compileUnit.AddPart(Part.Type.COIL);
            var coil2 = compileUnit.AddPart(Part.Type.COIL);

            compileUnit.AddPowerrail(new Dictionary<Part, string> {
                    { contact, "in" }
            });

            compileUnit.AddIdentWire(Access.Type.GLOBAL_VARIABLE, alarmGenerationData.GetConsumerAddress(placeholders.Parse), contact, "operand");
            compileUnit.AddIdentWire(Access.Type.GLOBAL_VARIABLE, alarmGenerationData.GetCoil1Address(placeholders.Parse), coil1, "operand");
            compileUnit.AddIdentWire(Access.Type.GLOBAL_VARIABLE, alarmGenerationData.GetCoil2Address(placeholders.Parse), coil2, "operand");

            compileUnit.AddBoolANDWire(contact, "out", coil1, "in");
            compileUnit.AddBoolANDWire(coil1, "out", coil2, "in");
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