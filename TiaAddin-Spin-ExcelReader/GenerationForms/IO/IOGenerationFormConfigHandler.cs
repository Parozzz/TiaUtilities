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

        public IOGenerationFormConfigHandler(IOGenerationForm form, IOConfiguration config)
        {
            this.form = form;
            this.config = config;
        }

        public void Init()
        {
            form.fcConfigButton.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("FC");
                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Nome")
                    .TextBox(config.FCBlockName)
                    .TextChanged(str => config.FCBlockName = str));

                configForm.AddConfigLine(new ConfigFormLine()
                    .Title("Numero")
                    .TextBox("" + config.FCBlockNumber)
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
        }

        private void RegisterTextChanged(TextBox textBox, Action<string> handler)
        {
            textBox.TextChanged += (object sender, EventArgs args) =>
            {
                handler.Invoke(textBox.Text);
            };
        }

        private void RegisterTextChanged(ComboBox comboBox, Action<string> handler)
        {
            comboBox.TextChanged += (object sender, EventArgs args) =>
            {
                handler.Invoke(comboBox.Text);
            };
        }

        private void RegisterUIntChanged(ComboBox comboBox, Action<uint> handler)
        {
            comboBox.KeyPress += (object sender, KeyPressEventArgs args) =>
            {
                args.Handled = Char.IsLetter(args.KeyChar);
            };

            UIntChanged(comboBox.Text, handler); //Apply immediately values!
            comboBox.TextChanged += (object sender, EventArgs args) => { UIntChanged(comboBox.Text, handler); };
        }

        private void RegisterUIntChanged(TextBox textBox, Action<uint> handler)
        {
            textBox.KeyPress += (object sender, KeyPressEventArgs args) =>
            {
                args.Handled = Char.IsLetter(args.KeyChar);
            };

            UIntChanged(textBox.Text, handler); //Apply immediately values!
            textBox.TextChanged += (object sender, EventArgs args) => { UIntChanged(textBox.Text, handler); };
        }

        private void UIntChanged(string str, Action<uint> handler)
        {
            if (uint.TryParse(str, out uint result))
            {
                handler.Invoke(result);
            }
        }


        private void RegisterIntChanged(TextBox textBox, Action<int> handler)
        {
            textBox.KeyPress += (object sender, KeyPressEventArgs args) =>
            {
                args.Handled = Char.IsLetter(args.KeyChar);
            };

            IntChanged(textBox.Text, handler); //Apply immediately values!
            textBox.TextChanged += (object sender, EventArgs args) => { IntChanged(textBox.Text, handler); };
        }

        private void RegisterIntChanged(ComboBox comboBox, Action<int> handler)
        {
            comboBox.KeyPress += (object sender, KeyPressEventArgs args) =>
            {
                args.Handled = Char.IsLetter(args.KeyChar);
            };

            IntChanged(comboBox.Text, handler); //Apply immediately values!
            comboBox.TextChanged += (object sender, EventArgs args) => { IntChanged(comboBox.Text, handler); };
        }


        private void IntChanged(string str, Action<int> handler)
        {
            if (int.TryParse(str, out int result))
            {
                handler.Invoke(result);
            }
        }


    }
}
