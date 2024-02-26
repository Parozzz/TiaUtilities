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
            this.defaultFieldValuePanel = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.defaultFieldValuePanel_1 = new System.Windows.Forms.TableLayoutPanel();
            this.defaultIoNameLabel = new System.Windows.Forms.Label();
            this.defaultIoNameTextBox = new TiaXmlReader.CustomControls.FlatTextBox();
            this.defaultVariableLabel = new System.Windows.Forms.Label();
            this.defaultVariableTextBox = new TiaXmlReader.CustomControls.FlatTextBox();
            this.ioTagTablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.ioTagTablePanel_1 = new System.Windows.Forms.TableLayoutPanel();
            this.ioTagTableNameLabel = new System.Windows.Forms.Label();
            this.ioTagTableNameTextBox = new TiaXmlReader.CustomControls.FlatTextBox();
            this.ioTagTableSplitEveryLabel = new System.Windows.Forms.Label();
            this.ioTagTableSplitEveryTextBox = new TiaXmlReader.CustomControls.FlatTextBox();
            this.divisionTypeComboBox = new TiaXmlReader.CustomControls.FlatComboBox();
            this.divisionTypeLabel = new System.Windows.Forms.Label();
            this.memoryTypeComboBox = new TiaXmlReader.CustomControls.FlatComboBox();
            this.memoryTypeLabel = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.segmentNamesPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.segmentNamesPanel_1 = new System.Windows.Forms.TableLayoutPanel();
            this.segmentNamePerBitLabel = new System.Windows.Forms.Label();
            this.segmentNamePerBitTextBox = new TiaXmlReader.CustomControls.FlatTextBox();
            this.segmentNamePerByteLabel = new System.Windows.Forms.Label();
            this.segmentNamePerByteTextBox = new TiaXmlReader.CustomControls.FlatTextBox();
            this.fcPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.fcNameLabel = new System.Windows.Forms.Label();
            this.fcNameTextBox = new TiaXmlReader.CustomControls.FlatTextBox();
            this.fcNumberLabel = new System.Windows.Forms.Label();
            this.fcNumberTextBox = new TiaXmlReader.CustomControls.FlatTextBox();
            this.prefixesLabel = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.prefixInputDBLabel = new System.Windows.Forms.Label();
            this.prefixInputDBTextBox = new TiaXmlReader.CustomControls.FlatTextBox();
            this.prefixOutputDBLabel = new System.Windows.Forms.Label();
            this.prefixOutputDBTextBox = new TiaXmlReader.CustomControls.FlatTextBox();
            this.prefixInputMerkerLabel = new System.Windows.Forms.Label();
            this.prefixInputMerkerTextBox = new TiaXmlReader.CustomControls.FlatTextBox();
            this.prefixOutputMerkerLabel = new System.Windows.Forms.Label();
            this.prefixOutputMerkerTextBox = new TiaXmlReader.CustomControls.FlatTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.mainPanel.SuspendLayout();
            this.configPanel.SuspendLayout();
            this.defaultFieldValuePanel.SuspendLayout();
            this.defaultFieldValuePanel_1.SuspendLayout();
            this.ioTagTablePanel.SuspendLayout();
            this.ioTagTablePanel_1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.segmentNamesPanel.SuspendLayout();
            this.segmentNamesPanel_1.SuspendLayout();
            this.fcPanel.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.prefixesLabel.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
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
            this.configPanel.Controls.Add(this.label6);
            this.configPanel.Controls.Add(this.fcPanel);
            this.configPanel.Controls.Add(this.ioTagTablePanel);
            this.configPanel.Controls.Add(this.defaultFieldValuePanel);
            this.configPanel.Controls.Add(this.segmentNamesPanel);
            this.configPanel.Controls.Add(this.prefixesLabel);
            this.configPanel.Controls.Add(this.divisionTypeComboBox);
            this.configPanel.Controls.Add(this.divisionTypeLabel);
            this.configPanel.Controls.Add(this.memoryTypeComboBox);
            this.configPanel.Controls.Add(this.memoryTypeLabel);
            this.configPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configPanel.Location = new System.Drawing.Point(3, 3);
            this.configPanel.Name = "configPanel";
            this.configPanel.Size = new System.Drawing.Size(1087, 190);
            this.configPanel.TabIndex = 3;
            // 
            // defaultFieldValuePanel
            // 
            this.defaultFieldValuePanel.AutoSize = true;
            this.defaultFieldValuePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.defaultFieldValuePanel.BackColor = System.Drawing.Color.Transparent;
            this.defaultFieldValuePanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.defaultFieldValuePanel.ColumnCount = 1;
            this.defaultFieldValuePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.defaultFieldValuePanel.Controls.Add(this.label2, 0, 0);
            this.defaultFieldValuePanel.Controls.Add(this.defaultFieldValuePanel_1, 0, 1);
            this.defaultFieldValuePanel.Location = new System.Drawing.Point(4, 101);
            this.defaultFieldValuePanel.Name = "defaultFieldValuePanel";
            this.defaultFieldValuePanel.RowCount = 2;
            this.defaultFieldValuePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.defaultFieldValuePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.defaultFieldValuePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.defaultFieldValuePanel.Size = new System.Drawing.Size(348, 86);
            this.defaultFieldValuePanel.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(4, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(340, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Valori default campi";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // defaultFieldValuePanel_1
            // 
            this.defaultFieldValuePanel_1.AutoSize = true;
            this.defaultFieldValuePanel_1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.defaultFieldValuePanel_1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.defaultFieldValuePanel_1.ColumnCount = 2;
            this.defaultFieldValuePanel_1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.defaultFieldValuePanel_1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.defaultFieldValuePanel_1.Controls.Add(this.defaultIoNameLabel, 0, 0);
            this.defaultFieldValuePanel_1.Controls.Add(this.defaultIoNameTextBox, 1, 0);
            this.defaultFieldValuePanel_1.Controls.Add(this.defaultVariableLabel, 0, 1);
            this.defaultFieldValuePanel_1.Controls.Add(this.defaultVariableTextBox, 1, 1);
            this.defaultFieldValuePanel_1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.defaultFieldValuePanel_1.Location = new System.Drawing.Point(4, 25);
            this.defaultFieldValuePanel_1.Name = "defaultFieldValuePanel_1";
            this.defaultFieldValuePanel_1.RowCount = 2;
            this.defaultFieldValuePanel_1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.defaultFieldValuePanel_1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.defaultFieldValuePanel_1.Size = new System.Drawing.Size(340, 57);
            this.defaultFieldValuePanel_1.TabIndex = 2;
            // 
            // defaultIoNameLabel
            // 
            this.defaultIoNameLabel.AutoSize = true;
            this.defaultIoNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.defaultIoNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.defaultIoNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.defaultIoNameLabel.Location = new System.Drawing.Point(4, 1);
            this.defaultIoNameLabel.Name = "defaultIoNameLabel";
            this.defaultIoNameLabel.Size = new System.Drawing.Size(72, 27);
            this.defaultIoNameLabel.TabIndex = 0;
            this.defaultIoNameLabel.Text = "Nome IO";
            this.defaultIoNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // defaultIoNameTextBox
            // 
            this.defaultIoNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.defaultIoNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.defaultIoNameTextBox.Location = new System.Drawing.Point(83, 4);
            this.defaultIoNameTextBox.Name = "defaultIoNameTextBox";
            this.defaultIoNameTextBox.Size = new System.Drawing.Size(253, 21);
            this.defaultIoNameTextBox.TabIndex = 1;
            this.defaultIoNameTextBox.Text = "{memory_type}{byte}_{bit}";
            // 
            // defaultVariableLabel
            // 
            this.defaultVariableLabel.AutoSize = true;
            this.defaultVariableLabel.BackColor = System.Drawing.Color.Transparent;
            this.defaultVariableLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.defaultVariableLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.defaultVariableLabel.Location = new System.Drawing.Point(4, 29);
            this.defaultVariableLabel.Name = "defaultVariableLabel";
            this.defaultVariableLabel.Size = new System.Drawing.Size(72, 27);
            this.defaultVariableLabel.TabIndex = 0;
            this.defaultVariableLabel.Text = "Variabile";
            this.defaultVariableLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // defaultVariableTextBox
            // 
            this.defaultVariableTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.defaultVariableTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.defaultVariableTextBox.Location = new System.Drawing.Point(83, 32);
            this.defaultVariableTextBox.Name = "defaultVariableTextBox";
            this.defaultVariableTextBox.Size = new System.Drawing.Size(253, 21);
            this.defaultVariableTextBox.TabIndex = 1;
            this.defaultVariableTextBox.Text = "{io_name}";
            // 
            // ioTagTablePanel
            // 
            this.ioTagTablePanel.AutoSize = true;
            this.ioTagTablePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ioTagTablePanel.BackColor = System.Drawing.Color.Transparent;
            this.ioTagTablePanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.ioTagTablePanel.ColumnCount = 1;
            this.ioTagTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.ioTagTablePanel.Controls.Add(this.label1, 0, 0);
            this.ioTagTablePanel.Controls.Add(this.ioTagTablePanel_1, 0, 1);
            this.ioTagTablePanel.Location = new System.Drawing.Point(220, 16);
            this.ioTagTablePanel.Name = "ioTagTablePanel";
            this.ioTagTablePanel.RowCount = 2;
            this.ioTagTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ioTagTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ioTagTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ioTagTablePanel.Size = new System.Drawing.Size(286, 86);
            this.ioTagTablePanel.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(278, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tabella Tag IO";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ioTagTablePanel_1
            // 
            this.ioTagTablePanel_1.AutoSize = true;
            this.ioTagTablePanel_1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ioTagTablePanel_1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.ioTagTablePanel_1.ColumnCount = 2;
            this.ioTagTablePanel_1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.ioTagTablePanel_1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.ioTagTablePanel_1.Controls.Add(this.ioTagTableNameLabel, 0, 0);
            this.ioTagTablePanel_1.Controls.Add(this.ioTagTableNameTextBox, 1, 0);
            this.ioTagTablePanel_1.Controls.Add(this.ioTagTableSplitEveryLabel, 0, 1);
            this.ioTagTablePanel_1.Controls.Add(this.ioTagTableSplitEveryTextBox, 1, 1);
            this.ioTagTablePanel_1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ioTagTablePanel_1.Location = new System.Drawing.Point(4, 25);
            this.ioTagTablePanel_1.Name = "ioTagTablePanel_1";
            this.ioTagTablePanel_1.RowCount = 2;
            this.ioTagTablePanel_1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ioTagTablePanel_1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ioTagTablePanel_1.Size = new System.Drawing.Size(278, 57);
            this.ioTagTablePanel_1.TabIndex = 2;
            // 
            // ioTagTableNameLabel
            // 
            this.ioTagTableNameLabel.AutoSize = true;
            this.ioTagTableNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.ioTagTableNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ioTagTableNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ioTagTableNameLabel.Location = new System.Drawing.Point(4, 1);
            this.ioTagTableNameLabel.Name = "ioTagTableNameLabel";
            this.ioTagTableNameLabel.Size = new System.Drawing.Size(134, 27);
            this.ioTagTableNameLabel.TabIndex = 0;
            this.ioTagTableNameLabel.Text = "Nome";
            this.ioTagTableNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ioTagTableNameTextBox
            // 
            this.ioTagTableNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ioTagTableNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ioTagTableNameTextBox.Location = new System.Drawing.Point(145, 4);
            this.ioTagTableNameTextBox.Name = "ioTagTableNameTextBox";
            this.ioTagTableNameTextBox.Size = new System.Drawing.Size(129, 21);
            this.ioTagTableNameTextBox.TabIndex = 1;
            this.ioTagTableNameTextBox.Text = "fcTabellaIO";
            // 
            // ioTagTableSplitEveryLabel
            // 
            this.ioTagTableSplitEveryLabel.AutoSize = true;
            this.ioTagTableSplitEveryLabel.BackColor = System.Drawing.Color.Transparent;
            this.ioTagTableSplitEveryLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ioTagTableSplitEveryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ioTagTableSplitEveryLabel.Location = new System.Drawing.Point(4, 29);
            this.ioTagTableSplitEveryLabel.Name = "ioTagTableSplitEveryLabel";
            this.ioTagTableSplitEveryLabel.Size = new System.Drawing.Size(134, 27);
            this.ioTagTableSplitEveryLabel.TabIndex = 0;
            this.ioTagTableSplitEveryLabel.Text = "Nuova ogni n. bits";
            this.ioTagTableSplitEveryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ioTagTableSplitEveryTextBox
            // 
            this.ioTagTableSplitEveryTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ioTagTableSplitEveryTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ioTagTableSplitEveryTextBox.Location = new System.Drawing.Point(145, 32);
            this.ioTagTableSplitEveryTextBox.Name = "ioTagTableSplitEveryTextBox";
            this.ioTagTableSplitEveryTextBox.Size = new System.Drawing.Size(129, 21);
            this.ioTagTableSplitEveryTextBox.TabIndex = 1;
            this.ioTagTableSplitEveryTextBox.Text = "250";
            this.ioTagTableSplitEveryTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // divisionTypeComboBox
            // 
            this.divisionTypeComboBox.FormattingEnabled = true;
            this.divisionTypeComboBox.Items.AddRange(new object[] {
            "BitPerSegmento",
            "BytePerSegmento"});
            this.divisionTypeComboBox.Location = new System.Drawing.Point(622, 59);
            this.divisionTypeComboBox.Name = "divisionTypeComboBox";
            this.divisionTypeComboBox.Size = new System.Drawing.Size(121, 21);
            this.divisionTypeComboBox.TabIndex = 3;
            this.divisionTypeComboBox.Text = "BytePerSegmento";
            // 
            // divisionTypeLabel
            // 
            this.divisionTypeLabel.AutoSize = true;
            this.divisionTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.divisionTypeLabel.Location = new System.Drawing.Point(517, 60);
            this.divisionTypeLabel.Name = "divisionTypeLabel";
            this.divisionTypeLabel.Size = new System.Drawing.Size(98, 18);
            this.divisionTypeLabel.TabIndex = 2;
            this.divisionTypeLabel.Text = "Tipo divisione";
            // 
            // memoryTypeComboBox
            // 
            this.memoryTypeComboBox.FormattingEnabled = true;
            this.memoryTypeComboBox.Items.AddRange(new object[] {
            "DB",
            "Merker"});
            this.memoryTypeComboBox.Location = new System.Drawing.Point(622, 32);
            this.memoryTypeComboBox.Name = "memoryTypeComboBox";
            this.memoryTypeComboBox.Size = new System.Drawing.Size(121, 21);
            this.memoryTypeComboBox.TabIndex = 1;
            this.memoryTypeComboBox.Text = "DB";
            // 
            // memoryTypeLabel
            // 
            this.memoryTypeLabel.AutoSize = true;
            this.memoryTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memoryTypeLabel.Location = new System.Drawing.Point(516, 33);
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
            this.dataGridView.Location = new System.Drawing.Point(3, 199);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView.Size = new System.Drawing.Size(1087, 531);
            this.dataGridView.TabIndex = 2;
            // 
            // segmentNamesPanel
            // 
            this.segmentNamesPanel.AutoSize = true;
            this.segmentNamesPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.segmentNamesPanel.BackColor = System.Drawing.Color.Transparent;
            this.segmentNamesPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.segmentNamesPanel.ColumnCount = 1;
            this.segmentNamesPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.segmentNamesPanel.Controls.Add(this.label3, 0, 0);
            this.segmentNamesPanel.Controls.Add(this.segmentNamesPanel_1, 0, 1);
            this.segmentNamesPanel.Location = new System.Drawing.Point(351, 101);
            this.segmentNamesPanel.Name = "segmentNamesPanel";
            this.segmentNamesPanel.RowCount = 2;
            this.segmentNamesPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.segmentNamesPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.segmentNamesPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.segmentNamesPanel.Size = new System.Drawing.Size(409, 86);
            this.segmentNamesPanel.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(4, 1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(401, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "Nomi segmenti generati";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // segmentNamesPanel_1
            // 
            this.segmentNamesPanel_1.AutoSize = true;
            this.segmentNamesPanel_1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.segmentNamesPanel_1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.segmentNamesPanel_1.ColumnCount = 2;
            this.segmentNamesPanel_1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.segmentNamesPanel_1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.segmentNamesPanel_1.Controls.Add(this.segmentNamePerBitLabel, 0, 0);
            this.segmentNamesPanel_1.Controls.Add(this.segmentNamePerBitTextBox, 1, 0);
            this.segmentNamesPanel_1.Controls.Add(this.segmentNamePerByteLabel, 0, 1);
            this.segmentNamesPanel_1.Controls.Add(this.segmentNamePerByteTextBox, 1, 1);
            this.segmentNamesPanel_1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.segmentNamesPanel_1.Location = new System.Drawing.Point(4, 25);
            this.segmentNamesPanel_1.Name = "segmentNamesPanel_1";
            this.segmentNamesPanel_1.RowCount = 2;
            this.segmentNamesPanel_1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.segmentNamesPanel_1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.segmentNamesPanel_1.Size = new System.Drawing.Size(401, 57);
            this.segmentNamesPanel_1.TabIndex = 2;
            // 
            // segmentNamePerBitLabel
            // 
            this.segmentNamePerBitLabel.AutoSize = true;
            this.segmentNamePerBitLabel.BackColor = System.Drawing.Color.Transparent;
            this.segmentNamePerBitLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.segmentNamePerBitLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.segmentNamePerBitLabel.Location = new System.Drawing.Point(4, 1);
            this.segmentNamePerBitLabel.Name = "segmentNamePerBitLabel";
            this.segmentNamePerBitLabel.Size = new System.Drawing.Size(133, 27);
            this.segmentNamePerBitLabel.TabIndex = 0;
            this.segmentNamePerBitLabel.Text = "Divisione per bit";
            this.segmentNamePerBitLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // segmentNamePerBitTextBox
            // 
            this.segmentNamePerBitTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.segmentNamePerBitTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.segmentNamePerBitTextBox.Location = new System.Drawing.Point(144, 4);
            this.segmentNamePerBitTextBox.Name = "segmentNamePerBitTextBox";
            this.segmentNamePerBitTextBox.Size = new System.Drawing.Size(253, 21);
            this.segmentNamePerBitTextBox.TabIndex = 1;
            this.segmentNamePerBitTextBox.Text = "{memory_type}{byte}_{bit} - {comment}";
            // 
            // segmentNamePerByteLabel
            // 
            this.segmentNamePerByteLabel.AutoSize = true;
            this.segmentNamePerByteLabel.BackColor = System.Drawing.Color.Transparent;
            this.segmentNamePerByteLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.segmentNamePerByteLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.segmentNamePerByteLabel.Location = new System.Drawing.Point(4, 29);
            this.segmentNamePerByteLabel.Name = "segmentNamePerByteLabel";
            this.segmentNamePerByteLabel.Size = new System.Drawing.Size(133, 27);
            this.segmentNamePerByteLabel.TabIndex = 0;
            this.segmentNamePerByteLabel.Text = "Divisione per byte";
            this.segmentNamePerByteLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // segmentNamePerByteTextBox
            // 
            this.segmentNamePerByteTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.segmentNamePerByteTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.segmentNamePerByteTextBox.Location = new System.Drawing.Point(144, 32);
            this.segmentNamePerByteTextBox.Name = "segmentNamePerByteTextBox";
            this.segmentNamePerByteTextBox.Size = new System.Drawing.Size(253, 21);
            this.segmentNamePerByteTextBox.TabIndex = 1;
            this.segmentNamePerByteTextBox.Text = "{memory_type}B{byte}";
            // 
            // fcPanel
            // 
            this.fcPanel.AutoSize = true;
            this.fcPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.fcPanel.BackColor = System.Drawing.Color.Transparent;
            this.fcPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.fcPanel.ColumnCount = 1;
            this.fcPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.fcPanel.Controls.Add(this.label4, 0, 0);
            this.fcPanel.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.fcPanel.Location = new System.Drawing.Point(4, 16);
            this.fcPanel.Name = "fcPanel";
            this.fcPanel.RowCount = 2;
            this.fcPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.fcPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.fcPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.fcPanel.Size = new System.Drawing.Size(217, 86);
            this.fcPanel.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(4, 1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(209, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "FC";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.fcNameLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.fcNameTextBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.fcNumberLabel, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.fcNumberTextBox, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(4, 25);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(209, 57);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // fcNameLabel
            // 
            this.fcNameLabel.AutoSize = true;
            this.fcNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.fcNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fcNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fcNameLabel.Location = new System.Drawing.Point(4, 1);
            this.fcNameLabel.Name = "fcNameLabel";
            this.fcNameLabel.Size = new System.Drawing.Size(65, 27);
            this.fcNameLabel.TabIndex = 0;
            this.fcNameLabel.Text = "Nome";
            this.fcNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // fcNameTextBox
            // 
            this.fcNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fcNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fcNameTextBox.Location = new System.Drawing.Point(76, 4);
            this.fcNameTextBox.Name = "fcNameTextBox";
            this.fcNameTextBox.Size = new System.Drawing.Size(129, 21);
            this.fcNameTextBox.TabIndex = 1;
            this.fcNameTextBox.Text = "fcAppoggi";
            // 
            // fcNumberLabel
            // 
            this.fcNumberLabel.AutoSize = true;
            this.fcNumberLabel.BackColor = System.Drawing.Color.Transparent;
            this.fcNumberLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fcNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fcNumberLabel.Location = new System.Drawing.Point(4, 29);
            this.fcNumberLabel.Name = "fcNumberLabel";
            this.fcNumberLabel.Size = new System.Drawing.Size(65, 27);
            this.fcNumberLabel.TabIndex = 0;
            this.fcNumberLabel.Text = "Numero";
            this.fcNumberLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // fcNumberTextBox
            // 
            this.fcNumberTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fcNumberTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fcNumberTextBox.Location = new System.Drawing.Point(76, 32);
            this.fcNumberTextBox.Name = "fcNumberTextBox";
            this.fcNumberTextBox.Size = new System.Drawing.Size(129, 21);
            this.fcNumberTextBox.TabIndex = 1;
            this.fcNumberTextBox.Text = "175";
            this.fcNumberTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // prefixesLabel
            // 
            this.prefixesLabel.AutoSize = true;
            this.prefixesLabel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.prefixesLabel.BackColor = System.Drawing.Color.Transparent;
            this.prefixesLabel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.prefixesLabel.ColumnCount = 1;
            this.prefixesLabel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.prefixesLabel.Controls.Add(this.label5, 0, 0);
            this.prefixesLabel.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.prefixesLabel.Location = new System.Drawing.Point(759, 45);
            this.prefixesLabel.Name = "prefixesLabel";
            this.prefixesLabel.RowCount = 2;
            this.prefixesLabel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.prefixesLabel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.prefixesLabel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.prefixesLabel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.prefixesLabel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.prefixesLabel.Size = new System.Drawing.Size(260, 142);
            this.prefixesLabel.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(4, 1);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(252, 20);
            this.label5.TabIndex = 0;
            this.label5.Text = "Prefissi variabili";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.prefixInputDBLabel, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.prefixInputDBTextBox, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.prefixOutputDBLabel, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.prefixOutputDBTextBox, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.prefixInputMerkerLabel, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.prefixInputMerkerTextBox, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.prefixOutputMerkerLabel, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.prefixOutputMerkerTextBox, 1, 3);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(4, 25);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(252, 113);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // prefixInputDBLabel
            // 
            this.prefixInputDBLabel.AutoSize = true;
            this.prefixInputDBLabel.BackColor = System.Drawing.Color.Transparent;
            this.prefixInputDBLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prefixInputDBLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prefixInputDBLabel.Location = new System.Drawing.Point(4, 1);
            this.prefixInputDBLabel.Name = "prefixInputDBLabel";
            this.prefixInputDBLabel.Size = new System.Drawing.Size(117, 27);
            this.prefixInputDBLabel.TabIndex = 0;
            this.prefixInputDBLabel.Text = "Input(DB)";
            this.prefixInputDBLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // prefixInputDBTextBox
            // 
            this.prefixInputDBTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prefixInputDBTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prefixInputDBTextBox.Location = new System.Drawing.Point(128, 4);
            this.prefixInputDBTextBox.Name = "prefixInputDBTextBox";
            this.prefixInputDBTextBox.Size = new System.Drawing.Size(120, 21);
            this.prefixInputDBTextBox.TabIndex = 1;
            this.prefixInputDBTextBox.Text = "IN.";
            // 
            // prefixOutputDBLabel
            // 
            this.prefixOutputDBLabel.AutoSize = true;
            this.prefixOutputDBLabel.BackColor = System.Drawing.Color.Transparent;
            this.prefixOutputDBLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prefixOutputDBLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prefixOutputDBLabel.Location = new System.Drawing.Point(4, 29);
            this.prefixOutputDBLabel.Name = "prefixOutputDBLabel";
            this.prefixOutputDBLabel.Size = new System.Drawing.Size(117, 27);
            this.prefixOutputDBLabel.TabIndex = 0;
            this.prefixOutputDBLabel.Text = "Output(DB)";
            this.prefixOutputDBLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // prefixOutputDBTextBox
            // 
            this.prefixOutputDBTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prefixOutputDBTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prefixOutputDBTextBox.Location = new System.Drawing.Point(128, 32);
            this.prefixOutputDBTextBox.Name = "prefixOutputDBTextBox";
            this.prefixOutputDBTextBox.Size = new System.Drawing.Size(120, 21);
            this.prefixOutputDBTextBox.TabIndex = 1;
            this.prefixOutputDBTextBox.Text = "OUT.";
            // 
            // prefixInputMerkerLabel
            // 
            this.prefixInputMerkerLabel.AutoSize = true;
            this.prefixInputMerkerLabel.BackColor = System.Drawing.Color.Transparent;
            this.prefixInputMerkerLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prefixInputMerkerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prefixInputMerkerLabel.Location = new System.Drawing.Point(4, 57);
            this.prefixInputMerkerLabel.Name = "prefixInputMerkerLabel";
            this.prefixInputMerkerLabel.Size = new System.Drawing.Size(117, 27);
            this.prefixInputMerkerLabel.TabIndex = 2;
            this.prefixInputMerkerLabel.Text = "Input(Merker)";
            this.prefixInputMerkerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // prefixInputMerkerTextBox
            // 
            this.prefixInputMerkerTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prefixInputMerkerTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prefixInputMerkerTextBox.Location = new System.Drawing.Point(128, 60);
            this.prefixInputMerkerTextBox.Name = "prefixInputMerkerTextBox";
            this.prefixInputMerkerTextBox.Size = new System.Drawing.Size(120, 21);
            this.prefixInputMerkerTextBox.TabIndex = 3;
            this.prefixInputMerkerTextBox.Text = "MI_";
            // 
            // prefixOutputMerkerLabel
            // 
            this.prefixOutputMerkerLabel.AutoSize = true;
            this.prefixOutputMerkerLabel.BackColor = System.Drawing.Color.Transparent;
            this.prefixOutputMerkerLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prefixOutputMerkerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prefixOutputMerkerLabel.Location = new System.Drawing.Point(4, 85);
            this.prefixOutputMerkerLabel.Name = "prefixOutputMerkerLabel";
            this.prefixOutputMerkerLabel.Size = new System.Drawing.Size(117, 27);
            this.prefixOutputMerkerLabel.TabIndex = 4;
            this.prefixOutputMerkerLabel.Text = "Output(Merker)";
            this.prefixOutputMerkerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // prefixOutputMerkerTextBox
            // 
            this.prefixOutputMerkerTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prefixOutputMerkerTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prefixOutputMerkerTextBox.Location = new System.Drawing.Point(128, 88);
            this.prefixOutputMerkerTextBox.Name = "prefixOutputMerkerTextBox";
            this.prefixOutputMerkerTextBox.Size = new System.Drawing.Size(120, 21);
            this.prefixOutputMerkerTextBox.TabIndex = 5;
            this.prefixOutputMerkerTextBox.Text = "MO_";
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
            // IOGenerationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1093, 537);
            this.Controls.Add(this.mainPanel);
            this.Name = "IOGenerationForm";
            this.Text = "IOGenerationForm";
            this.mainPanel.ResumeLayout(false);
            this.configPanel.ResumeLayout(false);
            this.configPanel.PerformLayout();
            this.defaultFieldValuePanel.ResumeLayout(false);
            this.defaultFieldValuePanel.PerformLayout();
            this.defaultFieldValuePanel_1.ResumeLayout(false);
            this.defaultFieldValuePanel_1.PerformLayout();
            this.ioTagTablePanel.ResumeLayout(false);
            this.ioTagTablePanel.PerformLayout();
            this.ioTagTablePanel_1.ResumeLayout(false);
            this.ioTagTablePanel_1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.segmentNamesPanel.ResumeLayout(false);
            this.segmentNamesPanel.PerformLayout();
            this.segmentNamesPanel_1.ResumeLayout(false);
            this.segmentNamesPanel_1.PerformLayout();
            this.fcPanel.ResumeLayout(false);
            this.fcPanel.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.prefixesLabel.ResumeLayout(false);
            this.prefixesLabel.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.Panel configPanel;
        private CustomControls.FlatComboBox divisionTypeComboBox;
        private System.Windows.Forms.Label divisionTypeLabel;
        private CustomControls.FlatComboBox memoryTypeComboBox;
        private System.Windows.Forms.Label memoryTypeLabel;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.TableLayoutPanel ioTagTablePanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel ioTagTablePanel_1;
        private System.Windows.Forms.Label ioTagTableNameLabel;
        private CustomControls.FlatTextBox ioTagTableNameTextBox;
        private System.Windows.Forms.Label ioTagTableSplitEveryLabel;
        private CustomControls.FlatTextBox ioTagTableSplitEveryTextBox;
        private System.Windows.Forms.TableLayoutPanel defaultFieldValuePanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel defaultFieldValuePanel_1;
        private System.Windows.Forms.Label defaultIoNameLabel;
        private CustomControls.FlatTextBox defaultIoNameTextBox;
        private System.Windows.Forms.Label defaultVariableLabel;
        private CustomControls.FlatTextBox defaultVariableTextBox;
        private System.Windows.Forms.TableLayoutPanel segmentNamesPanel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel segmentNamesPanel_1;
        private System.Windows.Forms.Label segmentNamePerBitLabel;
        private CustomControls.FlatTextBox segmentNamePerBitTextBox;
        private System.Windows.Forms.Label segmentNamePerByteLabel;
        private CustomControls.FlatTextBox segmentNamePerByteTextBox;
        private System.Windows.Forms.TableLayoutPanel fcPanel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label fcNameLabel;
        private CustomControls.FlatTextBox fcNameTextBox;
        private System.Windows.Forms.Label fcNumberLabel;
        private CustomControls.FlatTextBox fcNumberTextBox;
        private System.Windows.Forms.TableLayoutPanel prefixesLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label prefixInputDBLabel;
        private CustomControls.FlatTextBox prefixInputDBTextBox;
        private System.Windows.Forms.Label prefixOutputDBLabel;
        private CustomControls.FlatTextBox prefixOutputDBTextBox;
        private System.Windows.Forms.Label prefixInputMerkerLabel;
        private CustomControls.FlatTextBox prefixInputMerkerTextBox;
        private System.Windows.Forms.Label prefixOutputMerkerLabel;
        private CustomControls.FlatTextBox prefixOutputMerkerTextBox;
        private System.Windows.Forms.Label label6;
    }
}