using TiaXmlReader.Utility;
using TiaXmlReader.Generation.Placeholders;
using TiaXmlReader.Languages;
using SimaticML.Blocks;
using SimaticML.Blocks.FlagNet.nPart;
using SimaticML.Blocks.FlagNet.nAccess;
using SimaticML.Enums;
using SimaticML.Blocks.FlagNet;
using SimaticML.API;
using TiaUtilities.Generation.GenForms.Alarm.Tab;
using TiaUtilities.Generation.Alarms;

namespace TiaXmlReader.Generation.Alarms
{

    public class AlarmXmlGenerator(AlarmMainConfiguration mainConfig)
    {
        private readonly AlarmMainConfiguration mainConfig = mainConfig;
        private readonly Dictionary<string, string> alarmListDict = [];

        private BlockFC? fc;

        public void Init()
        {
            fc = new();
            fc.Init();
            fc.AttributeList.BlockName = mainConfig.FCBlockName;
            fc.AttributeList.BlockNumber = mainConfig.FCBlockNumber;
            fc.AttributeList.AutoNumber = (mainConfig.FCBlockNumber > 0);
        }

        public void GenerateAlarms(string name, AlarmTabConfiguration tabConfig, List<AlarmData> alarmDataList, List<DeviceData> deviceDataList)
        {
            if(fc == null)
            {
                return;
            }

            var fullAlarmList = "";

            var segment = new SimaticLADSegment();

            var nextAlarmNum = tabConfig.StartingAlarmNum;
            switch (tabConfig.PartitionType)
            {
                case AlarmPartitionType.DEVICE:
                    foreach (var deviceData in deviceDataList)
                    {
                        if (tabConfig.GroupingType == AlarmGroupingType.GROUP)
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

                            var parsedAlarmData = ReplaceAlarmDataWithDefaultAndPrefix(tabConfig, alarmData);

                            placeholderHandler.SetDeviceData(deviceData)
                                .SetAlarmData(parsedAlarmData)
                                .SetAlarmNum(nextAlarmNum++, tabConfig.AlarmNumFormat);
                            fullAlarmList += placeholderHandler.Parse(this.mainConfig.AlarmTextInList) + '\n';

                            if (tabConfig.GroupingType == AlarmGroupingType.ONE)
                            {
                                segment = new SimaticLADSegment();
                                segment.Title[LocalizationVariables.CULTURE] = placeholderHandler.Parse(mainConfig.OneEachSegmentName);
                            }

                            FillAlarmSegment(tabConfig, segment, placeholderHandler, parsedAlarmData);

                            if(tabConfig.GroupingType == AlarmGroupingType.ONE)
                            {
                                segment.Create(this.fc);
                            }
                        }

                        var lastAlarmNum = nextAlarmNum - 1;
                        var alarmCount = (lastAlarmNum - startAlarmNum) + 1;

                        if(tabConfig.AntiSlipNumber > 0)
                        {
                            uint slippingAlarmCount = 0;
                            if (alarmCount < tabConfig.AntiSlipNumber)
                            {
                                slippingAlarmCount = tabConfig.AntiSlipNumber - alarmCount;
                            }
                            else
                            {
                                slippingAlarmCount = tabConfig.AntiSlipNumber % alarmCount;
                            }

                            if(slippingAlarmCount > 0)
                            {
                                nextAlarmNum += slippingAlarmCount;

                                for (var x = 0; x < slippingAlarmCount; x++)
                                {
                                    placeholderHandler.SetAlarmNum((uint)(lastAlarmNum + x + 1), tabConfig.AlarmNumFormat);
                                    fullAlarmList += placeholderHandler.Parse(this.mainConfig.EmptyAlarmTextInList) + '\n';
                                }

                                if (tabConfig.GenerateEmptyAlarmAntiSlip)
                                {
                                    GenerateEmptyAlarms(tabConfig, lastAlarmNum + 1, slippingAlarmCount, segment); //CompileUnit only used for group division
                                    lastAlarmNum += slippingAlarmCount;
                                }
                            }
                        }

                        if (tabConfig.GroupingType == AlarmGroupingType.GROUP)
                        {
                            placeholderHandler.SetStartEndAlarmNum(startAlarmNum, lastAlarmNum, tabConfig.AlarmNumFormat);

                            segment.Title[LocalizationVariables.CULTURE] = placeholderHandler.Parse(mainConfig.GroupSegmentName);
                            segment.Create(this.fc);
                        }

                        nextAlarmNum += tabConfig.SkipNumberAfterGroup;
                        for (var x = 0; x < tabConfig.SkipNumberAfterGroup; x++)
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

                        var parsedAlarmData = ReplaceAlarmDataWithDefaultAndPrefix(tabConfig, alarmData);

                        if (tabConfig.GroupingType == AlarmGroupingType.GROUP)
                        {
                            segment = new SimaticLADSegment();
                        }

                        var placeholderHandler = new GenerationPlaceholderHandler();

                        var startAlarmNum = nextAlarmNum;
                        foreach (var deviceData in deviceDataList)
                        {
                            placeholderHandler.SetDeviceData(deviceData)
                                    .SetAlarmData(parsedAlarmData)
                                    .SetAlarmNum(nextAlarmNum++, tabConfig.AlarmNumFormat);
                            fullAlarmList += placeholderHandler.Parse(this.mainConfig.AlarmTextInList) + '\n';

                            if (tabConfig.GroupingType == AlarmGroupingType.ONE)
                            {
                                segment = new SimaticLADSegment();
                                segment.Title[LocalizationVariables.CULTURE] = placeholderHandler.Parse(mainConfig.OneEachSegmentName);
                            }

                            FillAlarmSegment(tabConfig, segment, placeholderHandler, parsedAlarmData);

                            if (tabConfig.GroupingType == AlarmGroupingType.ONE)
                            {
                                segment.Create(this.fc);
                            }
                        }

                        var lastAlarmNum = nextAlarmNum - 1;
                        var alarmCount = (lastAlarmNum - startAlarmNum) + 1;
                        if (tabConfig.AntiSlipNumber > 0)
                        {
                            uint slippingAlarmCount = 0;
                            if (alarmCount < tabConfig.AntiSlipNumber)
                            {
                                slippingAlarmCount = alarmCount - tabConfig.AntiSlipNumber;
                            }
                            else
                            {
                                slippingAlarmCount = tabConfig.AntiSlipNumber % alarmCount;
                            }

                            if (slippingAlarmCount > 0)
                            {
                                nextAlarmNum += slippingAlarmCount;

                                for (var x = 0; x < slippingAlarmCount; x++)
                                {
                                    placeholderHandler.SetAlarmNum((uint)(lastAlarmNum + x + 1), tabConfig.AlarmNumFormat);
                                    fullAlarmList += placeholderHandler.Parse(this.mainConfig.EmptyAlarmTextInList) + '\n';
                                }

                                if (tabConfig.GenerateEmptyAlarmAntiSlip)
                                {
                                    GenerateEmptyAlarms(tabConfig, lastAlarmNum + 1, slippingAlarmCount, segment); //CompileUnit only used for group division
                                    lastAlarmNum += slippingAlarmCount;
                                }
                            }
                        }

                        if (tabConfig.GroupingType == AlarmGroupingType.GROUP)
                        {
                            placeholderHandler.SetStartEndAlarmNum(startAlarmNum, lastAlarmNum, tabConfig.AlarmNumFormat);

                            segment.Title[LocalizationVariables.CULTURE] = placeholderHandler.Parse(mainConfig.GroupSegmentName);
                            segment.Create(this.fc);
                        }

                        nextAlarmNum += tabConfig.SkipNumberAfterGroup;
                        for (var x = 0; x < tabConfig.SkipNumberAfterGroup; x++)
                        {
                            fullAlarmList += '\n';
                        }
                    }
                    break;
            }

            GenerateEmptyAlarms(tabConfig, nextAlarmNum, tabConfig.EmptyAlarmAtEnd);

            alarmListDict.Add(name, fullAlarmList);
        }

        private void GenerateEmptyAlarms(AlarmTabConfiguration tabConfig,  uint startAlarmNum, uint alarmCount, SimaticLADSegment? externalGroupSegment = null)
        {
            var emptyAlarmData = new AlarmData()
            {
                AlarmVariable = tabConfig.EmptyAlarmContactAddress,
                Coil1Address = tabConfig.DefaultCoil1Address,
                Coil2Address = tabConfig.DefaultCoil2Address,
                TimerAddress = tabConfig.EmptyAlarmTimerAddress,
                TimerType = tabConfig.EmptyAlarmTimerType,
                TimerValue = tabConfig.EmptyAlarmTimerValue,
                Description = "",
                Enable = true
            };

            var alarmNum = startAlarmNum;

            SimaticLADSegment? segment = externalGroupSegment;
            if (segment == null && tabConfig.GroupingType == AlarmGroupingType.GROUP)
            {
                var placeholderHandler = new GenerationPlaceholderHandler()
                    .SetAlarmData(emptyAlarmData)
                    .SetStartEndAlarmNum(alarmNum, alarmNum + (alarmCount - 1), tabConfig.AlarmNumFormat);

                segment = new SimaticLADSegment();
                segment.Title[LocalizationVariables.CULTURE] = placeholderHandler.Parse(mainConfig.GroupEmptyAlarmSegmentName);
            }

            for (int j = 0; j < alarmCount; j++)
            {
                var placeholderHandler = new GenerationPlaceholderHandler().SetAlarmData(emptyAlarmData).SetAlarmNum(alarmNum++, tabConfig.AlarmNumFormat);

                if (tabConfig.GroupingType == AlarmGroupingType.ONE)
                {
                    segment = new SimaticLADSegment();
                    segment.Title[LocalizationVariables.CULTURE] = placeholderHandler.Parse(mainConfig.OneEachEmptyAlarmSegmentName);
                }

                ArgumentNullException.ThrowIfNull(segment, nameof(segment));
                FillAlarmSegment(tabConfig, segment, placeholderHandler, emptyAlarmData);
            }
        }

        private AlarmData ReplaceAlarmDataWithDefaultAndPrefix(AlarmTabConfiguration tabConfig, AlarmData alarmData)
        {
            return new AlarmData()
            {
                AlarmVariable = tabConfig.AlarmAddressPrefix + alarmData.AlarmVariable,
                Coil1Address = string.IsNullOrEmpty(alarmData.Coil1Address) ? tabConfig.DefaultCoil1Address : (tabConfig.CoilAddressPrefix + alarmData.Coil1Address),
                Coil2Address = string.IsNullOrEmpty(alarmData.Coil2Address) ? tabConfig.DefaultCoil2Address : (tabConfig.SetCoilAddressPrefix + alarmData.Coil2Address),
                TimerAddress = string.IsNullOrEmpty(alarmData.TimerAddress) ? tabConfig.DefaultTimerAddress : (tabConfig.TimerAddressPrefix + alarmData.TimerAddress),
                TimerType = string.IsNullOrEmpty(alarmData.TimerType) ? tabConfig.DefaultTimerType : alarmData.TimerType,
                TimerValue = string.IsNullOrEmpty(alarmData.TimerValue) ? tabConfig.DefaultTimerValue : alarmData.TimerValue,
                Description = alarmData.Description,
                Enable = alarmData.Enable
            };

        }

        private void FillAlarmSegment(AlarmTabConfiguration tabConfig, SimaticLADSegment segment, GenerationPlaceholderHandler placeholders, AlarmData alarmData)
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
                && alarmData.IsAddressValid(alarmData.TimerAddress))
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

            SimaticPart? coil1 = null;
            if (!string.IsNullOrEmpty(alarmData.Coil1Address) && alarmData.IsAddressValid(alarmData.Coil1Address))
            {
                var coil1Variable = new SimaticGlobalVariable(placeholders.Parse(alarmData.Coil1Address));
                coil1 = CreateCoil(tabConfig.Coil1Type, coil1Variable);
            }

            SimaticPart? coil2 = null;
            if (!string.IsNullOrEmpty(alarmData.Coil2Address) && alarmData.IsAddressValid(alarmData.Coil2Address))
            {
                var coil2Variable = new SimaticGlobalVariable(placeholders.Parse(alarmData.Coil2Address));
                coil2 = CreateCoil(tabConfig.Coil1Type, coil2Variable);
            }

            var simaticPartList = new List<SimaticPart?>
            {
                contact,
                timer,
                coil1,
                coil2
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
        
        private static SimaticPart? CreateCoil(AlarmCoilType coilType, SimaticVariable simaticVariable)
        {
            return coilType switch
            {
                AlarmCoilType.COIL => new CoilPart() { Operand = simaticVariable },
                AlarmCoilType.SET => new SetCoilPart() { Operand = simaticVariable },
                AlarmCoilType.RESET => new ResetCoilPart() { Operand = simaticVariable },
                _ => null,
            };
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

            foreach(var entry in alarmListDict)
            {
                var alarmTextPath = exportPath + $"/Texts_{entry.Key.Replace("\\", "_").Replace("/", "_")}.txt";

                using var stream = File.CreateText(alarmTextPath);
                stream.Write(entry.Value);
            }
        }

    }
}
