namespace TiaUtilities.Generation.GridHandler.JSScript
{
    partial class GridScriptForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridScriptForm));
            mainPanel = new TableLayoutPanel();
            topLabel = new Label();
            miniToolStrip = new ToolStrip();
            executeLineButton = new Button();
            executeAllButton = new Button();
            autoFormatButton = new Button();
            toolStrip1 = new ToolStrip();
            buttonPanel = new TableLayoutPanel();
            splitContainer1 = new SplitContainer();
            jsonContextTextBox = new FastColoredTextBoxNS.FastColoredTextBox();
            jsonContextPanel = new TableLayoutPanel();
            jsonContextLabel = new Label();
            logPanel = new TableLayoutPanel();
            logLabel = new Label();
            logTextBox = new TextBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            scriptTabControl = new CustomControls.InteractableTabControl();
            variablesPanel = new TableLayoutPanel();
            variablesLabel = new Label();
            variablesTreeView = new TreeView();
            mainPanel.SuspendLayout();
            buttonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)jsonContextTextBox).BeginInit();
            jsonContextPanel.SuspendLayout();
            logPanel.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            variablesPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.AutoSize = true;
            mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainPanel.ColumnCount = 1;
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainPanel.Controls.Add(topLabel, 0, 0);
            mainPanel.Controls.Add(buttonPanel, 0, 3);
            mainPanel.Controls.Add(splitContainer1, 0, 1);
            mainPanel.Controls.Add(tableLayoutPanel1, 0, 2);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 0);
            mainPanel.Name = "mainPanel";
            mainPanel.RowCount = 4;
            mainPanel.RowStyles.Add(new RowStyle());
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            mainPanel.RowStyles.Add(new RowStyle());
            mainPanel.Size = new Size(800, 450);
            mainPanel.TabIndex = 0;
            // 
            // topLabel
            // 
            topLabel.AutoSize = true;
            topLabel.Dock = DockStyle.Top;
            topLabel.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            topLabel.Location = new Point(3, 0);
            topLabel.Name = "topLabel";
            topLabel.Size = new Size(794, 25);
            topLabel.TabIndex = 0;
            topLabel.Text = "JS Expression";
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // miniToolStrip
            // 
            miniToolStrip.AccessibleName = "Selezione nuovo elemento";
            miniToolStrip.AccessibleRole = AccessibleRole.ButtonDropDown;
            miniToolStrip.AutoSize = false;
            miniToolStrip.CanOverflow = false;
            miniToolStrip.Dock = DockStyle.None;
            miniToolStrip.GripStyle = ToolStripGripStyle.Hidden;
            miniToolStrip.Location = new Point(9, 3);
            miniToolStrip.Name = "miniToolStrip";
            miniToolStrip.RenderMode = ToolStripRenderMode.System;
            miniToolStrip.Size = new Size(111, 25);
            miniToolStrip.TabIndex = 3;
            // 
            // executeLineButton
            // 
            executeLineButton.AutoSize = true;
            executeLineButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            executeLineButton.Dock = DockStyle.Top;
            executeLineButton.FlatAppearance.BorderColor = SystemColors.ControlDarkDark;
            executeLineButton.FlatAppearance.BorderSize = 2;
            executeLineButton.FlatStyle = FlatStyle.Flat;
            executeLineButton.Font = new Font("Segoe UI", 12F);
            executeLineButton.Location = new Point(509, 3);
            executeLineButton.Name = "executeLineButton";
            executeLineButton.Size = new Size(248, 35);
            executeLineButton.TabIndex = 2;
            executeLineButton.Text = "ExecuteOne";
            executeLineButton.UseVisualStyleBackColor = true;
            // 
            // executeAllButton
            // 
            executeAllButton.AutoSize = true;
            executeAllButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            executeAllButton.Dock = DockStyle.Top;
            executeAllButton.FlatAppearance.BorderColor = SystemColors.ControlDarkDark;
            executeAllButton.FlatAppearance.BorderSize = 2;
            executeAllButton.FlatStyle = FlatStyle.Flat;
            executeAllButton.Font = new Font("Segoe UI", 12F);
            executeAllButton.Location = new Point(256, 3);
            executeAllButton.Name = "executeAllButton";
            executeAllButton.Size = new Size(247, 35);
            executeAllButton.TabIndex = 1;
            executeAllButton.Text = "ExecuteAll";
            executeAllButton.UseVisualStyleBackColor = true;
            // 
            // autoFormatButton
            // 
            autoFormatButton.AutoSize = true;
            autoFormatButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            autoFormatButton.Dock = DockStyle.Top;
            autoFormatButton.FlatAppearance.BorderColor = SystemColors.ControlDarkDark;
            autoFormatButton.FlatAppearance.BorderSize = 2;
            autoFormatButton.FlatStyle = FlatStyle.Flat;
            autoFormatButton.Font = new Font("Segoe UI", 12F);
            autoFormatButton.Location = new Point(3, 3);
            autoFormatButton.Name = "autoFormatButton";
            autoFormatButton.Size = new Size(247, 35);
            autoFormatButton.TabIndex = 0;
            autoFormatButton.Text = "AutoFormat";
            autoFormatButton.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            toolStrip1.Location = new Point(255, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.RenderMode = ToolStripRenderMode.System;
            toolStrip1.Size = new Size(111, 25);
            toolStrip1.TabIndex = 3;
            toolStrip1.Text = "toolStrip1";
            // 
            // buttonPanel
            // 
            buttonPanel.AutoSize = true;
            buttonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            buttonPanel.ColumnCount = 3;
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            buttonPanel.Controls.Add(executeLineButton, 2, 0);
            buttonPanel.Controls.Add(executeAllButton, 1, 0);
            buttonPanel.Controls.Add(autoFormatButton, 0, 0);
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.Location = new Point(20, 407);
            buttonPanel.Margin = new Padding(20, 2, 20, 2);
            buttonPanel.Name = "buttonPanel";
            buttonPanel.RowCount = 1;
            buttonPanel.RowStyles.Add(new RowStyle());
            buttonPanel.Size = new Size(760, 41);
            buttonPanel.TabIndex = 2;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(3, 28);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(logPanel);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(jsonContextPanel);
            splitContainer1.Size = new Size(794, 108);
            splitContainer1.SplitterDistance = 450;
            splitContainer1.SplitterWidth = 10;
            splitContainer1.TabIndex = 3;
            // 
            // jsonContextTextBox
            // 
            jsonContextTextBox.AutoCompleteBracketsList = new char[]
    {
    '(',
    ')',
    '{',
    '}',
    '[',
    ']',
    '"',
    '"',
    '\'',
    '\''
    };
            jsonContextTextBox.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*(?<range>:)\\s*(?<range>[^;]+);";
            jsonContextTextBox.AutoScrollMinSize = new Size(123, 14);
            jsonContextTextBox.AutoSize = true;
            jsonContextTextBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            jsonContextTextBox.BackBrush = null;
            jsonContextTextBox.BackColor = SystemColors.Control;
            jsonContextTextBox.CharHeight = 14;
            jsonContextTextBox.CharWidth = 8;
            jsonContextTextBox.DefaultMarkerSize = 8;
            jsonContextTextBox.DisabledColor = Color.FromArgb(100, 180, 180, 180);
            jsonContextTextBox.Dock = DockStyle.Fill;
            jsonContextTextBox.Font = new Font("Courier New", 9.75F);
            jsonContextTextBox.Hotkeys = resources.GetString("jsonContextTextBox.Hotkeys");
            jsonContextTextBox.IsReplaceMode = false;
            jsonContextTextBox.Location = new Point(3, 23);
            jsonContextTextBox.Name = "jsonContextTextBox";
            jsonContextTextBox.Paddings = new Padding(0);
            jsonContextTextBox.SelectionColor = Color.FromArgb(60, 0, 0, 255);
            jsonContextTextBox.ServiceColors = (FastColoredTextBoxNS.ServiceColors)resources.GetObject("jsonContextTextBox.ServiceColors");
            jsonContextTextBox.Size = new Size(328, 82);
            jsonContextTextBox.TabIndex = 0;
            jsonContextTextBox.Text = "JSON CONTEXT";
            jsonContextTextBox.Zoom = 100;
            // 
            // jsonContextPanel
            // 
            jsonContextPanel.AutoSize = true;
            jsonContextPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            jsonContextPanel.ColumnCount = 1;
            jsonContextPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            jsonContextPanel.Controls.Add(jsonContextLabel, 0, 0);
            jsonContextPanel.Controls.Add(jsonContextTextBox, 0, 1);
            jsonContextPanel.Dock = DockStyle.Fill;
            jsonContextPanel.Location = new Point(0, 0);
            jsonContextPanel.Name = "jsonContextPanel";
            jsonContextPanel.RowCount = 2;
            jsonContextPanel.RowStyles.Add(new RowStyle());
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            jsonContextPanel.Size = new Size(334, 108);
            jsonContextPanel.TabIndex = 0;
            // 
            // jsonContextLabel
            // 
            jsonContextLabel.AutoSize = true;
            jsonContextLabel.Dock = DockStyle.Top;
            jsonContextLabel.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            jsonContextLabel.Location = new Point(3, 0);
            jsonContextLabel.Name = "jsonContextLabel";
            jsonContextLabel.Size = new Size(328, 20);
            jsonContextLabel.TabIndex = 0;
            jsonContextLabel.Text = "JSON CONTEXT";
            jsonContextLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // logPanel
            // 
            logPanel.AutoSize = true;
            logPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            logPanel.ColumnCount = 1;
            logPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            logPanel.Controls.Add(logLabel, 0, 0);
            logPanel.Controls.Add(logTextBox, 0, 1);
            logPanel.Dock = DockStyle.Fill;
            logPanel.Location = new Point(0, 0);
            logPanel.Name = "logPanel";
            logPanel.RowCount = 2;
            logPanel.RowStyles.Add(new RowStyle());
            logPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            logPanel.Size = new Size(450, 108);
            logPanel.TabIndex = 0;
            // 
            // logLabel
            // 
            logLabel.AutoSize = true;
            logLabel.Dock = DockStyle.Top;
            logLabel.Font = new Font("Segoe UI", 11.25F);
            logLabel.Location = new Point(3, 0);
            logLabel.Name = "logLabel";
            logLabel.Size = new Size(444, 20);
            logLabel.TabIndex = 0;
            logLabel.Text = "Log";
            logLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // logTextBox
            // 
            logTextBox.BackColor = SystemColors.Control;
            logTextBox.BorderStyle = BorderStyle.None;
            logTextBox.Dock = DockStyle.Fill;
            logTextBox.Location = new Point(3, 23);
            logTextBox.Multiline = true;
            logTextBox.Name = "logTextBox";
            logTextBox.Size = new Size(444, 82);
            logTextBox.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(scriptTabControl, 1, 0);
            tableLayoutPanel1.Controls.Add(variablesPanel, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(3, 142);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(794, 260);
            tableLayoutPanel1.TabIndex = 4;
            // 
            // scriptTabControl
            // 
            scriptTabControl.Dock = DockStyle.Fill;
            scriptTabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            scriptTabControl.Location = new Point(136, 3);
            scriptTabControl.Name = "scriptTabControl";
            scriptTabControl.Padding = new Point(12, 5);
            scriptTabControl.RequireConfirmationBeforeClosing = false;
            scriptTabControl.SelectedIndex = 0;
            scriptTabControl.Size = new Size(655, 254);
            scriptTabControl.TabIndex = 0;
            // 
            // variablesPanel
            // 
            variablesPanel.AutoSize = true;
            variablesPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            variablesPanel.ColumnCount = 1;
            variablesPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            variablesPanel.Controls.Add(variablesLabel, 0, 0);
            variablesPanel.Controls.Add(variablesTreeView, 0, 1);
            variablesPanel.Dock = DockStyle.Left;
            variablesPanel.Location = new Point(3, 3);
            variablesPanel.Name = "variablesPanel";
            variablesPanel.RowCount = 2;
            variablesPanel.RowStyles.Add(new RowStyle());
            variablesPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            variablesPanel.Size = new Size(127, 254);
            variablesPanel.TabIndex = 1;
            // 
            // variablesLabel
            // 
            variablesLabel.AutoSize = true;
            variablesLabel.Dock = DockStyle.Top;
            variablesLabel.Font = new Font("Segoe UI", 11.25F);
            variablesLabel.Location = new Point(3, 0);
            variablesLabel.Name = "variablesLabel";
            variablesLabel.Size = new Size(121, 20);
            variablesLabel.TabIndex = 2;
            variablesLabel.Text = "Variables";
            variablesLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // variablesTreeView
            // 
            variablesTreeView.BackColor = SystemColors.Control;
            variablesTreeView.Dock = DockStyle.Left;
            variablesTreeView.Location = new Point(3, 23);
            variablesTreeView.Name = "variablesTreeView";
            variablesTreeView.Size = new Size(121, 228);
            variablesTreeView.TabIndex = 3;
            // 
            // GridScriptForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(mainPanel);
            Name = "GridScriptForm";
            Text = "GridScriptForm";
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            buttonPanel.ResumeLayout(false);
            buttonPanel.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)jsonContextTextBox).EndInit();
            jsonContextPanel.ResumeLayout(false);
            jsonContextPanel.PerformLayout();
            logPanel.ResumeLayout(false);
            logPanel.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            variablesPanel.ResumeLayout(false);
            variablesPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel mainPanel;
        private Label topLabel;
        private TableLayoutPanel buttonPanel;
        private Button executeLineButton;
        private Button executeAllButton;
        private Button autoFormatButton;
        private ToolStrip miniToolStrip;
        private ToolStrip toolStrip1;
        private SplitContainer splitContainer1;
        private FastColoredTextBoxNS.FastColoredTextBox jsonContextTextBox;
        private TableLayoutPanel logPanel;
        private Label logLabel;
        private TextBox logTextBox;
        private TableLayoutPanel jsonContextPanel;
        private Label jsonContextLabel;
        private TableLayoutPanel tableLayoutPanel1;
        private CustomControls.InteractableTabControl scriptTabControl;
        private TableLayoutPanel variablesPanel;
        private Label variablesLabel;
        private TreeView variablesTreeView;
    }
}