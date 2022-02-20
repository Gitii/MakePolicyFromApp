using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
        Logger = logger;
        AppLifetime = appLifetime;
        GenerateOperation = generateOperation;
        AnalyzeOperation = analyzeOperation;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var args = Environment.GetCommandLineArgs().Skip(1).ToArray();

        var rc = new RootCommand();

        var analyzeCommand = AnalyzeArguments.CreateCommand();
        analyzeCommand.Handler = CommandHandler.Create(AnalyzeAddAndReturnExitCodeAsync);
        rc.AddCommand(analyzeCommand);

        var generateCommand = GenerateArguments.CreateCommand();
        generateCommand.Handler = CommandHandler.Create(GenerateAndReturnExitCodeAsync);
        rc.AddCommand(generateCommand);

        await rc.InvokeAsync(args).ConfigureAwait(false);

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
