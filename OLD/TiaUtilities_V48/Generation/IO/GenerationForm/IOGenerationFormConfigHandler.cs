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

namespace TiaXmlReader.Generation.IO.GenerationForm
{
    public class IOGenerationFormConfigHandler
    {
        private readonly IOGenerationForm form;
        private readonly IOConfiguration config;
        private readonly GridHandler<IOConfiguration, IOData> gridHandler;

        public IOGenerationFormConfigHandler(IOGenerationForm form, IOConfiguration config, GridHandler<IOConfiguration, IOData> gridHandler)
        {
            this.form = form;
            this.config = config;
            this.gridHandler = gridHandler;
        }

        public void Init()
        {
            form.groupingTypeComboBox.SelectedValue = config.GroupingType;
            form.groupingTypeComboBox.SelectionChangeCommitted += (sender, args) => config.GroupingType = (IOGroupingTypeEnum)form.groupingTypeComboBox.SelectedValue;

            form.memoryTypeComboBox.SelectedValue = config.MemoryType;
            form.memoryTypeComboBox.SelectionChangeCommitted += (sender, args) => config.MemoryType = (IOMemoryTypeEnum)form.memoryTypeComboBox.SelectedValue;

            form.fcConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("FC");

                var mainGroup = configForm.Init();
                mainGroup.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Nome")
                     .ControlText(config.FCBlockName)
                     .TextChanged(str => config.FCBlockName = str);

                mainGroup.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Numero")
                     .ControlText(config.FCBlockNumber)
                     .UIntChanged(num => config.FCBlockNumber = num);

                SetupConfigForm(form.fcConfigButton, configForm);
            };

            form.dbConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("DB Appoggi");

                var mainGroup = configForm.Init();
                mainGroup.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Nome > " + GenerationPlaceholders.IO.CONFIG_DB_NAME)
                     .ControlText(config.DBName)
                     .TextChanged(str => config.DBName = str);

                mainGroup.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Numero > " + GenerationPlaceholders.IO.CONFIG_DB_NUMBER)
                     .ControlText(config.DBNumber)
                     .UIntChanged(num => config.DBNumber = num);

                mainGroup.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Valore Default Input")
                     .ControlText(config.DefaultDBInputVariable)
                     .TextChanged(str => config.DefaultDBInputVariable = str);

                mainGroup.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Valore Default Output")
                     .ControlText(config.DefaultDBOutputVariable)
                     .TextChanged(str => config.DefaultDBOutputVariable = str);

                SetupConfigForm(form.dbConfigButton, configForm);
            };

            form.variableTableConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Tabella Appoggi");

                var mainGroup = configForm.Init();
                mainGroup.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Nome")
                     .ControlText(config.VariableTableName)
                     .TextChanged(str => config.VariableTableName = str);

                mainGroup.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Nuova ogni n° bit")
                     .ControlText(config.VariableTableSplitEvery)
                     .UIntChanged(num => config.VariableTableSplitEvery = num);

                var inputGroup = mainGroup.AddGroup();
                inputGroup.AddLine(ConfigFormLineTypes.LABEL).LabelText("Variabile Input");

                inputGroup.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Indirizzo Start")
                     .ControlText(config.VariableTableInputStartAddress)
                     .UIntChanged(num => config.VariableTableInputStartAddress = num);

                inputGroup.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Valore Default")
                     .ControlText(config.DefaultMerkerInputVariable)
                     .TextChanged(str => config.DefaultMerkerInputVariable = str);

                var outputGroup = mainGroup.AddGroup();
                outputGroup.AddLine(ConfigFormLineTypes.LABEL).LabelText("Variabile Output");

                outputGroup.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Indirizzo Start")
                     .ControlText(config.VariableTableOutputStartAddress)
                     .UIntChanged(num => config.VariableTableOutputStartAddress = num);

                outputGroup.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Valore Default")
                     .ControlText(config.DefaultMerkerOutputVariable)
                     .TextChanged(str => config.DefaultMerkerOutputVariable = str);

                SetupConfigForm(form.variableTableConfigButton, configForm);
            };

            form.ioTableConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Tabella IN/OUT");

                var mainGroup = configForm.Init();
                mainGroup.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Nome")
                     .ControlText(config.IOTableName)
                     .TextChanged(str => config.IOTableName = str);

                mainGroup.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Nuova ogni n° bit")
                     .ControlText(config.IOTableSplitEvery)
                     .UIntChanged(num => config.IOTableSplitEvery = num);

                mainGroup.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Nome tag default")
                     .ControlText(config.DefaultIoName)
                     .TextChanged(str => config.DefaultIoName = str);

                SetupConfigForm(form.ioTableConfigButton, configForm);
            };

            form.segmentNameConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Nomi segmenti generati");

                var mainGroup = configForm.Init();
                mainGroup.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Divisione per bit")
                     .ControlText(config.SegmentNameBitGrouping)
                     .TextChanged(str => config.SegmentNameBitGrouping = str);

                mainGroup.AddLine(ConfigFormLineTypes.TEXT_BOX).LabelText("Divisione per byte")
                     .ControlText(config.SegmentNameByteGrouping)
                     .TextChanged(str => config.SegmentNameByteGrouping = str);

                SetupConfigForm(form.segmentNameConfigButton, configForm);
            };
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
