using DevPodcast.Data.EntityFramework;
using DevPodcast.Domain;
using DevPodcast.Services.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevPodcast.Services.Core.Updaters
{
    public abstract class Updater
    {
        protected static IDbContextFactory  DbContextFactory { get; set; }
        protected static IConfiguration Configuration { get; set; }
        protected static ILogger<IUpdater> Logger { get; set; }

        protected Updater(ILogger<IUpdater> logger, IConfiguration configuration,IDbContextFactory dbContextFactory)
        {
            DbContextFactory = dbContextFactory;
            Configuration = configuration;
            Logger = logger;
        }
    }
}