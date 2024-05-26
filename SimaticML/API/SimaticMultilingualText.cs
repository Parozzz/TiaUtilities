using SimaticML.LanguageText;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimaticML.API
{
    public class SimaticMultilingualText(ICulturedText culturedText) : ICulturedText
    {

        public SimaticMultilingualText() : this(new CulturedTextImpl()) {  }

        public string? this[CultureInfo culture]
        {
            get => culturedText[culture];
            set => culturedText[culture] = value;
        }

        public Dictionary<CultureInfo, string> GetDictionary()
        {
            return culturedText.GetDictionary();
        }

        private class CulturedTextImpl : ICulturedText
        {
            private readonly Dictionary<CultureInfo, string> dict;
            public CulturedTextImpl()
            {
                this.dict = [];
            }

            public string? this[CultureInfo culture]
            {
                get => this.dict.TryGetValue(culture, out var value) ? value : null;
                set
                {
                    if(value == null)
                    {
                        this.dict.Remove(culture);
                    }
                    else if(!this.dict.TryAdd(culture, value))
                    {
                        this.dict[culture] = value;
                    }
                }
            }

            public Dictionary<CultureInfo, string> GetDictionary()
            {
                return dict.ToDictionary();
            }
        }
    }


}
