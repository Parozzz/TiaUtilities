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
            mainPanel = new TableLayoutPanel();
            topLabel = new Label();
            infoSplitContainer = new SplitContainer();
            logPanel = new TableLayoutPanel();
            logLabel = new Label();
            logTextBox = new TextBox();
            jsonContextPanel = new TableLayoutPanel();
            jsonContextLabel = new Label();
            scriptSplitContainer = new SplitContainer();
            variablesPanel = new TableLayoutPanel();
            variablesLabel = new Label();
            variablesTreeView = new TreeView();
            scriptTabControl = new CustomControls.InteractableTabControl();
            buttonPanel = new TableLayoutPanel();
            executeLineButton = new Button();
            executeAllButton = new Button();
            autoFormatButton = new Button();
            miniToolStrip = new ToolStrip();
            toolStrip1 = new ToolStrip();
            centralSplitContainer = new SplitContainer();
            mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)infoSplitContainer).BeginInit();
            infoSplitContainer.Panel1.SuspendLayout();
            infoSplitContainer.Panel2.SuspendLayout();
            infoSplitContainer.SuspendLayout();
            logPanel.SuspendLayout();
            jsonContextPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)scriptSplitContainer).BeginInit();
            scriptSplitContainer.Panel1.SuspendLayout();
            scriptSplitContainer.Panel2.SuspendLayout();
            scriptSplitContainer.SuspendLayout();
            variablesPanel.SuspendLayout();
            buttonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)centralSplitContainer).BeginInit();
            centralSplitContainer.Panel1.SuspendLayout();
            centralSplitContainer.Panel2.SuspendLayout();
            centralSplitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.AutoSize = true;
            mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainPanel.ColumnCount = 1;
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainPanel.Controls.Add(topLabel, 0, 0);
            mainPanel.Controls.Add(centralSplitContainer, 0, 1);
            mainPanel.Controls.Add(buttonPanel, 0, 2);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 0);
            mainPanel.Name = "mainPanel";
            mainPanel.RowCount = 3;
            mainPanel.RowStyles.Add(new RowStyle());
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.RowStyles.Add(new RowStyle());
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
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
            // infoSplitContainer
            // 
            infoSplitContainer.Dock = DockStyle.Fill;
            infoSplitContainer.Location = new Point(0, 0);
            infoSplitContainer.Name = "infoSplitContainer";
            // 
            // infoSplitContainer.Panel1
            // 
            infoSplitContainer.Panel1.Controls.Add(logPanel);
            // 
            // infoSplitContainer.Panel2
            // 
            infoSplitContainer.Panel2.Controls.Add(jsonContextPanel);
            infoSplitContainer.Size = new Size(794, 75);
            infoSplitContainer.SplitterDistance = 450;
            infoSplitContainer.SplitterWidth = 10;
            infoSplitContainer.TabIndex = 3;
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
            logPanel.Size = new Size(450, 75);
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
            logTextBox.BorderStyle = BorderStyle.FixedSingle;
            logTextBox.Dock = DockStyle.Fill;
            logTextBox.Location = new Point(3, 23);
            logTextBox.Multiline = true;
            logTextBox.Name = "logTextBox";
            logTextBox.ScrollBars = ScrollBars.Both;
            logTextBox.Size = new Size(444, 49);
            logTextBox.TabIndex = 4;
            // 
            // jsonContextPanel
            // 
            jsonContextPanel.AutoSize = true;
            jsonContextPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            jsonContextPanel.ColumnCount = 1;
            jsonContextPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            jsonContextPanel.Controls.Add(jsonContextLabel, 0, 0);
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
            jsonContextPanel.Size = new Size(334, 75);
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
            // scriptSplitContainer
            // 
            scriptSplitContainer.Dock = DockStyle.Fill;
            scriptSplitContainer.Location = new Point(0, 0);
            scriptSplitContainer.Name = "scriptSplitContainer";
            // 
            // scriptSplitContainer.Panel1
            // 
            scriptSplitContainer.Panel1.Controls.Add(variablesPanel);
            // 
            // scriptSplitContainer.Panel2
            // 
            scriptSplitContainer.Panel2.Controls.Add(scriptTabControl);
            scriptSplitContainer.Size = new Size(794, 289);
            scriptSplitContainer.SplitterDistance = 150;
            scriptSplitContainer.SplitterWidth = 10;
            scriptSplitContainer.TabIndex = 1;
            // 
            // variablesPanel
            // 
            variablesPanel.AutoScroll = true;
            variablesPanel.AutoSize = true;
            variablesPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            variablesPanel.ColumnCount = 1;
            variablesPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            variablesPanel.Controls.Add(variablesLabel, 0, 0);
            variablesPanel.Controls.Add(variablesTreeView, 0, 1);
            variablesPanel.Dock = DockStyle.Fill;
            variablesPanel.Location = new Point(0, 0);
            variablesPanel.Name = "variablesPanel";
            variablesPanel.RowCount = 2;
            variablesPanel.RowStyles.Add(new RowStyle());
            variablesPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            variablesPanel.Size = new Size(150, 289);
            variablesPanel.TabIndex = 1;
            // 
            // variablesLabel
            // 
            variablesLabel.AutoSize = true;
            variablesLabel.Dock = DockStyle.Fill;
            variablesLabel.Font = new Font("Segoe UI", 11.25F);
            variablesLabel.Location = new Point(3, 0);
            variablesLabel.Name = "variablesLabel";
            variablesLabel.Size = new Size(144, 20);
            variablesLabel.TabIndex = 2;
            variablesLabel.Text = "Variables";
            variablesLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // variablesTreeView
            // 
            variablesTreeView.BackColor = SystemColors.Control;
            variablesTreeView.BorderStyle = BorderStyle.FixedSingle;
            variablesTreeView.Dock = DockStyle.Fill;
            variablesTreeView.Location = new Point(3, 23);
            variablesTreeView.Name = "variablesTreeView";
            variablesTreeView.Size = new Size(144, 263);
            variablesTreeView.TabIndex = 3;
            // 
            // scriptTabControl
            // 
            scriptTabControl.Dock = DockStyle.Fill;
            scriptTabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            scriptTabControl.Location = new Point(0, 0);
            scriptTabControl.Name = "scriptTabControl";
            scriptTabControl.Padding = new Point(12, 5);
            scriptTabControl.RequireConfirmationBeforeClosing = false;
            scriptTabControl.SelectedIndex = 0;
            scriptTabControl.Size = new Size(634, 289);
            scriptTabControl.TabIndex = 0;
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
            // toolStrip1
            // 
            toolStrip1.Location = new Point(255, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.RenderMode = ToolStripRenderMode.System;
            toolStrip1.Size = new Size(111, 25);
            toolStrip1.TabIndex = 3;
            toolStrip1.Text = "toolStrip1";
            // 
            // centralSplitContainer
            // 
            centralSplitContainer.Dock = DockStyle.Fill;
            centralSplitContainer.Location = new Point(3, 28);
            centralSplitContainer.Name = "centralSplitContainer";
            centralSplitContainer.Orientation = Orientation.Horizontal;
            // 
            // centralSplitContainer.Panel1
            // 
            centralSplitContainer.Panel1.Controls.Add(infoSplitContainer);
            // 
            // centralSplitContainer.Panel2
            // 
            centralSplitContainer.Panel2.Controls.Add(scriptSplitContainer);
            centralSplitContainer.Size = new Size(794, 374);
            centralSplitContainer.SplitterDistance = 75;
            centralSplitContainer.SplitterWidth = 10;
            centralSplitContainer.TabIndex = 1;
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
            infoSplitContainer.Panel1.ResumeLayout(false);
            infoSplitContainer.Panel1.PerformLayout();
            infoSplitContainer.Panel2.ResumeLayout(false);
            infoSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)infoSplitContainer).EndInit();
            infoSplitContainer.ResumeLayout(false);
            logPanel.ResumeLayout(false);
            logPanel.PerformLayout();
            jsonContextPanel.ResumeLayout(false);
            jsonContextPanel.PerformLayout();
            scriptSplitContainer.Panel1.ResumeLayout(false);
            scriptSplitContainer.Panel1.PerformLayout();
            scriptSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)scriptSplitContainer).EndInit();
            scriptSplitContainer.ResumeLayout(false);
            variablesPanel.ResumeLayout(false);
            variablesPanel.PerformLayout();
            buttonPanel.ResumeLayout(false);
            buttonPanel.PerformLayout();
            centralSplitContainer.Panel1.ResumeLayout(false);
            centralSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)centralSplitContainer).EndInit();
            centralSplitContainer.ResumeLayout(false);
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
        private SplitContainer infoSplitContainer;
        private TableLayoutPanel logPanel;
        private Label logLabel;
        private TextBox logTextBox;
        private TableLayoutPanel jsonContextPanel;
        private Label jsonContextLabel;
        private CustomControls.InteractableTabControl scriptTabControl;
        private TableLayoutPanel variablesPanel;
        private Label variablesLabel;
        private TreeView variablesTreeView;
        private SplitContainer scriptSplitContainer;
        private SplitContainer centralSplitContainer;
    }
}