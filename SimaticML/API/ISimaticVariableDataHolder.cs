using System.Globalization;

namespace SimaticML.API
{
    public interface ISimaticVariableDataHolder
    {

        public string GetName();
        public void AddComment(CultureInfo cultureInfo, string commentText);
    }
}
