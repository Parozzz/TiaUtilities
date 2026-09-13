using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TiaUtilities.Utility
{
    public static class Validate
    {
        public static T NotNull<T>([NotNull] T? obj, [CallerArgumentExpression(nameof(obj))] string? paramName = null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException($"{paramName} null");
            }

            return obj;
        }
    }
}
