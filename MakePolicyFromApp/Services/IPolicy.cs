using System.Threading.Tasks;

namespace MakePolicyFromApp.Services;

public interface IPolicy
{
    public Task<string> GenerateAsync(string directory);

    Task<string> MakePolicyHumanReadableAsync(
        string policyContent,
        string contextDirectory,
        string contextName
    );
}
