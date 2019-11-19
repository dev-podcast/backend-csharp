using DevPodcast.Services.Core.Interfaces;
using DevPodcast.Services.Core.Updaters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
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
            var tasks = new List<Task>();
            foreach (var updater in updaters)
                tasks.Add(updater.UpdateDataAsync());
            
            var allTasks = Task.WhenAll(tasks);
            try
            {
                allTasks.Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }


            if (allTasks.Exception != null)
            {
                _logger.LogError(allTasks.Exception, allTasks.Exception.Message);
                throw allTasks.Exception;

            }
            return allTasks;
        }
    }
}