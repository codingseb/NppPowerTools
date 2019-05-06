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
                        NppMenuCmd nppMenuCmd = (NppMenuCmd)int.Parse(node.Attributes["id"].Value.Trim());

                        return new NPTCommand()
                        {
                            Name = node.Attributes["name"].Value.Replace("&",string.Empty),
                            CommandAction = () => BNpp.NotepadPP.CallMenuCommand(nppMenuCmd)
                        };
                    })
                    .ToList();
            }
            catch { }
        }
    }
}
