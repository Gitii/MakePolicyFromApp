using System;
using System.Runtime.InteropServices;

namespace MakePolicyFromApp.Security;

static class WinTrust
{
    private static readonly IntPtr InvalidHandleValue = new IntPtr(-1);

    // GUID of the action to perform
    private const string WintrustActionGenericVerifyV2 = "{00AAC56B-CD44-11d0-8CC2-00C04FC295EE}";

    [DllImport(
        "wintrust.dll",
        ExactSpelling = true,
        SetLastError = false,
        CharSet = CharSet.Unicode
    )]
    static extern WinVerifyTrustResult WinVerifyTrust(
        [In] IntPtr hwnd,
        [In] [MarshalAs(UnmanagedType.LPStruct)] Guid pgActionId,
        [In] WinTrustData pWvtData
    );

    // call WinTrust.WinVerifyTrust() to check embedded file signature
    public static (bool, string) VerifyEmbeddedSignature(string fileName)
    {
        using var wtfi = new WinTrustFileInfo(fileName);
        using var wtd = new WinTrustData(wtfi);
        Guid guidAction = new Guid(WintrustActionGenericVerifyV2);
        WinVerifyTrustResult result = WinVerifyTrust(InvalidHandleValue, guidAction, wtd);
        return (result == WinVerifyTrustResult.Success, result.ToString());
    }
}
