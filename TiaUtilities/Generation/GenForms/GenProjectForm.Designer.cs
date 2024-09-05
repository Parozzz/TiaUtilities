namespace TiaUtilities.Generation.GenForms
{
    partial class GenProjectForm
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
            saveMenuItem = new ToolStripMenuItem();
            saveAsMenuItem = new ToolStripMenuItem();
            loadMenuItem = new ToolStripMenuItem();
            importExportMenuItem = new ToolStripMenuItem();
            exportXMLMenuItem = new ToolStripMenuItem();
            preferencesMenuItem = new ToolStripMenuItem();
            projectTableLayout = new TableLayoutPanel();
            formTableLayout = new TableLayoutPanel();
            topMenuStrip.SuspendLayout();
            formTableLayout.SuspendLayout();
            SuspendLayout();
            // 
            // topMenuStrip
            // 
            topMenuStrip.Items.AddRange(new ToolStripItem[] { fileMenuItem, importExportMenuItem, preferencesMenuItem });
            topMenuStrip.Location = new Point(0, 0);
            topMenuStrip.Name = "topMenuStrip";
            topMenuStrip.Size = new Size(800, 24);
            topMenuStrip.TabIndex = 0;
            topMenuStrip.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            fileMenuItem.DropDownItems.AddRange(new ToolStripItem[] { saveMenuItem, saveAsMenuItem, loadMenuItem });
            fileMenuItem.Name = "fileMenuItem";
            fileMenuItem.Size = new Size(37, 20);
            fileMenuItem.Text = "File";
            // 
            // saveMenuItem
            // 
            saveMenuItem.Name = "saveMenuItem";
            saveMenuItem.Size = new Size(180, 22);
            saveMenuItem.Text = "Save";
            // 
            // saveAsMenuItem
            // 
            saveAsMenuItem.Name = "saveAsMenuItem";
            saveAsMenuItem.Size = new Size(180, 22);
            saveAsMenuItem.Text = "SaveAs";
            // 
            // loadMenuItem
            // 
            loadMenuItem.Name = "loadMenuItem";
            loadMenuItem.Size = new Size(180, 22);
            loadMenuItem.Text = "Load";
            // 
            // importExportMenuItem
            // 
            importExportMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportXMLMenuItem });
            importExportMenuItem.Name = "importExportMenuItem";
            importExportMenuItem.Size = new Size(94, 20);
            importExportMenuItem.Text = "Import/Export";
            // 
            // exportXMLMenuItem
            // 
            exportXMLMenuItem.Name = "exportXMLMenuItem";
            exportXMLMenuItem.Size = new Size(180, 22);
            exportXMLMenuItem.Text = "Export XML";
            // 
            // preferencesMenuItem
            // 
            preferencesMenuItem.Name = "preferencesMenuItem";
            preferencesMenuItem.Size = new Size(80, 20);
            preferencesMenuItem.Text = "Preferences";
            // 
            // projectTableLayout
            // 
            projectTableLayout.AutoSize = true;
            projectTableLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            projectTableLayout.ColumnCount = 1;
            projectTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            projectTableLayout.Dock = DockStyle.Fill;
            projectTableLayout.Location = new Point(3, 27);
            projectTableLayout.Name = "projectTableLayout";
            projectTableLayout.RowCount = 2;
            projectTableLayout.RowStyles.Add(new RowStyle());
            projectTableLayout.RowStyles.Add(new RowStyle());
            projectTableLayout.Size = new Size(794, 420);
            projectTableLayout.TabIndex = 1;
            // 
            // formTableLayout
            // 
            formTableLayout.ColumnCount = 1;
            formTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            formTableLayout.Controls.Add(topMenuStrip, 0, 0);
            formTableLayout.Controls.Add(projectTableLayout, 0, 1);
            formTableLayout.Dock = DockStyle.Fill;
            formTableLayout.Location = new Point(0, 0);
            formTableLayout.Name = "formTableLayout";
            formTableLayout.RowCount = 2;
            formTableLayout.RowStyles.Add(new RowStyle());
            formTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            formTableLayout.Size = new Size(800, 450);
            formTableLayout.TabIndex = 0;
            // 
            // GenerationProjectForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(formTableLayout);
            Name = "GenerationProjectForm";
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
        private ToolStripMenuItem preferencesMenuItem;
        private TableLayoutPanel projectTableLayout;
        private TableLayoutPanel formTableLayout;
        public ToolStripMenuItem importExportMenuItem;
    }
}