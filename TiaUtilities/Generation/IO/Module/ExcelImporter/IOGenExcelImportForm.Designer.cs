namespace TiaUtilities.Generation.IO.Module.ExcelImporter
{
    partial class IOGenerationExcelImportForm
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
            MainTableLayoutPanel = new TableLayoutPanel();
            TopPanel = new TableLayoutPanel();
            importExcelButton = new Button();
            configButton = new Button();
            BottomPanel = new TableLayoutPanel();
            cancelButton = new Button();
            acceptButton = new Button();
            MainTableLayoutPanel.SuspendLayout();
            TopPanel.SuspendLayout();
            BottomPanel.SuspendLayout();
            SuspendLayout();
            // 
            // MainTableLayoutPanel
            // 
            MainTableLayoutPanel.AutoSize = true;
            MainTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            MainTableLayoutPanel.ColumnCount = 1;
            MainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            MainTableLayoutPanel.Controls.Add(TopPanel, 0, 0);
            MainTableLayoutPanel.Controls.Add(BottomPanel, 0, 2);
            MainTableLayoutPanel.Dock = DockStyle.Fill;
            MainTableLayoutPanel.Location = new Point(0, 0);
            MainTableLayoutPanel.Name = "MainTableLayoutPanel";
            MainTableLayoutPanel.RowCount = 3;
            MainTableLayoutPanel.RowStyles.Add(new RowStyle());
            MainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            MainTableLayoutPanel.RowStyles.Add(new RowStyle());
            MainTableLayoutPanel.Size = new Size(800, 385);
            MainTableLayoutPanel.TabIndex = 0;
            // 
            // TopPanel
            // 
            TopPanel.AutoSize = true;
            TopPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            TopPanel.ColumnCount = 2;
            TopPanel.ColumnStyles.Add(new ColumnStyle());
            TopPanel.ColumnStyles.Add(new ColumnStyle());
            TopPanel.Controls.Add(importExcelButton, 0, 0);
            TopPanel.Controls.Add(configButton, 0, 0);
            TopPanel.Dock = DockStyle.Top;
            TopPanel.Location = new Point(3, 3);
            TopPanel.Name = "TopPanel";
            TopPanel.RowCount = 1;
            TopPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            TopPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 41F));
            TopPanel.Size = new Size(794, 41);
            TopPanel.TabIndex = 1;
            // 
            // importExcelButton
            // 
            importExcelButton.AutoSize = true;
            importExcelButton.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            importExcelButton.Location = new Point(190, 3);
            importExcelButton.Name = "importExcelButton";
            importExcelButton.Size = new Size(181, 35);
            importExcelButton.TabIndex = 0;
            importExcelButton.Text = "Importa Excel";
            importExcelButton.UseVisualStyleBackColor = true;
            // 
            // configButton
            // 
            configButton.AutoSize = true;
            configButton.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            configButton.Location = new Point(3, 3);
            configButton.Name = "configButton";
            configButton.Size = new Size(181, 35);
            configButton.TabIndex = 1;
            configButton.Text = "Configurazione";
            configButton.UseVisualStyleBackColor = true;
            // 
            // BottomPanel
            // 
            BottomPanel.AutoSize = true;
            BottomPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BottomPanel.ColumnCount = 3;
            BottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            BottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            BottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            BottomPanel.Controls.Add(cancelButton, 2, 0);
            BottomPanel.Controls.Add(acceptButton, 0, 0);
            BottomPanel.Dock = DockStyle.Bottom;
            BottomPanel.Location = new Point(3, 341);
            BottomPanel.Name = "BottomPanel";
            BottomPanel.RowCount = 1;
            BottomPanel.RowStyles.Add(new RowStyle());
            BottomPanel.Size = new Size(794, 41);
            BottomPanel.TabIndex = 1;
            // 
            // cancelButton
            // 
            cancelButton.AutoSize = true;
            cancelButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Dock = DockStyle.Fill;
            cancelButton.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cancelButton.Location = new Point(478, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(313, 35);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "Cancella";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // acceptButton
            // 
            acceptButton.AutoSize = true;
            acceptButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            acceptButton.DialogResult = DialogResult.Cancel;
            acceptButton.Dock = DockStyle.Fill;
            acceptButton.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold);
            acceptButton.Location = new Point(3, 3);
            acceptButton.Name = "acceptButton";
            acceptButton.Size = new Size(311, 35);
            acceptButton.TabIndex = 0;
            acceptButton.Text = "Accetta";
            acceptButton.UseVisualStyleBackColor = true;
            // 
            // IOGenerationExcelImportForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(800, 385);
            ControlBox = false;
            Controls.Add(MainTableLayoutPanel);
            Name = "IOGenerationExcelImportForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "IO Generation Excel Importer";
            MainTableLayoutPanel.ResumeLayout(false);
            MainTableLayoutPanel.PerformLayout();
            TopPanel.ResumeLayout(false);
            TopPanel.PerformLayout();
            BottomPanel.ResumeLayout(false);
            BottomPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel MainTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel TopPanel;
        private System.Windows.Forms.Button importExcelButton;
        private System.Windows.Forms.TableLayoutPanel BottomPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button acceptButton;
        private System.Windows.Forms.Button configButton;
    }
}
