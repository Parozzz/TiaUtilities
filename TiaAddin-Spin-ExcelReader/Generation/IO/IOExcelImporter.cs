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
using ClosedXML.Excel;

namespace TiaXmlReader.Generation.IO
{
    public class IOExcelImporter
    {
        private readonly IOConfiguration configuration;
        private readonly List<IOData> ioDataList;

        public IOExcelImporter()        {
            this.configuration = new IOConfiguration();
            this.ioDataList = new List<IOData>();
        }

        public IOConfiguration GetConfiguration()
        {
            return configuration;
        }

        public List<IOData> GetDataList()
        {
            return ioDataList;
        }

        public void ImportExcelConfig(IXLWorksheet worksheet)
        {
            configuration.FCBlockName = worksheet.Cell("C5").Value.ToString();
            configuration.FCBlockNumber = (uint)worksheet.Cell("C6").Value.GetNumber();

            configuration.MemoryType = worksheet.Cell("C8").Value.ToString();
            configuration.GroupingType = worksheet.Cell("C9").Value.ToString();

            configuration.DBName = worksheet.Cell("C12").Value.ToString();
            configuration.DBNumber = (uint)worksheet.Cell("C13").Value.GetNumber();

            configuration.VariableTableName = worksheet.Cell("C16").Value.ToString();
            configuration.VariableTableStartAddress = (uint)worksheet.Cell("C17").Value.GetNumber();
            configuration.VariableTableSplitEvery = (uint)worksheet.Cell("C18").Value.GetNumber();

            configuration.IOTableName = worksheet.Cell("C21").Value.ToString();
            configuration.IOTableSplitEvery = (uint)worksheet.Cell("C22").Value.GetNumber();

            configuration.DefaultIoName = worksheet.Cell("C25").Value.ToString();
            configuration.DefaultVariableName = worksheet.Cell("C26").Value.ToString();

            configuration.SegmentNameBitGrouping = worksheet.Cell("C29").Value.ToString();
            configuration.SegmentNameByteGrouping = worksheet.Cell("C30").Value.ToString();

            configuration.PrefixInputDB = worksheet.Cell("C33").Value.ToString();
            configuration.PrefixInputMerker = worksheet.Cell("C34").Value.ToString();
            configuration.PrefixOutputDB = worksheet.Cell("C35").Value.ToString();
            configuration.PrefixOutputMerker = worksheet.Cell("C36").Value.ToString();

            ioDataList.Clear();

            uint cadIndex = 5;
            while (true)
            {
                var ioAddress = worksheet.Cell("E" + cadIndex).Value;
                var ioName = worksheet.Cell("F" + cadIndex).Value;
                var dbName = worksheet.Cell("G" + cadIndex).Value;
                var variableName = worksheet.Cell("H" + cadIndex).Value;
                var comment = worksheet.Cell("I" + cadIndex).Value;
                cadIndex++;

                if (!ioAddress.IsText || string.IsNullOrEmpty(ioAddress.ToString()))
                {
                    break;
                }

                ioDataList.Add(new IOData()
                {
                    Address = ioAddress.ToString(),
                    IOName = ioName.ToString(),
                    DBName = dbName.ToString(),
                    Variable = variableName.ToString(),
                    Comment = comment.ToString(),
                });
            }
        }

    }
}
