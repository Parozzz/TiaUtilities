namespace TiaAddin_Spin_ExcelReader
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
            this.FilePathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ParseFileButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // FilePathTextBox
            // 
            this.FilePathTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FilePathTextBox.Location = new System.Drawing.Point(143, 17);
            this.FilePathTextBox.Name = "FilePathTextBox";
            this.FilePathTextBox.Size = new System.Drawing.Size(645, 29);
            this.FilePathTextBox.TabIndex = 0;
            this.FilePathTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FilePathTextBox_MouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "File Excel";
            // 
            // ParseFileButton
            // 
            this.ParseFileButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ParseFileButton.Location = new System.Drawing.Point(257, 71);
            this.ParseFileButton.Name = "ParseFileButton";
            this.ParseFileButton.Size = new System.Drawing.Size(311, 39);
            this.ParseFileButton.TabIndex = 2;
            this.ParseFileButton.Text = "Parse File";
            this.ParseFileButton.UseVisualStyleBackColor = true;
            this.ParseFileButton.Click += new System.EventHandler(this.ParseFileButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 120);
            this.Controls.Add(this.ParseFileButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FilePathTextBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox FilePathTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ParseFileButton;
    }
}

