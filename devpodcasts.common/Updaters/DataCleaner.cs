using devpodcasts.Data.EntityFramework;
using devpodcasts.common.Interfaces;
using Microsoft.Extensions.Logging;

namespace devpodcasts.common.Updaters;

    public class DataCleaner : IDataCleaner
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DataCleaner> _logger;
    
        public DataCleaner(ILogger<DataCleaner> logger, IDbContextFactory dbContextFactory)
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