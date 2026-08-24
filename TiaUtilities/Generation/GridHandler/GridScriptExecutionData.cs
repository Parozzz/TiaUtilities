using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.JSScript;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridScriptExecutionData : IJsScriptExecutionData
    {
        public Func<object> RequestData { get => RequestDataCallback; }
        public IJsScriptExecutionData.DataDescriptor Descriptor { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

        private readonly IGridHandler gridHandler;
        public GridScriptExecutionData(IGridHandler gridHandler)
        {
            this.gridHandler = gridHandler;
        }

        public Object RequestDataCallback()
        {
        }

        public void Done()
        {
            throw new NotImplementedException();
        }
    }
}
