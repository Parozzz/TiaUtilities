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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IOName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Variable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainPanel.ColumnCount = 1;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainPanel.Controls.Add(this.dataGridView, 0, 1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.RowCount = 1;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.Size = new System.Drawing.Size(1093, 537);
            this.mainPanel.TabIndex = 1;
            // 
            // dataGridView
            // 
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Address,
            this.IOName,
            this.DB,
            this.Variable,
            this.Comment});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView.Size = new System.Drawing.Size(1087, 531);
            this.dataGridView.TabIndex = 2;
            // 
            // Address
            // 
            this.Address.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.Address.DefaultCellStyle = dataGridViewCellStyle1;
            this.Address.HeaderText = "Indirizzo";
            this.Address.MaxInputLength = 10;
            this.Address.Name = "Address";
            this.Address.Width = 60;
            // 
            // IOName
            // 
            this.IOName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this.IOName.DefaultCellStyle = dataGridViewCellStyle2;
            this.IOName.HeaderText = "Nome IO";
            this.IOName.Name = "IOName";
            this.IOName.Width = 74;
            // 
            // DB
            // 
            this.DB.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            this.DB.DefaultCellStyle = dataGridViewCellStyle3;
            this.DB.HeaderText = "DB";
            this.DB.Name = "DB";
            this.DB.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DB.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.DB.Width = 28;
            // 
            // Variable
            // 
            this.Variable.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            this.Variable.DefaultCellStyle = dataGridViewCellStyle4;
            this.Variable.HeaderText = "Variabile";
            this.Variable.Name = "Variable";
            this.Variable.Width = 72;
            // 
            // Comment
            // 
            this.Comment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            this.Comment.DefaultCellStyle = dataGridViewCellStyle5;
            this.Comment.HeaderText = "Commento";
            this.Comment.Name = "Comment";
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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Address;
        private System.Windows.Forms.DataGridViewTextBoxColumn IOName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DB;
        private System.Windows.Forms.DataGridViewTextBoxColumn Variable;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
    }
}