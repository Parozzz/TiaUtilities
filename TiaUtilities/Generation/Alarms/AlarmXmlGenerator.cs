using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Vml.Office;
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

    public class AlarmXmlItem(BlockFC blockFC, BlockUDT blockUDT, string alarmList, Dictionary<uint, string?> alarmDescriptionDict)
    {
        public BlockFC BlockFC { get; init; } = blockFC;
        public BlockUDT BlockUDT { get; init; } = blockUDT;
        public string AlarmList { get; init; } = alarmList;
        public Dictionary<uint, string?> AlarmDescriptionDict { get; init; } = alarmDescriptionDict;
    }

    public class AlarmXmlGenerator(AlarmMainConfiguration mainConfig)
    {
        private readonly AlarmMainConfiguration mainConfig = mainConfig;

        private readonly Dictionary<string, AlarmXmlItem> itemDict = [];

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

            Dictionary<uint, string?> alarmDescriptionDict = [];
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

                    var alarmNum = nextAlarmNum;
                    nextAlarmNum++;

                    var parsedAlarmData = ReplaceAlarmDataWithDefaultAndPrefix(tabConfig, template.TemplateConfig, alarmData);

                    placeholdersHandler.DeviceData = deviceData;
                    placeholdersHandler.AlarmData = parsedAlarmData;
                    placeholdersHandler.SetAlarmNum(alarmNum, mainConfig.AlarmNumFormat);

                    var comment = placeholdersHandler.Parse(mainConfig.AlarmCommentTemplate);
                    comment = placeholdersHandler.Parse(comment);

                    alarmDescriptionDict.Add(alarmNum, comment);
                    fullAlarmList += $"{comment}'\n'";

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
                            placeholdersHandler.SetAlarmNum((uint)(lastAlarmNum + x + 1), mainConfig.AlarmNumFormat);
                            fullAlarmList += placeholdersHandler.Parse(mainConfig.AlarmCommentTemplateSpare) + '\n';
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
                    placeholdersHandler.SetStartEndAlarmNum(startAlarmNum, lastAlarmNum, mainConfig.AlarmNumFormat);

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

            BlockUDT blockUDT = new();
            blockUDT.Init();
            blockUDT.AttributeList.BlockName = placeholdersHandler.Parse(mainConfig.UDTBlockName) ?? $"INVALID_{name}";

            placeholdersHandler.Clear();
            placeholdersHandler.TabName = name;
            for (uint alarmNum = 1; alarmNum <= tabConfig.TotalAlarmNum; alarmNum++)
            {
                placeholdersHandler.SetAlarmNum(alarmNum, mainConfig.AlarmNumFormat);

                var getDone = alarmDescriptionDict.TryGetValue(alarmNum, out string? almDescription);
                if (!getDone)
                {
                    almDescription = placeholdersHandler.Parse(mainConfig.AlarmCommentTemplateSpare);
                }

                var alarmName = placeholdersHandler.Parse(mainConfig.AlarmNameTemplate) ?? $"INVALID_{alarmNum}";

                var member = blockUDT.AttributeList.NONE.AddMember(alarmName, SimaticDataType.BOOLEAN);
                member.Comment[LocaleVariables.CULTURE] = almDescription;
            }


            AlarmXmlItem item = new(fc, blockUDT, fullAlarmList, alarmDescriptionDict);
            this.itemDict.Add(name, item);
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
                placeholdersHandler.SetStartEndAlarmNum(alarmNum, alarmNum + (alarmCount - 1), mainConfig.AlarmNumFormat);

                segment = new SimaticLADSegment();
                segment.Title[LocaleVariables.CULTURE] = placeholdersHandler.Parse(mainConfig.GroupEmptyAlarmSegmentName);
            }

            for (int j = 0; j < alarmCount; j++)
            {
                placeholdersHandler.AlarmData = emptyAlarmData;
                placeholdersHandler.SetAlarmNum(alarmNum++, mainConfig.AlarmNumFormat);

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

            foreach (var (name, item) in itemDict)
            {
                var blockFC = item.BlockFC;
                var blockUDT = item.BlockUDT;
                var alarmList = item.AlarmList;

                var fcXmlDocument = SimaticMLAPI.CreateDocument(blockFC);
                fcXmlDocument.Save(exportPath + $"/[FC]{name}_{blockFC.AttributeList.BlockName}.xml");

                var udtXmlDocument = SimaticMLAPI.CreateDocument(blockUDT);
                udtXmlDocument.Save(exportPath + $"/[UDT]{name}_{blockUDT.AttributeList.BlockName}.xml");

                var alarmTextPath = exportPath + $"/Texts_{name.Replace("\\", "_").Replace("/", "_")}.txt";
                using var stream = File.CreateText(alarmTextPath);
                stream.Write(alarmList);
            }
        }

    }
}
