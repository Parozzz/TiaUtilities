namespace SpinXmlReader
{
    partial class Form1
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
            this.generateAlarmsFCButton = new System.Windows.Forms.Button();
            this.xmlPathTextBlock = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tiaVersionComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.generateInOutButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.cadExcelPathTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // configExcelPathTextBox
            // 
            this.configExcelPathTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.configExcelPathTextBox.Location = new System.Drawing.Point(305, 21);
            this.configExcelPathTextBox.Name = "configExcelPathTextBox";
            this.configExcelPathTextBox.Size = new System.Drawing.Size(909, 21);
            this.configExcelPathTextBox.TabIndex = 0;
            this.configExcelPathTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ConfigExcelPathTextBox_MouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(17, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(282, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "File Excel Configurazione";
            // 
            // generateAlarmsFCButton
            // 
            this.generateAlarmsFCButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.generateAlarmsFCButton.Location = new System.Drawing.Point(242, 197);
            this.generateAlarmsFCButton.Name = "generateAlarmsFCButton";
            this.generateAlarmsFCButton.Size = new System.Drawing.Size(249, 43);
            this.generateAlarmsFCButton.TabIndex = 5;
            this.generateAlarmsFCButton.Text = "Genera FC Allarmi";
            this.generateAlarmsFCButton.UseVisualStyleBackColor = true;
            this.generateAlarmsFCButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GenerateAlarmsFCButton_MouseClick);
            // 
            // xmlPathTextBlock
            // 
            this.xmlPathTextBlock.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xmlPathTextBlock.Location = new System.Drawing.Point(305, 91);
            this.xmlPathTextBlock.Name = "xmlPathTextBlock";
            this.xmlPathTextBlock.Size = new System.Drawing.Size(909, 21);
            this.xmlPathTextBlock.TabIndex = 6;
            this.xmlPathTextBlock.MouseClick += new System.Windows.Forms.MouseEventHandler(this.XMLPathTextBlock_MouseClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(119, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(180, 25);
            this.label2.TabIndex = 7;
            this.label2.Text = "File XML Export";
            // 
            // tiaVersionComboBox
            // 
            this.tiaVersionComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.tiaVersionComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tiaVersionComboBox.FormattingEnabled = true;
            this.tiaVersionComboBox.Items.AddRange(new object[] {
            "17",
            "18"});
            this.tiaVersionComboBox.Location = new System.Drawing.Point(305, 127);
            this.tiaVersionComboBox.Name = "tiaVersionComboBox";
            this.tiaVersionComboBox.Size = new System.Drawing.Size(48, 32);
            this.tiaVersionComboBox.TabIndex = 8;
            this.tiaVersionComboBox.Text = "17";
            this.tiaVersionComboBox.TextUpdate += new System.EventHandler(this.tiaVersionComboBox_TextUpdate);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(152, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(147, 25);
            this.label3.TabIndex = 9;
            this.label3.Text = "Versione TIA";
            // 
            // generateInOutButton
            // 
            this.generateInOutButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.generateInOutButton.Location = new System.Drawing.Point(735, 197);
            this.generateInOutButton.Name = "generateInOutButton";
            this.generateInOutButton.Size = new System.Drawing.Size(249, 43);
            this.generateInOutButton.TabIndex = 10;
            this.generateInOutButton.Text = "Genera FC Appoggi";
            this.generateInOutButton.UseVisualStyleBackColor = true;
            this.generateInOutButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GenerateInOutButton_MouseClick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(129, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(170, 25);
            this.label4.TabIndex = 11;
            this.label4.Text = "File Excel CAD";
            // 
            // cadExcelPathTextBox
            // 
            this.cadExcelPathTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cadExcelPathTextBox.Location = new System.Drawing.Point(305, 56);
            this.cadExcelPathTextBox.Name = "cadExcelPathTextBox";
            this.cadExcelPathTextBox.Size = new System.Drawing.Size(909, 21);
            this.cadExcelPathTextBox.TabIndex = 12;
            this.cadExcelPathTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CadExcelPathTextBox_MouseClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1226, 275);
            this.Controls.Add(this.cadExcelPathTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.generateInOutButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tiaVersionComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.xmlPathTextBlock);
            this.Controls.Add(this.generateAlarmsFCButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.configExcelPathTextBox);
            this.Name = "Form1";
            this.Text = "TIA Block Generator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox configExcelPathTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button generateAlarmsFCButton;
        private System.Windows.Forms.TextBox xmlPathTextBlock;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox tiaVersionComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button generateInOutButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox cadExcelPathTextBox;
    }
}

