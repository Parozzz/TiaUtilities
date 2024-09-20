using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Generation.GridHandler.JSScript
{
    public class ScriptInfo() : ICleanable
    {
        private string _name = "JS_SCRIPT";
        [JsonProperty]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                this.dirty = true;
            }
        }

        private string _text = string.Empty;
        [JsonProperty]
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                this.dirty = true;
            }
        }

        private bool dirty;

        public bool IsDirty() => this.dirty;
        public void Wash() => this.dirty = false;
    }
}
