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
                configForm.AddTextBoxLine("Nome")
                    .ControlText(config.FCBlockName)
                    .TextChanged(str => config.FCBlockName = str);

                configForm.AddTextBoxLine("Numero")
                    .ControlText(config.FCBlockNumber)
                    .UIntChanged(num => config.FCBlockNumber = num);

                SetupConfigForm(form.fcConfigButton, configForm);
            };

            form.dbConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("DB Appoggi");
                configForm.AddTextBoxLine("Nome")
                    .ControlText(config.DBName)
                    .TextChanged(str => config.DBName = str);

                configForm.AddTextBoxLine("Numero")
                    .ControlText(config.DBNumber)
                    .UIntChanged(num => config.DBNumber = num);

                configForm.AddTextBoxLine("Prefisso In")
                    .ControlText(config.PrefixInputDB)
                    .TextChanged(str => config.PrefixInputDB = str);

                configForm.AddTextBoxLine("Prefisso Out")
                    .ControlText(config.PrefixOutputDB)
                    .TextChanged(str => config.PrefixOutputDB = str);

                configForm.AddTextBoxLine("Nome variabile default")
                    .ControlText(config.DefaultVariableName)
                    .TextChanged(str => config.DefaultVariableName = str);

                SetupConfigForm(form.dbConfigButton, configForm);
            };

            form.variableTableConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Tabella Appoggi");
                configForm.AddTextBoxLine("Nome")
                    .ControlText(config.VariableTableName)
                    .TextChanged(str => config.VariableTableName = str);

                configForm.AddTextBoxLine("Indirizzo Start")
                    .ControlText(config.VariableTableStartAddress)
                    .UIntChanged(num => config.VariableTableStartAddress = num);

                configForm.AddTextBoxLine("Nuova ogni n° bit")
                    .ControlText(config.VariableTableSplitEvery)
                    .UIntChanged(num => config.VariableTableSplitEvery = num);

                configForm.AddTextBoxLine("Prefisso In")
                    .ControlText(config.PrefixInputMerker)
                    .TextChanged(str => config.PrefixInputMerker = str);

                configForm.AddTextBoxLine("Prefisso Out")
                    .ControlText(config.PrefixOutputMerker)
                    .TextChanged(str => config.PrefixOutputMerker = str);

                configForm.AddTextBoxLine("Nome variabile default")
                    .ControlText(config.DefaultVariableName)
                    .TextChanged(str => config.DefaultVariableName = str);

                SetupConfigForm(form.variableTableConfigButton, configForm);
            };

            form.ioTableConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Tabella IN/OUT");
                configForm.AddTextBoxLine("Nome")
                    .ControlText(config.IOTableName)
                    .TextChanged(str => config.IOTableName = str);

                configForm.AddTextBoxLine("Nuova ogni n° bit")
                    .ControlText(config.IOTableSplitEvery)
                    .UIntChanged(num => config.IOTableSplitEvery = num);

                configForm.AddTextBoxLine("Nome tag default")
                    .ControlText(config.DefaultIoName)
                    .TextChanged(str => config.DefaultIoName = str);

                SetupConfigForm(form.ioTableConfigButton, configForm);
            };

            form.segmentNameConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Nomi segmenti generati");
                configForm.AddTextBoxLine("Divisione per bit")
                    .ControlText(config.SegmentNameBitGrouping)
                    .TextChanged(str => config.SegmentNameBitGrouping = str);

                configForm.AddTextBoxLine("Divisione per byte")
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
