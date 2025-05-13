using SimaticML.Blocks;
using SimaticML.Blocks.FlagNet;
using SimaticML.Blocks.FlagNet.nPart;
using SimaticML.Enums;

namespace SimaticML.API
{
    public abstract class SimaticPart(PartType partType)
    {
        //all LADDER blocks have input / output connections ("in" and "out" for contact, "en" and "eno" for blocks).
        //Some, like a call for an FC, have more named connections.
        public abstract string InputConName { get; }
        public abstract string OutputConName { get; }

        public PartType PartType { get; init; } = partType;
        public SimaticMultilingualText Comment { get; init; } = new();

        //A part can only have 1 previous part but multiple next
        //(The OR is considered one single part when connection something AFTER. Before the OR, the part will have multiple connections)
        public SimaticPart? Next { get; set; }
        public List<SimaticPart> Branches { get; init; } = [];

        public virtual bool IsCloser() => false;

        public virtual Part GetPart(CompileUnit compileUnit)
        {
            var part = compileUnit.CreatePart();
            part.PartType = this.PartType;
            foreach (var entry in this.Comment.GetDictionary())
            {
                part.Comment[entry.Key] = entry.Value;
            }
            return part;
        }

        public virtual void CreateIdentWires(CompileUnit compileUnit) { }

        public override string ToString() => $"Type={PartType}";

        public virtual SimaticPart AND(SimaticPart nextPart)
        {
            var last = this.FindLast();
            last.Next = nextPart;
            return this;
        }

        public virtual SimaticPart Branch(SimaticPart branch)
        {
            this.Branches.Add(branch);
            return branch;
        }

        public SimaticPart FindLast() => this.Next == null ? this : this.Next.FindLast();

        public virtual SimaticPart OR(SimaticPart nextPart)
        {
            var orPart = new OrPart();
            orPart.PartList.Add(this);
            orPart.PartList.Add(nextPart);
            orPart.PreviousPartAndConnections.AddRange(orPart.PartList);

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

        public List<SimaticPart> PreviousPartAndConnections { get; init; } = [];
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
            if (nextPart is OrPart orPart)
            {//If i Or another OrPart, it will be incorporated!
                this.PartList.AddRange(orPart.PartList);
                return this;
            }

            //Since when creating a new AND connection i always return the first in it, when adding an OR there needs to be two information:
            //1) What is the parts involved in the OR (So the OutputConName of the parts can be added into the InputConName of the ore)
            //2) What parts are involved in the AND connections with what comes before (Some the previous part OutputConName will be connected to multiple parts InputConName)
            //Since the nextPart is always the first of a chain, the parts involved in the OR will be the last of this chain,
            //while the one involved in the previous and will be the one passed directly (Since all the AND will return the first).

            var lastOr = this.FindLastOR();
            lastOr.PartList.Add(nextPart.FindLast());

            this.PreviousPartAndConnections.Add(nextPart);
            return this;
        }

        private OrPart FindLastOR() => this.Next is OrPart orPart ? orPart.FindLastOR() : this;

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

    public class MovePart() : SimaticPart(PartType.MOVE)
    {
        public override string InputConName => "en";
        public override string OutputConName => "eno";

        public SimaticVariable? IN { get; set; }
        public List<SimaticVariable> OUT { get; init; } = [];

        public override Part GetPart(CompileUnit compileUnit)
        {
            var part = base.GetPart(compileUnit);

            part.TemplateValueName = "Card";
            part.TemplateValueType = "Cardinality";
            part.TemplateValue = OUT.Count.ToString();

            if (IN != default)
            {
                compileUnit.CreateWire().CreateIdentCon(IN, part, "in");
            }

            if (OUT.Count > 0)
            {
                for (int x = 0; x < OUT.Count; x++)
                {
                    var outVariable = OUT[x];
                    compileUnit.CreateWire().CreateIdentCon(outVariable, part, "out" + (x + 1));
                }
            }

            return part;
        }

        public override bool IsCloser() => true;

        public override string ToString() => $"{base.ToString()}, IN=[{IN}], OUT=[{string.Join(" ", OUT)}]";
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

            if (InstanceScope != default && InstanceAddress != null)
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
