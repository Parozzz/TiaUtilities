using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.SimaticML;

namespace TiaXmlReader.SimaticML.Enums
{
    public enum SimaticProgrammingLanguage
    {
        LADDER,
        DB,
        SAFE_DB,
        SAFE_LADDER,
        AWL,
        SCL,
        INVALID
    }

    public static class SimaticProgrammingLanguageExtension
    {
        public static string GetSimaticMLString(this SimaticProgrammingLanguage programmingLanguage)
        {
            switch (programmingLanguage)
            {
                case SimaticProgrammingLanguage.LADDER: return "LAD";
                case SimaticProgrammingLanguage.DB: return "DB";
                case SimaticProgrammingLanguage.SAFE_DB: return "F_DB";
                case SimaticProgrammingLanguage.SAFE_LADDER: return "F_LAD";
                case SimaticProgrammingLanguage.AWL: return "STL";
                case SimaticProgrammingLanguage.SCL: return "SCL";
                default:
                    throw new Exception("SimaticProgrammingLanguage " + programmingLanguage.ToString() + "not yet implemented.");
            }
        }
    }

    public static class SimaticProgrammingLanguageUtil
    {
        public static SimaticProgrammingLanguage GetFromSimaticMLString(string str, bool throwException = false)
        {
            foreach(SimaticProgrammingLanguage programmingLanguage in Enum.GetValues(typeof(SimaticProgrammingLanguage)))
            {
                if(programmingLanguage.GetSimaticMLString() == str)
                {
                    return programmingLanguage;
                }
            }

            if (throwException)
            {
                throw new ArgumentException("SimaticProgrammingLanguage " + str + " has not been implemented yet.");
            }

            return SimaticProgrammingLanguage.INVALID;
        }
    }
}
