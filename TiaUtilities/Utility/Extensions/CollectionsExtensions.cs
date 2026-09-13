using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Utility.Extensions
{
    public static class CollectionsExtensions
    {
        public static void Swap<T>(this IList<T> list, int firstIndex, int secondIndex)
        {
            if (list == null)
            {
                return;
            }

            Contract.Requires(firstIndex >= 0 && firstIndex < list.Count);
            Contract.Requires(secondIndex >= 0 && secondIndex < list.Count);
            if (firstIndex == secondIndex)
            {
                return;
            }

            T temp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = temp;
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable)
        {
            return (IEnumerable<T>)enumerable.Where(t => t is not null);
        }

        public static bool InRange(this Array array, int index) => index >= 0 && index < array.Length;

        public static bool InRange<T>(this IList<T> array, int index) => index >= 0 && index < array.Count;

        public static bool TryGet<T>(this IList<T> list, int index, [NotNullWhen(true)] out T? value) 
        {
            if(!list.InRange(index))
            {
                value = default;
                return false;
            }

            value = list[index]!;
            return true;
        }

    }
}
