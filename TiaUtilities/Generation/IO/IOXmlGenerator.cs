using System;
using System.Collections.Generic;
using System.Linq;
using TiaXmlReader.SimaticML;
using TiaXmlReader.SimaticML.nBlockAttributeList;
using TiaXmlReader.Utility;
using System.Collections;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.SimaticML.Blocks;
using TiaXmlReader.SimaticML.Blocks.FlagNet.nAccess;
using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.SimaticML.TagTable;
using TiaXmlReader.SimaticML.Blocks.FlagNet.nPart;

namespace TiaXmlReader.Generation.IO
{
    internal class IOXmlGenerator
    {
        private readonly IOConfiguration config;
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
            this.config = configuration;
            this.ioDataList = ioDataList;

            this.ioTagTableList = new List<XMLTagTable>();
            this.variableTagTableList = new List<XMLTagTable>();
        }

        public void GenerateBlocks()
        {
            fc = new BlockFC();
            fc.Init();
            fc.GetBlockAttributes().SetBlockName(config.FCBlockName).SetBlockNumber(config.FCBlockNumber).SetAutoNumber(config.FCBlockNumber > 0);

            uint tagCounter = 0;
            uint merkerCounter = 0;

            var lastByteAddress = -1;
            var lastMemoryArea = SimaticMemoryArea.INPUT;

            var ioAddressDict = new Dictionary<string, uint>();
            var duplicatedAddressDict = new Dictionary<string, uint>();

            //Order list by ADRESS TYPE - BYTE - BIT 
            foreach (var ioData in ioDataList.OrderBy(x => ((int)x.GetAddressMemoryArea()) * Math.Pow(10, 9) + x.GetAddressByte() * Math.Pow(10, 3) + x.GetAddressBit()).ToList())
            {
                //If name of variable is \ i will ignore everything and skip to the next
                if (ioData.Variable == "\\" || ioData.IOName == "\\")
                {
                    continue;
                }

                ioData.LoadDefaults(config, out bool ioNameDefault, out bool variableDefault, out bool merkerAddressDefault);

                var placeholders = new GenerationPlaceholders().SetIOData(ioData);
                ioData.ParsePlaceholders(placeholders);

                if (ioTagTable == null || (config.IOTableSplitEvery > 0 && tagCounter % config.IOTableSplitEvery == 0))
                {
                    ioTagTable = new XMLTagTable();
                    ioTagTable.SetTagTableName(config.IOTableName + "_" + tagCounter);
                    ioTagTableList.Add(ioTagTable);
                }
                tagCounter++;

                //Set it with default tag name. In case it has a specific one, it will be overwritten below.
                var ioTag = ioTagTable.AddTag()
                    .SetBoolean(ioData.GetAddressMemoryArea(), ioData.GetAddressByte(), ioData.GetAddressBit())
                    .SetTagName(FixDuplicateAddress(ioData.IOName, ioAddressDict))
                    .SetCommentText(LocalizationVariables.CULTURE, ioData.Comment);

                string inOutAddress = null;
                if (!variableDefault)
                {
                    inOutAddress = $"{ioData.Variable}";
                }
                else
                {
                    switch (config.MemoryType)
                    {
                        case IOMemoryTypeEnum.MERKER:
                            if (variableTagTable == null || (config.VariableTableSplitEvery > 0 && merkerCounter % config.VariableTableSplitEvery == 0))
                            {
                                variableTagTable = new XMLTagTable();
                                variableTagTable.SetTagTableName(config.VariableTableName + "_" + merkerCounter);
                                variableTagTableList.Add(variableTagTable);
                            }
                            merkerCounter++;

                            var merkerVariablePrefix = ioData.GetAddressMemoryArea() == SimaticMemoryArea.INPUT ? config.PrefixInputMerker : config.PrefixOutputMerker;
                            var merkerVariableAddress = FixDuplicateAddress(merkerVariablePrefix + ioData.Variable, duplicatedAddressDict);

                            var merkerVariableTag = SimaticTagAddress.FromAddress(ioData.MerkerAddress);
                            if(merkerVariableAddress == null)
                            {
                                throw new Exception("Cannot parse Merker VariableAddress for " + ioData.MerkerAddress);
                            }

                            inOutAddress = variableTagTable.AddTag()
                                            .SetTagName(merkerVariableAddress)
                                            .SetBoolean(SimaticMemoryArea.MERKER, merkerVariableTag.ByteOffset, merkerVariableTag.BitOffset)
                                            .SetCommentText(LocalizationVariables.CULTURE, ioData.Comment)
                                            .GetTagName();
                            break;
                        case IOMemoryTypeEnum.DB:
                            if (db == null)
                            {
                                db = new BlockGlobalDB();
                                db.Init();
                                db.GetAttributes().SetBlockName(config.DBName).SetBlockNumber(config.DBNumber).SetAutoNumber(config.DBNumber > 0);
                            }

                            var dbMemberPrefix = ioData.GetAddressMemoryArea() == SimaticMemoryArea.INPUT ? config.PrefixInputDB : config.PrefixOutputDB;
                            var dbMemberAddress = FixDuplicateAddress(dbMemberPrefix + ioData.Variable, duplicatedAddressDict);

                            var section = db.GetAttributes().ComputeSection(SectionTypeEnum.STATIC);
                            inOutAddress = section.AddMembersFromAddress(dbMemberAddress, SimaticDataType.BOOLEAN)
                                    .SetCommentText(LocalizationVariables.CULTURE, ioData.Comment)
                                    .GetCompleteSymbol();
                            break;
                    }
                }

                if (config.GroupingType == IOGroupingTypeEnum.PER_BIT)
                {
                    compileUnit = fc.AddCompileUnit();
                    compileUnit.Init();
                    compileUnit.ComputeBlockTitle().SetText(LocalizationVariables.CULTURE, placeholders.Parse(config.SegmentNameBitGrouping));
                }
                else if (config.GroupingType == IOGroupingTypeEnum.PER_BYTE && (ioData.GetAddressByte() != lastByteAddress || ioData.GetAddressMemoryArea() != lastMemoryArea || compileUnit == null))
                {
                    lastByteAddress = (int)ioData.GetAddressByte();
                    lastMemoryArea = ioData.GetAddressMemoryArea();

                    compileUnit = fc.AddCompileUnit();
                    compileUnit.Init();
                    compileUnit.ComputeBlockTitle().SetText(LocalizationVariables.CULTURE, placeholders.Parse(config.SegmentNameByteGrouping));
                }

                switch (ioData.GetAddressMemoryArea())
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

            fc.UpdateID_UId(new IDGenerator());

            var xmlDocument = SimaticMLParser.CreateDocument();
            xmlDocument.DocumentElement.AppendChild(fc.Generate(xmlDocument));
            xmlDocument.Save(exportPath + "/fcExport_" + fc.GetBlockAttributes().GetBlockName() + ".xml");

            foreach(var ioTagTable in ioTagTableList)
            {
                ioTagTable.UpdateID_UId(new IDGenerator());

                xmlDocument = SimaticMLParser.CreateDocument();
                xmlDocument.DocumentElement.AppendChild(ioTagTable.Generate(xmlDocument));
                xmlDocument.Save(exportPath + "/ioTagTable_" + ioTagTable.GetTagTableName() + ".xml");
            }

            foreach(var variableTagTable in variableTagTableList)
            {
                variableTagTable.UpdateID_UId(new IDGenerator());

                xmlDocument = SimaticMLParser.CreateDocument();
                xmlDocument.DocumentElement.AppendChild(variableTagTable.Generate(xmlDocument));
                xmlDocument.Save(exportPath + "/tagTableExport_" + variableTagTable.GetTagTableName() + ".xml");
            }

            if (db != null)
            {
                db.UpdateID_UId(new IDGenerator());

                xmlDocument = SimaticMLParser.CreateDocument();
                xmlDocument.DocumentElement.AppendChild(db.Generate(xmlDocument));
                xmlDocument.Save(exportPath + "/dbExport_" + db.GetAttributes().GetBlockName() + ".xml");
            }
        }
    }
}
