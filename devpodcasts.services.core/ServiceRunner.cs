using DevPodcast.Services.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevPodcast.Services.Core
{
    public class ServiceRunner
    {
        private readonly ILogger<ServiceRunner> _logger;


        public ServiceRunner(ILogger<ServiceRunner> logger)
        {
            _logger = logger;
        }

        public Task Run(ICollection<IUpdater> updaters)
        {
            return Task.Run(() =>
            {
                updaters.ForEach((updater) =>
                {
                    updater.UpdateDataAsync().Wait();
                    updater.Dispose();
                });
            });
        }
    }
}