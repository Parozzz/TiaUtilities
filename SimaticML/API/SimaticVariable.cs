using SimaticML.Blocks.FlagNet.nAccess;
using SimaticML.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimaticML.Blocks.FlagNet
{
    public abstract class SimaticVariable(SimaticVariableScope variableScope)
    {
        public SimaticVariableScope Scope { get; init; } = variableScope;

        public abstract Access? CreateAccess(CompileUnit compileUnit);
    }

    public class SimaticLocalVariable(string address) : SimaticVariable(SimaticVariableScope.LOCAL_VARIABLE)
    {
        public string Address { get; init; } = address;

        public override Access CreateAccess(CompileUnit compileUnit)
        {
            var access = compileUnit.CreateAccess();
            access.SetAddress(this.Address);
            return access;
        }
    }

    public class SimaticGlobalVariable(string address) : SimaticVariable(SimaticVariableScope.GLOBAL_VARIABLE)
    {
        public string Address { get; init; } = address;

        public override Access CreateAccess(CompileUnit compileUnit)
        {
            var access = compileUnit.CreateAccess();
            access.SetAddress(this.Address);
            return access;
        }
    }

    public class SimaticLocalConstant(string constantName) : SimaticVariable(SimaticVariableScope.LOCAL_CONSTANT)
    {
        public string ConstantName { get; init; } = constantName;

        public override Access? CreateAccess(CompileUnit compileUnit)
        {
            var access = compileUnit.CreateAccess();
            access.ConstantName = ConstantName;
            return access;
        }
    }

    public class SimaticGlobalConstant(string constantName) : SimaticVariable(SimaticVariableScope.GLOBAL_CONSTANT)
    {
        public string ConstantName { get; set; } = constantName;

        public override Access? CreateAccess(CompileUnit compileUnit)
        {
            var access = compileUnit.CreateAccess();
            access.ConstantName = ConstantName;
            return access;
        }
    }

    public class SimaticLiteralConstant(SimaticDataType dataType, string constantValue) : SimaticVariable(SimaticVariableScope.LITERAL_CONSTANT)
    {
        public SimaticDataType ConstantType { get; init; } = dataType;
        public string ConstantValue { get; init; } = constantValue;

        public override Access? CreateAccess(CompileUnit compileUnit)
        {
            var access = compileUnit.CreateAccess();
            access.ConstantType = ConstantType;
            access.ConstantValue = ConstantValue;
            return access;
        }
    }

    public class SimaticTypedConstant(string constantValue) : SimaticVariable(SimaticVariableScope.TYPED_CONSTANT)
    {
        public string ConstantValue { get; init; } = constantValue;

        public override Access? CreateAccess(CompileUnit compileUnit)
        {
            var access = compileUnit.CreateAccess();
            access.ConstantValue = ConstantValue;
            return access;
        }
    }
}
