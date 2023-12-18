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
    public class GenerationConsumerAlarms : IGeneration
    {
        private string fcBlockName;
        private uint fcBlockNumber;

        private uint startingAlarmNum;
        private string alarmNumFormat;

        private string groupingType;
        private string divisionType;
        private uint antiSlipNumber;
        private uint skipNumberAfterGroup;

        private string defaultCoilAddress;
        private string defaultSetAddress;
        private string defaultTimerAddress;
        private string defaultTimerType;
        private string defaultTimerTime;

        private string userAddressPrefix;

        private string segmentNameAlarmDivisionType;
        private string segmentNameUserDivisionType;

        private readonly List<AlarmData> alarmDataList;
        private readonly List<UserData> userDataList;

        private BlockFC fc;
        private CompileUnit compileUnit;

        public GenerationConsumerAlarms()
        {
            this.alarmDataList = new List<AlarmData>();
            this.userDataList = new List<UserData>();
        }

        public void ImportExcelConfig(IXLWorksheet worksheet)
        {
            fcBlockName = worksheet.Cell("C5").Value.ToString();
            fcBlockNumber = (uint)worksheet.Cell("C6").Value.GetNumber();
            startingAlarmNum = (uint)worksheet.Cell("C8").Value.GetNumber();
            alarmNumFormat = worksheet.Cell("C9").Value.ToString();
            groupingType = worksheet.Cell("C10").Value.ToString();
            divisionType = worksheet.Cell("C11").Value.ToString();
            antiSlipNumber = (uint)worksheet.Cell("C13").Value.GetNumber();
            skipNumberAfterGroup = (uint)worksheet.Cell("C14").Value.GetNumber();

            defaultCoilAddress = worksheet.Cell("C16").Value.ToString();
            defaultSetAddress = worksheet.Cell("C17").Value.ToString();
            defaultTimerAddress = worksheet.Cell("C18").Value.ToString();
            defaultTimerType = worksheet.Cell("C19").Value.ToString();
            defaultTimerTime = worksheet.Cell("C20").Value.ToString();

            userAddressPrefix = worksheet.Cell("C22").Value.ToString();

            segmentNameAlarmDivisionType = worksheet.Cell("C24").Value.ToString();
            segmentNameUserDivisionType = worksheet.Cell("C25").Value.ToString();

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
                    UserAddress = userAddress.ToString(),
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

                            placeholders.SetConsumerData(consumerData)
                                .SetAlarmData(alarmData)
                                .SetAlarmNum(nextAlarmNum++, alarmNumFormat);

                            if (divisionType == "UnoPerSegmento")
                            {
                                compileUnit = fc.AddCompileUnit();
                                compileUnit.Init();
                                compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(segmentNameUserDivisionType));
                            }

                            FillAlarmCompileUnit(compileUnit, placeholders, alarmData);
                        }

                        if (divisionType == "GruppoPerSegmento")
                        {
                            placeholders.SetStartEndAlarmNum(startAlarmNum, nextAlarmNum - 1, alarmNumFormat);
                            compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(segmentNameUserDivisionType));
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
                                    .SetAlarmData(alarmData)
                                    .SetAlarmNum(nextAlarmNum++, alarmNumFormat);

                            if (divisionType == "UnoPerSegmento")
                            {
                                compileUnit = fc.AddCompileUnit();
                                compileUnit.Init();
                                compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(segmentNameAlarmDivisionType));
                            }

                            FillAlarmCompileUnit(compileUnit, placeholders, alarmData);
                        }

                        if (divisionType == "GruppoPerSegmento")
                        {
                            placeholders.SetStartEndAlarmNum(startAlarmNum, nextAlarmNum - 1, alarmNumFormat);
                            compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, placeholders.Parse(segmentNameAlarmDivisionType));
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

        private void FillAlarmCompileUnit(CompileUnit compileUnit, GenerationPlaceholders placeholders, AlarmData alarmData)
        {
            var contact = new ContactPartData(compileUnit);
            var setCoil = new SetCoilPartData(compileUnit);
            var coil = new CoilPartData(compileUnit);

            var fullUserAddress = placeholders.Parse(userAddressPrefix) + placeholders.Parse(alarmData.UserAddress);
            contact.CreateIdentWire(GlobalVariableAccessData.Create(compileUnit, fullUserAddress));

            var fullCoilAddress = placeholders.ParseFullOr(alarmData.SetCoilAddress, defaultSetAddress);
            setCoil.CreateIdentWire(GlobalVariableAccessData.Create(compileUnit, fullCoilAddress));

            var fullSetAddress = placeholders.ParseFullOr(alarmData.CoilAddress, defaultCoilAddress);
            coil.CreateIdentWire(GlobalVariableAccessData.Create(compileUnit, fullSetAddress));

            if (!string.IsNullOrEmpty(alarmData.TimerAddress) || !string.IsNullOrEmpty(defaultTimerAddress))
            {
                PartType partType;

                var timerType = string.IsNullOrEmpty(alarmData.TimerType) ? defaultTimerType : alarmData.TimerType;
                switch (timerType.ToLower())
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
                timer.SetPartInstance(SimaticVariableScope.GLOBAL_VARIABLE, placeholders.ParseFullOr(alarmData.TimerAddress, defaultTimerAddress))
                    .SetTimeValue(placeholders.ParseFullOr(alarmData.TimerValue, defaultTimerTime));

                contact.CreatePowerrailConnection()
                    .CreateOutputConnection(timer)
                    .CreateOutputConnection(setCoil)
                    .CreateOutputConnection(coil);
            }
            else
            {
                contact.CreatePowerrailConnection()
                    .CreateOutputConnection(setCoil)
                    .CreateOutputConnection(coil);
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
