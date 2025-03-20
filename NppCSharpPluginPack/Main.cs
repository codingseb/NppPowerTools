// NPP plugin platform for .Net v0.91.57 by Kasper B. Graversen etc.
using Kbg.NppPluginNET.PluginInfrastructure;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using static Kbg.NppPluginNET.PluginInfrastructure.Win32;

namespace Kbg.NppPluginNET
{
    class Main
    {
        #region " Fields "
        internal const string PluginName = "NppPowerTools";
        public static readonly string PluginConfigDirectory = Path.Combine(Npp.NotepadPP.GetPluginConfigPath(), PluginName);
        public const string PluginRepository = "https://github.com/codingseb/NppPowerTools";

        #endregion

        #region " Startup/CleanUp "

        static internal void CommandMenuInit()
        {
            // first make it so that all references to any third-party dependencies point to the correct location
            // see https://github.com/oleg-shilo/cs-script.npp/issues/66#issuecomment-1086657272 for more info
            AppDomain.CurrentDomain.AssemblyResolve += LoadDependency;

            // Initialization of your plugin commands

            // with function :
            // SetCommand(int index,                            // zero based number to indicate the order of command
            //            string commandName,                   // the command name that you want to see in plugin menu
            //            NppFuncItemDelegate functionPointer,  // the symbol of function (function pointer) associated with this command. The body should be defined below. See Step 4.
            //            ShortcutKey *shortcut,                // optional. Define a shortcut to trigger this command
            //            bool check0nInit                      // optional. Make this menu item be checked visually
            //            );
            
            // the "&" before the "D" means that D is an accelerator key for selecting this option 
            PluginBase.SetCommand(0, "&Documentation", Docs);
            
            // this inserts a separator
            PluginBase.SetCommand(1, "---", null);
            PluginBase.SetCommand(2, "Hello Notepad++", HelloFX);
            PluginBase.SetCommand(3, "What is Notepad++?", WhatIsNpp);

            PluginBase.SetCommand(4, "---", null);
            PluginBase.SetCommand(5, "Current &Full Path", InsertCurrentFullFilePath);
            PluginBase.SetCommand(6, "Current Directory", InsertCurrentDirectory);

            PluginBase.SetCommand(7, "---", null);
           
            PluginBase.SetCommand(8, "Get File Names Demo", GetFileNamesDemo);
            PluginBase.SetCommand(9, "Print Scroll and Row Information", PrintScrollInformation);
            PluginBase.SetCommand(10, "---", null);
        }

        private static Assembly LoadDependency(object sender, ResolveEventArgs args)
        {
            string assemblyFile = Path.Combine(Npp.PluginDllDirectory, new AssemblyName(args.Name).Name) + ".dll";
            if (File.Exists(assemblyFile))
                return Assembly.LoadFrom(assemblyFile);
            return null;
        }

        public static void OnNotification(ScNotification notification)
        {
            uint code = notification.Header.Code;

            //// changing tabs
            switch (code)
            {
            // when the lexer language changed, re-check whether this is a document where we close HTML tags.
            case (uint)NppMsg.NPPN_LANGCHANGED:
                // Lexer Language changed
                break;
            // the editor color scheme changed, so update form colors
            case (uint)NppMsg.NPPN_WORDSTYLESUPDATED:
                RestyleEverything();
                return;
            case (uint)NppMsg.NPPN_READY:
            case (uint)NppMsg.NPPN_NATIVELANGCHANGED:
                // Npp UI Language changed
                break;
            }
        }

        static internal void PluginCleanUp()
        {
            // dispose of any forms
        }
#endregion

        #region " Menu functions "

        /// <summary>
        /// open GitHub repo with the web browser
        /// </summary>
        private static void Docs()
        {
            OpenUrlInWebBrowser(PluginRepository);
        }

        public static void OpenUrlInWebBrowser(string url)
        {
            try
            {
                var ps = new ProcessStartInfo(url)
                {
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(ps);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format("While attempting to open URL {0} in web browser, got exception\r\n{1}", 2, url, ex),
                    "Could not open url in web browser",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        static void PrintScrollInformation()
        {
            ScrollInfo scrollInfo = Npp.Editor.GetScrollInfo(ScrollInfoMask.SIF_RANGE | ScrollInfoMask.SIF_TRACKPOS | ScrollInfoMask.SIF_PAGE, ScrollInfoBar.SB_VERT);
            var scrollRatio = (double)scrollInfo.nTrackPos / (scrollInfo.nMax - scrollInfo.nPage);
            var scrollPercentage = Math.Min(scrollRatio, 1) * 100;
            Npp.Editor.ReplaceSel($@"The maximum row in the current document was {scrollInfo.nMax + 1}.
                                        A maximum of {scrollInfo.nPage} rows is visible at a time.
                                        The current scroll ratio is {Math.Round(scrollPercentage, 2)}%.
                                        ");
        }

        /// <summary>
        /// open a new file, write a hello world type message, then play with the zoom
        /// </summary>
        static void HelloFX()
        {
            Npp.NotepadPP.FileNew();
            Npp.Editor.SetText("Hello, Notepad++...from.NET!");
            var rest = Npp.Editor.GetLine(0);
            Npp.Editor.SetText(rest + rest + rest);
            new Thread(CallbackHelloFX).Start();
        }

        static void CallbackHelloFX()
        {
            int currentZoomLevel = Npp.Editor.GetZoom();
            int i = currentZoomLevel;
            for (int j = 0; j < 4; j++)
            {
                for (; i >= -10; i--)
                {
                    Npp.Editor.SetZoom(i);
                    Thread.Sleep(30);
                }
                Thread.Sleep(100);
                for (; i <= 20; i++)
                {
                    Thread.Sleep(30);
                    Npp.Editor.SetZoom(i);
                }
                Thread.Sleep(100);
            }
            for (; i >= currentZoomLevel; i--)
            {
                Thread.Sleep(30);
                Npp.Editor.SetZoom(i);
            }
        }

        /// <summary>
        /// open a new buffer and slowly write out the text of text2display in the WhatIsNpp method above.
        /// </summary>
        static void WhatIsNpp()
        {
            string text2display = "Notepad++ is a free (as in \"free speech\" and also as in \"free beer\") " +
                "source code editor and Notepad replacement that supports several languages.\n" +
                "Running in the MS Windows environment, its use is governed by GPL License.\n\n" +
                "Based on a powerful editing component Scintilla, Notepad++ is written in C++ and " +
                "uses pure Win32 API and STL which ensures a higher execution speed and smaller program size.\n" +
                "By optimizing as many routines as possible without losing user friendliness, Notepad++ is trying " +
                "to reduce the world carbon dioxide emissions. When using less CPU power, the PC can throttle down " +
                "and reduce power consumption, resulting in a greener environment.";
            new Thread(new ParameterizedThreadStart(CallbackWhatIsNpp)).Start(text2display);
        }

        static void CallbackWhatIsNpp(object data)
        {
            string text2display = (string)data;
            Npp.NotepadPP.FileNew();
            string newFileName = Npp.NotepadPP.GetCurrentFilePath();

            Random srand = new(DateTime.Now.Millisecond);
            int rangeMin = 0;
            int rangeMax = 250;
            for (int i = 0; i < text2display.Length; i++)
            {
                Thread.Sleep(srand.Next(rangeMin, rangeMax) + 30);
                if (Npp.NotepadPP.GetCurrentFilePath() != newFileName)
                    break;
                Npp.Editor.AppendTextAndMoveCursor(text2display[i].ToString());
            }
        }

        static void InsertCurrentFullFilePath()
        {
            Npp.Editor.ReplaceSel(Npp.NotepadPP.GetCurrentFilePath());
        }

        static void InsertCurrentDirectory()
        {
            Npp.Editor.ReplaceSel(Path.GetDirectoryName(Npp.NotepadPP.GetCurrentFilePath()));
        }


        static void GetFileNamesDemo()
        {
            string[] fileNames = Npp.NotepadPP.GetOpenFileNames();
            MessageBox.Show(fileNames.Length.ToString(), "Number of opened files:");
                    
            foreach (string file in fileNames)
                MessageBox.Show(file);
        }

        //form opening stuff

        /// <summary>
        /// Apply the appropriate styling
        /// (either generic control styling or Notepad++ styling as the case may be)
        /// to all forms.
        /// </summary>
        public static void RestyleEverything()
        {
        }

        #endregion
    }
}   
