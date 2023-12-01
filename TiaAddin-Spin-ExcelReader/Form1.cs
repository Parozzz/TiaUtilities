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

        private void CadExcelPathTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                CheckFileExists = true
            };

            var result = fileDialog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                cadExcelPathTextBox.Text = fileDialog.FileName;
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
                using (var workbook = new XLWorkbook(configExcelPathTextBox.Text))
                {
                    var worksheet = workbook.Worksheets.Worksheet(1);

                    var blockName = worksheet.Cell("C4").Value.GetText();
                    var blockNumber = (uint)worksheet.Cell("C5").Value.GetNumber();
                    var startingAlarmNum = (uint)worksheet.Cell("C7").Value.GetNumber();
                    var alarmNumFormat = worksheet.Cell("C8").Value.GetText();
                    var divisionType = worksheet.Cell("C9").Value.GetText();
                    var groupingType = worksheet.Cell("C10").Value.GetText();
                    var skipNumberAfterGroup = (uint)worksheet.Cell("C11").Value.GetNumber();

                    var blockAttributes = fc.GetBlockAttributes();
                    blockAttributes.SetBlockName(blockName);
                    if (blockNumber > 0)
                    {
                        blockAttributes.SetBlockNumber(blockNumber);
                        blockAttributes.SetAutoNumber(true);
                    }

                    var alarmDataList = new List<AlarmData>();

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

                        var alarmData = new AlarmData(consumerAddressValue.GetText(), coil1AddressValue.GetText(), coil2AddressValue.GetText(), descriptionValue.GetText(), bool.Parse(enableValue.GetText()));
                        alarmDataList.Add(alarmData);
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

                                foreach (var generationData in alarmDataList)
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

                                nextAlarmNum += skipNumberAfterGroup;
                            }
                            break;
                        case "PerTipoAllarme":
                            foreach (var generationData in alarmDataList)
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


                                nextAlarmNum += skipNumberAfterGroup;
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

        private void FillAlarmCompileUnit(CompileUnit compileUnit, GenerationPlaceholders placeholders, AlarmData alarmGenerationData, ref uint nextAlarmNum)
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

        private void GenerateInOutButton_MouseClick(object sender, MouseEventArgs e)
        {
            var fc = new BlockFC();
            fc.Init();
            try
            {
                using(var cadWorkbook = new XLWorkbook(cadExcelPathTextBox.Text))
                {
                    var cadWorksheet = cadWorkbook.Worksheets.Worksheet(1);

                    var cadDataList = new List<CadData>();

                    uint cadIndex = 2;
                    while (true)
                    {
                        var addressValue = cadWorksheet.Cell("A" + cadIndex).Value;
                        var comment1Value = cadWorksheet.Cell("E" + cadIndex).Value;
                        var comment2Value = cadWorksheet.Cell("F" + cadIndex).Value;
                        var comment3Value = cadWorksheet.Cell("G" + cadIndex).Value;
                        var comment4Value = cadWorksheet.Cell("H" + cadIndex).Value;
                        var cadPageValue = cadWorksheet.Cell("K" + cadIndex).Value;
                        var cadPanelValue = cadWorksheet.Cell("M" + cadIndex).Value;
                        var cadTypeValue = cadWorksheet.Cell("P" + cadIndex).Value;

                        if(!addressValue.IsText && !comment1Value.IsText && !comment2Value.IsText && !comment3Value.IsText && !comment4Value.IsText && !cadPageValue.IsText && !cadPanelValue.IsText && !cadTypeValue.IsText)
                        {
                            break;
                        }

                        cadDataList.Add(new CadData()
                        {
                            Address = addressValue.GetText(),
                            Comment1 = comment1Value.GetText(),
                            Comment2 = comment2Value.GetText(),
                            Comment3 = comment3Value.GetText(),
                            Comment4 = comment4Value.GetText(),
                            CadPage = cadPageValue.GetText(),
                            CadPanel = cadPanelValue.GetText(),
                            CadType = cadTypeValue.GetText()
                        });
                    }

                    using (var configWorkbook = new XLWorkbook(configExcelPathTextBox.Text))
                    {
                        var configWorksheet = configWorkbook.Worksheets.Worksheet(1);

                        var blockName = configWorksheet.Cell("C5").Value.GetText();
                        var blockNumber = (uint)configWorksheet.Cell("C6").Value.GetNumber();

                        var dbName = configWorksheet.Cell("C9").Value.GetText();
                        var dbNumber = (uint)configWorksheet.Cell("C10").Value.GetNumber();

                        var variableTableName = configWorksheet.Cell("C13").Value.GetText();

                        var memoryType = configWorksheet.Cell("C15").Value.GetText();
                        var groupingType = configWorksheet.Cell("C16").Value.GetText();

                        var variableName = configWorksheet.Cell("C18").Value.GetText();
                        var variableComment = configWorksheet.Cell("C19").Value.GetText();
                        var segmentName = configWorksheet.Cell("C20").Value.GetText();

                        var inOutData = new InOutData()
                        {
                            VariableName = variableName,
                            VariableComment = variableComment,
                            SegmentName = segmentName
                        };

                        switch(memoryType)
                        {
                            case "Merker":
                                break;
                            case "GlobalDB":
                                var db = new GlobalDB();
                                db.Init();

                                

                                foreach (var cadData in cadDataList)
                                {
                                    var placeholders = new GenerationPlaceholders()
                                        .SetCadData(cadData);

                                    var staticSection = db.GetAttributes().ComputeSection(SectionTypeEnum.STATIC);
                                    staticSection.AddMember(placeholders.Parse(inOutData.VariableName), "bool");

                                }
                                break;
                        }


                    }
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.ToString());
            }
        }

        private void FillInOutCompileUnit(CompileUnit compileUnit, GenerationPlaceholders placeholders, CadData cadData, InOutData inOutData, string inputAddress, string outputAddress)
        {
            compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(inOutData.SegmentName));

            var contact = compileUnit.AddPart(Part.Type.CONTACT);
            var coil = compileUnit.AddPart(Part.Type.COIL);

            compileUnit.AddPowerrail(new Dictionary<Part, string> {
                    { contact, "in" }
            });

            compileUnit.AddIdentWire(Access.Type.GLOBAL_VARIABLE, inputAddress, contact, "operand");
            compileUnit.AddIdentWire(Access.Type.GLOBAL_VARIABLE, outputAddress, coil, "operand");
            compileUnit.AddBoolANDWire(contact, "out", coil, "in");
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