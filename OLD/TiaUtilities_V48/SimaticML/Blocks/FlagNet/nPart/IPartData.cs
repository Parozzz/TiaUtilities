﻿using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.SimaticML.Blocks;
using TiaXmlReader.SimaticML.Blocks.FlagNet;
using TiaXmlReader.SimaticML.Blocks.FlagNet.nAccess;

namespace TiaXmlReader.SimaticML.Blocks.FlagNet.nPart
{
    public abstract class IPartData
    {
        protected readonly CompileUnit compileUnit;
        protected readonly Part part;
        public IPartData(CompileUnit compileUnit, PartType partType)
        {
            this.compileUnit = compileUnit;
            this.part = compileUnit.CreatePart().SetPartType(partType);
        }

        public Part GetPart() => part;

        public PartType GetPartType() => part.GetPartType();


        //all LADDER blocks have input / output connections ("in" and "out" for contact, "en" and "eno" for blocks).
        //Some, like FC, have more named connections.
        public abstract string GetInputConName();

        public abstract string GetOuputConName();

        public IPartData CreatePowerrailConnection()
        {
            this.compileUnit.AddPowerrailConnections(this.part, this.GetInputConName());
            return this;
        }

        public T CreateInputConnection<T>(T inputPartData) where T : IPartData
        {
            this.compileUnit.CreateWire()
                .CreateNameCon(inputPartData.GetPart(), inputPartData.GetOuputConName())
                .CreateNameCon(this.part, this.GetInputConName());
            return inputPartData;
        }

        public T CreateOutputConnection<T>(T outputPartData) where T : IPartData
        {
            this.compileUnit.CreateWire()
                .CreateNameCon(this.part, this.GetOuputConName())
                .CreateNameCon(outputPartData.GetPart(), outputPartData.GetInputConName());
            return outputPartData;
        }

        public IPartData CreateOpenCon()
        {
            this.compileUnit.CreateWire().CreateOpenCon();
            return this;
        }
    }

    public abstract class SimplePartData : IPartData
    {
        public SimplePartData(CompileUnit compileUnit, PartType partType) : base(compileUnit, partType) { }

        public override string GetInputConName() => "in";
        public override string GetOuputConName() => "out";
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
            return this.compileUnit.CreateWire().CreateIdentCon(accessData.GetAccess(), part, "operand");
        }
    }
    public class ContactPartData : SimpleIdenfiablePartData
    {
        public ContactPartData(CompileUnit compileUnit) : base(compileUnit, PartType.CONTACT) { }

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

    public class TimerPartData : IPartData
    {
        private Access timeValueAccess;
        public TimerPartData(CompileUnit compileUnit, PartType partType) : base(compileUnit, partType)
        {
        }

        public override string GetInputConName() => "IN";
        public override string GetOuputConName() => "Q";

        public TimerPartData SetPartInstance(SimaticVariableScope scope, string address)
        {
            part.GetPartInstance()
                .SetVariableScope(scope)
                .SetAddress(address);

            part.SetTemplateValue("Time")
                .SetTemplateValueName("time_type")
                .SetTemplateValueType("Type");

            SetTimeValue("T#0s");
            this.compileUnit.CreateWire().CreateNameCon(part, "ET").CreateOpenCon();
            this.compileUnit.CreateWire().CreateIdentCon(timeValueAccess, part, "PT");

            return this;
        }

        public TimerPartData SetTimeValue(string timeValue)
        {
            timeValueAccess = (timeValueAccess ?? compileUnit.CreateAccess())
                .SetVariableScope(SimaticVariableScope.TYPED_CONSTANT)
                .SetConstantValue(timeValue);
            return this;
        }
    }
}
