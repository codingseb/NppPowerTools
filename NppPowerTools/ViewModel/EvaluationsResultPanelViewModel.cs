using NppPowerTools.PluginInfrastructure;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace NppPowerTools
{
    public class EvaluationsResultPanelViewModel
    {
        private Window resultWindow = null;

        public ObservableCollection<string> Results { get; set; } = new ObservableCollection<string>();

        public ICommand ClearCommand { get; set; }

        public void ShowResult(string result)
        {
            ShowResultsWindow();

            Results.Add(result);
        }

        public void ShowResultsWindow()
        {
            if (resultWindow == null)
            {
                resultWindow = new Window
                {
                    Title = "Evaluations Results",
                    Content = new EvaluationsResultsPanel(),
                    DataContext = this,
                };

                resultWindow.Show();

                NppTbData nppTbData = new NppTbData
                {
                    hClient = new WindowInteropHelper(resultWindow).Handle,
                    pszName = "Evaluations Results",
                    dlgID = 1,
                    uMask = NppTbMsg.DWS_DF_CONT_BOTTOM,
                    pszModuleName = Main.PluginName
                };

                IntPtr ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(nppTbData));
                Marshal.StructureToPtr(nppTbData, ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, ptrNppTbData);
            }
            else
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, new WindowInteropHelper(resultWindow).Handle);
            }
        }

        #region Singleton

        private static EvaluationsResultPanelViewModel instance = null;

        public static EvaluationsResultPanelViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EvaluationsResultPanelViewModel();
                }

                return instance;
            }
        }

        private EvaluationsResultPanelViewModel()
        {
            ClearCommand = new RelayCommand(_ => Results.Clear());
        }

        #endregion
    }
}
