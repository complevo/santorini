using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Santorini.Players.Host
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

                    services.AddSingleton<RandomPlayer>();
                })
                .Configure(app =>
                {
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    //app.UseHsts();
                    app.UseMvc()
                       .UseHttpsRedirection();
                })
                .ConfigureLogging(configLogging =>
                {
                    configLogging
                        .AddConsole()
                        .AddDebug()
                        .SetMinimumLevel(LogLevel.Debug);
                })
                .Build();

            await host.RunAsync();
        }
    }
}
