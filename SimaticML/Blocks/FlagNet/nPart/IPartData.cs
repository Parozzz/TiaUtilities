using SimaticML.Blocks.FlagNet.nAccess;
using SimaticML.Enums;

namespace SimaticML.Blocks.FlagNet.nPart
{
    public abstract class IPartData
    {        
        //all LADDER blocks have input / output connections ("in" and "out" for contact, "en" and "eno" for blocks).
        //Some, like a call for an FC, have more named connections.
        public abstract string InputConName { get; }
        public abstract string OutputConName { get; }

        protected readonly CompileUnit compileUnit;
        protected readonly Part part;
        public IPartData(CompileUnit compileUnit, PartType partType)
        {
            this.compileUnit = compileUnit;
            this.part = compileUnit.CreatePart();
            this.part.PartType = partType;
        }

        public Part GetPart() => part;

        public PartType GetPartType() => part.PartType;

        /*
        public IPartData CreatePowerrailConnection()
        {
            this.compileUnit.Powerrail.Add(this.part, this.InputConName);
            return this;
        }*/

        public T CreateInputConnection<T>(T inputPartData) where T : IPartData
        {
            this.compileUnit.CreateWire()
                .CreateNameCon(inputPartData.GetPart(), inputPartData.OutputConName)
                .CreateNameCon(this.part, this.InputConName);
            return inputPartData;
        }

        public T CreateOutputConnection<T>(T outputPartData) where T : IPartData
        {
            this.compileUnit.CreateWire()
                .CreateNameCon(this.part, this.OutputConName)
                .CreateNameCon(outputPartData.GetPart(), outputPartData.InputConName);
            return outputPartData;
        }

        public IPartData CreateOpenCon()
        {
            this.compileUnit.CreateWire().CreateOpenCon();
            return this;
        }

        public static IPartData operator &(IPartData partData, IPartData outputPartData) => partData.CreateOutputConnection(outputPartData);
    }

    public abstract class SimplePartData(CompileUnit compileUnit, PartType partType) : IPartData(compileUnit, partType)
    {
        public override string InputConName => "in";
        public override string OutputConName => "out";
    }

    public class NOTPartData(CompileUnit compileUnit) : SimplePartData(compileUnit, PartType.NOT) { }

    public abstract class SimpleIdenfiablePartData(CompileUnit compileUnit, PartType partType) : SimplePartData(compileUnit, partType)
    {
        public Wire CreateIdentWire(IAccessData accessData)
        {
            return this.compileUnit.CreateWire().CreateIdentCon(accessData.Access, part, "operand");
        }
    }

    public class ContactPartData(CompileUnit compileUnit) : SimpleIdenfiablePartData(compileUnit, PartType.CONTACT)
    {
        public ContactPartData SetNegated()
        {
            this.part.Negated = true;
            return this;
        }
    }
    public class CoilPartData(CompileUnit compileUnit) : SimpleIdenfiablePartData(compileUnit, PartType.COIL)
    {
        public CoilPartData SetNegated()
        {
            this.part.Negated = true;
            return this;
        }
    }
    public class SetCoilPartData : SimpleIdenfiablePartData
    {
        public SetCoilPartData(CompileUnit compileUnit) : base(compileUnit, PartType.SET_COIL) { }
    }
    public class ResetCoilPartData : SimpleIdenfiablePartData
    {
        public ResetCoilPartData(CompileUnit compileUnit) : base(compileUnit, PartType.RESET_COIL) { }
    }

    public class TimerPartData : IPartData
    {
        public override string InputConName => "IN";
        public override string OutputConName => "Q";

        public SimaticVariableScope InstanceScope { get => part.Instance.VariableScope; set => part.Instance.VariableScope = value; }
        public string Address { get => part.Instance.GetAddress(); set => part.Instance.SetAddress(value); }

        public string TimeTypedConstant 
        { 
            get => this.timeValueAccess.ConstantValue; 
            set
            {
                this.timeValueAccess.VariableScope = SimaticVariableScope.TYPED_CONSTANT;
                this.timeValueAccess.ConstantValue = value;
            }
        }
        public string TimeLocalVariable
        {
            get => this.timeValueAccess.GetAddress();
            set
            {
                this.timeValueAccess.VariableScope = SimaticVariableScope.LOCAL_VARIABLE;
                this.timeValueAccess.SetAddress(value);
            }
        }
        public string TimeGlobalVariable
        {
            get => this.timeValueAccess.GetAddress();
            set
            {
                this.timeValueAccess.VariableScope = SimaticVariableScope.GLOBAL_VARIABLE;
                this.timeValueAccess.SetAddress(value);
            }
        }

        private readonly Access timeValueAccess;
        public TimerPartData(CompileUnit compileUnit, PartType partType) : base(compileUnit, partType)
        {
            this.timeValueAccess = compileUnit.CreateAccess();

            base.part.TemplateValue = "Time";
            base.part.TemplateValueName = "time_type";
            base.part.TemplateValueType = "Type";
            this.TimeTypedConstant = "T#0s";

            this.compileUnit.CreateWire().CreateNameCon(part, "ET").CreateOpenCon();
            this.compileUnit.CreateWire().CreateIdentCon(timeValueAccess, part, "PT");
        }
    }
}
