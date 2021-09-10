using MakePolicyFromApp.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakePolicyFromApp.Operations
{
    class Generate : IOperation<GenerateArguments>
    {
        private ILogger<MainService> logger { get; }
        private IExtractor extractor { get; }
        private IPolicy policy { get; }

        public Generate(ILogger<MainService> logger, IExtractor extractor, IPolicy policy)
        {
            this.logger = logger;
            this.extractor = extractor;
            this.policy = policy;
        }

        public async Task StartAsync(GenerateArguments args)
        {
            logger.LogInformation($"Analyzing file {args.InputFile}...");

            var inputFile = args.InputFile;

            if (!File.Exists(inputFile))
            {
                throw new FileNotFoundException("The input file doesn't exit!", inputFile);
            }

            var (rootDirectory, installerDirectory, appDirectory) = GetOutputDirectory();


            try
            {
                await IO.CopyToDirectoryAsync(args.InputFile, installerDirectory);

                await extractor.Extract(inputFile, appDirectory);

                var generatedPolicy = await this.policy.Generate(rootDirectory);

                var contextName = args.ContextName ?? GetContextNameFromFile(args.InputFile);

                var betterPolicy =
                    await this.policy.MakePolicyHumanReadable(generatedPolicy, rootDirectory, contextName);

                await WriteOutput(betterPolicy, args.OutputFile);
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

        private async Task WriteOutput(string betterPolicy, string? outputFile)
        {
            if (string.IsNullOrEmpty(outputFile))
            {
                Console.WriteLine(betterPolicy);
            }
            else
            {
                await File.WriteAllTextAsync(outputFile, betterPolicy, Encoding.UTF8);
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
}