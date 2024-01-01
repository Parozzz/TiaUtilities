using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.SimaticML
{



    public class SimaticAddressComponent
    {
        public string Name;
        public List<SimaticAddressArrayIndex> ArrayIndexes { get; private set; }

        public SimaticAddressComponent()
        {
            this.ArrayIndexes = new List<SimaticAddressArrayIndex>();
        }
    }

    public class SimaticAddressArrayIndex
    {
        public List<SimaticAddressComponent> Components { get; private set; }

        public SimaticAddressArrayIndex()
        {
            this.Components = new List<SimaticAddressComponent>();
        }
    }

    public class SimaticTagAddress
    {
        public static SimaticTagAddress FromAddress(string address)
        {
            if (address.StartsWith("%")) //If reading the address directly from the SimaticML file, i will remove it.
            {
                address = address.Substring(1);
            }

            var memoryArea = SimaticMemoryAreaUtil.GetFromAddress(address);
            if (memoryArea == SimaticMemoryArea.UNDEFINED)
            {
                return null;
            }

            char lengthChar = Char.ToUpper(address[1]);
            string offsetString = Char.IsDigit(lengthChar) ? address.Substring(1) : address.Substring(2);

            uint length = 0; //0 means bit or special like timers, counters or UDT.
            switch (lengthChar)
            {
                case 'B':
                    length = 1;
                    break;
                case 'W':
                    length = 2;
                    break;
                case 'D':
                    length = 4;
                    break;
                case 'L':
                    length = 8;
                    break;
            }

            var splitOffsetString = offsetString.Split('.');
            if (splitOffsetString.Length == 0 || splitOffsetString.Length > 2)
            {
                return null;
            }

            if (!uint.TryParse(splitOffsetString[0], out uint byteOffset))
            {
                return null;
            }

            uint bitOffset = 0;
            if (splitOffsetString.Length == 2 && !uint.TryParse(splitOffsetString[1], out bitOffset))
            {
                return null;
            }

            return new SimaticTagAddress()
            {
                memoryArea = memoryArea,
                length = length,
                byteOffset = byteOffset,
                bitOffset = bitOffset,
            };
        }

        public SimaticMemoryArea memoryArea;
        public uint length;
        public uint byteOffset;
        public uint bitOffset;

        public long GetSortingNumber()
        {
            return (long) ((int)memoryArea * Math.Pow(10, 9) + byteOffset * Math.Pow(10, 3) + bitOffset);
        }

        public string GetAddress()
        {
            return memoryArea.GetTIAMnemonic() + SimaticMLUtil.GetLengthIdentifier(length) + byteOffset + (length == 0 ? ("." + bitOffset) : "");
        }

        public string GetSimaticMLAddress()
        {
            return "%" + this.GetAddress();
        }

        public override string ToString()
        {
            return GetAddress();
        }
    }

    public static class SimaticMLUtil
    {

        public static string WrapAddressComponent(string component)
        {
            return '\"' + component + '\"';
        }

        public static string WrapAddressComponentIfRequired(string component)
        {
            return SimaticMLUtil.ContainsSpecialChars(component) ? SimaticMLUtil.WrapAddressComponent(component) : component;
        }


        public static string GetLengthIdentifier(uint len)
        {
            switch (len)
            {
                case 1: return "B";
                case 2: return "W";
                case 4: return "D";
                case 8: return "L";
                default:
                    return "";
            }
        }

        public static bool ContainsSpecialChars(string str)
        {
            return str.Contains('.') || str.Contains(" ") || str.Contains(",") || str.Contains("\"") || str.Contains("\'") || str.Contains("[") || str.Contains("]");
        }

        public static uint GetBitFromTagAddress(string address)
        {
            address = address.Replace("%", "");
            if (string.IsNullOrEmpty(address))
            {
                return 0;
            }

            int firstDigit = 0;
            for (var x = 0; x < address.Length; x++)
            {
                var c = address[x];
                if (Char.IsDigit(c))
                {
                    firstDigit = x;
                    break;
                }
            }

            var substring = address.Substring(firstDigit);
            return uint.Parse(substring.Split('.')[1]);
        }

        public static uint GetByteFromTagAddress(string address)
        {
            address = address.Replace("%", "");
            if (string.IsNullOrEmpty(address))
            {
                return 0;
            }

            int firstDigit = 0;
            for (var x = 0; x < address.Length; x++)
            {
                var c = address[x];
                if (Char.IsDigit(c))
                {
                    firstDigit = x;
                    break;
                }
            }

            var substring = address.Substring(firstDigit);
            return uint.Parse(substring.Split('.')[0]);
        }

        public static string JoinComponentsIntoAddress(List<string> components)
        {
            string joinedSymbol = "";
            foreach (var component in components)
            {
                var finalComponent = component;
                if (SimaticMLUtil.ContainsSpecialChars(component))
                {//If the component contains a
                    finalComponent = '\"' + finalComponent + '\"';
                }

                joinedSymbol += finalComponent + ".";
            }
            return joinedSymbol.Substring(0, joinedSymbol.Length - 1); //Get the joined component names without the final dot.
        }

        public static List<SimaticAddressComponent> SplitFullAddressIntoComponents(string address)
        {
            var components = FindComponentsInAddress(address);
            return components;
        }

        private static List<SimaticAddressComponent> FindComponentsInAddress(string address)
        {
            var componentList = new List<SimaticAddressComponent>();

            //"Blocco_dati_1"."Wow,"["Blocco_dati_1"."Wow,"]
            //The first part of the address cannot be an array so it can't contain any indexes of array.

            //REMEMBER, THERE COULD BE NO DOUBLE QUOTES INSIDE A COMPONENT OF AN ADDRESS!
            var loopComponent = new LoopComponent();

            var splitAddress = address.Split('.');
            for (int x = 0; x < splitAddress.Length; x++)
            {
                var str = splitAddress[x];
                if (loopComponent.waiting)
                {
                    str = loopComponent.str + "." + str;
                }

                var doubleQuoteCount = str.Count(t => t == '\"');
                if (doubleQuoteCount == 0 || doubleQuoteCount % 2 == 0) //If it has no double quote or is even, it means the address is complete (There cannot be double quotes inside an address).
                {
                    if (str.Contains("["))
                    {
                        if (str.EndsWith("]"))
                        {
                            var openSquareIndex = str.IndexOf("[");
                            var closedSquareIndex = str.LastIndexOf("]"); //Get the last index. If there are two square bracket one after the other (Array[i[1]]) i want the last.

                            var mainComponent = new SimaticAddressComponent()
                            {
                                Name = str.Substring(0, openSquareIndex).Replace("\"", "")
                            };

                            var arrayIndexesStr = str.Substring(openSquareIndex + 1, closedSquareIndex - openSquareIndex - 1); //-1 because i don't won't the final square

                            //If it contais a comma, it probably (Because it can be contains inside the address) means is an array with multiple depth (Like a matrix, with two indexes).
                            //So split it preventively to catch this case.
                            var subComponents = FindComponentsInArrayIndex(arrayIndexesStr);
                            mainComponent.ArrayIndexes.AddRange(subComponents);

                            componentList.Add(mainComponent);

                            loopComponent.Clear();
                            continue;
                        }
                        //else
                        //{
                        //    throw new Exception("Error while parsing address " + string.Join(",", splitAddress) + ". Cannot parse " + str);
                        //}
                    }
                    else
                    {
                        var mainComponent = new SimaticAddressComponent()
                        {
                            Name = str.Replace("\"", "")
                        };
                        componentList.Add(mainComponent);

                        loopComponent.Clear();
                        continue;
                    }
                }

                loopComponent.waiting = true;
                loopComponent.str = str;
            }

            return componentList;
        }

        private static List<SimaticAddressArrayIndex> FindComponentsInArrayIndex(string arrayIndexAddress)
        {
            var arrayIndexesList = new List<SimaticAddressArrayIndex>();

            var loopComponent = new LoopComponent();

            var splitArrayIndexes = arrayIndexAddress.Split(',');
            for (int x = 0; x < splitArrayIndexes.Length; x++)
            {
                var str = splitArrayIndexes[x];
                if (loopComponent.waiting)
                {
                    //In this case i don't have to join with a dot, since i am trying to find the complete address between commans for array indexes.
                    str = loopComponent.str + str;
                }

                var doubleQuoteCount = str.Count(t => t == '\"');
                if (doubleQuoteCount == 0 || doubleQuoteCount % 2 == 0)
                {
                    var components = FindComponentsInAddress(str);

                    var componentArrayIndexes = new SimaticAddressArrayIndex();
                    componentArrayIndexes.Components.AddRange(components);
                    arrayIndexesList.Add(componentArrayIndexes);

                    loopComponent.Clear();
                    continue;
                }

                loopComponent.waiting = true;
                loopComponent.str = str;
            }

            return arrayIndexesList;
        }

        private class LoopComponent
        {
            public bool waiting = false;
            public string str = "";

            public void Clear()
            {
                waiting = false;
                str = "";
            }
        }
    }
}

/*
 
            var loopAddress = address;
            while (true)
            {
                var indexOfDot = loopAddress.IndexOf('.');

                var str = indexOfDot == -1 ? loopAddress : loopAddress.Substring(0, indexOfDot);
                while (true)
                {
                    //If the string does not start with a Double Quote, it means that there will be no point in the middle of it (TIA Won't allow it).
                    //If the string start with a double quote it must end with another one. If it doesn't do it, i will skip the dot since i am in the middle of a string
                    //(Like "DB.TEST".Var1, "DB.TEST" is in itself a component followed by Var1)
                    if (!str.StartsWith("\"") || str.EndsWith("\""))
                    {
                        break;
                    }

                    indexOfDot += loopAddress.IndexOf('.', indexOfDot + 1); //Need to increment the old index since IndexOf return a value starting from zero.
                    if (indexOfDot == -1)
                    {
                        throw new Exception("Error while parsing address " + address + ". Cannot parse " + loopAddress);
                    }

                    str = loopAddress.Substring(0, indexOfDot);
                }

                str = str.Replace("\"", "");

                if (str.Contains("[") && str.EndsWith("]")) //If it contains the open square and ENDS with a closed square (It could be containing them inside the actual name of address) is an array access.
                {

                }

                componentList.Add(str); //There cannot be any double quote in the address.
                if (indexOfDot == -1)
                {
                    break;
                }

                loopAddress = loopAddress.Substring(indexOfDot + 1);
            }

 */