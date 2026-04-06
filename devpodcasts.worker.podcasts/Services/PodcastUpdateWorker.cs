using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using devpodcasts.common.Interfaces;
using devpodcasts.common.Updaters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace devpodcasts.Worker.Podcasts.Services;

internal class PodcastUpdateWorker(IServiceProvider serviceProvider, ILogger<PodcastUpdateWorker> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<PodcastUpdateWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Podcast Update Worker starting at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var startTime = DateTimeOffset.Now;
                _logger.LogInformation("Starting podcast update cycle at: {time}", startTime);

                using (var scope = _serviceProvider.CreateScope())
                {
                    List<IUpdater> updaters =
                    [
                        scope.ServiceProvider.GetRequiredService<IBasePodcastUpdater>(),
                        scope.ServiceProvider.GetRequiredService<IItunesPodcastUpdater>(),
                        scope.ServiceProvider.GetRequiredService<IITunesEpisodeUpdater>(),
                        scope.ServiceProvider.GetRequiredService<IDataCleaner>()
                    ];

                    foreach (var updater in updaters)
                    {
                        if (stoppingToken.IsCancellationRequested) break;
                        
                        var updaterName = updater.GetType().Name;
                        _logger.LogInformation("Running updater: {updaterType}", updaterName);
                        
                        var updaterStartTime = DateTimeOffset.Now;
                        await updater.UpdateDataAsync();
                        var updaterDuration = DateTimeOffset.Now - updaterStartTime;
                        
                        _logger.LogInformation("Updater {updaterType} completed in {duration}", updaterName, updaterDuration);
                    }
                }

                var cycleDuration = DateTimeOffset.Now - startTime;
                _logger.LogInformation("Podcast update cycle completed successfully at: {time}. Total duration: {duration}", DateTimeOffset.Now, cycleDuration);
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
