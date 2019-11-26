using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using DevPodcast.Services.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DevPodcast.Services.Core.Updaters
{
    public class DataCleaner : Updater, IUpdater
    {
        public DataCleaner(ILogger<DataCleaner> logger, IConfiguration configuration, IDbContextFactory dbContextFactory)
              : base(logger, configuration, dbContextFactory)
        {
        }
        public Task UpdateDataAsync()
        {
            return Task.Run(async () =>
            {
                //Clean up podcasts without episodes
                await RemovePodcastsWithoutEpisodes().ConfigureAwait(false);
            });
        }

        public async Task RemovePodcastsWithoutEpisodes()
        {
            var context  = DbContextFactory.CreateDbContext(Configuration);
            var podcasts = context.Podcast.Where(p => p.Episodes.Count == 0).ToList();
            context.Podcast.RemoveRange(podcasts);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}