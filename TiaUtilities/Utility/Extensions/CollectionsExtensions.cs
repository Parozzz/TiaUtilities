using System;
using System.Collections.Generic;
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
            return (IEnumerable<T>) enumerable.Where(t => t is not null);
        }

    }
}
