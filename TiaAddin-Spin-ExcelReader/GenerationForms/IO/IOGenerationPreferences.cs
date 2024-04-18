using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.GenerationForms.IO
{
    public class IOGenerationPreferences
    {
        [JsonProperty] public Color PreviewColor = Color.DarkGreen;
    }
}
