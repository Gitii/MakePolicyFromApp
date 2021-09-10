using System;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MakePolicyFromApp
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                await CreateHostBuilder(args).Build().RunAsync();

                return 0;
            }
            catch (Exception e)
            {
                throw e;

                return -1;
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.AddHostedService<MainService>();
                    services.AddSingleton<IOperation<GenerateArguments>, Operations.Generate>();
                    services.AddTransient<Services.IExtractor, Services.Extractor>();
                    services.AddTransient<Services.IPolicy, Services.Policy>();
                    services.AddTransient<Services.IPowershell, Services.Powershell>();
                });
        }
    }
}