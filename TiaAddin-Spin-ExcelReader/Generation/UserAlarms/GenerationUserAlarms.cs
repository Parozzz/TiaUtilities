using ClosedXML.Excel;
using SpinXmlReader;
using SpinXmlReader.Block;
using SpinXmlReader.SimaticML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using TiaXmlReader.AlarmGeneration;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.AccessNamespace;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.PartNamespace;
using TiaXmlReader.SimaticML.Enums;

namespace TiaXmlReader.Generation
{

    public enum GroupingTypeEnum
    {
        ByUser,
        ByAlarmType
    }

    public enum DivisionTypeEnum
    {
        OnePerSegment,
        GroupPerSegment
    }

    public class GenerationUserAlarms : IGeneration
    {
        private string fcBlockName;
        private uint fcBlockNumber;
        private bool coilFirst;

        private string groupingType;
        private string divisionType;

        private uint startingAlarmNum;
        private string alarmNumFormat;
        private uint antiSlipNumber;
        private uint skipNumberAfterGroup;
        private bool generateEmptyAlarmAntiSlip;
        private uint emptyAlarmAtEnd;
        private string emptyAlarmContactAddress;
        
        private string defaultCoilAddress;
        private string defaultSetCoilAddress;
        private string defaultTimerAddress;
        private string defaultTimerType;
        private string defaultTimerValue;

        private string alarmAddressPrefix;
        private string coilAddressPrefix;
        private string setCoilAddressPrefix;
        private string timerAddressPrefix;

        private string oneEachSegmentName;
        private string oneEachEmptyAlarmSegmentName;
        private string groupSegmentName;
        private string groupEmptyAlarmSegmentName;

        private readonly List<AlarmData> alarmDataList;
        private readonly List<UserData> userDataList;

        private BlockFC fc;
        private CompileUnit compileUnit;
        private string fullAlarmList;

        public GenerationUserAlarms()
        {
            this.alarmDataList = new List<AlarmData>();
            this.userDataList = new List<UserData>();
        }

        public void ImportExcelConfig(IXLWorksheet worksheet)
        {
            fcBlockName = worksheet.Cell("C5").Value.ToString();
            fcBlockNumber = (uint)worksheet.Cell("C6").Value.GetNumber();
            coilFirst = worksheet.Cell("C7").Value.ToString().ToLower() == "true";

            groupingType = worksheet.Cell("C9").Value.ToString();
            divisionType = worksheet.Cell("C10").Value.ToString();

            startingAlarmNum = (uint)worksheet.Cell("C13").Value.GetNumber();
            alarmNumFormat = worksheet.Cell("C14").Value.ToString();
            antiSlipNumber = (uint)worksheet.Cell("C15").Value.GetNumber();
            skipNumberAfterGroup = (uint)worksheet.Cell("C16").Value.GetNumber();
            generateEmptyAlarmAntiSlip = worksheet.Cell("C17").Value.ToString().ToLower() == "true";
            emptyAlarmAtEnd = (uint)worksheet.Cell("C18").Value.GetNumber();
            emptyAlarmContactAddress = worksheet.Cell("C19").Value.ToString();

            defaultCoilAddress = worksheet.Cell("C22").Value.ToString();
            defaultSetCoilAddress = worksheet.Cell("C23").Value.ToString();
            defaultTimerAddress = worksheet.Cell("C24").Value.ToString();
            defaultTimerType = worksheet.Cell("C25").Value.ToString();
            defaultTimerValue = worksheet.Cell("C26").Value.ToString();

            alarmAddressPrefix = worksheet.Cell("C29").Value.ToString();
            coilAddressPrefix = worksheet.Cell("C30").Value.ToString();
            setCoilAddressPrefix = worksheet.Cell("C31").Value.ToString();
            timerAddressPrefix = worksheet.Cell("C32").Value.ToString();

            oneEachSegmentName = worksheet.Cell("C35").Value.ToString();
            oneEachEmptyAlarmSegmentName = worksheet.Cell("C36").Value.ToString();
            groupSegmentName = worksheet.Cell("C37").Value.ToString();
            groupEmptyAlarmSegmentName = worksheet.Cell("C38").Value.ToString();

            alarmDataList.Clear();

            uint variablesCellIndex = 5;
            while (true)
            {
                var userAddress = worksheet.Cell("H" + variablesCellIndex).Value;
                var coilAddress = worksheet.Cell("I" + variablesCellIndex).Value;
                var setCoilAddress = worksheet.Cell("J" + variablesCellIndex).Value;
                var timerAddress = worksheet.Cell("K" + variablesCellIndex).Value;
                var timerType = worksheet.Cell("L" + variablesCellIndex).Value;
                var timerValue = worksheet.Cell("M" + variablesCellIndex).Value;
                var description = worksheet.Cell("N" + variablesCellIndex).Value;
                var enable = worksheet.Cell("O" + variablesCellIndex).Value;
                variablesCellIndex++;

                if (!userAddress.IsText)
                {
                    break;
                }

                var alarmData = new AlarmData()
                {
                    AlarmAddress = userAddress.ToString(),
                    CoilAddress = coilAddress.ToString(),
                    SetCoilAddress = setCoilAddress.ToString(),
                    TimerAddress = timerAddress.ToString(),
                    TimerType = timerType.ToString(),
                    TimerValue = timerValue.ToString(),
                    Description = description.ToString(),
                    Enable = bool.TryParse(enable.ToString(), out bool enableResult) && enableResult,
                };
                alarmDataList.Add(alarmData);
            }

            userDataList.Clear();

            var consumerCellIndex = 5;
            while (true)
            {
                var userName = worksheet.Cell("E" + consumerCellIndex).Value;
                var userDescription = worksheet.Cell("F" + consumerCellIndex).Value;
                consumerCellIndex++;

                if (!userName.IsText)
                {
                    break;
                }

                var consumerData = new UserData()
                {
                    Name = userName.ToString(),
                    Description = userDescription.ToString(),
                };
                userDataList.Add(consumerData);
            }
        }

        public void GenerateBlocks()
        {
            fc = new BlockFC();
            fc.Init();
            fc.GetBlockAttributes().SetBlockName(fcBlockName).SetBlockNumber(fcBlockNumber).SetAutoNumber(fcBlockNumber > 0);

            fullAlarmList = "";

            GroupingTypeEnum groupingTypeEnum;
            switch (groupingType)
            {
                case "PerUtenza":
                    groupingTypeEnum = GroupingTypeEnum.ByUser;
                    break;
                case "PerTipoAllarme":
                    groupingTypeEnum = GroupingTypeEnum.ByAlarmType;
                    break;
                default:
                    MessageBox.Show("Tipo raggruppamento errato. Usati valori predefiniti.", "Errore dati");
                    return;
            }

            DivisionTypeEnum divisionTypeEnum;
            switch (divisionType)
            {
                case "UnoPerSegmento":
                    divisionTypeEnum = DivisionTypeEnum.OnePerSegment;
                    break;
                case "GruppoPerSegmento":
                    divisionTypeEnum = DivisionTypeEnum.GroupPerSegment;
                    break;
                default:
                    MessageBox.Show("Tipo divisione errato. Usati valori predefiniti.", "Errore dati");
                    return;
            }


            var nextAlarmNum = startingAlarmNum;
            switch (groupingTypeEnum)
            {
                case GroupingTypeEnum.ByUser:
                    foreach (var consumerData in userDataList)
                    {
                        if (divisionTypeEnum == DivisionTypeEnum.GroupPerSegment)
                        {
                            compileUnit = fc.AddCompileUnit();
                            compileUnit.Init();
                        }

                        var placeholders = new GenerationPlaceholders();

                        var startAlarmNum = nextAlarmNum;
                        foreach (var alarmData in alarmDataList)
                        {
                            if (!alarmData.Enable)
                            {
                                continue;
                            }

                            var parsedAlarmData = ReplaceAlarmDataWithDefaultAndPrefix(alarmData);

                            placeholders.SetConsumerData(consumerData)
                                .SetAlarmData(parsedAlarmData)
                                .SetAlarmNum(nextAlarmNum++, alarmNumFormat);
                            fullAlarmList += placeholders.Parse(GenerationPlaceholders.ALARM_DESCRIPTION) + '\n';

                            if (divisionTypeEnum == DivisionTypeEnum.OnePerSegment)
                            {
                                compileUnit = fc.AddCompileUnit();
                                compileUnit.Init();
                                compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(oneEachSegmentName));
                            }

                            FillAlarmCompileUnit(compileUnit, placeholders, parsedAlarmData);
                        }

                        var lastAlarmNum = nextAlarmNum - 1;
                        var alarmCount = (lastAlarmNum - startAlarmNum) + 1;

                        var slippingAlarmCount = antiSlipNumber % alarmCount;
                        if (antiSlipNumber > 0 && slippingAlarmCount > 0)
                        {
                            nextAlarmNum += slippingAlarmCount;

                            if (generateEmptyAlarmAntiSlip)
                            {
                                GenerateEmptyAlarms(divisionTypeEnum, groupingTypeEnum, lastAlarmNum + 1, slippingAlarmCount, compileUnit); //CompileUnit only used for group division
                                lastAlarmNum += slippingAlarmCount;
                            }
                        }

                        if (divisionTypeEnum == DivisionTypeEnum.GroupPerSegment)
                        {
                            placeholders.SetStartEndAlarmNum(startAlarmNum, lastAlarmNum, alarmNumFormat);
                            compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(groupSegmentName));
                        }

                        nextAlarmNum += skipNumberAfterGroup;
                    }
                    break;
                case GroupingTypeEnum.ByAlarmType:
                    foreach (var alarmData in alarmDataList)
                    {
                        if (!alarmData.Enable)
                        {
                            continue;
                        }

                        var parsedAlarmData = ReplaceAlarmDataWithDefaultAndPrefix(alarmData);

                        if (divisionTypeEnum == DivisionTypeEnum.GroupPerSegment)
                        {
                            compileUnit = fc.AddCompileUnit();
                            compileUnit.Init();
                        }

                        var placeholders = new GenerationPlaceholders();

                        var startAlarmNum = nextAlarmNum;
                        foreach (var consumerData in userDataList)
                        {
                            placeholders.SetConsumerData(consumerData)
                                    .SetAlarmData(parsedAlarmData)
                                    .SetAlarmNum(nextAlarmNum++, alarmNumFormat);
                            fullAlarmList += placeholders.Parse(GenerationPlaceholders.ALARM_DESCRIPTION) + '\n';

                            if (divisionTypeEnum == DivisionTypeEnum.OnePerSegment)
                            {
                                compileUnit = fc.AddCompileUnit();
                                compileUnit.Init();
                                compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(oneEachSegmentName));
                            }

                            FillAlarmCompileUnit(compileUnit, placeholders, parsedAlarmData);
                        }

                        var lastAlarmNum = nextAlarmNum - 1;
                        var alarmCount = (lastAlarmNum - startAlarmNum) + 1;

                        var slippingAlarmCount = antiSlipNumber % alarmCount;
                        if (antiSlipNumber > 0 && slippingAlarmCount > 0)
                        {
                            nextAlarmNum += slippingAlarmCount;

                            if (generateEmptyAlarmAntiSlip)
                            {
                                GenerateEmptyAlarms(divisionTypeEnum, groupingTypeEnum, lastAlarmNum + 1, slippingAlarmCount, compileUnit); //CompileUnit only used for group division
                                lastAlarmNum += slippingAlarmCount;
                            }
                        }

                        if (divisionTypeEnum == DivisionTypeEnum.GroupPerSegment)
                        {
                            placeholders.SetStartEndAlarmNum(startAlarmNum, lastAlarmNum, alarmNumFormat);
                            compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(groupSegmentName));
                        }

                        nextAlarmNum += skipNumberAfterGroup;
                    }
                    break;
            }

            GenerateEmptyAlarms(divisionTypeEnum, groupingTypeEnum, nextAlarmNum, emptyAlarmAtEnd);
        }

        private void GenerateEmptyAlarms(DivisionTypeEnum divisionType, GroupingTypeEnum groupingType, uint startAlarmNum, uint alarmCount, CompileUnit externalGroupCompileUnit = null)
        {
            var emptyAlarmData = new AlarmData()
            {
                AlarmAddress = emptyAlarmContactAddress,
                CoilAddress = defaultCoilAddress,
                SetCoilAddress = defaultSetCoilAddress,
                TimerAddress = defaultTimerAddress,
                TimerType = defaultTimerType,
                TimerValue = defaultTimerValue,
                Description = "",
                Enable = true
            };

            var alarmNum = startAlarmNum;

            CompileUnit compileUnit = externalGroupCompileUnit;
            if (compileUnit == null && divisionType == DivisionTypeEnum.GroupPerSegment)
            {
                var placeholders = new GenerationPlaceholders()
                    .SetAlarmData(emptyAlarmData)
                    .SetStartEndAlarmNum(alarmNum, alarmNum + (alarmCount - 1), alarmNumFormat);

                compileUnit = fc.AddCompileUnit();
                compileUnit.Init();
                compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(groupEmptyAlarmSegmentName));
            }

            for (int j = 0; j < alarmCount; j++)
            {
                var placeholders = new GenerationPlaceholders().SetAlarmData(emptyAlarmData).SetAlarmNum(alarmNum++, alarmNumFormat);

                if (divisionType == DivisionTypeEnum.OnePerSegment)
                {
                    compileUnit = fc.AddCompileUnit();
                    compileUnit.Init();
                    compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(oneEachEmptyAlarmSegmentName));
                }

                FillAlarmCompileUnit(compileUnit, placeholders, emptyAlarmData);
            }
        }

        private AlarmData ReplaceAlarmDataWithDefaultAndPrefix(AlarmData alarmData)
        {
            return new AlarmData()
            {
                AlarmAddress = alarmAddressPrefix + alarmData.AlarmAddress,
                CoilAddress = string.IsNullOrEmpty(alarmData.CoilAddress) ? defaultCoilAddress : (coilAddressPrefix + alarmData.CoilAddress),
                SetCoilAddress = string.IsNullOrEmpty(alarmData.SetCoilAddress) ? defaultSetCoilAddress : (setCoilAddressPrefix + alarmData.SetCoilAddress),
                TimerAddress = string.IsNullOrEmpty(alarmData.TimerAddress) ? defaultTimerAddress : (timerAddressPrefix + alarmData.TimerAddress),
                TimerType = Utils.StringFullOr(alarmData.TimerType, defaultTimerType),
                TimerValue = Utils.StringFullOr(alarmData.TimerValue, defaultTimerValue),
                Description = alarmData.Description,
                Enable = alarmData.Enable
            };

        }

        private void FillAlarmCompileUnit(CompileUnit compileUnit, GenerationPlaceholders placeholders, AlarmData alarmData)
        {
            var parsedContactAddress = placeholders.Parse(alarmData.AlarmAddress);

            IAccessData contactAccessData;
            switch (parsedContactAddress.ToLower())
            {
                case "false":
                    contactAccessData = LiteralConstantAccessData.Create(compileUnit, SimaticML.SimaticDataType.BOOLEAN, "FALSE");
                    break;
                case "0":
                    contactAccessData = LiteralConstantAccessData.Create(compileUnit, SimaticML.SimaticDataType.BOOLEAN, "0");
                    break;
                case "true":
                    contactAccessData = LiteralConstantAccessData.Create(compileUnit, SimaticML.SimaticDataType.BOOLEAN, "TRUE");
                    break;
                case "1":
                    contactAccessData = LiteralConstantAccessData.Create(compileUnit, SimaticML.SimaticDataType.BOOLEAN, "1");
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
                coilFirst ? coil : setCoil,
                coilFirst ? setCoil : coil
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

            var xmlDocument = SimaticMLParser.CreateDocument();
            xmlDocument.DocumentElement.AppendChild(fc.Generate(xmlDocument, new IDGenerator()));
            xmlDocument.Save(exportPath + "/fcExport_" + fc.GetBlockAttributes().GetBlockName() + ".xml");

            var alarmTextPath = exportPath + "/alarmsText.txt";
            using (var stream = File.CreateText(alarmTextPath))
            {
                stream.Write(fullAlarmList);
            }
        }

    }
}
