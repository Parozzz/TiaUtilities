using Microsoft.WindowsAPICodePack.Dialogs;
using SimaticML.API;
using SimaticML.Blocks;
using TiaUtilities.CustomControls;

namespace TiaUtilities.DbVisualization
{
    public partial class TreeViewDBVisualization : Form
    {
        private readonly TriStateTreeView treeView;
        public TreeViewDBVisualization()
        {
            InitializeComponent();


            this.treeView = new()
            {
                CheckBoxes = true,
                FullRowSelect = true,
                Dock = DockStyle.Fill,
            };
            this.tableLayoutPanel.Controls.Add(this.treeView);

            this.textBox.Click += (sender, args) =>
            {
                var fileDialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = false,
                    EnsurePathExists = true,
                    EnsureValidNames = true,
                    Multiselect = true,
                    DefaultExtension = ".xml",
                    Filters = { new CommonFileDialogFilter("XML Files", "*.xml") }
                };

                if (fileDialog.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                foreach (var filePath in fileDialog.FileNames)
                {
                    if (string.IsNullOrEmpty(filePath))
                    {
                        continue;
                    }

                    var xmlNodeConfiguration = SimaticMLAPI.ParseFile(filePath);
                    if (xmlNodeConfiguration is BlockDB blockDB)
                    {
                        LoadDB(blockDB);
                    }
                }
            };
        }

        public void LoadDB(BlockDB blockDB)
        {
            var memberAddresses = blockDB.GetAllMemberAddress();

            this.treeView.BeginUpdate();
            foreach (var memberAddress in memberAddresses)
            {
                var node = this.treeView.Nodes.Add(memberAddress);
                node.Nodes.Add("A");
                node.Nodes.Add("B");
            }
            this.treeView.UpdateAllNodesState();
            this.treeView.EndUpdate();
        }
    }
}
