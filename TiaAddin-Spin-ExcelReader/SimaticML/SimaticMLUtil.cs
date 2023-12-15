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
        private readonly string name;
        private readonly List<SimaticAddressComponent> components;

        public SimaticAddressComponent(string name)
        {
            this.name = name;
            this.components = new List<SimaticAddressComponent>();
        }

        public string GetName()
        {
            return name;
        }

        public List<SimaticAddressComponent> GetComponents()
        {
            return components;
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

        public static List<string> SplitAddressIntoComponents(string address)
        {
            var componentList = new List<string>();

            //"Blocco_dati_1"."Wow,"[0]
            //The first part of the address cannot be an array so it can't contain any indexes of array.

            //REMEMBER, THERE COULD BE NO DOUBLE QUOTES INSIDE A COMPONENT OF AN ADDRESS!
            bool waitingCurrentComponent = false;
            int startCurrentComponent = 0;

            var splitAddress = address.Split('.');
            for (int x = 0; x < splitAddress.Length; x++)
            {
                var str = splitAddress[x];
                if (str.StartsWith("\""))
                {
                    if (str.EndsWith("\""))
                    {
                        componentList.Add(str);
                        continue;
                    }

                    if (waitingCurrentComponent)
                    {
                        if(str.EndsWith("\"")) //If it ends with a double quote, the address is completed like this "Wow,"
                        {
                            var segment = new ArraySegment<string>(splitAddress, startCurrentComponent, x);
                            componentList.Add(string.Join(".", segment.Array));
                            continue;
                        }
                        else if(str.Contains("\"") && str.Contains("[")) //If it contains a double quote the address is already completed. This is only valid if also contains an Array sliceAccess (Square braket)
                        {

                        }
                    }

                    startCurrentComponent = x;
                    waitingCurrentComponent = true;
                }
            }

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

            return componentList;
        }
    }
}
