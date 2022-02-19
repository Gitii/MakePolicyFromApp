namespace MakePolicyFromApp.Services;

public readonly struct Signature
{
    public string FileName { get; init; }

    public bool IsValid { get; init; }

    public string VerificationDetails { get; init; }
}
