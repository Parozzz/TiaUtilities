# TiaUtilities

This project has born out of necessity to decrease time of creating certain part of industrial automation via TIA Portal.

At the moment, this is what exists:
1) An API to parse SimaticML .xml files generated from TIAOpenness, modify values inside them (Still WIP) and generate a new file to be imported again in TIA. SUPPORT ONLY LADDER LOGIC (LAD).
2) A series of tools to generate ladder fc blocks to automate repetitive task (Alias generation and Alarm generation).
   - A series of grids to better handle repetitive task before done inside TIA Portal
   - Javascript support for better customization
   - Many specific tools to improve QOL
   - Grids support CTRL+Z / CTRL+Y
   - Localization (Still WIP, mixed language for now)
3) AddIn for TIA to import / export blocks to .xml files.

## SimaticML API
Still WIP, but adds the ability to create segments programmatically. Still needs quite a grasp on how siemens generate .xml files since names are taken from there.

Below an example on how to create an FC block.
```C#
var fc = new BlockFC();

//Add basic block information, like name, number and programming language.
fc.AttributeList.BlockName = "FC_TEST";
fc.AttributeList.BlockNumber = 123;
fc.AttributeList.ProgrammingLanguage = SimaticProgrammingLanguage.LADDER;

//Add two temp variables to the specific section.
fc.AttributeList.TEMP.AddMember("tVar1", SimaticDataType.BOOLEAN);
fc.AttributeList.TEMP.AddMember("tVar2", SimaticDataType.BOOLEAN);

var compileUnit = fc.AddCompileUnit();

//Create the parts that will form the compileUnit (Segment).
//A Part is everything that does something inside a segment (Contact, Coil, Block) that is not an FC/FB.
var contact = new ContactPartData(compileUnit);
var coil = new CoilPartData(compileUnit);

//Create the access.
//"Access" are used to create an access a variable that needs to be used inside the block.
var contactAccess = compileUnit.AccessFactory.AddLocalVariable("tVar1");
var coilAccess = compileUnit.AccessFactory.AddLocalVariable("tVar2");

//Create the wires for coil and contact.
//Wires are how everything connect (IdentWire is association Part - Access).
contact.CreateIdentWire(contactAccess);
coil.CreateIdentWire(coilAccess);

//Create the connections wires
//Now create the interconnection between the created wires
// |
// | --- || --- () 
// |
var wire = compileUnit.Powerrail & contact & coil; //Create a AND connection between all the parts.

//Update all internal ID and UID of the block.
fc.UpdateID_UId(new IDGenerator());

//Create skeleton for the XML Document and add the FC to it.
var xmlDocument = SimaticMLAPI.CreateDocument(fc);

//Save the file.
xmlDocument.Save(Directory.GetCurrentDirectory() + "/fc.xml");
```
![image](https://github.com/Parozzz/TiaUtilities/assets/29524775/4c149485-f08a-46ed-a8b5-f24a3c973e0f)

### Mentions
- FastColoredTextBox (For JS Editor) - https://github.com/PavelTorgashov/FastColoredTextBox
- Jint (For JS parsing) - https://github.com/sebastienros/jint
- RJControls (RJCodeAdvance) - https://github.com/RJCodeAdvance/Custom-TextBox-2--Rounded-Placeholder
