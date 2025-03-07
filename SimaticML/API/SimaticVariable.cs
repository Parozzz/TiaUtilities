using SimaticML.Blocks.FlagNet.nAccess;
using SimaticML.Enums;
using System.Globalization;

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

    public abstract class SimaticNamedVariable(SimaticVariableScope variableScope,
        ISimaticVariableCollection? variableCollection = null, ISimaticVariableDataHolder? dataHolder = null) : SimaticVariable(variableScope)
    {

        internal readonly ISimaticVariableCollection? variableCollection = variableCollection;
        internal readonly ISimaticVariableDataHolder? dataHolder = dataHolder;

        public abstract string GetName();

        public SimaticDataType? FetchDataType()
        {
            if (this.variableCollection == null)
            {
                return null;
            }

            var name = this.GetName();
            return this.variableCollection.FetchDataTypeOf(name);
        }

        public void AddComment(CultureInfo cultureInfo, string commentText)
        {
            if (this.dataHolder != null)
            {
                this.dataHolder.AddComment(cultureInfo, commentText);
            }
        }

        public override string ToString() => $"Scope={Scope}, Name={this.GetName()}";
    }

    public class SimaticLocalVariable : SimaticNamedVariable
    {
        public string Address { get; init; }

        public SimaticLocalVariable(ISimaticVariableCollection variableCollection, ISimaticVariableDataHolder dataHolder)
            : base(SimaticVariableScope.LOCAL_VARIABLE, variableCollection, dataHolder)
        {
            this.Address = dataHolder.GetName();
        }

        public SimaticLocalVariable(string address) : base(SimaticVariableScope.LOCAL_VARIABLE)
        {
            this.Address = address;
        }

        public override Access CreateAccess(CompileUnit compileUnit)
        {
            var access = base.CreateAccess(compileUnit);
            access.SetAddress(this.Address);
            return access;
        }

        public override string GetName()
        {
            return this.Address;
        }

        public override string ToString() => $"{base.ToString()}, Address={Address}";
    }

    public class SimaticGlobalVariable : SimaticNamedVariable
    {
        public string Address { get; init; }

        public SimaticGlobalVariable(ISimaticVariableCollection variableCollection, ISimaticVariableDataHolder dataHolder)
            : base(SimaticVariableScope.GLOBAL_VARIABLE, variableCollection, dataHolder)
        {
            this.Address = dataHolder.GetName();
        }

        public SimaticGlobalVariable(string address) : base(SimaticVariableScope.GLOBAL_VARIABLE)
        {
            this.Address = address;
        }

        public override Access CreateAccess(CompileUnit compileUnit)
        {
            var access = base.CreateAccess(compileUnit);
            access.SetAddress(this.Address);
            return access;
        }

        public override string GetName()
        {
            return this.Address;
        }

        public override string ToString() => $"{base.ToString()}, Address={Address}";
    }

    public class SimaticLocalConstant : SimaticNamedVariable
    {
        public string ConstantName { get; init; }

        public SimaticLocalConstant(ISimaticVariableCollection variableCollection, ISimaticVariableDataHolder dataHolder)
            : base(SimaticVariableScope.LOCAL_CONSTANT, variableCollection, dataHolder)
        {
            this.ConstantName = dataHolder.GetName();
        }

        public SimaticLocalConstant(string constantName) : base(SimaticVariableScope.LOCAL_CONSTANT)
        {
            this.ConstantName = constantName;
        }

        public override Access CreateAccess(CompileUnit compileUnit)
        {
            var access = base.CreateAccess(compileUnit);
            access.ConstantName = ConstantName;
            return access;
        }

        public override string GetName()
        {
            return this.ConstantName;
        }

        public override string ToString() => $"{base.ToString()}, Name={ConstantName}";
    }

    public class SimaticGlobalConstant : SimaticNamedVariable
    {
        public string ConstantName { get; init; }

        public SimaticGlobalConstant(ISimaticVariableCollection variableCollection, ISimaticVariableDataHolder dataHolder)
            : base(SimaticVariableScope.GLOBAL_CONSTANT, variableCollection, dataHolder)
        {
            this.ConstantName = dataHolder.GetName();
        }

        public SimaticGlobalConstant(string constantName) : base(SimaticVariableScope.GLOBAL_CONSTANT)
        {
            this.ConstantName = constantName;
        }

        public override Access CreateAccess(CompileUnit compileUnit)
        {
            var access = base.CreateAccess(compileUnit);
            access.ConstantName = ConstantName;
            return access;
        }

        public override string GetName()
        {
            return this.ConstantName;
        }

        public override string ToString() => $"{base.ToString()}, Name={ConstantName}";
    }

    public class SimaticLiteralConstant(SimaticDataType dataType, string constantValue)
        : SimaticVariable(SimaticVariableScope.LITERAL_CONSTANT)
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

    public class SimaticTypedConstant(string constantValue)
        : SimaticVariable(SimaticVariableScope.TYPED_CONSTANT)
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
