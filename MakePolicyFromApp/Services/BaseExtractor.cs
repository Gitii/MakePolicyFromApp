using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MakePolicyFromApp.Services;

abstract class BaseExtractor : IExtractor
{
    private ILogger<MainService> Logger { get; }

    protected abstract string GetExtractorDownloadUrl();

    protected abstract string DisplayName { get; }

    protected BaseExtractor(ILogger<MainService> logger)
    {
        Logger = logger;
    }

    public async Task DownloadAsync(string targetFilePath)
    {
        string downloadUrl = GetExtractorDownloadUrl();

        Logger.LogInformation($"Downloading {downloadUrl}...");
        using var client = new HttpClient();

        Logger.LogInformation($"Writing to {targetFilePath}...");
        await using var stream = await client.GetStreamAsync(downloadUrl).ConfigureAwait(false);
        var output = File.OpenWrite(targetFilePath);
        await using var _ = output.ConfigureAwait(false);
        await stream.CopyToAsync(output).ConfigureAwait(false);
    }

    public Task ExtractToAsync(string sourceZipPath, string targetDirectory)
    {
        ZipFile.ExtractToDirectory(sourceZipPath, targetDirectory);
        return Task.CompletedTask;
    }

    public abstract string Name { get; set; }

    public async Task<string> ExtractAsync(string fileName, string outputDirectory)
    {
        fileName = Path.GetFullPath(fileName);
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException(fileName);
        }

        var directory = GetDownloadDirectory();

        var fullPath = GetAppPath(directory);

        Directory.CreateDirectory(directory);

        if (!File.Exists(fullPath))
        {
            await DownloadAndExtractAsync(directory).ConfigureAwait(false);

            if (!File.Exists(fullPath))
            {
                throw new Exception(
                    $"Download and extracted {DisplayName} but failed to find executable!"
                );
            }
        }

        await ExtractNowAsync(fullPath, fileName, outputDirectory).ConfigureAwait(false);

        Logger.LogDebug($"Extracted to {outputDirectory}");

        return outputDirectory;
    }

    private async Task<string> ExtractNowAsync(
        string extractorFilePath,
        string archiveFilePath,
        string outputDirectory
    )
    {
        Directory.CreateDirectory(outputDirectory);

        try
        {
            var startInfo = new ProcessStartInfo();
            SetupStartInfo(startInfo, archiveFilePath, outputDirectory);

            startInfo.FileName = extractorFilePath;
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;

            var process = Process.Start(startInfo);

            if (process == null)
            {
                throw new Exception(
                    $"Failed to start {DisplayName}: {extractorFilePath}"
                );
            }

            await process.WaitForExitAsync().ConfigureAwait(false);

            if (process.ExitCode != 0)
            {
                var stdErr = await process.StandardError.ReadToEndAsync().ConfigureAwait(false);
                var stdOut = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);

                throw new Exception(
                    $"Failed to extract using {DisplayName} ({TranslateExitCodes(process.ExitCode)}):{Environment.NewLine}{stdErr}{Environment.NewLine}{stdOut}"
                        .Trim()
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

    protected abstract void SetupStartInfo(ProcessStartInfo startInfo, string archiveFilePath, string outputDirectory);

    protected abstract string TranslateExitCodes(int processExitCode);

    private async Task DownloadAndExtractAsync(string directory)
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            Logger.LogDebug($"Downloading {DisplayName} to " + tempFile);
            await DownloadAsync(tempFile).ConfigureAwait(false);

            Logger.LogDebug($"Extracting {DisplayName} to " + tempFile);
            await ExtractToAsync(tempFile, directory).ConfigureAwait(false);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    protected virtual string GetDownloadDirectory()
    {
        return Path.Join(AppDomain.CurrentDomain.BaseDirectory, "deps", Name);
    }

    protected abstract string GetAppPath(string baseDirectory);
}
