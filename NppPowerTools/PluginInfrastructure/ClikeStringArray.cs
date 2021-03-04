﻿// NPP plugin platform for .Net v0.94.00 by Kasper B. Graversen etc.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NppPowerTools.PluginInfrastructure
{
    public class ClikeStringArray : IDisposable
    {
        private List<IntPtr> _nativeItems;
        private bool _disposed;

        public ClikeStringArray(int num, int stringCapacity)
        {
            NativePointer = Marshal.AllocHGlobal((num + 1) * IntPtr.Size);
            _nativeItems = new List<IntPtr>();
            for (int i = 0; i < num; i++)
            {
                IntPtr item = Marshal.AllocHGlobal(stringCapacity);
                Marshal.WriteIntPtr(NativePointer + (i * IntPtr.Size), item);
                _nativeItems.Add(item);
            }
            Marshal.WriteIntPtr(NativePointer + (num * IntPtr.Size), IntPtr.Zero);
        }
        public ClikeStringArray(List<string> lstStrings)
        {
            NativePointer = Marshal.AllocHGlobal((lstStrings.Count + 1) * IntPtr.Size);
            _nativeItems = new List<IntPtr>();
            for (int i = 0; i < lstStrings.Count; i++)
            {
                IntPtr item = Marshal.StringToHGlobalUni(lstStrings[i]);
                Marshal.WriteIntPtr(NativePointer + (i * IntPtr.Size), item);
                _nativeItems.Add(item);
            }
            Marshal.WriteIntPtr(NativePointer + (lstStrings.Count * IntPtr.Size), IntPtr.Zero);
        }

        public IntPtr NativePointer { get; }
        public List<string> ManagedStringsAnsi { get { return _getManagedItems(false); } }
        public List<string> ManagedStringsUnicode { get { return _getManagedItems(true); } }
        private List<string> _getManagedItems(bool unicode)
        {
            List<string> _managedItems = new List<string>();
            for (int i = 0; i < _nativeItems.Count; i++)
            {
                if (unicode) _managedItems.Add(Marshal.PtrToStringUni(_nativeItems[i]));
                else _managedItems.Add(Marshal.PtrToStringAnsi(_nativeItems[i]));
            }
            return _managedItems;
        }

        public void Dispose()
        {
                if (!_disposed)
                {
                _disposed = true;
                try
                {
                    for (int i = 0; i < _nativeItems.Count; i++)
                        if (_nativeItems[i] != IntPtr.Zero) Marshal.FreeHGlobal(_nativeItems[i]);
                    if (NativePointer != IntPtr.Zero) Marshal.FreeHGlobal(NativePointer);
                }
            catch { }
            }
        }
        ~ClikeStringArray()
        {
            Dispose();
        }
    }
}