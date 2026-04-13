using DocumentFormat.OpenXml.Drawing.Charts;
using TiaUtilities.Languages;
using TiaUtilities.Resources;
using TiaUtilities.Utility;
using static TiaUtilities.SettingsNew.SettingsValueNameLabel;

namespace TiaUtilities.CustomControls.EditableTab
{
    public partial class EditableTabControlQuickEditForm : Form
    {
        public class RowData()
        {
            public required TabPage TabPage { get; init; }
            public required TextBox NameTextBox { get; init; }
            public required RowDataInfo Info { get; init; }
            public required TableLayoutPanel Panel { get; init; }
        }

        public class RowDataInfo()
        {
            public required int Index { get; set; }
            public bool NeedsDeletition { get; set; } = false;
        }

        public List<RowData> RowsData { get; init; }

        private readonly EditableTabControl tabControl;

        private readonly Button acceptButton;

        public EditableTabControlQuickEditForm(EditableTabControl tabControl)
        {
            InitializeComponent();

            this.RowsData = [];

            this.tabControl = tabControl;
            this.acceptButton = new();

            Init();
        }

        private void Init()
        {
            this.mainPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            this.mainPanel.SizeChanged += (sender, args) =>
            {
                this.ClientSize = this.mainPanel.Size;
            };

            this.acceptButton.Text = Locale.GENERICS_ACCEPT;
            this.acceptButton.Anchor = AnchorStyles.None;
            this.acceptButton.AutoSize = true;
            this.acceptButton.Padding = new(5);
            this.acceptButton.Margin = Padding.Empty;
            this.acceptButton.Click += (sender, args) => CloseWithResult(DialogResult.OK);

            LoadTabControl();
            RedrawMainPanel();
        }

        private void LoadTabControl()
        {
            this.RowsData.Clear();

            int index = 0;
            foreach (TabPage tabPage in this.tabControl.TabPages)
            {
                if (tabPage is EditableNewTabPage)
                {
                    index++;
                    continue;
                }

                CreateTabEditControl(tabPage, index);
                index++;
            }
        }

        private Control CreateTabEditControl(TabPage tabPage, int index)
        {
            RowDataInfo currentDataInfo = new() { Index = index };

            Font regularFont = new(Font.SystemFontName, 11.5f, FontStyle.Regular);

            TextBox nameTextBox = new()
            {
                Text = tabPage.Text,
                BorderStyle = BorderStyle.None,
                BackColor = this.BackColor,
                Font = regularFont,
                Margin = Padding.Empty,
                Width = 300
            };

            var buttonsSize = regularFont.Height + 5;

            Button dragButton = new()
            {
                MaximumSize = new(buttonsSize, buttonsSize),
                BackColor = this.BackColor,
                BackgroundImage = ImageResources.DRAG_8187494,
                BackgroundImageLayout = ImageLayout.Stretch,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 1, BorderColor = SystemColors.ActiveBorder },
                Margin = new Padding(0, 0, 10, 0),
                AllowDrop = true,
                TabStop = false
            };
            dragButton.DragEnter += (sender, args) => HandleDragEnter(args);
            dragButton.DragDrop += (sender, args) => HandleDragDrop(currentDataInfo, args);
            dragButton.MouseDown += (sender, args) => dragButton.DoDragDrop(currentDataInfo, DragDropEffects.Move, null, Point.Empty, true);

            Button closeButton = new()
            {
                MaximumSize = new(buttonsSize, buttonsSize),
                BackgroundImage = ImageResources.CLOSE_193002,
                BackgroundImageLayout = ImageLayout.Stretch,
                BackColor = this.BackColor,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 1, BorderColor = SystemColors.ActiveBorder },
                Margin = new Padding(0, 0, 10, 0),
                TabStop = false
            };
            closeButton.Click += (sender, args) =>
            {
                currentDataInfo.NeedsDeletition = !currentDataInfo.NeedsDeletition;
                closeButton.BackgroundImage = currentDataInfo.NeedsDeletition ? ImageResources.CLOSE_193002_FF001C : ImageResources.CLOSE_193002;
            };

            TableLayoutPanel panel = new()
            {
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                RowStyles = { new(SizeType.AutoSize), new(SizeType.AutoSize), new(SizeType.AutoSize) },
                ColumnStyles = { new(SizeType.AutoSize) },
                Padding = new(0, 5, 0, 5),
                Margin = Padding.Empty,
                AllowDrop = true,
            };
            panel.DragEnter += (sender, args) => HandleDragEnter(args);
            panel.DragDrop += (sender, args) => HandleDragDrop(currentDataInfo, args);

            panel.Controls.Add(dragButton, 0, 0);
            panel.Controls.Add(closeButton, 1, 0);
            panel.Controls.Add(nameTextBox, 2, 0);

            this.RowsData.Add(new()
            {
                TabPage = tabPage,
                NameTextBox = nameTextBox,
                Info = currentDataInfo,
                Panel = panel,
            });

            return panel;
        }

        private void HandleDragEnter(DragEventArgs args)
        {
            var data = args.Data;
            if (data == null)
            {
                return;
            }

            var dataObj = data.GetData(typeof(RowDataInfo));
            args.Effect = dataObj == null ? DragDropEffects.None : DragDropEffects.Move;
        }

        private void HandleDragDrop(RowDataInfo dataInfo, DragEventArgs args)
        {
            var dataObj = args.Data?.GetData(typeof(RowDataInfo));
            if (dataObj is RowDataInfo draggedDataInfo)
            {
                this.SwapIndexes(dataInfo, draggedDataInfo);
            }
        }

        private void SwapIndexes(RowDataInfo firstDataInfo, RowDataInfo secondDataInfo)
        {
            var firstIndex = firstDataInfo.Index;
            var secondIndex = secondDataInfo.Index;

            firstDataInfo.Index = secondIndex;
            secondDataInfo.Index = firstIndex;

            RowsData.Sort((r1, r2) => Comparer<int>.Default.Compare(r1.Info.Index, r2.Info.Index));

            this.RedrawMainPanel();
        }

        private void RedrawMainPanel()
        {
            this.mainPanel.SuspendLayout();

            this.mainPanel.Controls.Clear();
            this.mainPanel.RowStyles.Clear();

            foreach (var rowData in RowsData.OrderBy(rowData => rowData.Info.Index))
            {
                this.mainPanel.RowStyles.Add(new(SizeType.AutoSize));
                this.mainPanel.Controls.Add(rowData.Panel, 0, rowData.Info.Index);
            }

            this.mainPanel.RowStyles.Add(new(SizeType.AutoSize));
            this.mainPanel.Controls.Add(this.acceptButton);

            this.mainPanel.ResumeLayout(true);
        }

        private void CloseWithResult(DialogResult result = DialogResult.Cancel)
        {
            this.DialogResult = result;
            this.Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                switch (keyData)
                {
                    case Keys.Escape:
                        this.CloseWithResult(DialogResult.Cancel);
                        return true;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
