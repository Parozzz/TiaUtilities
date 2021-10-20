using SpinAddin.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TiaAddin_Spin_ExcelReader.BlockData
{
    public enum AccessScopeEnum
    {
        GENERIC,
        LOCALVARIABLE,
        GLOBALVARIABLE,
        TYPEDCONSTANT,
        LITERALCONSTANT,
    }

    public class Access : UIdObject
    {
        public AccessScopeEnum Scope { get; protected internal set; }

        public Access()
        {

        }

    }
}
