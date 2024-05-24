using SimaticML.Blocks.FlagNet.nAccess;
using SimaticML.Blocks.FlagNet.nPart;
using SimaticML.Blocks.FlagNet;
using SimaticML.Blocks;
using SimaticML.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimaticML.API
{
    public abstract class SimaticPart
    {
        //all LADDER blocks have input / output connections ("in" and "out" for contact, "en" and "eno" for blocks).
        //Some, like a call for an FC, have more named connections.
        public abstract string InputConName { get; }
        public abstract string OutputConName { get; }

        protected readonly PartType partType;
        public SimaticPart(PartType partType)
        {
            this.partType = partType;
        }

        public PartType GetPartType() => partType;

        public abstract Part CreatePart(CompileUnit compileUnit);


    }

    public abstract class SimplePart(PartType partType) : SimaticPart(partType)
    {
        public override string InputConName => "in";
        public override string OutputConName => "out";
    }

    public abstract class OperandPart(PartType partType) : SimplePart(partType)
    {
        public SimaticVariable? Operand { get; set; }

        public override Part CreatePart(CompileUnit compileUnit)
        {
            var part = compileUnit.CreatePart();
            if(Operand != null)
            {
                compileUnit.CreateWire().CreateIdentCon(Operand, part, "operand");
            }
            return part;
        }
    }

    public class ContactPart() : OperandPart(PartType.CONTACT)
    {
        public bool Negated { get; set; }

        public override Part CreatePart(CompileUnit compileUnit)
        {
            var part = base.CreatePart(compileUnit);
            part.Negated = Negated;
            return part;
        }
    }

    public class CoilPart() : OperandPart(PartType.COIL)
    {
        public bool Negated { get; set; }

        public override Part CreatePart(CompileUnit compileUnit)
        {
            var part = base.CreatePart(compileUnit);
            part.Negated = Negated;
            return part;
        }
    }

    public class SetCoilPart() : OperandPart(PartType.SET_COIL) { }

    public class ResetCoilPart() : OperandPart(PartType.RESET_COIL) { }

    public class TimerPart : SimaticPart
    {
        public override string InputConName => "IN";
        public override string OutputConName => "Q";

        public SimaticVariableScope InstanceScope { get; set; }
        public string? InstanceAddress { get; set; }
        public SimaticVariable? PT { get; set; }
        public SimaticVariable? ET { get; set; }

        public TimerPart(PartType partType) : base(partType)
        {
        }

        public override Part CreatePart(CompileUnit compileUnit)
        {
            var part = compileUnit.CreatePart();

            part.TemplateValue = "Time";
            part.TemplateValueName = "time_type";
            part.TemplateValueType = "Type";

            if (InstanceScope != default && InstanceAddress != null)
            {
                part.Instance.VariableScope = this.InstanceScope;
                part.Instance.SetAddress(this.InstanceAddress);
            }

            if(PT == null)
            {
                compileUnit.CreateWire().CreateNameCon(part, "PT").CreateOpenCon();
            }
            else
            {
                compileUnit.CreateWire().CreateIdentCon(PT, part, "PT");
            }

            if (ET == null)
            {
                compileUnit.CreateWire().CreateNameCon(part, "ET").CreateOpenCon();
            }
            else
            {
                compileUnit.CreateWire().CreateIdentCon(ET, part, "ET");
            }

            return part;
        }
    }
}
