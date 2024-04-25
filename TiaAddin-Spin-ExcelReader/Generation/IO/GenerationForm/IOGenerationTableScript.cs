using Jint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Utility;

namespace TiaXmlReader.Generation.IO.GenerationForm
{
    public class IOGenerationTableScript
    {
        private readonly GridHandler<IOConfiguration, IOData> gridHandler;
        private readonly IOGenerationSettings settings;

        public IOGenerationTableScript(GridHandler<IOConfiguration, IOData> gridHandler, IOGenerationSettings settings)
        {
            this.gridHandler = gridHandler;
            this.settings = settings;
        }

        public void ShowConfigForm(IWin32Window window)
        {
            var configForm = new ConfigForm("Espressione JS")
            {
                ControlWidth = 950,
                CloseOnOutsideClick = false,
            };

            configForm.AddLine("Variables: address, ioName, dbName, variable, comment");

            configForm.AddJavascriptTextBoxLine(null, height: 350)
                .ControlText(settings.JSTableScript)
                .TextChanged(str => settings.JSTableScript = str);

            configForm.AddButtonPanelLine(null)
                .AddButton("Conferma", () =>
                {
                    if(this.ParseJS())
                    {
                        configForm.Close();
                    }
                    
                })
                .AddButton("Annulla", () => configForm.Close());

            configForm.StartShowingAtCursor();
            configForm.Init();
            configForm.Show(window);
        }

        private bool ParseJS()
        {
            try
            {
                if (string.IsNullOrEmpty(settings.JSTableScript))
                {
                    return false;
                }

                var changedDataDict = new Dictionary<int, IOData>();
                foreach (var entry in gridHandler.DataSource.GetNotEmptyDataDict())
                {
                    var rowIndex = entry.Value;
                    var data = entry.Key;
                    using (var engine = new Engine())
                    {
                        engine.SetValue("address", data.Address ?? "");
                        engine.SetValue("ioName", data.IOName ?? "");
                        engine.SetValue("dbName", data.DBName ?? "");
                        engine.SetValue("variable", data.Variable ?? "");
                        engine.SetValue("comment", data.Comment ?? "");

                        var eval = engine.Evaluate(settings.JSTableScript);

                        var addressJSValue = engine.GetValue("address");
                        var address = addressJSValue.IsString() ? addressJSValue.AsString() : data.Address;

                        var ioNameJSValue = engine.GetValue("ioName");
                        var ioName = ioNameJSValue.IsString() ? ioNameJSValue.AsString() : data.IOName;

                        var dbNameJSValue = engine.GetValue("dbName");
                        var dbName = dbNameJSValue.IsString() ? dbNameJSValue.AsString() : data.DBName;

                        var variableJSValue = engine.GetValue("variable");
                        var variable = variableJSValue.IsString() ? variableJSValue.AsString() : data.Variable;

                        var commentJSValue = engine.GetValue("comment");
                        var comment = commentJSValue.IsString() ? commentJSValue.AsString() : data.Comment;

                        if (address != data.Address || ioName != data.IOName || dbName != data.DBName || variable != data.Variable || comment != data.Comment)
                        {
                            changedDataDict.Add(rowIndex, new IOData()
                            {
                                Address = address,
                                IOName = ioName,
                                DBName = dbName,
                                Variable = variable,
                                Comment = comment,
                            });
                        }
                    }
                }

                this.gridHandler.ChangeMultipleRows(changedDataDict);

                return true;
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            return false;
        }
    }
}
