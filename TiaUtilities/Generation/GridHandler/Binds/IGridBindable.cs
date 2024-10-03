using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Generation.GridHandler.Binds
{
    public interface IGridBindable
    {
        public void BindToGridHandler(GridHandlerBind? handlerBind);
    }
}
