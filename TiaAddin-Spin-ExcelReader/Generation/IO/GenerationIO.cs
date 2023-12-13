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

namespace TiaXmlReader.Generation.IO
{
    internal class GenerationIO : IGeneration
    {
        private string fcBlockName;
        private uint fbBlockNumber;

        private string dbName;
        private uint dbNumber;

        private string variableTableName;
        private int variableTableStartAddress;

        private string memoryType;
        private string groupingType;
        private string defaultIoTagName;
        private string defaultVariableName;
        private string segmentNameBitGrouping;
        private string segmentNameByteGrouping;

        private readonly List<IOData> ioDataList;

        private BlockFC fc;
        private CompileUnit compileUnit;
        private BlockGlobalDB db;
        private XMLTagTable ioTagTable;
        private XMLTagTable supportsTagTable;

        public GenerationIO()
        {
            ioDataList = new List<IOData>();
        }

        public void ImportExcelConfig(IXLWorksheet worksheet)
        {
            ioDataList.Clear();

            uint cadIndex = 5;
            while (true)
            {
                var address = worksheet.Cell("E" + cadIndex).Value;
                var ioTagName = worksheet.Cell("F" + cadIndex).Value;
                var ioTagComment = worksheet.Cell("G" + cadIndex).Value;
                var variableAddress = worksheet.Cell("H" + cadIndex).Value;
                var variableComment = worksheet.Cell("I" + cadIndex).Value;
                cadIndex++;

                if (!address.IsText && !ioTagName.IsText && !ioTagComment.IsText && !variableAddress.IsText && !variableComment.IsText)
                {
                    break;
                }

                if (address.IsText && !string.IsNullOrEmpty(address.GetText())) //If there is no address, i don't care about other data.
                {
                    ioDataList.Add(new IOData()
                    {
                        Address = address.GetText(),
                        IOTagName = ioTagName.GetText(),
                        IOTagComment = ioTagComment.GetText(),
                        VariableAddress = variableAddress.GetText(),
                        VariableComment = variableComment.GetText()
                    });
                }
            }

            fcBlockName = worksheet.Cell("C5").Value.GetText();
            fbBlockNumber = (uint)worksheet.Cell("C6").Value.GetNumber();

            dbName = worksheet.Cell("C9").Value.GetText();
            dbNumber = (uint)worksheet.Cell("C10").Value.GetNumber();

            variableTableName = worksheet.Cell("C13").Value.GetText();
            variableTableStartAddress = (int)worksheet.Cell("C14").Value.GetNumber();

            memoryType = worksheet.Cell("C16").Value.GetText();
            groupingType = worksheet.Cell("C17").Value.GetText();

            defaultIoTagName = worksheet.Cell("C19").Value.GetText();
            defaultVariableName = worksheet.Cell("C20").Value.GetText();
            segmentNameBitGrouping = worksheet.Cell("C21").Value.GetText();
            segmentNameByteGrouping = worksheet.Cell("C22").Value.GetText();
        }

        public void GenerateBlocks()
        {
            fc = new BlockFC();
            fc.Init();
            fc.GetBlockAttributes().SetBlockName(fcBlockName).SetBlockNumber(fbBlockNumber).SetAutoNumber(fbBlockNumber > 0);

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
            var lastMemoryArea = SimaticMemoryArea.INPUT;
            var merkerByte = variableTableStartAddress;
            var merkerBit = 0;
            //Order list by ADRESS TYPE - BYTE - BIT 
            foreach (var ioData in ioDataList.OrderBy(x => ((int)x.GetMemoryArea()) * 10000 + x.GetAddressByte() * 100 + x.GetAddressBit()).ToList())
            {
                var placeholders = new GenerationPlaceholders()
                    .SetIOData(ioData);

                //Set it with default tag name. In case it has a specific one, it will be overwritten below.
                var ioTag = ioTagTable.AddTag()
                    .SetDataTypeName("Bool")
                    .SetTagName(placeholders.Parse(defaultIoTagName))
                    .SetLogicalAddress("%" + ioData.GetMemoryArea().GetTIAMnemonic() + ioData.GetAddressByte() + "." + ioData.GetAddressBit());

                if (!string.IsNullOrEmpty(ioData.IOTagName))
                {
                    ioTag.SetTagName(placeholders.Parse(ioData.IOTagName));
                }
                if (!string.IsNullOrEmpty(ioData.IOTagComment))
                {
                    ioTag.SetCommentText(Constants.DEFAULT_CULTURE, placeholders.Parse(ioData.IOTagComment));
                }

                string inOutAddress = null;
                switch (memoryType)
                {
                    case "Merker":
                        var variableTag = supportsTagTable.AddTag()
                                        .SetTagName(placeholders.Parse(defaultVariableName))
                                        .SetBoolean(SimaticMemoryArea.MERKER, merkerByte, merkerBit);
                        if (!string.IsNullOrEmpty(ioData.VariableAddress))
                        {
                            variableTag.SetTagName(placeholders.Parse(ioData.VariableAddress));
                        }
                        if (!string.IsNullOrEmpty(ioData.VariableComment))
                        {
                            variableTag.SetCommentText(Constants.DEFAULT_CULTURE, placeholders.Parse(ioData.VariableComment));
                        }
                        inOutAddress = variableTag.GetTagName();

                        merkerBit++;
                        if (merkerBit >= 8)
                        {
                            merkerByte++;
                            merkerBit = 0;
                        }
                        break;
                    case "GlobalDB":
                        Member variableMember;
                        if(string.IsNullOrEmpty(ioData.VariableAddress))
                        {
                            variableMember = db.GetAttributes().ComputeSection(SectionTypeEnum.STATIC)
                                .AddMember(placeholders.Parse(defaultVariableName), SimaticDataType.BOOLEAN);
                        }
                        else
                        {
                            variableMember = db.GetAttributes().ComputeSection(SectionTypeEnum.STATIC)
                                .AddMembersFromAddress(ioData.VariableAddress, SimaticDataType.BOOLEAN);
                        }

                        if (!string.IsNullOrEmpty(ioData.VariableComment))
                        {
                            variableMember.SetComment(Constants.DEFAULT_CULTURE, placeholders.Parse(ioData.VariableComment));
                        }
                        inOutAddress = variableMember.GetCompleteSymbol();
                        break;
                }

                if (groupingType == "BitPerSegmento")
                {
                    compileUnit = fc.AddCompileUnit();
                    compileUnit.Init();
                    compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(segmentNameBitGrouping));
                }
                else if (groupingType == "BytePerSegmento" && (ioData.GetAddressByte() != lastByteAddress || ioData.GetMemoryArea() != lastMemoryArea || compileUnit == null))
                {
                    lastByteAddress = (int)ioData.GetAddressByte();
                    lastMemoryArea = ioData.GetMemoryArea();

                    compileUnit = fc.AddCompileUnit();
                    compileUnit.Init();
                    compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(segmentNameByteGrouping));
                }

                switch(ioData.GetMemoryArea())
                {
                    case SimaticMemoryArea.INPUT:
                        FillInOutCompileUnit(compileUnit, ioTag.GetTagName(), inOutAddress);
                        break;
                    case SimaticMemoryArea.OUTPUT:
                        FillInOutCompileUnit(compileUnit, inOutAddress, ioTag.GetTagName());
                        break;
                }
            }
        }

        private void FillInOutCompileUnit(CompileUnit compileUnit, string ioTagAddress, string outputAddress)
        {
            var contact = new ContactPartData(compileUnit);
            var coil = new CoilPartData(compileUnit);

            contact.CreateIdentWire(GlobalVariableAccessData.Create(compileUnit, ioTagAddress));
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
