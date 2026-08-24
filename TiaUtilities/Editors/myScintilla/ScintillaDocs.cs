using System.Reflection;
using System.Text.Json;
using TiaUtilities.Editors.Javascript;

namespace TiaUtilities.Editors.myScintilla
{
    public class ScintillaDocs
    {
        public static Dictionary<string, JsDoc> JS_DOCS
        {
            get
            {
                _jsDocs ??= LoadJsDocs() ?? [];
                return _jsDocs;
            }
        }
        private static Dictionary<string, JsDoc>? _jsDocs;

        private static Dictionary<string, JsDoc>? LoadJsDocs()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TiaUtilities.Editors.myScintilla.JsDoc.json");
            if (stream == null)
            {
                return null;
            }

            var jsonList = JsonSerializer.Deserialize<List<JsDoc>>(stream);
            if (jsonList == null || jsonList.Count == 0)
            {
                return [];
            }

            // Indicizza per nome così da fare ricerche istantanee
            return jsonList.ToDictionary(item => item.Name, item => item);
        }


    }
}
