namespace SimaticML.Enums
{
    public enum SimaticVariableScope
    {
        [SimaticEnum("Undef")] UNKNOW = 0, //Default value
        [SimaticEnum("GlobalVariable")] GLOBAL_VARIABLE,
        [SimaticEnum("LocalVariable")] LOCAL_VARIABLE,
        [SimaticEnum("LocalConstant")] LOCAL_CONSTANT,
        [SimaticEnum("LiteralConstant")] LITERAL_CONSTANT,
        [SimaticEnum("TypedConstant")] TYPED_CONSTANT,
        [SimaticEnum("GlobalConstant")] GLOBAL_CONSTANT,
        [SimaticEnum("Label")] LABEL,
    }
}
