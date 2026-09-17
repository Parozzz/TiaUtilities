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
        public static void IsTrue( bool condition, string? message = null, [CallerArgumentExpression(nameof(condition))] string? paramName = null)
        {
            if (!condition)
            {
                // Se l'utente specifica un messaggio usa quello, altrimenti ne genera uno automatico con l'espressione
                string errorMessage = message ?? $"'{paramName}' is not True.";
                throw new ArgumentException(errorMessage, paramName);
            }
        }
    }
}
