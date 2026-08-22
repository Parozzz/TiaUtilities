using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Editors.myScintilla
{
    public static class ScintillaUtils
    {
        public static List<string> JS_SUGGESTIONS = [
            "Array",
            "Array.from",
            "Array.isArray",
            "async",
            "await",
            "boolean",
            "Boolean",
            "break",
            "case",
            "catch",
            "class",
            "console",
            "Console",
            "console.error",
            "console.log",
            "console.warn",
            "const",
            "continue",
            "Date",
            "Date.now",
            "Date.parse",
            "debugger",
            "decodeURI",
            "default",
            "delete",
            "do",
            "else",
            "encodeURI",
            "export",
            "extends",
            "false",
            "finally",
            "for",
            "function",
            "if",
            "import",
            "in",
            "instanceof",
            "isNaN",
            "JSON",
            "JSON.parse",
            "JSON.stringify",
            "Math",
            "Math.abs",
            "Math.ceil",
            "Math.floor",
            "Math.max",
            "Math.min",
            "Math.pow",
            "Math.random",
            "Math.round",
            "Math.sqrt",
            "new",
            "null",
            "number",
            "Number",
            "Number.isInteger",
            "Number.parseFloat",
            "Number.parseInt",
            "Object",
            "Object.assign",
            "Object.keys",
            "Object.values",
            "parseFloat",
            "parseInt",
            "return",
            "string",
            "String",
            "String.fromCharCode",
            "super",
            "switch",
            "this",
            "throw",
            "true",
            "try",
            "typeof",
            "undefined",
            "var",
            "void",
            "while",
            "with",
            "yield"
        ];

        public enum BraceType
        {
            None = 0,
            Opening = 1,
            Closing = 2
        }

        public static BraceType IsJSBrace(int c)
        {
            return c switch
            {
                '(' or '[' or '{' or '<' => BraceType.Opening,
                ')' or ']' or '}' or '>' => BraceType.Closing,
                _ => BraceType.None,
            };
        }

        public static char GetMatchingJSBrace(int c)
        {
            return c switch
            {
                '(' => ')',
                '[' => ']',
                '{' => '}',
                '<' => '>',
                ')' => '(',
                ']' => '[',
                '}' => '{',
                '>' => '<',
                _ => throw new ArgumentException($"Character '{c}' is not a recognized JS brace."),
            };
        }

        public static string GetFullFunctionName(Scintilla scintilla, int endPos)
        {
            int wordStart = scintilla.WordStartPosition(endPos, true);

            string functionName = scintilla.GetTextRange(wordStart, endPos - wordStart);
            if (wordStart > 0 && scintilla.GetCharAt(wordStart - 1) == '.')
            {
                // Se il carattere prima della parola è un punto, cerca la funzione completa (es. Math.floor)
                int objectStart = scintilla.WordStartPosition(wordStart - 2, true);

                var objectName = scintilla.GetTextRange(objectStart, (wordStart - 1) - objectStart);
                functionName = objectName + "." + functionName;
            }

            return functionName;
        }
    }
}
