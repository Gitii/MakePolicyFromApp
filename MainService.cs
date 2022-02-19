using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CommandLine;

namespace MakePolicyFromApp;

class MainService : IHostedService
{
    private ILogger<MainService> Logger { get; }
    private IHostApplicationLifetime AppLifetime { get; }
    private IOperation<GenerateArguments> GenerateOperation { get; }
    private IOperation<AnalyzeArguments> AnalyzeOperation { get; }

    public MainService(
        ILogger<MainService> logger,
        IHostApplicationLifetime appLifetime,
        IOperation<GenerateArguments> generateOperation,
        IOperation<AnalyzeArguments> analyzeOperation
    )
    {
        this.Logger = logger;
        this.AppLifetime = appLifetime;
        this.GenerateOperation = generateOperation;
        this.AnalyzeOperation = analyzeOperation;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var args = Environment.GetCommandLineArgs().Skip(1).ToArray();

        var result = CommandLine.Parser.Default.ParseArguments<GenerateArguments, AnalyzeArguments>(
            args
        );
        var returnCode = await result.MapResult<GenerateArguments, AnalyzeArguments, Task<int>>(
            GenerateAndReturnExitCodeAsync,
            AnalyzeAddAndReturnExitCodeAsync,
            (errs) => Task.FromResult(-1)
        ).ConfigureAwait(false);

        AppLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task<int> GenerateAndReturnExitCodeAsync(GenerateArguments args)
    {
        await GenerateOperation.StartAsync(args).ConfigureAwait(false);

        return 0;
    }

    private async Task<int> AnalyzeAddAndReturnExitCodeAsync(AnalyzeArguments args)
    {
        await AnalyzeOperation.StartAsync(args).ConfigureAwait(false);

        return 0;
    }
}
