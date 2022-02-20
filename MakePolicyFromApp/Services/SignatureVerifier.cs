using MakePolicyFromApp.Security;

namespace MakePolicyFromApp.Services;

class SignatureVerifier : ISignatureVerifier
{
    public Signature VerifySignature(string fileName)
    {
        var (ok, details) = WinTrust.VerifyEmbeddedSignature(fileName);

        return new Signature()
        {
            FileName = fileName,
            IsValid = ok,
            VerificationDetails = details
        };
    }
}
