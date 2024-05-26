using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimaticML.LanguageText
{
    public interface ICulturedText
    {
        public string? this[CultureInfo culture] { get; set; }

        public Dictionary<CultureInfo, string> GetDictionary();
    }
}
