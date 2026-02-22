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
            dbDuplicationMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            generateIOMenuItem = new ToolStripMenuItem();
            generateAlarmsMenuItem = new ToolStripMenuItem();
            testToolStripMenuItem = new ToolStripMenuItem();
            importXMLToolStripMenuItem = new ToolStripMenuItem();
            jSToolStripMenuItem = new ToolStripMenuItem();
            sampleXMLMenuItem = new ToolStripMenuItem();
            testProjectMenuItem = new ToolStripMenuItem();
            svgToolStripMenuItem = new ToolStripMenuItem();
            dbVisualizationMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            LogWorker = new System.ComponentModel.BackgroundWorker();
            MainLayoutPanel = new TableLayoutPanel();
            TopMenuStrip.SuspendLayout();
            MainLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // TopMenuStrip
            // 
            TopMenuStrip.ImageScalingSize = new Size(20, 20);
            TopMenuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, programMenuItem, dbDuplicationMenuItem, toolStripMenuItem1, generateIOMenuItem, generateAlarmsMenuItem, testToolStripMenuItem });
            TopMenuStrip.Location = new Point(0, 0);
            TopMenuStrip.Name = "TopMenuStrip";
            TopMenuStrip.Padding = new Padding(5, 2, 0, 2);
            TopMenuStrip.Size = new Size(906, 29);
            TopMenuStrip.TabIndex = 11;
            TopMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { saveMenuItem, loadToolStripMenuItem, toolStripSeparator2 });
            fileToolStripMenuItem.Font = new Font("Segoe UI", 12F);
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 25);
            fileToolStripMenuItem.Text = "File";
            // 
            // saveMenuItem
            // 
            saveMenuItem.Name = "saveMenuItem";
            saveMenuItem.Size = new Size(114, 26);
            saveMenuItem.Text = "Save";
            // 
            // loadToolStripMenuItem
            // 
            loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            loadToolStripMenuItem.Size = new Size(114, 26);
            loadToolStripMenuItem.Text = "Load";
            loadToolStripMenuItem.Click += LoadToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(111, 6);
            // 
            // programMenuItem
            // 
            programMenuItem.DropDownItems.AddRange(new ToolStripItem[] { programSettingsMenuItem });
            programMenuItem.Font = new Font("Segoe UI", 12F);
            programMenuItem.Name = "programMenuItem";
            programMenuItem.Size = new Size(83, 25);
            programMenuItem.Text = "Program";
            // 
            // programSettingsMenuItem
            // 
            programSettingsMenuItem.Name = "programSettingsMenuItem";
            programSettingsMenuItem.Size = new Size(136, 26);
            programSettingsMenuItem.Text = "Settings";
            // 
            // dbDuplicationMenuItem
            // 
            dbDuplicationMenuItem.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dbDuplicationMenuItem.Name = "dbDuplicationMenuItem";
            dbDuplicationMenuItem.Size = new Size(125, 25);
            dbDuplicationMenuItem.Text = "DB Duplication";
            dbDuplicationMenuItem.Click += DbDuplicationMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(12, 25);
            // 
            // generateIOMenuItem
            // 
            generateIOMenuItem.Font = new Font("Segoe UI", 12F);
            generateIOMenuItem.Name = "generateIOMenuItem";
            generateIOMenuItem.Size = new Size(105, 25);
            generateIOMenuItem.Text = "Generate IO";
            generateIOMenuItem.Click += GenerateIOMenuItem_Click;
            // 
            // generateAlarmsMenuItem
            // 
            generateAlarmsMenuItem.Font = new Font("Segoe UI", 12F);
            generateAlarmsMenuItem.Name = "generateAlarmsMenuItem";
            generateAlarmsMenuItem.Size = new Size(138, 25);
            generateAlarmsMenuItem.Text = "Generate Alarms";
            generateAlarmsMenuItem.Click += GenerateAlarmsToolStripMenuItem_Click;
            // 
            // testToolStripMenuItem
            // 
            testToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { importXMLToolStripMenuItem, jSToolStripMenuItem, sampleXMLMenuItem, testProjectMenuItem, svgToolStripMenuItem, dbVisualizationMenuItem, settingsToolStripMenuItem });
            testToolStripMenuItem.Font = new Font("Segoe UI", 12F);
            testToolStripMenuItem.Name = "testToolStripMenuItem";
            testToolStripMenuItem.Size = new Size(48, 25);
            testToolStripMenuItem.Text = "Test";
            // 
            // importXMLToolStripMenuItem
            // 
            importXMLToolStripMenuItem.Name = "importXMLToolStripMenuItem";
            importXMLToolStripMenuItem.Size = new Size(188, 26);
            importXMLToolStripMenuItem.Text = "Import XML";
            importXMLToolStripMenuItem.Click += ImportXMLToolStripMenuItem_Click;
            // 
            // jSToolStripMenuItem
            // 
            jSToolStripMenuItem.Name = "jSToolStripMenuItem";
            jSToolStripMenuItem.Size = new Size(188, 26);
            jSToolStripMenuItem.Text = "JS";
            jSToolStripMenuItem.Click += JSToolStripMenuItem_Click;
            // 
            // sampleXMLMenuItem
            // 
            sampleXMLMenuItem.Name = "sampleXMLMenuItem";
            sampleXMLMenuItem.Size = new Size(188, 26);
            sampleXMLMenuItem.Text = "Sample XML";
            // 
            // testProjectMenuItem
            // 
            testProjectMenuItem.Name = "testProjectMenuItem";
            testProjectMenuItem.Size = new Size(188, 26);
            testProjectMenuItem.Text = "Project";
            // 
            // svgToolStripMenuItem
            // 
            svgToolStripMenuItem.Name = "svgToolStripMenuItem";
            svgToolStripMenuItem.Size = new Size(188, 26);
            svgToolStripMenuItem.Text = "Svg";
            svgToolStripMenuItem.Click += SvgToolStripMenuItem_Click;
            // 
            // dbVisualizationMenuItem
            // 
            dbVisualizationMenuItem.Name = "dbVisualizationMenuItem";
            dbVisualizationMenuItem.Size = new Size(188, 26);
            dbVisualizationMenuItem.Text = "DbVisualization";
            dbVisualizationMenuItem.Click += dbVisualizationMenuItem_Click;
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(188, 26);
            settingsToolStripMenuItem.Text = "Settings";
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
            // 
            // MainLayoutPanel
            // 
            MainLayoutPanel.AutoSize = true;
            MainLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            MainLayoutPanel.ColumnCount = 1;
            MainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            MainLayoutPanel.Controls.Add(TopMenuStrip, 0, 0);
            MainLayoutPanel.Dock = DockStyle.Fill;
            MainLayoutPanel.Location = new Point(0, 0);
            MainLayoutPanel.Margin = new Padding(4, 3, 4, 3);
            MainLayoutPanel.Name = "MainLayoutPanel";
            MainLayoutPanel.RowCount = 2;
            MainLayoutPanel.RowStyles.Add(new RowStyle());
            MainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            MainLayoutPanel.Size = new Size(906, 49);
            MainLayoutPanel.TabIndex = 14;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(906, 49);
            Controls.Add(MainLayoutPanel);
            MainMenuStrip = TopMenuStrip;
            Margin = new Padding(4, 3, 4, 3);
            Name = "MainForm";
            Text = "AppoggioMan";
            TopMenuStrip.ResumeLayout(false);
            TopMenuStrip.PerformLayout();
            MainLayoutPanel.ResumeLayout(false);
            MainLayoutPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.MenuStrip TopMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem dbDuplicationMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem generateIOMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateAlarmsMenuItem;
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
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem saveMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem programMenuItem;
        private ToolStripMenuItem programSettingsMenuItem;
    }
}

