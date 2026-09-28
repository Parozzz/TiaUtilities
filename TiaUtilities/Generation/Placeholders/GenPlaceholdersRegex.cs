using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TiaUtilities.Generation.Placeholders
{
    public static partial class GenPlaceholdersRegex
    {
        // Genera il codice di matching a tempo di compilazione
        [GeneratedRegex(@"\{([^{}]+)\}")]
        private static partial Regex PlaceholdersRegex();

        [GeneratedRegex(@"\{[^{}]*_(\d+)\}")]
        private static partial Regex PlaceholderNumbersRegex();

        public static IEnumerable<string> GetPlaceholders(string input)
        {
            return PlaceholdersRegex()
                .Matches(input)
                .Select(m => m.Groups[1].Value);
        }

        public static bool HasPlaceholders(string input) => PlaceholdersRegex().Count(input) > 0;

        public static string StripPlaceholders(string input) => PlaceholdersRegex().Replace(input, "");

        public static IEnumerable<int> GetPlaceholderNumbers(string input)
        {
            return PlaceholderNumbersRegex()
                .Matches(input)
                .Select(m => int.Parse(m.Groups[1].Value));
        }
    }
}
