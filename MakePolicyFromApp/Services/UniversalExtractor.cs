using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;

namespace MakePolicyFromApp.Services;

class UniversalExtractor : BaseExtractor
{
    private const string DownloadUrl =
        "https://github.com/Bioruebe/UniExtract2/releases/download/v2.0.0-rc.3/UniExtractRC3.zip";

    public UniversalExtractor(ILogger<MainService> logger) : base(logger)
    {
    }

    protected override string GetExtractorDownloadUrl()
    {
        return DownloadUrl;
    }

    protected override string DisplayName { get; } = "UniversalExtract";

    public override string Name { get; set; } = "universal";

    protected override void SetupStartInfo(ProcessStartInfo startInfo, string archiveFilePath, string outputDirectory)
    {
        startInfo.ArgumentList.Add(archiveFilePath);
        startInfo.ArgumentList.Add(outputDirectory);
    }

    protected override string TranslateExitCodes(int exitCode)
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

    protected override string GetAppPath(string baseDirectory)
    {
        return Path.Join(baseDirectory, "UniExtract", "UniExtract.exe");
    }
}
