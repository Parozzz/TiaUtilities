using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpinAddIn.BlockData.Access
{
    public class TypedConstantAccess : Access
    {
        public string ConstantValue { get; protected internal set; }

        public TypedConstantAccess()
        {

        }
    }
}
