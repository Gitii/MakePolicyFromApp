using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MakePolicyFromApp;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        await CreateHostBuilder(args).Build().RunAsync().ConfigureAwait(false);

        return 0;
    }

    static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices(
                (_, services) =>
                {
                    services.AddHostedService<MainService>();
                    services.AddSingleton<IOperation<GenerateArguments>, Operations.Generate>();
                    services.AddSingleton<IOperation<AnalyzeArguments>, Operations.Analyze>();
                    services.AddTransient<Services.IExtractor, Services.UniversalExtractor>();
                    services.AddTransient<Services.IExtractor, Services.InnoSetupExtractor>();
                    services.AddTransient<Services.IPolicy, Services.Policy>();
                    services.AddTransient<Services.IPowershell, Services.Powershell>();
                    services.AddTransient<
                        Services.ISignatureVerifier,
                        Services.SignatureVerifier
                    >();
                }
            );
    }
}
