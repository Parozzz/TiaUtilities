using SimaticML.Blocks.FlagNet.nPart;
using SimaticML.Blocks.FlagNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimaticML.Blocks;
using DocumentFormat.OpenXml.Vml.Office;

namespace SimaticML.API
{
    public class SimaticLADSegment
    {
        enum PartConnectionType
        {
            AND,
            OR,
        };

        public class PowerrailWire
        {
            private readonly SimaticLADSegment segment;
            internal readonly Dictionary<SimaticPart, string> partConnectionDict = [];

            public PowerrailWire(SimaticLADSegment segment)
            {
                this.segment = segment;
            }

            public PowerrailWire Add(Dictionary<SimaticPart, string> dict)
            {
                foreach (var entry in dict)
                {
                    this.partConnectionDict.Add(entry.Key, entry.Value);
                }
                return this;
            }

            public SimaticPart Add(SimaticPart part, string partConnection)
            {
                this.partConnectionDict.Add(part, partConnection);
                return part;
            }

            public static SimaticPart operator &(PowerrailWire powerrail, SimaticPart partData) => powerrail.Add(partData, partData.InputConName);
        }

        public PowerrailWire Powerrail { get; init; }

        private readonly List<List<SimaticPart>> parts;
        public SimaticLADSegment()
        {
            this.parts = [];

            this.Powerrail = new();
        }

        public void Add(BlockFC fc)
        {
            var compileUnit = fc.AddCompileUnit();

            foreach(var entry in this.Powerrail.partConnectionDict)
            {
                var part = entry.Key.CreatePart(compileUnit);
                var connectionName = entry.Value;
                compileUnit.Powerrail.Add(part, connectionName);
            }

            
        }

    }
}
