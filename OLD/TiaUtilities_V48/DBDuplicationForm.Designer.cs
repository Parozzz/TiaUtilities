namespace TiaXmlReader
{
    partial class DBDuplicationForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.dbXMLPathTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.memberReplacementComboBox = new System.Windows.Forms.ComboBox();
            this.analyzeFileButton = new System.Windows.Forms.Button();
            this.replaceAndExportButton = new System.Windows.Forms.Button();
            this.replaceDBNameCheckBox = new System.Windows.Forms.CheckBox();
            this.replacementList1TextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.startingDBNumberTextBox = new System.Windows.Forms.TextBox();
            this.newDBNameTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.replacementList2TextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.replacementList3TextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.newNameTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(77, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "DB XML File";
            // 
            // dbXMLPathTextBox
            // 
            this.dbXMLPathTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.dbXMLPathTextBox.Location = new System.Drawing.Point(237, 12);
            this.dbXMLPathTextBox.Name = "dbXMLPathTextBox";
            this.dbXMLPathTextBox.Size = new System.Drawing.Size(715, 20);
            this.dbXMLPathTextBox.TabIndex = 2;
            this.dbXMLPathTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DBXMLPathTextBox_MouseClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(-3, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(223, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "Variables to replace";
            // 
            // memberReplacementComboBox
            // 
            this.memberReplacementComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memberReplacementComboBox.FormattingEnabled = true;
            this.memberReplacementComboBox.Location = new System.Drawing.Point(237, 57);
            this.memberReplacementComboBox.Name = "memberReplacementComboBox";
            this.memberReplacementComboBox.Size = new System.Drawing.Size(332, 28);
            this.memberReplacementComboBox.TabIndex = 5;
            // 
            // analyzeFileButton
            // 
            this.analyzeFileButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.analyzeFileButton.Location = new System.Drawing.Point(262, 550);
            this.analyzeFileButton.Name = "analyzeFileButton";
            this.analyzeFileButton.Size = new System.Drawing.Size(161, 43);
            this.analyzeFileButton.TabIndex = 6;
            this.analyzeFileButton.Text = "Analyze File";
            this.analyzeFileButton.UseVisualStyleBackColor = true;
            this.analyzeFileButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AnalyzeFileButton_MouseClick);
            // 
            // replaceAndExportButton
            // 
            this.replaceAndExportButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.replaceAndExportButton.Location = new System.Drawing.Point(488, 550);
            this.replaceAndExportButton.Name = "replaceAndExportButton";
            this.replaceAndExportButton.Size = new System.Drawing.Size(215, 43);
            this.replaceAndExportButton.TabIndex = 7;
            this.replaceAndExportButton.Text = "Replace And Export";
            this.replaceAndExportButton.UseVisualStyleBackColor = true;
            this.replaceAndExportButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ReplaceAndExportButton_MouseClick);
            // 
            // replaceDBNameCheckBox
            // 
            this.replaceDBNameCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.replaceDBNameCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.replaceDBNameCheckBox.Location = new System.Drawing.Point(12, 142);
            this.replaceDBNameCheckBox.Name = "replaceDBNameCheckBox";
            this.replaceDBNameCheckBox.Size = new System.Drawing.Size(239, 29);
            this.replaceDBNameCheckBox.TabIndex = 8;
            this.replaceDBNameCheckBox.Text = "Replace DB Name";
            this.replaceDBNameCheckBox.UseVisualStyleBackColor = true;
            this.replaceDBNameCheckBox.CheckedChanged += new System.EventHandler(this.ReplaceDBNameCheckBox_CheckedChanged);
            // 
            // replacementList1TextBox
            // 
            this.replacementList1TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.replacementList1TextBox.Location = new System.Drawing.Point(15, 267);
            this.replacementList1TextBox.MaxLength = 100000;
            this.replacementList1TextBox.Multiline = true;
            this.replacementList1TextBox.Name = "replacementList1TextBox";
            this.replacementList1TextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.replacementList1TextBox.Size = new System.Drawing.Size(280, 264);
            this.replacementList1TextBox.TabIndex = 9;
            this.replacementList1TextBox.TextChanged += new System.EventHandler(this.ReplacementList1TextBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(52, 239);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(207, 25);
            this.label3.TabIndex = 10;
            this.label3.Text = "Replacement list 1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 170);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(220, 25);
            this.label4.TabIndex = 11;
            this.label4.Text = "Starting DB Number";
            // 
            // startingDBNumberTextBox
            // 
            this.startingDBNumberTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.startingDBNumberTextBox.Location = new System.Drawing.Point(237, 169);
            this.startingDBNumberTextBox.MaxLength = 100000;
            this.startingDBNumberTextBox.Name = "startingDBNumberTextBox";
            this.startingDBNumberTextBox.Size = new System.Drawing.Size(107, 26);
            this.startingDBNumberTextBox.TabIndex = 12;
            this.startingDBNumberTextBox.TextChanged += new System.EventHandler(this.StartingDBNumberTextBox_TextChanged);
            // 
            // newDBNameTextBox
            // 
            this.newDBNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.newDBNameTextBox.Location = new System.Drawing.Point(237, 199);
            this.newDBNameTextBox.MaxLength = 100000;
            this.newDBNameTextBox.Name = "newDBNameTextBox";
            this.newDBNameTextBox.Size = new System.Drawing.Size(715, 26);
            this.newDBNameTextBox.TabIndex = 14;
            this.newDBNameTextBox.Text = "{replacement1}{replacement2}{replacement3}";
            this.newDBNameTextBox.TextChanged += new System.EventHandler(this.NewDBNameTextBox_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(70, 200);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(162, 25);
            this.label5.TabIndex = 13;
            this.label5.Text = "New DB Name";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(379, 239);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(207, 25);
            this.label6.TabIndex = 16;
            this.label6.Text = "Replacement list 2";
            // 
            // replacementList2TextBox
            // 
            this.replacementList2TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.replacementList2TextBox.Location = new System.Drawing.Point(342, 267);
            this.replacementList2TextBox.MaxLength = 100000;
            this.replacementList2TextBox.Multiline = true;
            this.replacementList2TextBox.Name = "replacementList2TextBox";
            this.replacementList2TextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.replacementList2TextBox.Size = new System.Drawing.Size(280, 264);
            this.replacementList2TextBox.TabIndex = 15;
            this.replacementList2TextBox.TextChanged += new System.EventHandler(this.ReplacementList2TextBox_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(706, 239);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(207, 25);
            this.label7.TabIndex = 18;
            this.label7.Text = "Replacement list 3";
            // 
            // replacementList3TextBox
            // 
            this.replacementList3TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.replacementList3TextBox.Location = new System.Drawing.Point(669, 267);
            this.replacementList3TextBox.MaxLength = 100000;
            this.replacementList3TextBox.Multiline = true;
            this.replacementList3TextBox.Name = "replacementList3TextBox";
            this.replacementList3TextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.replacementList3TextBox.Size = new System.Drawing.Size(280, 264);
            this.replacementList3TextBox.TabIndex = 17;
            this.replacementList3TextBox.TextChanged += new System.EventHandler(this.ReplacementList3TextBox_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(8, 91);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(212, 25);
            this.label8.TabIndex = 19;
            this.label8.Text = "New variable name";
            // 
            // newNameTextBox
            // 
            this.newNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.newNameTextBox.Location = new System.Drawing.Point(237, 90);
            this.newNameTextBox.MaxLength = 100000;
            this.newNameTextBox.Name = "newNameTextBox";
            this.newNameTextBox.Size = new System.Drawing.Size(715, 26);
            this.newNameTextBox.TabIndex = 20;
            this.newNameTextBox.Text = "{replacement1}{replacement2}{replacement3}";
            this.newNameTextBox.TextChanged += new System.EventHandler(this.NewNameTextBox_TextChanged);
            // 
            // DBDuplicationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(965, 602);
            this.Controls.Add(this.newNameTextBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.replacementList3TextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.replacementList2TextBox);
            this.Controls.Add(this.newDBNameTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.startingDBNumberTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.replacementList1TextBox);
            this.Controls.Add(this.replaceDBNameCheckBox);
            this.Controls.Add(this.replaceAndExportButton);
            this.Controls.Add(this.analyzeFileButton);
            this.Controls.Add(this.memberReplacementComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dbXMLPathTextBox);
            this.Name = "DBDuplicationForm";
            this.Text = "DBA Cloning";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox dbXMLPathTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox memberReplacementComboBox;
        private System.Windows.Forms.Button analyzeFileButton;
        private System.Windows.Forms.Button replaceAndExportButton;
        private System.Windows.Forms.CheckBox replaceDBNameCheckBox;
        private System.Windows.Forms.TextBox replacementList1TextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox startingDBNumberTextBox;
        private System.Windows.Forms.TextBox newDBNameTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox replacementList2TextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox replacementList3TextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox newNameTextBox;
    }
}