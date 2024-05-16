using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Generation.IO.GenerationForm;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.Placeholders;
using TiaUtilities.Generation.Configuration.Utility;
using TiaXmlReader.Languages;

namespace TiaXmlReader.Generation.IO.GenerationForm
{
    public class IOGenerationFormConfigHandler(IOGenerationForm form, IOConfiguration config, GridHandler<IOConfiguration, IOData> gridHandler)
    {
        private readonly IOGenerationForm form = form;
        private readonly IOConfiguration config = config;
        private readonly GridHandler<IOConfiguration, IOData> gridHandler = gridHandler;

        public void Init()
        {
            form.groupingTypeComboBox.SelectedValue = config.GroupingType;
            form.groupingTypeComboBox.SelectionChangeCommitted += (sender, args) => config.GroupingType = (IOGroupingTypeEnum)form.groupingTypeComboBox.SelectedValue;

            form.memoryTypeComboBox.SelectedValue = config.MemoryType;
            form.memoryTypeComboBox.SelectionChangeCommitted += (sender, args) => config.MemoryType = (IOMemoryTypeEnum)form.memoryTypeComboBox.SelectedValue;

            form.fcConfigButton.Click += (sender, args) =>
            {
                var configForm = new ConfigForm(Localization.Get("IO_GEN_CONFIG_FC")) { ControlWidth = 150 };

                var mainGroup = configForm.Init();
                mainGroup.AddTextBox().LocalizedLabel("GENERICS_NAME")
                     .ControlText(config.FCBlockName)
                     .TextChanged(str => config.FCBlockName = str);

                mainGroup.AddTextBox().LocalizedLabel("GENERICS_NUMBER")
                     .ControlText(config.FCBlockNumber)
                     .UIntChanged(num => config.FCBlockNumber = num);

                SetupConfigForm(form.fcConfigButton, configForm);
            };

            form.dbConfigButton.Click += (sender, args) =>
            {
                var configForm = new ConfigForm(Localization.Get("IO_GEN_CONFIG_ALIAS_DB")) { ControlWidth = 250 };

                var mainGroup = configForm.Init();
                mainGroup.AddTextBox().LocalizedLabel("GENERICS_NAME", " > " + GenerationPlaceholders.IO.CONFIG_DB_NAME)
                     .ControlText(config.DBName)
                     .TextChanged(str => config.DBName = str);

                mainGroup.AddTextBox().LocalizedLabel("GENERICS_NUMBER", " > " + GenerationPlaceholders.IO.CONFIG_DB_NUMBER)
                     .ControlText(config.DBNumber)
                     .UIntChanged(num => config.DBNumber = num);

                mainGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_ALIAS_DB_IN_DEFAULT")
                     .ControlText(config.DefaultDBInputVariable)
                     .TextChanged(str => config.DefaultDBInputVariable = str);

                mainGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_ALIAS_DB_OUT_DEFAULT")
                     .ControlText(config.DefaultDBOutputVariable)
                     .TextChanged(str => config.DefaultDBOutputVariable = str);

                SetupConfigForm(form.dbConfigButton, configForm);
            };

            form.variableTableConfigButton.Click += (sender, args) =>
            {
                var configForm = new ConfigForm(Localization.Get("IO_GEN_CONFIG_ALIAS_TABLE")) { ControlWidth = 250 };

                var mainGroup = configForm.Init();
                mainGroup.AddTextBox().LocalizedLabel("GENERICS_NAME")
                     .ControlText(config.VariableTableName)
                     .TextChanged(str => config.VariableTableName = str);

                mainGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_ALIAS_TABLE_NEW_EVERY")
                     .ControlText(config.VariableTableSplitEvery)
                     .UIntChanged(num => config.VariableTableSplitEvery = num);

                var inputGroup = mainGroup.AddGroup();
                inputGroup.AddLabel().LocalizedLabel("IO_GEN_CONFIG_ALIAS_TABLE_INPUT_VARIABLE");

                inputGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_ALIAS_TABLE_VARIABLE_START_ADDRESS")
                     .ControlText(config.VariableTableInputStartAddress)
                     .UIntChanged(num => config.VariableTableInputStartAddress = num);

                inputGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_ALIAS_TABLE_VARIABLE_DEFAULT")
                     .ControlText(config.DefaultMerkerInputVariable)
                     .TextChanged(str => config.DefaultMerkerInputVariable = str);

                var outputGroup = mainGroup.AddGroup();
                outputGroup.AddLabel().LocalizedLabel("IO_GEN_CONFIG_ALIAS_TABLE_OUTPUT_VARIABLE");

                outputGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_ALIAS_TABLE_VARIABLE_START_ADDRESS")
                     .ControlText(config.VariableTableOutputStartAddress)
                     .UIntChanged(num => config.VariableTableOutputStartAddress = num);

                outputGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_ALIAS_TABLE_VARIABLE_DEFAULT")
                     .ControlText(config.DefaultMerkerOutputVariable)
                     .TextChanged(str => config.DefaultMerkerOutputVariable = str);

                SetupConfigForm(form.variableTableConfigButton, configForm);
            };

            form.ioTableConfigButton.Click += (sender, args) =>
            {
                var configForm = new ConfigForm(Localization.Get("IO_GEN_CONFIG_IO_TABLE")) { ControlWidth = 285 };

                var mainGroup = configForm.Init();
                mainGroup.AddTextBox().LocalizedLabel("GENERICS_NAME")
                     .ControlText(config.IOTableName)
                     .TextChanged(str => config.IOTableName = str);

                mainGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_IO_TABLE_NEW_EVERY")
                     .ControlText(config.IOTableSplitEvery)
                     .UIntChanged(num => config.IOTableSplitEvery = num);

                mainGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_IO_TABLE_DEFAULT_NAME")
                     .ControlText(config.DefaultIoName)
                     .TextChanged(str => config.DefaultIoName = str);

                SetupConfigForm(form.ioTableConfigButton, configForm);
            };

            form.segmentNameConfigButton.Click += (sender, args) =>
            {
                var configForm = new ConfigForm(Localization.Get("IO_GEN_CONFIG_SEGMENT")) { ControlWidth = 400 };

                var mainGroup = configForm.Init();
                mainGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_SEGMENT_BIT_DIVISION")
                     .ControlText(config.SegmentNameBitGrouping)
                     .TextChanged(str => config.SegmentNameBitGrouping = str);

                mainGroup.AddTextBox().LocalizedLabel("IO_GEN_CONFIG_SEGMENT_BYTE_DIVISION")
                     .ControlText(config.SegmentNameByteGrouping)
                     .TextChanged(str => config.SegmentNameByteGrouping = str);

                SetupConfigForm(form.segmentNameConfigButton, configForm);
            };
        }

        public void Translate()
        {
            form.memoryTypeLabel.Text = Localization.Get("IO_CONFIG_MEMORY_TYPE");
            form.groupingTypeLabel.Text = Localization.Get("IO_CONFIG_GROUPING_TYPE");

            form.fcConfigButton.Text = Localization.Get("IO_GEN_CONFIG_FC");
            form.dbConfigButton.Text = Localization.Get("IO_GEN_CONFIG_ALIAS_DB");
            form.variableTableConfigButton.Text = Localization.Get("IO_GEN_CONFIG_ALIAS_TABLE");
            form.ioTableConfigButton.Text = Localization.Get("IO_GEN_CONFIG_IO_TABLE");
            form.segmentNameConfigButton.Text = Localization.Get("IO_GEN_CONFIG_SEGMENT");
        }

        private void SetupConfigForm(Control button, ConfigForm configForm)
        {
            configForm.StartShowingAtControl(button);
            configForm.Init();
            configForm.Show(form);

            gridHandler.Refresh();
        }

    }
}
