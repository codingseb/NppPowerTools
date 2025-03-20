using Kbg.NppPluginNET;
using Kbg.NppPluginNET.PluginInfrastructure;
using NppPowerTools.Utils;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace NppPowerTools
{
    public sealed class EvaluationsResultPanelViewModel : ViewModelBase
    {
        private Window resultWindow;

        public ObservableCollection<EvaluationResult> Results { get; set; } = new ObservableCollection<EvaluationResult>();

        public ICommand ClearCommand { get; set; }

        public bool ReverseSorting
        {
            get { return Config.Instance.ReverseSortingInResultsWindow; }
            set
            {
                Config.Instance.ReverseSortingInResultsWindow = value;
                Results = new ObservableCollection<EvaluationResult>(Results.Reverse());
            }
        }

        public void ShowResult(object result)
        {
            ShowResultsWindow();

            result ??= Config.Instance.TextWhenResultIsNull;

            if (ReverseSorting)
                Results.Insert(0, new EvaluationResult { Result = result });
            else
                Results.Add(new EvaluationResult { Result = result });
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

                NppTbData nppTbData = new()
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

        #region Singleton and propertyChanged

        private static EvaluationsResultPanelViewModel instance;

        public static EvaluationsResultPanelViewModel Instance => instance ??= new EvaluationsResultPanelViewModel();

        private EvaluationsResultPanelViewModel()
        {
            ClearCommand = new RelayCommand(_ =>
            {
                Results.Clear();
                Evaluation.ResetEvaluator();
            });
        }

        #endregion
    }
}
