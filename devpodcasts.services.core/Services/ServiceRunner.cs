using System.Collections.Generic;
using System.Threading.Tasks;
using devpodcasts.common.Interfaces;
using Microsoft.Extensions.Logging;

namespace devpodcasts.Services.Core.Services;

public interface IServiceRunner
{
    Task RunAsync(ICollection<IUpdater> updaters);
}

public class ServiceRunner : IServiceRunner
{
    private readonly ILogger<ServiceRunner> _logger;


    public ServiceRunner(ILogger<ServiceRunner> logger)
    {
        _logger = logger;
    }

    public async Task RunAsync(ICollection<IUpdater> updaters)
    {
        foreach (var updater in updaters) await updater.UpdateDataAsync();
    }
}