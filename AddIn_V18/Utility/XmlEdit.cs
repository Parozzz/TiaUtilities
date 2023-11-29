using System;
using System.Diagnostics;
using System.Xml;
using Siemens.Engineering;

namespace FCFBConverter.Utility
{
    public static class XmlEdit
    {
        private static int _tiaVersion = 0;
        public static void GetTiaVersion(Project project)
        {
            // Gets the TiaPortal version from the project filename extension
            var projectPath = project.Path.ToString();
            int.TryParse(projectPath.Substring(projectPath.Length - 2), out _tiaVersion);
        }

        private static string GetNamespaceForTiaVersion()
        {
            switch (_tiaVersion)
            {
                default:
                case 16:
                    return @"http://www.siemens.com/automation/Openness/SW/Interface/v4";
                case 17:
                    return @"http://www.siemens.com/automation/Openness/SW/Interface/v5";
            }
        }

        public static bool FCtoFB(string filePath, bool removeReturnMembers)
        {
            try
            {
                //open document
                var document = new XmlDocument();
                document.Load(filePath);

                //select <SW.Blocks.FC> as "root"
                var swBlocksFc = document.SelectSingleNode("//SW.Blocks.FC");

                //set XML namespaces
                var nsmgr = new XmlNamespaceManager(document.NameTable);
                nsmgr.AddNamespace("ns", GetNamespaceForTiaVersion());

                //remove <Section Name="Return"/> and move <Member>s to <Section Name="Output">
                var sectionReturn = swBlocksFc.SelectSingleNode(".//ns:Section[@Name='Return']", nsmgr);
                if (removeReturnMembers == false)
                {
                    var sectionOutput = swBlocksFc.SelectSingleNode(".//ns:Section[@Name='Output']", nsmgr);

                    var members = sectionReturn.SelectNodes("./ns:Member", nsmgr);
                    foreach (XmlNode member in members)
                    {
                        if (member.Attributes.GetNamedItem("Datatype").Value != "Void")
                        {
                            sectionOutput.AppendChild(member);
                        }
                    }
                }

                sectionReturn.ParentNode.RemoveChild(sectionReturn);

                //create new <SW.Blocks.FB> node
                XmlNode swBlocksFb = document.CreateElement("SW.Blocks.FB");

                //add "ID" attribute to <SW.Blocks.FB>
                XmlNode attributeId = document.CreateAttribute("ID");
                attributeId.Value = swBlocksFc.SelectSingleNode("@ID").Value;
                swBlocksFb.Attributes.SetNamedItem(attributeId);

                //copy everything from <SW.Blocks.FC> to <SW.Blocks.FB> to switch the blocktype to FB
                foreach (XmlNode child in swBlocksFc.SelectNodes("./*"))
                {
                    swBlocksFb.AppendChild(child);
                }

                //replace <SW.Blocks.FB> with the new <SW.Blocks.FC>
                swBlocksFc.ParentNode.ReplaceChild(swBlocksFb, swBlocksFc);

                //save the document
                document.Save(filePath);
                return true;
            }
            catch(Exception ex)
            {
                Trace.TraceError("Exception during XML editing:" + Environment.NewLine + ex);
                return false;
            }
        }

        public static bool FBtoFC(string filePath, MoveStaticVariables moveStaticVariables)
        {
            try
            {
                //Open document
                var document = new XmlDocument();
                document.Load(filePath);

                //select <SW.Blocks.FB> as "root"
                var swBlocksFb = document.SelectSingleNode("//SW.Blocks.FB");

                //set XML namespaces
                var nsmgr = new XmlNamespaceManager(document.NameTable);
                nsmgr.AddNamespace("ns", GetNamespaceForTiaVersion());

                //remove <Section Name="Static"/> and move <Member>s to desired <Section>
                var sectionStatic = swBlocksFb.SelectSingleNode(".//ns:Section[@Name='Static']", nsmgr);
                if (moveStaticVariables == MoveStaticVariables.InOut)
                {
                    var sectionInOut = swBlocksFb.SelectSingleNode(".//ns:Section[@Name='InOut']", nsmgr);
                    foreach (XmlNode member in sectionStatic.SelectNodes("./ns:Member", nsmgr))
                    {
                        sectionInOut.AppendChild(member);
                    }
                }
                else if (moveStaticVariables == MoveStaticVariables.Temp)
                {
                    var sectionTemp = swBlocksFb.SelectSingleNode(".//ns:Section[@Name='Temp']", nsmgr);
                    foreach (XmlNode member in sectionStatic.SelectNodes("./ns:Member", nsmgr))
                    {
                        sectionTemp.AppendChild(member);
                    }
                }

                sectionStatic.ParentNode.RemoveChild(sectionStatic);

                //if the block already has a "Ret_Val" defined
                var sectionOutput = swBlocksFb.SelectSingleNode(".//ns:Section[@Name='Output']", nsmgr);
                foreach (XmlNode member in sectionOutput.SelectNodes("./ns:Member", nsmgr))
                {
                    if (member.Attributes.GetNamedItem("Name").Value == "Ret_Val")
                    {
                        //create template string for <Section Name='Return'>
                        var sectionReturnString =
                            "<Section Name='Return'>" +
                            $"<Member Name='Ret_Val' Datatype='{member.Attributes.GetNamedItem("Datatype").Value}' Accessibility='Public' />" +
                            "</Section>";

                        //remove old "Ret_Val"
                        member.ParentNode.RemoveChild(member);

                        //create new <Section Name="Return"> node
                        var sectionReturn = new XmlDocument();
                        sectionReturn.LoadXml(sectionReturnString);

                        //insert new <Section Name="Return"> after <Section Name="Constant">
                        var sectionConstant = swBlocksFb.SelectSingleNode(".//ns:Section[@Name='Constant']", nsmgr);
                        if (sectionConstant?.ParentNode != null && swBlocksFb.OwnerDocument != null && sectionReturn.DocumentElement != null)
                        {
                            sectionConstant.ParentNode.InsertAfter(swBlocksFb.OwnerDocument.ImportNode(sectionReturn.DocumentElement, true),sectionConstant);
                        }

                        break;
                    }
                }
                
                // remove all <StartValue> tags (except from Constant section) because FCs do not support start values
                var startValues = swBlocksFb.SelectNodes(".//ns:Section[@Name!='Constant']//ns:StartValue", nsmgr);
                foreach (XmlNode node in startValues)
                {
                    node?.ParentNode?.RemoveChild(node);
                }

                // remove all <SetPoint> tags from <Section Name="Temp">
                var setPoints = swBlocksFb.SelectNodes(".//ns:Section[@Name='Temp']//ns:BooleanAttribute[@Name='SetPoint']", nsmgr);
                foreach (XmlNode node in setPoints)
                {
                    node?.ParentNode?.RemoveChild(node);
                }

                // remove all <SetPoint> tags from <Section Name="InOut">
                setPoints = swBlocksFb.SelectNodes(".//ns:Section[@Name='InOut']//ns:BooleanAttribute[@Name='SetPoint']", nsmgr);
                foreach (XmlNode node in setPoints)
                {
                    node?.ParentNode?.RemoveChild(node);
                }

                //remove <MemoryReserve>
                var memoryReserve = swBlocksFb.SelectSingleNode(".//MemoryReserve");
                memoryReserve?.ParentNode?.RemoveChild(memoryReserve);

                //create new <SW.Blocks.FC> node
                XmlNode swBlocksFc = document.CreateElement("SW.Blocks.FC");

                //add "ID" attribute to <SW.Blocks.FC>
                XmlNode attributeId = document.CreateAttribute("ID");
                attributeId.Value = swBlocksFb.SelectSingleNode("@ID").Value;
                swBlocksFc.Attributes.SetNamedItem(attributeId);

                //copy everything from <SW.Blocks.FB> to <SW.Blocks.FC> to switch the blocktype to FB
                foreach (XmlNode child in swBlocksFb.SelectNodes("./*"))
                {
                    swBlocksFc.AppendChild(child);
                }

                //replace <SW.Blocks.FB> with the new <SW.Blocks.FC>
                swBlocksFb.ParentNode.ReplaceChild(swBlocksFc, swBlocksFb);

                //save the document
                document.Save(filePath);
                return true;
            }
            catch(Exception ex)
            {
                Trace.TraceError("Exception during XML editing:" + Environment.NewLine + ex);
                return false;
            }
        }
    }
}