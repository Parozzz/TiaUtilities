using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaAddin_Spin_ExcelReader.BlockData
{
    public class Wire
    {
        public bool IsPowerrail { get; private set; }
        public Wire(bool powerrail)
        {
            this.IsPowerrail = powerrail;
        }
    }
}
