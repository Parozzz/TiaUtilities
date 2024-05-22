using TiaXmlReader.Generation.Placeholders;
using TiaXmlReader.Languages;
using SimaticML.TagTable;
using SimaticML.Blocks;
using SimaticML.Enums;
using SimaticML.Blocks.FlagNet.nPart;
using SimaticML.Blocks.FlagNet.nAccess;
using SimaticML;
using SimaticML.nBlockAttributeList;

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

            this.ioTagTableList = [];
            this.variableTagTableList = [];
        }

        public void GenerateBlocks()
        {
            fc = new BlockFC();
            fc.Init();
            fc.AttributeList.BlockName = config.FCBlockName;
            fc.AttributeList.BlockNumber = config.FCBlockNumber;
            fc.AttributeList.AutoNumber = (config.FCBlockNumber > 0);

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

                var placeholderHandler = new GenerationPlaceholderHandler().SetIOData(ioData, this.config);
                ioData.ParsePlaceholders(placeholderHandler);

                if (ioTagTable == null || (config.IOTableSplitEvery > 0 && tagCounter % config.IOTableSplitEvery == 0))
                {
                    ioTagTable = new XMLTagTable();
                    ioTagTable.TableName = config.IOTableName + "_" + tagCounter;
                    ioTagTableList.Add(ioTagTable);
                }
                tagCounter++;

                //Set it with default tag name. In case it has a specific one, it will be overwritten below.
                var ioTag = ioTagTable.AddTag()
                    .SetBoolean(ioData.GetAddressMemoryArea(), ioData.GetAddressByte(), ioData.GetAddressBit());
                ioTag.TagName = FixDuplicateAddress(ioData.IOName, ioAddressDict);
                ioTag.Comment[LocalizationVariables.CULTURE] = ioData.Comment;

                string? inOutAddress = null;
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
                                variableTagTable.TableName = config.VariableTableName + "_" + merkerCounter;
                                variableTagTableList.Add(variableTagTable);
                            }
                            merkerCounter++;

                            var merkerVariableAddress = FixDuplicateAddress(ioData.Variable, duplicatedAddressDict);

                            var merkerVariableTag = SimaticTagAddress.FromAddress(ioData.MerkerAddress);
                            if(merkerVariableAddress == null)
                            {
                                throw new Exception("Cannot parse Merker VariableAddress for " + ioData.MerkerAddress);
                            }

                            var tag = variableTagTable.AddTag();
                            tag.TagName = merkerVariableAddress;
                            tag.Comment[LocalizationVariables.CULTURE] = ioData.Comment;
                            tag.SetBoolean(SimaticMemoryArea.MERKER, merkerVariableTag.ByteOffset, merkerVariableTag.BitOffset);

                            inOutAddress = merkerVariableAddress;
                            break;
                        case IOMemoryTypeEnum.DB:
                            if (db == null)
                            {
                                db = new BlockGlobalDB();
                                db.Init();
                                db.AttributeList.BlockName = config.DBName;
                                db.AttributeList.BlockNumber = config.DBNumber;
                                db.AttributeList.AutoNumber = (config.DBNumber > 0);
                            }

                            var dbMemberAddress = FixDuplicateAddress(ioData.Variable, duplicatedAddressDict);

                            var member = db.AttributeList.STATIC.AddMembersFromAddress(dbMemberAddress, SimaticDataType.BOOLEAN) ?? throw new InvalidDataException();
                            member.Comment.SetText(LocalizationVariables.CULTURE, ioData.Comment);

                            inOutAddress = member.GetCompleteSymbol();

                            break;
                    }
                }

                if (config.GroupingType == IOGroupingTypeEnum.PER_BIT)
                {
                    compileUnit = fc.AddCompileUnit();
                    compileUnit.Init();
                    compileUnit.Title[LocalizationVariables.CULTURE] = placeholderHandler.Parse(config.SegmentNameBitGrouping);
                }
                else if (config.GroupingType == IOGroupingTypeEnum.PER_BYTE && (ioData.GetAddressByte() != lastByteAddress || ioData.GetAddressMemoryArea() != lastMemoryArea || compileUnit == null))
                {
                    lastByteAddress = (int)ioData.GetAddressByte();
                    lastMemoryArea = ioData.GetAddressMemoryArea();

                    compileUnit = fc.AddCompileUnit();
                    compileUnit.Init();
                    compileUnit.Title[LocalizationVariables.CULTURE] = placeholderHandler.Parse(config.SegmentNameByteGrouping);
                }


                if(inOutAddress != null) 
                {
                    switch (ioData.GetAddressMemoryArea())
                    {
                        case SimaticMemoryArea.INPUT:
                            FillInOutCompileUnit(compileUnit, ioTag.TagName, inOutAddress);
                            break;
                        case SimaticMemoryArea.OUTPUT:
                            FillInOutCompileUnit(compileUnit, inOutAddress, ioTag.TagName);
                            break;
                    }
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

        private void FillInOutCompileUnit(CompileUnit compileUnit, string contactAddress, string coilAddress)
        {
            var contact = new ContactPartData(compileUnit);
            var coil = new CoilPartData(compileUnit);

            contact.CreateIdentWire(compileUnit.AccessFactory.AddGlobalVariable(contactAddress));
            coil.CreateIdentWire(compileUnit.AccessFactory.AddGlobalVariable(coilAddress));

            var _ = compileUnit.Powerrail & contact & coil;
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

            var xmlDocument = SimaticMLAPI.CreateDocument(fc);
            xmlDocument.Save(exportPath + "/fcExport_" + fc.AttributeList.BlockName + ".xml");

            foreach(var ioTagTable in ioTagTableList)
            {
                ioTagTable.UpdateID_UId(new IDGenerator());

                xmlDocument = SimaticMLAPI.CreateDocument(ioTagTable);
                xmlDocument.Save(exportPath + "/ioTagTable_" + ioTagTable.TableName + ".xml");
            }

            foreach(var variableTagTable in variableTagTableList)
            {
                variableTagTable.UpdateID_UId(new IDGenerator());

                xmlDocument = SimaticMLAPI.CreateDocument(variableTagTable);
                xmlDocument.Save(exportPath + "/tagTableExport_" + variableTagTable.TableName + ".xml");
            }

            if (db != null)
            {
                db.UpdateID_UId(new IDGenerator());

                xmlDocument = SimaticMLAPI.CreateDocument(db);
                xmlDocument.Save(exportPath + "/dbExport_" + db.AttributeList.BlockName + ".xml");
            }
        }
    }
}
