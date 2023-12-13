namespace SpinXmlReader
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
            this.configExcelPathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.exportPathTextBlock = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tiaVersionComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.generateInOutButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.dbDuplicationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // configExcelPathTextBox
            // 
            this.configExcelPathTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.configExcelPathTextBox.Location = new System.Drawing.Point(221, 46);
            this.configExcelPathTextBox.Name = "configExcelPathTextBox";
            this.configExcelPathTextBox.Size = new System.Drawing.Size(909, 21);
            this.configExcelPathTextBox.TabIndex = 0;
            this.configExcelPathTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ConfigExcelPathTextBox_MouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(17, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "File Excel Config.";
            // 
            // exportPathTextBlock
            // 
            this.exportPathTextBlock.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportPathTextBlock.Location = new System.Drawing.Point(221, 79);
            this.exportPathTextBlock.Name = "exportPathTextBlock";
            this.exportPathTextBlock.Size = new System.Drawing.Size(909, 21);
            this.exportPathTextBlock.TabIndex = 6;
            this.exportPathTextBlock.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ExportPathTextBlock_MouseClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(61, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(154, 25);
            this.label2.TabIndex = 7;
            this.label2.Text = "Export Folder";
            // 
            // tiaVersionComboBox
            // 
            this.tiaVersionComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.tiaVersionComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tiaVersionComboBox.FormattingEnabled = true;
            this.tiaVersionComboBox.Items.AddRange(new object[] {
            "16",
            "17",
            "18"});
            this.tiaVersionComboBox.Location = new System.Drawing.Point(221, 108);
            this.tiaVersionComboBox.Name = "tiaVersionComboBox";
            this.tiaVersionComboBox.Size = new System.Drawing.Size(48, 32);
            this.tiaVersionComboBox.TabIndex = 8;
            this.tiaVersionComboBox.Text = "17";
            this.tiaVersionComboBox.SelectedIndexChanged += new System.EventHandler(this.TiaVersionComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(81, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 25);
            this.label3.TabIndex = 9;
            this.label3.Text = "TIA Version";
            // 
            // generateInOutButton
            // 
            this.generateInOutButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.generateInOutButton.Location = new System.Drawing.Point(395, 144);
            this.generateInOutButton.Name = "generateInOutButton";
            this.generateInOutButton.Size = new System.Drawing.Size(347, 43);
            this.generateInOutButton.TabIndex = 10;
            this.generateInOutButton.Text = "Generate XML Export Files";
            this.generateInOutButton.UseVisualStyleBackColor = true;
            this.generateInOutButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GenerateXMLExportFiles_MouseClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dbDuplicationMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1136, 29);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // dbDuplicationMenuItem
            // 
            this.dbDuplicationMenuItem.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dbDuplicationMenuItem.Name = "dbDuplicationMenuItem";
            this.dbDuplicationMenuItem.Size = new System.Drawing.Size(125, 25);
            this.dbDuplicationMenuItem.Text = "DB Duplication";
            this.dbDuplicationMenuItem.Click += new System.EventHandler(this.DbDuplicationMenuItem_Click);
            // 
            // MainImportExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1136, 207);
            this.Controls.Add(this.generateInOutButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tiaVersionComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.exportPathTextBlock);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.configExcelPathTextBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainImportExportForm";
            this.Text = "AppoggioMan";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox configExcelPathTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox exportPathTextBlock;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox tiaVersionComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button generateInOutButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem dbDuplicationMenuItem;
    }
}

