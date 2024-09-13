using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Generation.GridHandler.JSScript
{
    public class GridScriptContainer() : ICleanable
    {
        public class ContainerSave
        {
            [JsonProperty] public Dictionary<string, string> Scripts { get; set; } = [];
        }

        public class ScriptInfo() : ICleanable
        {
            private string _name = "JS_SCRIPT";
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

        public ReadOnlyCollection<ScriptInfo> Scripts { get => this.scriptInfoList.AsReadOnly(); }
        public int Count { get => scriptInfoList.Count; }

        private readonly List<ScriptInfo> scriptInfoList = [];

        public ContainerSave CreateSave()
        {
            ContainerSave save = new();
            foreach (var scriptInfo in this.scriptInfoList)
            {
                save.Scripts.Add(scriptInfo.Name, scriptInfo.Text);
            }
            return save;
        }

        public void LoadSave(ContainerSave save)
        {
            this.scriptInfoList.Clear();

            foreach (var entry in save.Scripts)
            {
                var name = entry.Key;
                var value = entry.Value;

                var scriptInfo = this.AddScript();
                scriptInfo.Name = name;
                scriptInfo.Text = value;
            }
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
