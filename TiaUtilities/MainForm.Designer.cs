namespace TiaXmlReader
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
            tiaVersionComboBox = new ComboBox();
            tiaVersionLabel = new Label();
            TopMenuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            loadToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            autoSaveMenuItem = new ToolStripMenuItem();
            autoSaveTimeTextBox = new ToolStripTextBox();
            toolStripSeparator1 = new ToolStripSeparator();
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
            languageLabel = new Label();
            languageComboBox = new ComboBox();
            LogWorker = new System.ComponentModel.BackgroundWorker();
            MainLayoutPanel = new TableLayoutPanel();
            SettingsLayoutPanel = new TableLayoutPanel();
            TopMenuStrip.SuspendLayout();
            MainLayoutPanel.SuspendLayout();
            SettingsLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // tiaVersionComboBox
            // 
            tiaVersionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            tiaVersionComboBox.FlatStyle = FlatStyle.Flat;
            tiaVersionComboBox.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tiaVersionComboBox.FormattingEnabled = true;
            tiaVersionComboBox.Items.AddRange(new object[] { "16", "17", "18", "19" });
            tiaVersionComboBox.Location = new Point(146, 39);
            tiaVersionComboBox.Margin = new Padding(4, 3, 4, 3);
            tiaVersionComboBox.Name = "tiaVersionComboBox";
            tiaVersionComboBox.Size = new Size(56, 32);
            tiaVersionComboBox.TabIndex = 8;
            // 
            // tiaVersionLabel
            // 
            tiaVersionLabel.AutoSize = true;
            tiaVersionLabel.Dock = DockStyle.Fill;
            tiaVersionLabel.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tiaVersionLabel.Location = new Point(4, 36);
            tiaVersionLabel.Margin = new Padding(4, 0, 4, 0);
            tiaVersionLabel.Name = "tiaVersionLabel";
            tiaVersionLabel.Size = new Size(134, 37);
            tiaVersionLabel.TabIndex = 9;
            tiaVersionLabel.Text = "TIA Version";
            tiaVersionLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // TopMenuStrip
            // 
            TopMenuStrip.ImageScalingSize = new Size(20, 20);
            TopMenuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, dbDuplicationMenuItem, toolStripMenuItem1, generateIOMenuItem, generateAlarmsMenuItem, testToolStripMenuItem });
            TopMenuStrip.Location = new Point(0, 0);
            TopMenuStrip.Name = "TopMenuStrip";
            TopMenuStrip.Padding = new Padding(5, 2, 0, 2);
            TopMenuStrip.Size = new Size(601, 29);
            TopMenuStrip.TabIndex = 11;
            TopMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadToolStripMenuItem, toolStripSeparator2, autoSaveMenuItem, autoSaveTimeTextBox, toolStripSeparator1 });
            fileToolStripMenuItem.Font = new Font("Segoe UI", 12F);
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 25);
            fileToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            loadToolStripMenuItem.Size = new Size(160, 26);
            loadToolStripMenuItem.Text = "Load";
            loadToolStripMenuItem.Click += LoadToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(157, 6);
            // 
            // autoSaveMenuItem
            // 
            autoSaveMenuItem.Enabled = false;
            autoSaveMenuItem.Font = new Font("Segoe UI", 12F);
            autoSaveMenuItem.Name = "autoSaveMenuItem";
            autoSaveMenuItem.Size = new Size(160, 26);
            autoSaveMenuItem.Text = "Auto Save";
            // 
            // autoSaveTimeTextBox
            // 
            autoSaveTimeTextBox.Name = "autoSaveTimeTextBox";
            autoSaveTimeTextBox.Size = new Size(100, 23);
            autoSaveTimeTextBox.Text = "120";
            autoSaveTimeTextBox.TextBoxTextAlign = HorizontalAlignment.Center;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(157, 6);
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
            testToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { importXMLToolStripMenuItem, jSToolStripMenuItem, sampleXMLMenuItem, testProjectMenuItem, svgToolStripMenuItem, dbVisualizationMenuItem });
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
            // languageLabel
            // 
            languageLabel.AutoSize = true;
            languageLabel.Dock = DockStyle.Fill;
            languageLabel.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            languageLabel.Location = new Point(4, 0);
            languageLabel.Margin = new Padding(4, 0, 4, 0);
            languageLabel.Name = "languageLabel";
            languageLabel.Size = new Size(134, 36);
            languageLabel.TabIndex = 13;
            languageLabel.Text = "Lingua";
            languageLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // languageComboBox
            // 
            languageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            languageComboBox.FlatStyle = FlatStyle.Flat;
            languageComboBox.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            languageComboBox.FormattingEnabled = true;
            languageComboBox.Location = new Point(146, 3);
            languageComboBox.Margin = new Padding(4, 3, 4, 3);
            languageComboBox.Name = "languageComboBox";
            languageComboBox.Size = new Size(93, 32);
            languageComboBox.TabIndex = 12;
            // 
            // MainLayoutPanel
            // 
            MainLayoutPanel.AutoSize = true;
            MainLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            MainLayoutPanel.ColumnCount = 1;
            MainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            MainLayoutPanel.Controls.Add(TopMenuStrip, 0, 0);
            MainLayoutPanel.Controls.Add(SettingsLayoutPanel, 0, 1);
            MainLayoutPanel.Dock = DockStyle.Fill;
            MainLayoutPanel.Location = new Point(0, 0);
            MainLayoutPanel.Margin = new Padding(4, 3, 4, 3);
            MainLayoutPanel.Name = "MainLayoutPanel";
            MainLayoutPanel.RowCount = 2;
            MainLayoutPanel.RowStyles.Add(new RowStyle());
            MainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            MainLayoutPanel.Size = new Size(601, 108);
            MainLayoutPanel.TabIndex = 14;
            // 
            // SettingsLayoutPanel
            // 
            SettingsLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            SettingsLayoutPanel.AutoSize = true;
            SettingsLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            SettingsLayoutPanel.ColumnCount = 2;
            SettingsLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            SettingsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            SettingsLayoutPanel.Controls.Add(languageLabel, 0, 0);
            SettingsLayoutPanel.Controls.Add(languageComboBox, 1, 0);
            SettingsLayoutPanel.Controls.Add(tiaVersionLabel, 0, 1);
            SettingsLayoutPanel.Controls.Add(tiaVersionComboBox, 1, 1);
            SettingsLayoutPanel.Location = new Point(179, 32);
            SettingsLayoutPanel.Name = "SettingsLayoutPanel";
            SettingsLayoutPanel.RowCount = 2;
            SettingsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            SettingsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            SettingsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            SettingsLayoutPanel.Size = new Size(243, 73);
            SettingsLayoutPanel.TabIndex = 14;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(601, 108);
            Controls.Add(MainLayoutPanel);
            MainMenuStrip = TopMenuStrip;
            Margin = new Padding(4, 3, 4, 3);
            Name = "MainForm";
            Text = "AppoggioMan";
            TopMenuStrip.ResumeLayout(false);
            TopMenuStrip.PerformLayout();
            MainLayoutPanel.ResumeLayout(false);
            MainLayoutPanel.PerformLayout();
            SettingsLayoutPanel.ResumeLayout(false);
            SettingsLayoutPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.ComboBox tiaVersionComboBox;
        private System.Windows.Forms.Label tiaVersionLabel;
        private System.Windows.Forms.MenuStrip TopMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem dbDuplicationMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem generateIOMenuItem;
        private System.Windows.Forms.Label languageLabel;
        private System.Windows.Forms.ComboBox languageComboBox;
        private System.Windows.Forms.ToolStripMenuItem generateAlarmsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem autoSaveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jSToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker LogWorker;
        private System.Windows.Forms.TableLayoutPanel MainLayoutPanel;
        private TableLayoutPanel SettingsLayoutPanel;
        private ToolStripTextBox autoSaveTimeTextBox;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem sampleXMLMenuItem;
        private ToolStripMenuItem testProjectMenuItem;
        private ToolStripMenuItem loadToolStripMenuItem;
        private ToolStripMenuItem svgToolStripMenuItem;
        private ToolStripMenuItem dbVisualizationMenuItem;
    }
}

