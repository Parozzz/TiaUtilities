using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpinAddIn.BlockData.Access
{
    public class LocalVariableAccess : Access
    {
        public string Symbol { get; protected internal set; }

        public LocalVariableAccess()
        {

        }
    }
}
