using SimaticML.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimaticML.Blocks.FlagNet.nAccess
{
    public class AccessFactory
    {
        private readonly CompileUnit compileUnit;
        public AccessFactory(CompileUnit compileUnit) 
        { 
            this.compileUnit = compileUnit;
        }

        public LocalVariableAccess AddLocalVariable(string address)
        {
            var access = this.compileUnit.CreateAccess();
            return new(access) { Address = address };
        }

        public GlobalVariableAccess AddGlobalVariable(string address)
        {
            var access = this.compileUnit.CreateAccess();
            return new(access) { Address = address };
        }

        public LocalConstantAccess AddLocalConstant(string constantName)
        {
            var access = this.compileUnit.CreateAccess();
            return new(access) { ConstantName = constantName };
        }

        public GlobalConstantAccess AddGlobalConstant(string constantName)
        {
            var access = this.compileUnit.CreateAccess();
            return new(access) { ConstantName = constantName };
        }

        public LiteralConstantAccess AddLiteralConstant(SimaticDataType dataType, string constantValue)
        {
            var access = this.compileUnit.CreateAccess();
            return new(access) { ConstantType = dataType, ConstantValue = constantValue };
        }

        public TypedConstantAccess AddTypedConstant(string constantValue)
        {
            var access = this.compileUnit.CreateAccess();
            return new(access) { ConstantValue = constantValue };
        }
    }
}
