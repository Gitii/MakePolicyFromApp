using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CommandLine;
using CommandLine.Text;

namespace MakePolicyFromApp
{
    class MainService : IHostedService
    {
        private ILogger<MainService> logger { get; }
        private IHostApplicationLifetime appLifetime { get; }
        private IOperation<GenerateArguments> generateOperation { get; }
        private IOperation<AnalyzeArguments> analyzeOperation { get; }

        public MainService(ILogger<MainService> logger, IHostApplicationLifetime appLifetime,
            IOperation<GenerateArguments> generateOperation, IOperation<AnalyzeArguments> analyzeOperation)
        {
            this.logger = logger;
            this.appLifetime = appLifetime;
            this.generateOperation = generateOperation;
            this.analyzeOperation = analyzeOperation;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var args = Environment.GetCommandLineArgs().Skip(1).ToArray();

            var result = CommandLine.Parser.Default.ParseArguments<GenerateArguments, AnalyzeArguments>(args);
            var returnCode = await result
                .MapResult<GenerateArguments, AnalyzeArguments, Task<int>>(
                    GenerateAndReturnExitCode,
                    AnalyzeAddAndReturnExitCode,
                    (errs) => Task.FromResult(-1));

            appLifetime.StopApplication();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }

        private async Task<int> GenerateAndReturnExitCode(GenerateArguments args)
        {
            await this.generateOperation.StartAsync(args);

            return 0;
        }

        private async Task<int> AnalyzeAddAndReturnExitCode(AnalyzeArguments args)
        {
            await this.analyzeOperation.StartAsync(args);

            return 0;
        }
    }
}