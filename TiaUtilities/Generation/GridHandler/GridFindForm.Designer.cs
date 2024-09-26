using TiaUtilities.CustomControls;

namespace TiaUtilities.Generation.GridHandler
{
    partial class GridFindForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            FindLabel = new Label();
            FindButton = new Button();
            ReplaceButton = new Button();
            MainTable = new TableLayoutPanel();
            FindTable = new TableLayoutPanel();
            FindTextBox = new RJTextBox();
            ReplaceTable = new TableLayoutPanel();
            ReplaceLabel = new Label();
            ReplaceTextBox = new RJTextBox();
            CheckboxPanel = new TableLayoutPanel();
            MatchCaseCheckBox = new CheckBox();
            ButtonPanel = new TableLayoutPanel();
            ReplaceAllButton = new Button();
            MainTable.SuspendLayout();
            FindTable.SuspendLayout();
            ReplaceTable.SuspendLayout();
            CheckboxPanel.SuspendLayout();
            ButtonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // FindLabel
            // 
            FindLabel.Dock = DockStyle.Fill;
            FindLabel.Font = new Font("Segoe UI", 9F);
            FindLabel.Location = new Point(0, 0);
            FindLabel.Margin = new Padding(0);
            FindLabel.Name = "FindLabel";
            FindLabel.Size = new Size(100, 29);
            FindLabel.TabIndex = 1;
            FindLabel.Text = "Find";
            FindLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FindButton
            // 
            FindButton.AutoSize = true;
            FindButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            FindButton.Dock = DockStyle.Fill;
            FindButton.FlatAppearance.BorderColor = Color.LightGray;
            FindButton.FlatStyle = FlatStyle.Flat;
            FindButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            FindButton.Location = new Point(20, 0);
            FindButton.Margin = new Padding(0);
            FindButton.Name = "FindButton";
            FindButton.Size = new Size(98, 26);
            FindButton.TabIndex = 4;
            FindButton.Text = "Find";
            FindButton.UseVisualStyleBackColor = true;
            // 
            // ReplaceButton
            // 
            ReplaceButton.AutoSize = true;
            ReplaceButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ReplaceButton.Dock = DockStyle.Fill;
            ReplaceButton.FlatAppearance.BorderColor = Color.LightGray;
            ReplaceButton.FlatStyle = FlatStyle.Flat;
            ReplaceButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            ReplaceButton.Location = new Point(167, 0);
            ReplaceButton.Margin = new Padding(0);
            ReplaceButton.Name = "ReplaceButton";
            ReplaceButton.Size = new Size(98, 26);
            ReplaceButton.TabIndex = 5;
            ReplaceButton.Text = "Replace";
            ReplaceButton.UseVisualStyleBackColor = true;
            // 
            // MainTable
            // 
            MainTable.AutoSize = true;
            MainTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            MainTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            MainTable.ColumnCount = 1;
            MainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            MainTable.Controls.Add(FindTable, 0, 0);
            MainTable.Controls.Add(ReplaceTable, 0, 1);
            MainTable.Controls.Add(CheckboxPanel, 0, 2);
            MainTable.Controls.Add(ButtonPanel, 0, 3);
            MainTable.Dock = DockStyle.Fill;
            MainTable.Location = new Point(0, 0);
            MainTable.Margin = new Padding(0);
            MainTable.Name = "MainTable";
            MainTable.Padding = new Padding(3);
            MainTable.RowCount = 4;
            MainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 49.99999F));
            MainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50.00001F));
            MainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            MainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            MainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            MainTable.Size = new Size(441, 121);
            MainTable.TabIndex = 7;
            // 
            // FindTable
            // 
            FindTable.AutoSize = true;
            FindTable.ColumnCount = 2;
            FindTable.ColumnStyles.Add(new ColumnStyle());
            FindTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.000008F));
            FindTable.Controls.Add(FindLabel, 0, 0);
            FindTable.Controls.Add(FindTextBox, 1, 0);
            FindTable.Dock = DockStyle.Fill;
            FindTable.GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
            FindTable.Location = new Point(4, 4);
            FindTable.Margin = new Padding(0);
            FindTable.Name = "FindTable";
            FindTable.RowCount = 1;
            FindTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            FindTable.Size = new Size(433, 29);
            FindTable.TabIndex = 0;
            // 
            // FindTextBox
            // 
            FindTextBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            FindTextBox.BackColor = SystemColors.Control;
            FindTextBox.BorderColor = Color.LightGray;
            FindTextBox.BorderFocusColor = Color.Gray;
            FindTextBox.BorderRadius = 0;
            FindTextBox.BorderSize = 2;
            FindTextBox.Dock = DockStyle.Fill;
            FindTextBox.Location = new Point(102, 2);
            FindTextBox.Margin = new Padding(2);
            FindTextBox.Multiline = false;
            FindTextBox.Name = "FindTextBox";
            FindTextBox.Padding = new Padding(4, 4, 10, 4);
            FindTextBox.PasswordChar = false;
            FindTextBox.ReadOnly = false;
            FindTextBox.ScrollBars = ScrollBars.None;
            FindTextBox.Size = new Size(329, 24);
            FindTextBox.TabIndex = 2;
            FindTextBox.TextAlign = HorizontalAlignment.Left;
            FindTextBox.TextLeftPadding = 4;
            FindTextBox.TextTopBottomPadding = 4;
            FindTextBox.UnderlineColor = Color.Transparent;
            FindTextBox.Underlined = false;
            // 
            // ReplaceTable
            // 
            ReplaceTable.AutoSize = true;
            ReplaceTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ReplaceTable.ColumnCount = 2;
            ReplaceTable.ColumnStyles.Add(new ColumnStyle());
            ReplaceTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            ReplaceTable.Controls.Add(ReplaceLabel, 0, 0);
            ReplaceTable.Controls.Add(ReplaceTextBox, 1, 0);
            ReplaceTable.Dock = DockStyle.Fill;
            ReplaceTable.GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
            ReplaceTable.Location = new Point(4, 34);
            ReplaceTable.Margin = new Padding(0);
            ReplaceTable.Name = "ReplaceTable";
            ReplaceTable.RowCount = 1;
            ReplaceTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            ReplaceTable.Size = new Size(433, 30);
            ReplaceTable.TabIndex = 1;
            // 
            // ReplaceLabel
            // 
            ReplaceLabel.Dock = DockStyle.Fill;
            ReplaceLabel.Font = new Font("Segoe UI", 9F);
            ReplaceLabel.Location = new Point(0, 0);
            ReplaceLabel.Margin = new Padding(0);
            ReplaceLabel.Name = "ReplaceLabel";
            ReplaceLabel.Size = new Size(100, 30);
            ReplaceLabel.TabIndex = 1;
            ReplaceLabel.Text = "Replace";
            ReplaceLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ReplaceTextBox
            // 
            ReplaceTextBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ReplaceTextBox.BackColor = SystemColors.Control;
            ReplaceTextBox.BorderColor = Color.LightGray;
            ReplaceTextBox.BorderFocusColor = Color.Gray;
            ReplaceTextBox.BorderRadius = 0;
            ReplaceTextBox.BorderSize = 2;
            ReplaceTextBox.Dock = DockStyle.Fill;
            ReplaceTextBox.Location = new Point(102, 2);
            ReplaceTextBox.Margin = new Padding(2);
            ReplaceTextBox.Multiline = false;
            ReplaceTextBox.Name = "ReplaceTextBox";
            ReplaceTextBox.Padding = new Padding(4, 4, 10, 4);
            ReplaceTextBox.PasswordChar = false;
            ReplaceTextBox.ReadOnly = false;
            ReplaceTextBox.ScrollBars = ScrollBars.None;
            ReplaceTextBox.Size = new Size(329, 24);
            ReplaceTextBox.TabIndex = 2;
            ReplaceTextBox.TextAlign = HorizontalAlignment.Left;
            ReplaceTextBox.TextLeftPadding = 4;
            ReplaceTextBox.TextTopBottomPadding = 4;
            ReplaceTextBox.UnderlineColor = Color.HotPink;
            ReplaceTextBox.Underlined = false;
            // 
            // CheckboxPanel
            // 
            CheckboxPanel.AutoSize = true;
            CheckboxPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            CheckboxPanel.ColumnCount = 5;
            CheckboxPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            CheckboxPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            CheckboxPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            CheckboxPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            CheckboxPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            CheckboxPanel.Controls.Add(MatchCaseCheckBox, 1, 0);
            CheckboxPanel.Dock = DockStyle.Fill;
            CheckboxPanel.Location = new Point(4, 65);
            CheckboxPanel.Margin = new Padding(0);
            CheckboxPanel.Name = "CheckboxPanel";
            CheckboxPanel.RowCount = 1;
            CheckboxPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            CheckboxPanel.Size = new Size(433, 25);
            CheckboxPanel.TabIndex = 3;
            // 
            // MatchCaseCheckBox
            // 
            MatchCaseCheckBox.AutoSize = true;
            MatchCaseCheckBox.Dock = DockStyle.Fill;
            MatchCaseCheckBox.FlatAppearance.BorderSize = 0;
            MatchCaseCheckBox.FlatStyle = FlatStyle.Flat;
            MatchCaseCheckBox.Location = new Point(23, 3);
            MatchCaseCheckBox.Name = "MatchCaseCheckBox";
            MatchCaseCheckBox.Size = new Size(92, 19);
            MatchCaseCheckBox.TabIndex = 0;
            MatchCaseCheckBox.Text = "Match case";
            MatchCaseCheckBox.UseVisualStyleBackColor = true;
            // 
            // ButtonPanel
            // 
            ButtonPanel.AutoSize = true;
            ButtonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ButtonPanel.ColumnCount = 7;
            ButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            ButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            ButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
            ButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            ButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
            ButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            ButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            ButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            ButtonPanel.Controls.Add(FindButton, 1, 0);
            ButtonPanel.Controls.Add(ReplaceButton, 3, 0);
            ButtonPanel.Controls.Add(ReplaceAllButton, 5, 0);
            ButtonPanel.Dock = DockStyle.Fill;
            ButtonPanel.Location = new Point(4, 91);
            ButtonPanel.Margin = new Padding(0);
            ButtonPanel.Name = "ButtonPanel";
            ButtonPanel.RowCount = 1;
            ButtonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            ButtonPanel.Size = new Size(433, 26);
            ButtonPanel.TabIndex = 2;
            // 
            // ReplaceAllButton
            // 
            ReplaceAllButton.AutoSize = true;
            ReplaceAllButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ReplaceAllButton.Dock = DockStyle.Fill;
            ReplaceAllButton.FlatAppearance.BorderColor = Color.LightGray;
            ReplaceAllButton.FlatStyle = FlatStyle.Flat;
            ReplaceAllButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            ReplaceAllButton.Location = new Point(314, 0);
            ReplaceAllButton.Margin = new Padding(0);
            ReplaceAllButton.Name = "ReplaceAllButton";
            ReplaceAllButton.Size = new Size(98, 26);
            ReplaceAllButton.TabIndex = 6;
            ReplaceAllButton.Text = "ReplaceAll";
            ReplaceAllButton.UseVisualStyleBackColor = true;
            // 
            // GridFindForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(441, 121);
            Controls.Add(MainTable);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "GridFindForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Grid - Find and Replace";
            MainTable.ResumeLayout(false);
            MainTable.PerformLayout();
            FindTable.ResumeLayout(false);
            ReplaceTable.ResumeLayout(false);
            CheckboxPanel.ResumeLayout(false);
            CheckboxPanel.PerformLayout();
            ButtonPanel.ResumeLayout(false);
            ButtonPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label FindLabel;
        private TableLayoutPanel MainTable;
        private TableLayoutPanel FindTable;
        private TableLayoutPanel ReplaceTable;
        private Label ReplaceLabel;
        private TableLayoutPanel ButtonPanel;
        public Button FindButton;
        public Button ReplaceButton;
        public Button ReplaceAllButton;
        private TableLayoutPanel CheckboxPanel;
        public CheckBox MatchCaseCheckBox;
        private RJTextBox FindTextBox;
        private RJTextBox ReplaceTextBox;
    }
}