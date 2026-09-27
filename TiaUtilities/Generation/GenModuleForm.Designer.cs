namespace TiaUtilities.Generation
{
    partial class GenModuleForm
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
            topMenuStrip = new MenuStrip();
            fileMenuItem = new ToolStripMenuItem();
            loadMenuItem = new ToolStripMenuItem();
            saveMenuItem = new ToolStripMenuItem();
            saveAsMenuItem = new ToolStripMenuItem();
            programMenuItem = new ToolStripMenuItem();
            programSettingsMenuItem = new ToolStripMenuItem();
            programModuleSetupMenuItem = new ToolStripMenuItem();
            toolsMenuItem = new ToolStripMenuItem();
            toolsPlaceholderViewerMenuItem = new ToolStripMenuItem();
            toolsTextsEditorMenuItem = new ToolStripMenuItem();
            importExportMenuItem = new ToolStripMenuItem();
            exportXMLMenuItem = new ToolStripMenuItem();
            formTableLayout = new TableLayoutPanel();
            selectControlButtonPanel = new FlowLayoutPanel();
            topMenuStrip.SuspendLayout();
            formTableLayout.SuspendLayout();
            SuspendLayout();
            // 
            // topMenuStrip
            // 
            topMenuStrip.ImageScalingSize = new Size(20, 20);
            topMenuStrip.Items.AddRange(new ToolStripItem[] { fileMenuItem, programMenuItem, toolsMenuItem, importExportMenuItem });
            topMenuStrip.Location = new Point(0, 0);
            topMenuStrip.Name = "topMenuStrip";
            topMenuStrip.Padding = new Padding(7, 3, 0, 3);
            topMenuStrip.Size = new Size(914, 30);
            topMenuStrip.TabIndex = 0;
            topMenuStrip.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            fileMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadMenuItem, saveMenuItem, saveAsMenuItem });
            fileMenuItem.Name = "fileMenuItem";
            fileMenuItem.Size = new Size(46, 24);
            fileMenuItem.Text = "File";
            // 
            // loadMenuItem
            // 
            loadMenuItem.Name = "loadMenuItem";
            loadMenuItem.Size = new Size(139, 26);
            loadMenuItem.Text = "Load";
            // 
            // saveMenuItem
            // 
            saveMenuItem.Name = "saveMenuItem";
            saveMenuItem.Size = new Size(139, 26);
            saveMenuItem.Text = "Save";
            // 
            // saveAsMenuItem
            // 
            saveAsMenuItem.Name = "saveAsMenuItem";
            saveAsMenuItem.Size = new Size(139, 26);
            saveAsMenuItem.Text = "SaveAs";
            // 
            // programMenuItem
            // 
            programMenuItem.DropDownItems.AddRange(new ToolStripItem[] { programSettingsMenuItem, programModuleSetupMenuItem });
            programMenuItem.Font = new Font("Segoe UI", 9F);
            programMenuItem.Name = "programMenuItem";
            programMenuItem.Size = new Size(80, 24);
            programMenuItem.Text = "Program";
            // 
            // programSettingsMenuItem
            // 
            programSettingsMenuItem.Name = "programSettingsMenuItem";
            programSettingsMenuItem.Size = new Size(181, 26);
            programSettingsMenuItem.Text = "Settings";
            // 
            // programModuleSetupMenuItem
            // 
            programModuleSetupMenuItem.Name = "programModuleSetupMenuItem";
            programModuleSetupMenuItem.Size = new Size(181, 26);
            programModuleSetupMenuItem.Text = "ModuleSetup";
            // 
            // toolsMenuItem
            // 
            toolsMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolsPlaceholderViewerMenuItem, toolsTextsEditorMenuItem });
            toolsMenuItem.Name = "toolsMenuItem";
            toolsMenuItem.Size = new Size(58, 24);
            toolsMenuItem.Text = "Tools";
            // 
            // toolsPlaceholderViewerMenuItem
            // 
            toolsPlaceholderViewerMenuItem.Name = "toolsPlaceholderViewerMenuItem";
            toolsPlaceholderViewerMenuItem.Size = new Size(217, 26);
            toolsPlaceholderViewerMenuItem.Text = "Placeholder viewer";
            // 
            // toolsTextsEditorMenuItem
            // 
            toolsTextsEditorMenuItem.Name = "toolsTextsEditorMenuItem";
            toolsTextsEditorMenuItem.Size = new Size(217, 26);
            toolsTextsEditorMenuItem.Text = "Texts Editor";
            // 
            // importExportMenuItem
            // 
            importExportMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportXMLMenuItem });
            importExportMenuItem.Name = "importExportMenuItem";
            importExportMenuItem.Size = new Size(117, 24);
            importExportMenuItem.Text = "Import/Export";
            // 
            // exportXMLMenuItem
            // 
            exportXMLMenuItem.Name = "exportXMLMenuItem";
            exportXMLMenuItem.Size = new Size(168, 26);
            exportXMLMenuItem.Text = "Export XML";
            // 
            // formTableLayout
            // 
            formTableLayout.ColumnCount = 1;
            formTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            formTableLayout.Controls.Add(topMenuStrip, 0, 0);
            formTableLayout.Controls.Add(selectControlButtonPanel, 0, 1);
            formTableLayout.Dock = DockStyle.Fill;
            formTableLayout.Location = new Point(0, 0);
            formTableLayout.Margin = new Padding(3, 4, 3, 4);
            formTableLayout.Name = "formTableLayout";
            formTableLayout.RowCount = 3;
            formTableLayout.RowStyles.Add(new RowStyle());
            formTableLayout.RowStyles.Add(new RowStyle());
            formTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            formTableLayout.Size = new Size(914, 600);
            formTableLayout.TabIndex = 0;
            // 
            // selectControlButtonPanel
            // 
            selectControlButtonPanel.AutoSize = true;
            selectControlButtonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            selectControlButtonPanel.Dock = DockStyle.Fill;
            selectControlButtonPanel.Location = new Point(3, 33);
            selectControlButtonPanel.Name = "selectControlButtonPanel";
            selectControlButtonPanel.Size = new Size(908, 1);
            selectControlButtonPanel.TabIndex = 1;
            // 
            // GenModuleForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(formTableLayout);
            DoubleBuffered = true;
            Margin = new Padding(3, 4, 3, 4);
            Name = "GenModuleForm";
            Text = "GenerationProjectForm";
            topMenuStrip.ResumeLayout(false);
            topMenuStrip.PerformLayout();
            formTableLayout.ResumeLayout(false);
            formTableLayout.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private MenuStrip topMenuStrip;
        private ToolStripMenuItem fileMenuItem;
        private ToolStripMenuItem saveMenuItem;
        private ToolStripMenuItem saveAsMenuItem;
        private ToolStripMenuItem loadMenuItem;
        private ToolStripMenuItem exportXMLMenuItem;
        private TableLayoutPanel formTableLayout;
        public ToolStripMenuItem importExportMenuItem;
        private ToolStripMenuItem programMenuItem;
        private ToolStripMenuItem programSettingsMenuItem;
        private ToolStripMenuItem toolsMenuItem;
        private ToolStripMenuItem toolsPlaceholderViewerMenuItem;
        public ToolStripMenuItem programModuleSetupMenuItem;
        private ToolStripMenuItem toolsTextsEditorMenuItem;
        private FlowLayoutPanel selectControlButtonPanel;
    }
}
