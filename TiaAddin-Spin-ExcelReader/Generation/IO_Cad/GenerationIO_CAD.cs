using ClosedXML.Excel;
using SpinXmlReader.Block;
using SpinXmlReader.TagTable;
using SpinXmlReader;
using System;
using System.Collections.Generic;
using System.Linq;
using TiaXmlReader.SimaticML;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.PartNamespace;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.AccessNamespace;
using SpinXmlReader.SimaticML;
using TiaXmlReader.SimaticML.nBlockAttributeList;
using TiaXmlReader.Utility;

namespace TiaXmlReader.Generation.IO_Cad
{
    internal class GenerationIO_CAD : IGeneration
    {
        private string fcBlockName;
        private uint fcBlockNumber;

        private string memoryType;
        private string groupingType;

        private string dbName;
        private uint dbNumber;

        private string variableTableName;
        private uint variableTableStartAddress;
        private uint variableTableSplitEvery;

        private string ioTableName;
        private uint ioTableSplitEvery;

        private string defaultPrimaryIOName;
        private string defaultSecondaryIOName;
        private string defaultVariableName;
        private string defaultIOComment;
        private string defaultVariableComment;

        private string segmentNameBitGrouping;
        private string segmentNameByteGrouping;

        private string prefixInputDB;
        private string prefixInputMerker;
        private string prefixOutputDB;
        private string prefixOutputMerker;

        private readonly List<CadData> cadDataList;

        private BlockFC fc;
        private CompileUnit compileUnit;
        private BlockGlobalDB db;

        private XMLTagTable ioTagTable;
        private readonly List<XMLTagTable> ioTagTableList;

        private XMLTagTable variableTagTable;
        private readonly List<XMLTagTable> variableTagTableList;

        public GenerationIO_CAD()
        {
            cadDataList = new List<CadData>();

            this.ioTagTableList = new List<XMLTagTable>();
            this.variableTagTableList = new List<XMLTagTable>();
        }

        public void ImportExcelConfig(IXLWorksheet worksheet)
        {
            cadDataList.Clear();

            uint cadIndex = 5;
            while (true)
            {
                //Generation data
                var ioName = worksheet.Cell("E" + cadIndex).Value;
                var dbName = worksheet.Cell("F" + cadIndex).Value;
                var variableName = worksheet.Cell("G" + cadIndex).Value;
                //Cad export
                var address = worksheet.Cell("H" + cadIndex).Value;
                var comment1 = worksheet.Cell("L" + cadIndex).Value;
                var comment2 = worksheet.Cell("M" + cadIndex).Value;
                var comment3 = worksheet.Cell("N" + cadIndex).Value;
                var comment4 = worksheet.Cell("O" + cadIndex).Value;
                var mnemonic = worksheet.Cell("P" + cadIndex).Value;
                var wireNum = worksheet.Cell("Q" + cadIndex).Value;
                var page = worksheet.Cell("R" + cadIndex).Value;
                var panel = worksheet.Cell("T" + cadIndex).Value;
                cadIndex++;

                //If none of the fields are text, i will stop going down the table.
                if (!address.IsText && !comment1.IsText && !comment2.IsText && !comment3.IsText && !comment4.IsText && !mnemonic.IsText && !wireNum.IsText && !page.IsText && !panel.IsText)
                {
                    break;
                }

                if (address.IsText && !string.IsNullOrWhiteSpace(address.ToString())) //If there is no address, i don't care about other data.
                {
                    cadDataList.Add(new CadData()
                    {
                        IOName = ioName.ToString(),
                        DBName = dbName.ToString(),
                        VariableName = variableName.ToString(),
                        CadAddress = address.ToString(),
                        Comment1 = comment1.ToString(),
                        Comment2 = comment2.ToString(),
                        Comment3 = comment3.ToString(),
                        Comment4 = comment4.ToString(),
                        Mnemonic = mnemonic.ToString(),
                        WireNum = wireNum.ToString(),
                        Page = page.ToString(),
                        Panel = panel.ToString(),
                    });
                }
            }

            fcBlockName = worksheet.Cell("C5").Value.ToString();
            fcBlockNumber = (uint)worksheet.Cell("C6").Value.GetNumber();

            memoryType = worksheet.Cell("C8").Value.ToString();
            groupingType = worksheet.Cell("C9").Value.ToString();

            dbName = worksheet.Cell("C12").Value.ToString();
            dbNumber = (uint)worksheet.Cell("C13").Value.GetNumber();

            variableTableName = worksheet.Cell("C16").Value.ToString();
            variableTableStartAddress = (uint)worksheet.Cell("C17").Value.GetNumber();
            variableTableSplitEvery = (uint)worksheet.Cell("C18").Value.GetNumber();

            ioTableName = worksheet.Cell("C21").Value.ToString();
            ioTableSplitEvery = (uint)worksheet.Cell("C22").Value.GetNumber();

            defaultPrimaryIOName = worksheet.Cell("C25").Value.ToString();
            defaultSecondaryIOName = worksheet.Cell("C26").Value.ToString();
            defaultVariableName = worksheet.Cell("C27").Value.ToString();
            defaultIOComment = worksheet.Cell("C28").Value.ToString();
            defaultVariableComment = worksheet.Cell("C29").Value.ToString();

            segmentNameBitGrouping = worksheet.Cell("C32").Value.ToString();
            segmentNameByteGrouping = worksheet.Cell("C33").Value.ToString();

            prefixInputDB = worksheet.Cell("C36").Value.ToString();
            prefixInputMerker = worksheet.Cell("C37").Value.ToString();
            prefixOutputDB = worksheet.Cell("C38").Value.ToString();
            prefixOutputMerker = worksheet.Cell("C39").Value.ToString();

        }

        public void GenerateBlocks()
        {
            fc = new BlockFC();
            fc.Init();
            fc.GetBlockAttributes().SetBlockName(fcBlockName).SetBlockNumber(fcBlockNumber).SetAutoNumber(fcBlockNumber > 0);

            uint tagCounter = 0;
            uint variableTagCounter = 0;

            var lastByteAddress = -1;
            var lastMemoryArea = SimaticMemoryArea.INPUT;

            uint merkerByte = variableTableStartAddress;
            uint merkerBit = 0;

            var ioAddressDict = new Dictionary<string, uint>();
            var variableAddressDict = new Dictionary<string, uint>();

            //Order list by ADRESS TYPE - BYTE - BIT 
            foreach (var cadData in cadDataList.OrderBy(x => ((int)x.GetCadMemoryArea().GetSimatic()) * Math.Pow(10, 9) + x.GetAddressByte() * Math.Pow(10, 3) + x.GetAddressBit()).ToList())
            {
                //If name of variable is \ i will ignore everything and skip to the next
                if (cadData.VariableName == "\\" || cadData.IOName == "\\")
                {
                    continue;
                }

                cadData.IOName = string.IsNullOrEmpty(cadData.IOName) ? defaultPrimaryIOName : cadData.IOName;
                cadData.VariableName = string.IsNullOrEmpty(cadData.VariableName) ? defaultVariableName : cadData.VariableName;
                cadData.IOComment = defaultIOComment;
                cadData.VariableComment = defaultVariableComment;

                var placeholders = new GenerationPlaceholders().SetCadData(cadData);

                //I have a secondary value that is used if the PARSED default primary is empty.
                cadData.IOName = string.IsNullOrEmpty(placeholders.Parse(cadData.IOName)) ? defaultSecondaryIOName : cadData.IOName;

                //After parsing secondary data, i will reset all the placeholders to have updated value with secondary default to be used correctly.
                placeholders.SetCadData(cadData);
                while(cadData.ParsePlaceholders(placeholders)) { } //Parsing until all the placeholders has been done. Since it a placeholder can contains another placeholders, this way i am sure none remains.

                if (ioTagTable == null || (ioTableSplitEvery > 0 && tagCounter % ioTableSplitEvery == 0))
                {
                    ioTagTable = new XMLTagTable();
                    ioTagTable.SetTagTableName(ioTableName + "_" + tagCounter);
                    ioTagTableList.Add(ioTagTable);
                }
                tagCounter++;

                //Set it with default tag name. In case it has a specific one, it will be overwritten below.
                var ioTag = ioTagTable.AddTag()
                    .SetBoolean(cadData.GetSimaticMemoryArea(), cadData.GetAddressByte(), cadData.GetAddressBit())
                    .SetTagName(FixDuplicateAddress(cadData.IOName, ioAddressDict))
                    .SetCommentText(SystemVariables.CULTURE, cadData.IOComment);

                string inOutAddress = null;
                if (!string.IsNullOrEmpty(cadData.DBName) && !string.IsNullOrEmpty(cadData.VariableName))
                {
                    inOutAddress = $"{SimaticMLUtil.WrapAddressComponentIfRequired(cadData.DBName)}.{cadData.VariableName}";
                }
                else
                {
                    switch (memoryType)
                    {
                        case "Merker":
                            if (variableTagTable == null || (variableTableSplitEvery > 0 && variableTagCounter % variableTableSplitEvery == 0))
                            {
                                variableTagTable = new XMLTagTable();
                                variableTagTable.SetTagTableName(variableTableName + "_" + variableTagCounter);
                                variableTagTableList.Add(variableTagTable);
                            }
                            variableTagCounter++;

                            var tagAddressPrefix = cadData.GetSimaticMemoryArea() == SimaticMemoryArea.INPUT ? prefixInputMerker : prefixOutputMerker;
                            var fullTagAddress = FixDuplicateAddress(tagAddressPrefix + cadData.VariableName, variableAddressDict);
                            inOutAddress = variableTagTable.AddTag()
                                            .SetTagName(fullTagAddress)
                                            .SetBoolean(SimaticMemoryArea.MERKER, merkerByte, merkerBit)
                                            .SetCommentText(SystemVariables.CULTURE, cadData.VariableComment)
                                            .GetTagName();
                            if (merkerBit++ >= 8)
                            {
                                merkerByte++;
                                merkerBit = 0;
                            }
                            break;
                        case "DB":
                            if (db == null)
                            {
                                db = new BlockGlobalDB();
                                db.Init();
                                db.GetAttributes().SetBlockName(dbName).SetBlockNumber(dbNumber).SetAutoNumber(dbNumber > 0);
                            }

                            var memberAddressPrefix = cadData.GetSimaticMemoryArea() == SimaticMemoryArea.INPUT ? prefixInputDB : prefixOutputDB;
                            var fullMemberAddress = FixDuplicateAddress(memberAddressPrefix + cadData.VariableName, variableAddressDict);
                            inOutAddress = db.GetAttributes().ComputeSection(SectionTypeEnum.STATIC)
                                    .AddMembersFromAddress(fullMemberAddress, SimaticDataType.BOOLEAN)
                                    .SetCommentText(SystemVariables.CULTURE, cadData.VariableComment)
                                    .GetCompleteSymbol();
                            break;
                    }
                }

                if (groupingType == "BitPerSegmento")
                {
                    compileUnit = fc.AddCompileUnit();
                    compileUnit.Init();
                    compileUnit.ComputeBlockTitle().SetText(SystemVariables.CULTURE, placeholders.Parse(segmentNameBitGrouping));
                }
                else if (groupingType == "BytePerSegmento" && (cadData.GetAddressByte() != lastByteAddress || cadData.GetSimaticMemoryArea() != lastMemoryArea || compileUnit == null))
                {
                    lastByteAddress = (int)cadData.GetAddressByte();
                    lastMemoryArea = cadData.GetSimaticMemoryArea();

                    compileUnit = fc.AddCompileUnit();
                    compileUnit.Init();
                    compileUnit.ComputeBlockTitle().SetText(SystemVariables.CULTURE, placeholders.Parse(segmentNameByteGrouping));
                }

                switch (cadData.GetSimaticMemoryArea())
                {
                    case SimaticMemoryArea.INPUT:
                        FillInOutCompileUnit(compileUnit, ioTag.GetTagName(), inOutAddress);
                        break;
                    case SimaticMemoryArea.OUTPUT:
                        FillInOutCompileUnit(compileUnit, inOutAddress, ioTag.GetTagName());
                        break;
                }
            }
        }
        private string FixDuplicateAddress(string address, Dictionary<string, uint> dict)
        {
            if (!dict.TryGetValue(address, out uint count))
            {
                dict.Add(address, 0);
            }

            dict[address] = ++count;
            return address + (count <= 1 ? "" : $"({count})");
        }

        private void FillInOutCompileUnit(CompileUnit compileUnit, string inputAddress, string outputAddress)
        {
            var contact = new ContactPartData(compileUnit);
            var coil = new CoilPartData(compileUnit);

            contact.CreateIdentWire(GlobalVariableAccessData.Create(compileUnit, inputAddress));
            coil.CreateIdentWire(GlobalVariableAccessData.Create(compileUnit, outputAddress));

            contact.CreatePowerrailConnection()
                .CreateOutputConnection(coil);
        }

        public void ExportXML(string exportPath)
        {
            if (string.IsNullOrEmpty(exportPath))
            {
                return;
            }

            if (fc == null || ioTagTable == null)
            {
                throw new ArgumentNullException("Blocks has not been generated");
            }

            var xmlDocument = SimaticMLParser.CreateDocument();
            xmlDocument.DocumentElement.AppendChild(fc.Generate(xmlDocument, new IDGenerator()));
            xmlDocument.Save(exportPath + "/fcExport_" + fc.GetBlockAttributes().GetBlockName() + ".xml");

            foreach (var ioTagTable in ioTagTableList)
            {
                xmlDocument = SimaticMLParser.CreateDocument();
                xmlDocument.DocumentElement.AppendChild(ioTagTable.Generate(xmlDocument, new IDGenerator()));
                xmlDocument.Save(exportPath + "/ioTagTable_" + ioTagTable.GetTagTableName() + ".xml");
            }

            foreach (var variableTagTable in variableTagTableList)
            {
                xmlDocument = SimaticMLParser.CreateDocument();
                xmlDocument.DocumentElement.AppendChild(variableTagTable.Generate(xmlDocument, new IDGenerator()));
                xmlDocument.Save(exportPath + "/tagTableExport_" + variableTagTable.GetTagTableName() + ".xml");
            }

            if (db != null)
            {
                xmlDocument = SimaticMLParser.CreateDocument();
                xmlDocument.DocumentElement.AppendChild(db.Generate(xmlDocument, new IDGenerator()));
                xmlDocument.Save(exportPath + "/dbExport_" + db.GetAttributes().GetBlockName() + ".xml");
            }
        }
    }
}
