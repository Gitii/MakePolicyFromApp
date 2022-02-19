using System;
using System.Runtime.InteropServices;

namespace MakePolicyFromApp.Security;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
sealed record WinTrustFileInfo : IDisposable
{
    UInt32 StructSize = (UInt32)Marshal.SizeOf(typeof(WinTrustFileInfo));
    IntPtr pszFilePath; // required, file name to be verified
    IntPtr hFile = IntPtr.Zero; // optional, open handle to FilePath
    IntPtr pgKnownSubject = IntPtr.Zero; // optional, subject type if it is known

    public WinTrustFileInfo(String filePath)
    {
        pszFilePath = Marshal.StringToCoTaskMemAuto(filePath);
    }

    public void Dispose()
    {
        if (pszFilePath != IntPtr.Zero)
        {
            Marshal.FreeCoTaskMem(pszFilePath);
            pszFilePath = IntPtr.Zero;
        }
    }
}
