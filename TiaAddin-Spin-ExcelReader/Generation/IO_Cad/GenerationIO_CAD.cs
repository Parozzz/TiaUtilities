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
        private string fcBlockName;
        private uint fcBlockNumber;

        private string memoryType;
        private string groupingType;

        private string dbName;
        private uint dbNumber;

        private string variableTableName;
        private uint variableTableStartAddress;
        private uint variableTableSplitEvery;

        private string ioTableName;
        private uint ioTableSplitEvery;

        private string defaultIOName;
        private string defaultVariableName;
        private string defaultComment;

        private string segmentNameBitGrouping;
        private string segmentNameByteGrouping;

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

            uint cadIndex = 5;
            while (true)
            {
                //Generation data
                var ioName = worksheet.Cell("E" + cadIndex).Value;
                var dbName = worksheet.Cell("F" + cadIndex).Value;
                var variableName = worksheet.Cell("G" + cadIndex).Value;
                //Cad export
                var address = worksheet.Cell("H" + cadIndex).Value;
                var comment1 = worksheet.Cell("L" + cadIndex).Value;
                var comment2 = worksheet.Cell("M" + cadIndex).Value;
                var comment3 = worksheet.Cell("N" + cadIndex).Value;
                var comment4 = worksheet.Cell("O" + cadIndex).Value;
                var mnemonic = worksheet.Cell("P" + cadIndex).Value;
                var wireNum = worksheet.Cell("Q" + cadIndex).Value;
                var page = worksheet.Cell("R" + cadIndex).Value;
                var panel = worksheet.Cell("T" + cadIndex).Value;
                cadIndex++;

                //If none of the fields are text, i will stop going down the table.
                if (!address.IsText && !comment1.IsText && !comment2.IsText && !comment3.IsText && !comment4.IsText && !mnemonic.IsText && !wireNum.IsText && !pageValue.IsText && !panelValue.IsText)
                {
                    break;
                }

                if (address.IsText && !string.IsNullOrEmpty(address.ToString())) //If there is no address, i don't care about other data.
                {
                    cadDataList.Add(new CadData()
                    {
                        IOName = ioName.ToString(),
                        DBName = dbName.ToString(),
                        VariableName = variableName.ToString(),
                        CadAddress = address.ToString(),
                        Comment1 = comment1.ToString(),
                        Comment2 = comment2.ToString(),
                        Comment3 = comment3.ToString(),
                        Comment4 = comment4.ToString(),
                        Mnemonic = mnemonic.ToString(),
                        WireNum = wireNum.ToString(),
                        Page = page.ToString(),
                        Panel = panel.ToString(),
                    });
                }
            }

            fcBlockName = worksheet.Cell("C5").Value.ToString();
            fcBlockNumber = (uint)worksheet.Cell("C6").Value.GetNumber();

            memoryType = worksheet.Cell("C8").Value.ToString();
            groupingType = worksheet.Cell("C9").Value.ToString();

            dbName = worksheet.Cell("C12").Value.ToString();
            dbNumber = (uint)worksheet.Cell("C13").Value.GetNumber();

            variableTableName = worksheet.Cell("C16").Value.ToString();
            variableTableStartAddress = (uint)worksheet.Cell("C17").Value.GetNumber();
            variableTableSplitEvery = (uint)worksheet.Cell("C18").Value.GetNumber();

            ioTableName = worksheet.Cell("C21").Value.ToString();
            ioTableSplitEvery = (uint)worksheet.Cell("C22").Value.GetNumber();

            defaultIOName = worksheet.Cell("C25").Value.ToString();
            defaultVariableName = worksheet.Cell("C26").Value.ToString();
            defaultComment = worksheet.Cell("C27").Value.ToString();

            segmentNameBitGrouping = worksheet.Cell("C30").Value.ToString();
            segmentNameByteGrouping = worksheet.Cell("C31").Value.ToString();
        }

        public void GenerateBlocks()
        {
            fc = new BlockFC();
            fc.Init();
            fc.GetBlockAttributes().SetBlockName(fcBlockName).SetBlockNumber(fcBlockNumber).SetAutoNumber(fcBlockNumber > 0);

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
