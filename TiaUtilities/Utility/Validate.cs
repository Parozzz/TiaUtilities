using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Utility
{
    public static class Validate
    {
        public static T NotNull<T>(T? obj)
        {
            if (obj == null) throw new ArgumentNullException("Object null");

            return obj;
        }
    }
}
