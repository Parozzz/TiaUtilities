using ClosedXML.Excel;
using SpinXmlReader;
using SpinXmlReader.Block;
using System;
using System.Collections.Generic;
using TiaXmlReader.AlarmGeneration;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.AccessNamespace;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.PartNamespace;

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

        private readonly List<AlarmData> alarmDataList;
        private readonly List<ConsumerData> consumerDataList;

        private BlockFC fc;
        private CompileUnit compileUnit;

        public GenerationConsumerAlarms()
        {
            this.alarmDataList = new List<AlarmData>();
            this.consumerDataList = new List<ConsumerData>();
        }

        public void ImportExcelConfig(IXLWorksheet worksheet)
        {
            fcBlockName = worksheet.Cell("C5").Value.GetText();
            fcBlockNumber = (uint)worksheet.Cell("C6").Value.GetNumber();
            startingAlarmNum = (uint)worksheet.Cell("C8").Value.GetNumber();
            alarmNumFormat = worksheet.Cell("C9").Value.GetText();
            groupingType = worksheet.Cell("C10").Value.GetText();
            divisionType = worksheet.Cell("C11").Value.GetText();
            antiSlipNumber = (uint)worksheet.Cell("C13").Value.GetNumber();
            skipNumberAfterGroup = (uint)worksheet.Cell("C14").Value.GetNumber();

            alarmDataList.Clear();

            uint variablesCellIndex = 4;
            while (true)
            {
                var consumerAddressValue = worksheet.Cell("H" + variablesCellIndex).Value;
                var coil1AddressValue = worksheet.Cell("I" + variablesCellIndex).Value;
                var coil2AddressValue = worksheet.Cell("J" + variablesCellIndex).Value;
                var descriptionValue = worksheet.Cell("K" + variablesCellIndex).Value;
                var enableValue = worksheet.Cell("L" + variablesCellIndex).Value;

                variablesCellIndex++;

                if (!consumerAddressValue.IsText || !coil1AddressValue.IsText || !coil2AddressValue.IsText || !descriptionValue.IsText || !enableValue.IsText)
                {
                    break;
                }

                var alarmData = new AlarmData(consumerAddressValue.GetText(), coil1AddressValue.GetText(), coil2AddressValue.GetText(), descriptionValue.GetText(), bool.Parse(enableValue.GetText()));
                alarmDataList.Add(alarmData);
            }

            consumerDataList.Clear();

            var consumerCellIndex = 4;
            while (true)
            {
                var consumerNameValue = worksheet.Cell("E" + consumerCellIndex).Value;
                var dbNameValue = worksheet.Cell("F" + consumerCellIndex).Value;
                consumerCellIndex++;

                if (!consumerNameValue.IsText || !dbNameValue.IsText)
                {
                    break;
                }

                consumerDataList.Add(new ConsumerData(consumerNameValue.GetText(), dbNameValue.GetText()));
            }
        }

        public void GenerateBlocks()
        {
            GlobalIDGenerator.ResetID();

            fc = new BlockFC();
            fc.Init();
            fc.GetBlockAttributes().SetBlockName(fcBlockName).SetBlockNumber(fcBlockNumber).SetAutoNumber(fcBlockNumber > 0);

            var nextAlarmNum = startingAlarmNum;
            switch (groupingType)
            {
                case "PerUtenza":
                    foreach (var consumerData in consumerDataList)
                    {
                        if (divisionType == "GruppoPerSegmento")
                        {
                            compileUnit = fc.AddCompileUnit();
                            compileUnit.Init();
                        }

                        foreach (var generationData in alarmDataList)
                        {
                            if (!generationData.GetEnable())
                            {
                                continue;
                            }

                            if (divisionType == "UnoPerSegmento")
                            {
                                compileUnit = fc.AddCompileUnit();
                                compileUnit.Init();
                            }

                            var placeholders = new GenerationPlaceholders().SetConsumerData(consumerData).SetAlarmNum(nextAlarmNum++, alarmNumFormat);
                            compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, generationData.GetDescription(placeholders.Parse));

                            FillAlarmCompileUnit(compileUnit, placeholders, generationData);
                        }

                        if(antiSlipNumber > 0 && nextAlarmNum % antiSlipNumber != 0)
                        {
                            nextAlarmNum = (nextAlarmNum / antiSlipNumber) * antiSlipNumber + antiSlipNumber;
                        }

                        nextAlarmNum += skipNumberAfterGroup;
                    }
                    break;
                case "PerTipoAllarme":
                    foreach (var generationData in alarmDataList)
                    {
                        if (!generationData.GetEnable())
                        {
                            continue;
                        }

                        if (divisionType == "GruppoPerSegmento")
                        {
                            compileUnit = fc.AddCompileUnit();
                            compileUnit.Init();
                        }

                        foreach (var consumerData in consumerDataList)
                        {
                            if (divisionType == "UnoPerSegmento")
                            {
                                compileUnit = fc.AddCompileUnit();
                                compileUnit.Init();
                            }

                            var placeholders = new GenerationPlaceholders().SetConsumerData(consumerData).SetAlarmNum(nextAlarmNum++, alarmNumFormat);
                            compileUnit.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, generationData.GetDescription(placeholders.Parse));
                            FillAlarmCompileUnit(compileUnit, placeholders, generationData);
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
            var coil1 = new CoilPartData(compileUnit);
            var coil2 = new CoilPartData(compileUnit);

            contact.CreateIdentWire(GlobalVariableAccessData.Create(compileUnit, alarmData.GetConsumerAddress(placeholders.Parse)));
            coil1.CreateIdentWire(GlobalVariableAccessData.Create(compileUnit, alarmData.GetCoil1Address(placeholders.Parse)));
            coil2.CreateIdentWire(GlobalVariableAccessData.Create(compileUnit, alarmData.GetCoil2Address(placeholders.Parse)));

            contact.CreatePowerrailConnection()
                .CreateOutputConnection(coil1)
                .CreateOutputConnection(coil2);
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

            var xmlDocument = SiemensMLParser.CreateDocument();
            xmlDocument.DocumentElement.AppendChild(fc.Generate(xmlDocument));
            xmlDocument.Save(exportPath + "/fcExport_" + fc.GetBlockAttributes().GetBlockName() + ".xml");
        }

    }
}
