using DevPodcast.Services.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DevPodcast.Services.Core.Updaters
{
    public abstract class Updater
    {
        protected static IDbContextFactory  DbContextFactory { get; set; }
        protected static ILogger<IUpdater> Logger { get; set; }

        protected Updater(ILogger<IUpdater> logger,IDbContextFactory dbContextFactory)
        {
            DbContextFactory = dbContextFactory;
            Logger = logger;
        }
    }
}