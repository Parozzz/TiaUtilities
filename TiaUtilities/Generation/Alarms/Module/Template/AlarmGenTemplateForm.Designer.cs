﻿using TiaUtilities.Generation.Alarms.Module.Template;
using TiaUtilities.Generation.Alarms.Module;

namespace TiaUtilities.Generation.Alarms.Module.Template
{
    partial class AlarmGenTemplateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlarmGenTemplateForm));
            mainPanel = new TableLayoutPanel();
            topPanel = new FlowLayoutPanel();
            selectPanel = new TableLayoutPanel();
            selectLabel = new Label();
            selectComboBox = new TiaUtilities.CustomControls.FlatComboBox();
            addButton = new Button();
            removeButton = new Button();
            renameButton = new Button();
            cloneButton = new Button();
            configurationButton = new Button();
            mainPanel.SuspendLayout();
            topPanel.SuspendLayout();
            selectPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.AutoSize = true;
            mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainPanel.ColumnCount = 1;
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainPanel.Controls.Add(topPanel, 0, 0);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 0);
            mainPanel.Name = "mainPanel";
            mainPanel.RowCount = 2;
            mainPanel.RowStyles.Add(new RowStyle());
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.Size = new Size(1184, 661);
            mainPanel.TabIndex = 0;
            // 
            // topPanel
            // 
            topPanel.AutoSize = true;
            topPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            topPanel.Controls.Add(selectPanel);
            topPanel.Controls.Add(addButton);
            topPanel.Controls.Add(removeButton);
            topPanel.Controls.Add(renameButton);
            topPanel.Controls.Add(cloneButton);
            topPanel.Controls.Add(configurationButton);
            topPanel.Dock = DockStyle.Fill;
            topPanel.Location = new Point(3, 3);
            topPanel.Name = "topPanel";
            topPanel.Size = new Size(1178, 37);
            topPanel.TabIndex = 0;
            // 
            // selectPanel
            // 
            selectPanel.AutoSize = true;
            selectPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            selectPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            selectPanel.ColumnCount = 2;
            selectPanel.ColumnStyles.Add(new ColumnStyle());
            selectPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            selectPanel.Controls.Add(selectLabel, 0, 0);
            selectPanel.Controls.Add(selectComboBox, 1, 0);
            selectPanel.Location = new Point(3, 3);
            selectPanel.Name = "selectPanel";
            selectPanel.RowCount = 1;
            selectPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            selectPanel.Size = new Size(265, 31);
            selectPanel.TabIndex = 0;
            // 
            // selectLabel
            // 
            selectLabel.AutoSize = true;
            selectLabel.Dock = DockStyle.Fill;
            selectLabel.Location = new Point(4, 1);
            selectLabel.Name = "selectLabel";
            selectLabel.Size = new Size(90, 29);
            selectLabel.TabIndex = 0;
            selectLabel.Text = "Select Template";
            selectLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // selectComboBox
            // 
            selectComboBox.BackColor = SystemColors.Control;
            selectComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            selectComboBox.FormattingEnabled = true;
            selectComboBox.Location = new Point(101, 4);
            selectComboBox.Name = "selectComboBox";
            selectComboBox.Size = new Size(160, 23);
            selectComboBox.TabIndex = 1;
            // 
            // addButton
            // 
            addButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            addButton.BackgroundImage = (Image)resources.GetObject("addButton.BackgroundImage");
            addButton.BackgroundImageLayout = ImageLayout.Zoom;
            addButton.Dock = DockStyle.Left;
            addButton.FlatAppearance.BorderColor = SystemColors.Control;
            addButton.FlatStyle = FlatStyle.Flat;
            addButton.Font = new Font("Segoe UI", 11F);
            addButton.Location = new Point(279, 0);
            addButton.Margin = new Padding(8, 0, 0, 0);
            addButton.Name = "addButton";
            addButton.Size = new Size(37, 37);
            addButton.TabIndex = 1;
            addButton.UseVisualStyleBackColor = true;
            // 
            // removeButton
            // 
            removeButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            removeButton.BackgroundImage = (Image)resources.GetObject("removeButton.BackgroundImage");
            removeButton.BackgroundImageLayout = ImageLayout.Zoom;
            removeButton.Dock = DockStyle.Left;
            removeButton.FlatAppearance.BorderColor = SystemColors.Control;
            removeButton.FlatStyle = FlatStyle.Flat;
            removeButton.Font = new Font("Segoe UI", 11F);
            removeButton.Location = new Point(324, 0);
            removeButton.Margin = new Padding(8, 0, 0, 0);
            removeButton.Name = "removeButton";
            removeButton.Size = new Size(37, 37);
            removeButton.TabIndex = 2;
            removeButton.UseVisualStyleBackColor = true;
            // 
            // renameButton
            // 
            renameButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            renameButton.BackgroundImage = (Image)resources.GetObject("renameButton.BackgroundImage");
            renameButton.BackgroundImageLayout = ImageLayout.Zoom;
            renameButton.Dock = DockStyle.Left;
            renameButton.FlatAppearance.BorderColor = SystemColors.Control;
            renameButton.FlatStyle = FlatStyle.Flat;
            renameButton.Font = new Font("Segoe UI", 11F);
            renameButton.Location = new Point(369, 0);
            renameButton.Margin = new Padding(8, 0, 0, 0);
            renameButton.Name = "renameButton";
            renameButton.Size = new Size(40, 37);
            renameButton.TabIndex = 3;
            renameButton.UseVisualStyleBackColor = true;
            // 
            // cloneButton
            // 
            cloneButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            cloneButton.BackgroundImage = (Image)resources.GetObject("cloneButton.BackgroundImage");
            cloneButton.BackgroundImageLayout = ImageLayout.Zoom;
            cloneButton.Dock = DockStyle.Left;
            cloneButton.FlatAppearance.BorderColor = SystemColors.Control;
            cloneButton.FlatStyle = FlatStyle.Flat;
            cloneButton.Font = new Font("Segoe UI", 11F);
            cloneButton.Location = new Point(417, 0);
            cloneButton.Margin = new Padding(8, 0, 0, 0);
            cloneButton.Name = "cloneButton";
            cloneButton.Size = new Size(40, 37);
            cloneButton.TabIndex = 4;
            cloneButton.UseVisualStyleBackColor = true;
            // 
            // configurationButton
            // 
            configurationButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            configurationButton.BackgroundImage = (Image)resources.GetObject("configurationButton.BackgroundImage");
            configurationButton.BackgroundImageLayout = ImageLayout.Zoom;
            configurationButton.Dock = DockStyle.Left;
            configurationButton.FlatAppearance.BorderColor = SystemColors.Control;
            configurationButton.FlatStyle = FlatStyle.Flat;
            configurationButton.Font = new Font("Segoe UI", 11F);
            configurationButton.Location = new Point(465, 0);
            configurationButton.Margin = new Padding(8, 0, 0, 0);
            configurationButton.Name = "configurationButton";
            configurationButton.Size = new Size(40, 37);
            configurationButton.TabIndex = 5;
            configurationButton.UseVisualStyleBackColor = true;
            // 
            // AlarmGenTemplateForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1184, 661);
            Controls.Add(mainPanel);
            DoubleBuffered = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AlarmGenTemplateForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Template Editor";
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            topPanel.ResumeLayout(false);
            topPanel.PerformLayout();
            selectPanel.ResumeLayout(false);
            selectPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel mainPanel;
        private FlowLayoutPanel topPanel;
        private TableLayoutPanel selectPanel;
        private Label selectLabel;
        private TiaUtilities.CustomControls.FlatComboBox selectComboBox;
        private Button addButton;
        private Button removeButton;
        private Button renameButton;
        private Button cloneButton;
        private Button configurationButton;
    }
}
