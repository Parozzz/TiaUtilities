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
            selectConfigurationPanel = new FlowLayoutPanel();
            toggleModeButton = new Button();
            selectConfigurationLabel = new Label();
            selectConfigurationComboBox = new ComboBoxFilterable();
            searchLabel = new Label();
            searchTextBox = new TextBox();
            selectedConfigurationPanel = new FlowLayoutPanel();
            selectedConfigurationNameLabel = new Label();
            stepFlowPanel = new FlowLayoutPanel();
            bottomPanel = new Panel();
            controlsPanel = new TableLayoutPanelColorizable();
            searchPanel = new TableLayoutPanelColorizable();
            mainTable.SuspendLayout();
            selectConfigurationPanel.SuspendLayout();
            selectedConfigurationPanel.SuspendLayout();
            bottomPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainTable
            // 
            mainTable.AutoSize = true;
            mainTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainTable.ColumnCount = 1;
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainTable.Controls.Add(selectConfigurationPanel, 0, 0);
            mainTable.Controls.Add(selectedConfigurationPanel, 0, 1);
            mainTable.Controls.Add(stepFlowPanel, 0, 2);
            mainTable.Controls.Add(bottomPanel, 0, 3);
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
            // selectConfigurationPanel
            // 
            selectConfigurationPanel.AutoSize = true;
            selectConfigurationPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            selectConfigurationPanel.Controls.Add(toggleModeButton);
            selectConfigurationPanel.Controls.Add(selectConfigurationLabel);
            selectConfigurationPanel.Controls.Add(selectConfigurationComboBox);
            selectConfigurationPanel.Controls.Add(searchLabel);
            selectConfigurationPanel.Controls.Add(searchTextBox);
            selectConfigurationPanel.Dock = DockStyle.Fill;
            selectConfigurationPanel.Location = new Point(5, 5);
            selectConfigurationPanel.Margin = new Padding(0);
            selectConfigurationPanel.Name = "selectConfigurationPanel";
            selectConfigurationPanel.Padding = new Padding(5);
            selectConfigurationPanel.Size = new Size(872, 40);
            selectConfigurationPanel.TabIndex = 0;
            // 
            // toggleModeButton
            // 
            toggleModeButton.AutoSize = true;
            toggleModeButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            toggleModeButton.FlatAppearance.BorderSize = 0;
            toggleModeButton.FlatStyle = FlatStyle.Flat;
            toggleModeButton.Location = new Point(5, 5);
            toggleModeButton.Margin = new Padding(0, 0, 10, 0);
            toggleModeButton.Name = "toggleModeButton";
            toggleModeButton.Size = new Size(46, 30);
            toggleModeButton.TabIndex = 3;
            toggleModeButton.Text = "IMG";
            toggleModeButton.UseVisualStyleBackColor = true;
            // 
            // selectConfigurationLabel
            // 
            selectConfigurationLabel.Anchor = AnchorStyles.None;
            selectConfigurationLabel.AutoSize = true;
            selectConfigurationLabel.Location = new Point(61, 10);
            selectConfigurationLabel.Margin = new Padding(0, 0, 10, 0);
            selectConfigurationLabel.Name = "selectConfigurationLabel";
            selectConfigurationLabel.Size = new Size(176, 20);
            selectConfigurationLabel.TabIndex = 0;
            selectConfigurationLabel.Text = "Seleziona configurazione";
            selectConfigurationLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // selectConfigurationComboBox
            // 
            selectConfigurationComboBox.Anchor = AnchorStyles.None;
            selectConfigurationComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            selectConfigurationComboBox.DropDownHoveredBackColor = Color.LightSeaGreen;
            selectConfigurationComboBox.DropDownHoveredForeColor = Color.Black;
            selectConfigurationComboBox.FlatStyle = FlatStyle.System;
            selectConfigurationComboBox.FormattingEnabled = true;
            selectConfigurationComboBox.Location = new Point(247, 6);
            selectConfigurationComboBox.Margin = new Padding(0);
            selectConfigurationComboBox.Name = "selectConfigurationComboBox";
            selectConfigurationComboBox.Size = new Size(151, 28);
            selectConfigurationComboBox.TabIndex = 0;
            selectConfigurationComboBox.TabStop = false;
            // 
            // searchLabel
            // 
            searchLabel.Anchor = AnchorStyles.None;
            searchLabel.AutoSize = true;
            searchLabel.Location = new Point(398, 10);
            searchLabel.Margin = new Padding(0, 0, 10, 0);
            searchLabel.Name = "searchLabel";
            searchLabel.Size = new Size(87, 20);
            searchLabel.TabIndex = 1;
            searchLabel.Text = "Cerca valori";
            // 
            // searchTextBox
            // 
            searchTextBox.Anchor = AnchorStyles.None;
            searchTextBox.Location = new Point(495, 6);
            searchTextBox.Margin = new Padding(0);
            searchTextBox.Name = "searchTextBox";
            searchTextBox.Size = new Size(250, 27);
            searchTextBox.TabIndex = 2;
            // 
            // selectedConfigurationPanel
            // 
            selectedConfigurationPanel.Anchor = AnchorStyles.None;
            selectedConfigurationPanel.AutoSize = true;
            selectedConfigurationPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            selectedConfigurationPanel.Controls.Add(selectedConfigurationNameLabel);
            selectedConfigurationPanel.Location = new Point(321, 49);
            selectedConfigurationPanel.Margin = new Padding(3, 4, 3, 4);
            selectedConfigurationPanel.Name = "selectedConfigurationPanel";
            selectedConfigurationPanel.Size = new Size(240, 42);
            selectedConfigurationPanel.TabIndex = 1;
            // 
            // selectedConfigurationNameLabel
            // 
            selectedConfigurationNameLabel.Anchor = AnchorStyles.None;
            selectedConfigurationNameLabel.AutoSize = true;
            selectedConfigurationNameLabel.Location = new Point(0, 11);
            selectedConfigurationNameLabel.Margin = new Padding(0, 11, 0, 11);
            selectedConfigurationNameLabel.Name = "selectedConfigurationNameLabel";
            selectedConfigurationNameLabel.Size = new Size(240, 20);
            selectedConfigurationNameLabel.TabIndex = 2;
            selectedConfigurationNameLabel.Text = "SELECTED CONFIGURATION NAME";
            selectedConfigurationNameLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // stepFlowPanel
            // 
            stepFlowPanel.AutoScroll = true;
            stepFlowPanel.AutoSize = true;
            stepFlowPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            stepFlowPanel.Dock = DockStyle.Fill;
            stepFlowPanel.Location = new Point(12, 102);
            stepFlowPanel.Margin = new Padding(7, 7, 7, 8);
            stepFlowPanel.Name = "stepFlowPanel";
            stepFlowPanel.Size = new Size(858, 1);
            stepFlowPanel.TabIndex = 0;
            // 
            // bottomPanel
            // 
            bottomPanel.AutoScroll = true;
            bottomPanel.AutoSize = true;
            bottomPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            bottomPanel.BackColor = Color.Transparent;
            bottomPanel.Controls.Add(controlsPanel);
            bottomPanel.Controls.Add(searchPanel);
            bottomPanel.Dock = DockStyle.Fill;
            bottomPanel.Location = new Point(11, 117);
            bottomPanel.Margin = new Padding(6, 7, 6, 7);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new Size(860, 592);
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
            controlsPanel.Margin = new Padding(11, 13, 11, 13);
            controlsPanel.Name = "controlsPanel";
            controlsPanel.RowCount = 1;
            controlsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            controlsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            controlsPanel.Size = new Size(860, 0);
            controlsPanel.TabIndex = 0;
            // 
            // searchPanel
            // 
            searchPanel.AutoSize = true;
            searchPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            searchPanel.BackColor = Color.Transparent;
            searchPanel.ColumnCount = 1;
            searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Location = new Point(0, 0);
            searchPanel.Margin = new Padding(11, 13, 11, 13);
            searchPanel.Name = "searchPanel";
            searchPanel.RowCount = 1;
            searchPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            searchPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            searchPanel.Size = new Size(860, 0);
            searchPanel.TabIndex = 1;
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
            selectConfigurationPanel.ResumeLayout(false);
            selectConfigurationPanel.PerformLayout();
            selectedConfigurationPanel.ResumeLayout(false);
            selectedConfigurationPanel.PerformLayout();
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
        private Label searchLabel;
        private TextBox searchTextBox;
        private Button toggleModeButton;
        private TableLayoutPanelColorizable searchPanel;
    }
}