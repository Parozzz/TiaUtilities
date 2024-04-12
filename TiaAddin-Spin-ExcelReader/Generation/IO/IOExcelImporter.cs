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
using System.Windows.Forms;
using TiaXmlReader.Localization;

namespace TiaXmlReader.Generation.IO
{
    public class IOExcelImporter
    {
        private readonly IOConfiguration config;
        private readonly List<IOData> ioDataList;

        public IOExcelImporter()
        {
            this.config = new IOConfiguration();
            this.ioDataList = new List<IOData>();
        }

        public IOConfiguration GetConfiguration()
        {
            return config;
        }

        public List<IOData> GetDataList()
        {
            return ioDataList;
        }

        public void ImportExcelConfig(IXLWorksheet worksheet)
        {
            config.FCBlockName = worksheet.Cell("C5").Value.ToString();
            config.FCBlockNumber = (uint)worksheet.Cell("C6").Value.GetNumber();

            if(!LocalizationHelper.TryGetEnumByDescription(worksheet.Cell("C8").Value.ToString(), out IOMemoryTypeEnum memoryType))
            {
                MessageBox.Show("Memory type is invalid.", "Invalid configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            config.MemoryType = memoryType;

            if (!LocalizationHelper.TryGetEnumByDescription(worksheet.Cell("C9").Value.ToString(), out IOGroupingTypeEnum groupingType))
            {
                MessageBox.Show("Groping type is invalid.", "Invalid configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            config.GroupingType = groupingType;

            config.DBName = worksheet.Cell("C12").Value.ToString();
            config.DBNumber = (uint)worksheet.Cell("C13").Value.GetNumber();

            config.VariableTableName = worksheet.Cell("C16").Value.ToString();
            config.VariableTableStartAddress = (uint)worksheet.Cell("C17").Value.GetNumber();
            config.VariableTableSplitEvery = (uint)worksheet.Cell("C18").Value.GetNumber();

            config.IOTableName = worksheet.Cell("C21").Value.ToString();
            config.IOTableSplitEvery = (uint)worksheet.Cell("C22").Value.GetNumber();

            config.DefaultIoName = worksheet.Cell("C25").Value.ToString();
            config.DefaultVariableName = worksheet.Cell("C26").Value.ToString();

            config.SegmentNameBitGrouping = worksheet.Cell("C29").Value.ToString();
            config.SegmentNameByteGrouping = worksheet.Cell("C30").Value.ToString();

            config.PrefixInputDB = worksheet.Cell("C33").Value.ToString();
            config.PrefixInputMerker = worksheet.Cell("C34").Value.ToString();
            config.PrefixOutputDB = worksheet.Cell("C35").Value.ToString();
            config.PrefixOutputMerker = worksheet.Cell("C36").Value.ToString();

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
