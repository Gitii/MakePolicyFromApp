using System;
using System.Runtime.InteropServices;

namespace MakePolicyFromApp.Security;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
record WinTrustData : IDisposable
{
    UInt32 StructSize = (UInt32)Marshal.SizeOf(typeof(WinTrustData));
    IntPtr PolicyCallbackData = IntPtr.Zero;

    IntPtr SIPClientData = IntPtr.Zero;

    // required: UI choice
    WinTrustDataUiChoice UIChoice = WinTrustDataUiChoice.None;

    // required: certificate revocation check options
    WinTrustDataRevocationChecks RevocationChecks = WinTrustDataRevocationChecks.None;

    // required: which structure is being passed in?
    WinTrustDataChoice UnionChoice = WinTrustDataChoice.File;

    // individual file
    IntPtr FileInfoPtr;
    WinTrustDataStateAction StateAction = WinTrustDataStateAction.Ignore;
    IntPtr StateData = IntPtr.Zero;
    String URLReference = null;
    WinTrustDataProvFlags ProvFlags = WinTrustDataProvFlags.RevocationCheckChainExcludeRoot;
    WinTrustDataUiContext UIContext = WinTrustDataUiContext.Execute;

    // constructor for silent WinTrustDataChoice.File check
    public WinTrustData(WinTrustFileInfo fileInfo)
    {
        // On Win7SP1+, don't allow MD2 or MD4 signatures
        if ((Environment.OSVersion.Version.Major > 6) ||
            ((Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor > 1)) ||
            ((Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor == 1) &&
             !String.IsNullOrEmpty(Environment.OSVersion.ServicePack)))
        {
            ProvFlags |= WinTrustDataProvFlags.DisableMd2AndMd4;
        }

        WinTrustFileInfo wtfiData = fileInfo;
        FileInfoPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(WinTrustFileInfo)));
        Marshal.StructureToPtr(wtfiData, FileInfoPtr, false);
    }

    public void Dispose()
    {
        if (FileInfoPtr != IntPtr.Zero)
        {
            Marshal.FreeCoTaskMem(FileInfoPtr);
            FileInfoPtr = IntPtr.Zero;
        }
    }
}
