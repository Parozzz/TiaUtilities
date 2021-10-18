using SpinAddIn.BlockData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaAddin_Spin_ExcelReader.BlockData
{
    public class LiteralConstantAccess : Access
    {
        public string ConstantType { get; internal protected set; }
        public string ConstantValue { get; internal protected set; }

        internal LiteralConstantAccess() {  }
    }
}
