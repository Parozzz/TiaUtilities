using SimaticML.Enums;

namespace SimaticML.API
{
    public interface ISimaticVariableCollection
    {
        public SimaticDataType? FetchDataTypeOf(string variableName);
    }
}
