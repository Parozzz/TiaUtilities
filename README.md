# TiaUtilities

This project has born out of necessity to decrease time of creating certain part of industrial automation via TIA Portal.

At the moment, this is what exists:
1) An API to parse SimaticML .xml files generated from TIAOpenness, modify values inside them (Still WIP) and generate a new file to be imported again in TIA. SUPPORT ONLY LADDER LOGIC (LAD).
2) A series of tools to generate ladder fc blocks to automate repetitive task (Alias generation and Alarm generation).
   - A series of grids to better handle repetitive task before done inside TIA Portal
   - Javascript support for better customization
   - Many specific tools to improve QOL
   - Grids support CTRL+Z / CTRL+Y
   - Fully Localized (IT - EN)
3) AddIn for TIA to import / export blocks to .xml files.
4) Supports version of TIA from V16 up to V19 (Previous version are very different and will not be implemented)

## SimaticML API
Still WIP, but adds the ability to create segments programmatically. Still needs quite a grasp on how siemens generate .xml files since names are taken from there.

You can find some examples in ![SimaticMLExamples](https://github.com/Parozzz/TiaUtilities/blob/main/SimaticML/SimaticMLExamples.cs)

Below an example on how to create an FC block.
```C#
            //Use BlockFC for creating a Function and BlockFB for creating a Function Block.
            var fc = new BlockFC();
            fc.Init();

            //Add basic block information, like name, number and programming language.
            var attributeList = fc.AttributeList;
            attributeList.BlockName = "FC_TEST";
            attributeList.BlockNumber = 123;
            attributeList.ProgrammingLanguage = SimaticProgrammingLanguage.LADDER;

            var createSafeFC = false;
            if(createSafeFC)
            {
                attributeList.ProgrammingLanguage = SimaticProgrammingLanguage.SAFE_LADDER;
            }

            //While creating a variable, you need to select the Section first (They are stored inside the attribute list).
            //INPUT - Use for BlockFC / BlockFB
            //OUTPUT - Use for BlockFC / BlockFB
            //INOUT - Use for BlockFC / BlockFB
            //STATIC - Use for BlockFB / BlockDB / BlockInstanceDB
            //TEMP - Use for BlockFC / BlockFB
            //CONSTANT - Use for BlockFC / BlockFB
            //RETURN - Use for BlockFC / BlockFB
            //NONE - Use for BlockUDT

            //Create some TEMP (Temporary) variables, to laser use for the contacts of the logic, beforehand.
            var contactVariables = new List<SimaticVariable>();
            for (int i = 0; i < 10; i++)
            {
                var var = attributeList.TEMP.AddVariable($"tContact{i}", SimaticDataType.BOOLEAN);
                contactVariables.Add(var);
            }
            //Create some TEMP variables for the coils.
            var coil1Var = attributeList.TEMP.AddVariable("tCoil1", SimaticDataType.BOOLEAN);
            var coil2Var = attributeList.TEMP.AddVariable("tCoil2", SimaticDataType.BOOLEAN);

            //Create the parts that will form the compileUnit (Segment). While creating them, associate them with the previous create variables.
            //A Part is everything that does something inside a segment (Contact, Coil, Block) that is not a call for an FC/FB.
            var contactParts = new ContactPart[10];
            for (int i = 0; i < 10; i++)
            {
                contactParts[i] = new ContactPart() { Operand = contactVariables[i] };
            }
            var coil1 = new CoilPart() { Operand = coil1Var };
            var coil2 = new CoilPart() { Operand = coil2Var };

            //Create the segment. For now, only ladder segments are *still partially* implemented.
            var segment = new SimaticLADSegment();
            segment.Title[CultureInfo.CurrentCulture] = "Segment Title!";
            segment.Comment[CultureInfo.CurrentCulture] = "Segment Comment! Much information here ...";

            //Create the connections between the parts
            //Brackets are important! C# will prioritize & to |, so the logic might break if not using them!
            var _ = segment.Powerrail & (contactParts[0] & (((contactParts[1] | contactParts[2]) & (contactParts[3] | contactParts[4])) | (contactParts[5] & contactParts[6])) & (contactParts[7] | contactParts[8]) | contactParts[9]) & coil1 & coil2;

            //This will add the segment to the Block.
            segment.Create(fc);

            //Create skeleton for the XML Document and add the FC to it.
            var xmlDocument = SimaticMLAPI.CreateDocument(fc);

            var result = NativeFileDialogCore.Dialog.FileSave(".xml");
            if (result.IsOk)
            {
                var path = result.Path;
                if (string.IsNullOrEmpty(path))
                {
                    return;
                }
                //Save the file.
                xmlDocument.Save(path);
            }
```
![image](https://github.com/user-attachments/assets/e6007aa7-04d4-4cd7-b3ed-b7f147c30743)

### Mentions
- FastColoredTextBox (For JS Editor) - https://github.com/PavelTorgashov/FastColoredTextBox
- Jint (For JS parsing) - https://github.com/sebastienros/jint
- RJControls (RJCodeAdvance) - https://github.com/RJCodeAdvance/Custom-TextBox-2--Rounded-Placeholder
