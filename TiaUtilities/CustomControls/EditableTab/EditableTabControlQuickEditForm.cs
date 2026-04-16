using DocumentFormat.OpenXml.Drawing.Charts;
using TiaUtilities.Languages;
using TiaUtilities.Resources;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;
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
            this.mainPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
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

            LoadTabs();
            DrawMainPanel();
        }

        private void LoadTabs()
        {
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

        private void DrawMainPanel()
        {
            this.mainPanel.RowStyles.Clear();
            this.mainPanel.ColumnStyles.Clear();

            var columnCount = (this.RowsData.Count / 30) + 1;
            this.mainPanel.ColumnCount = columnCount;
            for(int x = 0; x < columnCount; x++)
            {
                ColumnStyle columnStyle = new(SizeType.AutoSize, 50f);
                this.mainPanel.ColumnStyles.Add(columnStyle);
            }

            var rowCount = this.RowsData.Count >= 30 ? 30 : (this.RowsData.Count % 30) + 1;
            this.mainPanel.RowCount = rowCount;
            for (int x = 0; x < rowCount; x++)
            {
                RowStyle rowStyle = new(SizeType.AutoSize);
                this.mainPanel.RowStyles.Add(rowStyle);
            }

            foreach (var rowData in this.RowsData.OrderBy(rowData => rowData.Info.Index))
            {
                var index = rowData.Info.Index;

                int column = index / 30;
                int row = index % 30;
                this.mainPanel.Controls.Add(rowData.Panel, column, row);
            }
            
            this.mainPanel.RowStyles.Add(new(SizeType.AutoSize));
            this.mainPanel.Controls.Add(this.acceptButton);

            this.mainPanel.SetColumnSpan(this.acceptButton, columnCount);
        }

        private Control CreateTabEditControl(TabPage tabPage, int index)
        {
            RowDataInfo currentDataInfo = new() { Index = index };

            Font regularFont = new(Font.SystemFontName, 12f, FontStyle.Regular);

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
                Anchor = AnchorStyles.None,
                MaximumSize = new(buttonsSize*3/4, buttonsSize*3/4),
                BackColor = this.BackColor,
                BackgroundImage = ImageResources.DRAG_8187494,
                BackgroundImageLayout = ImageLayout.Stretch,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.Transparent, MouseDownBackColor = Color.Transparent },
                Padding = Padding.Empty,
                Margin = new Padding(0, 0, 3, 0),
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
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.Transparent, MouseDownBackColor = Color.Transparent },
                Padding = Padding.Empty,
                Margin = new Padding(0, 0, 5, 0),
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
                Padding = new(0, 1, 0, 1),
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
                (dataInfo.Index, draggedDataInfo.Index) = (draggedDataInfo.Index, dataInfo.Index);

                this.mainPanel.SuspendLayout();
                foreach (var rowData in RowsData.OrderBy(r => r.Info.Index))
                {
                    var index = rowData.Info.Index;
                    int column = index / 30;
                    int row = index % 30;

                    this.mainPanel.SetCellPosition(rowData.Panel, new(column, row));
                }
                this.mainPanel.ResumeLayout(true);
            }
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
