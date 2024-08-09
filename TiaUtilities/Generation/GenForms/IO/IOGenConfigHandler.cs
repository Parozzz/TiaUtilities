using TiaXmlReader.Generation.IO;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.Placeholders;
using TiaUtilities.Generation.Configuration.Utility;
using TiaXmlReader.Languages;
using TiaUtilities.Generation.GenForms.IO;

namespace TiaUtilities.Generation.GenForms.IO
{
    public class IOGenConfigHandler(IOGenConfigTopControl configControl, IOMainConfiguration mainConfig)
    {
        public void Init()
        {
            {
                var comboBox = configControl.groupingTypeComboBox;
                comboBox.SelectedValue = mainConfig.GroupingType;
                comboBox.SelectionChangeCommitted += (sender, args) => mainConfig.GroupingType = (IOGroupingTypeEnum)(comboBox.SelectedValue ?? default(IOGroupingTypeEnum));
            }

            {
                var comboBox = configControl.memoryTypeComboBox;
                comboBox.SelectedValue = mainConfig.MemoryType;
                comboBox.SelectionChangeCommitted += (sender, args) => mainConfig.MemoryType = (IOMemoryTypeEnum)(comboBox.SelectedValue ?? default(IOMemoryTypeEnum));
            }

            {
                var button = configControl.dbConfigButton;
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
                var button = configControl.variableTableConfigButton;
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
                var button = configControl.ioTableConfigButton;
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

        }


        private static void SetupConfigForm(Control button, ConfigForm configForm)
        {
            configForm.StartShowingAtControl(button);
            //configForm.Init();
            configForm.Show(button.FindForm());
        }
    }
}
