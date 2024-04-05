using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.Generation.IO
{
    public class IOConfiguration
    {
        public string FCBlockName = "fcTest_IO";
        public uint FCBlockNumber = 195;

        public string MemoryType = "DB";
        public string GroupingType = "BitPerSegmento";

        public string DBName = "TestIO_DB";
        public uint DBNumber = 196;

        public string VariableTableName = "VariableTable";
        public uint VariableTableStartAddress = 2890;
        public uint VariableTableSplitEvery = 250;

        public string IOTableName = "IOTags";
        public uint IOTableSplitEvery = 250;

        public string DefaultIoName = "{memory_type}{byte}_{bit}";
        public string DefaultVariableName = "{io_name}";

        public string SegmentNameBitGrouping = "{memory_type}{byte}_{bit} - {comment}";
        public string SegmentNameByteGrouping = "{memory_type}B{byte}";

        public string PrefixInputDB = "IN.";
        public string PrefixInputMerker = "MI_";
        public string PrefixOutputDB = "OUT.";
        public string PrefixOutputMerker = "MO_";

    }
}
