using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimaticML.Blocks
{
    public interface IProgramBlock
    {
        public CompileUnit AddCompileUnit();
    }
}
