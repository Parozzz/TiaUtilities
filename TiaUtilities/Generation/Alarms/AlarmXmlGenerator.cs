using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.Utility;
using TiaXmlReader.SimaticML;
using TiaXmlReader.SimaticML.Blocks;
using TiaXmlReader.SimaticML.Blocks.FlagNet.nAccess;
using TiaXmlReader.SimaticML.Blocks.FlagNet.nPart;
using TiaXmlReader.Generation.Placeholders;
using TiaXmlReader.Languages;

namespace TiaXmlReader.Generation.Alarms
{

    public class AlarmXmlGenerator
    {
        private readonly AlarmConfiguration config;
        private readonly List<AlarmData> alarmDataList;
        private readonly List<DeviceData> deviceDataList;

        private BlockFC fc;
        private CompileUnit compileUnit;
        private string fullAlarmList;

        public AlarmXmlGenerator(AlarmConfiguration config, List<AlarmData> alarmDataList, List<DeviceData> deviceDataList)
        {
            this.config = config;
            this.alarmDataList = alarmDataList;
            this.deviceDataList = deviceDataList;
        }

        public void GenerateBlocks()
        {
            fc = new BlockFC();
            fc.Init();
            fc.GetBlockAttributes().SetBlockName(config.FCBlockName).SetBlockNumber(config.FCBlockNumber).SetAutoNumber(config.FCBlockNumber > 0);

            fullAlarmList = "";

            var nextAlarmNum = config.StartingAlarmNum;
            switch (config.PartitionType)
            {
                case AlarmPartitionType.DEVICE:
                    foreach (var deviceData in deviceDataList)
                    {
                        if (config.GroupingType == AlarmGroupingType.GROUP)
                        {
                            compileUnit = fc.AddCompileUnit();
                            compileUnit.Init();
                        }

                        var placeholderHandler = new GenerationPlaceholderHandler();

                        var startAlarmNum = nextAlarmNum;
                        foreach (var alarmData in alarmDataList)
                        {
                            if (!alarmData.Enable)
                            {
                                continue;
                            }

                            var parsedAlarmData = ReplaceAlarmDataWithDefaultAndPrefix(alarmData);

                            placeholderHandler.SetDeviceData(deviceData)
                                .SetAlarmData(parsedAlarmData)
                                .SetAlarmNum(nextAlarmNum++, config.AlarmNumFormat);
                            fullAlarmList += placeholderHandler.Parse(this.config.AlarmTextInList) + '\n';

                            if (config.GroupingType == AlarmGroupingType.ONE)
                            {
                                compileUnit = fc.AddCompileUnit();
                                compileUnit.Init();
                                compileUnit.ComputeBlockTitle().SetText(LocalizationVariables.CULTURE, placeholderHandler.Parse(config.OneEachSegmentName));
                            }

                            FillAlarmCompileUnit(compileUnit, placeholderHandler, parsedAlarmData);
                        }

                        var lastAlarmNum = nextAlarmNum - 1;
                        var alarmCount = (lastAlarmNum - startAlarmNum) + 1;

                        var slippingAlarmCount = config.AntiSlipNumber % alarmCount;
                        if (config.AntiSlipNumber > 0 && slippingAlarmCount > 0)
                        {
                            nextAlarmNum += slippingAlarmCount;

                            if (config.GenerateEmptyAlarmAntiSlip)
                            {
                                GenerateEmptyAlarms(config.GroupingType, config.PartitionType, lastAlarmNum + 1, slippingAlarmCount, compileUnit); //CompileUnit only used for group division
                                lastAlarmNum += slippingAlarmCount;
                            }

                            for (var x = 0; x < slippingAlarmCount; x++)
                            {
                                fullAlarmList += placeholderHandler.Parse(this.config.EmptyAlarmTextInList) + '\n';
                            }
                        }

                        if (config.GroupingType == AlarmGroupingType.GROUP)
                        {
                            placeholderHandler.SetStartEndAlarmNum(startAlarmNum, lastAlarmNum, config.AlarmNumFormat);
                            compileUnit.ComputeBlockTitle().SetText(LocalizationVariables.CULTURE, placeholderHandler.Parse(config.GroupSegmentName));
                        }

                        nextAlarmNum += config.SkipNumberAfterGroup;
                        for (var x = 0; x < config.SkipNumberAfterGroup; x++)
                        {
                            fullAlarmList += '\n';
                        }
                    }
                    break;
                case AlarmPartitionType.ALARM_TYPE:
                    foreach (var alarmData in alarmDataList)
                    {
                        if (!alarmData.Enable)
                        {
                            continue;
                        }

                        var parsedAlarmData = ReplaceAlarmDataWithDefaultAndPrefix(alarmData);

                        if (config.GroupingType == AlarmGroupingType.GROUP)
                        {
                            compileUnit = fc.AddCompileUnit();
                            compileUnit.Init();
                        }

                        var placeholderHandler = new GenerationPlaceholderHandler();

                        var startAlarmNum = nextAlarmNum;
                        foreach (var deviceData in deviceDataList)
                        {
                            placeholderHandler.SetDeviceData(deviceData)
                                    .SetAlarmData(parsedAlarmData)
                                    .SetAlarmNum(nextAlarmNum++, config.AlarmNumFormat);
                            fullAlarmList += placeholderHandler.Parse(this.config.AlarmTextInList) + '\n';

                            if (config.GroupingType == AlarmGroupingType.ONE)
                            {
                                compileUnit = fc.AddCompileUnit();
                                compileUnit.Init();
                                compileUnit.ComputeBlockTitle().SetText(LocalizationVariables.CULTURE, placeholderHandler.Parse(config.OneEachSegmentName));
                            }

                            FillAlarmCompileUnit(compileUnit, placeholderHandler, parsedAlarmData);
                        }

                        var lastAlarmNum = nextAlarmNum - 1;
                        var alarmCount = (lastAlarmNum - startAlarmNum) + 1;

                        var slippingAlarmCount = config.AntiSlipNumber % alarmCount;
                        if (config.AntiSlipNumber > 0 && slippingAlarmCount > 0)
                        {
                            nextAlarmNum += slippingAlarmCount;

                            if (config.GenerateEmptyAlarmAntiSlip)
                            {
                                GenerateEmptyAlarms(config.GroupingType, config.PartitionType, lastAlarmNum + 1, slippingAlarmCount, compileUnit); //CompileUnit only used for group division
                                lastAlarmNum += slippingAlarmCount;
                            }

                            for (var x = 0; x < slippingAlarmCount; x++)
                            {
                                fullAlarmList += placeholderHandler.Parse(this.config.EmptyAlarmTextInList) + '\n';
                            }
                        }

                        if (config.GroupingType == AlarmGroupingType.GROUP)
                        {
                            placeholderHandler.SetStartEndAlarmNum(startAlarmNum, lastAlarmNum, config.AlarmNumFormat);
                            compileUnit.ComputeBlockTitle().SetText(LocalizationVariables.CULTURE, placeholderHandler.Parse(config.GroupSegmentName));
                        }

                        nextAlarmNum += config.SkipNumberAfterGroup;
                        for (var x = 0; x < config.SkipNumberAfterGroup; x++)
                        {
                            fullAlarmList += '\n';
                        }
                    }
                    break;
            }

            GenerateEmptyAlarms(config.GroupingType, config.PartitionType, nextAlarmNum, config.EmptyAlarmAtEnd);
        }

        private void GenerateEmptyAlarms(AlarmGroupingType groupingType, AlarmPartitionType partitionType, uint startAlarmNum, uint alarmCount, CompileUnit externalGroupCompileUnit = null)
        {
            var emptyAlarmData = new AlarmData()
            {
                AlarmVariable = config.EmptyAlarmContactAddress,
                CoilAddress = config.DefaultCoilAddress,
                SetCoilAddress = config.DefaultSetCoilAddress,
                TimerAddress = config.EmptyAlarmTimerAddress,
                TimerType = config.EmptyAlarmTimerType,
                TimerValue = config.EmptyAlarmTimerValue,
                Description = "",
                Enable = true
            };

            var alarmNum = startAlarmNum;

            CompileUnit compileUnit = externalGroupCompileUnit;
            if (compileUnit == null && groupingType == AlarmGroupingType.GROUP)
            {
                var placeholderHandler = new GenerationPlaceholderHandler()
                    .SetAlarmData(emptyAlarmData)
                    .SetStartEndAlarmNum(alarmNum, alarmNum + (alarmCount - 1), config.AlarmNumFormat);

                compileUnit = fc.AddCompileUnit();
                compileUnit.Init();
                compileUnit.ComputeBlockTitle().SetText(LocalizationVariables.CULTURE, placeholderHandler.Parse(config.GroupEmptyAlarmSegmentName));
            }

            for (int j = 0; j < alarmCount; j++)
            {
                var placeholderHandler = new GenerationPlaceholderHandler().SetAlarmData(emptyAlarmData).SetAlarmNum(alarmNum++, config.AlarmNumFormat);

                if (groupingType == AlarmGroupingType.ONE)
                {
                    compileUnit = fc.AddCompileUnit();
                    compileUnit.Init();
                    compileUnit.ComputeBlockTitle().SetText(LocalizationVariables.CULTURE, placeholderHandler.Parse(config.OneEachEmptyAlarmSegmentName));
                }

                FillAlarmCompileUnit(compileUnit, placeholderHandler, emptyAlarmData);
            }
        }

        private AlarmData ReplaceAlarmDataWithDefaultAndPrefix(AlarmData alarmData)
        {
            return new AlarmData()
            {
                AlarmVariable = config.AlarmAddressPrefix + alarmData.AlarmVariable,
                CoilAddress = string.IsNullOrEmpty(alarmData.CoilAddress) ? config.DefaultCoilAddress : (config.CoilAddressPrefix + alarmData.CoilAddress),
                SetCoilAddress = string.IsNullOrEmpty(alarmData.SetCoilAddress) ? config.DefaultSetCoilAddress : (config.SetCoilAddressPrefix + alarmData.SetCoilAddress),
                TimerAddress = string.IsNullOrEmpty(alarmData.TimerAddress) ? config.DefaultTimerAddress : (config.TimerAddressPrefix + alarmData.TimerAddress),
                TimerType = Utils.StringFullOr(alarmData.TimerType, config.DefaultTimerType),
                TimerValue = Utils.StringFullOr(alarmData.TimerValue, config.DefaultTimerValue),
                Description = alarmData.Description,
                Enable = alarmData.Enable
            };

        }

        private void FillAlarmCompileUnit(CompileUnit compileUnit, GenerationPlaceholderHandler placeholders, AlarmData alarmData)
        {
            var parsedContactAddress = placeholders.Parse(alarmData.AlarmVariable);

            IAccessData contactAccessData;
            switch (parsedContactAddress.ToLower())
            {
                case "false":
                    contactAccessData = LiteralConstantAccessData.Create(compileUnit, SimaticDataType.BOOLEAN, "FALSE");
                    break;
                case "0":
                    contactAccessData = LiteralConstantAccessData.Create(compileUnit, SimaticDataType.BOOLEAN, "0");
                    break;
                case "true":
                    contactAccessData = LiteralConstantAccessData.Create(compileUnit, SimaticDataType.BOOLEAN, "TRUE");
                    break;
                case "1":
                    contactAccessData = LiteralConstantAccessData.Create(compileUnit, SimaticDataType.BOOLEAN, "1");
                    break;
                default:
                    contactAccessData = GlobalVariableAccessData.Create(compileUnit, parsedContactAddress);
                    break;
            }

            var contact = new ContactPartData(compileUnit);
            contact.CreateIdentWire(contactAccessData);
            contact.CreatePowerrailConnection();

            IPartData timer = null;
            if (!string.IsNullOrEmpty(alarmData.TimerAddress) && alarmData.TimerAddress.ToLower() != "\\")
            {
                PartType partType;
                switch (alarmData.TimerType.ToLower())
                {
                    case "ton":
                        partType = PartType.TON;
                        break;
                    case "tof":
                        partType = PartType.TOF;
                        break;
                    default:
                        throw new Exception("Unknow timer type of " + alarmData.TimerType);
                }

                timer = new TimerPartData(compileUnit, partType);
                ((TimerPartData)timer)
                    .SetPartInstance(SimaticVariableScope.GLOBAL_VARIABLE, placeholders.Parse(alarmData.TimerAddress))
                    .SetTimeValue(alarmData.TimerValue);
            }

            IPartData setCoil = null;
            if (alarmData.SetCoilAddress.ToLower() != "\\")
            {
                setCoil = new SetCoilPartData(compileUnit);
                ((SetCoilPartData)setCoil).CreateIdentWire(GlobalVariableAccessData.Create(compileUnit, placeholders.Parse(alarmData.SetCoilAddress)));
            }

            IPartData coil = null;
            if (alarmData.CoilAddress.ToLower() != "\\")
            {
                coil = new CoilPartData(compileUnit);
                ((CoilPartData)coil).CreateIdentWire(GlobalVariableAccessData.Create(compileUnit, placeholders.Parse(alarmData.CoilAddress)));
            }

            var partDataList = new List<IPartData>
            {
                contact,
                timer,
                config.CoilFirst ? coil : setCoil,
                config.CoilFirst ? setCoil : coil
            };

            IPartData previousPartData = null;
            foreach (var partData in partDataList)
            {
                if (previousPartData == null)
                {
                    previousPartData = partData;
                    continue;
                }

                if (partData != null)
                {
                    previousPartData.CreateOutputConnection(previousPartData = partData);
                }
            }

            if (previousPartData == contact)
            {
                contact.CreateOpenCon();
            }
        }

        public void ExportXML(string exportPath)
        {
            if (string.IsNullOrEmpty(exportPath))
            {
                return;
            }

            if (fc == null)
            {
                throw new ArgumentNullException("Blocks has not been generated");
            }

            fc.UpdateID_UId(new IDGenerator());

            var xmlDocument = SimaticMLParser.CreateDocument();
            xmlDocument.DocumentElement.AppendChild(fc.Generate(xmlDocument));
            xmlDocument.Save(exportPath + "/fcExport_" + fc.GetBlockAttributes().GetBlockName() + ".xml");

            var alarmTextPath = exportPath + "/alarmsText.txt";
            using (var stream = File.CreateText(alarmTextPath))
            {
                stream.Write(fullAlarmList);
            }
        }

    }
}
