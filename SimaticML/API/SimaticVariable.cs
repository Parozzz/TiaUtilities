using SimaticML.Blocks.FlagNet.nAccess;
using SimaticML.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace SimaticML.Blocks.FlagNet
{
    public abstract class SimaticVariable(SimaticVariableScope variableScope)
    {
        public SimaticVariableScope Scope { get; init; } = variableScope;

        public virtual Access CreateAccess(CompileUnit compileUnit)
        {
            var access = compileUnit.CreateAccess();
            access.VariableScope = Scope;
            return access;
        }

        public override string ToString() => $"Scope={Scope}";
    }

    public class SimaticLocalVariable(string address) : SimaticVariable(SimaticVariableScope.LOCAL_VARIABLE)
    {
        public string Address { get; init; } = address;

        public override Access CreateAccess(CompileUnit compileUnit)
        {
            var access = base.CreateAccess(compileUnit);
            access.SetAddress(this.Address);
            return access;
        }

        public override string ToString() => $"{base.ToString()}, Address={Address}";
    }

    public class SimaticGlobalVariable(string address) : SimaticVariable(SimaticVariableScope.GLOBAL_VARIABLE)
    {
        public string Address { get; init; } = address;

        public override Access CreateAccess(CompileUnit compileUnit)
        {
            var access = base.CreateAccess(compileUnit);
            access.SetAddress(this.Address);
            return access;
        }

        public override string ToString() => $"{base.ToString()}, Address={Address}";
    }

    public class SimaticLocalConstant(string constantName) : SimaticVariable(SimaticVariableScope.LOCAL_CONSTANT)
    {
        public string ConstantName { get; init; } = constantName;

        public override Access CreateAccess(CompileUnit compileUnit)
        {
            var access = base.CreateAccess(compileUnit);
            access.ConstantName = ConstantName;
            return access;
        }

        public override string ToString() => $"{base.ToString()}, Name={ConstantName}";
    }

    public class SimaticGlobalConstant(string constantName) : SimaticVariable(SimaticVariableScope.GLOBAL_CONSTANT)
    {
        public string ConstantName { get; set; } = constantName;

        public override Access CreateAccess(CompileUnit compileUnit)
        {
            var access = base.CreateAccess(compileUnit);
            access.ConstantName = ConstantName;
            return access;
        }

        public override string ToString() => $"{base.ToString()}, Name={ConstantName}";
    }

    public class SimaticLiteralConstant(SimaticDataType dataType, string constantValue) : SimaticVariable(SimaticVariableScope.LITERAL_CONSTANT)
    {
        public SimaticDataType ConstantType { get; init; } = dataType;
        public string ConstantValue { get; init; } = constantValue;

        public override Access CreateAccess(CompileUnit compileUnit)
        {
            var access = base.CreateAccess(compileUnit);
            access.ConstantType = ConstantType;
            access.ConstantValue = ConstantValue;
            return access;
        }

        public override string ToString() => $"{base.ToString()}, Type={ConstantType}, Value={ConstantValue}";
    }

    public class SimaticTypedConstant(string constantValue) : SimaticVariable(SimaticVariableScope.TYPED_CONSTANT)
    {
        public string ConstantValue { get; init; } = constantValue;

        public override Access CreateAccess(CompileUnit compileUnit)
        {
            var access = base.CreateAccess(compileUnit);
            access.ConstantValue = ConstantValue;
            return access;
        }

        public override string ToString() => $"{base.ToString()}, Value={ConstantValue}";
    }
}
