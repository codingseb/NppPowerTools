using NppPowerTools.PluginInfrastructure;
using NppPowerTools.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace NppPowerTools
{
    public class EvaluationsResultPanelViewModel : INotifyPropertyChanged
    {
        private Window resultWindow = null;

        public ObservableCollection<object> Results { get; set; } = new ObservableCollection<object>();

        public ICommand ClearCommand { get; set; }

        public bool ReverseSorting
        {
            get { return Config.Instance.ReverseSortingInResultsWindow; }
            set
            {
                Config.Instance.ReverseSortingInResultsWindow = value;
                Results = new ObservableCollection<object>(Results.Reverse());
            }
        }

        public void ShowResult(object result)
        {
            ShowResultsWindow();

            if (ReverseSorting)
                Results.Insert(0, result);
            else
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

        #region Singleton and propertyChanged

        private static EvaluationsResultPanelViewModel instance = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
