using DevPodcast.Data.EntityFramework;
using DevPodcast.Services.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DevPodcast.Services.Core.Updaters
{
    public class DataCleaner : Updater, IUpdater
    {
        public DataCleaner(ILogger<DataCleaner> logger, IDbContextFactory dbContextFactory)
              : base(logger, dbContextFactory)
        {
            Context = dbContextFactory.CreateDbContext();
        }

        public ApplicationDbContext Context { get; set; }
        public Task UpdateDataAsync()
        {
            return Task.Run(async () =>
            {
                await RemovePodcastsWithoutEpisodes().ConfigureAwait(false);
            });
        }

        public async Task RemovePodcastsWithoutEpisodes()
        {
             Context = DbContextFactory.CreateDbContext();
            var podcasts = Context.Podcast.Where(p => p.Episodes.Count == 0).ToList();
            Context.Podcast.RemoveRange(podcasts);
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}