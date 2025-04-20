using SimaticML.API;
using SimaticML.Blocks.FlagNet;
using SimaticML.Blocks;
using SimaticML.Enums;
using System.Globalization;

namespace SimaticML
{
    public class SimaticMLExamples
    {
        public static void CreateFCExample()
        {
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

                xmlDocument.Save(path);
            }
            //Save the file.
            //xmlDocument.Save(Directory.GetCurrentDirectory() + "/fc.xml");
        }

        public static void CreateGlobalDBExample()
        {
            //Use BlockFC for creating a Function and BlockFB for creating a Function Block.
            var db = new BlockGlobalDB();
            db.Init();

            //Add basic block information, like name, number and programming language.
            var attributeList = db.AttributeList;
            attributeList.BlockName = "DB_TEST_NICE!";
            attributeList.BlockNumber = 69;

            //If you want to create a SAFE DB, set the programming language to SAFE_DB (As below)
            //Remember that while creating a safety block, you will have limited data types and cannot use them all :)

            //Create some STATIC variables. BlockGlobalDB only allows to add to STATIC.
            //You could use the create variables inside a segment and will directly reference this created DB.
            var createSafeDB = true;
            if(createSafeDB)
            {
                attributeList.ProgrammingLanguage = SimaticProgrammingLanguage.SAFE_DB;

                var bool1 = attributeList.STATIC.AddVariable("BOOL_1", SimaticDataType.BOOLEAN);
                var int1 = attributeList.STATIC.AddVariable("INT_1", SimaticDataType.INT);
                var word1 = attributeList.STATIC.AddVariable("WORD_1", SimaticDataType.WORD);
            }
            else
            {
                var lword = attributeList.STATIC.AddVariable("LWORD_1", SimaticDataType.LWORD);
                var int1 = attributeList.STATIC.AddVariable("INT_1", SimaticDataType.INT);
                var real1 = attributeList.STATIC.AddVariable("REAL_1", SimaticDataType.REAL);
            }



            //Create skeleton for the XML Document and add the FC to it.
            var xmlDocument = SimaticMLAPI.CreateDocument(db);

            var result = NativeFileDialogCore.Dialog.FileSave(".xml");
            if (result.IsOk)
            {
                var path = result.Path;
                if (string.IsNullOrEmpty(path))
                {
                    return;
                }

                xmlDocument.Save(path);
            }
            //Save the file.
            //xmlDocument.Save(Directory.GetCurrentDirectory() + "/fc.xml");
        }

        public static void CreateUDTExample()
        {
            //Use BlockFC for creating a Function and BlockFB for creating a Function Block.
            BlockUDT blockUDT = new();
            blockUDT.Init();
            blockUDT.AttributeList.BlockName = "BLOCK_UDT";

            //Add basic block information, like name, number and programming language.
            var attributeList = blockUDT.AttributeList;
            attributeList.BlockName = "UDT_WOW!";

            //Create some NONE variables. BlockUDT only allows to add to NONE.
            attributeList.NONE.AddVariable("LWORD_1", SimaticDataType.LWORD);
            attributeList.NONE.AddVariable("INT_1", SimaticDataType.INT);
            attributeList.NONE.AddVariable("tReal1", SimaticDataType.REAL);

            //Create skeleton for the XML Document and add the FC to it.
            var xmlDocument = SimaticMLAPI.CreateDocument(blockUDT);

            var result = NativeFileDialogCore.Dialog.FileSave(".xml");
            if (result.IsOk)
            {
                var path = result.Path;
                if (string.IsNullOrEmpty(path))
                {
                    return;
                }

                xmlDocument.Save(path);
            }
            //Save the file.
            //xmlDocument.Save(Directory.GetCurrentDirectory() + "/fc.xml");
        }
    }
}
