using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.Utility.Extensions
{
    public static class DictionaryExtensions
    {
        public static void Compute<K, V>(this Dictionary<K, V> dictionary, K key, V value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
                return;
            }

            dictionary.Add(key, value);
        }

        public static V GetOrDefault<K, V>(this Dictionary<K, V> dictionary, K key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }

            return default;
        }
    }
}
