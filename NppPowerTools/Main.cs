﻿// NPP plugin platform for .Net v0.91.57 by Kasper B. Graversen etc.
using Kbg.NppPluginNET.PluginInfrastructure;
using NppPowerTools;
using NppPowerTools.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Kbg.NppPluginNET
{
    class Main
    {
        #region " Fields "

        internal const string PluginName = "NppPowerTools";
        public static readonly string PluginConfigDirectory = Path.Combine(Npp.NotepadPP.GetPluginConfigPath(), PluginName);
        public const string PluginRepository = "https://github.com/codingseb/NppPowerTools";
        private static System.Windows.Window optionsWindow;
        private const string OPTION_WINDOW_TITLE = "Options - Npp Power Tools";

        #endregion

        #region " DllImports "

        //Import the FindWindow API to find our window
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow([MarshalAs(UnmanagedType.LPWStr)] string ClassName, [MarshalAs(UnmanagedType.LPWStr)] string WindowName);

        //Import the SetForeground API to activate it
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int windowLongFlags, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern long SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;

        private enum WindowLongFlags
        {
            GWL_EXSTYLE = -20,
            GWLP_HINSTANCE = -6,
            GWLP_HWNDPARENT = -8,
            GWL_ID = -12,
            GWL_STYLE = -16,
            GWL_USERDATA = -21,
            GWL_WNDPROC = -4,
            DWLP_USER = 0x8,
            DWLP_MSGRESULT = 0x0,
            DWLP_DLGPROC = 0x4,
            WS_EX_LAYERED = 0x80000
        }

        private enum LayeredWindowAttributesFlags : byte
        {
            LWA_COLORKEY = 0x1,
            LWA_ALPHA = 0x2
        }

        #endregion

        #region " Startup/CleanUp "

        static internal void CommandMenuInit()
        {
            // first make it so that all references to any third-party dependencies point to the correct location
            // see https://github.com/oleg-shilo/cs-script.npp/issues/66#issuecomment-1086657272 for more info
            AppDomain.CurrentDomain.AssemblyResolve += LoadDependency;

            NPTCommands.InitCommands();
            NPTCommands.InitLanguages();
            DBConfig.InitDBTypesList();

            // Initialization of your plugin commands

            // with function :
            // SetCommand(int index,                            // zero based number to indicate the order of command
            //            string commandName,                   // the command name that you want to see in plugin menu
            //            NppFuncItemDelegate functionPointer,  // the symbol of function (function pointer) associated with this command. The body should be defined below. See Step 4.
            //            ShortcutKey *shortcut,                // optional. Define a shortcut to trigger this command
            //            bool check0nInit                      // optional. Make this menu item be checked visually
            //            );

            // the "&" before the "D" means that D is an accelerator key for selecting this option 
            int menuIndex = 0;

            PluginBase.SetCommand(menuIndex++, "Commands Panel", () => CommandFindViewModel.Instance.Show(), new ShortcutKey(true, false, true, Keys.P));
            PluginBase.SetCommand(menuIndex++, "---", null);
            PluginBase.SetCommand(menuIndex++, "Expression Execute", () => Evaluation.Process(false), new ShortcutKey(true, false, false, Keys.E));
            PluginBase.SetCommand(menuIndex++, "Script Execute", () => Evaluation.Process(true), new ShortcutKey(true, false, true, Keys.E));
            PluginBase.SetCommand(menuIndex++, "---", null);

            for (int i = 0; i < Config.Instance.ResultOuts.Count; i++)
            {
                int value = i;
                PluginBase.SetCommand(menuIndex++,
                    Config.Instance.ResultOuts[i].Name,
                    () => SetEvaluationOutput(value),
                    new ShortcutKey(false, true, false, Keys.NumPad0 + i),
                    i == Config.Instance.CurrentResultOutIndex);
            }

            PluginBase.SetCommand(menuIndex++, "---", null);

            PluginBase.SetCommand(menuIndex++, "Reset to last Script or Expression", () =>
            {
                Npp.Text = Config.Instance.LastScripts.Count > 0 ? Config.Instance.LastScripts[0] : string.Empty;
                Npp.Editor.DocumentEnd();
            }, new ShortcutKey(true, false, true, Keys.L));

            PluginBase.SetCommand(menuIndex++, "Last Scripts/Expression History", () =>
            {
                CommandFindViewModel.Instance.Show();
                CommandFindViewModel.Instance.Find = "::";
                CommandFindViewModel.Instance.FindSelectionStart = 2;
                CommandFindViewModel.Instance.FindSelectionLength = 0;
            }, new ShortcutKey(true, false, false, Keys.D0));

            for (int i = 0; i < 9; i++)
            {
                int value = i;
                PluginBase.SetCommand(menuIndex++,
                    $"Keep {value + 1} line", () =>
                    {
                        Npp.Text = string.Join("\r\n", Npp.Text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Take(value + 1));
                        Npp.Editor.DocumentEnd();
                    },
                    new ShortcutKey(true, false, false, Keys.D1 + i));
            }

            PluginBase.SetCommand(menuIndex++, "---", null);
            PluginBase.SetCommand(menuIndex++, "Reset variables", () => Evaluation.ResetVariables(), new ShortcutKey(false, true, false, Keys.Delete));
            PluginBase.SetCommand(menuIndex++, "Clear Results", () => EvaluationsResultPanelViewModel.Instance.Results.Clear(), new ShortcutKey(false, true, true, Keys.Delete));
            PluginBase.SetCommand(menuIndex++, "---", null);
            PluginBase.SetCommand(menuIndex++, "Options", ShowOptionWindow, new ShortcutKey(true, false, true, Keys.O));
            PluginBase.SetCommand(menuIndex++, "Show config file directory", () => Process.Start(Path.Combine(new NotepadPPGateway().GetPluginConfigPath(), "NppPowerTools")));
            PluginBase.SetCommand(menuIndex++, "About", () => MessageBox.Show($"Npp Power Tools\r\nVersion : {Assembly.GetExecutingAssembly().GetName().Version}\r\nAuthor : CodingSeb", "About"));
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

        public static void SetEvaluationOutput(int value)
        {
            try
            {
                Npp.NotepadPP.SetPluginMenuChecked(Config.Instance.CurrentResultOutIndex, false);
                Npp.NotepadPP.SetPluginMenuChecked(value, true);
                Config.Instance.CurrentResultOutIndex = value;
                Config.Instance.Save();

                MessageBox.Show($"Result output : \"{Config.Instance.ResultOuts[value].Name}\" selected.", "Info", MessageBoxButtons.OK);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{exception.Message}\r\n{exception.StackTrace}{(exception.InnerException == null ? string.Empty : $"\r\nInner Exception :\r\n{exception.InnerException.Message}\r\n{exception.InnerException.StackTrace}")}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal static void ShowOptionWindow()
        {
            IntPtr hWnd = FindWindow(null, OPTION_WINDOW_TITLE);

            if (hWnd.ToInt64() > 0)
            {
                SetForegroundWindow(hWnd);
            }
            else
            {
                if (optionsWindow == null)
                {
                    optionsWindow = new System.Windows.Window
                    {
                        Title = OPTION_WINDOW_TITLE,
                        MinWidth = 450,
                        MinHeight = 350,
                        Width = 600,
                        Height = 500,
                        Content = new OptionsWindowContent(),
                    };

                    optionsWindow.Closed += OptionsWindow_Closed;
                }
            }

            optionsWindow.Show();

            SetWindowLong(new WindowInteropHelper(optionsWindow).Handle, (int)WindowLongFlags.GWLP_HWNDPARENT, PluginBase.nppData._nppHandle);
            SetLayeredWindowAttributes(new WindowInteropHelper(optionsWindow).Handle, 0, 128, LWA_ALPHA);
        }

        private static void OptionsWindow_Closed(object sender, EventArgs e)
        {
            optionsWindow.Closed -= OptionsWindow_Closed;
            optionsWindow = null;
            Config.Instance.Save();
        }

        #endregion
    }
}   
