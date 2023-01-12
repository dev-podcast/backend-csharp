using DevPodcast.Data.EntityFramework;
using DevPodcast.Services.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DevPodcast.Services.Core.Updaters
{
    public class DataCleaner : IDataCleaner
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IDataCleaner> _logger;
    
        public DataCleaner(ILogger<IDataCleaner> logger, IDbContextFactory dbContextFactory)
        {
            _logger = logger;
            _context = dbContextFactory.CreateDbContext();
        }

        public Task UpdateDataAsync()
        {
            return Task.Run(async () =>
            {
                _logger.LogInformation("Starting data cleaner...");
                await RemovePodcastsWithoutEpisodes().ConfigureAwait(false);
                _logger.LogInformation("Finished cleaning data");
            });
        }

        private async Task RemovePodcastsWithoutEpisodes()
        {
            //_context = DbContextFactory.CreateDbContext();
            var podcasts = _context.Podcast.Where(p => p.Episodes.Count == 0).ToList();
            _context.Podcast.RemoveRange(podcasts);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

    public interface IDataCleaner : IUpdater
    {

    }
}