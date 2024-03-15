using SpinXmlReader.Block;
using SpinXmlReader.TagTable;
using SpinXmlReader;
using System;
using System.Collections.Generic;
using System.Linq;
using TiaXmlReader.SimaticML;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.PartNamespace;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.AccessNamespace;
using SpinXmlReader.SimaticML;
using TiaXmlReader.SimaticML.nBlockAttributeList;

namespace TiaXmlReader.Generation.IO
{
    internal class IOXmlGenerator
    {
        private IOConfiguration configuration;
        private readonly List<IOData> ioDataList;

        private BlockFC fc;
        private CompileUnit compileUnit;
        private BlockGlobalDB db;

        private XMLTagTable ioTagTable;
        private readonly List<XMLTagTable> ioTagTableList;

        private XMLTagTable variableTagTable;
        private readonly List<XMLTagTable> variableTagTableList;


        public IOXmlGenerator(IOConfiguration configuration, List<IOData> ioDataList)
        {
            this.configuration = configuration;
            this.ioDataList = ioDataList;

            this.ioTagTableList = new List<XMLTagTable>();
            this.variableTagTableList = new List<XMLTagTable>();
        }


        public void GenerateBlocks()
        {
            fc = new BlockFC();
            fc.Init();
            fc.GetBlockAttributes().SetBlockName(configuration.FCBlockName).SetBlockNumber(configuration.FCBlockNumber).SetAutoNumber(configuration.FCBlockNumber > 0);

            uint tagCounter = 0;
            uint variableTagCounter = 0;

            var lastByteAddress = -1;
            var lastMemoryArea = SimaticMemoryArea.INPUT;

            uint merkerByte = configuration.VariableTableStartAddress;
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

                ioData.IOName = string.IsNullOrEmpty(ioData.IOName) ? configuration.DefaultIoName : ioData.IOName;
                ioData.VariableName = string.IsNullOrEmpty(ioData.VariableName) ? configuration.DefaultVariableName : ioData.VariableName;

                var placeholders = new GenerationPlaceholders().SetIOData(ioData);
                ioData.ParsePlaceholders(placeholders);

                if (ioTagTable == null || (configuration.IOTableSplitEvery > 0 && tagCounter % configuration.IOTableSplitEvery == 0))
                {
                    ioTagTable = new XMLTagTable();
                    ioTagTable.SetTagTableName(configuration.IOTableName + "_" + tagCounter);
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
                    switch (configuration.MemoryType)
                    {
                        case "Merker":
                            if (variableTagTable == null || (configuration.VariableTableSplitEvery > 0 && variableTagCounter % configuration.VariableTableSplitEvery == 0))
                            {
                                variableTagTable = new XMLTagTable();
                                variableTagTable.SetTagTableName(configuration.VariableTableName + "_" + variableTagCounter);
                                variableTagTableList.Add(variableTagTable);
                            }
                            variableTagCounter++;

                            var tagAddressPrefix = ioData.GetMemoryArea() == SimaticMemoryArea.INPUT ? configuration.PrefixInputMerker : configuration.PrefixOutputMerker;
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
                                db.GetAttributes().SetBlockName(configuration.DBName).SetBlockNumber(configuration.DBNumber).SetAutoNumber(configuration.DBNumber > 0);
                            }

                            var memberAddressPrefix = ioData.GetMemoryArea() == SimaticMemoryArea.INPUT ? configuration.PrefixInputDB : configuration.PrefixOutputDB;
                            var fullMemberAddress = FixDuplicateAddress(memberAddressPrefix + ioData.VariableName, variableAddressDict);
                            inOutAddress = db.GetAttributes().ComputeSection(SectionTypeEnum.STATIC)
                                    .AddMembersFromAddress(fullMemberAddress, SimaticDataType.BOOLEAN)
                                    .SetCommentText(Constants.DEFAULT_CULTURE, ioData.Comment)
                                    .GetCompleteSymbol();
                            break;
                    }
                }

                if (configuration.GroupingType == "BitPerSegmento")
                {
                    compileUnit = fc.AddCompileUnit();
                    compileUnit.Init();
                    compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(configuration.SegmentNameBitGrouping));
                }
                else if (configuration.GroupingType == "BytePerSegmento" && (ioData.GetAddressByte() != lastByteAddress || ioData.GetMemoryArea() != lastMemoryArea || compileUnit == null))
                {
                    lastByteAddress = (int)ioData.GetAddressByte();
                    lastMemoryArea = ioData.GetMemoryArea();

                    compileUnit = fc.AddCompileUnit();
                    compileUnit.Init();
                    compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(configuration.SegmentNameByteGrouping));
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
