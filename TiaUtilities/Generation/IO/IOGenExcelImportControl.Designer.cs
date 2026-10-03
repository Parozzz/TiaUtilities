namespace TiaUtilities.Generation.IO
{
    partial class IOGenExcelImportControl
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

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            mainPanel = new TableLayoutPanel();
            bottomPanel = new TableLayoutPanel();
            importExcelButton = new Button();
            acceptButton = new Button();
            cancelButton = new Button();
            mainPanel.SuspendLayout();
            bottomPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.AutoSize = true;
            mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainPanel.ColumnCount = 1;
            mainPanel.ColumnStyles.Add(new ColumnStyle());
            mainPanel.Controls.Add(bottomPanel, 0, 1);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 0);
            mainPanel.Name = "mainPanel";
            mainPanel.RowCount = 2;
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.RowStyles.Add(new RowStyle());
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            mainPanel.Size = new Size(800, 385);
            mainPanel.TabIndex = 0;
            // 
            // bottomPanel
            // 
            bottomPanel.AutoSize = true;
            bottomPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            bottomPanel.ColumnCount = 3;
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            bottomPanel.Controls.Add(importExcelButton, 0, 0);
            bottomPanel.Controls.Add(acceptButton, 1, 0);
            bottomPanel.Controls.Add(cancelButton, 21, 0);
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Location = new Point(3, 335);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.RowCount = 1;
            bottomPanel.RowStyles.Add(new RowStyle());
            bottomPanel.Size = new Size(794, 47);
            bottomPanel.TabIndex = 1;
            // 
            // importExcelButton
            // 
            importExcelButton.AutoSize = true;
            importExcelButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            importExcelButton.Dock = DockStyle.Fill;
            importExcelButton.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            importExcelButton.Location = new Point(10, 3);
            importExcelButton.Margin = new Padding(10, 3, 10, 3);
            importExcelButton.Name = "importExcelButton";
            importExcelButton.Size = new Size(244, 41);
            importExcelButton.TabIndex = 0;
            importExcelButton.Text = "Importa Excel";
            importExcelButton.UseVisualStyleBackColor = true;
            // 
            // acceptButton
            // 
            acceptButton.AutoSize = true;
            acceptButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            acceptButton.DialogResult = DialogResult.Cancel;
            acceptButton.Dock = DockStyle.Fill;
            acceptButton.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold);
            acceptButton.Location = new Point(274, 3);
            acceptButton.Margin = new Padding(10, 3, 10, 3);
            acceptButton.Name = "acceptButton";
            acceptButton.Size = new Size(244, 41);
            acceptButton.TabIndex = 0;
            acceptButton.Text = "Accetta";
            acceptButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            cancelButton.AutoSize = true;
            cancelButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Dock = DockStyle.Fill;
            cancelButton.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cancelButton.Location = new Point(538, 3);
            cancelButton.Margin = new Padding(10, 3, 10, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(246, 41);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "Cancella";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // IOGenExcelImportControl
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(mainPanel);
            Name = "IOGenExcelImportControl";
            Size = new Size(800, 385);
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            bottomPanel.ResumeLayout(false);
            bottomPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.Button importExcelButton;
        private System.Windows.Forms.TableLayoutPanel bottomPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button acceptButton;
    }
}
