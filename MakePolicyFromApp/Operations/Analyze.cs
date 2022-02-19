using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MakePolicyFromApp.Services;
using Microsoft.Extensions.Logging;

namespace MakePolicyFromApp.Operations;

class Analyze : IOperation<AnalyzeArguments>
{
    private ILogger<MainService> Logger { get; }
    private IExtractor Extractor { get; }
    private ISignatureVerifier SignatureVerifier { get; }

    public Analyze(ILogger<MainService> logger, IExtractor extractor, ISignatureVerifier signatureVerifier)
    {
        this.Logger = logger;
        this.Extractor = extractor;
        this.SignatureVerifier = signatureVerifier;
    }

    public async Task StartAsync(AnalyzeArguments args)
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

            var executablesAndDlls = FindAllExecutablesAndDlls(rootDirectory);

            var signatures = executablesAndDlls
                .Select((filePath) => SignatureVerifier.VerifySignature(filePath))
                .ToList();

            var table = new ConsoleTables.ConsoleTable("Result", "FileName");
            foreach (Signature signature in signatures)
            {
                table.AddRow(signature.VerificationDetails,
                    Path.GetRelativePath(rootDirectory, signature.FileName));
            }

            Console.WriteLine(table.ToString());
        }
        finally
        {
            Directory.Delete(rootDirectory, true);
        }
    }

    private IList<string> FindAllExecutablesAndDlls(string directory)
    {
        return (new[] { "*.dll", "*.exe" })
            .SelectMany((pattern) => Directory.EnumerateFiles(directory, pattern, SearchOption.AllDirectories))
            .OrderBy((s) => s)
            .ToList();
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
}
