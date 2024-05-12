using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Utility;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigButtonPanelLine : ConfigLine<ConfigButtonPanelLine>
    {
        private readonly TableLayoutPanel panel;

        public ConfigButtonPanelLine()
        {
            panel = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                Padding = new Padding(0),
                Margin = new Padding(left: 20, right: 20, top: 2, bottom: 2),
                ColumnCount = 0,
                RowCount = 1,
                RowStyles = { new RowStyle(SizeType.AutoSize) }
            };
        }

        public ConfigButtonPanelLine AddButton(string buttonText, Action clickAction)
        {
            panel.ColumnCount++;
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            var button = new Button
            {
                Text = buttonText,
                Dock = DockStyle.Top,
                AutoSize = true
            };
            button.Click += (sender, args) => clickAction.Invoke();
            panel.Controls.Add(button);

            return this;
        }

        public override Control GetControl()
        {
            return panel;
        }
    }
}
