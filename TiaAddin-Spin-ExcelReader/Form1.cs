using System;
using System.Windows.Forms;
using System.Xml;
using TiaAddin_Spin_ExcelReader.TagTable;

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
                Filter = "XML Files (*.xml)|*.xml"
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

                var serializable = SiemensXMLParser.ParseXML(xmlDocument);
                if (serializable is XMLTagTable tagTable)
                {

                }
                else if (serializable is FCData fcData)
                {

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
