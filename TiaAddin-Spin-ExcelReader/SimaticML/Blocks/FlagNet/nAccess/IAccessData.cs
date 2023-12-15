using SpinXmlReader.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.SimaticML.Enums;

namespace TiaXmlReader.SimaticML.BlockFCFB.FlagNet.AccessNamespace
{
    public abstract class IAccessData
    {
        protected readonly Access access;
        public IAccessData(CompileUnit compileUnit = null)
        {
            access = new Access(compileUnit).SetVariableScope(this.GetScope());
        }

        public Access GetAccess()
        {
            return access;
        }

        public abstract SimaticVariableScope GetScope();
    }

    public class LocalVariableAccessData : IAccessData
    {
        public static LocalVariableAccessData Create(CompileUnit compileUnit, string address)
        {
            return new LocalVariableAccessData(compileUnit).SetAddress(address);
        }

        public LocalVariableAccessData(CompileUnit compileUnit) : base(compileUnit)
        {

        }

        public string GetAddress()
        {
            return this.access.GetAddress();
        }

        public LocalVariableAccessData SetAddress(string address)
        {
            this.access.SetAddress(address);
            return this;
        }

        public override SimaticVariableScope GetScope()
        {
            return SimaticVariableScope.LOCAL_VARIABLE;
        }
    }

    public class GlobalVariableAccessData : IAccessData
    {
        public static GlobalVariableAccessData Create(CompileUnit compileUnit, string address)
        {
            return new GlobalVariableAccessData(compileUnit).SetAddress(address);
        }

        public GlobalVariableAccessData(CompileUnit compileUnit) : base(compileUnit)
        {

        }

        public string GetAddress()
        {
            return this.access.GetAddress();
        }

        public GlobalVariableAccessData SetAddress(string address)
        {
            this.access.SetAddress(address);
            return this;
        }

        public override SimaticVariableScope GetScope()
        {
            return SimaticVariableScope.GLOBAL_VARIABLE;
        }
    }

    public class LocalConstantAccessData : IAccessData
    {
        public static LocalConstantAccessData Create(CompileUnit compileUnit, string constantName)
        {
            return new LocalConstantAccessData(compileUnit).SetConstantName(constantName);
        }

        public LocalConstantAccessData(CompileUnit compileUnit) : base(compileUnit)
        {

        }

        public string GetConstantName()
        {
            return this.access.GetConstantName();
        }

        public LocalConstantAccessData SetConstantName(string constantName)
        {
            this.access.SetConstantName(constantName);
            return this;
        }

        public override SimaticVariableScope GetScope()
        {
            return SimaticVariableScope.LOCAL_CONSTANT;
        }
    }

    public class GlobalConstantAccessData : IAccessData
    {
        public static GlobalConstantAccessData Create(CompileUnit compileUnit, string constantName)
        {
            return new GlobalConstantAccessData(compileUnit).SetConstantName(constantName);
        }

        public GlobalConstantAccessData(CompileUnit compileUnit) : base(compileUnit)
        {

        }

        public string GetConstantName()
        {
            return this.access.GetConstantName();
        }

        public GlobalConstantAccessData SetConstantName(string constantName)
        {
            this.access.SetConstantName(constantName);
            return this;
        }

        public override SimaticVariableScope GetScope()
        {
            return SimaticVariableScope.GLOBAL_CONSTANT;
        }
    }

    public class LiteralConstantAccessData : IAccessData
    {
        public static LiteralConstantAccessData Create(CompileUnit compileUnit, SimaticDataType dataType, string constantValue)
        {
            return new LiteralConstantAccessData(compileUnit).SetConstantType(dataType).SetConstantValue(constantValue);
        }

        public LiteralConstantAccessData(CompileUnit compileUnit) : base(compileUnit)
        {

        }

        public SimaticDataType GetConstantType()
        {
            return this.access.GetConstantType();
        }

        public LiteralConstantAccessData SetConstantType(SimaticDataType dataType)
        {
            this.access.SetConstantType(dataType);
            return this;
        }

        public string GetConstantValue()
        {
            return this.access.GetConstantValue();
        }

        public LiteralConstantAccessData SetConstantValue(string constantValue)
        {
            this.access.SetConstantValue(constantValue);
            return this;
        }

        public override SimaticVariableScope GetScope()
        {
            return SimaticVariableScope.LITERAL_CONSTANT;
        }
    }

    public class TypedConstantAccessData : IAccessData
    {
        public static TypedConstantAccessData Create(CompileUnit compileUnit, string constantValue)
        {
            return new TypedConstantAccessData(compileUnit).SetConstantValue(constantValue);
        }

        public TypedConstantAccessData(CompileUnit compileUnit) : base(compileUnit)
        {

        }

        public string GetConstantValue()
        {
            return this.access.GetConstantValue();
        }

        public TypedConstantAccessData SetConstantValue(string constantValue)
        {
            this.access.SetConstantValue(constantValue);
            return this;
        }

        public override SimaticVariableScope GetScope()
        {
            return SimaticVariableScope.TYPED_CONSTANT;
        }
    }
}
