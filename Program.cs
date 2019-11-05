using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Svetomech.GamesCleaner.Services;

namespace Svetomech.GamesCleaner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<GamesCleanerWorker>();
                });
    }
}
