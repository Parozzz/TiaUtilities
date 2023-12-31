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
using TiaXmlReader.Generation.IO_Cad;

namespace TiaXmlReader.Generation.IO
{
    internal class GenerationIO : IGeneration
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

        private string defaultIoName;
        private string defaultVariableName;

        private string segmentNameBitGrouping;
        private string segmentNameByteGrouping;

        private string prefixInputDB;
        private string prefixInputMerker;
        private string prefixOutputDB;
        private string prefixOutputMerker;

        private readonly List<IOData> ioDataList;

        private BlockFC fc;
        private CompileUnit compileUnit;
        private BlockGlobalDB db;

        private XMLTagTable ioTagTable;
        private readonly List<XMLTagTable> ioTagTableList;

        private XMLTagTable variableTagTable;
        private readonly List<XMLTagTable> variableTagTableList;

        public GenerationIO()
        {
            ioDataList = new List<IOData>();

            this.ioTagTableList = new List<XMLTagTable>();
            this.variableTagTableList = new List<XMLTagTable>();
        }

        public void ImportExcelConfig(IXLWorksheet worksheet)
        {
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

            defaultIoName = worksheet.Cell("C25").Value.ToString();
            defaultVariableName = worksheet.Cell("C26").Value.ToString();

            segmentNameBitGrouping = worksheet.Cell("C29").Value.ToString();
            segmentNameByteGrouping = worksheet.Cell("C30").Value.ToString();

            prefixInputDB = worksheet.Cell("C33").Value.ToString();
            prefixInputMerker = worksheet.Cell("C34").Value.ToString();
            prefixOutputDB = worksheet.Cell("C35").Value.ToString();
            prefixOutputMerker = worksheet.Cell("C36").Value.ToString();

            ioDataList.Clear();

            uint cadIndex = 5;
            while (true)
            {
                var ioAddress = worksheet.Cell("E" + cadIndex).Value;
                var ioName = worksheet.Cell("F" + cadIndex).Value;
                var dbName = worksheet.Cell("G" + cadIndex).Value;
                var variableName = worksheet.Cell("H" + cadIndex).Value;
                var comment = worksheet.Cell("I" + cadIndex).Value;
                cadIndex++;

                if (!ioAddress.IsText || string.IsNullOrEmpty(ioAddress.ToString()))
                {
                    break;
                }

                ioDataList.Add(new IOData()
                {
                    IOAddress = ioAddress.ToString(),
                    IOName = ioName.ToString(),
                    DBName = dbName.ToString(),
                    VariableName = variableName.ToString(),
                    Comment = comment.ToString(),
                });
            }
        }

        public void GenerateBlocks()
        {
            fc = new BlockFC();
            fc.Init();
            fc.GetBlockAttributes().SetBlockName(fcBlockName).SetBlockNumber(fcBlockNumber).SetAutoNumber(fcBlockNumber > 0);

            uint tagCounter = 0;
            uint variableTagCounter = 0;

            var lastByteAddress = -1;
            var lastMemoryArea = SimaticMemoryArea.INPUT;

            uint merkerByte = variableTableStartAddress;
            uint merkerBit = 0;

            var ioAddressDict = new Dictionary<string, uint>();
            var variableAddressDict = new Dictionary<string, uint>();

            //Order list by ADRESS TYPE - BYTE - BIT 
            foreach (var ioData in ioDataList.OrderBy(x => ((int)x.GetMemoryArea()) * Math.Pow(10, 9) + x.GetAddressByte() * Math.Pow(10, 3) + x.GetAddressBit()).ToList())
            {
                //If name of variable is \ i will ignore everything and skip to the next
                if (ioData.VariableName == "\\" || ioData.IOName == "\\")
                {
                    continue;
                }

                ioData.IOName = string.IsNullOrEmpty(ioData.IOName) ? defaultIoName : ioData.IOName;
                ioData.VariableName = string.IsNullOrEmpty(ioData.VariableName) ? defaultVariableName : ioData.VariableName;

                var placeholders = new GenerationPlaceholders().SetIOData(ioData);
                ioData.ParsePlaceholders(placeholders);

                if (ioTagTable == null || (ioTableSplitEvery > 0 && tagCounter % ioTableSplitEvery == 0))
                {
                    ioTagTable = new XMLTagTable();
                    ioTagTable.SetTagTableName(ioTableName + "_" + tagCounter);
                    ioTagTableList.Add(ioTagTable);
                }
                tagCounter++;

                //Set it with default tag name. In case it has a specific one, it will be overwritten below.
                var ioTag = ioTagTable.AddTag()
                    .SetBoolean(ioData.GetMemoryArea(), ioData.GetAddressByte(), ioData.GetAddressBit())
                    .SetTagName(FixDuplicateAddress(ioData.IOName, ioAddressDict))
                    .SetCommentText(Constants.DEFAULT_CULTURE, ioData.Comment);

                string inOutAddress = null;
                if (!string.IsNullOrEmpty(ioData.DBName) && !string.IsNullOrEmpty(ioData.VariableName))
                {
                    inOutAddress = $"{SimaticMLUtil.WrapAddressComponentIfRequired(ioData.DBName)}.{ioData.VariableName}";
                }
                else
                {
                    switch (memoryType)
                    {
                        case "Merker":
                            if (variableTagTable == null || (variableTableSplitEvery > 0 && variableTagCounter % variableTableSplitEvery == 0))
                            {
                                variableTagTable = new XMLTagTable();
                                variableTagTable.SetTagTableName(variableTableName + "_" + variableTagCounter);
                                variableTagTableList.Add(variableTagTable);
                            }
                            variableTagCounter++;

                            var tagAddressPrefix = ioData.GetMemoryArea() == SimaticMemoryArea.INPUT ? prefixInputMerker : prefixOutputMerker;
                            var fullTagAddress = FixDuplicateAddress(tagAddressPrefix + ioData.VariableName, variableAddressDict);
                            inOutAddress = variableTagTable.AddTag()
                                            .SetTagName(fullTagAddress)
                                            .SetBoolean(SimaticMemoryArea.MERKER, merkerByte, merkerBit)
                                            .SetCommentText(Constants.DEFAULT_CULTURE, ioData.Comment)
                                            .GetTagName();
                            if (merkerBit++ >= 8)
                            {
                                merkerByte++;
                                merkerBit = 0;
                            }
                            break;
                        case "DB":
                            if (db == null)
                            {
                                db = new BlockGlobalDB();
                                db.Init();
                                db.GetAttributes().SetBlockName(dbName).SetBlockNumber(dbNumber).SetAutoNumber(dbNumber > 0);
                            }

                            var memberAddressPrefix = ioData.GetMemoryArea() == SimaticMemoryArea.INPUT ? prefixInputDB : prefixOutputDB;
                            var fullMemberAddress = FixDuplicateAddress(memberAddressPrefix + ioData.VariableName, variableAddressDict);
                            inOutAddress = db.GetAttributes().ComputeSection(SectionTypeEnum.STATIC)
                                    .AddMembersFromAddress(fullMemberAddress, SimaticDataType.BOOLEAN)
                                    .SetComment(Constants.DEFAULT_CULTURE, ioData.Comment)
                                    .GetCompleteSymbol();
                            break;
                    }
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

                switch (ioData.GetMemoryArea())
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

        private string FixDuplicateAddress(string address, Dictionary<string, uint> dict)
        {
            if (!dict.TryGetValue(address, out uint count))
            {
                dict.Add(address, 0);
            }

            dict[address] = ++count;
            return address + (count <= 1 ? "" : $"({count})");
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

            foreach(var ioTagTable in ioTagTableList)
            {
                xmlDocument = SimaticMLParser.CreateDocument();
                xmlDocument.DocumentElement.AppendChild(ioTagTable.Generate(xmlDocument, new IDGenerator()));
                xmlDocument.Save(exportPath + "/ioTagTable_" + ioTagTable.GetTagTableName() + ".xml");
            }

            foreach(var variableTagTable in variableTagTableList)
            {
                xmlDocument = SimaticMLParser.CreateDocument();
                xmlDocument.DocumentElement.AppendChild(variableTagTable.Generate(xmlDocument, new IDGenerator()));
                xmlDocument.Save(exportPath + "/tagTableExport_" + variableTagTable.GetTagTableName() + ".xml");
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
