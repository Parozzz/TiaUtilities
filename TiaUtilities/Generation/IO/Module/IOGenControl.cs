﻿using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Languages;
using TiaUtilities.Generation.Configuration;
using TiaUtilities.Generation.Placeholders;

namespace TiaUtilities.Generation.IO.Module
{
    public partial class IOGenControl : UserControl
    {
        private readonly DataGridView suggestionDataGridView;
        public IOGenControl(DataGridView suggestionDataGridView)
        {
            //This is a subordinated control. Init is called in the class that add this.
            this.suggestionDataGridView = suggestionDataGridView;

            InitializeComponent();

            this.gridsTabControl.RequireConfirmationBeforeClosing = true;

            this.AutoSize = true; //AutoSize set here otherwise while doing the UI, everything will be shrinked to minimun (So useless)
            this.Dock = DockStyle.Fill;

            this.mainSplitContainer.Panel1.Controls.Add(this.suggestionDataGridView);

            GenUtils.FillComboBoxWithEnumTranslation(this.memoryTypeComboBox, typeof(IOMemoryTypeEnum));
            GenUtils.FillComboBoxWithEnumTranslation(this.groupingTypeComboBox, typeof(IOGroupingTypeEnum));

            Translate();
        }

        public void BindConfig(IOMainConfiguration mainConfig)
        {
            {
                var comboBox = this.groupingTypeComboBox;
                comboBox.SelectedValue = mainConfig.GroupingType;
                comboBox.SelectionChangeCommitted += (sender, args) => mainConfig.GroupingType = (IOGroupingTypeEnum)(comboBox.SelectedValue ?? default(IOGroupingTypeEnum));
            }

            {
                var comboBox = this.memoryTypeComboBox;
                comboBox.SelectedValue = mainConfig.MemoryType;
                comboBox.SelectionChangeCommitted += (sender, args) => mainConfig.MemoryType = (IOMemoryTypeEnum)(comboBox.SelectedValue ?? default(IOMemoryTypeEnum));
            }

            {
                var button = this.dbConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text) { ControlWidth = 250 };
                    configForm.SetConfiguration(mainConfig, MainForm.Settings.PresetIOMainConfiguration);

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().Label($"{Locale.GENERICS_NAME} > {GenPlaceholders.IO.CONFIG_DB_NAME}").BindText(() => mainConfig.DBName);
                    mainGroup.AddTextBox().Label($"{Locale.GENERICS_NUMBER} > {GenPlaceholders.IO.CONFIG_DB_NUMBER}").BindUInt(() => mainConfig.DBNumber);
                    mainGroup.AddCheckBox().Label(Locale.IO_GEN_CONFIG_ALIAS_DB_GENERATED_DEFINED_VARIABLES).BindChecked(() => mainConfig.GenerateDefinedVariableAnyway).ControlNoAdapt();
                    mainGroup.AddTextBox().Label(Locale.IO_GEN_CONFIG_ALIAS_DB_IN_DEFAULT).BindText(() => mainConfig.DefaultDBInputVariable);
                    mainGroup.AddTextBox().Label(Locale.IO_GEN_CONFIG_ALIAS_DB_OUT_DEFAULT).BindText(() => mainConfig.DefaultDBOutputVariable);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = this.ioTableConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text) { ControlWidth = 285 };
                    configForm.SetConfiguration(mainConfig, MainForm.Settings.PresetIOMainConfiguration);

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().Label(Locale.GENERICS_NAME).BindText(() => mainConfig.IOTableName);
                    mainGroup.AddTextBox().Label(Locale.IO_GEN_CONFIG_IO_TABLE_NEW_EVERY).BindUInt(() => mainConfig.IOTableSplitEvery);
                    mainGroup.AddTextBox().Label(Locale.IO_GEN_CONFIG_IO_TABLE_DEFAULT_NAME).BindText(() => mainConfig.DefaultIoName);
                    SetupConfigForm(button, configForm);
                };
            }


            {
                var button = this.variableTableConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text) { ControlWidth = 250 };
                    configForm.SetConfiguration(mainConfig, MainForm.Settings.PresetIOMainConfiguration);

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().Label(Locale.GENERICS_NAME).BindText(() => mainConfig.VariableTableName);
                    mainGroup.AddTextBox().Label(Locale.IO_GEN_CONFIG_ALIAS_TABLE_NEW_EVERY).BindUInt(() => mainConfig.VariableTableSplitEvery);

                    var inputGroup = mainGroup.AddGroup();
                    inputGroup.AddLabel().Label(Locale.IO_GEN_CONFIG_ALIAS_TABLE_INPUT_VARIABLE);
                    inputGroup.AddTextBox().Label(Locale.IO_GEN_CONFIG_ALIAS_TABLE_VARIABLE_START_ADDRESS).BindUInt(() => mainConfig.VariableTableInputStartAddress);
                    inputGroup.AddTextBox().Label(Locale.IO_GEN_CONFIG_ALIAS_TABLE_VARIABLE_DEFAULT).BindText(() => mainConfig.DefaultMerkerInputVariable);

                    var outputGroup = mainGroup.AddGroup();
                    outputGroup.AddLabel().Label(Locale.IO_GEN_CONFIG_ALIAS_TABLE_OUTPUT_VARIABLE);
                    outputGroup.AddTextBox().Label(Locale.IO_GEN_CONFIG_ALIAS_TABLE_VARIABLE_START_ADDRESS).BindUInt(() => mainConfig.VariableTableOutputStartAddress);
                    outputGroup.AddTextBox().Label(Locale.IO_GEN_CONFIG_ALIAS_TABLE_VARIABLE_DEFAULT).BindText(() => mainConfig.DefaultMerkerOutputVariable);

                    SetupConfigForm(button, configForm);
                };
            }

            mainConfig.Subscribe(() => mainConfig.MemoryType, this.UpdateConfigPanel);
            this.UpdateConfigPanel(mainConfig.MemoryType);
        }

        private void Translate()
        {
            this.memoryTypeLabel.Text = Locale.IO_CONFIG_MEMORY_TYPE;
            this.groupingTypeLabel.Text = Locale.IO_CONFIG_GROUPING_TYPE;

            this.ioTableConfigButton.Text = Locale.IO_GEN_CONFIG_IO_TABLE;
            this.dbConfigButton.Text = Locale.IO_GEN_CONFIG_ALIAS_DB;
            this.variableTableConfigButton.Text = Locale.IO_GEN_CONFIG_ALIAS_TABLE;
        }

        private void UpdateConfigPanel(IOMemoryTypeEnum memoryType)
        {
            this.configButtonPanel.SuspendLayout();

            this.configButtonPanel.Controls.Remove(dbConfigButton);
            this.configButtonPanel.Controls.Remove(variableTableConfigButton);
            if (memoryType == IOMemoryTypeEnum.DB)
            {
                this.configButtonPanel.Controls.Add(dbConfigButton);
            }
            else if (memoryType == IOMemoryTypeEnum.MERKER)
            {
                this.configButtonPanel.Controls.Add(variableTableConfigButton);
            }

            this.configButtonPanel.ResumeLayout();
        }

        private static void SetupConfigForm(Control button, ConfigForm configForm)
        {
            configForm.StartShowingAtControl(button);
            //configForm.Init();
            configForm.Show(button.FindForm());
        }
    }
}
