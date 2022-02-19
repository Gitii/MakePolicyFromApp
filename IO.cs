using System.IO;
using System.Threading.Tasks;

namespace MakePolicyFromApp;

static class Io
{
    public static Task CopyToDirectoryAsync(string inputFile, string outputDirectory)
    {
        var outputFile = Path.Join(outputDirectory, Path.GetFileName(inputFile));
        return CopyFileAsync(inputFile, outputFile);
    }

    public static async Task CopyFileAsync(string sourcePath, string destinationPath)
    {
        await using Stream source = File.OpenRead(sourcePath);
        await using Stream destination = File.Create(destinationPath);
        await source.CopyToAsync(destination).ConfigureAwait(false);
    }
}
