using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace TiaUtilities.Generation.GridHandler.JSScript
{
    public class GridScriptContainer() : ICleanable
    {
        public class ContainerSave
        {
            [JsonProperty] public List<ScriptInfo> Scripts { get; set; } = [];
        }

        public class ScriptInfo() : ICleanable
        {
            private string _name = "JS_SCRIPT";
            [JsonProperty] public string Name
            {
                get => _name;
                set
                {
                    _name = value;
                    this.dirty = true;
                }
            }

            private string _text = string.Empty;
            [JsonProperty] public string Text
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

        public ReadOnlyCollection<ScriptInfo> Scripts { get => this.scriptInfoList.AsReadOnly(); }
        public int Count { get => scriptInfoList.Count; }

        private readonly List<ScriptInfo> scriptInfoList = [];

        public ContainerSave CreateSave()
        {
            ContainerSave save = new();
            foreach (var scriptInfo in this.scriptInfoList)
            {
                save.Scripts.Add(scriptInfo);
            }
            return save;
        }

        public void LoadSave(ContainerSave save)
        {
            this.scriptInfoList.Clear();
            scriptInfoList.AddRange(save.Scripts);
        }

        public ScriptInfo AddScript()
        {
            ScriptInfo scriptInfo = new();
            scriptInfoList.Add(scriptInfo);
            return scriptInfo;
        }

        public void RemoveScript(ScriptInfo scriptInfo)
        {
            this.scriptInfoList.Remove(scriptInfo);
        }

        public bool IsDirty() => this.scriptInfoList.Any(x => x.IsDirty());
        public void Wash() => this.scriptInfoList.ForEach(x => x.Wash());
    }
}
