using SimaticML.API;
using SimaticML.Enums;

namespace SimaticML.Blocks
{
    public class BlockGlobalDB : BlockDB
    {
        public const string NODE_NAME = "SW.Blocks.GlobalDB";

        public BlockGlobalDB() : base(BlockGlobalDB.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            //==== INIT CONFIGURATION ====
        }

        public void Init()
        {
            this.Title[SimaticMLAPI.CULTURE] = "";
            this.Comment[SimaticMLAPI.CULTURE] = "";

            //Add static sections
            var _ = this.AttributeList.STATIC;

            this.AttributeList.ProgrammingLanguage = SimaticProgrammingLanguage.DB;
            this.AttributeList.MemoryReserve = 100;
            this.AttributeList.MemoryLayout = "Optimized";
            this.AttributeList.HeaderAuthor = SimaticMLAPI.HEADER_AUTHOR;
            this.AttributeList.HeaderFamily = SimaticMLAPI.HEADER_FAMILY;
        }
    }

}
