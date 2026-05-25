namespace TiaUtilities.Generation.TextsEditor
{
    partial class GenModuleTextsEditorForm
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
            mainPanel = new TableLayoutPanel();
            bottomButtonPanel = new TableLayoutPanel();
            saveButton = new Button();
            importSvgButton = new Button();
            exportSvgButton = new Button();
            mainPanel.SuspendLayout();
            bottomButtonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.AutoSize = true;
            mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainPanel.ColumnCount = 1;
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainPanel.Controls.Add(bottomButtonPanel, 0, 1);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 0);
            mainPanel.Name = "mainPanel";
            mainPanel.RowCount = 2;
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.RowStyles.Add(new RowStyle());
            mainPanel.Size = new Size(800, 450);
            mainPanel.TabIndex = 0;
            // 
            // bottomButtonPanel
            // 
            bottomButtonPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            bottomButtonPanel.AutoSize = true;
            bottomButtonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            bottomButtonPanel.ColumnCount = 7;
            bottomButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            bottomButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            bottomButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            bottomButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            bottomButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            bottomButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            bottomButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            bottomButtonPanel.Controls.Add(saveButton, 1, 0);
            bottomButtonPanel.Controls.Add(importSvgButton, 3, 0);
            bottomButtonPanel.Controls.Add(exportSvgButton, 5, 0);
            bottomButtonPanel.Location = new Point(3, 396);
            bottomButtonPanel.Name = "bottomButtonPanel";
            bottomButtonPanel.RowCount = 1;
            bottomButtonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            bottomButtonPanel.Size = new Size(794, 51);
            bottomButtonPanel.TabIndex = 0;
            // 
            // saveButton
            // 
            saveButton.Anchor = AnchorStyles.None;
            saveButton.AutoSize = true;
            saveButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            saveButton.Location = new Point(108, 5);
            saveButton.Margin = new Padding(5);
            saveButton.Name = "saveButton";
            saveButton.Padding = new Padding(5);
            saveButton.Size = new Size(75, 41);
            saveButton.TabIndex = 0;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            // 
            // importSvgButton
            // 
            importSvgButton.Anchor = AnchorStyles.None;
            importSvgButton.AutoSize = true;
            importSvgButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            importSvgButton.Location = new Point(325, 5);
            importSvgButton.Margin = new Padding(5);
            importSvgButton.Name = "importSvgButton";
            importSvgButton.Padding = new Padding(5);
            importSvgButton.Size = new Size(143, 41);
            importSvgButton.TabIndex = 1;
            importSvgButton.Text = "Import Svg file";
            importSvgButton.UseVisualStyleBackColor = true;
            // 
            // exportSvgButton
            // 
            exportSvgButton.Anchor = AnchorStyles.None;
            exportSvgButton.AutoSize = true;
            exportSvgButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            exportSvgButton.Location = new Point(576, 5);
            exportSvgButton.Margin = new Padding(5);
            exportSvgButton.Name = "exportSvgButton";
            exportSvgButton.Padding = new Padding(5);
            exportSvgButton.Size = new Size(143, 41);
            exportSvgButton.TabIndex = 2;
            exportSvgButton.Text = "Export Svg file";
            exportSvgButton.UseVisualStyleBackColor = true;
            // 
            // GenModuleTextsEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(mainPanel);
            Name = "GenModuleTextsEditorForm";
            Text = "GenModuleTextEditor";
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            bottomButtonPanel.ResumeLayout(false);
            bottomButtonPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel mainPanel;
        private TableLayoutPanel bottomButtonPanel;
        private Button saveButton;
        private Button importSvgButton;
        private Button exportSvgButton;
    }
}