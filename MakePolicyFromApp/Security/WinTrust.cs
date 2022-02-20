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
        return (result == WinVerifyTrustResult.Success, ToFriendlyDescription(result));
    }

    private static string ToFriendlyDescription(WinVerifyTrustResult result)
    {
        switch (result)
        {
            case WinVerifyTrustResult.Success:
                return "ok: Signature valid";
            case WinVerifyTrustResult.ProviderUnknown:
                return "err: Provider unknown";
            case WinVerifyTrustResult.ActionUnknown:
                return "err: Action unknown";
            case WinVerifyTrustResult.SubjectFormUnknown:
                return "err: subject form unknown";
            case WinVerifyTrustResult.SubjectNotTrusted:
                return "err: subject not trusted";
            case WinVerifyTrustResult.FileNotSigned:
                return "err: file not signed";
            case WinVerifyTrustResult.SubjectExplicitlyDistrusted:
                return "err: subject explicitly distrusted";
            case WinVerifyTrustResult.SignatureOrFileCorrupt:
                return "err: signature or file corrupt";
            case WinVerifyTrustResult.SubjectCertExpired:
                return "err: subject certificate expired";
            case WinVerifyTrustResult.SubjectCertificateRevoked:
                return "err: subject certificate revoked";
            case WinVerifyTrustResult.UntrustedRoot:
                return "err: untrusted root";
            default:
                throw new ArgumentOutOfRangeException(nameof(result), result, null);
        }
    }
}
