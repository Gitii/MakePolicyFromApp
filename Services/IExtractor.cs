using System.Threading.Tasks;

namespace MakePolicyFromApp.Services;

public interface IExtractor
{
    public Task<string> ExtractAsync(string fileName, string outputDirectory);
}
