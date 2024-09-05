using TiaXmlReader.Languages;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.Configuration;
using TiaUtilities.Generation.Configuration.Utility;
using TiaXmlReader.Generation.Placeholders;

namespace TiaUtilities.Generation.GenForms.IO
{
    public partial class IOGenConfigTopControl : UserControl
    {
        public delegate void MemoryTypeChangedEventHandler(object? sender, MemoryTypeChangedEventArgs eventArgs);
        public record MemoryTypeChangedEventArgs(IOMemoryTypeEnum MemoryType);

        public IOGenConfigTopControl()
        {//This is a subordinated control. Init is called in the class that add this.
            InitializeComponent();

            this.Dock = DockStyle.Fill;

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

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().LocalizedLabel("GENERICS_NAME", " > " + GenerationPlaceholders.IO.CONFIG_DB_NAME)
                         .ControlText(mainConfig.DBName)
                         .TextChanged(str => mainConfig.DBName = str);

                    mainGroup.AddTextBox().LocalizedLabel("GENERICS_NUMBER", " > " + GenerationPlaceholders.IO.CONFIG_DB_NUMBER)
                         .ControlText(mainConfig.DBNumber)
                         .UIntChanged(num => mainConfig.DBNumber = num);

                    mainGroup.AddCheckBox().LocalizedLabel("IO_GEN_CONFIG_ALIAS_DB_GENERATED_DEFINED_VARIABLES").ControlNoAdapt()
                         .Value(mainConfig.GenerateDefinedVariableAnyway)
                         .CheckedChanged(b => mainConfig.GenerateDefinedVariableAnyway = b);

                    mainGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_ALIAS_DB_IN_DEFAULT")
                         .ControlText(mainConfig.DefaultDBInputVariable)
                         .TextChanged(str => mainConfig.DefaultDBInputVariable = str);

                    mainGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_ALIAS_DB_OUT_DEFAULT")
                         .ControlText(mainConfig.DefaultDBOutputVariable)
                         .TextChanged(str => mainConfig.DefaultDBOutputVariable = str);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = this.variableTableConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text) { ControlWidth = 250 };

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().LocalizedLabel("GENERICS_NAME")
                         .ControlText(mainConfig.VariableTableName)
                         .TextChanged(str => mainConfig.VariableTableName = str);

                    mainGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_ALIAS_TABLE_NEW_EVERY")
                         .ControlText(mainConfig.VariableTableSplitEvery)
                         .UIntChanged(num => mainConfig.VariableTableSplitEvery = num);

                    var inputGroup = mainGroup.AddGroup();
                    inputGroup.AddLabel().LocalizedLabel("IO_GEN_CONFIG_ALIAS_TABLE_INPUT_VARIABLE");

                    inputGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_ALIAS_TABLE_VARIABLE_START_ADDRESS")
                         .ControlText(mainConfig.VariableTableInputStartAddress)
                         .UIntChanged(num => mainConfig.VariableTableInputStartAddress = num);

                    inputGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_ALIAS_TABLE_VARIABLE_DEFAULT")
                         .ControlText(mainConfig.DefaultMerkerInputVariable)
                         .TextChanged(str => mainConfig.DefaultMerkerInputVariable = str);

                    var outputGroup = mainGroup.AddGroup();
                    outputGroup.AddLabel().LocalizedLabel("IO_GEN_CONFIG_ALIAS_TABLE_OUTPUT_VARIABLE");

                    outputGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_ALIAS_TABLE_VARIABLE_START_ADDRESS")
                         .ControlText(mainConfig.VariableTableOutputStartAddress)
                         .UIntChanged(num => mainConfig.VariableTableOutputStartAddress = num);

                    outputGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_ALIAS_TABLE_VARIABLE_DEFAULT")
                         .ControlText(mainConfig.DefaultMerkerOutputVariable)
                         .TextChanged(str => mainConfig.DefaultMerkerOutputVariable = str);

                    SetupConfigForm(button, configForm);
                };
            }

            {
                var button = this.ioTableConfigButton;
                button.Click += (sender, args) =>
                {
                    var configForm = new ConfigForm(button.Text) { ControlWidth = 285 };

                    var mainGroup = configForm.Init();
                    mainGroup.AddTextBox().LocalizedLabel("GENERICS_NAME")
                         .ControlText(mainConfig.IOTableName)
                         .TextChanged(str => mainConfig.IOTableName = str);

                    mainGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_IO_TABLE_NEW_EVERY")
                         .ControlText(mainConfig.IOTableSplitEvery)
                         .UIntChanged(num => mainConfig.IOTableSplitEvery = num);

                    mainGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_IO_TABLE_DEFAULT_NAME")
                         .ControlText(mainConfig.DefaultIoName)
                         .TextChanged(str => mainConfig.DefaultIoName = str);

                    SetupConfigForm(button, configForm);
                };
            }

            mainConfig.Subscribe(() => mainConfig.MemoryType, this.UpdateConfigPanel);
            this.UpdateConfigPanel(mainConfig.MemoryType);
        }

        private void Translate()
        {
            this.memoryTypeLabel.Text = Localization.Get("IO_CONFIG_MEMORY_TYPE");
            this.groupingTypeLabel.Text = Localization.Get("IO_CONFIG_GROUPING_TYPE");

            this.dbConfigButton.Text = Localization.Get("IO_GEN_CONFIG_ALIAS_DB");
            this.variableTableConfigButton.Text = Localization.Get("IO_GEN_CONFIG_ALIAS_TABLE");
            this.ioTableConfigButton.Text = Localization.Get("IO_GEN_CONFIG_IO_TABLE");
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
