using System.Linq.Expressions;
using System.Reflection;
using TiaXmlReader.Generation.Configuration;

namespace TiaUtilities.Generation.Configuration.Utility
{
    public static class ConfigLineUtils
    {
        public static PropertyInfo ValidateBindExpression(IConfigGroup? configGroup, Expression expression, out object configuration)
        {
            if (expression is not MemberExpression memberExpression || memberExpression.Member is not PropertyInfo propertyInfo)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }
            else if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
            {
                throw new ArgumentException("PropertyInfo must be abled to be read and written");
            }

            configuration = configGroup?.GetConfigForm().Configuration ?? throw new ArgumentException("Configuration cannot be null while binding text");
            if (propertyInfo.DeclaringType == null || configuration.GetType() != propertyInfo.DeclaringType)
            {
                throw new ArgumentException($"Configuration type is different from expression. Expected {propertyInfo.DeclaringType}, Actual {configuration.GetType()}");
            }

            return propertyInfo;
        }
    }
}
