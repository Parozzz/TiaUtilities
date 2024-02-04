using ClosedXML.Excel;
using SpinXmlReader;
using SpinXmlReader.Block;
using SpinXmlReader.SimaticML;
using System;
using System.Collections.Generic;
using TiaXmlReader.AlarmGeneration;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.AccessNamespace;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.PartNamespace;
using TiaXmlReader.SimaticML.Enums;

namespace TiaXmlReader.Generation
{
    public class GenerationUserAlarms : IGeneration
    {
        private string fcBlockName;
        private uint fcBlockNumber;

        private string groupingType;
        private string divisionType;

        private uint startingAlarmNum;
        private string alarmNumFormat;
        private uint antiSlipNumber;
        private uint skipNumberAfterGroup;

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
        private string perUserSegmentName;

        private readonly List<AlarmData> alarmDataList;
        private readonly List<UserData> userDataList;

        private BlockFC fc;
        private CompileUnit compileUnit;

        public GenerationUserAlarms()
        {
            this.alarmDataList = new List<AlarmData>();
            this.userDataList = new List<UserData>();
        }

        public void ImportExcelConfig(IXLWorksheet worksheet)
        {
            fcBlockName = worksheet.Cell("C5").Value.ToString();
            fcBlockNumber = (uint)worksheet.Cell("C6").Value.GetNumber();

            groupingType = worksheet.Cell("C8").Value.ToString();
            divisionType = worksheet.Cell("C9").Value.ToString();

            startingAlarmNum = (uint)worksheet.Cell("C12").Value.GetNumber();
            alarmNumFormat = worksheet.Cell("C13").Value.ToString();
            antiSlipNumber = (uint)worksheet.Cell("C14").Value.GetNumber();
            skipNumberAfterGroup = (uint)worksheet.Cell("C15").Value.GetNumber();

            defaultCoilAddress = worksheet.Cell("C18").Value.ToString();
            defaultSetCoilAddress = worksheet.Cell("C19").Value.ToString();
            defaultTimerAddress = worksheet.Cell("C20").Value.ToString();
            defaultTimerType = worksheet.Cell("C21").Value.ToString();
            defaultTimerValue = worksheet.Cell("C22").Value.ToString();

            alarmAddressPrefix = worksheet.Cell("C25").Value.ToString();
            coilAddressPrefix = worksheet.Cell("C26").Value.ToString();
            setCoilAddressPrefix = worksheet.Cell("C27").Value.ToString();
            timerAddressPrefix = worksheet.Cell("C28").Value.ToString();

            oneEachSegmentName = worksheet.Cell("C31").Value.ToString();
            perUserSegmentName = worksheet.Cell("C32").Value.ToString();

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
                    Enable = bool.TryParse(enable.ToString(), out bool enableResult) ? enableResult : false,
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

            var nextAlarmNum = startingAlarmNum;
            switch (groupingType)
            {
                case "PerUtenza":
                    foreach (var consumerData in userDataList)
                    {
                        if (divisionType == "GruppoPerSegmento")
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

                            if (divisionType == "UnoPerSegmento")
                            {
                                compileUnit = fc.AddCompileUnit();
                                compileUnit.Init();
                                compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(oneEachSegmentName));
                            }

                            FillAlarmCompileUnit(compileUnit, placeholders, parsedAlarmData);
                        }

                        if (divisionType == "GruppoPerSegmento")
                        {
                            placeholders.SetStartEndAlarmNum(startAlarmNum, nextAlarmNum - 1, alarmNumFormat);
                            compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(perUserSegmentName));
                        }

                        if (antiSlipNumber > 0 && nextAlarmNum % antiSlipNumber != 0)
                        {
                            nextAlarmNum = (nextAlarmNum / antiSlipNumber) * antiSlipNumber + antiSlipNumber;
                        }

                        nextAlarmNum += skipNumberAfterGroup;
                    }
                    break;
                case "PerTipoAllarme":
                    foreach (var alarmData in alarmDataList)
                    {
                        if (!alarmData.Enable)
                        {
                            continue;
                        }

                        var parsedAlarmData = ReplaceAlarmDataWithDefaultAndPrefix(alarmData);

                        if (divisionType == "GruppoPerSegmento")
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

                            if (divisionType == "UnoPerSegmento")
                            {
                                compileUnit = fc.AddCompileUnit();
                                compileUnit.Init();
                                compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(oneEachSegmentName));
                            }

                            FillAlarmCompileUnit(compileUnit, placeholders, parsedAlarmData);
                        }

                        if (divisionType == "GruppoPerSegmento")
                        {
                            placeholders.SetStartEndAlarmNum(startAlarmNum, nextAlarmNum - 1, alarmNumFormat);
                            compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(perUserSegmentName));
                        }

                        if (antiSlipNumber > 0 && nextAlarmNum % antiSlipNumber != 0)
                        {
                            nextAlarmNum = (nextAlarmNum / antiSlipNumber) * antiSlipNumber + antiSlipNumber;
                        }

                        nextAlarmNum += skipNumberAfterGroup;
                    }
                    break;
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
            var contact = new ContactPartData(compileUnit);

            var parsedContactAddress = placeholders.Parse(alarmData.AlarmAddress);
            contact.CreateIdentWire(GlobalVariableAccessData.Create(compileUnit, parsedContactAddress));
            contact.CreatePowerrailConnection();

            IPartData nextPartData = contact;
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

                var timer = new TimerPartData(compileUnit, partType);
                timer.SetPartInstance(SimaticVariableScope.GLOBAL_VARIABLE, placeholders.Parse(alarmData.TimerAddress))
                    .SetTimeValue(alarmData.TimerValue);

                contact.CreateOutputConnection(nextPartData = timer);
            }

            if (alarmData.SetCoilAddress.ToLower() != "\\")
            {
                var setCoil = new SetCoilPartData(compileUnit);
                setCoil.CreateIdentWire(GlobalVariableAccessData.Create(compileUnit, placeholders.Parse(alarmData.SetCoilAddress)));

                nextPartData.CreateOutputConnection(nextPartData = setCoil);
            }

            if (alarmData.CoilAddress.ToLower() != "\\")
            {
                var coil = new CoilPartData(compileUnit);
                coil.CreateIdentWire(GlobalVariableAccessData.Create(compileUnit, placeholders.Parse(alarmData.CoilAddress)));
                nextPartData.CreateOutputConnection(nextPartData = coil);
            }

            if (nextPartData == contact)
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
        }

    }
}
