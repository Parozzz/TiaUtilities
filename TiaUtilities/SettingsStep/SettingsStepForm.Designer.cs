using TiaUtilities.CustomControls;
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
            mainTable = new TableLayoutPanelNoScrollbarsColorizable();
            selectedConfigurationNameLabel = new Label();
            selectContainerFlowPanel = new FlowLayoutPanel();
            selectConfigurationLabel = new Label();
            selectConfigurationComboBox = new ComboBoxFilterable();
            stepFlowPanel = new FlowLayoutPanel();
            mainTable.SuspendLayout();
            selectContainerFlowPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainTable
            // 
            mainTable.AutoSize = true;
            mainTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainTable.ColumnCount = 1;
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainTable.Controls.Add(selectedConfigurationNameLabel, 0, 1);
            mainTable.Controls.Add(selectContainerFlowPanel, 0, 0);
            mainTable.Controls.Add(stepFlowPanel, 0, 2);
            mainTable.Dock = DockStyle.Fill;
            mainTable.Location = new Point(0, 0);
            mainTable.Margin = new Padding(0);
            mainTable.Name = "mainTable";
            mainTable.Padding = new Padding(5);
            mainTable.RowCount = 4;
            mainTable.RowStyles.Add(new RowStyle());
            mainTable.RowStyles.Add(new RowStyle());
            mainTable.RowStyles.Add(new RowStyle());
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            mainTable.Size = new Size(882, 721);
            mainTable.TabIndex = 0;
            // 
            // selectedConfigurationNameLabel
            // 
            selectedConfigurationNameLabel.AutoSize = true;
            selectedConfigurationNameLabel.Dock = DockStyle.Fill;
            selectedConfigurationNameLabel.Location = new Point(5, 53);
            selectedConfigurationNameLabel.Margin = new Padding(0, 10, 0, 10);
            selectedConfigurationNameLabel.Name = "selectedConfigurationNameLabel";
            selectedConfigurationNameLabel.Size = new Size(872, 20);
            selectedConfigurationNameLabel.TabIndex = 2;
            selectedConfigurationNameLabel.Text = "label1";
            selectedConfigurationNameLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // selectContainerFlowPanel
            // 
            selectContainerFlowPanel.AutoSize = true;
            selectContainerFlowPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            selectContainerFlowPanel.Controls.Add(selectConfigurationLabel);
            selectContainerFlowPanel.Controls.Add(selectConfigurationComboBox);
            selectContainerFlowPanel.Dock = DockStyle.Fill;
            selectContainerFlowPanel.Location = new Point(5, 5);
            selectContainerFlowPanel.Margin = new Padding(0);
            selectContainerFlowPanel.Name = "selectContainerFlowPanel";
            selectContainerFlowPanel.Padding = new Padding(5);
            selectContainerFlowPanel.Size = new Size(872, 38);
            selectContainerFlowPanel.TabIndex = 0;
            // 
            // selectConfigurationLabel
            // 
            selectConfigurationLabel.AutoSize = true;
            selectConfigurationLabel.Dock = DockStyle.Left;
            selectConfigurationLabel.Location = new Point(5, 5);
            selectConfigurationLabel.Margin = new Padding(0, 0, 10, 0);
            selectConfigurationLabel.Name = "selectConfigurationLabel";
            selectConfigurationLabel.Size = new Size(176, 28);
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
            selectConfigurationComboBox.Location = new Point(191, 5);
            selectConfigurationComboBox.Margin = new Padding(0);
            selectConfigurationComboBox.Name = "selectConfigurationComboBox";
            selectConfigurationComboBox.Size = new Size(151, 28);
            selectConfigurationComboBox.TabIndex = 0;
            selectConfigurationComboBox.TabStop = false;
            // 
            // stepFlowPanel
            // 
            stepFlowPanel.AutoScroll = true;
            stepFlowPanel.AutoSize = true;
            stepFlowPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            stepFlowPanel.Dock = DockStyle.Fill;
            stepFlowPanel.Location = new Point(12, 90);
            stepFlowPanel.Margin = new Padding(7, 7, 7, 8);
            stepFlowPanel.Name = "stepFlowPanel";
            stepFlowPanel.Size = new Size(858, 1);
            stepFlowPanel.TabIndex = 0;
            // 
            // SettingsStepForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(882, 721);
            Controls.Add(mainTable);
            Name = "SettingsStepForm";
            Text = "Settings";
            TopMost = true;
            mainTable.ResumeLayout(false);
            mainTable.PerformLayout();
            selectContainerFlowPanel.ResumeLayout(false);
            selectContainerFlowPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanelNoScrollbarsColorizable mainTable;
        private FlowLayoutPanel selectContainerFlowPanel;
        private Label selectConfigurationLabel;
        private ComboBoxFilterable selectConfigurationComboBox;
        private FlowLayoutPanel stepFlowPanel;
        private Label selectedConfigurationNameLabel;
    }
}