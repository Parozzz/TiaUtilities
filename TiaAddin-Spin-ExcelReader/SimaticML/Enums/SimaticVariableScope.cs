using SpinXmlReader.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.AccessNamespace;

namespace TiaXmlReader.SimaticML.Enums
{
    public enum SimaticVariableScope
    {
        UNKNOW = 0, //Default value
        LOCAL_VARIABLE,
        LOCAL_CONSTANT,
        LITERAL_CONSTANT,
        TYPED_CONSTANT,
        GLOBAL_CONSTANT,
        GLOBAL_VARIABLE,
        LABEL,
    }

    public static class SimaticVariableScopeExtension
    {
        public static string GetSimaticMLString(this SimaticVariableScope type)
        {
            switch (type)
            {
                case SimaticVariableScope.LOCAL_VARIABLE: return "LocalVariable";
                case SimaticVariableScope.LOCAL_CONSTANT: return "LocalConstant";
                case SimaticVariableScope.LITERAL_CONSTANT: return "LiteralConstant";
                case SimaticVariableScope.TYPED_CONSTANT: return "TypedConstant";
                case SimaticVariableScope.GLOBAL_CONSTANT: return "GlobalConstant";
                case SimaticVariableScope.GLOBAL_VARIABLE: return "GlobalVariable";
                case SimaticVariableScope.UNKNOW: return "Undef";
                //case AccessScope.LABEL: return "Label";
                default:
                    throw new Exception("VariableScope " + type.ToString() + "  not yet implemented");
            }
        }

        public static IAccessData CreateAccessData(this SimaticVariableScope type, CompileUnit compileUnit)
        {
            switch (type)
            {
                case SimaticVariableScope.LOCAL_VARIABLE: return new LocalVariableAccessData(compileUnit);
                case SimaticVariableScope.LOCAL_CONSTANT: return new LocalConstantAccessData(compileUnit);
                case SimaticVariableScope.LITERAL_CONSTANT: return new LiteralConstantAccessData(compileUnit);
                case SimaticVariableScope.TYPED_CONSTANT: return new TypedConstantAccessData(compileUnit);
                case SimaticVariableScope.GLOBAL_CONSTANT: return new GlobalConstantAccessData(compileUnit);
                case SimaticVariableScope.GLOBAL_VARIABLE: return new GlobalVariableAccessData(compileUnit);
                //case AccessScope.LABEL: return "Label";
                default:
                    throw new Exception("VariableScope " + type.ToString() + "  not yet implemented");
            }
        }
    }
}
