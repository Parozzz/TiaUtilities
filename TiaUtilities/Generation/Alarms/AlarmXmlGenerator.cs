using TiaXmlReader.Utility;
using TiaXmlReader.Generation.Placeholders;
using TiaXmlReader.Languages;
using SimaticML.Blocks;
using SimaticML.Blocks.FlagNet.nPart;
using SimaticML.Blocks.FlagNet.nAccess;
using SimaticML.Enums;
using SimaticML.Blocks.FlagNet;
using SimaticML.API;

namespace TiaXmlReader.Generation.Alarms
{

    public class AlarmXmlGenerator
    {
        private readonly AlarmConfiguration config;
        private readonly List<AlarmData> alarmDataList;
        private readonly List<DeviceData> deviceDataList;

        private BlockFC? fc;
        private string fullAlarmList = "";

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
            fc.AttributeList.BlockName = config.FCBlockName;
            fc.AttributeList.BlockNumber = config.FCBlockNumber;
            fc.AttributeList.AutoNumber = (config.FCBlockNumber > 0);

            fullAlarmList = "";

            var segment = new SimaticLADSegment();

            var nextAlarmNum = config.StartingAlarmNum;
            switch (config.PartitionType)
            {
                case AlarmPartitionType.DEVICE:
                    foreach (var deviceData in deviceDataList)
                    {
                        if (config.GroupingType == AlarmGroupingType.GROUP)
                        {
                            segment = new SimaticLADSegment();
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
                                segment = new SimaticLADSegment();
                                segment.Title[LocalizationVariables.CULTURE] = placeholderHandler.Parse(config.OneEachSegmentName);
                            }

                            FillAlarmSegment(segment, placeholderHandler, parsedAlarmData);

                            if(config.GroupingType == AlarmGroupingType.ONE)
                            {
                                segment.Create(this.fc);
                            }
                        }

                        var lastAlarmNum = nextAlarmNum - 1;
                        var alarmCount = (lastAlarmNum - startAlarmNum) + 1;

                        var slippingAlarmCount = alarmCount == 0 ? 0 : config.AntiSlipNumber % alarmCount;
                        if (config.AntiSlipNumber > 0 && slippingAlarmCount > 0)
                        {
                            nextAlarmNum += slippingAlarmCount;

                            if (config.GenerateEmptyAlarmAntiSlip)
                            {
                                GenerateEmptyAlarms(config.GroupingType, config.PartitionType, lastAlarmNum + 1, slippingAlarmCount, segment); //CompileUnit only used for group division
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

                            segment.Title[LocalizationVariables.CULTURE] = placeholderHandler.Parse(config.GroupSegmentName);
                            segment.Create(this.fc);
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
                            segment = new SimaticLADSegment();
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
                                segment = new SimaticLADSegment();
                                segment.Title[LocalizationVariables.CULTURE] = placeholderHandler.Parse(config.OneEachSegmentName);
                            }

                            FillAlarmSegment(segment, placeholderHandler, parsedAlarmData);

                            if (config.GroupingType == AlarmGroupingType.ONE)
                            {
                                segment.Create(this.fc);
                            }
                        }

                        var lastAlarmNum = nextAlarmNum - 1;
                        var alarmCount = (lastAlarmNum - startAlarmNum) + 1;

                        var slippingAlarmCount = config.AntiSlipNumber % alarmCount;
                        if (config.AntiSlipNumber > 0 && slippingAlarmCount > 0)
                        {
                            nextAlarmNum += slippingAlarmCount;

                            if (config.GenerateEmptyAlarmAntiSlip)
                            {
                                GenerateEmptyAlarms(config.GroupingType, config.PartitionType, lastAlarmNum + 1, slippingAlarmCount, segment); //CompileUnit only used for group division
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

                            segment.Title[LocalizationVariables.CULTURE] = placeholderHandler.Parse(config.GroupSegmentName);
                            segment.Create(this.fc);
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

        private void GenerateEmptyAlarms(AlarmGroupingType groupingType, AlarmPartitionType partitionType, uint startAlarmNum, uint alarmCount, SimaticLADSegment? externalGroupSegment = null)
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

            SimaticLADSegment? segment = externalGroupSegment;
            if (segment == null && groupingType == AlarmGroupingType.GROUP)
            {
                var placeholderHandler = new GenerationPlaceholderHandler()
                    .SetAlarmData(emptyAlarmData)
                    .SetStartEndAlarmNum(alarmNum, alarmNum + (alarmCount - 1), config.AlarmNumFormat);

                segment = new SimaticLADSegment();
                segment.Title[LocalizationVariables.CULTURE] = placeholderHandler.Parse(config.GroupEmptyAlarmSegmentName);
            }

            for (int j = 0; j < alarmCount; j++)
            {
                var placeholderHandler = new GenerationPlaceholderHandler().SetAlarmData(emptyAlarmData).SetAlarmNum(alarmNum++, config.AlarmNumFormat);

                if (groupingType == AlarmGroupingType.ONE)
                {
                    segment = new SimaticLADSegment();
                    segment.Title[LocalizationVariables.CULTURE] = placeholderHandler.Parse(config.OneEachEmptyAlarmSegmentName);
                }

                ArgumentNullException.ThrowIfNull(segment, nameof(segment));
                FillAlarmSegment(segment, placeholderHandler, emptyAlarmData);
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
                TimerType = string.IsNullOrEmpty(alarmData.TimerType) ? config.DefaultTimerType : alarmData.TimerType,
                TimerValue = string.IsNullOrEmpty(alarmData.TimerValue) ? config.DefaultTimerValue : alarmData.TimerValue,
                Description = alarmData.Description,
                Enable = alarmData.Enable
            };

        }

        private void FillAlarmSegment(SimaticLADSegment segment, GenerationPlaceholderHandler placeholders, AlarmData alarmData)
        {
            if (string.IsNullOrEmpty(alarmData.AlarmVariable))
            {
                return;
            }

            var parsedContactAddress = placeholders.Parse(alarmData.AlarmVariable);
            var contact = new ContactPart()
            {
                Operand = parsedContactAddress.ToLower() switch
                {
                    "false" => new SimaticLiteralConstant(SimaticDataType.BOOLEAN, "FALSE"),
                    "0" => new SimaticLiteralConstant(SimaticDataType.BOOLEAN, "0"),
                    "true" => new SimaticLiteralConstant(SimaticDataType.BOOLEAN, "TRUE"),
                    "1" => new SimaticLiteralConstant(SimaticDataType.BOOLEAN, "1"),
                    _ => new SimaticGlobalVariable(parsedContactAddress),
                }
            };

            TimerPart? timer = null;
            if (!string.IsNullOrEmpty(alarmData.TimerAddress)
                && !string.IsNullOrEmpty(alarmData.TimerType)
                && !string.IsNullOrEmpty(alarmData.TimerValue)
                && alarmData.IsTimerAddressValid())
            {
                var partType = alarmData.TimerType.ToLower() switch
                {
                    "ton" => PartType.TON,
                    "tof" => PartType.TOF,
                    _ => throw new Exception("Unknow timer type of " + alarmData.TimerType),
                };

                timer = new TimerPart(partType)
                {
                    InstanceScope = SimaticVariableScope.GLOBAL_VARIABLE,
                    InstanceAddress = placeholders.Parse(alarmData.TimerAddress),
                    PT = new SimaticTypedConstant(alarmData.TimerValue),
                };
            }

            SetCoilPart? setCoil = null;
            if (!string.IsNullOrEmpty(alarmData.SetCoilAddress) && alarmData.SetCoilAddress != "\\")
            {
                var setCoilVariable = new SimaticGlobalVariable(placeholders.Parse(alarmData.SetCoilAddress));
                setCoil = new SetCoilPart() { Operand = setCoilVariable };
            }

            CoilPart? coil = null;
            if (!string.IsNullOrEmpty(alarmData.CoilAddress) && alarmData.CoilAddress != "\\")
            {
                var coilVariable = new SimaticGlobalVariable(placeholders.Parse(alarmData.CoilAddress));
                coil = new CoilPart() { Operand = coilVariable };
            }

            var simaticPartList = new List<SimaticPart?>
            {
                contact,
                timer,
                config.CoilFirst ? coil : setCoil,
                config.CoilFirst ? setCoil : coil
            };

            SimaticPart? lastPart = null;
            foreach (var simaticPart in simaticPartList)
            {
                if(simaticPart == null)
                {
                    continue;
                }

                if (lastPart == null)
                {
                    segment.Powerrail.Add(simaticPart);
                    lastPart = simaticPart;
                    continue;
                }

                lastPart.AND(simaticPart);
                lastPart = simaticPart;
            }
        }

        public void ExportXML(string exportPath)
        {
            if (string.IsNullOrEmpty(exportPath))
            {
                return;
            }

            ArgumentNullException.ThrowIfNull(fc, nameof(fc));

            var xmlDocument = SimaticMLAPI.CreateDocument(fc);
            xmlDocument.Save(exportPath + "/fcExport_" + fc.AttributeList.BlockName + ".xml");

            var alarmTextPath = exportPath + "/alarmsText.txt";

            using var stream = File.CreateText(alarmTextPath);
            stream.Write(fullAlarmList);
        }

    }
}
