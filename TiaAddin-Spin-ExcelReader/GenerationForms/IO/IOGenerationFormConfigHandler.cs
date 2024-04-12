using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Generation.IO;

namespace TiaXmlReader.GenerationForms.IO
{
    public class IOGenerationFormConfigHandler
    {
        private readonly IOGenerationForm form;
        private readonly IOConfiguration config;
        private readonly DataGridView dataGridView;

        public IOGenerationFormConfigHandler(IOGenerationForm form, IOConfiguration config, DataGridView dataGridView)
        {
            this.form = form;
            this.config = config;
            this.dataGridView = dataGridView;
        }

        public void Init()
        {
            form.groupingTypeComboBox.SelectionChangeCommitted += (object sender, EventArgs args) =>
            {
                config.GroupingType = (IOGroupingTypeEnum) form.groupingTypeComboBox.SelectedValue;
                this.dataGridView.Refresh();
            };

            form.memoryTypeComboBox.SelectionChangeCommitted += (object sender, EventArgs args) =>
            {
                config.MemoryType = (IOMemoryTypeEnum)form.memoryTypeComboBox.SelectedValue;
                this.dataGridView.Refresh();
            };

            form.fcConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("FC");
                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Nome")
                    .TextBox(config.FCBlockName)
                    .TextChanged(str => config.FCBlockName = str));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Numero")
                    .TextBox(config.FCBlockNumber)
                    .NumericOnly()
                    .UIntChanged(num => config.FCBlockNumber = num));

                SetupConfigForm(form.fcConfigButton, configForm);
            };

            form.dbConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("DB Appoggi");
                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Nome")
                    .TextBox(config.DBName)
                    .TextChanged(str => config.DBName = str));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Numero")
                    .TextBox(config.DBNumber)
                    .NumericOnly()
                    .UIntChanged(num => config.DBNumber = num));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Prefisso In")
                    .TextBox(config.PrefixInputDB)
                    .TextChanged(str => config.PrefixInputDB = str));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Prefisso Out")
                    .TextBox(config.PrefixOutputDB)
                    .TextChanged(str => config.PrefixOutputDB = str));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Nome variabile default")
                    .TextBox(config.DefaultVariableName)
                    .TextChanged(str => config.DefaultVariableName = str));

                SetupConfigForm(form.dbConfigButton, configForm);
            };

            form.variableTableConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Tabella Appoggi");
                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Nome")
                    .TextBox(config.VariableTableName)
                    .TextChanged(str => config.VariableTableName = str));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Indirizzo Start")
                    .TextBox(config.VariableTableStartAddress)
                    .NumericOnly()
                    .UIntChanged(num => config.VariableTableStartAddress = num));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Nuova ogni n° bit")
                    .TextBox(config.VariableTableSplitEvery)
                    .NumericOnly()
                    .UIntChanged(num => config.VariableTableSplitEvery = num));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Prefisso In")
                    .TextBox(config.PrefixInputMerker)
                    .TextChanged(str => config.PrefixInputMerker = str));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Prefisso Out")
                    .TextBox(config.PrefixOutputMerker)
                    .TextChanged(str => config.PrefixOutputMerker = str));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Nome variabile default")
                    .TextBox(config.DefaultVariableName)
                    .TextChanged(str => config.DefaultVariableName = str));

                SetupConfigForm(form.variableTableConfigButton, configForm);
            };

            form.ioTableConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Tabella IN/OUT");
                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Nome")
                    .TextBox(config.IOTableName)
                    .TextChanged(str => config.IOTableName = str));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Nuova ogni n° bit")
                    .TextBox(config.IOTableSplitEvery)
                    .NumericOnly()
                    .UIntChanged(num => config.IOTableSplitEvery = num));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Nome tag default")
                    .TextBox(config.DefaultIoName)
                    .TextChanged(str => config.DefaultIoName = str));

                SetupConfigForm(form.ioTableConfigButton, configForm);
            };

            form.segmentNameConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Nomi segmenti generati");
                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Divisione per bit")
                    .TextBox(config.SegmentNameBitGrouping)
                    .TextChanged(str => config.SegmentNameBitGrouping = str));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Divisione per byte")
                    .TextBox(config.SegmentNameByteGrouping)
                    .TextChanged(str => config.SegmentNameByteGrouping = str));

                SetupConfigForm(form.segmentNameConfigButton, configForm);
            };
        }

        private void SetupConfigForm(Control button, ConfigForm configForm)
        {
            var loc = button.PointToScreen(Point.Empty);
            //loc.X += fcConfigButton.Width;
            loc.Y += button.Height;
            configForm.StartPosition = FormStartPosition.Manual;
            configForm.Location = loc;

            configForm.Init();
            configForm.ShowDialog(form);

            dataGridView.Refresh();
        }

    }
}
