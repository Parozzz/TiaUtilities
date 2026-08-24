using TiaUtilities.CustomControls.EditableTab;

namespace TiaUtilities.JSScript
{
    partial class JSScriptForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JSScriptForm));
            mainPanel = new TableLayoutPanel();
            toolsFlowPanel = new FlowLayoutPanel();
            toolsLabel = new Label();
            toolsSplitter = new Splitter();
            runButton = new Button();
            bottom = new SplitContainer();
            left = new SplitContainer();
            variablesPanel = new TableLayoutPanel();
            variablesLabel = new Label();
            variablesTreeView = new TreeView();
            right = new SplitContainer();
            scriptTabControl = new EditableTabControl();
            jsonContextPanel = new TableLayoutPanel();
            jsonContextLabel = new Label();
            jsonContextScintilla = new ScintillaNET.Scintilla();
            logPanel = new TableLayoutPanel();
            consoleLabel = new Label();
            loggerScintilla = new ScintillaNET.Scintilla();
            miniToolStrip = new ToolStrip();
            toolStrip1 = new ToolStrip();
            mainPanel.SuspendLayout();
            toolsFlowPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bottom).BeginInit();
            bottom.Panel1.SuspendLayout();
            bottom.Panel2.SuspendLayout();
            bottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)left).BeginInit();
            left.Panel1.SuspendLayout();
            left.Panel2.SuspendLayout();
            left.SuspendLayout();
            variablesPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)right).BeginInit();
            right.Panel1.SuspendLayout();
            right.Panel2.SuspendLayout();
            right.SuspendLayout();
            jsonContextPanel.SuspendLayout();
            logPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.AutoSize = true;
            mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainPanel.ColumnCount = 1;
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainPanel.Controls.Add(toolsFlowPanel, 0, 0);
            mainPanel.Controls.Add(bottom, 0, 1);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 0);
            mainPanel.Margin = new Padding(3, 4, 3, 4);
            mainPanel.Name = "mainPanel";
            mainPanel.RowCount = 2;
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.Size = new Size(1182, 753);
            mainPanel.TabIndex = 0;
            // 
            // toolsFlowPanel
            // 
            toolsFlowPanel.AutoSize = true;
            toolsFlowPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            toolsFlowPanel.Controls.Add(toolsLabel);
            toolsFlowPanel.Controls.Add(toolsSplitter);
            toolsFlowPanel.Controls.Add(runButton);
            toolsFlowPanel.Dock = DockStyle.Fill;
            toolsFlowPanel.Location = new Point(0, 0);
            toolsFlowPanel.Margin = new Padding(0);
            toolsFlowPanel.Name = "toolsFlowPanel";
            toolsFlowPanel.Size = new Size(1182, 40);
            toolsFlowPanel.TabIndex = 2;
            // 
            // toolsLabel
            // 
            toolsLabel.AutoSize = true;
            toolsLabel.Dock = DockStyle.Fill;
            toolsLabel.Location = new Point(3, 0);
            toolsLabel.Name = "toolsLabel";
            toolsLabel.Size = new Size(44, 41);
            toolsLabel.TabIndex = 1;
            toolsLabel.Text = "Tools";
            toolsLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // toolsSplitter
            // 
            toolsSplitter.BackColor = Color.Black;
            toolsSplitter.Cursor = Cursors.Hand;
            toolsSplitter.Location = new Point(53, 3);
            toolsSplitter.Name = "toolsSplitter";
            toolsSplitter.Size = new Size(4, 35);
            toolsSplitter.TabIndex = 2;
            toolsSplitter.TabStop = false;
            // 
            // runButton
            // 
            runButton.BackgroundImage = Properties.Resources.play_6444203_007435;
            runButton.BackgroundImageLayout = ImageLayout.Zoom;
            runButton.FlatAppearance.BorderSize = 0;
            runButton.FlatAppearance.MouseDownBackColor = Color.DarkGray;
            runButton.FlatAppearance.MouseOverBackColor = Color.Gainsboro;
            runButton.FlatStyle = FlatStyle.Flat;
            runButton.ImageAlign = ContentAlignment.MiddleLeft;
            runButton.Location = new Point(63, 3);
            runButton.Name = "runButton";
            runButton.Size = new Size(35, 35);
            runButton.TabIndex = 0;
            runButton.UseVisualStyleBackColor = true;
            // 
            // bottom
            // 
            bottom.Dock = DockStyle.Fill;
            bottom.Location = new Point(3, 44);
            bottom.Margin = new Padding(3, 4, 3, 4);
            bottom.Name = "bottom";
            bottom.Orientation = Orientation.Horizontal;
            // 
            // bottom.Panel1
            // 
            bottom.Panel1.Controls.Add(left);
            // 
            // bottom.Panel2
            // 
            bottom.Panel2.Controls.Add(logPanel);
            bottom.Size = new Size(1176, 705);
            bottom.SplitterDistance = 572;
            bottom.SplitterWidth = 13;
            bottom.TabIndex = 1;
            // 
            // left
            // 
            left.Dock = DockStyle.Fill;
            left.Location = new Point(0, 0);
            left.Margin = new Padding(3, 4, 3, 4);
            left.Name = "left";
            // 
            // left.Panel1
            // 
            left.Panel1.Controls.Add(variablesPanel);
            // 
            // left.Panel2
            // 
            left.Panel2.Controls.Add(right);
            left.Size = new Size(1176, 572);
            left.SplitterDistance = 221;
            left.SplitterWidth = 10;
            left.TabIndex = 1;
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
            variablesPanel.Margin = new Padding(3, 4, 3, 4);
            variablesPanel.Name = "variablesPanel";
            variablesPanel.RowCount = 2;
            variablesPanel.RowStyles.Add(new RowStyle());
            variablesPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            variablesPanel.Size = new Size(221, 572);
            variablesPanel.TabIndex = 1;
            // 
            // variablesLabel
            // 
            variablesLabel.AutoSize = true;
            variablesLabel.Dock = DockStyle.Fill;
            variablesLabel.Font = new Font("Segoe UI", 11.25F);
            variablesLabel.Location = new Point(3, 0);
            variablesLabel.Name = "variablesLabel";
            variablesLabel.Size = new Size(215, 25);
            variablesLabel.TabIndex = 2;
            variablesLabel.Text = "Variables";
            variablesLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // variablesTreeView
            // 
            variablesTreeView.BackColor = SystemColors.Control;
            variablesTreeView.BorderStyle = BorderStyle.FixedSingle;
            variablesTreeView.Dock = DockStyle.Fill;
            variablesTreeView.Location = new Point(3, 29);
            variablesTreeView.Margin = new Padding(3, 4, 3, 4);
            variablesTreeView.Name = "variablesTreeView";
            variablesTreeView.Size = new Size(215, 539);
            variablesTreeView.TabIndex = 3;
            // 
            // right
            // 
            right.Dock = DockStyle.Fill;
            right.Location = new Point(0, 0);
            right.Name = "right";
            // 
            // right.Panel1
            // 
            right.Panel1.Controls.Add(scriptTabControl);
            // 
            // right.Panel2
            // 
            right.Panel2.Controls.Add(jsonContextPanel);
            right.Size = new Size(945, 572);
            right.SplitterDistance = 649;
            right.TabIndex = 0;
            // 
            // scriptTabControl
            // 
            scriptTabControl.AllowDrop = true;
            scriptTabControl.Dock = DockStyle.Fill;
            scriptTabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            scriptTabControl.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Italic);
            scriptTabControl.Location = new Point(0, 0);
            scriptTabControl.Name = "scriptTabControl";
            scriptTabControl.Padding = new Point(12, 5);
            scriptTabControl.RequireConfirmationBeforeClosing = false;
            scriptTabControl.SelectedIndex = 0;
            scriptTabControl.Size = new Size(649, 572);
            scriptTabControl.TabIndex = 0;
            // 
            // jsonContextPanel
            // 
            jsonContextPanel.AutoSize = true;
            jsonContextPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            jsonContextPanel.ColumnCount = 1;
            jsonContextPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            jsonContextPanel.Controls.Add(jsonContextLabel, 0, 0);
            jsonContextPanel.Controls.Add(jsonContextScintilla, 0, 1);
            jsonContextPanel.Dock = DockStyle.Fill;
            jsonContextPanel.Location = new Point(0, 0);
            jsonContextPanel.Margin = new Padding(3, 4, 3, 4);
            jsonContextPanel.Name = "jsonContextPanel";
            jsonContextPanel.RowCount = 2;
            jsonContextPanel.RowStyles.Add(new RowStyle());
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
            jsonContextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
            jsonContextPanel.Size = new Size(292, 572);
            jsonContextPanel.TabIndex = 0;
            // 
            // jsonContextLabel
            // 
            jsonContextLabel.AutoSize = true;
            jsonContextLabel.Dock = DockStyle.Top;
            jsonContextLabel.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            jsonContextLabel.Location = new Point(3, 0);
            jsonContextLabel.Name = "jsonContextLabel";
            jsonContextLabel.Size = new Size(286, 25);
            jsonContextLabel.TabIndex = 0;
            jsonContextLabel.Text = "JSON CONTEXT";
            jsonContextLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // jsonContextScintilla
            // 
            jsonContextScintilla.AutocompleteListSelectedBackColor = Color.FromArgb(0, 120, 212);
            jsonContextScintilla.Dock = DockStyle.Fill;
            jsonContextScintilla.Font = new Font("Verdana", 10F);
            jsonContextScintilla.LexerName = null;
            jsonContextScintilla.Location = new Point(3, 28);
            jsonContextScintilla.Name = "jsonContextScintilla";
            jsonContextScintilla.ScrollWidth = 57;
            jsonContextScintilla.Size = new Size(286, 541);
            jsonContextScintilla.TabIndex = 1;
            // 
            // logPanel
            // 
            logPanel.AutoSize = true;
            logPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            logPanel.ColumnCount = 1;
            logPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            logPanel.Controls.Add(consoleLabel, 0, 0);
            logPanel.Controls.Add(loggerScintilla, 0, 1);
            logPanel.Dock = DockStyle.Fill;
            logPanel.Location = new Point(0, 0);
            logPanel.Margin = new Padding(3, 4, 3, 4);
            logPanel.Name = "logPanel";
            logPanel.RowCount = 2;
            logPanel.RowStyles.Add(new RowStyle());
            logPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            logPanel.Size = new Size(1176, 120);
            logPanel.TabIndex = 0;
            // 
            // consoleLabel
            // 
            consoleLabel.AutoSize = true;
            consoleLabel.Dock = DockStyle.Top;
            consoleLabel.Font = new Font("Segoe UI", 11.25F);
            consoleLabel.Location = new Point(3, 0);
            consoleLabel.Name = "consoleLabel";
            consoleLabel.Size = new Size(1170, 25);
            consoleLabel.TabIndex = 0;
            consoleLabel.Text = "Console";
            consoleLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // loggerScintilla
            // 
            loggerScintilla.AutocompleteListSelectedBackColor = Color.FromArgb(0, 120, 212);
            loggerScintilla.Dock = DockStyle.Fill;
            loggerScintilla.LexerName = null;
            loggerScintilla.Location = new Point(3, 28);
            loggerScintilla.Name = "loggerScintilla";
            loggerScintilla.ScrollWidth = 57;
            loggerScintilla.Size = new Size(1170, 89);
            loggerScintilla.TabIndex = 1;
            // 
            // miniToolStrip
            // 
            miniToolStrip.AccessibleName = "Selezione nuovo elemento";
            miniToolStrip.AccessibleRole = AccessibleRole.ButtonDropDown;
            miniToolStrip.AutoSize = false;
            miniToolStrip.CanOverflow = false;
            miniToolStrip.Dock = DockStyle.None;
            miniToolStrip.GripStyle = ToolStripGripStyle.Hidden;
            miniToolStrip.ImageScalingSize = new Size(20, 20);
            miniToolStrip.Location = new Point(9, 3);
            miniToolStrip.Name = "miniToolStrip";
            miniToolStrip.RenderMode = ToolStripRenderMode.System;
            miniToolStrip.Size = new Size(111, 25);
            miniToolStrip.TabIndex = 3;
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new Size(20, 20);
            toolStrip1.Location = new Point(255, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.RenderMode = ToolStripRenderMode.System;
            toolStrip1.Size = new Size(111, 25);
            toolStrip1.TabIndex = 3;
            toolStrip1.Text = "toolStrip1";
            // 
            // JSScriptForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1182, 753);
            Controls.Add(mainPanel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            Name = "JSScriptForm";
            Text = "Javascript Editor";
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            toolsFlowPanel.ResumeLayout(false);
            toolsFlowPanel.PerformLayout();
            bottom.Panel1.ResumeLayout(false);
            bottom.Panel2.ResumeLayout(false);
            bottom.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bottom).EndInit();
            bottom.ResumeLayout(false);
            left.Panel1.ResumeLayout(false);
            left.Panel1.PerformLayout();
            left.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)left).EndInit();
            left.ResumeLayout(false);
            variablesPanel.ResumeLayout(false);
            variablesPanel.PerformLayout();
            right.Panel1.ResumeLayout(false);
            right.Panel2.ResumeLayout(false);
            right.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)right).EndInit();
            right.ResumeLayout(false);
            jsonContextPanel.ResumeLayout(false);
            jsonContextPanel.PerformLayout();
            logPanel.ResumeLayout(false);
            logPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel mainPanel;
        private TableLayoutPanel buttonPanel;
        private ToolStrip miniToolStrip;
        private ToolStrip toolStrip1;
        private TableLayoutPanel logPanel;
        private Label consoleLabel;
        private TableLayoutPanel jsonContextPanel;
        private Label jsonContextLabel;
        private EditableTabControl scriptTabControl;
        private TableLayoutPanel variablesPanel;
        private Label variablesLabel;
        private TreeView variablesTreeView;
        private SplitContainer left;
        private SplitContainer bottom;
        private ScintillaNET.Scintilla jsonContextScintilla;
        private ScintillaNET.Scintilla loggerScintilla;
        private SplitContainer right;
        private FlowLayoutPanel toolsFlowPanel;
        private Button runButton;
        private Label toolsLabel;
        private Splitter toolsSplitter;
    }
}