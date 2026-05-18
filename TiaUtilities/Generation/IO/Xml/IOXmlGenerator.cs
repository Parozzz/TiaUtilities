using SimaticML;
using SimaticML.API;
using SimaticML.Blocks;
using SimaticML.Blocks.FlagNet;
using SimaticML.Enums;
using SimaticML.nBlockAttributeList;
using SimaticML.TagTable;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.IO.Configurations;
using TiaUtilities.Generation.IO.Data;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Languages;

namespace TiaUtilities.Generation.IO.Xml
{
    internal class IOXmlGenerator(IOMainConfiguration mainConfig)
    {
        private readonly IOMainConfiguration mainConfig = mainConfig;

        private readonly Dictionary<string, BlockFC> fcDict = [];
        private readonly Dictionary<string, List<XMLTagTable>> ioTagTableDict = [];

        private readonly BlockGlobalDB globalDB = new();
        private readonly List<XMLTagTable> variableTagTableList = [];

        public void Init()
        {
        }

        public void GenerateAlias(string tabName, GridDataPreviewer<IOData> previewer, IOTabConfiguration tabConfig, List<IOData> ioDataList)
        {
            IOGenPlaceholderHandler placeholderHandler = new(previewer, this.mainConfig, tabConfig)
            {
                TabName = tabName
            };

            this.globalDB.Init();
            this.globalDB.AttributeList.BlockName = placeholderHandler.ParseNotNull(mainConfig.DBName);
            this.globalDB.AttributeList.BlockNumber = mainConfig.DBNumber;
            this.globalDB.AttributeList.AutoNumber = (mainConfig.DBNumber > 0);

            XMLTagTable? variableTagTable = null;

            BlockFC fc = new();
            fc.Init();
            fc.AttributeList.BlockName = placeholderHandler.ParseNotNull(tabConfig.FCBlockName);
            fc.AttributeList.BlockNumber = tabConfig.FCBlockNumber;
            fc.AttributeList.AutoNumber = (tabConfig.FCBlockNumber > 0);

            uint tagCounter = 0;
            uint merkerCounter = 0;

            var lastByteAddress = -1;
            var lastMemoryArea = SimaticMemoryArea.INPUT;

            var ioAddressDict = new Dictionary<string, uint>();
            var duplicatedAddressDict = new Dictionary<string, uint>();

            List<XMLTagTable> ioTagTableList = [];
            ioTagTableDict.Add(tabName, ioTagTableList);

            XMLTagTable ioTagTable = new() { TableName = $"{placeholderHandler.Parse(mainConfig.IOTableName)}_{tagCounter}" };
            ioTagTableList.Add(ioTagTable);

            SimaticLADSegment? segment = null;

            //Order list by ADRESS TYPE - BYTE - BIT 
            foreach (var ioData in ioDataList.OrderBy(IOXmlGenerator.OrderByAddress))
            {
                //If name of variable is \ i will ignore everything and skip to the next
                if (GenUtils.DATA_INVALID_CHARS.Contains(ioData.Variable) || GenUtils.DATA_INVALID_CHARS.Contains(ioData.IOName))
                {
                    continue;
                }

                ioData.LoadDefaults(previewer, mainConfig);

                placeholderHandler.Clear();
                placeholderHandler.TabName = tabName;
                placeholderHandler.IOData = ioData;

                ioData.ParsePlaceholders(placeholderHandler);

                if (tagCounter > 0 && mainConfig.IOTableSplitEvery > 0 && (tagCounter % mainConfig.IOTableSplitEvery == 0))
                {
                    ioTagTable = new() { TableName = $"{placeholderHandler.Parse(mainConfig.IOTableName)}_{tagCounter}" };
                    ioTagTableList.Add(ioTagTable);
                }
                tagCounter++;

                //Set it with default tag name. In case it has a specific one, it will be overwritten below.
                var ioTag = ioTagTable.AddTag();
                ioTag.TagName = FixDuplicateAddress(ioData.IOName, ioAddressDict);
                ioTag.Comment[LocaleVariables.CULTURE] = ioData.Comment;
                ioTag.SetBoolean(ioData.GetAddressMemoryArea(), ioData.GetAddressByte(), ioData.GetAddressBit());

                string? inOutAddress = null;
                switch (mainConfig.MemoryType)
                {
                    case IOMemoryTypeEnum.MERKER:
                        if (variableTagTable == null || (mainConfig.VariableTableSplitEvery > 0 && merkerCounter % mainConfig.VariableTableSplitEvery == 0))
                        {
                            variableTagTable = new() 
                            { 
                                TableName = placeholderHandler.ParseNotNull($"{mainConfig.VariableTableName}_{merkerCounter}")
                            };
                            variableTagTableList.Add(variableTagTable);
                        }
                        merkerCounter++;

                        var merkerVariableAddress = FixDuplicateAddress(ioData.Variable, duplicatedAddressDict);

                        var merkerVariableTag = SimaticTagAddress.FromAddress(ioData.MerkerAddress);
                        if (merkerVariableAddress == null || merkerVariableTag == null)
                        {
                            throw new Exception("Cannot parse Merker VariableAddress for " + ioData.MerkerAddress);
                        }

                        var tag = variableTagTable.AddTag();
                        tag.TagName = merkerVariableAddress;
                        tag.Comment[LocaleVariables.CULTURE] = ioData.Comment;
                        tag.SetBoolean(SimaticMemoryArea.MERKER, merkerVariableTag.ByteOffset, merkerVariableTag.BitOffset);

                        inOutAddress = $"\"{merkerVariableAddress}\""; //Add double quote to avoid this address to be parsed as a DB call (eg. to avoid I0.0 to be parsed as "I0"."0" instead "I0.0")
                        break;
                    case IOMemoryTypeEnum.DB:
                        var dbMemberAddress = FixDuplicateAddress(ioData.Variable, duplicatedAddressDict);

                        var member = globalDB.AttributeList.STATIC.AddMembersFromAddress(dbMemberAddress, SimaticDataType.BOOLEAN) ?? throw new InvalidDataException();
                        member.Comment[LocaleVariables.CULTURE] = ioData.Comment;

                        inOutAddress = member.GetCompleteSymbol();
                        break;
                }

                if (mainConfig.GroupingType == IOGroupingTypeEnum.PER_BIT)
                {
                    segment?.Create(fc);

                    segment = new SimaticLADSegment();
                    segment.Title[LocaleVariables.CULTURE] = placeholderHandler.Parse(tabConfig.SegmentNameBitGrouping);
                }
                else if (mainConfig.GroupingType == IOGroupingTypeEnum.PER_BYTE && (ioData.GetAddressByte() != lastByteAddress || ioData.GetAddressMemoryArea() != lastMemoryArea || segment == null))
                {
                    segment?.Create(fc);

                    lastByteAddress = (int)ioData.GetAddressByte();
                    lastMemoryArea = ioData.GetAddressMemoryArea();

                    segment = new SimaticLADSegment();
                    segment.Title[LocaleVariables.CULTURE] = placeholderHandler.Parse(tabConfig.SegmentNameByteGrouping);
                }

                if (inOutAddress != null)
                {
                    ArgumentNullException.ThrowIfNull(segment, nameof(segment));

                    switch (ioData.GetAddressMemoryArea())
                    {
                        case SimaticMemoryArea.INPUT:
                            FillOutSegment(segment, ioTag.TagName, ioData.Negated, inOutAddress);
                            break;
                        case SimaticMemoryArea.OUTPUT:
                            FillOutSegment(segment, inOutAddress, ioData.Negated, ioTag.TagName);
                            break;
                        default:
                            throw new ArgumentException("Invalid IOData MemoryArea");
                    }
                }
            }

            segment?.Create(fc); //The last segment would not be generated otherwise (Since they are created during a group change!)

            this.fcDict.Add(tabName, fc);
        }

        private static double OrderByAddress(IOData iOData)
        {
            return ((int)iOData.GetAddressMemoryArea()) * Math.Pow(10, 9) + iOData.GetAddressByte() * Math.Pow(10, 3) + iOData.GetAddressBit();
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

        private static void FillOutSegment(SimaticLADSegment segment, string contactAddress, bool negated, string coilAddress)
        {
            var contact = new ContactPart() { Operand = new SimaticGlobalVariable(contactAddress), Negated = negated };
            var coil = new CoilPart() { Operand = new SimaticGlobalVariable(coilAddress) };
            var _ = segment.Powerrail & contact & coil;
        }

        public void ExportXML(string exportPath)
        {
            if (string.IsNullOrEmpty(exportPath) || fcDict.Count == 0 || (this.globalDB == null && this.ioTagTableDict.Count == 0))
            {
                return;
            }

            foreach (var entry in fcDict)
            {
                var name = entry.Key;
                var fc = entry.Value;

                var xmlDocument = SimaticMLAPI.CreateDocument(fc);
                xmlDocument.Save(exportPath + $"/[FC]-{fc.AttributeList.BlockName}.xml");
            }

            foreach (var entry in ioTagTableDict)
            {
                var name = entry.Key;
                var ioTagTableList = entry.Value;

                foreach (var ioTagTable in ioTagTableList)
                {
                    var xmlDocument = SimaticMLAPI.CreateDocument(ioTagTable);
                    xmlDocument.Save(exportPath + $"/[TagTable_IO]-{ioTagTable.TableName}.xml");
                }
            }

            foreach (var variableTagTable in variableTagTableList)
            {
                var xmlDocument = SimaticMLAPI.CreateDocument(variableTagTable);
                xmlDocument.Save(exportPath + $"/[TagTable_Alias]-{variableTagTable.TableName}.xml");
            }

            if (globalDB != null)
            {
                var xmlDocument = SimaticMLAPI.CreateDocument(globalDB);
                xmlDocument.Save(exportPath + $"/[DB_Alias]-{globalDB.AttributeList.BlockName}.xml");
            }
        }
    }
}
