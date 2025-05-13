using TiaUtilities.Generation.Configuration;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigSeparatorLine : ConfigLine<ConfigSeparatorLine>
    {
        private readonly Panel panel;
        public ConfigSeparatorLine()
        {
            panel = new Panel()
            {
                AutoSize = true,
                Height = 10,
                Dock = DockStyle.Fill,
                //BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Gray,
            };
        }

        public override Control? GetControl()
        {
            return panel;
        }
    }
}
