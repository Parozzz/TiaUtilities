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

        public SimaticLADSegment()
        {
            this.Powerrail = new();
            this.Title = new();
            this.Comment = new();
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
                var part = SegmentPart.Create(compileUnit, simaticPart);
                ArgumentNullException.ThrowIfNull(part, nameof(part));

                compileUnit.Powerrail.Add(part.Part, simaticPart.InputConName);

                ParsePart(compileUnit, part);
            }
        }

        private void ParsePart(CompileUnit compileUnit, SegmentPart previousPart)
        {
            var part = SegmentPart.Create(compileUnit, previousPart.And);
            if(part == null)
            {
                return;
            }

            if(part.SimaticPart is OrPart nextOrSimaticPart)
            {
                var or = part;

                var wire = compileUnit.CreateWire()
                                            .CreateNameCon(previousPart.Part, previousPart.OutputConName);

                for (int i = 0; i < nextOrSimaticPart.PartList.Count; i++)
                {
                    var loopPart = SegmentPart.Create(compileUnit, nextOrSimaticPart.PartList[i]);
                    ArgumentNullException.ThrowIfNull(loopPart, nameof(loopPart));

                    wire.CreateNameCon(loopPart.Part, loopPart.InputConName);

                    compileUnit.CreateWire()
                                    .CreateNameCon(or.Part, or.InputConName + (i + 1))
                                    .CreateNameCon(loopPart.Part, loopPart.OutputConName);
                }
            }
            else
            {
                compileUnit.CreateWire()
                    .CreateNameCon(previousPart.Part, previousPart.OutputConName)
                    .CreateNameCon(part.Part, part.InputConName);
            }


            ParsePart(compileUnit, part);

            /*
            if (previousSimaticPart.AndPart != null)
            {
                var nextSimaticPart = previousSimaticPart.AndPart;

                if (nextSimaticPart is OrPart or)
                {
                    var orPart = nextSimaticPart.FillCompileUnit(compileUnit);

                    var wire = compileUnit.CreateWire()
                                        .CreateNameCon(previousPart, previousSimaticPart.OutputConName);

                    for (int i = 0; i < or.PartList.Count; i++)
                    {
                        var simaticPart = or.PartList[i];
                        var part = simaticPart.FillCompileUnit(compileUnit);

                        wire.CreateNameCon(part, simaticPart.InputConName);

                        compileUnit.CreateWire()
                                        .CreateNameCon(orPart, or.InputConName + (i + 1))
                                        .CreateNameCon(part, simaticPart.OutputConName);

                        ParsePart(compileUnit, part, simaticPart);
                    }

                    ParsePart(compileUnit, orPart, or);
                }
                else
                {
                    var nextPart = nextSimaticPart.FillCompileUnit(compileUnit);

                    compileUnit.CreateWire()
                                    .CreateNameCon(previousPart, previousSimaticPart.OutputConName)
                                    .CreateNameCon(nextPart, nextSimaticPart.InputConName);

                    ParsePart(compileUnit, nextPart, nextSimaticPart);
                }
            }*/
        }

        private class SegmentPart
        {
            public static SegmentPart? Create(CompileUnit compileUnit, SimaticPart? simaticPart)
            {
                return simaticPart == null ? null : new(compileUnit, simaticPart);
            }

            public string InputConName { get => this.SimaticPart.InputConName; }
            public string OutputConName { get => this.SimaticPart.OutputConName; }

            public SimaticPart SimaticPart { get; init; }
            public Part Part { get; init; }

            public SimaticPart? And { get => this.SimaticPart.Next; }

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