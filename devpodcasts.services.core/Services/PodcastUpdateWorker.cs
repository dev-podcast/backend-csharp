using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using devpodcasts.common.Interfaces;
using devpodcasts.common.Updaters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace devpodcasts.Services.Core.Services;

public class PodcastUpdateWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PodcastUpdateWorker> _logger;

    public PodcastUpdateWorker(IServiceProvider serviceProvider, ILogger<PodcastUpdateWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Podcast Update Worker starting at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting podcast update cycle at: {time}", DateTimeOffset.Now);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var updaters = new List<IUpdater>
                    {
                        scope.ServiceProvider.GetRequiredService<IBasePodcastUpdater>(),
                        scope.ServiceProvider.GetRequiredService<IItunesPodcastUpdater>(),
                        scope.ServiceProvider.GetRequiredService<IITunesEpisodeUpdater>(),
                        scope.ServiceProvider.GetRequiredService<IDataCleaner>()
                    };

                    foreach (var updater in updaters)
                    {
                        if (stoppingToken.IsCancellationRequested) break;
                        _logger.LogInformation("Running updater: {updaterType}", updater.GetType().Name);
                        await updater.UpdateDataAsync();
                    }
                }

                _logger.LogInformation("Podcast update cycle completed successfully at: {time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the podcast update cycle.");
            }

            _logger.LogInformation("Waiting 24 hours for the next update cycle...");
            try
            {
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Expected when stopping
            }
        }

        _logger.LogInformation("Podcast Update Worker stopping at: {time}", DateTimeOffset.Now);
    }
}
