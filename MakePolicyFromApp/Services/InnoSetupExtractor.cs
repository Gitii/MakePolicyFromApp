using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;

namespace MakePolicyFromApp.Services;

class InnoSetupExtractor : BaseExtractor
{
    public InnoSetupExtractor(ILogger<MainService> logger) : base(logger) { }

    protected override string GetExtractorDownloadUrl()
    {
        const string URL =
            "https://github.com/dscharrer/innoextract/releases/download/1.9/innoextract-1.9-windows.zip";

        return URL;
    }

    protected override string DisplayName { get; } = "InnoExtract";
    public override string Name { get; set; } = "innoextract";

    protected override void SetupStartInfo(
        ProcessStartInfo startInfo,
        string archiveFilePath,
        string outputDirectory
    )
    {
        startInfo.ArgumentList.Add("--silent");
        startInfo.ArgumentList.Add("-d");
        startInfo.ArgumentList.Add(outputDirectory);
        startInfo.ArgumentList.Add(archiveFilePath);
    }

    protected override string TranslateExitCodes(int processExitCode)
    {
        return processExitCode.ToString();
    }

    protected override string GetAppPath(string baseDirectory)
    {
        return Path.Join(baseDirectory, "innoextract.exe");
    }
}
