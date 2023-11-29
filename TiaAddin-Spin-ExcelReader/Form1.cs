using SpinXmlReader.Block;
using SpinXmlReader.TagTable;
using System;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace SpinXmlReader
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void FilePathTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml",
                CheckFileExists = false
            };

            var result = fileDialog.ShowDialog();
            if(result == DialogResult.OK || result == DialogResult.Yes)
            {
                FilePathTextBox.Text = fileDialog.FileName;
            }
        }

        private void ParseFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(FilePathTextBox.Text);

                var parsedBlock = SiemensMLParser.ParseXML(xmlDocument);
                if(parsedBlock is XMLTagTable)
                {

                }
                else if (parsedBlock is BlockFC)
                {
                }
                else if (parsedBlock is BlockFB) 
                { 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            GlobalIDGenerator.ResetID();

            var fc = SiemensMLParser.CreateEmptyFC();

            //BLOCK ATTRIBUTES
            var inputSection = fc.GetBlockAttributes().ComputeSection(SectionTypeEnum.INPUT);

            var member = inputSection.AddMember();
            member.SetMemberName("VariableName");
            member.SetMemberDataType("Int");
            //BLOCK ATTRIBUTES

            //COMPILE UNITS
            var compileUnit = fc.AddCompileUnit();
            compileUnit.Init();

            var contactPart = compileUnit.AddPart(Part.Type.CONTACT).SetNegated();
            var coilPart = compileUnit.AddPart(Part.Type.COIL);

            var powerrail = compileUnit.AddWire();
            powerrail.SetPowerrail();
            powerrail.AddPowerrailCon(contactPart.GetLocalObjectData().GetUId(), "in");

            compileUnit.AddIdentWire(TiaXmlReader.Access.Type.GLOBAL_VARIABLE, "IO.IN_01", contactPart.GetLocalObjectData().GetUId(), "operand");
            compileUnit.AddIdentWire(TiaXmlReader.Access.Type.GLOBAL_VARIABLE, "IO.IN_02", coilPart.GetLocalObjectData().GetUId(), "operand");

            var contactToCoilWire = compileUnit.AddWire();
            contactToCoilWire.SetWireStart(contactPart.GetLocalObjectData().GetUId(), "out");
            contactToCoilWire.SetWireExit(coilPart.GetLocalObjectData().GetUId(), "in");
            //COMPILE UNITS

            var xmlDocument = SiemensMLParser.CreateDocument();
            xmlDocument.DocumentElement.AppendChild(fc.Generate(xmlDocument));
            xmlDocument.Save(FilePathTextBox.Text);
        }

        private void generateTagTableButton_Click(object sender, EventArgs e)
        {
            var tagTable = SiemensMLParser.CreateEmptyTagTable();

            var tag = tagTable.AddTag();
            tag.SetLogicalAddress("%M40.0");
            tag.SetTagName("TagName?!");
            tag.SetDataTypeName("bool");

            var xmlDocument = SiemensMLParser.CreateDocument();
            xmlDocument.DocumentElement.AppendChild(tagTable.Generate(xmlDocument));
            xmlDocument.Save(FilePathTextBox.Text);
        }
    }
}
