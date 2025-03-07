using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimaticML
{
    public interface ISimaticVariableDataHolder
    {

        public string GetName();
        public void AddComment(CultureInfo cultureInfo, string commentText);
    }
}
