using NppPowerTools.PluginInfrastructure;
using NppPowerTools.Utils;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Interop;

namespace NppPowerTools
{
    internal static class Main
    {
        internal const string PluginName = "Npp Power Tools";

        private static System.Windows.Window optionsWindow = null;
        private const string OPTION_WINDOW_TITLE = "Options - Npp Power Tools";

        private static int outsCommandsIndex;

        //Import the FindWindow API to find our window
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow([MarshalAs(UnmanagedType.LPWStr)] string ClassName, [MarshalAs(UnmanagedType.LPWStr)] string WindowName);

        //Import the SetForeground API to activate it
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int windowLongFlags, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern long SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;

        private enum WindowLongFlags : int
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

        public static void OnNotification(ScNotification notification)
        {

        }

        internal static void CommandMenuInit()
        {
            int menuIndex = 0;
            PluginBase.SetCommand(menuIndex++, "Expression Execute", () => Evaluation.Process(false), new ShortcutKey(true, false, false, Keys.E));
            PluginBase.SetCommand(menuIndex++, "Script Execute", () => Evaluation.Process(true), new ShortcutKey(true, false, true, Keys.E));
            PluginBase.SetCommand(menuIndex++, "---", null);

            outsCommandsIndex = menuIndex;

            for (int i = 0; i < Config.Instance.ResultOuts.Count; i++)
            {
                int value = i;
                PluginBase.SetCommand(i + outsCommandsIndex,
                    Config.Instance.ResultOuts[i].Name,
                    () => SetEvaluationOutput(value),
                    new ShortcutKey(false, true, false, Keys.NumPad0 + i),
                    i == Config.Instance.CurrentResultOutIndex);
            }

            menuIndex += Config.Instance.ResultOuts.Count;

            PluginBase.SetCommand(menuIndex++, "---", null);
            PluginBase.SetCommand(menuIndex++, "Reset variables", () => Evaluation.ResetVariables(), new ShortcutKey(false, true, false, Keys.Delete));
            PluginBase.SetCommand(menuIndex++, "Clear Results", () => EvaluationsResultPanelViewModel.Instance.Results.Clear(), new ShortcutKey(false, true, true, Keys.Delete));
            PluginBase.SetCommand(menuIndex++, "---", null);
            PluginBase.SetCommand(menuIndex++, "Options", ShowOptionWindow, new ShortcutKey(true, false, true, Keys.O));
            PluginBase.SetCommand(menuIndex++, "About", () => MessageBox.Show($"Npp Power Tools\r\nVersion : {Assembly.GetExecutingAssembly().GetName().Version}\r\nAuthor : CodingSeb", "About"));
        }


        public static void SetEvaluationOutput(int value)
        {
            try
            {
                BNpp.NotepadPP.SetPluginMenuChecked(Config.Instance.CurrentResultOutIndex + outsCommandsIndex, false);
                BNpp.NotepadPP.SetPluginMenuChecked(value + outsCommandsIndex, true);
                Config.Instance.CurrentResultOutIndex = value;
                Config.Instance.Save();

                MessageBox.Show($"Result output : \"{Config.Instance.ResultOuts[value].Name }\" selected.", "Info", MessageBoxButtons.OK);
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
                        SizeToContent = System.Windows.SizeToContent.WidthAndHeight,
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
    }
}