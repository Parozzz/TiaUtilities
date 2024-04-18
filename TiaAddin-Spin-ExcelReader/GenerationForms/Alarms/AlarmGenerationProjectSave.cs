using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.GenerationForms.Alarms
{
    public class AlarmGenerationProjectSave
    {
        public const string EXTENSION = "json";
        public static string DEFAULT_FILE_PATH = Directory.GetCurrentDirectory() + @"\tempIOSave." + EXTENSION;

    }
}
