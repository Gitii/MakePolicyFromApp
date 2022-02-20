using System.Threading.Tasks;

namespace MakePolicyFromApp.Services;

public interface IExtractor
{
    public string Name { get; set; }

    public Task<string> ExtractAsync(string fileName, string outputDirectory);
}
