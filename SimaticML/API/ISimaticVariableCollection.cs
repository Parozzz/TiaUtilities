using SimaticML.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimaticML.API
{
    public interface ISimaticVariableCollection
    {
        public SimaticDataType? FetchDataTypeOf(string variableName);
    }
}
