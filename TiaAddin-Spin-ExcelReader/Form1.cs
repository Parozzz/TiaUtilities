using System;
using System.Windows.Forms;
using System.Xml;
using TiaAddin_Spin_ExcelReader.BlockData;

namespace TiaAddin_Spin_ExcelReader
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

                var fcData = new FCData(xmlDocument);
                fcData.ParseXMLDocument();

                "".Equals("");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
