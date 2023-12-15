using SpinXmlReader.Block;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.AccessNamespace;

namespace TiaXmlReader.SimaticML.BlockFCFB.FlagNet.PartNamespace
{
    public abstract class IPartData
    {
        protected readonly CompileUnit compileUnit;
        protected readonly Part part;
        public IPartData(CompileUnit compileUnit, PartType partType)
        {
            this.compileUnit = compileUnit;
            this.part = new Part(compileUnit).SetPartType(partType);
        }

        public Part GetPart()
        {
            return part;
        }

        public PartType GetPartType()
        {
            return part.GetPartType();
        }

        //all LADDER blocks have input / output connections ("in" and "out" for contact, "en" and "eno" for blocks).
        //Some, like FC, have more named connections.
        public abstract string GetInputConName();

        public abstract string GetOuputConName();
    }

    public abstract class SimplePartData : IPartData
    {
        public SimplePartData(CompileUnit compileUnit, PartType partType) : base(compileUnit, partType) { }

        public override string GetInputConName() { return "in"; }
        public override string GetOuputConName() { return "out"; }

        public SimplePartData CreatePowerrailConnection()
        {
            this.compileUnit.AddPowerrailSingleConnection(this.part, this.GetInputConName());
            return this;
        }

        public T CreateInputConnection<T>(T inputPartData) where T : IPartData
        {
            new Wire(this.compileUnit)
                .AddNameCon(inputPartData.GetPart(), inputPartData.GetOuputConName())
                .AddNameCon(this.part, this.GetInputConName());
            return inputPartData;
        }

        public T CreateOutputConnection<T>(T outputPartData) where T : IPartData
        {
            new Wire(this.compileUnit)
                .AddNameCon(this.part, this.GetOuputConName())
                .AddNameCon(outputPartData.GetPart(), outputPartData.GetInputConName());
            return outputPartData;
        }
    }
    public class NOTPartData : SimplePartData
    {
        public NOTPartData(CompileUnit compileUnit) : base(compileUnit, PartType.NOT) { }
    }

    public abstract class SimpleIdenfiablePartData : SimplePartData
    {
        public SimpleIdenfiablePartData(CompileUnit compileUnit, PartType partType) : base(compileUnit, partType) { }

        public Wire CreateIdentWire(IAccessData accessData)
        {
            var access = accessData.GetAccess();
            return new Wire(compileUnit)
                  .SetIdentCon(access.GetUId(), part.GetLocalObjectData().GetUId(), "operand");
        }
    }

    public class ContactPartData : SimpleIdenfiablePartData
    {
        public ContactPartData(CompileUnit compileUnit) : base(compileUnit, PartType.CONTACT)
        {
        }

        public ContactPartData SetNegated()
        {
            this.part.SetNegated();
            return this;
        }
    }

    public class CoilPartData : SimpleIdenfiablePartData
    {
        public CoilPartData(CompileUnit compileUnit) : base(compileUnit, PartType.COIL) { }

        public CoilPartData SetNegated()
        {
            this.part.SetNegated();
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
}
