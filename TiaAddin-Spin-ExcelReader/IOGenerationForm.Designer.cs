namespace TiaXmlReader
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
            this.testButton = new System.Windows.Forms.Button();
            this.ioPanel = new System.Windows.Forms.TableLayoutPanel();
            this.flatComboBox4 = new TiaXmlReader.CustomControls.FlatComboBox();
            this.flatComboBox3 = new TiaXmlReader.CustomControls.FlatComboBox();
            this.flatComboBox2 = new TiaXmlReader.CustomControls.FlatComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.flatComboBox1 = new TiaXmlReader.CustomControls.FlatComboBox();
            this.flatTextBox1 = new TiaXmlReader.CustomControls.FlatTextBox();
            this.mainPanel.SuspendLayout();
            this.ioPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainPanel.ColumnCount = 1;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainPanel.Controls.Add(this.testButton, 0, 0);
            this.mainPanel.Controls.Add(this.ioPanel, 0, 1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.RowCount = 2;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.Size = new System.Drawing.Size(1093, 537);
            this.mainPanel.TabIndex = 1;
            // 
            // testButton
            // 
            this.testButton.Location = new System.Drawing.Point(3, 3);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(75, 23);
            this.testButton.TabIndex = 1;
            this.testButton.Text = "button1";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.TestButton_Click);
            // 
            // ioPanel
            // 
            this.ioPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.ioPanel.ColumnCount = 5;
            this.ioPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.ioPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.ioPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.ioPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.ioPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.ioPanel.Controls.Add(this.flatComboBox4, 0, 1);
            this.ioPanel.Controls.Add(this.flatComboBox3, 0, 1);
            this.ioPanel.Controls.Add(this.flatComboBox2, 0, 1);
            this.ioPanel.Controls.Add(this.label5, 4, 0);
            this.ioPanel.Controls.Add(this.label4, 3, 0);
            this.ioPanel.Controls.Add(this.label3, 2, 0);
            this.ioPanel.Controls.Add(this.label2, 1, 0);
            this.ioPanel.Controls.Add(this.label1, 0, 0);
            this.ioPanel.Controls.Add(this.flatComboBox1, 0, 1);
            this.ioPanel.Controls.Add(this.flatTextBox1, 4, 1);
            this.ioPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ioPanel.Location = new System.Drawing.Point(3, 32);
            this.ioPanel.Name = "ioPanel";
            this.ioPanel.RowCount = 2;
            this.ioPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.ioPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.ioPanel.Size = new System.Drawing.Size(1087, 502);
            this.ioPanel.TabIndex = 0;
            // 
            // flatComboBox4
            // 
            this.flatComboBox4.BackColor = System.Drawing.SystemColors.Control;
            this.flatComboBox4.BorderColor = System.Drawing.SystemColors.Control;
            this.flatComboBox4.ButtonColor = System.Drawing.SystemColors.ControlDark;
            this.flatComboBox4.Dock = System.Windows.Forms.DockStyle.Top;
            this.flatComboBox4.FormattingEnabled = true;
            this.flatComboBox4.Location = new System.Drawing.Point(544, 37);
            this.flatComboBox4.Margin = new System.Windows.Forms.Padding(0);
            this.flatComboBox4.Name = "flatComboBox4";
            this.flatComboBox4.Size = new System.Drawing.Size(216, 21);
            this.flatComboBox4.TabIndex = 17;
            // 
            // flatComboBox3
            // 
            this.flatComboBox3.BackColor = System.Drawing.SystemColors.Control;
            this.flatComboBox3.BorderColor = System.Drawing.SystemColors.Control;
            this.flatComboBox3.ButtonColor = System.Drawing.SystemColors.Control;
            this.flatComboBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.flatComboBox3.FormattingEnabled = true;
            this.flatComboBox3.Location = new System.Drawing.Point(110, 37);
            this.flatComboBox3.Margin = new System.Windows.Forms.Padding(0);
            this.flatComboBox3.Name = "flatComboBox3";
            this.flatComboBox3.Size = new System.Drawing.Size(216, 21);
            this.flatComboBox3.TabIndex = 16;
            // 
            // flatComboBox2
            // 
            this.flatComboBox2.BackColor = System.Drawing.SystemColors.Control;
            this.flatComboBox2.BorderColor = System.Drawing.SystemColors.Control;
            this.flatComboBox2.ButtonColor = System.Drawing.SystemColors.ControlDark;
            this.flatComboBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.flatComboBox2.FormattingEnabled = true;
            this.flatComboBox2.Location = new System.Drawing.Point(327, 37);
            this.flatComboBox2.Margin = new System.Windows.Forms.Padding(0);
            this.flatComboBox2.Name = "flatComboBox2";
            this.flatComboBox2.Size = new System.Drawing.Size(216, 21);
            this.flatComboBox2.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(764, 1);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(319, 35);
            this.label5.TabIndex = 4;
            this.label5.Text = "Commento";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(547, 1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(210, 35);
            this.label4.TabIndex = 3;
            this.label4.Text = "Variabile";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(330, 1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(210, 35);
            this.label3.TabIndex = 2;
            this.label3.Text = "DB";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(113, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(210, 35);
            this.label2.TabIndex = 1;
            this.label2.Text = "Nome IO";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "Indirizzo IO";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flatComboBox1
            // 
            this.flatComboBox1.BackColor = System.Drawing.SystemColors.Control;
            this.flatComboBox1.BorderColor = System.Drawing.SystemColors.Control;
            this.flatComboBox1.ButtonColor = System.Drawing.SystemColors.Control;
            this.flatComboBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flatComboBox1.FormattingEnabled = true;
            this.flatComboBox1.Location = new System.Drawing.Point(1, 37);
            this.flatComboBox1.Margin = new System.Windows.Forms.Padding(0);
            this.flatComboBox1.Name = "flatComboBox1";
            this.flatComboBox1.Size = new System.Drawing.Size(108, 21);
            this.flatComboBox1.TabIndex = 13;
            // 
            // flatTextBox1
            // 
            this.flatTextBox1.BackColor = System.Drawing.SystemColors.Control;
            this.flatTextBox1.BorderColor = System.Drawing.SystemColors.Control;
            this.flatTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flatTextBox1.Location = new System.Drawing.Point(761, 37);
            this.flatTextBox1.Margin = new System.Windows.Forms.Padding(0);
            this.flatTextBox1.Name = "flatTextBox1";
            this.flatTextBox1.Size = new System.Drawing.Size(325, 20);
            this.flatTextBox1.TabIndex = 14;
            this.flatTextBox1.Text = "adsad";
            // 
            // IOGenerationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1093, 537);
            this.Controls.Add(this.mainPanel);
            this.Name = "IOGenerationForm";
            this.Text = "IOGenerationForm";
            this.mainPanel.ResumeLayout(false);
            this.ioPanel.ResumeLayout(false);
            this.ioPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.Button testButton;
        private System.Windows.Forms.TableLayoutPanel ioPanel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private CustomControls.FlatComboBox flatComboBox1;
        private CustomControls.FlatTextBox flatTextBox1;
        private CustomControls.FlatComboBox flatComboBox4;
        private CustomControls.FlatComboBox flatComboBox3;
        private CustomControls.FlatComboBox flatComboBox2;
    }
}