using ClosedXML.Excel;
using SpinXmlReader.Block;
using SpinXmlReader.TagTable;
using SpinXmlReader;
using System;
using System.Collections.Generic;
using System.Linq;
using TiaXmlReader.SimaticML;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.PartNamespace;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.AccessNamespace;
using static SpinXmlReader.Block.Section;
using SpinXmlReader.SimaticML;

namespace TiaXmlReader.Generation.IO_Cad
{
    internal class GenerationIO_CAD : IGeneration
    {
        private string blockName;
        private uint blockNumber;

        private string dbName;
        private uint dbNumber;

        private string variableTableName;
        private uint variableTableStartAddress;

        private string memoryType;
        private string groupingType;
        private string ioTagName;
        private string ioTagComment;
        private string variableName;
        private string variableComment;
        private string segmentNameBitGrouping;
        private string segmentNameByteGrouping;
        private string structGrouping;

        private readonly List<CadData> cadDataList;

        private BlockFC fc;
        private CompileUnit compileUnit;
        private BlockGlobalDB db;
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
            variableTableStartAddress = (uint) worksheet.Cell("C14").Value.GetNumber();

            memoryType = worksheet.Cell("C16").Value.GetText();
            groupingType = worksheet.Cell("C17").Value.GetText();

            ioTagName = worksheet.Cell("C19").Value.GetText();
            ioTagComment = worksheet.Cell("C20").Value.GetText();
            variableName = worksheet.Cell("C21").Value.GetText();
            variableComment = worksheet.Cell("C22").Value.GetText();
            segmentNameBitGrouping = worksheet.Cell("C23").Value.GetText();
            segmentNameByteGrouping = worksheet.Cell("C24").Value.GetText();
            structGrouping = worksheet.Cell("C25").Value.GetText();
        }

        public void GenerateBlocks()
        {
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
                    db = new BlockGlobalDB();
                    db.Init();
                    db.GetAttributes().SetBlockName(dbName).SetBlockNumber(dbNumber).SetAutoNumber(dbNumber > 0);
                    break;
            }

            var lastByteAddress = -1;
            var lastMemoryType = CadDataSiemensMemoryType.UNDEFINED;

            uint merkerByte = variableTableStartAddress;
            uint merkerBit = 0;

            //Order list by ADRESS TYPE - BYTE - BIT 
            foreach (var cadData in cadDataList.OrderBy(x => ((int)x.GetAddressType()) * 10000 + x.GetAddressByte() * 100 + x.GetAddressBit()).ToList())
            {
                var placeholders = new GenerationPlaceholders()
                    .SetCadData(cadData);

                var ioTag = ioTagTable.AddTag()
                    .SetDataType(SimaticDataType.BOOLEAN)
                    .SetTagName(placeholders.Parse(ioTagName))
                    .SetLogicalAddress(cadData.GetAddressType().GetSimatic(), cadData.GetAddressByte(), cadData.GetAddressBit())
                    .SetCommentText(Constants.DEFAULT_CULTURE, placeholders.Parse(ioTagComment));

                string outputAddress = null;
                switch (memoryType)
                {
                    case "Merker":
                        var tag = supportsTagTable.AddTag()
                                        .SetTagName(placeholders.Parse(variableName))
                                        .SetCommentText(Constants.DEFAULT_CULTURE, placeholders.Parse(variableComment))
                                        .SetBoolean(SimaticMemoryArea.MERKER, merkerByte, merkerBit);
                        outputAddress = tag.GetTagName();

                        merkerBit++;
                        if (merkerBit >= 8)
                        {
                            merkerByte++;
                            merkerBit = 0;
                        }
                        break;
                    case "GlobalDB":
                        string memberAddress = placeholders.Parse(variableName);
                        switch (structGrouping)
                        {
                            case "Comment1":
                                if (!string.IsNullOrEmpty(cadData.Comment1))
                                {
                                    memberAddress = $"{SimaticMLUtil.WrapAddressComponent(cadData.Comment1)}.{placeholders.Parse(variableName)}";
                                }
                                break;
                            case "Comment2":
                                if (!string.IsNullOrEmpty(cadData.Comment2))
                                {
                                    memberAddress = $"{SimaticMLUtil.WrapAddressComponent(cadData.Comment2)}.{placeholders.Parse(variableName)}";
                                }
                                break;
                            case "Comment3":
                                if (!string.IsNullOrEmpty(cadData.Comment3))
                                {
                                    memberAddress = $"{SimaticMLUtil.WrapAddressComponent(cadData.Comment3)}.{placeholders.Parse(variableName)}";
                                }
                                break;
                            case "Comment4":
                                if (!string.IsNullOrEmpty(cadData.Comment4))
                                {
                                    memberAddress = $"{SimaticMLUtil.WrapAddressComponent(cadData.Comment4)}.{placeholders.Parse(variableName)}";
                                }
                                break;
                        }

                        var member = db.GetAttributes().ComputeSection(SectionTypeEnum.STATIC)
                            .AddMembersFromAddress(memberAddress, SimaticDataType.BOOLEAN)
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
            var contact = new ContactPartData(compileUnit);
            var coil = new CoilPartData(compileUnit);

            contact.CreateIdentWire(GlobalVariableAccessData.Create(compileUnit, inputAddress));
            coil.CreateIdentWire(GlobalVariableAccessData.Create(compileUnit, outputAddress));

            contact.CreatePowerrailConnection()
                .CreateOutputConnection(coil);
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

            var xmlDocument = SimaticMLParser.CreateDocument();
            xmlDocument.DocumentElement.AppendChild(fc.Generate(xmlDocument, new IDGenerator()));
            xmlDocument.Save(exportPath + "/fcExport_" + fc.GetBlockAttributes().GetBlockName() + ".xml");

            xmlDocument = SimaticMLParser.CreateDocument();
            xmlDocument.DocumentElement.AppendChild(ioTagTable.Generate(xmlDocument, new IDGenerator()));
            xmlDocument.Save(exportPath + "/ioTagTable.xml");

            if (supportsTagTable != null)
            {
                xmlDocument = SimaticMLParser.CreateDocument();
                xmlDocument.DocumentElement.AppendChild(supportsTagTable.Generate(xmlDocument, new IDGenerator()));
                xmlDocument.Save(exportPath + "/tagTableExport_" + supportsTagTable.GetTagTableName() + ".xml");
            }

            if (db != null)
            {
                xmlDocument = SimaticMLParser.CreateDocument();
                xmlDocument.DocumentElement.AppendChild(db.Generate(xmlDocument, new IDGenerator()));
                xmlDocument.Save(exportPath + "/dbExport_" + db.GetAttributes().GetBlockName() + ".xml");
            }
        }
    }
}
