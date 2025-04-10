using SimaticML.API;
using SimaticML.Blocks;
using SimaticML.Blocks.FlagNet;
using SimaticML.Blocks.FlagNet.nPart;
using SimaticML.Enums;
using TiaUtilities.Generation.Alarms;
using TiaUtilities.Generation.Alarms.Module.Template;
using TiaUtilities.Generation.Placeholders;
using TiaXmlReader.Generation.Placeholders;
using TiaXmlReader.Languages;

namespace TiaXmlReader.Generation.Alarms
{

    public class AlarmXmlGenerator(AlarmMainConfiguration mainConfig)
    {
        private readonly AlarmMainConfiguration mainConfig = mainConfig;

        private readonly Dictionary<string, BlockFC> fcDict = [];
        private readonly Dictionary<string, string> alarmListDict = [];

        public void Init()
        {
        }

        public void GenerateAlarms(string name, AlarmTabConfiguration tabConfig, AlarmGenTemplateHandler templateHandler, List<DeviceData> deviceDataList)
        {
            AlarmGenPlaceholdersHandler placeholdersHandler = new(this.mainConfig, tabConfig);
            placeholdersHandler.TabName = name;

            BlockFC fc = new();
            fc.Init();
            fc.AttributeList.BlockName = placeholdersHandler.ParseNotNull(mainConfig.FCBlockName);
            fc.AttributeList.BlockNumber = mainConfig.FCBlockNumber;
            fc.AttributeList.AutoNumber = (mainConfig.FCBlockNumber > 0);

            var fullAlarmList = "";

            var segment = new SimaticLADSegment();

            var nextAlarmNum = tabConfig.StartingAlarmNum;
            //Switching to a Template system broke the ability to use the Partition system of the alarms (AlarmPartitionType.DEVICE / ALARM_TYPE)
            foreach (var deviceData in deviceDataList)
            {
                if (tabConfig.GroupingType == AlarmGroupingType.GROUP)
                {
                    segment = new SimaticLADSegment();
                }

                var template = templateHandler.FindTemplate(deviceData.Template);
                if (template == null)
                {
                    continue;
                }

                var alarmDataList = template.AlarmGridSave?.RowData.Values;
                if (alarmDataList == null || alarmDataList.Count == 0)
                {
                    continue;
                }

                placeholdersHandler.Clear();
                placeholdersHandler.TabName = name;

                var startAlarmNum = nextAlarmNum;
                foreach (var alarmData in alarmDataList)
                {
                    if (!alarmData.Enable)
                    {
                        continue;
                    }

                    var parsedAlarmData = ReplaceAlarmDataWithDefaultAndPrefix(tabConfig, template.TemplateConfig, alarmData);

                    placeholdersHandler.DeviceData = deviceData;
                    placeholdersHandler.AlarmData = parsedAlarmData;
                    placeholdersHandler.SetAlarmNum(nextAlarmNum++, tabConfig.AlarmNumFormat);
                    fullAlarmList += placeholdersHandler.Parse(this.mainConfig.AlarmTextInList) + '\n';

                    if (tabConfig.GroupingType == AlarmGroupingType.ONE)
                    {
                        segment = new SimaticLADSegment();
                        segment.Title[LocaleVariables.CULTURE] = placeholdersHandler.Parse(mainConfig.OneEachSegmentName);
                    }

                    FillAlarmSegment(tabConfig, segment, placeholdersHandler, parsedAlarmData);

                    if (tabConfig.GroupingType == AlarmGroupingType.ONE)
                    {
                        segment.Create(fc);
                    }
                }

                var lastAlarmNum = nextAlarmNum - 1;
                var alarmCount = (lastAlarmNum - startAlarmNum) + 1;

                if (tabConfig.AntiSlipNumber > 0)
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

                    if (slippingAlarmCount > 0)
                    {
                        nextAlarmNum += slippingAlarmCount;

                        for (var x = 0; x < slippingAlarmCount; x++)
                        {
                            placeholdersHandler.SetAlarmNum((uint)(lastAlarmNum + x + 1), tabConfig.AlarmNumFormat);
                            fullAlarmList += placeholdersHandler.Parse(this.mainConfig.EmptyAlarmTextInList) + '\n';
                        }

                        if (tabConfig.GenerateEmptyAlarmAntiSlip)
                        {
                            GenerateEmptyAlarms(placeholdersHandler, tabConfig, lastAlarmNum + 1, slippingAlarmCount, segment); //CompileUnit only used for group division
                            lastAlarmNum += slippingAlarmCount;
                        }
                    }
                }

                if (tabConfig.GroupingType == AlarmGroupingType.GROUP)
                {
                    placeholdersHandler.SetStartEndAlarmNum(startAlarmNum, lastAlarmNum, tabConfig.AlarmNumFormat);

                    segment.Title[LocaleVariables.CULTURE] = placeholdersHandler.Parse(mainConfig.GroupSegmentName);
                    segment.Create(fc);
                }

                nextAlarmNum += tabConfig.SkipNumberAfterGroup;
                for (var x = 0; x < tabConfig.SkipNumberAfterGroup; x++)
                {
                    fullAlarmList += '\n';
                }
            }

            GenerateEmptyAlarms(placeholdersHandler, tabConfig, nextAlarmNum, tabConfig.EmptyAlarmAtEnd);

            fcDict.Add(name, fc);
            alarmListDict.Add(name, fullAlarmList);
        }

        private void GenerateEmptyAlarms(AlarmGenPlaceholdersHandler placeholdersHandler, AlarmTabConfiguration tabConfig, uint startAlarmNum, uint alarmCount, SimaticLADSegment? externalGroupSegment = null)
        {
            var emptyAlarmData = new AlarmData()
            {
                AlarmVariable = tabConfig.EmptyAlarmContactAddress,
                Coil1Address = tabConfig.DefaultCoil1Address,
                Coil1Type = tabConfig.DefaultCoil1Type.ToString(),
                Coil2Address = tabConfig.DefaultCoil2Address,
                Coil2Type = tabConfig.DefaultCoil2Type.ToString(),
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
                placeholdersHandler.AlarmData = emptyAlarmData;
                placeholdersHandler.SetStartEndAlarmNum(alarmNum, alarmNum + (alarmCount - 1), tabConfig.AlarmNumFormat);

                segment = new SimaticLADSegment();
                segment.Title[LocaleVariables.CULTURE] = placeholdersHandler.Parse(mainConfig.GroupEmptyAlarmSegmentName);
            }

            for (int j = 0; j < alarmCount; j++)
            {
                placeholdersHandler.AlarmData = emptyAlarmData;
                placeholdersHandler.SetAlarmNum(alarmNum++, tabConfig.AlarmNumFormat);

                if (tabConfig.GroupingType == AlarmGroupingType.ONE)
                {
                    segment = new SimaticLADSegment();
                    segment.Title[LocaleVariables.CULTURE] = placeholdersHandler.Parse(mainConfig.OneEachEmptyAlarmSegmentName);
                }

                ArgumentNullException.ThrowIfNull(segment, nameof(segment));
                FillAlarmSegment(tabConfig, segment, placeholdersHandler, emptyAlarmData);
            }
        }

        private static AlarmData ReplaceAlarmDataWithDefaultAndPrefix(AlarmTabConfiguration tabConfig, AlarmTemplateConfiguration templateConfig, AlarmData alarmData)
        {
            AlarmCoilType coil1Type = tabConfig.DefaultCoil1Type;
            if (Enum.TryParse(alarmData.Coil1Type, out AlarmCoilType res1))
            {
                coil1Type = res1;
            }

            AlarmCoilType coil2Type = tabConfig.DefaultCoil2Type;
            if (Enum.TryParse(alarmData.Coil2Type, out AlarmCoilType res2))
            {
                coil2Type = res2;
            }

            return new AlarmData()
            {
                AlarmVariable = (templateConfig.StandaloneAlarms ? "" : tabConfig.AlarmAddressPrefix) + alarmData.AlarmVariable,
                CustomVariableAddress = string.IsNullOrEmpty(alarmData.CustomVariableAddress) ? tabConfig.DefaultCustomVarAddress : alarmData.CustomVariableAddress,
                CustomVariableValue = string.IsNullOrEmpty(alarmData.CustomVariableValue) ? tabConfig.DefaultCustomVarValue : alarmData.CustomVariableValue,
                Coil1Address = string.IsNullOrEmpty(alarmData.Coil1Address) ? tabConfig.DefaultCoil1Address : (tabConfig.Coil1AddressPrefix + alarmData.Coil1Address),
                Coil1Type = coil1Type.ToString(),
                Coil2Address = string.IsNullOrEmpty(alarmData.Coil2Address) ? tabConfig.DefaultCoil2Address : (tabConfig.Coil2AddressPrefix + alarmData.Coil2Address),
                Coil2Type = coil2Type.ToString(),
                TimerAddress = string.IsNullOrEmpty(alarmData.TimerAddress) ? tabConfig.DefaultTimerAddress : (tabConfig.TimerAddressPrefix + alarmData.TimerAddress),
                TimerType = string.IsNullOrEmpty(alarmData.TimerType) ? tabConfig.DefaultTimerType : alarmData.TimerType,
                TimerValue = string.IsNullOrEmpty(alarmData.TimerValue) ? tabConfig.DefaultTimerValue : alarmData.TimerValue,
                Description = alarmData.Description,
                Enable = alarmData.Enable
            };

        }

        private void FillAlarmSegment(AlarmTabConfiguration tabConfig, SimaticLADSegment segment, GenPlaceholderHandler placeholders, AlarmData alarmData)
        {
            if (string.IsNullOrEmpty(alarmData.AlarmVariable))
            {
                return;
            }

            var parsedContactAddress = placeholders.Parse(alarmData.AlarmVariable);
            SimaticPart contact = new ContactPart()
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

            var parsedCustomVarAddress = placeholders.Parse(alarmData.CustomVariableAddress);
            var parsedCustomVarValue = placeholders.Parse(alarmData.CustomVariableValue);
            if (mainConfig.EnableCustomVariable &&
                !string.IsNullOrEmpty(parsedCustomVarAddress) && AlarmData.IsAddressValid(parsedCustomVarAddress) &&
                !string.IsNullOrEmpty(parsedCustomVarValue) && AlarmData.IsAddressValid(parsedCustomVarValue))
            {

                SimaticVariable inVar;
                if (parsedCustomVarValue.Contains('.') && float.TryParse(parsedCustomVarValue, out _))
                {
                    inVar = new SimaticLiteralConstant(SimaticDataType.REAL, parsedCustomVarValue);
                }
                else if (long.TryParse(parsedCustomVarValue, out _))
                {
                    inVar = new SimaticLiteralConstant(SimaticDataType.DINT, parsedCustomVarValue);
                }
                else
                {
                    inVar = new SimaticTypedConstant(parsedCustomVarValue);
                }

                contact.Branch(new MovePart()
                {
                    IN = inVar,
                    OUT = { new SimaticGlobalVariable(parsedCustomVarAddress) }
                });
            }

            TimerPart? timer = null;
            if (mainConfig.EnableTimer &&
                !string.IsNullOrEmpty(alarmData.TimerAddress) &&
                !string.IsNullOrEmpty(alarmData.TimerType) &&
                !string.IsNullOrEmpty(alarmData.TimerValue) &&
                AlarmData.IsAddressValid(alarmData.TimerAddress))
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
            if (!string.IsNullOrEmpty(alarmData.Coil1Address) && AlarmData.IsAddressValid(alarmData.Coil1Address))
            {
                var coilVariable = new SimaticGlobalVariable(placeholders.Parse(alarmData.Coil1Address));

                AlarmCoilType coilType = tabConfig.DefaultCoil1Type;
                if (Enum.TryParse(alarmData.Coil1Type, ignoreCase: true, out AlarmCoilType result))
                {
                    coilType = result;
                }

                coil1 = CreateCoil(coilType, coilVariable);
            }

            SimaticPart? coil2 = null;
            if (!string.IsNullOrEmpty(alarmData.Coil2Address) && AlarmData.IsAddressValid(alarmData.Coil2Address))
            {
                var coilVariable = new SimaticGlobalVariable(placeholders.Parse(alarmData.Coil2Address));

                AlarmCoilType coilType = tabConfig.DefaultCoil2Type;
                if (Enum.TryParse(alarmData.Coil2Type, ignoreCase: true, out AlarmCoilType result))
                {
                    coilType = result;
                }

                coil2 = CreateCoil(coilType, coilVariable);
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
                if (simaticPart == null)
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
                AlarmCoilType.NCOIL => new CoilPart() { Operand = simaticVariable, Negated = true },
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

            foreach (var entry in fcDict)
            {
                var name = entry.Key;
                var fc = entry.Value;

                var xmlDocument = SimaticMLAPI.CreateDocument(fc);
                xmlDocument.Save(exportPath + $"/[FC]{name}_{fc.AttributeList.BlockName}.xml");
            }

            foreach (var entry in alarmListDict)
            {
                var alarmTextPath = exportPath + $"/Texts_{entry.Key.Replace("\\", "_").Replace("/", "_")}.txt";

                using var stream = File.CreateText(alarmTextPath);
                stream.Write(entry.Value);
            }
        }

    }
}
