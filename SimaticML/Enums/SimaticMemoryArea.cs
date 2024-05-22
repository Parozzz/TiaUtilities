namespace SimaticML.Enums
{

    public enum SimaticMemoryArea
    {
        [SimaticEnum("I", "E")] INPUT = 1,
        [SimaticEnum("Q", "A")] OUTPUT = 2,
        [SimaticEnum("M")] MERKER = 3,
        [SimaticEnum("T")] TIMER = 4,
        [SimaticEnum("C")] COUNTER = 5,
        [SimaticEnum("UND")] UNDEFINED = 0 //Default value
    }
}
