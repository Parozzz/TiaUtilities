namespace TiaXmlReader
{
    partial class MainImportExportForm
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
            this.tiaVersionComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TopMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.AutoSaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoSaveComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.dbDuplicationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.generateIOMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateAlarmsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label4 = new System.Windows.Forms.Label();
            this.languageComboBox = new System.Windows.Forms.ComboBox();
            this.LogWorker = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.TopMenuStrip.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tiaVersionComboBox
            // 
            this.tiaVersionComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.tiaVersionComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tiaVersionComboBox.FormattingEnabled = true;
            this.tiaVersionComboBox.Items.AddRange(new object[] {
            "16",
            "17",
            "18",
            "19"});
            this.tiaVersionComboBox.Location = new System.Drawing.Point(202, 14);
            this.tiaVersionComboBox.Name = "tiaVersionComboBox";
            this.tiaVersionComboBox.Size = new System.Drawing.Size(49, 32);
            this.tiaVersionComboBox.TabIndex = 8;
            this.tiaVersionComboBox.Text = "17";
            this.tiaVersionComboBox.SelectedIndexChanged += new System.EventHandler(this.TiaVersionComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(62, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 25);
            this.label3.TabIndex = 9;
            this.label3.Text = "TIA Version";
            // 
            // TopMenuStrip
            // 
            this.TopMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.TopMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.dbDuplicationMenuItem,
            this.toolStripMenuItem1,
            this.generateIOMenuItem,
            this.generateAlarmsToolStripMenuItem,
            this.testToolStripMenuItem});
            this.TopMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.TopMenuStrip.Name = "TopMenuStrip";
            this.TopMenuStrip.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.TopMenuStrip.Size = new System.Drawing.Size(515, 29);
            this.TopMenuStrip.TabIndex = 11;
            this.TopMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator2,
            this.AutoSaveMenuItem});
            this.fileToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 25);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // AutoSaveMenuItem
            // 
            this.AutoSaveMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoSaveComboBox});
            this.AutoSaveMenuItem.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.AutoSaveMenuItem.Name = "AutoSaveMenuItem";
            this.AutoSaveMenuItem.Size = new System.Drawing.Size(180, 26);
            this.AutoSaveMenuItem.Text = "Auto Save";
            // 
            // autoSaveComboBox
            // 
            this.autoSaveComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.autoSaveComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.autoSaveComboBox.Name = "autoSaveComboBox";
            this.autoSaveComboBox.Size = new System.Drawing.Size(121, 23);
            // 
            // dbDuplicationMenuItem
            // 
            this.dbDuplicationMenuItem.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dbDuplicationMenuItem.Name = "dbDuplicationMenuItem";
            this.dbDuplicationMenuItem.Size = new System.Drawing.Size(125, 25);
            this.dbDuplicationMenuItem.Text = "DB Duplication";
            this.dbDuplicationMenuItem.Click += new System.EventHandler(this.DbDuplicationMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(12, 25);
            // 
            // generateIOMenuItem
            // 
            this.generateIOMenuItem.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.generateIOMenuItem.Name = "generateIOMenuItem";
            this.generateIOMenuItem.Size = new System.Drawing.Size(105, 25);
            this.generateIOMenuItem.Text = "Generate IO";
            this.generateIOMenuItem.Click += new System.EventHandler(this.GenerateIOMenuItem_Click);
            // 
            // generateAlarmsToolStripMenuItem
            // 
            this.generateAlarmsToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.generateAlarmsToolStripMenuItem.Name = "generateAlarmsToolStripMenuItem";
            this.generateAlarmsToolStripMenuItem.Size = new System.Drawing.Size(138, 25);
            this.generateAlarmsToolStripMenuItem.Text = "Generate Alarms";
            this.generateAlarmsToolStripMenuItem.Click += new System.EventHandler(this.GenerateAlarmsToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importXMLToolStripMenuItem,
            this.jSToolStripMenuItem});
            this.testToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(48, 25);
            this.testToolStripMenuItem.Text = "Test";
            // 
            // importXMLToolStripMenuItem
            // 
            this.importXMLToolStripMenuItem.Name = "importXMLToolStripMenuItem";
            this.importXMLToolStripMenuItem.Size = new System.Drawing.Size(180, 26);
            this.importXMLToolStripMenuItem.Text = "Import XML";
            this.importXMLToolStripMenuItem.Click += new System.EventHandler(this.ImportXMLToolStripMenuItem_Click);
            // 
            // jSToolStripMenuItem
            // 
            this.jSToolStripMenuItem.Name = "jSToolStripMenuItem";
            this.jSToolStripMenuItem.Size = new System.Drawing.Size(180, 26);
            this.jSToolStripMenuItem.Text = "JS";
            this.jSToolStripMenuItem.Click += new System.EventHandler(this.JSToolStripMenuItem_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(278, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 25);
            this.label4.TabIndex = 13;
            this.label4.Text = "Lingua";
            // 
            // languageComboBox
            // 
            this.languageComboBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.languageComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.languageComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.languageComboBox.FormattingEnabled = true;
            this.languageComboBox.Location = new System.Drawing.Point(367, 14);
            this.languageComboBox.Name = "languageComboBox";
            this.languageComboBox.Size = new System.Drawing.Size(80, 32);
            this.languageComboBox.TabIndex = 12;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.TopMenuStrip, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(515, 94);
            this.tableLayoutPanel1.TabIndex = 14;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.tiaVersionComboBox);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.languageComboBox);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 32);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(509, 59);
            this.panel1.TabIndex = 12;
            // 
            // MainImportExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 94);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MainMenuStrip = this.TopMenuStrip;
            this.Name = "MainImportExportForm";
            this.Text = "AppoggioMan";
            this.TopMenuStrip.ResumeLayout(false);
            this.TopMenuStrip.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox tiaVersionComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MenuStrip TopMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem dbDuplicationMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem generateIOMenuItem;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox languageComboBox;
        private System.Windows.Forms.ToolStripMenuItem generateAlarmsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem AutoSaveMenuItem;
        private System.Windows.Forms.ToolStripComboBox autoSaveComboBox;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jSToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker LogWorker;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
    }
}

