using Newtonsoft.Json;

namespace TiaUtilities.JSScript
{
    public class ScriptInfo() : ICleanable
    {
        [JsonProperty] public string Name
        {
            get => _name;
            set
            {
                _name = value;
                this.dirty = true;
            }
        }
        private string _name = "JS_SCRIPT";

        [JsonProperty] public string Text
        {
            get => _text;
            set
            {
                _text = value;
                this.dirty = true;
            }
        }
        private string _text = string.Empty;

        private bool dirty;

        public bool IsDirty() => this.dirty;
        public void Wash() => this.dirty = false;
    }
}
