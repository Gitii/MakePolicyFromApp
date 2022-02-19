namespace MakePolicyFromApp.Services;

public interface ISignatureVerifier
{
    public Signature VerifySignature(string fileName);
}
