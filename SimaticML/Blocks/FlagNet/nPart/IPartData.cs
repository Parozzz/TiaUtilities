namespace SimaticML.Blocks.FlagNet.nPart
{
    /*
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

        public abstract class SimplePart(CompileUnit compileUnit, PartType partType) : IPartData(compileUnit, partType)
        {
            public override string InputConName => "in";
            public override string OutputConName => "out";
        }

        public abstract class OperandPart(CompileUnit compileUnit, PartType partType) : SimplePart(compileUnit, partType)
        {
            public SimaticVariable Operand { set => this.CreateOperandIdentWire(value); }

            private Wire CreateOperandIdentWire(SimaticVariable simaticVariable)
            {
                return this.compileUnit.CreateWire().CreateIdentCon(simaticVariable, base.part, "operand");
            }

            public Wire CreateOperandIdentWire(Access access)
            {
                return this.compileUnit.CreateWire().CreateIdentCon(access, base.part, "operand");
            }
        }

        public class ContactPart(CompileUnit compileUnit) : OperandPart(compileUnit, PartType.CONTACT)
        {
            public bool Negated { get => this.part.Negated; set => this.part.Negated = value; }
        }

        public class CoilPart(CompileUnit compileUnit) : OperandPart(compileUnit, PartType.COIL)
        {
            public bool Negated { get => this.part.Negated; set => this.part.Negated = value; }
        }

        public class SetCoilPart(CompileUnit compileUnit) : OperandPart(compileUnit, PartType.SET_COIL) { }

        public class ResetCoilPart(CompileUnit compileUnit) : OperandPart(compileUnit, PartType.RESET_COIL) { }

        public class TimerPart : IPartData
        {
            public override string InputConName => "IN";
            public override string OutputConName => "Q";

            public SimaticVariableScope InstanceScope { get => part.Instance.VariableScope; set => part.Instance.VariableScope = value; }
            public string InstanceAddress { get => part.Instance.GetAddress(); set => part.Instance.SetAddress(value); }
            public SimaticVariable PT { set => this.compileUnit.CreateWire().CreateIdentCon(value, part, "PT"); }

            public TimerPart(CompileUnit compileUnit, PartType partType) : base(compileUnit, partType)
            {
                base.part.TemplateValue = "Time";
                base.part.TemplateValueName = "time_type";
                base.part.TemplateValueType = "Type";

                this.compileUnit.CreateWire().CreateNameCon(part, "ET").CreateOpenCon();
            }
        }*/
}
