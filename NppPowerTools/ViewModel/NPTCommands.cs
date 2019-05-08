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

                        ShortcutKey? shortcutKey = null;

                        try
                        {
                            shortcutKey = BNpp.NotepadPP.GetShortcutByCommandId(nppMenuCmd);
                        }
                        catch { }

                        return new NPTCommand()
                        {
                            Name = root.GetPrefix(id) + node.Attributes["name"].Value.Replace("&", string.Empty),
                            CommandId = nppMenuCmd,
                            Shortcut = shortcutKey,
                            CommandAction = win =>
                            {
                                BNpp.NotepadPP.CallMenuCommand(nppMenuCmd);
                                win.Close();
                            }
                        };
                    })
                    .Append(new NPTCommand()
                    {
                        Name = root.GetEntry("language") + "-> [@SetLanguage]",
                        CommandAction = win =>
                        {
                            CommandFindViewModel.Instance.Find = "@SetLanguage ";
                            CommandFindViewModel.Instance.FindSelectionStart = CommandFindViewModel.Instance.Find.Length;
                        }
                    })
                    .Append(new NPTCommand()
                    {
                        Name = "[@GetCurrentLanguage]",
                        CommandAction = win =>
                        {
                            string lang = BNpp.NotepadPP.GetCurrentLanguage().ToString();
                            BNpp.Text = lang;
                            win.Close();
                        }
                    })
                    .ToList();
            }
            catch { }
        }

        public static void InitLanguages()
        {
            NPTCommand CreateLangCommand(string name, LangType langType) => new NPTCommand()
            {
                Name = name,
                CommandAction = win =>
                {
                    BNpp.NotepadPP.SetCurrentLanguage(langType);
                    win.Close();
                }
            };

            Languages = new List<NPTCommand>
            {
                CreateLangCommand("ActionScript (Flash)", LangType.L_FLASH),
                CreateLangCommand("Ada", LangType.L_ADA),
                CreateLangCommand("ASN.1", LangType.L_ASN1),
                CreateLangCommand("ASP", LangType.L_ASP),
                CreateLangCommand("Assembly (ASM)", LangType.L_ASM),
                CreateLangCommand("AutoIt", LangType.L_AU3),
                CreateLangCommand("AviSynth", LangType.L_AVISYNTH),
                CreateLangCommand("BaanC", LangType.L_EXTERNAL),
                CreateLangCommand("Batch", LangType.L_BATCH),
                CreateLangCommand("Blitzbasic", LangType.L_BLITZBASIC),
                CreateLangCommand("C", LangType.L_C),
                CreateLangCommand("C# (CSharp)", LangType.L_CS),
                CreateLangCommand("C++", LangType.L_CPP),
                CreateLangCommand("Caml", LangType.L_CAML),
                CreateLangCommand("CMake", LangType.L_CMAKE),
                CreateLangCommand("COBOL", LangType.L_COBOL),
                CreateLangCommand("CSound", LangType.L_CSOUND),
                CreateLangCommand("CoffeScript", LangType.L_COFFEESCRIPT),
                CreateLangCommand("CSS", LangType.L_CSS),
                CreateLangCommand("D", LangType.L_D),
                CreateLangCommand("Diff", LangType.L_DIFF),
                CreateLangCommand("Erlang", LangType.L_ERLANG),
                CreateLangCommand("ESCRIPT", LangType.L_ESCRIPT),
                CreateLangCommand("Forth", LangType.L_FORTH),
                CreateLangCommand("Fortran (free form)", LangType.L_FORTRAN),
                CreateLangCommand("Fortran (fixed form)", LangType.L_FORTRAN_77),
                CreateLangCommand("Freebasic", LangType.L_FREEBASIC),
                CreateLangCommand("Gui4Cli", LangType.L_GUI4CLI),
                CreateLangCommand("Haskell", LangType.L_HASKELL),
                CreateLangCommand("HTML", LangType.L_HTML),
                CreateLangCommand("INI File", LangType.L_INI),
                CreateLangCommand("Inno Setup", LangType.L_INNO),
                CreateLangCommand("Intel Hex", LangType.L_INTELHEX),
                CreateLangCommand("Java", LangType.L_JAVA),
                CreateLangCommand("JavaScript", LangType.L_JAVASCRIPT),
                CreateLangCommand("JSON", LangType.L_JSON),
                CreateLangCommand("JSP", LangType.L_JSP),
                CreateLangCommand("KIXtart", LangType.L_KIX),
                CreateLangCommand("LISP", LangType.L_LISP),
                CreateLangCommand("LaTeX", LangType.L_LATEX),
                CreateLangCommand("Lua", LangType.L_LUA),
                CreateLangCommand("Makefile", LangType.L_MAKEFILE),
                CreateLangCommand("Matlab", LangType.L_MATLAB),
                CreateLangCommand("MMIXAL", LangType.L_MMIXAL),
                CreateLangCommand("MMIXAL", LangType.L_MMIXAL),
                CreateLangCommand("MS-DOS Style", LangType.L_ASCII),               
                CreateLangCommand("Nimrod", LangType.L_NIMROD),
                CreateLangCommand("Nncrontab", LangType.L_NNCRONTAB),
                CreateLangCommand("Normal Text", LangType.L_TEXT),
                CreateLangCommand("NSIS", LangType.L_NSIS),
                CreateLangCommand("Objective-C", LangType.L_OBJC),
                CreateLangCommand("OScript", LangType.L_OSCRIPT),
                CreateLangCommand("Pascal", LangType.L_PASCAL),
                CreateLangCommand("Perl", LangType.L_PERL),
                CreateLangCommand("PHP", LangType.L_PHP),
                CreateLangCommand("PostScript", LangType.L_PS),
                CreateLangCommand("PowerShell", LangType.L_POWERSHELL),
                CreateLangCommand("Properties", LangType.L_PROPS),
                CreateLangCommand("Purebasic", LangType.L_PUREBASIC),
                CreateLangCommand("Python", LangType.L_PYTHON),
                CreateLangCommand("R", LangType.L_R),
                CreateLangCommand("REBOL", LangType.L_REBOL),
                CreateLangCommand("Registry", LangType.L_REGISTRY),
                CreateLangCommand("Resource file", LangType.L_RC),
                CreateLangCommand("Ruby", LangType.L_RUBY),
                CreateLangCommand("Rust", LangType.L_RUST),
                CreateLangCommand("Shell (Bash)", LangType.L_BASH),
                CreateLangCommand("Scheme", LangType.L_SCHEME),
                CreateLangCommand("Smalltalk", LangType.L_SMALLTALK),
                CreateLangCommand("Spice", LangType.L_SPICE),
                CreateLangCommand("SQL", LangType.L_SQL),
                CreateLangCommand("Swift", LangType.L_SWIFT),
                CreateLangCommand("S-Record", LangType.L_SRECORD),
                CreateLangCommand("TCL", LangType.L_TCL),
                CreateLangCommand("Tektronix extended HEX", LangType.L_TEKTRONIXEXTENDEDHEX),
                CreateLangCommand("TeX", LangType.L_TEX),
                CreateLangCommand("txt2tags", LangType.L_TXT2TAGS),
                CreateLangCommand("Verilog", LangType.L_VERILOG),
                CreateLangCommand("VHDL", LangType.L_VHDL),
                CreateLangCommand("Visual Basic", LangType.L_VB),
                CreateLangCommand("Visual Prolog", LangType.L_VPROLOG),
                CreateLangCommand("XML", LangType.L_XML),
                CreateLangCommand("YAML", LangType.L_YAML)

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
            Regex findRegex = new Regex(charRegex.Replace(filter, match => "(?<between>[^"+ Regex.Escape(match.Value) +"]*)(?<match>" + Regex.Escape(match.Value) + ")") + "(?<end>.*)", RegexOptions.IgnoreCase);

            return commands?.FindAll(command =>
            {
                 Match match = findRegex.Match(command.Name);

                 if (match.Success)
                 {
                     List<Inline> inlines = new List<Inline>();

                     string end = match.Groups["end"].Value;

                     CaptureCollection mcaptures = match.Groups["match"].Captures;
                     CaptureCollection bcaptures = match.Groups["between"].Captures;

                     for (int i = 0; i < mcaptures.Count; i++)
                     {
                        inlines.Add(new Run(bcaptures[i].Value));
                        inlines.Add(new Span(new Run(mcaptures[i].Value))
                         {
                             FontWeight = FontWeights.Bold,
                             Background = Brushes.Yellow,
                         });
                     }

                    if (!string.IsNullOrEmpty(end))
                        inlines.Add(new Run(end));

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
