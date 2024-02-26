using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.Generation.IO
{
    public class IOConfiguration
    {
        public string FCBlockName;
        public uint FCBlockNumber;

        public string MemoryType;
        public string GroupingType;

        public string DBName;
        public uint DBNumber;

        public string VariableTableName;
        public uint VariableTableStartAddress;
        public uint VariableTableSplitEvery;

        public string IOTableName;
        public uint IOTableSplitEvery;

        public string DefaultIoName;
        public string DefaultVariableName;

        public string SegmentNameBitGrouping;
        public string SegmentNameByteGrouping;

        public string PrefixInputDB;
        public string PrefixInputMerker;
        public string PrefixOutputDB;
        public string PrefixOutputMerker;

    }
}
