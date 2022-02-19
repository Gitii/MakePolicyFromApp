using System;

namespace MakePolicyFromApp.Security;

[Flags]
enum WinTrustDataProvFlags : uint
{
    UseIe4TrustFlag = 0x00000001,
    NoIe4ChainFlag = 0x00000002,
    NoPolicyUsageFlag = 0x00000004,
    RevocationCheckNone = 0x00000010,
    RevocationCheckEndCert = 0x00000020,
    RevocationCheckChain = 0x00000040,
    RevocationCheckChainExcludeRoot = 0x00000080,
    SaferFlag = 0x00000100, // Used by software restriction policies. Should not be used.
    HashOnlyFlag = 0x00000200,
    UseDefaultOsverCheck = 0x00000400,
    LifetimeSigningFlag = 0x00000800,
    CacheOnlyUrlRetrieval = 0x00001000, // affects CRL retrieval and AIA retrieval
    DisableMd2AndMd4 = 0x00002000 // Win7 SP1+: Disallows use of MD2 or MD4 in the chain except for the root
}
