using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.CustomControls;

namespace TiaXmlReader
{
    public partial class IOGenerationForm : Form
    {
        public IOGenerationForm()
        {
            InitializeComponent();
        }

        public void Init()
        {
        }

        private void AddRow()
        {
            ioPanel.RowCount++;
            ioPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));

            var rowIndex = ioPanel.RowCount - 1;

            var ioAddressComboBox = new FlatComboBox();
            ioAddressComboBox.BackColor = SystemColors.Control;
            ioAddressComboBox.BorderColor = SystemColors.Control;
            ioAddressComboBox.ButtonColor = SystemColors.ControlDark;
            ioAddressComboBox.Margin = new Padding(0);
            ioAddressComboBox.FlatStyle = FlatStyle.Flat;
            ioAddressComboBox.Dock = DockStyle.Top;
            ioAddressComboBox.Font = new Font(Font.SystemFontName, 10);
            ioAddressComboBox.Items.Add("A");
            ioAddressComboBox.Items.Add("BS");
            ioAddressComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            ioAddressComboBox.KeyPress += (sender, e) => ioAddressComboBox.DroppedDown = false;
            ioPanel.Controls.Add(ioAddressComboBox, 0, rowIndex);

            var ioNameComboBox = new FlatComboBox();
            ioNameComboBox.BackColor = SystemColors.Control;
            ioNameComboBox.BorderColor = SystemColors.Control;
            ioNameComboBox.Margin = new Padding(0);
            ioNameComboBox.FlatStyle = FlatStyle.Flat;
            ioNameComboBox.Dock = DockStyle.Top;
            ioNameComboBox.Font = new Font(Font.SystemFontName, 10);
            ioPanel.Controls.Add(ioNameComboBox, 1, rowIndex);

            var dbComboBox = new FlatComboBox();
            dbComboBox.BackColor = SystemColors.Control;
            dbComboBox.BorderColor = SystemColors.Control;
            dbComboBox.Margin = new Padding(0);
            dbComboBox.FlatStyle = FlatStyle.Flat;
            dbComboBox.Dock = DockStyle.Top;
            dbComboBox.Font = new Font(Font.SystemFontName, 10);
            ioPanel.Controls.Add(dbComboBox, 2, rowIndex);

            var variabileComboBox = new FlatComboBox();
            variabileComboBox.BackColor = SystemColors.Control;
            variabileComboBox.BorderColor = SystemColors.Control;
            variabileComboBox.Margin = new Padding(0);
            variabileComboBox.FlatStyle = FlatStyle.Flat;
            variabileComboBox.Dock = DockStyle.Top;
            variabileComboBox.Font = new Font(Font.SystemFontName, 10);
            ioPanel.Controls.Add(variabileComboBox, 3, rowIndex);

            var commentRichTextBox = new RichTextBox();
            commentRichTextBox.Margin = new Padding(0);
            commentRichTextBox.Dock = DockStyle.Top;
            commentRichTextBox.BorderStyle = BorderStyle.None;
            commentRichTextBox.Font = new Font(Font.SystemFontName, 10);
            commentRichTextBox.Size = new Size(commentRichTextBox.Size.Width, 24);
            ioPanel.Controls.Add(commentRichTextBox, 4, rowIndex);
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            AddRow();
        }
    }
}
