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
    private readonly IReadOnlyList<IExtractor> _extractors;
    private ILogger<MainService> Logger { get; }
    private ISignatureVerifier SignatureVerifier { get; }

    public Analyze(
        ILogger<MainService> logger,
        IEnumerable<IExtractor> extractors,
        ISignatureVerifier signatureVerifier
    )
    {
        _extractors = extractors.ToList();
        this.Logger = logger;
        this.SignatureVerifier = signatureVerifier;
    }

    private IExtractor GetExtractor(string extractor, IReadOnlyList<IExtractor> extractors)
    {
        return extractors.FirstOrDefault(
            (e) => e.Name.Equals(extractor, StringComparison.OrdinalIgnoreCase)
        ) ?? throw new Exception($"There is no extractor with name '{extractor}' defined.");
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

            var extractor = GetExtractor(args.Extractor, _extractors);

            await extractor.ExtractAsync(inputFile, appDirectory).ConfigureAwait(false);

            var executablesAndDlls = FindAllExecutablesAndDlls(rootDirectory);

            var signatures = executablesAndDlls
                .Select((filePath) => SignatureVerifier.VerifySignature(filePath))
                .ToList();

            var table = new ConsoleTables.ConsoleTable("Result", "FileName");
            foreach (Signature signature in signatures)
            {
                table.AddRow(
                    signature.VerificationDetails,
                    Path.GetRelativePath(rootDirectory, signature.FileName)
                );
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
            .SelectMany(
                (pattern) =>
                    Directory.EnumerateFiles(directory, pattern, SearchOption.AllDirectories)
            )
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
