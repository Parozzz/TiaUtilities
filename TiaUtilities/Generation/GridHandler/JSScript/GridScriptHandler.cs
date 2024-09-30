using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Javascript;

namespace TiaUtilities.Generation.GridHandler.JSScript
{
    public class GridScriptHandler
    {

        public JavascriptErrorReportThread JSErrorThread { get; init; }

        private readonly List<ScriptInfo> scriptInfoList;
        private readonly ObservableCollection<GridScriptVariable> gridVariableList;
        private readonly ObservableCollection<GridScriptVariable> customVariableList;

        public GridScriptForm Form
        {
            get
            {
                if(_form == null)
                {
                    _form = new();
                    _form.Init();
                    _form.FormClosed += (sender, args) => _form = null;
                }

                return _form;
            }
        }
        private GridScriptForm? _form;

        public GridScriptHandler(JavascriptErrorReportThread jsErrorThread)
        {
            this.JSErrorThread = jsErrorThread;

            this.scriptInfoList = [];

            this.gridVariableList = [];
            this.customVariableList = [];
        }

        public void Init()
        {
            customVariableList.CollectionChanged += (sender, args) => this.UpdateVariableView();
            gridVariableList.CollectionChanged += (sender, args) => this.UpdateVariableView();
        }

        public ScriptInfo AddScript()
        {
            ScriptInfo scriptInfo = new();
            this.scriptInfoList.Add(scriptInfo);
            return scriptInfo;
        }

        public void RemoveScript(ScriptInfo scriptInfo)
        {
            this.scriptInfoList.Remove(scriptInfo);
        }
    }
}
