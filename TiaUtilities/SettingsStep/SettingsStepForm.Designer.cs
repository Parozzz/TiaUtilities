using TiaUtilities.CustomControls;
using TiaUtilities.Resources;
using TiaUtilities.SettingsStep.CustomControls;

namespace TiaUtilities.SettingsNew
{
    partial class SettingsStepForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsStepForm));
            mainTable = new TableLayoutPanelColorizable();
            selectedConfigurationPanel = new FlowLayoutPanel();
            selectedConfigurationNameLabel = new Label();
            selectConfigurationPanel = new FlowLayoutPanel();
            selectConfigurationLabel = new Label();
            selectConfigurationComboBox = new ComboBoxFilterable();
            stepFlowPanel = new FlowLayoutPanel();
            bottomPanel = new Panel();
            controlsPanel = new TableLayoutPanelColorizable();
            mainTable.SuspendLayout();
            selectedConfigurationPanel.SuspendLayout();
            selectConfigurationPanel.SuspendLayout();
            bottomPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainTable
            // 
            mainTable.AutoSize = true;
            mainTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainTable.ColumnCount = 1;
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainTable.Controls.Add(selectedConfigurationPanel, 0, 1);
            mainTable.Controls.Add(selectConfigurationPanel, 0, 0);
            mainTable.Controls.Add(stepFlowPanel, 0, 2);
            mainTable.Controls.Add(bottomPanel, 0, 3);
            mainTable.Dock = DockStyle.Fill;
            mainTable.Location = new Point(0, 0);
            mainTable.Margin = new Padding(0);
            mainTable.Name = "mainTable";
            mainTable.Padding = new Padding(4);
            mainTable.RowCount = 4;
            mainTable.RowStyles.Add(new RowStyle());
            mainTable.RowStyles.Add(new RowStyle());
            mainTable.RowStyles.Add(new RowStyle());
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 15F));
            mainTable.Size = new Size(772, 541);
            mainTable.TabIndex = 0;
            // 
            // selectedConfigurationPanel
            // 
            selectedConfigurationPanel.Anchor = AnchorStyles.None;
            selectedConfigurationPanel.AutoSize = true;
            selectedConfigurationPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            selectedConfigurationPanel.Controls.Add(selectedConfigurationNameLabel);
            selectedConfigurationPanel.Location = new Point(289, 39);
            selectedConfigurationPanel.Name = "selectedConfigurationPanel";
            selectedConfigurationPanel.Size = new Size(193, 31);
            selectedConfigurationPanel.TabIndex = 1;
            // 
            // selectedConfigurationNameLabel
            // 
            selectedConfigurationNameLabel.Anchor = AnchorStyles.None;
            selectedConfigurationNameLabel.AutoSize = true;
            selectedConfigurationNameLabel.Location = new Point(0, 8);
            selectedConfigurationNameLabel.Margin = new Padding(0, 8, 0, 8);
            selectedConfigurationNameLabel.Name = "selectedConfigurationNameLabel";
            selectedConfigurationNameLabel.Size = new Size(193, 15);
            selectedConfigurationNameLabel.TabIndex = 2;
            selectedConfigurationNameLabel.Text = "SELECTED CONFIGURATION NAME";
            selectedConfigurationNameLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // selectConfigurationPanel
            // 
            selectConfigurationPanel.AutoSize = true;
            selectConfigurationPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            selectConfigurationPanel.Controls.Add(selectConfigurationLabel);
            selectConfigurationPanel.Controls.Add(selectConfigurationComboBox);
            selectConfigurationPanel.Dock = DockStyle.Fill;
            selectConfigurationPanel.Location = new Point(4, 4);
            selectConfigurationPanel.Margin = new Padding(0);
            selectConfigurationPanel.Name = "selectConfigurationPanel";
            selectConfigurationPanel.Padding = new Padding(4);
            selectConfigurationPanel.Size = new Size(764, 32);
            selectConfigurationPanel.TabIndex = 0;
            // 
            // selectConfigurationLabel
            // 
            selectConfigurationLabel.AutoSize = true;
            selectConfigurationLabel.Dock = DockStyle.Left;
            selectConfigurationLabel.Location = new Point(4, 4);
            selectConfigurationLabel.Margin = new Padding(0, 0, 9, 0);
            selectConfigurationLabel.Name = "selectConfigurationLabel";
            selectConfigurationLabel.Size = new Size(138, 24);
            selectConfigurationLabel.TabIndex = 0;
            selectConfigurationLabel.Text = "Seleziona configurazione";
            selectConfigurationLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // selectConfigurationComboBox
            // 
            selectConfigurationComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            selectConfigurationComboBox.DropDownHoveredBackColor = Color.LightSeaGreen;
            selectConfigurationComboBox.DropDownHoveredForeColor = Color.Black;
            selectConfigurationComboBox.FlatStyle = FlatStyle.System;
            selectConfigurationComboBox.FormattingEnabled = true;
            selectConfigurationComboBox.Location = new Point(151, 4);
            selectConfigurationComboBox.Margin = new Padding(0);
            selectConfigurationComboBox.Name = "selectConfigurationComboBox";
            selectConfigurationComboBox.Size = new Size(133, 24);
            selectConfigurationComboBox.TabIndex = 0;
            selectConfigurationComboBox.TabStop = false;
            // 
            // stepFlowPanel
            // 
            stepFlowPanel.AutoScroll = true;
            stepFlowPanel.AutoSize = true;
            stepFlowPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            stepFlowPanel.Dock = DockStyle.Fill;
            stepFlowPanel.Location = new Point(10, 78);
            stepFlowPanel.Margin = new Padding(6, 5, 6, 6);
            stepFlowPanel.Name = "stepFlowPanel";
            stepFlowPanel.Size = new Size(752, 1);
            stepFlowPanel.TabIndex = 0;
            // 
            // bottomPanel
            // 
            bottomPanel.AutoScroll = true;
            bottomPanel.AutoSize = true;
            bottomPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            bottomPanel.BackColor = Color.Transparent;
            bottomPanel.Controls.Add(controlsPanel);
            bottomPanel.Dock = DockStyle.Fill;
            bottomPanel.Location = new Point(9, 89);
            bottomPanel.Margin = new Padding(5);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new Size(754, 443);
            bottomPanel.TabIndex = 3;
            // 
            // controlsPanel
            // 
            controlsPanel.AutoSize = true;
            controlsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            controlsPanel.BackColor = Color.Transparent;
            controlsPanel.ColumnCount = 1;
            controlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            controlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            controlsPanel.Dock = DockStyle.Top;
            controlsPanel.Location = new Point(0, 0);
            controlsPanel.Margin = new Padding(10);
            controlsPanel.Name = "controlsPanel";
            controlsPanel.RowCount = 1;
            controlsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            controlsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            controlsPanel.Size = new Size(754, 0);
            controlsPanel.TabIndex = 0;
            // 
            // SettingsStepForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(772, 541);
            Controls.Add(mainTable);
            Margin = new Padding(3, 2, 3, 2);
            Name = "SettingsStepForm";
            Text = "Settings";
            TopMost = true;
            mainTable.ResumeLayout(false);
            mainTable.PerformLayout();
            selectedConfigurationPanel.ResumeLayout(false);
            selectedConfigurationPanel.PerformLayout();
            selectConfigurationPanel.ResumeLayout(false);
            selectConfigurationPanel.PerformLayout();
            bottomPanel.ResumeLayout(false);
            bottomPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanelColorizable mainTable;
        private FlowLayoutPanel selectConfigurationPanel;
        private Label selectConfigurationLabel;
        private ComboBoxFilterable selectConfigurationComboBox;
        private FlowLayoutPanel stepFlowPanel;
        private Label selectedConfigurationNameLabel;
        private Panel bottomPanel;
        private TableLayoutPanelColorizable controlsPanel;
        private FlowLayoutPanel selectedConfigurationPanel;
    }
}