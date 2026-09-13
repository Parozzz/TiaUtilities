namespace TiaUtilities
{
    partial class MainForm
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            TopMenuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            saveMenuItem = new ToolStripMenuItem();
            loadToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            programMenuItem = new ToolStripMenuItem();
            programSettingsMenuItem = new ToolStripMenuItem();
            testToolStripMenuItem = new ToolStripMenuItem();
            importXMLToolStripMenuItem = new ToolStripMenuItem();
            jSToolStripMenuItem = new ToolStripMenuItem();
            sampleXMLMenuItem = new ToolStripMenuItem();
            testProjectMenuItem = new ToolStripMenuItem();
            svgToolStripMenuItem = new ToolStripMenuItem();
            dbVisualizationMenuItem = new ToolStripMenuItem();
            exportAllMembersToolStripMenuItem = new ToolStripMenuItem();
            createTextListsExcelToolStripMenuItem = new ToolStripMenuItem();
            questionMarkMenuItem = new ToolStripMenuItem();
            LogWorker = new System.ComponentModel.BackgroundWorker();
            MainLayoutPanel = new TableLayoutPanel();
            bottomPanel = new FlowLayoutPanel();
            ioGenButton = new Button();
            alarmGenButton = new Button();
            duplicateDBButton = new Button();
            testStepSettingsMenuItem = new ToolStripMenuItem();
            TopMenuStrip.SuspendLayout();
            MainLayoutPanel.SuspendLayout();
            bottomPanel.SuspendLayout();
            SuspendLayout();
            // 
            // TopMenuStrip
            // 
            TopMenuStrip.ImageScalingSize = new Size(20, 20);
            TopMenuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, programMenuItem, testToolStripMenuItem, questionMarkMenuItem });
            TopMenuStrip.Location = new Point(0, 0);
            TopMenuStrip.Name = "TopMenuStrip";
            TopMenuStrip.Padding = new Padding(6, 3, 0, 3);
            TopMenuStrip.Size = new Size(1035, 38);
            TopMenuStrip.TabIndex = 11;
            TopMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { saveMenuItem, loadToolStripMenuItem, toolStripSeparator2 });
            fileToolStripMenuItem.Font = new Font("Segoe UI", 12F);
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(56, 32);
            fileToolStripMenuItem.Text = "File";
            // 
            // saveMenuItem
            // 
            saveMenuItem.Name = "saveMenuItem";
            saveMenuItem.Size = new Size(141, 32);
            saveMenuItem.Text = "Save";
            // 
            // loadToolStripMenuItem
            // 
            loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            loadToolStripMenuItem.Size = new Size(141, 32);
            loadToolStripMenuItem.Text = "Load";
            loadToolStripMenuItem.Click += LoadToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(138, 6);
            // 
            // programMenuItem
            // 
            programMenuItem.DropDownItems.AddRange(new ToolStripItem[] { programSettingsMenuItem });
            programMenuItem.Font = new Font("Segoe UI", 12F);
            programMenuItem.Name = "programMenuItem";
            programMenuItem.Size = new Size(102, 32);
            programMenuItem.Text = "Program";
            // 
            // programSettingsMenuItem
            // 
            programSettingsMenuItem.Name = "programSettingsMenuItem";
            programSettingsMenuItem.Size = new Size(169, 32);
            programSettingsMenuItem.Text = "Settings";
            // 
            // testToolStripMenuItem
            // 
            testToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { importXMLToolStripMenuItem, jSToolStripMenuItem, sampleXMLMenuItem, testProjectMenuItem, svgToolStripMenuItem, dbVisualizationMenuItem, exportAllMembersToolStripMenuItem, createTextListsExcelToolStripMenuItem, testStepSettingsMenuItem });
            testToolStripMenuItem.Font = new Font("Segoe UI", 12F);
            testToolStripMenuItem.Name = "testToolStripMenuItem";
            testToolStripMenuItem.Size = new Size(59, 32);
            testToolStripMenuItem.Text = "Test";
            // 
            // importXMLToolStripMenuItem
            // 
            importXMLToolStripMenuItem.Name = "importXMLToolStripMenuItem";
            importXMLToolStripMenuItem.Size = new Size(268, 32);
            importXMLToolStripMenuItem.Text = "Import XML";
            importXMLToolStripMenuItem.Click += ImportXMLToolStripMenuItem_Click;
            // 
            // jSToolStripMenuItem
            // 
            jSToolStripMenuItem.Name = "jSToolStripMenuItem";
            jSToolStripMenuItem.Size = new Size(268, 32);
            jSToolStripMenuItem.Text = "JS";
            jSToolStripMenuItem.Click += JSToolStripMenuItem_Click;
            // 
            // sampleXMLMenuItem
            // 
            sampleXMLMenuItem.Name = "sampleXMLMenuItem";
            sampleXMLMenuItem.Size = new Size(268, 32);
            sampleXMLMenuItem.Text = "Sample XML";
            // 
            // testProjectMenuItem
            // 
            testProjectMenuItem.Name = "testProjectMenuItem";
            testProjectMenuItem.Size = new Size(268, 32);
            testProjectMenuItem.Text = "Project";
            // 
            // svgToolStripMenuItem
            // 
            svgToolStripMenuItem.Name = "svgToolStripMenuItem";
            svgToolStripMenuItem.Size = new Size(268, 32);
            svgToolStripMenuItem.Text = "Svg";
            svgToolStripMenuItem.Click += SvgToolStripMenuItem_Click;
            // 
            // dbVisualizationMenuItem
            // 
            dbVisualizationMenuItem.Name = "dbVisualizationMenuItem";
            dbVisualizationMenuItem.Size = new Size(268, 32);
            dbVisualizationMenuItem.Text = "DbVisualization";
            dbVisualizationMenuItem.Click += dbVisualizationMenuItem_Click;
            // 
            // exportAllMembersToolStripMenuItem
            // 
            exportAllMembersToolStripMenuItem.Name = "exportAllMembersToolStripMenuItem";
            exportAllMembersToolStripMenuItem.Size = new Size(268, 32);
            exportAllMembersToolStripMenuItem.Text = "ExportAllMembers";
            exportAllMembersToolStripMenuItem.Click += exportAllMembersToolStripMenuItem_Click;
            // 
            // createTextListsExcelToolStripMenuItem
            // 
            createTextListsExcelToolStripMenuItem.Name = "createTextListsExcelToolStripMenuItem";
            createTextListsExcelToolStripMenuItem.Size = new Size(268, 32);
            createTextListsExcelToolStripMenuItem.Text = "CreateTextListsExcel";
            createTextListsExcelToolStripMenuItem.Click += CreateTextListsExcelToolStripMenuItem_Click;
            // 
            // questionMarkMenuItem
            // 
            questionMarkMenuItem.Font = new Font("Segoe UI", 12F);
            questionMarkMenuItem.Name = "questionMarkMenuItem";
            questionMarkMenuItem.Size = new Size(35, 32);
            questionMarkMenuItem.Text = "?";
            questionMarkMenuItem.Click += QuestionMarkMenuItem_Click;
            // 
            // MainLayoutPanel
            // 
            MainLayoutPanel.AutoSize = true;
            MainLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            MainLayoutPanel.ColumnCount = 1;
            MainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            MainLayoutPanel.Controls.Add(TopMenuStrip, 0, 0);
            MainLayoutPanel.Controls.Add(bottomPanel, 0, 1);
            MainLayoutPanel.Dock = DockStyle.Fill;
            MainLayoutPanel.Location = new Point(0, 0);
            MainLayoutPanel.Margin = new Padding(5, 4, 5, 4);
            MainLayoutPanel.Name = "MainLayoutPanel";
            MainLayoutPanel.RowCount = 2;
            MainLayoutPanel.RowStyles.Add(new RowStyle());
            MainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            MainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
            MainLayoutPanel.Size = new Size(1035, 441);
            MainLayoutPanel.TabIndex = 14;
            // 
            // bottomPanel
            // 
            bottomPanel.Anchor = AnchorStyles.None;
            bottomPanel.AutoSize = true;
            bottomPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            bottomPanel.Controls.Add(ioGenButton);
            bottomPanel.Controls.Add(alarmGenButton);
            bottomPanel.Controls.Add(duplicateDBButton);
            bottomPanel.Location = new Point(57, 65);
            bottomPanel.Margin = new Padding(3, 4, 3, 4);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new Size(920, 349);
            bottomPanel.TabIndex = 1;
            // 
            // ioGenButton
            // 
            ioGenButton.AutoSize = true;
            ioGenButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ioGenButton.BackColor = SystemColors.ControlDark;
            ioGenButton.FlatStyle = FlatStyle.Flat;
            ioGenButton.Font = new Font("Segoe UI", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ioGenButton.ForeColor = SystemColors.ButtonFace;
            ioGenButton.Location = new Point(3, 4);
            ioGenButton.Margin = new Padding(3, 4, 3, 4);
            ioGenButton.MinimumSize = new Size(293, 341);
            ioGenButton.Name = "ioGenButton";
            ioGenButton.Size = new Size(293, 341);
            ioGenButton.TabIndex = 0;
            ioGenButton.Text = "IO Generation";
            ioGenButton.TextAlign = ContentAlignment.BottomCenter;
            ioGenButton.UseVisualStyleBackColor = false;
            // 
            // alarmGenButton
            // 
            alarmGenButton.AutoSize = true;
            alarmGenButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            alarmGenButton.BackColor = SystemColors.ControlDark;
            alarmGenButton.FlatStyle = FlatStyle.Flat;
            alarmGenButton.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            alarmGenButton.ForeColor = SystemColors.ButtonFace;
            alarmGenButton.Location = new Point(302, 4);
            alarmGenButton.Margin = new Padding(3, 4, 3, 4);
            alarmGenButton.MinimumSize = new Size(293, 341);
            alarmGenButton.Name = "alarmGenButton";
            alarmGenButton.Size = new Size(316, 341);
            alarmGenButton.TabIndex = 1;
            alarmGenButton.Text = "Alarm Generation";
            alarmGenButton.TextAlign = ContentAlignment.BottomCenter;
            alarmGenButton.UseVisualStyleBackColor = false;
            // 
            // duplicateDBButton
            // 
            duplicateDBButton.AutoSize = true;
            duplicateDBButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            duplicateDBButton.BackColor = SystemColors.ControlDark;
            duplicateDBButton.FlatStyle = FlatStyle.Flat;
            duplicateDBButton.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            duplicateDBButton.ForeColor = SystemColors.ButtonFace;
            duplicateDBButton.Location = new Point(624, 4);
            duplicateDBButton.Margin = new Padding(3, 4, 3, 4);
            duplicateDBButton.MinimumSize = new Size(293, 341);
            duplicateDBButton.Name = "duplicateDBButton";
            duplicateDBButton.Size = new Size(293, 341);
            duplicateDBButton.TabIndex = 2;
            duplicateDBButton.Text = "Duplicate DB";
            duplicateDBButton.TextAlign = ContentAlignment.BottomCenter;
            duplicateDBButton.UseVisualStyleBackColor = false;
            // 
            // testStepSettingsMenuItem
            // 
            testStepSettingsMenuItem.Name = "testStepSettingsMenuItem";
            testStepSettingsMenuItem.Size = new Size(268, 32);
            testStepSettingsMenuItem.Text = "TestStepSettings";
            testStepSettingsMenuItem.Click += TestStepSettingsMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1035, 441);
            Controls.Add(MainLayoutPanel);
            MainMenuStrip = TopMenuStrip;
            Margin = new Padding(5, 4, 5, 4);
            Name = "MainForm";
            Text = "AppoggioMan";
            TopMenuStrip.ResumeLayout(false);
            TopMenuStrip.PerformLayout();
            MainLayoutPanel.ResumeLayout(false);
            MainLayoutPanel.PerformLayout();
            bottomPanel.ResumeLayout(false);
            bottomPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.MenuStrip TopMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jSToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker LogWorker;
        private System.Windows.Forms.TableLayoutPanel MainLayoutPanel;
        private ToolStripMenuItem sampleXMLMenuItem;
        private ToolStripMenuItem testProjectMenuItem;
        private ToolStripMenuItem loadToolStripMenuItem;
        private ToolStripMenuItem svgToolStripMenuItem;
        private ToolStripMenuItem dbVisualizationMenuItem;
        private ToolStripMenuItem saveMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem programMenuItem;
        private ToolStripMenuItem programSettingsMenuItem;
        private FlowLayoutPanel ioGenPanel;
        private Button ioGenButton;
        private FlowLayoutPanel bottomPanel;
        private Button alarmGenButton;
        private Button button1;
        private Button duplicateDBButton;
        private ToolStripMenuItem exportAllMembersToolStripMenuItem;
        private ToolStripMenuItem questionMarkMenuItem;
        private ToolStripMenuItem createTextListsExcelToolStripMenuItem;
        private ToolStripMenuItem testStepSettingsMenuItem;
    }
}

