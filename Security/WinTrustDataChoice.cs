namespace MakePolicyFromApp.Security;

enum WinTrustDataChoice : uint
{
    File = 1,
    Catalog = 2,
    Blob = 3,
    Signer = 4,
    Certificate = 5
}
