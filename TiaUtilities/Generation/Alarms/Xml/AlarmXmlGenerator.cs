using ClosedXML.Excel;
using SimaticML.API;
using SimaticML.Blocks;
using SimaticML.Blocks.FlagNet;
using SimaticML.Blocks.FlagNet.nPart;
using SimaticML.Enums;
using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Generation.Alarms.Data;
using TiaUtilities.Generation.Alarms.Module.Template;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Languages;

namespace TiaUtilities.Generation.Alarms.Xml
{

    public class AlarmGroupXmlItem(BlockFC blockFC, BlockUDT blockUDT, string alarmList, List<AlarmXmlItem> items)
    {
        public BlockFC BlockFC { get; init; } = blockFC;
        public BlockUDT BlockUDT { get; init; } = blockUDT;
        public string AlarmList { get; init; } = alarmList;
        public List<AlarmXmlItem> Items { get; init; } = items;
    }

    public class AlarmXmlGenerator(AlarmMainConfiguration mainConfig)
    {
        private readonly AlarmMainConfiguration mainConfig = mainConfig;
        private readonly Dictionary<string, AlarmGroupXmlItem> alarmGroupDict = [];

        public void GenerateAlarms(string tabName, AlarmTabConfiguration tabConfig, AlarmGenTemplateHandler templateHandler, List<DeviceData> deviceDataList)
        {
            AlarmGenPlaceholdersHandler placeholdersHandler = new(this.mainConfig, tabConfig)
            {
                TabName = tabName
            };

            BlockFC fc = new();
            fc.Init();
            fc.AttributeList.BlockName = placeholdersHandler.ParseNotNull(mainConfig.FCBlockName);
            fc.AttributeList.BlockNumber = mainConfig.FCBlockNumber;
            fc.AttributeList.AutoNumber = (mainConfig.FCBlockNumber > 0);

            List<AlarmXmlItem> items = [];
            var fullAlarmList = "\'";

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

                var templateDataList = template.AlarmGridSave?.RowData.Values;
                if (templateDataList == null || templateDataList.Count == 0)
                {
                    continue;
                }

                placeholdersHandler.Clear();
                placeholdersHandler.LoadJSONObject(tabConfig.CustomPlaceholdersJSON);
                placeholdersHandler.TabName = tabName;

                if (deviceData.Placeholders != null)
                {
                    var placeholders = deviceData.Placeholders.Split(GenPlaceholders.Alarms.DEVICE_PLACEHOLDERS_GENERIC_SPLITTER);

                    int count = placeholders.Length;
                    for (int x = 0; x < count; x++)
                    {
                        placeholdersHandler.AddGenericPlaceholder(x + 1, placeholders[x]);
                    }

                    //A little hack to allow empty placeholder to be "Removed"
                    for(int x = count; x < 50; x++)
                    {
                        placeholdersHandler.AddGenericPlaceholder(x + 1, "");
                    }
                }

                var startAlarmNum = nextAlarmNum;
                foreach (var templateData in templateDataList)
                {
                    if (!templateData.Enable)
                    {
                        continue;
                    }

                    var alarmNum = nextAlarmNum;
                    nextAlarmNum++;

                    var parsedTemplateData = ReplaceTemplateDataWithDefaultAndPrefix(tabConfig, template.TemplateConfig, templateData);

                    placeholdersHandler.DeviceData = deviceData;
                    placeholdersHandler.TemplateData = parsedTemplateData;
                    placeholdersHandler.SetAlarmNum(alarmNum, mainConfig.AlarmNumFormat);

                    //Creation of AlarmItem for FULL Alarm
                    var comment = placeholdersHandler.ParseNotNull(mainConfig.AlarmCommentTemplate);

                    var alarmItem = this.CreateItem(ref hmiID, tabName, comment, placeholdersHandler, templateData, alarmNum, tabConfig);
                    items.Add(alarmItem);

                    if (tabConfig.GroupingType == AlarmGroupingType.ONE)
                    {
                        segment = new SimaticLADSegment();
                        segment.Title[LocaleVariables.CULTURE] = placeholdersHandler.ParseNotNull(mainConfig.OneEachSegmentName);
                    }

                    FillAlarmSegment(tabConfig, segment, placeholdersHandler, parsedTemplateData);

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

                            //Creation of EMPTY Alarm item for slipping
                            var comment = placeholdersHandler.ParseNotNull(mainConfig.AlarmCommentTemplateSpare);

                            var alarmItem = this.CreateItem(ref hmiID, tabName, comment, placeholdersHandler, templateData: null, loopAlarmNum, tabConfig);
                            items.Add(alarmItem);
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
                    //Creation of EMPTY Alarm for Skip After Group
                    var comment = "";

                    var alarmItem = this.CreateItem(ref hmiID, tabName, comment, placeholdersHandler, templateData: null, loopAlarmNum, tabConfig);
                    items.Add(alarmItem);
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

                    //Create of SPARE alarm from total alarms number
                    var comment = placeholdersHandler.ParseNotNull(mainConfig.AlarmCommentTemplateSpare);

                    var alarmItem = this.CreateItem(ref hmiID, tabName, comment, placeholdersHandler, templateData: null, x, tabConfig, empty: true);
                    items.Add(alarmItem);
                }

            }

            BlockUDT blockUDT = new();
            blockUDT.Init();
            blockUDT.AttributeList.BlockName = placeholdersHandler.ParseNotNull(mainConfig.UDTBlockName);

            foreach (var alarmItem in items)
            {
                var member = blockUDT.AttributeList.NONE.AddMember(alarmItem.AlarmVariableName, SimaticDataType.BOOLEAN);
                member.Comment[LocaleVariables.CULTURE] = alarmItem.AlarmVariableComment;

                fullAlarmList += $"{alarmItem.AlarmVariableComment}'\n'";
            }

            AlarmGroupXmlItem item = new(fc, blockUDT, fullAlarmList, items);
            this.alarmGroupDict.Add(tabName, item);
        }

        private AlarmXmlItem CreateItem(ref uint ID, string tabName, string comment, AlarmGenPlaceholdersHandler placeholdersHandler, TemplateData? templateData, uint alarmNum, AlarmTabConfiguration tabConfig, bool empty = false)
        {
            var alarmVariableName = placeholdersHandler.ParseNotNull(mainConfig.AlarmNameTemplate);
            var alarmVariableComment = comment;

            var hmiAlarmName = placeholdersHandler.ParseNotNull(mainConfig.HmiNameTemplate);
            var hmiAlarmText = empty ? "" : placeholdersHandler.ParseNotNull(mainConfig.HmiTextTemplate);
            var hmiTriggerTag = placeholdersHandler.ParseNotNull(mainConfig.HmiTriggerTagTemplate);
            var hmiAlarmClass = placeholdersHandler.ParseNotNull(string.IsNullOrEmpty(templateData?.HmiAlarmClass) ? tabConfig.DefaultHmiAlarmClass : templateData?.HmiAlarmClass);

            AlarmXmlItem item;
            if (this.mainConfig.HmiTriggerTagUseWordArray)
            {
                var triggerByte = (alarmNum - 1) / 16;
                var triggerBit = (alarmNum - 1) % 16;
                item = new(tabName,
                    alarmVariableName,
                    alarmVariableComment,
                    ID,
                    hmiAlarmName,
                    hmiAlarmText,
                    hmiAlarmClass,
                    hmiTriggerTag + $"[{triggerByte}]",
                    triggerBit);
            }
            else
            {
                item = new(tabName, 
                    alarmVariableName,
                    alarmVariableComment,
                    ID,
                    hmiAlarmName,
                    hmiAlarmText,
                    hmiAlarmClass,
                    hmiTriggerTag,
                    hmiTriggerBit: 0);
            }

            ID++;
            return item;
        }

        private void GenerateEmptyAlarms(AlarmGenPlaceholdersHandler placeholdersHandler, AlarmTabConfiguration tabConfig, uint startAlarmNum, uint alarmCount, SimaticLADSegment? externalGroupSegment = null)
        {
            var emptyTemplateData = new TemplateData()
            {
                AlarmVariable = tabConfig.EmptyAlarmContactAddress,
                AlarmNegated = false,
                Coil1Address = tabConfig.DefaultCoil1Address,
                Coil1Type = tabConfig.DefaultCoil1Type.ToString(),
                Coil2Address = tabConfig.DefaultCoil2Address,
                Coil2Type = tabConfig.DefaultCoil2Type.ToString(),
                TimerAddress = tabConfig.DefaultTimerAddress,
                TimerType = tabConfig.DefaultTimerType,
                TimerValue = tabConfig.DefaultTimerValue,
                Description = "",
                Enable = true
            };

            var alarmNum = startAlarmNum;

            SimaticLADSegment? segment = externalGroupSegment;
            if (segment == null && tabConfig.GroupingType == AlarmGroupingType.GROUP)
            {
                placeholdersHandler.TemplateData = emptyTemplateData;
                placeholdersHandler.SetStartEndAlarmNum(alarmNum, alarmNum + (alarmCount - 1), mainConfig.AlarmNumFormat);

                segment = new SimaticLADSegment();
                segment.Title[LocaleVariables.CULTURE] = placeholdersHandler.Parse(mainConfig.GroupEmptyAlarmSegmentName);
            }

            for (int j = 0; j < alarmCount; j++)
            {
                placeholdersHandler.TemplateData = emptyTemplateData;
                placeholdersHandler.SetAlarmNum(alarmNum++, mainConfig.AlarmNumFormat);

                if (tabConfig.GroupingType == AlarmGroupingType.ONE)
                {
                    segment = new SimaticLADSegment();
                    segment.Title[LocaleVariables.CULTURE] = placeholdersHandler.Parse(mainConfig.OneEachEmptyAlarmSegmentName);
                }

                ArgumentNullException.ThrowIfNull(segment, nameof(segment));
                FillAlarmSegment(tabConfig, segment, placeholdersHandler, emptyTemplateData);
            }
        }

        private static TemplateData ReplaceTemplateDataWithDefaultAndPrefix(AlarmTabConfiguration tabConfig, AlarmTemplateConfiguration templateConfig, TemplateData templateData)
        {
            AlarmCoilType coil1Type = tabConfig.DefaultCoil1Type;
            if (Enum.TryParse(templateData.Coil1Type, out AlarmCoilType res1))
            {
                coil1Type = res1;
            }

            AlarmCoilType coil2Type = tabConfig.DefaultCoil2Type;
            if (Enum.TryParse(templateData.Coil2Type, out AlarmCoilType res2))
            {
                coil2Type = res2;
            }

            return new TemplateData()
            {
                AlarmVariable = (templateConfig.StandaloneAlarms ? "" : tabConfig.AlarmAddressPrefix) + templateData.AlarmVariable,
                AlarmNegated = templateData.AlarmNegated,
                CustomVariableAddress = string.IsNullOrEmpty(templateData.CustomVariableAddress) ? tabConfig.DefaultCustomVarAddress : templateData.CustomVariableAddress,
                CustomVariableValue = string.IsNullOrEmpty(templateData.CustomVariableValue) ? tabConfig.DefaultCustomVarValue : templateData.CustomVariableValue,
                Coil1Address = string.IsNullOrEmpty(templateData.Coil1Address) ? tabConfig.DefaultCoil1Address : (tabConfig.Coil1AddressPrefix + templateData.Coil1Address),
                Coil1Type = coil1Type.ToString(),
                Coil2Address = string.IsNullOrEmpty(templateData.Coil2Address) ? tabConfig.DefaultCoil2Address : (tabConfig.Coil2AddressPrefix + templateData.Coil2Address),
                Coil2Type = coil2Type.ToString(),
                TimerAddress = string.IsNullOrEmpty(templateData.TimerAddress) ? tabConfig.DefaultTimerAddress : (tabConfig.TimerAddressPrefix + templateData.TimerAddress),
                TimerType = string.IsNullOrEmpty(templateData.TimerType) ? tabConfig.DefaultTimerType : templateData.TimerType,
                TimerValue = string.IsNullOrEmpty(templateData.TimerValue) ? tabConfig.DefaultTimerValue : templateData.TimerValue,
                Description = templateData.Description,
                Enable = templateData.Enable
            };

        }

        private void FillAlarmSegment(AlarmTabConfiguration tabConfig, SimaticLADSegment segment, GenPlaceholderHandler placeholders, TemplateData templateData)
        {
            if (string.IsNullOrEmpty(templateData.AlarmVariable))
            {
                return;
            }

            var parsedContactAddress = placeholders.Parse(templateData.AlarmVariable);
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
                Negated = templateData.AlarmNegated,
            };

            var parsedCustomVarAddress = placeholders.Parse(templateData.CustomVariableAddress);
            var parsedCustomVarValue = placeholders.Parse(templateData.CustomVariableValue);
            if (mainConfig.EnableCustomVariable &&
                !string.IsNullOrEmpty(parsedCustomVarAddress) && TemplateData.IsAddressValid(parsedCustomVarAddress) &&
                !string.IsNullOrEmpty(parsedCustomVarValue) && TemplateData.IsAddressValid(parsedCustomVarValue))
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
                !string.IsNullOrEmpty(templateData.TimerAddress) &&
                !string.IsNullOrEmpty(templateData.TimerType) &&
                !string.IsNullOrEmpty(templateData.TimerValue) &&
                TemplateData.IsAddressValid(templateData.TimerAddress))
            {
                var partType = templateData.TimerType.ToLower() switch
                {
                    "ton" => PartType.TON,
                    "tof" => PartType.TOF,
                    _ => throw new Exception("Unknow timer type of " + templateData.TimerType),
                };

                timer = new TimerPart(partType)
                {
                    InstanceScope = SimaticVariableScope.GLOBAL_VARIABLE,
                    InstanceAddress = placeholders.Parse(templateData.TimerAddress),
                    PT = new SimaticTypedConstant(templateData.TimerValue),
                };
            }

            SimaticPart? coil1 = null;
            if (!string.IsNullOrEmpty(templateData.Coil1Address) && TemplateData.IsAddressValid(templateData.Coil1Address))
            {
                var coilVariable = new SimaticGlobalVariable(placeholders.Parse(templateData.Coil1Address));

                AlarmCoilType coilType = tabConfig.DefaultCoil1Type;
                if (Enum.TryParse(templateData.Coil1Type, ignoreCase: true, out AlarmCoilType result))
                {
                    coilType = result;
                }

                coil1 = CreateCoil(coilType, coilVariable);
            }

            SimaticPart? coil2 = null;
            if (!string.IsNullOrEmpty(templateData.Coil2Address) && TemplateData.IsAddressValid(templateData.Coil2Address))
            {
                var coilVariable = new SimaticGlobalVariable(placeholders.Parse(templateData.Coil2Address));

                AlarmCoilType coilType = tabConfig.DefaultCoil2Type;
                if (Enum.TryParse(templateData.Coil2Type, ignoreCase: true, out AlarmCoilType result))
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

            AlarmXmlHmiAlarmsExcel hmiAlarmsExcel = new();

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

                foreach (var item in alarmGroupItem.Items)
                {
                    hmiAlarmsExcel.AddData(item);

                    if(!string.IsNullOrEmpty(item.HmiAlarmText))
                    {
                        beta80Sheet.Cell(excelRowIndex, 1).Value = beta80AlarmNum;
                        beta80Sheet.Cell(excelRowIndex, 2).Value = item.HmiAlarmText;
                        beta80Sheet.Cell(excelRowIndex, 3).Value = 4;
                        beta80Sheet.Cell(excelRowIndex, 4).Value = item.TabName.Replace("Z", "");
                        beta80Sheet.Cell(excelRowIndex, 5).Value = 1;
                    }

                    beta80AlarmNum++;
                    excelRowIndex++;
                }

            }

            var hmiAlarmsPath = $"{exportPath}/HmiAlarms.xlsx";
            hmiAlarmsExcel.SaveAs(hmiAlarmsPath);

            var beta80Path = $"{exportPath}/Beta80Alarms.xlsx";
            beta80Excel.SaveAs(beta80Path);
        }
    }
}
