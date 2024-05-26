using SimaticML.Blocks;
using SimaticML.Blocks.FlagNet;
using SimaticML.Blocks.FlagNet.nPart;
using SimaticML.Enums;
using System.Collections.ObjectModel;

namespace SimaticML.API
{
    public abstract class SimaticPart
    {
        //all LADDER blocks have input / output connections ("in" and "out" for contact, "en" and "eno" for blocks).
        //Some, like a call for an FC, have more named connections.
        public abstract string InputConName { get; }
        public abstract string OutputConName { get; }

        public PartType PartType { get; init; }
        public SimaticMultilingualText Comment { get; init; }
        public SimaticPart? Next { get; set; }

        public SimaticPart(PartType partType)
        {
            this.PartType = partType;
            this.Comment = new();
        }

        public virtual bool IsCloser() => false;

        public virtual Part GetPart(CompileUnit compileUnit)
        {
            var part = compileUnit.CreatePart();
            part.PartType = this.PartType;
            foreach(var entry in this.Comment.GetDictionary())
            {
                part.Comment[entry.Key] = entry.Value;
            }
            return part;
        }

        public virtual void CreateIdentWires(CompileUnit compileUnit) { }

        public override string ToString() => $"Type={PartType}";

        public virtual SimaticPart AND(SimaticPart nextPart)
        {
            this.FindLastAnd().Next = nextPart;
            return this;
        }

        private SimaticPart FindLastAnd() => this.Next == null ? this : this.Next.FindLastAnd();

        public virtual SimaticPart OR(SimaticPart nextPart)
        {
            var orPart = new OrPart();
            orPart.PartList.Add(this);
            orPart.PartList.Add(nextPart);
            return orPart;
        }

        public static SimaticPart operator &(SimaticPart part, SimaticPart nextLogicPart) => part.AND(nextLogicPart);
        public static SimaticPart operator |(SimaticPart part, SimaticPart nextLogicPart) => part.OR(nextLogicPart);
    }


    public abstract class SimplePart(PartType partType) : SimaticPart(partType)
    {
        public override string InputConName => "in";
        public override string OutputConName => "out";
    }

    public class OrPart() : SimplePart(PartType.OR)
    {//Here the InputConName will need the number of the input, it will be added later!
        public List<SimaticPart> PartList { get; init; } = [];

        public override Part GetPart(CompileUnit compileUnit)
        {
            var part = base.GetPart(compileUnit);
            part.TemplateValue = $"{PartList.Count}";
            part.TemplateValueName = "Card";
            part.TemplateValueType = "Cardinality";
            return part;
        }

        public override SimaticPart OR(SimaticPart nextPart)
        {
            if(this.Next != null)
            { //If this or has something AFTER this, it means that doing an OR now will result adding this into the OR.
                var or = new OrPart();
                or.PartList.Add(this);
                or.PartList.Add(nextPart);
                return or;
            }
            else if(nextPart is OrPart orPart)
            {//If i Or another OrPart, it will be incorporated!
                this.PartList.AddRange(orPart.PartList);
                return this;
            }

            this.PartList.Add(nextPart);
            return this;
        }

        public override string ToString() => $"{base.ToString()}, Count: {PartList.Count}";
    }

    public abstract class OperandPart(PartType partType) : SimplePart(partType)
    {
        public SimaticVariable? Operand { get; set; }

        public override Part GetPart(CompileUnit compileUnit)
        {
            var part = base.GetPart(compileUnit);
            if (Operand != null)
            {
                compileUnit.CreateWire().CreateIdentCon(Operand, part, "operand");
            }
            return part;
        }

        public override string ToString() => $"{base.ToString()}, Operand=[{Operand}]";
    }

    public class ContactPart() : OperandPart(PartType.CONTACT)
    {
        public bool Negated { get; set; }

        public override Part GetPart(CompileUnit compileUnit)
        {
            var part = base.GetPart(compileUnit);
            if (Negated)
            {
                part.Negated = "operand";
            }
            return part;
        }

        public override string ToString() => $"{base.ToString()}, Negated={Negated}";
    }

    public class CoilPart() : OperandPart(PartType.COIL)
    {
        public bool Negated { get; set; }

        public override bool IsCloser() => true;

        public override Part GetPart(CompileUnit compileUnit)
        {
            var part = base.GetPart(compileUnit);
            if (Negated)
            {
                part.Negated = "operand";
            }
            return part;
        }

        public override string ToString() => $"{base.ToString()}, Negated={Negated}";
    }

    public class SetCoilPart() : OperandPart(PartType.SET_COIL)
    {
        public override bool IsCloser() => true;
    }

    public class ResetCoilPart() : OperandPart(PartType.RESET_COIL)
    {
        public override bool IsCloser() => true;
    }

    public class TimerPart(PartType partType) : SimaticPart(partType)
    {
        public override string InputConName => "IN";
        public override string OutputConName => "Q";

        public SimaticVariableScope InstanceScope { get; set; }
        public string? InstanceAddress { get; set; }
        public SimaticVariable? PT { get; set; }
        public SimaticVariable? ET { get; set; }

        public override Part GetPart(CompileUnit compileUnit)
        {
            var part = base.GetPart(compileUnit);
            part.TemplateValue = "Time";
            part.TemplateValueName = "time_type";
            part.TemplateValueType = "Type";

            if(InstanceScope != default && InstanceAddress != null)
            {
                part.Instance.VariableScope = InstanceScope;
                part.Instance.SetAddress(InstanceAddress);
            }

            if (PT == null)
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


        public override bool IsCloser() => true;

        public override string ToString() => $"{base.ToString()}, PT=[{PT}], ET=[{ET}]";
    }
}
