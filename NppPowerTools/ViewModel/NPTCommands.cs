using NppPowerTools.PluginInfrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;

namespace NppPowerTools
{
    public static class NPTCommands
    {
        private static readonly Regex charRegex = new Regex(".", RegexOptions.Compiled);

        private static string currentNativeLangPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Notepad++",
            "nativeLang.xml");

        public static List<NPTCommand> Commands { get; private set; }

        public static List<NPTCommand> Languages { get; private set; }

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
                    .Append(new NPTCommand()
                    {
                        Name = root.GetEntry("language") + " [@SetLanguage]",
                        CommandAction = win =>
                        {
                            CommandFindViewModel.Instance.Find = "@SetLanguage ";
                            CommandFindViewModel.Instance.FindSelectionLength = 0;
                            CommandFindViewModel.Instance.FindSelectionStart = CommandFindViewModel.Instance.Find.Length;
                        }
                    })
                    .Append(new NPTCommand()
                    {
                        Name = "[@GetCurrentLanguage]",
                        CommandAction = win =>
                        {
                            string lang = BNpp.NotepadPP.GetCurrentLanguage().ToString();
                            BNpp.NotepadPP.FileNew();
                            BNpp.Text = lang;
                            win.Close();
                        }
                    })
                    .ToList();
            }
            catch { }
        }

            // L_TEXT, L_PHP, L_C, L_CPP, L_CS, L_OBJC, L_JAVA, L_RC,
            //L_HTML, L_XML, L_MAKEFILE, L_PASCAL, L_BATCH, L_INI, L_ASCII, L_USER,
            //L_ASP, L_SQL, L_VB, L_JS, L_CSS, L_PERL, L_PYTHON, L_LUA,
            //L_TEX, L_FORTRAN, L_BASH, L_FLASH, L_NSIS, L_TCL, L_LISP, L_SCHEME,
            //L_ASM, L_DIFF, L_PROPS, L_PS, L_RUBY, L_SMALLTALK, L_VHDL, L_KIX, L_AU3,
            //L_CAML, L_ADA, L_VERILOG, L_MATLAB, L_HASKELL, L_INNO, L_SEARCHRESULT,
            //L_CMAKE, L_YAML, L_COBOL, L_GUI4CLI, L_D, L_POWERSHELL, L_R, L_JSP,
            //L_COFFEESCRIPT, L_JSON, L_JAVASCRIPT, L_FORTRAN_77,
            //// Don't use L_JS, use L_JAVASCRIPT instead
            //// The end of enumated language type, so it should be always at the end
            //L_EXTERNAL

        public static void InitLanguages()
        {
            Languages = new List<NPTCommand>
            {
                new NPTCommand()
                {
                    Name = "ActionScript (Flash)",
                    CommandAction = win =>
                    {
                        BNpp.NotepadPP.SetCurrentLanguage(LangType.L_FLASH);
                        win.Close();
                    }
                },
                new NPTCommand()
                {
                    Name = "ASP",
                    CommandAction = win =>
                    {
                        BNpp.NotepadPP.SetCurrentLanguage(LangType.L_ASP);
                        win.Close();
                    }
                },
                new NPTCommand()
                {
                    Name = "C",
                    CommandAction = win =>
                    {
                        BNpp.NotepadPP.SetCurrentLanguage(LangType.L_C);
                        win.Close();
                    }
                },
                new NPTCommand()
                {
                    Name = "C++",
                    CommandAction = win =>
                    {
                        BNpp.NotepadPP.SetCurrentLanguage(LangType.L_CPP);
                        win.Close();
                    }
                },
                new NPTCommand()
                {
                    Name = "C#",
                    CommandAction = win =>
                    {
                        BNpp.NotepadPP.SetCurrentLanguage(LangType.L_CS);
                        win.Close();
                    }
                },
                new NPTCommand()
                {
                    Name = "Java",
                    CommandAction = win =>
                    {
                        BNpp.NotepadPP.SetCurrentLanguage(LangType.L_JAVA);
                        win.Close();
                    }
                },
                new NPTCommand()
                {
                    Name = "JavaScript",
                    CommandAction = win =>
                    {
                        BNpp.NotepadPP.SetCurrentLanguage(LangType.L_JAVASCRIPT);
                        win.Close();
                    }
                },
                new NPTCommand()
                {
                    Name = "JSON",
                    CommandAction = win =>
                    {
                        BNpp.NotepadPP.SetCurrentLanguage(LangType.L_JSON);
                        win.Close();
                    }
                },
                new NPTCommand()
                {
                    Name = "JSP",
                    CommandAction = win =>
                    {
                        BNpp.NotepadPP.SetCurrentLanguage(LangType.L_JSP);
                        win.Close();
                    }
                },
                new NPTCommand()
                {
                    Name = "MS-DOS Style",
                    CommandAction = win =>
                    {
                        BNpp.NotepadPP.SetCurrentLanguage(LangType.L_ASCII);
                        win.Close();
                    }
                },
                new NPTCommand()
                {
                    Name = "PHP",
                    CommandAction = win =>
                    {
                        BNpp.NotepadPP.SetCurrentLanguage(LangType.L_PHP);
                        win.Close();
                    }
                },
            };
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
                else if ((id - 45000).IsBetween(4, 13))
                {
                    prefix += root.GetEntry("encoding") + "->";
                }
                else if ((id - 45000).IsBetween(1, 3))
                {
                    prefix += root.GetEntry("edit") + "->" + root.GetSubEntry("edit-eolConversion") + "->";
                }
                else if (commandId.StartsWith("46"))
                {
                    prefix += root.GetEntry("language") + "->";
                }
                else if (commandId.StartsWith("47"))
                {
                    prefix += "?->";
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

        public static List<NPTCommand> RegexFilterCommands(this List<NPTCommand> commands, string filter)
        {
            Regex findRegex = new Regex("(?<start>.*)" + charRegex.Replace(filter, match => "(?<match>" + Regex.Escape(match.Value) + ")(?<between>.*)"), RegexOptions.IgnoreCase);

            return commands?.FindAll(command =>
            {
                 Match match = findRegex.Match(command.Name);

                 if (match.Success)
                 {
                     List<Inline> inlines = new List<Inline>();

                     string start = match.Groups["start"].Value;

                     if (!string.IsNullOrEmpty(start))
                         inlines.Add(new Run(start));

                     CaptureCollection mcaptures = match.Groups["match"].Captures;
                     CaptureCollection bcaptures = match.Groups["between"].Captures;

                     for (int i = 0; i < mcaptures.Count; i++)
                     {
                         inlines.Add(new Span(new Run(mcaptures[i].Value))
                         {
                             FontWeight = FontWeights.Bold,
                             Background = Brushes.Yellow,
                         });
                         inlines.Add(new Run(bcaptures[i].Value));
                     }

                     command.Inlines = inlines;

                     return true;
                 }
                 else
                 {
                     return false;
                 }
             });
        }
    }
}
