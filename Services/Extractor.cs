using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MakePolicyFromApp.Services
{
    public interface IExtractor
    {
        public Task<string> Extract(string fileName, string outputDirectory);
    }

    class Extractor : IExtractor
    {
        private const string DOWNLOAD_URL =
            "https://github.com/Bioruebe/UniExtract2/releases/download/v2.0.0-rc.3/UniExtractRC3.zip";

        private ILogger<MainService> logger { get; }

        public Extractor(ILogger<MainService> logger)
        {
            this.logger = logger;
        }

        public async Task Download(string targetFilePath)
        {
            logger.LogInformation($"Downloading {DOWNLOAD_URL}...");
            using var client = new HttpClient();

            logger.LogInformation($"Writing to {targetFilePath}...");
            await using var stream = await client.GetStreamAsync(DOWNLOAD_URL);
            await using var output = File.OpenWrite(targetFilePath);
            await stream.CopyToAsync(output);
        }

        public async Task ExtractTo(string sourceZipPath, string targetDirectory)
        {
            ZipFile.ExtractToDirectory(sourceZipPath, targetDirectory);
        }

        public async Task<string> Extract(string fileName, string outputDirectory)
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
                await DownloadAndExtract(directory);

                if (!File.Exists(fullPath))
                {
                    throw new Exception("Download and extracted UniExtract2 but failed to find executable!");
                }
            }

            await ExtractNow(fullPath, fileName, outputDirectory);

            logger.LogDebug($"Extracted to {outputDirectory}");

            return outputDirectory;
        }

        private async Task<string> ExtractNow(string universalExtractFilePath, string archiveFilePath,
            string outputDirectory)
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

                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    var stdErr = await process.StandardError.ReadToEndAsync();
                    var stdOut = await process.StandardOutput.ReadToEndAsync();

                    throw new Exception(
                        $"Failed to extract using UniExtract ({TranslateExitCodes(process.ExitCode)}):{Environment.NewLine}{stdErr}{Environment.NewLine}{stdOut}"
                            .Trim());
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

        private async Task DownloadAndExtract(string directory)
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                logger.LogDebug("Downloading UniExtract2 to " + tempFile);
                await Download(tempFile);

                logger.LogDebug("Extracting UniExtract2 to " + tempFile);
                await ExtractTo(tempFile, directory);
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
}