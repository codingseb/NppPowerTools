using NppPowerTools.PluginInfrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace NppPowerTools
{
    public static class NPTCommands
    {
        private static string currentNativeLangPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Notepad++",
            "nativeLang.xml");

        public static List<NPTCommand> Commands { get; private set; }

        public static void InitCommands()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(currentNativeLangPath);

                XmlNode root = doc.DocumentElement;

                XmlNodeList commandsNodesList = root.SelectNodes("//Native-Langue/Menu/Main/Commands/Item");

                Commands = commandsNodesList
                    .Cast<XmlNode>()
                    .OrderBy(node =>
                    {
                        string id = node.Attributes["id"].Value.Trim();

                        if (id.StartsWith("1"))
                            id = "9" + id;

                        return int.Parse(id);
                    })
                    .Select(node =>
                    {
                        string id = node.Attributes["id"].Value.Trim();
                        int nppMenuCmd = int.Parse(id);

                        return new NPTCommand()
                        {
                            Name = root.GetPrefix(id) + node.Attributes["name"].Value.Replace("&", string.Empty),
                            CommandId = nppMenuCmd,
                            CommandAction = win =>
                            {
                                BNpp.NotepadPP.CallMenuCommand(nppMenuCmd);
                                win.Close();
                            }
                        };
                    })
                    .ToList();
            }
            catch { }
        }

        private static string GetPrefix(this XmlNode root, string commandId)
        {
            string prefix = string.Empty;
            try
            {
                int id = int.Parse(commandId);

                if (commandId.StartsWith("41"))
                {
                    prefix += root.GetEntry("file") + "->";

                    int subId = id - 41000;

                    if (subId.IsOneOf(5, 9, 18, 24))
                        prefix += root.GetSubEntry("file-closeMore") + "->";
                    else if (subId.IsOneOf(19, 20))
                        prefix += root.GetSubEntry("file-openFolder") + "->";
                    else if (subId.IsBetween(21, 23))
                        prefix += root.GetSubEntry("file-recentFiles") + "->";
                }
                else if((id - 42000).IsOneOf(18,19,21,25,32) || id == 48016)
                {
                    prefix += root.GetEntry("macro") + "->";
                }
                else if((id - 45000).IsBetween(4,13))
                {
                    prefix += root.GetEntry("encoding") + "->";
                }
                else if((id - 45000).IsBetween(1,3))
                {
                    prefix += root.GetEntry("edit") + "->" + root.GetSubEntry("edit-eolConversion") + "->";
                }
                else if(commandId.StartsWith("50"))
                {
                    prefix += root.GetEntry("edit") + "->" + root.GetSubEntry("edit-autoCompletion") + "->";
                }
                else if (commandId.StartsWith("42"))
                {
                    prefix += root.GetEntry("edit") + "->";

                    int subId = id - 42000;

                    if (subId.IsBetween(29, 31))
                        prefix += root.GetSubEntry("edit-copyToClipboard") + "->";
                    else if (subId.IsOneOf(8, 9))
                        prefix += root.GetSubEntry("edit-indent") + "->";
                    else if (subId.IsOneOf(16, 17) || subId.IsBetween(67, 72))
                        prefix += root.GetSubEntry("edit-convertCaseTo") + "->";
                    else if (subId.IsBetween(10, 15) ||subId.IsBetween(55, 66) || subId.IsOneOf(77))
                        prefix += root.GetSubEntry("edit-lineOperations") + "->";
                    else if (subId.IsOneOf(22, 23, 47, 35, 36))
                        prefix += root.GetSubEntry("edit-comment") + "->";
                    else if (subId.IsBetween(42,46) ||subId.IsOneOf(54,53,24))
                        prefix += root.GetSubEntry("edit-blankOperations") + "->";
                    else if (subId.IsBetween(48,50) ||subId.IsOneOf(38,39))
                        prefix += root.GetSubEntry("edit-pasteSpecial") + "->";
                    else if (subId.IsBetween(73,76))
                        prefix += root.GetSubEntry("edit-onSelection") + "->";
                }
                else if (commandId.StartsWith("43"))
                {
                    prefix += root.GetEntry("search") + "->";

                    int subId = id - 43000;

                    if (subId.IsOneOf(22, 24, 26, 28, 30))
                        prefix += root.GetSubEntry("search-markAll") + "->";
                    else if (subId.IsOneOf(23, 25, 27, 29, 31, 32))
                        prefix += root.GetSubEntry("search-unmarkAll") + "->";
                    else if (subId.IsBetween(33, 38))
                        prefix += root.GetSubEntry("search-jumpUp") + "->";
                    else if (subId.IsBetween(39, 44))
                        prefix += root.GetSubEntry("search-jumpDown") + "->";
                    else if (subId.IsBetween(5, 8) || subId.IsBetween(18, 21) ||subId.IsOneOf(50,51))
                        prefix += root.GetSubEntry("search-bookmark") + "->";
                }
                else if(id.IsBetween(10001,10004))
                {
                    prefix += root.GetEntry("view") + "->" + root.GetSubEntry("view-moveCloneDocument") + "->";
                }
                else if (commandId.StartsWith("44"))
                {
                    prefix += root.GetEntry("view") + "->";

                    int subId = id - 44000;

                    //if (subId.IsOneOf())
                    //    prefix += root.GetSubEntry("view-currentFileIn") + "->";
                    if (subId.IsOneOf(19,20,25,26,41))
                        prefix += root.GetSubEntry("view-showSymbol") + "->";
                    else if (subId.IsOneOf(23,24,33))
                        prefix += root.GetSubEntry("view-zoom") + "->";
                    else if (subId.IsBetween(86,99) && subId != 97)
                        prefix += root.GetSubEntry("view-tab") + "->";
                    //else if (subId.IsBetween())
                    //    prefix += root.GetSubEntry("view-collapseLevel") + "->";
                    //else if (subId.IsOneOf())
                    //    prefix += root.GetSubEntry("view-uncollapseLevel") + "->";
                    else if (subId.IsBetween(81,83))
                        prefix += root.GetSubEntry("view-project") + "->";
                }
                else if (commandId.StartsWith("45"))
                {
                    prefix += root.GetEntry("view") + "->";

                    int subId = id - 45000;

                    //if (subId.IsOneOf())
                    //    prefix += root.GetSubEntry("view-currentFileIn") + "->";
                    if (subId.IsOneOf(19,20,25,26,41))
                        prefix += root.GetSubEntry("view-showSymbol") + "->";
                    else if (subId.IsOneOf(23,24,33))
                        prefix += root.GetSubEntry("view-zoom") + "->";
                    else if (subId.IsBetween(86,99) && subId != 97)
                        prefix += root.GetSubEntry("view-tab") + "->";
                    //else if (subId.IsBetween())
                    //    prefix += root.GetSubEntry("view-collapseLevel") + "->";
                    //else if (subId.IsOneOf())
                    //    prefix += root.GetSubEntry("view-uncollapseLevel") + "->";
                    else if (subId.IsBetween(81,83))
                        prefix += root.GetSubEntry("view-project") + "->";
                }
            }
            catch
            {
            }

            return prefix;
        }


        private static bool IsOneOf(this int id, params int[] values) => values.Contains(id);
        private static bool IsBetween(this int id, int greaterOrEqual, int smallerOrEqual) => id >= greaterOrEqual && id <= smallerOrEqual;
        private static string GetEntry(this XmlNode root, string menuId) => root?.SelectSingleNode($"//Native-Langue/Menu/Main/Entries/Item[@menuId='{menuId}']")?.Attributes["name"]?.Value?.Replace("&", string.Empty) ?? string.Empty;
        private static string GetSubEntry(this XmlNode root, string subMenuId) => root?.SelectSingleNode($"//Native-Langue/Menu/Main/SubEntries/Item[@subMenuId='{subMenuId}']")?.Attributes["name"]?.Value?.Replace("&", string.Empty) ?? string.Empty;
    }
}
