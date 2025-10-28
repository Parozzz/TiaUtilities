using ClosedXML.Excel;
using SimaticML.API;
using SimaticML.Blocks;
using SimaticML.Blocks.FlagNet;
using SimaticML.Blocks.FlagNet.nPart;
using SimaticML.Enums;
using TiaUtilities.Generation.Alarms.Module.Template;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Languages;

namespace TiaUtilities.Generation.Alarms
{

    public class AlarmGroupXmlItem(BlockFC blockFC, BlockUDT blockUDT, string alarmList, List<AlarmDataXmlItem> alarmDataItems)
    {
        public BlockFC BlockFC { get; init; } = blockFC;
        public BlockUDT BlockUDT { get; init; } = blockUDT;
        public string AlarmList { get; init; } = alarmList;
        public List<AlarmDataXmlItem> AlarmDataItems { get; init; } = alarmDataItems;
    }

    public class AlarmDataXmlItem(string tabName, string alarmVariableName, uint hmiID, string hmiAlarmName, string hmiAlarmText, string hmiAlarmClass, string hmiTriggerTag, uint hmiTriggerBit)
    {
        public string TabName { get; init; } = tabName;
        public string AlarmVariableName { get; init; } = alarmVariableName;

        public uint HmiID { get; init; } = hmiID;
        public string HmiAlarmName { get; init; } = hmiAlarmName;
        public string HmiAlarmText { get; init; } = hmiAlarmText;
        public string AlarmClass { get; init; } = hmiAlarmClass;
        public string TriggerTag { get; init; } = hmiTriggerTag;
        public uint TriggerBit { get; init; } = hmiTriggerBit;
    }

    public class AlarmXmlGenerator(AlarmMainConfiguration mainConfig)
    {
        private readonly AlarmMainConfiguration mainConfig = mainConfig;

        private readonly Dictionary<string, AlarmGroupXmlItem> alarmGroupDict = [];

        public void Init()
        {
        }

        public void GenerateAlarms(string tabName, AlarmTabConfiguration tabConfig, AlarmGenTemplateHandler templateHandler, List<DeviceData> deviceDataList)
        {
            AlarmGenPlaceholdersHandler placeholdersHandler = new(this.mainConfig, tabConfig);
            placeholdersHandler.TabName = tabName;

            BlockFC fc = new();
            fc.Init();
            fc.AttributeList.BlockName = placeholdersHandler.ParseNotNull(mainConfig.FCBlockName);
            fc.AttributeList.BlockNumber = mainConfig.FCBlockNumber;
            fc.AttributeList.AutoNumber = (mainConfig.FCBlockNumber > 0);

            List<AlarmDataXmlItem> hmiAlarmItems = [];
            var fullAlarmList = "";

            var segment = new SimaticLADSegment();

            var hmiID = tabConfig.HmiStartID;
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
                placeholdersHandler.LoadJSONObject(tabConfig.CustomPlaceholdersJSON);
                placeholdersHandler.TabName = tabName;

                if (deviceData.Placeholders != null)
                {
                    var placeholders = deviceData.Placeholders.Split(GenPlaceholders.Alarms.DEVICE_PLACEHOLDERS_GENERIC_SPLITTER);
                    for (int x = 0; x < placeholders.Length; x++)
                    {
                        placeholdersHandler.AddGenericPlaceholder(x + 1, placeholders[x]);
                    }
                }

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

                    var comment = placeholdersHandler.ParseNotNull(mainConfig.AlarmCommentTemplate);
                    fullAlarmList += $"{comment}'\n'";

                    hmiAlarmItems.Add(this.CreateHmiAlarmItem(ref hmiID, tabName, placeholdersHandler, alarmData, alarmNum, tabConfig));
                    
                    if (tabConfig.GroupingType == AlarmGroupingType.ONE)
                    {
                        segment = new SimaticLADSegment();
                        segment.Title[LocaleVariables.CULTURE] = placeholdersHandler.ParseNotNull(mainConfig.OneEachSegmentName);
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
                            var loopAlarmNum = (uint)(lastAlarmNum + x + 1);
                            placeholdersHandler.SetAlarmNum(loopAlarmNum, mainConfig.AlarmNumFormat);

                            hmiAlarmItems.Add(this.CreateHmiAlarmItem(ref hmiID, tabName, placeholdersHandler, alarmData: null, loopAlarmNum, tabConfig));
                            fullAlarmList += placeholdersHandler.ParseNotNull(mainConfig.AlarmCommentTemplateSpare) + '\n';
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

                for (uint x = 0; x < tabConfig.SkipNumberAfterGroup; x++)
                {
                    var loopAlarmNum = nextAlarmNum + x;

                    hmiAlarmItems.Add(this.CreateHmiAlarmItem(ref hmiID, tabName, placeholdersHandler, alarmData: null, loopAlarmNum, tabConfig));
                    fullAlarmList += '\n';
                }

                nextAlarmNum += tabConfig.SkipNumberAfterGroup;
            }

            GenerateEmptyAlarms(placeholdersHandler, tabConfig, nextAlarmNum, tabConfig.EmptyAlarmAtEnd);

            if (nextAlarmNum < (tabConfig.TotalAlarmNum + tabConfig.StartingAlarmNum))
            {
                placeholdersHandler.Clear(); 
                placeholdersHandler.LoadJSONObject(tabConfig.CustomPlaceholdersJSON);
                placeholdersHandler.TabName = tabName;

                for (uint x = nextAlarmNum; x < (tabConfig.TotalAlarmNum + tabConfig.StartingAlarmNum); x++)
                {
                    placeholdersHandler.SetAlarmNum(x, mainConfig.AlarmNumFormat);

                    var alarmItem = this.CreateHmiAlarmItem(ref hmiID, tabName, placeholdersHandler, alarmData: null, x, tabConfig, empty: true);
                    hmiAlarmItems.Add(alarmItem);

                    fullAlarmList += placeholdersHandler.ParseNotNull(mainConfig.AlarmCommentTemplateSpare) + '\n';
                }

            }

            BlockUDT blockUDT = new();
            blockUDT.Init();
            blockUDT.AttributeList.BlockName = placeholdersHandler.ParseNotNull(mainConfig.UDTBlockName);

            foreach (var hmiAlarmItem in hmiAlarmItems)
            {
                var member = blockUDT.AttributeList.NONE.AddMember(hmiAlarmItem.AlarmVariableName, SimaticDataType.BOOLEAN);
                member.Comment[LocaleVariables.CULTURE] = hmiAlarmItem.HmiAlarmText;
            }

            AlarmGroupXmlItem item = new(fc, blockUDT, fullAlarmList, hmiAlarmItems);
            this.alarmGroupDict.Add(tabName, item);
        }

        private AlarmDataXmlItem CreateHmiAlarmItem(ref uint ID, string tabName, AlarmGenPlaceholdersHandler placeholdersHandler, AlarmData? alarmData, uint alarmNum, AlarmTabConfiguration tabConfig, bool empty = false)
        {
            var alarmVariableName = placeholdersHandler.ParseNotNull(mainConfig.AlarmNameTemplate);

            var hmiAlarmName = placeholdersHandler.ParseNotNull(mainConfig.HmiNameTemplate);
            var hmiAlarmText = empty ? "" : placeholdersHandler.ParseNotNull(mainConfig.HmiTextTemplate);
            var hmiTriggerTag = placeholdersHandler.ParseNotNull(mainConfig.HmiTriggerTagTemplate);
            var hmiAlarmClass = placeholdersHandler.ParseNotNull(string.IsNullOrEmpty(alarmData?.HmiAlarmClass) ? tabConfig.DefaultHmiAlarmClass : alarmData?.HmiAlarmClass);

            AlarmDataXmlItem hmiItem;
            if (this.mainConfig.HmiTriggerTagUseWordArray)
            {
                var triggerByte = (alarmNum - 1) / 16;
                var triggerBit = (alarmNum - 1) % 16;
                hmiItem = new(tabName: tabName,
                    alarmVariableName: alarmVariableName,
                    hmiID: ID,
                    hmiAlarmName: hmiAlarmName,
                    hmiAlarmText: hmiAlarmText,
                    hmiAlarmClass: hmiAlarmClass,
                    hmiTriggerTag: hmiTriggerTag + $"[{triggerByte}]",
                    hmiTriggerBit: triggerBit);
            }
            else
            {
                hmiItem = new(tabName: tabName, 
                    alarmVariableName: alarmVariableName,
                    hmiID: ID,
                    hmiAlarmName: hmiAlarmName,
                    hmiAlarmText: hmiAlarmText,
                    hmiAlarmClass: hmiAlarmClass,
                    hmiTriggerTag: hmiTriggerTag,
                    hmiTriggerBit: 0);
            }

            ID++;
            return hmiItem;
        }

        private void GenerateEmptyAlarms(AlarmGenPlaceholdersHandler placeholdersHandler, AlarmTabConfiguration tabConfig, uint startAlarmNum, uint alarmCount, SimaticLADSegment? externalGroupSegment = null)
        {
            var emptyAlarmData = new AlarmData()
            {
                AlarmVariable = tabConfig.EmptyAlarmContactAddress,
                AlarmNegated = false,
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
                AlarmNegated = alarmData.AlarmNegated,
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
                },
                Negated = alarmData.AlarmNegated,
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

            XLWorkbook beta80Excel = new();
            var beta80Sheet = beta80Excel.Worksheets.Add("Allarmi");
            //CD_ERROR	DSC_ERROR	ERROR_TYPE	CD_GROUP	CD_CONVEYOR
            beta80Sheet.Cell(1, 1).Value = "CD_ERROR";
            beta80Sheet.Cell(1, 2).Value = "DSC_ERROR";
            beta80Sheet.Cell(1, 3).Value = "ERROR_TYPE";
            beta80Sheet.Cell(1, 4).Value = "CD_GROUP";
            beta80Sheet.Cell(1, 5).Value = "CD_CONVEYOR";

            XLWorkbook wb = new();
            var ws = wb.Worksheets.Add("DiscreteAlarms");
            ws.Cell(1, 1).Value = "ID";
            ws.Cell(1, 2).Value = "Name";
            ws.Cell(1, 3).Value = "Class";
            ws.Cell(1, 4).Value = $"Alarm text [{LocaleVariables.CULTURE.IetfLanguageTag}], Alarm text 1";
            ws.Cell(1, 5).Value = "Trigger tag";
            ws.Cell(1, 6).Value = "Trigger bit";

            int excelRowIndex = 2; //Starts from 1 and the first is the headers.

            foreach (var (alarmGroupName, alarmGroupItem) in alarmGroupDict)
            {
                var blockFC = alarmGroupItem.BlockFC;
                var fcXmlDocument = SimaticMLAPI.CreateDocument(blockFC);
                fcXmlDocument.Save(exportPath + $"/[FC]{alarmGroupName}_{blockFC.AttributeList.BlockName}.xml");

                var blockUDT = alarmGroupItem.BlockUDT;
                var udtXmlDocument = SimaticMLAPI.CreateDocument(blockUDT);
                udtXmlDocument.Save(exportPath + $"/[UDT]{alarmGroupName}_{blockUDT.AttributeList.BlockName}.xml");

                using var stream = File.CreateText(exportPath + $"/Texts_{alarmGroupName.Replace("\\", "_").Replace("/", "_")}.txt");
                stream.Write(alarmGroupItem.AlarmList);

                var beta80AlarmNum = 1;

                foreach (var alarmDataItem in alarmGroupItem.AlarmDataItems)
                {
                    ws.Cell(excelRowIndex, 1).Value = alarmDataItem.HmiID;
                    ws.Cell(excelRowIndex, 2).Value = alarmDataItem.HmiAlarmName;
                    ws.Cell(excelRowIndex, 3).Value = alarmDataItem.AlarmClass;
                    ws.Cell(excelRowIndex, 4).Value = alarmDataItem.HmiAlarmText;
                    ws.Cell(excelRowIndex, 5).Value = alarmDataItem.TriggerTag;
                    ws.Cell(excelRowIndex, 6).Value = alarmDataItem.TriggerBit;

                    if(!string.IsNullOrEmpty(alarmDataItem.HmiAlarmText))
                    {
                        beta80Sheet.Cell(excelRowIndex, 1).Value = beta80AlarmNum;
                        beta80Sheet.Cell(excelRowIndex, 2).Value = alarmDataItem.HmiAlarmText;
                        beta80Sheet.Cell(excelRowIndex, 3).Value = 4;
                        beta80Sheet.Cell(excelRowIndex, 4).Value = alarmDataItem.TabName.Replace("Z", "");
                        beta80Sheet.Cell(excelRowIndex, 5).Value = 1;
                    }

                    beta80AlarmNum++;
                    excelRowIndex++;
                }

            }

            var hmiAlarmsPath = exportPath + $"/HmiAlarms.xlsx";
            wb.SaveAs(hmiAlarmsPath);

            var beta80Path = exportPath + $"/Beta80Alarms.xlsx";
            beta80Excel.SaveAs(beta80Path);
        }
    }
}
