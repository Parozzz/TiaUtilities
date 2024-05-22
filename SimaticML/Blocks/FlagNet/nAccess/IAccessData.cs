using SimaticML.Enums;

namespace SimaticML.Blocks.FlagNet.nAccess
{
    public abstract class IAccessData
    {
        public abstract SimaticVariableScope Scope { get; }
        public Access Access { get; init; }

        public IAccessData(Access access)
        {
            this.Access = access;
            this.Access.VariableScope = this.Scope;
        }
    }

    public class AccessDataType<T> where T : IAccessData
    {
        public readonly static AccessDataType<LocalVariableAccess> LOCAL_VARIABLE = new();
        public readonly static AccessDataType<GlobalVariableAccess> GLOBAL_VARIABLE = new();
        public readonly static AccessDataType<LocalConstantAccess> LOCAL_CONSTANT = new();
        public readonly static AccessDataType<GlobalConstantAccess> GLOBAL_CONSTANT = new();
        public readonly static AccessDataType<LiteralConstantAccess> LITERAL_CONSTANT = new();
        public readonly static AccessDataType<TypedConstantAccess> TYPED_CONSTANT = new();

        private AccessDataType() { }
    }

    public class LocalVariableAccess(Access access) : IAccessData(access)
    {
        public override SimaticVariableScope Scope => SimaticVariableScope.LOCAL_VARIABLE;
        public string Address { get => this.Access.GetAddress(); set => this.Access.SetAddress(value); }
    }

    public class GlobalVariableAccess(Access access) : IAccessData(access)
    {
        public override SimaticVariableScope Scope => SimaticVariableScope.GLOBAL_VARIABLE;
        public string Address { get => this.Access.GetAddress(); set => this.Access.SetAddress(value); }
    }

    public class LocalConstantAccess(Access access) : IAccessData(access)
    {
        public override SimaticVariableScope Scope => SimaticVariableScope.LOCAL_CONSTANT;
        public string ConstantName { get => this.Access.ConstantName; set => this.Access.ConstantName = value; }
    }

    public class GlobalConstantAccess(Access access) : IAccessData(access)
    {
        public override SimaticVariableScope Scope => SimaticVariableScope.GLOBAL_CONSTANT;
        public string ConstantName { get => this.Access.ConstantName; set => this.Access.ConstantName = value; }
    }

    public class LiteralConstantAccess(Access access) : IAccessData(access)
    {
        public override SimaticVariableScope Scope => SimaticVariableScope.LITERAL_CONSTANT;
        public SimaticDataType ConstantType { get => this.Access.ConstantType; set => this.Access.ConstantType = value; }
        public string ConstantValue { get => this.Access.ConstantValue; set => this.Access.ConstantValue = value; }
    }

    public class TypedConstantAccess(Access access) : IAccessData(access)
    {
        public override SimaticVariableScope Scope => SimaticVariableScope.TYPED_CONSTANT;
        public string ConstantValue { get => this.Access.ConstantValue; set => this.Access.ConstantValue = value; }
    }
}
