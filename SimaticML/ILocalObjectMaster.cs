using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimaticML;

namespace SimaticML
{
    public interface ILocalObjectMaster
    {
        void UpdateLocalObjects();
        IDGenerator GetLocalIDGenerator();
    }
}
