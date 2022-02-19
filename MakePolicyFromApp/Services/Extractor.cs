using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace MakePolicyFromApp.Services;

class Extractor : IExtractor
{
    private const string DownloadUrl =
        "https://github.com/Bioruebe/UniExtract2/releases/download/v2.0.0-rc.3/UniExtractRC3.zip";

    private ILogger<MainService> Logger { get; }

    public Extractor(ILogger<MainService> logger)
    {
        this.Logger = logger;
    }

    public async Task DownloadAsync(string targetFilePath)
    {
        Logger.LogInformation($"Downloading {DownloadUrl}...");
        using var client = new HttpClient();

        Logger.LogInformation($"Writing to {targetFilePath}...");
        await using var stream = await client.GetStreamAsync(DownloadUrl).ConfigureAwait(false);
        var output = File.OpenWrite(targetFilePath);
        await using var _ = output.ConfigureAwait(false);
        await stream.CopyToAsync(output).ConfigureAwait(false);
    }

    public Task ExtractToAsync(string sourceZipPath, string targetDirectory)
    {
        ZipFile.ExtractToDirectory(sourceZipPath, targetDirectory);
        return Task.CompletedTask;
    }

    public async Task<string> ExtractAsync(string fileName, string outputDirectory)
    {
        fileName = Path.GetFullPath(fileName);
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException(fileName);
        }

        var (directory, fullPath) = this.GetAppPath();

        Directory.CreateDirectory(directory);

        if (!File.Exists(fullPath))
        {
            await DownloadAndExtractAsync(directory).ConfigureAwait(false);

            if (!File.Exists(fullPath))
            {
                throw new Exception(
                    "Download and extracted UniExtract2 but failed to find executable!"
                );
            }
        }

        await ExtractNowAsync(fullPath, fileName, outputDirectory).ConfigureAwait(false);

        Logger.LogDebug($"Extracted to {outputDirectory}");

        return outputDirectory;
    }

    private async Task<string> ExtractNowAsync(
        string universalExtractFilePath,
        string archiveFilePath,
        string outputDirectory
    )
    {
        Directory.CreateDirectory(outputDirectory);

        try
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = universalExtractFilePath;
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            // startInfo.ArgumentList.Add("/silent");
            startInfo.ArgumentList.Add(archiveFilePath);
            startInfo.ArgumentList.Add(outputDirectory);
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;

            var process = Process.Start(startInfo);

            if (process == null)
            {
                throw new Exception($"Failed to start UniExtract: {universalExtractFilePath}");
            }

            await process.WaitForExitAsync().ConfigureAwait(false);

            if (process.ExitCode != 0)
            {
                var stdErr = await process.StandardError.ReadToEndAsync().ConfigureAwait(false);
                var stdOut = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);

                throw new Exception(
                    $"Failed to extract using UniExtract ({TranslateExitCodes(process.ExitCode)}):{Environment.NewLine}{stdErr}{Environment.NewLine}{stdOut}".Trim()
                );
            }

            return outputDirectory;
        }
        catch
        {
            Directory.Delete(outputDirectory, true);

            throw;
        }
    }

    private string TranslateExitCodes(int exitCode)
    {
        switch (exitCode)
        {
            case 0:
                return "Success";
            case 3:
                return "STATUS_UNKNOWNEXE";
            case 4:
                return "STATUS_UNKNOWNEXT";
            case 5:
                return "STATUS_INVALIDFILE or STATUS_INVALIDDIR";
            case 6:
                return "STATUS_NOTPACKED";
            case 7:
                return "STATUS_NOTSUPPORTED";
            case 8:
                return "STATUS_MISSINGEXE";
            case 9:
                return "STATUS_TIMEOUT";
            case 10:
                return "STATUS_PASSWORD";
            case 11:
                return "STATUS_MISSINGDEF";
            case 12:
                return "STATUS_MOVEFAILED";
            case 13:
                return "STATUS_NOFREESPACE";
            case 14:
                return "STATUS_MISSINGPART";
            default:
                return $"Unknown exit code {exitCode}";
        }
    }

    private async Task DownloadAndExtractAsync(string directory)
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            Logger.LogDebug("Downloading UniExtract2 to " + tempFile);
            await DownloadAsync(tempFile).ConfigureAwait(false);

            Logger.LogDebug("Extracting UniExtract2 to " + tempFile);
            await ExtractToAsync(tempFile, directory).ConfigureAwait(false);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    private (string directory, string fullFilePath) GetAppPath()
    {
        var dir = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "deps", "UniExtract2");

        return (dir, Path.Join(dir, "UniExtract", "UniExtract.exe"));
    }
}
