using TiaXmlReader.Generation.Placeholders;
using TiaXmlReader.Languages;
using SimaticML.TagTable;
using SimaticML.Blocks;
using SimaticML.Enums;
using SimaticML.Blocks.FlagNet.nPart;
using SimaticML.Blocks.FlagNet.nAccess;
using SimaticML;
using SimaticML.nBlockAttributeList;
using SimaticML.Blocks.FlagNet;
using SimaticML.API;
using System.Net.Mail;
using TiaUtilities.Generation.IO;

namespace TiaXmlReader.Generation.IO
{
    internal class IOXmlGenerator(IOMainConfiguration mainConfig)
    {
        private readonly IOMainConfiguration mainConfig = mainConfig;

        private readonly Dictionary<string, BlockFC> fcDict = [];
        private BlockGlobalDB? db;

        private XMLTagTable? ioTagTable;
        private readonly List<XMLTagTable> ioTagTableList = [];

        private XMLTagTable? variableTagTable;
        private readonly List<XMLTagTable> variableTagTableList = [];

        public void Init()
        {
        }

        public void GenerateAlias(string name, IOTabConfiguration tabConfig, List<IOData> ioDataList)
        {
            var fc = new BlockFC();
            fc.Init();
            fc.AttributeList.BlockName = tabConfig.FCBlockName;
            fc.AttributeList.BlockNumber = tabConfig.FCBlockNumber;
            fc.AttributeList.AutoNumber = (tabConfig.FCBlockNumber > 0);

            uint tagCounter = 0;
            uint merkerCounter = 0;

            var lastByteAddress = -1;
            var lastMemoryArea = SimaticMemoryArea.INPUT;

            var ioAddressDict = new Dictionary<string, uint>();
            var duplicatedAddressDict = new Dictionary<string, uint>();

            SimaticLADSegment? segment = null;

            //Order list by ADRESS TYPE - BYTE - BIT 
            foreach (var ioData in ioDataList.OrderBy(x => ((int)x.GetAddressMemoryArea()) * Math.Pow(10, 9) + x.GetAddressByte() * Math.Pow(10, 3) + x.GetAddressBit()).ToList())
            {
                //If name of variable is \ i will ignore everything and skip to the next
                if (ioData.Variable == "\\" || ioData.IOName == "\\")
                {
                    continue;
                }

                ioData.LoadDefaults(mainConfig, out bool ioNameDefault, out bool variableDefault, out bool merkerAddressDefault);

                var placeholderHandler = new GenerationPlaceholderHandler().SetIOData(ioData, this.mainConfig);
                ioData.ParsePlaceholders(placeholderHandler);

                if (ioTagTable == null || (mainConfig.IOTableSplitEvery > 0 && tagCounter % mainConfig.IOTableSplitEvery == 0))
                {
                    ioTagTable = new XMLTagTable { TableName = mainConfig.IOTableName + "_" + tagCounter };
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
                    var member = AddMemberToDB(duplicatedAddressDict, ioData.Variable, ioData.Comment);
                    inOutAddress = $"{ioData.Variable}";
                }
                else
                {
                    switch (mainConfig.MemoryType)
                    {
                        case IOMemoryTypeEnum.MERKER:
                            if (variableTagTable == null || (mainConfig.VariableTableSplitEvery > 0 && merkerCounter % mainConfig.VariableTableSplitEvery == 0))
                            {
                                variableTagTable = new XMLTagTable();
                                variableTagTable.TableName = mainConfig.VariableTableName + "_" + merkerCounter;
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
                            var member = AddMemberToDB(duplicatedAddressDict, ioData.Variable, ioData.Comment);
                            inOutAddress = member.GetCompleteSymbol();

                            break;
                    }
                }

                if (mainConfig.GroupingType == IOGroupingTypeEnum.PER_BIT)
                {
                    segment?.Create(fc);

                    segment = new SimaticLADSegment();
                    segment.Title[LocalizationVariables.CULTURE] = placeholderHandler.Parse(tabConfig.SegmentNameBitGrouping);
                }
                else if (mainConfig.GroupingType == IOGroupingTypeEnum.PER_BYTE && (ioData.GetAddressByte() != lastByteAddress || ioData.GetAddressMemoryArea() != lastMemoryArea || segment == null))
                {
                    segment?.Create(fc);

                    lastByteAddress = (int)ioData.GetAddressByte();
                    lastMemoryArea = ioData.GetAddressMemoryArea();

                    segment = new SimaticLADSegment();
                    segment.Title[LocalizationVariables.CULTURE] = placeholderHandler.Parse(tabConfig.SegmentNameByteGrouping);
                }

                if (inOutAddress != null) 
                {
                    ArgumentNullException.ThrowIfNull(segment, nameof(segment));

                    switch (ioData.GetAddressMemoryArea())
                    {
                        case SimaticMemoryArea.INPUT:
                            FillOutSegment(segment, ioTag.TagName, inOutAddress);
                            break;
                        case SimaticMemoryArea.OUTPUT:
                            FillOutSegment(segment, inOutAddress, ioTag.TagName);
                            break;
                        default:
                            throw new ArgumentException("Invalid IOData MemoryArea");
                    }
                }
            }

            segment?.Create(fc); //The last segment would not be generated otherwise (Since they are created during a group change!)

            this.fcDict.Add(name, fc);
        }

        private Member AddMemberToDB(Dictionary<string, uint> duplicatedAddressDict, string variable, string comment)
        {
            if (db == null)
            {
                db = new BlockGlobalDB();
                db.Init();
                db.AttributeList.BlockName = mainConfig.DBName;
                db.AttributeList.BlockNumber = mainConfig.DBNumber;
                db.AttributeList.AutoNumber = (mainConfig.DBNumber > 0);
            }

            var dbMemberAddress = FixDuplicateAddress(variable, duplicatedAddressDict);

            var member = db.AttributeList.STATIC.AddMembersFromAddress(dbMemberAddress, SimaticDataType.BOOLEAN) ?? throw new InvalidDataException();
            member.Comment[LocalizationVariables.CULTURE] = comment;
            return member;
        }

        private static string FixDuplicateAddress(string? address, Dictionary<string, uint> dict)
        {
            address = address ?? SimaticMLAPI.DEFAULT_EMPTY_MEMBER_NAME;

            if (!dict.TryGetValue(address, out uint count))
            {
                dict.Add(address, 0);
            }

            dict[address] = ++count;
            return address + (count <= 1 ? "" : $"({count})");
        }

        private static void FillOutSegment(SimaticLADSegment segment, string contactAddress, string coilAddress)
        {
            var contact = new ContactPart() { Operand = new SimaticGlobalVariable(contactAddress) };
            var coil = new CoilPart() { Operand = new SimaticGlobalVariable(coilAddress) };
            var _ = segment.Powerrail & contact & coil;
        }

        public void ExportXML(string exportPath)
        {
            if (string.IsNullOrEmpty(exportPath) || fcDict.Count == 0 || (this.db == null && ioTagTable == null))
            {
                return;
            }

            foreach (var entry in fcDict)
            {
                var name = entry.Key;
                var fc = entry.Value;

                var xmlDocument = SimaticMLAPI.CreateDocument(fc);
                xmlDocument.Save(exportPath + $"/Export_FC_{name}_{fc.AttributeList.BlockName}.xml");
            }


            foreach (var ioTagTable in ioTagTableList)
            {
                var xmlDocument = SimaticMLAPI.CreateDocument(ioTagTable);
                xmlDocument.Save(exportPath + $"/Export_IOTagTable_{ioTagTable.TableName}.xml");
            }

            foreach(var variableTagTable in variableTagTableList)
            {
                var xmlDocument = SimaticMLAPI.CreateDocument(variableTagTable);
                xmlDocument.Save(exportPath + $"/Export_AliasTagTable_{variableTagTable.TableName}.xml");
            }

            if (db != null)
            {
                var xmlDocument = SimaticMLAPI.CreateDocument(db);
                xmlDocument.Save(exportPath + $"/Export_AliasDB_{db.AttributeList.BlockName}.xml");
            }
        }
    }
}
