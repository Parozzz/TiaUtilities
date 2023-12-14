using SpinXmlReader.Block;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.AccessNamespace;

namespace TiaXmlReader.SimaticML.BlockFCFB.FlagNet.PartNamespace
{
    public abstract class IPartData
    {
        protected readonly CompileUnit compileUnit;
        protected readonly Part part;
        public IPartData(CompileUnit compileUnit)
        {
            this.compileUnit = compileUnit;
            this.part = new Part(compileUnit).SetPartType(this.GetPartType());
        }

        public Part GetPart()
        {
            return part;
        }

        public abstract PartType GetPartType();

        //all LADDER blocks have input / output connections ("in" and "out" for contact, "en" and "eno" for blocks).
        //Some, like FC, have more named connections.
        public abstract string GetInputConName();

        public abstract string GetOuputConName();
    }

    public class ContactPartData : IPartData
    {
        public ContactPartData(CompileUnit compileUnit) : base(compileUnit)
        {
        }

        public override PartType GetPartType() { return PartType.CONTACT; }
        public override string GetInputConName() { return "in"; }
        public override string GetOuputConName() { return "out"; }

        public ContactPartData SetNegated()
        {
            this.part.SetNegated();
            return this;
        }
        
        public ContactPartData CreatePowerrailConnection()
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

        public Wire CreateIdentWire(IAccessData accessData)
        {
            var access = accessData.GetAccess();
            return new Wire(compileUnit)
                  .SetIdentCon(access.GetLocalObjectData().GetUId(), part.GetLocalObjectData().GetUId(), "operand");
        }
    }

    public class CoilPartData : IPartData
    {
        public CoilPartData(CompileUnit compileUnit) : base(compileUnit)
        {
        }

        public override PartType GetPartType() { return PartType.COIL; }
        public override string GetInputConName() { return "in"; }
        public override string GetOuputConName() { return "out"; }

        public CoilPartData CreatePowerrailConnection()
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

        public Wire CreateIdentWire(IAccessData accessData)
        {
            var access = accessData.GetAccess();
            return new Wire(compileUnit)
                  .SetIdentCon(access.GetLocalObjectData().GetUId(), part.GetLocalObjectData().GetUId(), "operand");
        }
    }
}
