namespace TiaXmlReader.GenerationForms.IO
{
    partial class IOGenerationForm
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
            this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.configPanel = new System.Windows.Forms.Panel();
            this.variableTableConfigButton = new System.Windows.Forms.Button();
            this.dbConfigButton = new System.Windows.Forms.Button();
            this.fcConfigButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.divisionTypeLabel = new System.Windows.Forms.Label();
            this.memoryTypeLabel = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.ioTableConfigButton = new System.Windows.Forms.Button();
            this.segmentNameConfigButton = new System.Windows.Forms.Button();
            this.divisionTypeComboBox = new TiaXmlReader.CustomControls.FlatComboBox();
            this.memoryTypeComboBox = new TiaXmlReader.CustomControls.FlatComboBox();
            this.mainPanel.SuspendLayout();
            this.configPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainPanel.ColumnCount = 1;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainPanel.Controls.Add(this.configPanel, 0, 0);
            this.mainPanel.Controls.Add(this.dataGridView, 0, 1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.RowCount = 2;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.Size = new System.Drawing.Size(1093, 537);
            this.mainPanel.TabIndex = 1;
            // 
            // configPanel
            // 
            this.configPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.configPanel.Controls.Add(this.segmentNameConfigButton);
            this.configPanel.Controls.Add(this.ioTableConfigButton);
            this.configPanel.Controls.Add(this.variableTableConfigButton);
            this.configPanel.Controls.Add(this.dbConfigButton);
            this.configPanel.Controls.Add(this.fcConfigButton);
            this.configPanel.Controls.Add(this.label6);
            this.configPanel.Controls.Add(this.divisionTypeComboBox);
            this.configPanel.Controls.Add(this.divisionTypeLabel);
            this.configPanel.Controls.Add(this.memoryTypeComboBox);
            this.configPanel.Controls.Add(this.memoryTypeLabel);
            this.configPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configPanel.Location = new System.Drawing.Point(3, 3);
            this.configPanel.Name = "configPanel";
            this.configPanel.Size = new System.Drawing.Size(1087, 80);
            this.configPanel.TabIndex = 3;
            // 
            // variableTableConfigButton
            // 
            this.variableTableConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold);
            this.variableTableConfigButton.Location = new System.Drawing.Point(217, 19);
            this.variableTableConfigButton.Margin = new System.Windows.Forms.Padding(0);
            this.variableTableConfigButton.Name = "variableTableConfigButton";
            this.variableTableConfigButton.Size = new System.Drawing.Size(183, 30);
            this.variableTableConfigButton.TabIndex = 12;
            this.variableTableConfigButton.Text = "Tabella Appoggi";
            this.variableTableConfigButton.UseVisualStyleBackColor = true;
            // 
            // dbConfigButton
            // 
            this.dbConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold);
            this.dbConfigButton.Location = new System.Drawing.Point(404, 19);
            this.dbConfigButton.Margin = new System.Windows.Forms.Padding(0);
            this.dbConfigButton.Name = "dbConfigButton";
            this.dbConfigButton.Size = new System.Drawing.Size(147, 30);
            this.dbConfigButton.TabIndex = 11;
            this.dbConfigButton.Text = "DB Appoggi";
            this.dbConfigButton.UseVisualStyleBackColor = true;
            // 
            // fcConfigButton
            // 
            this.fcConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold);
            this.fcConfigButton.Location = new System.Drawing.Point(4, 19);
            this.fcConfigButton.Name = "fcConfigButton";
            this.fcConfigButton.Size = new System.Drawing.Size(71, 30);
            this.fcConfigButton.TabIndex = 10;
            this.fcConfigButton.Text = "FC";
            this.fcConfigButton.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(1087, 16);
            this.label6.TabIndex = 9;
            this.label6.Text = "Placeholders: {memory_type} {bit} {byte} {io_name} {db_name} {variable_name} {com" +
    "ment} ";
            // 
            // divisionTypeLabel
            // 
            this.divisionTypeLabel.AutoSize = true;
            this.divisionTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.divisionTypeLabel.Location = new System.Drawing.Point(184, 54);
            this.divisionTypeLabel.Name = "divisionTypeLabel";
            this.divisionTypeLabel.Size = new System.Drawing.Size(98, 18);
            this.divisionTypeLabel.TabIndex = 2;
            this.divisionTypeLabel.Text = "Tipo divisione";
            // 
            // memoryTypeLabel
            // 
            this.memoryTypeLabel.AutoSize = true;
            this.memoryTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memoryTypeLabel.Location = new System.Drawing.Point(9, 54);
            this.memoryTypeLabel.Name = "memoryTypeLabel";
            this.memoryTypeLabel.Size = new System.Drawing.Size(100, 18);
            this.memoryTypeLabel.TabIndex = 0;
            this.memoryTypeLabel.Text = "Tipo memoria";
            // 
            // dataGridView
            // 
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 89);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView.Size = new System.Drawing.Size(1087, 531);
            this.dataGridView.TabIndex = 2;
            // 
            // ioTableConfigButton
            // 
            this.ioTableConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold);
            this.ioTableConfigButton.Location = new System.Drawing.Point(79, 19);
            this.ioTableConfigButton.Name = "ioTableConfigButton";
            this.ioTableConfigButton.Size = new System.Drawing.Size(134, 30);
            this.ioTableConfigButton.TabIndex = 13;
            this.ioTableConfigButton.Text = "Tabella IO";
            this.ioTableConfigButton.UseVisualStyleBackColor = true;
            // 
            // segmentNameConfigButton
            // 
            this.segmentNameConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold);
            this.segmentNameConfigButton.Location = new System.Drawing.Point(555, 19);
            this.segmentNameConfigButton.Margin = new System.Windows.Forms.Padding(0);
            this.segmentNameConfigButton.Name = "segmentNameConfigButton";
            this.segmentNameConfigButton.Size = new System.Drawing.Size(152, 30);
            this.segmentNameConfigButton.TabIndex = 14;
            this.segmentNameConfigButton.Text = "Nomi Segmenti";
            this.segmentNameConfigButton.UseVisualStyleBackColor = true;
            // 
            // divisionTypeComboBox
            // 
            this.divisionTypeComboBox.FormattingEnabled = true;
            this.divisionTypeComboBox.Items.AddRange(new object[] {
            "BitPerSegmento",
            "BytePerSegmento"});
            this.divisionTypeComboBox.Location = new System.Drawing.Point(284, 53);
            this.divisionTypeComboBox.Name = "divisionTypeComboBox";
            this.divisionTypeComboBox.Size = new System.Drawing.Size(112, 21);
            this.divisionTypeComboBox.TabIndex = 3;
            this.divisionTypeComboBox.Text = "BytePerSegmento";
            // 
            // memoryTypeComboBox
            // 
            this.memoryTypeComboBox.FormattingEnabled = true;
            this.memoryTypeComboBox.Items.AddRange(new object[] {
            "DB",
            "Merker"});
            this.memoryTypeComboBox.Location = new System.Drawing.Point(111, 53);
            this.memoryTypeComboBox.Name = "memoryTypeComboBox";
            this.memoryTypeComboBox.Size = new System.Drawing.Size(65, 21);
            this.memoryTypeComboBox.TabIndex = 1;
            this.memoryTypeComboBox.Text = "DB";
            // 
            // IOGenerationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1093, 537);
            this.Controls.Add(this.mainPanel);
            this.DoubleBuffered = true;
            this.Name = "IOGenerationForm";
            this.Text = "IOGenerationForm";
            this.mainPanel.ResumeLayout(false);
            this.configPanel.ResumeLayout(false);
            this.configPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.Panel configPanel;
        public CustomControls.FlatComboBox divisionTypeComboBox;
        private System.Windows.Forms.Label divisionTypeLabel;
        public CustomControls.FlatComboBox memoryTypeComboBox;
        private System.Windows.Forms.Label memoryTypeLabel;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.Button fcConfigButton;
        public System.Windows.Forms.Button dbConfigButton;
        public System.Windows.Forms.Button variableTableConfigButton;
        public System.Windows.Forms.Button ioTableConfigButton;
        public System.Windows.Forms.Button segmentNameConfigButton;
    }
}