﻿using devpodcasts.common.Extensions;
using devpodcasts.common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace devpodcasts.Services.Core
{
    public interface IServiceRunner
    {
        Task RunAsync(ICollection<IUpdater> updaters);
    }

    public class ServiceRunner: IServiceRunner
    {
        private readonly ILogger<ServiceRunner> _logger;


        public ServiceRunner(ILogger<ServiceRunner> logger)
        {
            _logger = logger;
        }

        public Task RunAsync(ICollection<IUpdater> updaters)
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