using devpodcasts.common.Interfaces;

namespace devpodcasts.Worker.Podcasts.Services;

internal interface IServiceRunner
{
    Task RunAsync(ICollection<IUpdater> updaters);
}

internal class ServiceRunner(ILogger<ServiceRunner> logger) : IServiceRunner
{
    private readonly ILogger<ServiceRunner> _logger = logger;


    public async Task RunAsync(ICollection<IUpdater> updaters)
    {
        foreach (var updater in updaters) await updater.UpdateDataAsync();
    }
}