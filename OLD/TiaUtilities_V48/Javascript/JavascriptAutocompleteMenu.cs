using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.Javascript
{
    public static class JavascriptAutocompleteMenu
    {
        public static Dictionary<string, string> GLOBAL_FUNCTION_PROPERTIES = new Dictionary<string, string>()         
        {
            { "eval(x)", "" },
            { "parseInt(string, radix)", "" },
            { "parseFloat(string)", "" },
            { "isNan(number)", "" },
            { "isFinite(number)", "" },
            { "decodeURI(encodedURI)", "" },
            { "decodeURIComponent(encodedURIComponent)", "" },
            { "encodeURI(uri)", "" },
            { "encodeURIComponent(uriComponent)", "" },
            { "escape(string)", "" },
            { "unescape(string)", "" },
        };

        public static void Show()
        {

        }
    }
}

/*
         "if", "in", "do", "var", "for", "new", "try", "let", "this", "else", "case", "void", "with", "enum",
        "while", "break", "catch", "throw", "const", "yield", "class", "super", "return", "typeof", "delete", "switch",
        "export", "import", "default", "finally", "extends", "function", "continue", "debugger", "instanceof",
        // contextual keywords (should at least include "null", "false" and "true")
        "as", "of", "get", "set", "false", "from", "null", "true", "async", "await", "static", "constructor",
        // some common identifiers/literals in our test data set (benchmarks + test suite)
        "undefined", "length", "object", "Object", "obj", "Array", "Math", "data", "done", "args", "arguments", "Symbol", "prototype",
        "options", "value", "name", "self", "key", "\"use strict\"", "use strict"
var autoCompleteItemList = new List<AutocompleteItem>();

var engine = new Engine();
foreach(var globalPropertyEntry in engine.Global.GetOwnProperties())
{
    var jsValue = globalPropertyEntry.Key;
    var globalPropertyDescriptor = globalPropertyEntry.Value;

    if (globalPropertyDescriptor.Value is Function function)
    {
        foreach(var functionPropertyEntry in function.GetOwnProperties())
        {
            if(functionPropertyEntry.Key.IsString() && functionPropertyEntry.Key.AsString() == "name")
            {
                var functionPropertyDescriptor = functionPropertyEntry.Value;
                if(functionPropertyDescriptor.Value.IsString())
                {
                    var item = new AutocompleteItem(functionPropertyDescriptor.Value.AsString()); //Text in the constructor is needed! Otherwise null exception is thrown.
                    autoCompleteItemList.Add(item);
                }

            }
            var ___ = "";
        }
        var __ = "";
    }

    var _ = "";
}

var item = new AutocompleteItem("wow") //Text in the constructor is needed! Otherwise null exception is thrown.
{
    MenuText = "wow",
    ToolTipTitle = "Desc",
    ToolTipText = "wowowowowow",
};

var m = new AutocompleteMenu(fctb);
m.Items.SetAutocompleteItems(autoCompleteItemList);
m.Show(true);
 */