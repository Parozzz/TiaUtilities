using ClosedXML.Excel;
using SpinXmlReader.Block;
using SpinXmlReader.TagTable;
using SpinXmlReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.Generation.Cad
{
    internal class GenerationIO_CAD : IGeneration
    {
        private string blockName;
        private uint blockNumber;

        private string dbName;
        private uint dbNumber;

        private string variableTableName;
        private int variableTableStartAddress;

        private string memoryType;
        private string groupingType;
        private string ioTagName;
        private string ioTagComment;
        private string variableName;
        private string variableComment;
        private string segmentNameBitGrouping;
        private string segmentNameByteGrouping;

        private readonly List<CadData> cadDataList;

        private BlockFC fc;
        private CompileUnit compileUnit;
        private GlobalDB db;
        private XMLTagTable ioTagTable;
        private XMLTagTable supportsTagTable;

        public GenerationIO_CAD()
        {
            cadDataList = new List<CadData>();
        }
        public void ImportExcelConfig(IXLWorksheet worksheet)
        {
            cadDataList.Clear();

            uint cadIndex = 4;
            while (true)
            {
                var addressValue = worksheet.Cell("E" + cadIndex).Value;
                var comment1Value = worksheet.Cell("I" + cadIndex).Value;
                var comment2Value = worksheet.Cell("J" + cadIndex).Value;
                var comment3Value = worksheet.Cell("K" + cadIndex).Value;
                var comment4Value = worksheet.Cell("L" + cadIndex).Value;
                var cadPageValue = worksheet.Cell("O" + cadIndex).Value;
                var cadPanelValue = worksheet.Cell("Q" + cadIndex).Value;
                var cadTypeValue = worksheet.Cell("T" + cadIndex).Value;
                cadIndex++;

                if (!addressValue.IsText && !comment1Value.IsText && !comment2Value.IsText && !comment3Value.IsText && !comment4Value.IsText && !cadPageValue.IsText && !cadPanelValue.IsText && !cadTypeValue.IsText)
                {
                    break;
                }

                if (addressValue.IsText && !string.IsNullOrEmpty(addressValue.GetText())) //If there is no address, i don't care about other data.
                {
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
            }

            blockName = worksheet.Cell("C5").Value.GetText();
            blockNumber = (uint)worksheet.Cell("C6").Value.GetNumber();

            dbName = worksheet.Cell("C9").Value.GetText();
            dbNumber = (uint)worksheet.Cell("C10").Value.GetNumber();

            variableTableName = worksheet.Cell("C13").Value.GetText();
            variableTableStartAddress = (int)worksheet.Cell("C14").Value.GetNumber();

            memoryType = worksheet.Cell("C16").Value.GetText();
            groupingType = worksheet.Cell("C17").Value.GetText();

            ioTagName = worksheet.Cell("C19").Value.GetText();
            ioTagComment = worksheet.Cell("C20").Value.GetText();
            variableName = worksheet.Cell("C21").Value.GetText();
            variableComment = worksheet.Cell("C22").Value.GetText();
            segmentNameBitGrouping = worksheet.Cell("C23").Value.GetText();
            segmentNameByteGrouping = worksheet.Cell("C24").Value.GetText();
        }

        public void GenerateBlocks()
        {
            GlobalIDGenerator.ResetID();

            fc = new BlockFC();
            fc.Init();
            fc.GetBlockAttributes().SetBlockName(blockName).SetBlockNumber(blockNumber).SetAutoNumber(blockNumber > 0);

            ioTagTable = new XMLTagTable();
            ioTagTable.SetTagTableName("IOTags");

            switch (memoryType)
            {
                case "Merker":
                    supportsTagTable = new XMLTagTable();
                    supportsTagTable.SetTagTableName(variableTableName);
                    break;
                case "GlobalDB":
                    db = new GlobalDB();
                    db.Init();
                    db.GetAttributes().SetBlockName(dbName).SetBlockNumber(dbNumber).SetAutoNumber(dbNumber > 0);
                    break;
            }

            var lastByteAddress = -1;
            var lastMemoryType = CadDataSiemensMemoryType.UNDEFINED;
            var merkerByte = variableTableStartAddress;
            var merkerBit = 0;
            //Order list by ADRESS TYPE - BYTE - BIT 
            foreach (var cadData in cadDataList.OrderBy(x => ((int)x.GetAddressType()) * 10000 + x.GetAddressByte() * 100 + x.GetAddressBit()).ToList())
            {
                var placeholders = new GenerationPlaceholders()
                    .SetCadData(cadData);

                var ioTag = ioTagTable.AddTag()
                    .SetDataTypeName("Bool")
                    .SetTagName(placeholders.Parse(ioTagName))
                    .SetLogicalAddress("%" + cadData.GetAddressType().GetInitial() + cadData.GetAddressByte() + "." + cadData.GetAddressBit())
                    .SetCommentText(Constants.DEFAULT_CULTURE, placeholders.Parse(ioTagComment));

                string outputAddress = null;
                switch (memoryType)
                {
                    case "Merker":
                        var tag = supportsTagTable.AddTag().SetTagName(placeholders.Parse(variableName))
                                        .SetDataTypeName("Bool")
                                        .SetLogicalAddress("%M" + merkerByte + "." + merkerBit)
                                        .SetCommentText(Constants.DEFAULT_CULTURE, placeholders.Parse(variableComment));
                        outputAddress = tag.GetTagName();

                        merkerBit++;
                        if (merkerBit >= 8)
                        {
                            merkerByte++;
                            merkerBit = 0;
                        }
                        break;
                    case "GlobalDB":
                        var member = db.GetAttributes().ComputeSection(SectionTypeEnum.STATIC)
                            .AddMember(placeholders.Parse(variableName), "Bool")
                            .SetComment(Constants.DEFAULT_CULTURE, placeholders.Parse(variableComment));
                        outputAddress = member.GetCompleteSymbol();
                        break;
                }

                if (groupingType == "BitPerSegmento")
                {
                    compileUnit = fc.AddCompileUnit();
                    compileUnit.Init();
                    compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(segmentNameBitGrouping));
                }
                else if (groupingType == "BytePerSegmento" && (cadData.GetAddressByte() != lastByteAddress || cadData.GetAddressType() != lastMemoryType || compileUnit == null))
                {
                    lastByteAddress = (int)cadData.GetAddressByte();
                    lastMemoryType = cadData.GetAddressType();

                    compileUnit = fc.AddCompileUnit();
                    compileUnit.Init();
                    compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(segmentNameByteGrouping));
                }

                FillInOutCompileUnit(compileUnit, ioTag.GetTagName(), outputAddress);
            }
        }

        private void FillInOutCompileUnit(CompileUnit compileUnit, string inputAddress, string outputAddress)
        {
            var contact = compileUnit.AddPart(Part.Type.CONTACT);
            var coil = compileUnit.AddPart(Part.Type.COIL);

            compileUnit.AddPowerrailSingleConnection(contact, "in");
            compileUnit.AddIdentWire(Access.Type.GLOBAL_VARIABLE, inputAddress, contact, "operand");
            compileUnit.AddIdentWire(Access.Type.GLOBAL_VARIABLE, outputAddress, coil, "operand");
            compileUnit.AddBoolANDWire(contact, "out", coil, "in");
        }

        public void ExportXML(string exportPath)
        {
            if (string.IsNullOrEmpty(exportPath))
            {
                return;
            }

            if (fc == null || ioTagTable == null)
            {
                throw new ArgumentNullException("Blocks has not been generated");
            }

            var xmlDocument = SiemensMLParser.CreateDocument();
            xmlDocument.DocumentElement.AppendChild(fc.Generate(xmlDocument));
            xmlDocument.Save(exportPath + "/fcExport_" + fc.GetBlockAttributes().GetBlockName() + ".xml");

            xmlDocument = SiemensMLParser.CreateDocument();
            xmlDocument.DocumentElement.AppendChild(ioTagTable.Generate(xmlDocument));
            xmlDocument.Save(exportPath + "/ioTagTable.xml");

            if (supportsTagTable != null)
            {
                xmlDocument = SiemensMLParser.CreateDocument();
                xmlDocument.DocumentElement.AppendChild(supportsTagTable.Generate(xmlDocument));
                xmlDocument.Save(exportPath + "/tagTableExport_" + supportsTagTable.GetTagTableName() + ".xml");
            }

            if (db != null)
            {
                xmlDocument = SiemensMLParser.CreateDocument();
                xmlDocument.DocumentElement.AppendChild(db.Generate(xmlDocument));
                xmlDocument.Save(exportPath + "/dbExport_" + db.GetAttributes().GetBlockName() + ".xml");
            }
        }
    }
}
