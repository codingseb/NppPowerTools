using NppPowerTools.PluginInfrastructure;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace NppPowerTools
{
    public sealed class ShowPropertiesViewModel
    {
        private Window propertiesWindow;

        public void ShowPropertiesWindow(object obj)
        {
            if (propertiesWindow == null)
            {
                propertiesWindow = new Window
                {
                    Title = "Inspect Properties",
                    Content = new ShowPropertiesView(),
                };

                propertiesWindow.Show();

                NppTbData nppTbData = new NppTbData
                {
                    hClient = new WindowInteropHelper(propertiesWindow).Handle,
                    pszName = "Inspect Properties",
                    dlgID = 2,
                    uMask = NppTbMsg.DWS_DF_CONT_RIGHT,
                    pszModuleName = Main.PluginName
                };

                IntPtr ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(nppTbData));
                Marshal.StructureToPtr(nppTbData, ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, ptrNppTbData);
            }
            else
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, new WindowInteropHelper(propertiesWindow).Handle);
            }

            propertiesWindow.DataContext = obj;
        }

        #region singleton
        private static ShowPropertiesViewModel instance;

        public static ShowPropertiesViewModel Instance
        {
            get
            {
                return instance ??= new ShowPropertiesViewModel();
            }
        }

        private ShowPropertiesViewModel()
        { }
        #endregion

    }
}
