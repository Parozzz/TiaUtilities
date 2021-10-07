using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpinAddIn
{

    public class FC
    {
    }

    public class CompileUnit //A.K.A Segment!
    {
        public string Title { get; }
        public string Comment { get; }
        public string ProgrammingLanguage { get; }


    }

    public class Interface
    {
        private readonly List<Section> sectionList;

        internal Interface()
        {
            sectionList = new List<Section>();
        }
    }

    public class Section
    {

        public class Member
        {

        }
    }

    public class NetworkSource
    {

    }

    public class FlagNet 
    {

    }
}
