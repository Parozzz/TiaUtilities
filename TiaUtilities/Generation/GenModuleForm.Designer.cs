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
            toolsMenuItem = new ToolStripMenuItem();
            toolsPlaceholderViewerMenuItem = new ToolStripMenuItem();
            importExportMenuItem = new ToolStripMenuItem();
            exportXMLMenuItem = new ToolStripMenuItem();
            formTableLayout = new TableLayoutPanel();
            topMenuStrip.SuspendLayout();
            formTableLayout.SuspendLayout();
            SuspendLayout();
            // 
            // topMenuStrip
            // 
            topMenuStrip.Items.AddRange(new ToolStripItem[] { fileMenuItem, programMenuItem, toolsMenuItem, importExportMenuItem });
            topMenuStrip.Location = new Point(0, 0);
            topMenuStrip.Name = "topMenuStrip";
            topMenuStrip.Size = new Size(800, 24);
            topMenuStrip.TabIndex = 0;
            topMenuStrip.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            fileMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadMenuItem, saveMenuItem, saveAsMenuItem });
            fileMenuItem.Name = "fileMenuItem";
            fileMenuItem.Size = new Size(37, 20);
            fileMenuItem.Text = "File";
            // 
            // loadMenuItem
            // 
            loadMenuItem.Name = "loadMenuItem";
            loadMenuItem.Size = new Size(111, 22);
            loadMenuItem.Text = "Load";
            // 
            // saveMenuItem
            // 
            saveMenuItem.Name = "saveMenuItem";
            saveMenuItem.Size = new Size(111, 22);
            saveMenuItem.Text = "Save";
            // 
            // saveAsMenuItem
            // 
            saveAsMenuItem.Name = "saveAsMenuItem";
            saveAsMenuItem.Size = new Size(111, 22);
            saveAsMenuItem.Text = "SaveAs";
            // 
            // programMenuItem
            // 
            programMenuItem.DropDownItems.AddRange(new ToolStripItem[] { programSettingsMenuItem });
            programMenuItem.Font = new Font("Segoe UI", 9F);
            programMenuItem.Name = "programMenuItem";
            programMenuItem.Size = new Size(65, 20);
            programMenuItem.Text = "Program";
            // 
            // programSettingsMenuItem
            // 
            programSettingsMenuItem.Name = "programSettingsMenuItem";
            programSettingsMenuItem.Size = new Size(116, 22);
            programSettingsMenuItem.Text = "Settings";
            // 
            // toolsMenuItem
            // 
            toolsMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolsPlaceholderViewerMenuItem });
            toolsMenuItem.Name = "toolsMenuItem";
            toolsMenuItem.Size = new Size(47, 20);
            toolsMenuItem.Text = "Tools";
            // 
            // toolsPlaceholderViewerMenuItem
            // 
            toolsPlaceholderViewerMenuItem.Name = "toolsPlaceholderViewerMenuItem";
            toolsPlaceholderViewerMenuItem.Size = new Size(173, 22);
            toolsPlaceholderViewerMenuItem.Text = "Placeholder viewer";
            // 
            // importExportMenuItem
            // 
            importExportMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportXMLMenuItem });
            importExportMenuItem.Name = "importExportMenuItem";
            importExportMenuItem.Size = new Size(93, 20);
            importExportMenuItem.Text = "Import/Export";
            // 
            // exportXMLMenuItem
            // 
            exportXMLMenuItem.Name = "exportXMLMenuItem";
            exportXMLMenuItem.Size = new Size(134, 22);
            exportXMLMenuItem.Text = "Export XML";
            // 
            // formTableLayout
            // 
            formTableLayout.ColumnCount = 1;
            formTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            formTableLayout.Controls.Add(topMenuStrip, 0, 0);
            formTableLayout.Dock = DockStyle.Fill;
            formTableLayout.Location = new Point(0, 0);
            formTableLayout.Name = "formTableLayout";
            formTableLayout.RowCount = 2;
            formTableLayout.RowStyles.Add(new RowStyle());
            formTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            formTableLayout.Size = new Size(800, 450);
            formTableLayout.TabIndex = 0;
            // 
            // GenModuleForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(formTableLayout);
            DoubleBuffered = true;
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
    }
}
