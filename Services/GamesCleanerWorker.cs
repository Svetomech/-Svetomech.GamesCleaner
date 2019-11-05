using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Svetomech.GamesCleaner.Services
{
    public class GamesCleanerWorker : BackgroundService
    {
        private readonly ILogger<GamesCleanerWorker> _logger;
        private readonly IConfiguration _configuration;

        public GamesCleanerWorker(ILogger<GamesCleanerWorker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            HashSet<string> gamesFoldersWhitelisted = _configuration.GetSection("Whitelist:Folders").Get<HashSet<string>>();
            HashSet<string> gamesFilesWhitelisted = _configuration.GetSection("Whitelist:Files").Get<HashSet<string>>();

            HashSet<string> gamesFilesFoldersWhitelisted = gamesFilesWhitelisted.Select(folder => Path.GetDirectoryName(folder)).ToHashSet();

            string[] gameLaunchersFoldersToClean = gamesFoldersWhitelisted.Select(folder => Path.GetDirectoryName(folder)).ToArray();

            foreach (string gameLauncherFolderToClean in gameLaunchersFoldersToClean)
            {
                string[] gameFoldersToClean = Directory.GetDirectories(gameLauncherFolderToClean);
                foreach (string gameFolderToClean in gameFoldersToClean)
                {
                    if (!gamesFoldersWhitelisted.Contains(gameFolderToClean))
                    {
                        if (!gamesFilesFoldersWhitelisted.Contains(gameFolderToClean))
                        {
                            try
                            {
                                Directory.Delete(gameFolderToClean, true);
                                _logger.LogInformation("Cleaned {0}", gameFolderToClean);
                            }
                            catch
                            {
                                _logger.LogWarning("Had troubles cleaning {0}", gameFolderToClean);
                            }
                        }
                        else
                        {
                            foreach (string gameFileToClean in Directory.GetFiles(gameFolderToClean, "*", SearchOption.AllDirectories))
                            {
                                if (!gamesFilesWhitelisted.Contains(gameFileToClean))
                                {
                                    try
                                    {
                                        File.Delete(gameFileToClean);
                                        _logger.LogInformation("Cleaned {0}", gameFolderToClean);
                                    }
                                    catch
                                    {
                                        _logger.LogWarning("Did NOT clean {0}. Do you have enough rights? Is it currently in use?", gameFolderToClean);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            _logger.LogInformation("Finished! Press any key to exit");
            Console.ReadKey();
            Process.GetCurrentProcess().Kill();

            return Task.CompletedTask;
        }
    }
}