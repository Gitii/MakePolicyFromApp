namespace MakePolicyFromApp.Security;

enum WinVerifyTrustResult : uint
{
    Success = 0,
    ProviderUnknown = 0x800b0001, // Trust provider is not recognized on this system
    ActionUnknown = 0x800b0002, // Trust provider does not support the specified action
    SubjectFormUnknown = 0x800b0003, // Trust provider does not support the form specified for the subject
    SubjectNotTrusted = 0x800b0004, // Subject failed the specified verification action
    FileNotSigned = 0x800B0100, // TRUST_E_NOSIGNATURE - File was not signed
    SubjectExplicitlyDistrusted = 0x800B0111, // Signer's certificate is in the Untrusted Publishers store
    SignatureOrFileCorrupt = 0x80096010, // TRUST_E_BAD_DIGEST - file was probably corrupt
    SubjectCertExpired = 0x800B0101, // CERT_E_EXPIRED - Signer's certificate was expired
    SubjectCertificateRevoked = 0x800B010C, // CERT_E_REVOKED Subject's certificate was revoked
    UntrustedRoot = 0x800B0109 // CERT_E_UNTRUSTEDROOT - A certification chain processed correctly but terminated in a root certificate that is not trusted by the trust provider.
}
