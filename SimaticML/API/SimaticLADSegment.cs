using SimaticML.Blocks.FlagNet.nPart;
using SimaticML.Blocks.FlagNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimaticML.Blocks;
using DocumentFormat.OpenXml.Vml.Office;
using System.Globalization;
using SimaticML.LanguageText;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Drawing;

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
            internal readonly List<SimaticPart> PartList = [];

            public SimaticPart Add(SimaticPart part)
            {
                this.PartList.Add(part);
                return part;
            }

            public static SimaticPart operator &(PowerrailWire powerrail, SimaticPart partData) => powerrail.Add(partData);
        }

        public PowerrailWire Powerrail { get; init; }
        public SimaticMultilingualText Title { get; init; }
        public SimaticMultilingualText Comment { get; init; }
        private readonly List<SegmentPart> segmentParts;

        public SimaticLADSegment()
        {
            this.Powerrail = new();
            this.Title = new();
            this.Comment = new();

            this.segmentParts = [];
        }

        public void Create(IProgramBlock programBlock)
        {
            var compileUnit = programBlock.AddCompileUnit();
            compileUnit.Init();

            compileUnit.ProgrammingLanguage = Enums.SimaticProgrammingLanguage.LADDER;

            foreach (var entry in Title.GetDictionary())
            {
                compileUnit.Title[entry.Key] = entry.Value;
            }

            foreach (var entry in Comment.GetDictionary())
            {
                compileUnit.Comment[entry.Key] = entry.Value;
            }

            foreach (var simaticPart in Powerrail.PartList)
            {
                var part = this.ComputePart(compileUnit, simaticPart);
                ArgumentNullException.ThrowIfNull(part, nameof(part));

                compileUnit.Powerrail.Add(part.Part, simaticPart.InputConName);

                ParsePart(compileUnit, part);
            }
        }

        private void ParsePart(CompileUnit compileUnit, SegmentPart previousPart)
        {
            var part = this.ComputePart(compileUnit, previousPart.Next);
            if (part == null)
            {
                return;
            }

            if (part.SimaticPart is OrPart nextOrSimaticPart)
            {
                var or = part;

                var wire = compileUnit.CreateWire().CreateNameCon(previousPart.Part, previousPart.OutputConName);
                foreach(var previousOrPart in nextOrSimaticPart.PreviousParts)
                {
                    var loopPart = this.ComputePart(compileUnit, previousOrPart) ?? throw new ArgumentNullException(nameof(previousOrPart));
                    wire.CreateNameCon(loopPart.Part, loopPart.InputConName);

                    //Here i need to parse the PREVIOUS part that could contains more connections afterwards.
                    this.ParsePart(compileUnit, loopPart);
                }

                for (int i = 0; i < nextOrSimaticPart.PartList.Count; i++)
                {
                    var loopPart = this.ComputePart(compileUnit, nextOrSimaticPart.PartList[i].FindLast()) ?? throw new ArgumentNullException(nameof(nextOrSimaticPart));

                    compileUnit.CreateWire()
                                    .CreateNameCon(or.Part, or.InputConName + (i + 1))
                                    .CreateNameCon(loopPart.Part, loopPart.OutputConName);

                    ParsePart(compileUnit, loopPart);
                }
            }
            else
            {
                compileUnit.CreateWire()
                    .CreateNameCon(previousPart.Part, previousPart.OutputConName)
                    .CreateNameCon(part.Part, part.InputConName);
            }

            ParsePart(compileUnit, part);

        }

        private SegmentPart? ComputePart(CompileUnit compileUnit, SimaticPart? simaticPart)
        {
            if(simaticPart == null)
            {
                return null;
            }

            SegmentPart? segmentPart = this.segmentParts.Where(p => p.SimaticPart == simaticPart).FirstOrDefault();
            if (segmentPart == null)
            {
                segmentPart = SegmentPart.Create(compileUnit, simaticPart);
                this.segmentParts.Add(segmentPart);
            }
            return segmentPart;
        }

        private class SegmentPart
        {
            public static SegmentPart Create(CompileUnit compileUnit, SimaticPart simaticPart)
            {
                return new(compileUnit, simaticPart);
            }

            public string InputConName { get => this.SimaticPart.InputConName; }
            public string OutputConName { get => this.SimaticPart.OutputConName; }

            public SimaticPart SimaticPart { get; init; }
            public Part Part { get; init; }
            public SimaticPart? Next { get => this.SimaticPart.Next; }

            private SegmentPart(CompileUnit compileUnit, SimaticPart simaticPart)
            {
                this.SimaticPart = simaticPart;
                this.Part = simaticPart.GetPart(compileUnit);
            }

            public override string ToString() => this.SimaticPart.ToString();
        }
    }

}


/*
foreach (var entry in simaticPartDict)
{
    var powerrailSimaticPart = entry.Key;

    var powerrailPart = powerrailSimaticPart.FillCompileUnit(compileUnit);
    if (this.Powerrail.partConnectionDict.TryGetValue(powerrailSimaticPart, out string? connectionName))
    {
        compileUnit.Powerrail.Add(powerrailPart, connectionName);
    }

    SimaticPart? lastSimaticPart = null;
    Part? lastPart = null;

    foreach (var simaticPart in entry.Value)
    {
        var part = simaticPart.FillCompileUnit(compileUnit);
        if (lastSimaticPart == null || lastPart == null)
        {
            lastSimaticPart = simaticPart;
            lastPart = part;
            continue;
        }

        compileUnit.CreateWire()
                        .CreateNameCon(lastPart, lastSimaticPart.OutputConName)
                        .CreateNameCon(part, simaticPart.InputConName);

        lastSimaticPart = simaticPart;
        lastPart = part;
    }

    if (lastSimaticPart != null && lastPart != null && !lastSimaticPart.IsCloser())
    {
        compileUnit.CreateWire().CreateNameCon(lastPart, lastSimaticPart.OutputConName).CreateOpenCon();
    }
}*/