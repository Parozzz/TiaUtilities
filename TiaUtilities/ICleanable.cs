using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities
{
    public interface ICleanable
    {
        public bool IsDirty();

        public void Wash();
    }
}
