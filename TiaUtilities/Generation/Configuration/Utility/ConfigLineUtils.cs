using System.Configuration;
using System.Linq.Expressions;
using System.Reflection;
using TiaXmlReader.Generation.Configuration;

namespace TiaUtilities.Generation.Configuration.Utility
{
    public static class ConfigLineUtils
    {
        public static PropertyInfo ValidateBindExpression(IConfigGroup? configGroup, Expression expression, out object configuration, out IEnumerable<object> otherConfigurations)
        {
            if (expression is not MemberExpression memberExpression || memberExpression.Member is not PropertyInfo propertyInfo)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }
            else if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
            {
                throw new ArgumentException("PropertyInfo must be abled to be read and written");
            }


            configuration = configGroup?.GetConfigForm().Configuration ?? throw new ArgumentException("Configuration cannot be null while binding an IConfigLine");
            otherConfigurations = configGroup?.GetConfigForm().OtherConfigurations ?? throw new ArgumentException("OtherConfigurations cannot be null while binding an IConfigLine");
            if (propertyInfo.DeclaringType == null || configuration.GetType() != propertyInfo.DeclaringType)
            {
                throw new ArgumentException($"Configuration type is different from expression. Expected {propertyInfo.DeclaringType}, Actual {configuration.GetType()}");
            }
            
            foreach(var otherConfig in otherConfigurations)
            {
                if (otherConfig.GetType() != propertyInfo.DeclaringType)
                {
                    throw new ArgumentException($"OtherConfiguration type is different from expression. Expected {propertyInfo.DeclaringType}, Actual {otherConfig.GetType()}");
                }
            }

            return propertyInfo;
        }
    }
}
