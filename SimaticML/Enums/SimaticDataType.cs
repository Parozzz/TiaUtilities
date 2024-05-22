namespace SimaticML.Enums
{
    public class SimaticDataType(string simaticMLString, uint simaticLength)
    {
        public static readonly SimaticDataType VOID = new("Void", 0);
        public static readonly SimaticDataType BOOLEAN = new("Bool", 0);
        public static readonly SimaticDataType BYTE = new("Byte", 1);
        public static readonly SimaticDataType USINT = new("USInt", 1);
        public static readonly SimaticDataType WORD = new("Word", 2);
        public static readonly SimaticDataType INT = new("Int", 2);
        public static readonly SimaticDataType UINT = new("UInt", 2);
        public static readonly SimaticDataType DWORD = new("DWord", 4);
        public static readonly SimaticDataType DINT = new("DInt", 4);
        public static readonly SimaticDataType UDINT = new("UDInt", 4);
        public static readonly SimaticDataType LWORD = new("LWord", 8);
        public static readonly SimaticDataType REAL = new("Real", 8);
        public static readonly SimaticDataType LREAL = new("LReal", 8);
        public static readonly SimaticDataType TIMER = new("Timer", 0);
        public static readonly SimaticDataType COUNTER = new("Counter", 0);
        public static readonly SimaticDataType STRUCTURE = new("Struct", 0);
        public static readonly SimaticDataType VARIANT = new("Any", 0);

        private static readonly Dictionary<string, SimaticDataType> TYPE_DICT = [];
        static SimaticDataType()
        {
            foreach (var field in typeof(SimaticDataType).GetFields().Where(f => f.IsStatic && f.FieldType == typeof(SimaticDataType)))
            {
                var fieldValue = field.GetValue(null);
                if(fieldValue is SimaticDataType dataType)
                {
                    SimaticDataType.AddDataType(dataType);
                }
            }
        }

        public static SimaticDataType FromSimaticMLString(string simaticString, bool throwException = false)
        {
            if (!TYPE_DICT.TryGetValue(simaticString, out SimaticDataType? value))
            {
                if(throwException)
                {
                    throw new ArgumentException("SimaticDataType " + simaticString + " has not been implemented yet.");
                }

                return SimaticDataType.VOID;
            }

            return value;
        }

        public static void AddDataType(SimaticDataType type)
        {
            var simaticString = type.SimaticMLString;
            if (!TYPE_DICT.TryAdd(simaticString, type))
            {
                throw new ArgumentException("SimaticDataType already exists inside TYPE_DICT");
            }
        }

        public string SimaticMLString { get; init; } = simaticMLString;
        public uint SimaticMLLength { get; init; } = simaticLength;

        public string GetSimaticLengthIdentifier()
        {
            return SimaticMLUtil.GetLengthIdentifier(SimaticMLLength);
        }
    }
}
