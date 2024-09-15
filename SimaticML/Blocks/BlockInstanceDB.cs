using SimaticML.API;
using SimaticML.Enums;

namespace SimaticML.Blocks
{
    public class BlockInstanceDB : BlockDB
    {
        public const string NODE_NAME = "SW.Blocks.InstanceDB";

        public BlockInstanceDB() : base(BlockInstanceDB.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            //==== INIT CONFIGURATION ====
        }

        public void Init()
        {
            this.Title[SimaticMLAPI.CULTURE] = "";
            this.Comment[SimaticMLAPI.CULTURE] = "";

            //Generate static section
            var _ = this.AttributeList.STATIC;

            this.AttributeList.ProgrammingLanguage = SimaticProgrammingLanguage.DB;
            this.AttributeList.HeaderAuthor = SimaticMLAPI.HEADER_AUTHOR;
            this.AttributeList.HeaderFamily = SimaticMLAPI.HEADER_FAMILY;
        }
    }

}
