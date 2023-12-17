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
        private readonly List<SimaticAddressArrayIndex> arrayIndexes;

        public SimaticAddressComponent(string name)
        {
            this.name = name;
            this.arrayIndexes = new List<SimaticAddressArrayIndex>();
        }

        public string GetName()
        {
            return name;
        }

        public List<SimaticAddressArrayIndex> GetArrayIndexes()
        {
            return arrayIndexes;
        }
    }

    public class SimaticAddressArrayIndex
    {
        private readonly List<SimaticAddressComponent> components;

        public SimaticAddressArrayIndex()
        {
            this.components = new List<SimaticAddressComponent>();
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
                            var mainComponent = new SimaticAddressComponent(str.Substring(0, str.IndexOf("[")).Replace("\"", ""));

                            var openSquareIndex = str.IndexOf("[");
                            var closedSquareIndex = str.IndexOf("]");
                            var arrayIndexesStr = str.Substring(openSquareIndex + 1, closedSquareIndex - openSquareIndex - 1); //Length decreased by one to ignore the closing square bracket.

                            //If it contais a comma, it probably (Because it can be contains inside the address) means is an array with multiple depth (Like a matrix, with two indexes).
                            //So split it preventively to catch this case.
                            var subComponents = FindComponentsInArrayIndex(arrayIndexesStr);
                            mainComponent.GetArrayIndexes().AddRange(subComponents);

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
                        var mainComponent = new SimaticAddressComponent(str.Replace("\"", ""));
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
                    componentArrayIndexes.GetComponents().AddRange(components);
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