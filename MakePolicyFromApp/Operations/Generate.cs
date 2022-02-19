using MakePolicyFromApp.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MakePolicyFromApp.Operations;

class Generate : IOperation<GenerateArguments>
{
    private ILogger<MainService> Logger { get; }
    private IExtractor Extractor { get; }
    private IPolicy Policy { get; }

    public Generate(ILogger<MainService> logger, IExtractor extractor, IPolicy policy)
    {
        this.Logger = logger;
        this.Extractor = extractor;
        this.Policy = policy;
    }

    public async Task StartAsync(GenerateArguments args)
    {
        Logger.LogInformation($"Analyzing file {args.InputFile}...");

        var inputFile = args.InputFile;

        if (!File.Exists(inputFile))
        {
            throw new FileNotFoundException("The input file doesn't exit!", inputFile);
        }

        var (rootDirectory, installerDirectory, appDirectory) = GetOutputDirectory();


        try
        {
            await Io.CopyToDirectoryAsync(args.InputFile, installerDirectory).ConfigureAwait(false);

            await Extractor.ExtractAsync(inputFile, appDirectory).ConfigureAwait(false);

            var generatedPolicy = await this.Policy.GenerateAsync(rootDirectory).ConfigureAwait(false);

            var contextName = args.ContextName ?? GetContextNameFromFile(args.InputFile);

            var betterPolicy =
                await this.Policy.MakePolicyHumanReadableAsync(generatedPolicy, rootDirectory, contextName)
                    .ConfigureAwait(false);

            await WriteOutputAsync(betterPolicy, args.OutputFile).ConfigureAwait(false);
        }
        finally
        {
            Directory.Delete(rootDirectory, true);
        }
    }

    private (string rootDirectory, string installerDirectory, string appDirectory) GetOutputDirectory()
    {
        var root = Path.Join(Path.GetTempPath(), (Guid.NewGuid()).ToString());
        var installer = Path.Join(root, "installer");
        var app = Path.Join(root, "app");

        Directory.CreateDirectory(root);
        Directory.CreateDirectory(installer);
        Directory.CreateDirectory(app);

        return (root, installer, app);
    }

    private async Task WriteOutputAsync(string betterPolicy, string? outputFile)
    {
        if (string.IsNullOrEmpty(outputFile))
        {
            Console.WriteLine(betterPolicy);
        }
        else
        {
            await File.WriteAllTextAsync(outputFile, betterPolicy, Encoding.UTF8).ConfigureAwait(false);
        }
    }

    private string GetContextNameFromFile(string filePath)
    {
        var info = FileVersionInfo.GetVersionInfo(filePath);

        var name = Path.GetFileName(filePath);

        if (!string.IsNullOrEmpty(info.ProductName))
        {
            name = info.ProductName;
        }

        if (!string.IsNullOrEmpty(info.ProductVersion))
        {
            name += $" ({info.ProductVersion})";
        }
        else if (!string.IsNullOrEmpty(info.FileVersion))
        {
            name += $" ({info.ProductVersion})";
        }

        return name;
    }
}
