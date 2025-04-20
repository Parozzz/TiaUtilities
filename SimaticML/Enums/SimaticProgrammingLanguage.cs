namespace SimaticML.Enums
{
    public enum SimaticProgrammingLanguage
    {
        [SimaticEnum("LAD")] LADDER,
        [SimaticEnum("DB")] DB,
        [SimaticEnum("F_DB")] SAFE_DB,
        [SimaticEnum("F_LAD")] SAFE_LADDER,
        [SimaticEnum("FBD")] FBD, //NOT IMPLEMENTED YET
        [SimaticEnum("F_FBD")] SAFE_FBD, //NOT IMPLEMENTED YET
        [SimaticEnum("STL")] AWL, //NOT IMPLEMENTED YET
        [SimaticEnum("SCL")] SCL, //NOT IMPLEMENTED YET
        [SimaticEnum("INVALID")] INVALID
    }
}
