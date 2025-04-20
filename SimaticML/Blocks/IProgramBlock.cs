using SimaticML.Enums;

namespace SimaticML.Blocks
{
    public interface IProgramBlock
    {
        public SimaticProgrammingLanguage GetProgrammingLanguage();
        public CompileUnit AddCompileUnit(SimaticProgrammingLanguage programmingLanguage);
    }
}
