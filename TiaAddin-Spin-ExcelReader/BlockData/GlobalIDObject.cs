using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaAddin_Spin_ExcelReader.BlockData
{
    public class GlobalIDGenerator
    {
        private uint counter = 1;

        public uint GetNextID()
        {
            var ret = counter;
            counter++;
            return ret;
        }
    }

    public interface GlobalIDObject
    {
        uint GetID();
    }
}
