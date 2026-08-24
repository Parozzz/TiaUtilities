using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.JSScript
{
    public interface IJsScriptExecutionData
    {

        public class DataDescriptor {

            public required string Name { get; init; }

            public readonly Dictionary<string, string> SimpleProperties = []; //key, value type

            public readonly List<DataDescriptor> ComplexProperties = [];

            public DataDescriptor AddComplex(string name) => new() { Name = name };
        }


        public Func<Object> RequestData { get; }

        public DataDescriptor Descriptor { get; init; }

        public void Done(Object? result);
    }
}
