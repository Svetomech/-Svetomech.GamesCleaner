using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using Serilog;
using Svetomech.GamesCleaner.Services;

namespace Svetomech.GamesCleaner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("USERDOMAIN", "");

            using var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    if (WindowsServiceHelpers.IsWindowsService())
                    {
                        string workingDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                        // Environment.CurrentDirectory = workingDirectory;
                        // Directory.SetCurrentDirectory(workingDirectory);
                        config.SetBasePath(workingDirectory);
                    }
                })
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<GamesCleanerWorker>();
                })
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration));
    }
}
