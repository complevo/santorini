using System.IO;
using System.Threading.Tasks;
using Flurl.Http.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Santorini.Host
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("hostsettings.json", optional: true);
                    configHost.AddEnvironmentVariables(prefix: "PREFIX_");
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddJsonFile("appsettings.json", optional: true)
                        .AddJsonFile(
                            $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                            optional: true)
                        .AddEnvironmentVariables(prefix: "PREFIX_")
                        .AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddHostedService<SantoriniGameHostedService>()
                        .AddSingleton<IFlurlClientFactory, PerHostFlurlClientFactory>()
                        .AddSingleton<IGameService, GameService>()
                        .Configure<GameSettings>(GameSettings.Section, hostContext.Configuration);
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                })
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
        }
    }
}
