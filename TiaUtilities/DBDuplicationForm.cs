using SimaticML.API;
using SimaticML.Blocks;
using SimaticML.nBlockAttributeList;
using TiaUtilities;

namespace TiaUtilities
{
    public partial class DBDuplicationForm : Form
    {
        private readonly ProgramSettings programSettings;
        private readonly Dictionary<string, uint> memberWordDict;

        public DBDuplicationForm(ProgramSettings saveData)
        {
            InitializeComponent();
            this.programSettings = saveData;
            this.dbXMLPathTextBox.Text = saveData.LastDBDuplicationFileName;
            this.replaceDBNameCheckBox.Checked = saveData.DBDuplicationReplaceDBName;
            this.startingDBNumberTextBox.Text = "" + saveData.DBDuplicationStartingNum;
            this.newNameTextBox.Text = saveData.DBDuplicationNewDBName;
            this.newDBNameTextBox.Text = saveData.DBDuplicationNewDBName;
            this.replacementList1TextBox.Text = saveData.DBDuplicationReplacementList1;
            this.replacementList2TextBox.Text = saveData.DBDuplicationReplacementList2;
            this.replacementList3TextBox.Text = saveData.DBDuplicationReplacementList3;

            this.memberWordDict = new Dictionary<string, uint>();
        }

        private void DBXMLPathTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml",
                CheckFileExists = true,
                FileName = programSettings.LastDBDuplicationFileName
            };

            var result = fileDialog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                dbXMLPathTextBox.Text = programSettings.LastDBDuplicationFileName = fileDialog.FileName;
            }
        }

        private BlockAttributeList LoadAttributeListFromXMLFile()
        {
            var xmlConfiguration = SimaticMLAPI.ParseFile(dbXMLPathTextBox.Text) ?? throw new Exception("Invalid XML File");
            if (xmlConfiguration is not BlockDB)
            {
                throw new Exception("Imported xml is not a DB or is invalid.");
            }

            return ((BlockDB)xmlConfiguration).AttributeList;
        }

        private void AnalyzeFileButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(dbXMLPathTextBox.Text) || !File.Exists(dbXMLPathTextBox.Text))
            {
                return;
            }

            var section = LoadAttributeListFromXMLFile().STATIC;

            memberWordDict.Clear();
            foreach (var member in section.GetItems())
            {
                AnalyzeMemberNames(member);
            }
            var wordList = memberWordDict.OrderBy(key => key.Value).Select(key => key.Key).ToList();

            memberReplacementComboBox.Items.Clear();
            memberReplacementComboBox.Items.AddRange(wordList.ToArray());
        }

        private void AnalyzeMemberNames(Member member)
        {
            var memberName = member.MemberName;
            if (!memberWordDict.ContainsKey(memberName))
            {
                memberWordDict.Add(memberName, 0);
            }

            memberWordDict[memberName] += 1;
            foreach (var subMember in member.GetItems())
            {
                this.AnalyzeMemberNames(subMember);
            }
        }

        private void ReplaceAndExportButton_MouseClick(object sender, MouseEventArgs e)
        {
            var startingDBNumber = uint.Parse(startingDBNumberTextBox.Text);

            var replacementArray1 = this.replacementList1TextBox.Text.Split('\n');
            var replacementArray2 = this.replacementList2TextBox.Text.Split('\n');
            var replacementArray3 = this.replacementList3TextBox.Text.Split('\n');
            for (int x = 0; x < replacementArray1.Length; x++)
            {
                var replacement1 = replacementArray1[x].Replace("\r", "");
                var replacement2 = (replacementArray2.Length > x ? replacementArray2[x] : "").Replace("\r", "");
                var replacement3 = (replacementArray3.Length > x ? replacementArray3[x] : "").Replace("\r", "");

                if (string.IsNullOrEmpty(replacement1) && string.IsNullOrEmpty(replacement2) && string.IsNullOrEmpty(replacement3))
                {
                    continue;
                }

                var attributeList = LoadAttributeListFromXMLFile();

                var section = attributeList.STATIC;
                if (!string.IsNullOrEmpty(memberReplacementComboBox.Text))
                {
                    foreach (var member in section.GetItems())
                    {
                        var newMemberName = this.newNameTextBox.Text
                            .Replace("{replacement1}", replacement1)
                            .Replace("{replacement2}", replacement2)
                            .Replace("{replacement3}", replacement3);
                        ReplaceMemberNames(member, memberReplacementComboBox.Text, newMemberName);
                    }
                }

                if (replaceDBNameCheckBox.Checked)
                {
                    var newBlockDBName = this.newDBNameTextBox.Text
                        .Replace("{replacement1}", replacement1)
                        .Replace("{replacement2}", replacement2)
                        .Replace("{replacement3}", replacement3);
                    attributeList.BlockName = newBlockDBName;
                }

                attributeList.BlockNumber = startingDBNumber++;
                /*
                if(!string.IsNullOrEmpty(programSettings.lastXMLExportPath))
                {
                    attributeList.GetParentConfiguration().UpdateID_UId(new IDGenerator());

                    var xmlDocument = SimaticMLParser.CreateDocument();
                    xmlDocument.DocumentElement.AppendChild(attributeList.GetParentConfiguration().Generate(xmlDocument));
                    xmlDocument.Save(programSettings.lastXMLExportPath + "/DB" + attributeList.GetBlockNumber() + "_" + attributeList.GetBlockName() + ".xml");
                }*/
            }
        }
        private void ReplaceMemberNames(Member member, string toReplace, string replacement)
        {
            if (toReplace.ToLower() == member.MemberName.ToLower())
            {
                member.MemberName = replacement;
            }

            foreach (var subMember in member.GetItems())
            {
                this.ReplaceMemberNames(subMember, toReplace, replacement);
            }
        }

        private void ReplaceDBNameCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            programSettings.DBDuplicationReplaceDBName = replaceDBNameCheckBox.Checked;
        }

        private void StartingDBNumberTextBox_TextChanged(object sender, System.EventArgs e)
        {
            if (uint.TryParse(startingDBNumberTextBox.Text, out uint num))
            {
                programSettings.DBDuplicationStartingNum = num;
            }
        }

        private void NewDBNameTextBox_TextChanged(object sender, System.EventArgs e)
        {
            programSettings.DBDuplicationNewDBName = newDBNameTextBox.Text;
        }

        private void NewNameTextBox_TextChanged(object sender, System.EventArgs e)
        {
            programSettings.DBDuplicationNewMemberName = newNameTextBox.Text;
        }

        private void ReplacementList1TextBox_TextChanged(object sender, System.EventArgs e)
        {
            programSettings.DBDuplicationReplacementList1 = replacementList1TextBox.Text;
        }

        private void ReplacementList2TextBox_TextChanged(object sender, System.EventArgs e)
        {
            programSettings.DBDuplicationReplacementList2 = replacementList2TextBox.Text;
        }
        private void ReplacementList3TextBox_TextChanged(object sender, System.EventArgs e)
        {
            programSettings.DBDuplicationReplacementList3 = replacementList3TextBox.Text;
        }
    }
}
