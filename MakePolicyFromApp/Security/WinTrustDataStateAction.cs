namespace MakePolicyFromApp.Security;

enum WinTrustDataStateAction : uint
{
    Ignore = 0x00000000,
    Verify = 0x00000001,
    Close = 0x00000002,
    AutoCache = 0x00000003,
    AutoCacheFlush = 0x00000004
}
