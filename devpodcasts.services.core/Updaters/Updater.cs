using DevPodcast.Data.EntityFramework;
using DevPodcast.Domain;
using DevPodcast.Services.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevPodcast.Services.Core.Updaters
{
    public abstract class Updater
    {
        protected IDbContextFactory DbContextFactory { get; }
        protected ApplicationDbContext Context { get; set; }
        protected IConfiguration Configuration { get; }
        protected ILogger<IUpdater> Logger { get; }

        protected Updater(ILogger<IUpdater> logger, IConfiguration configuration,IDbContextFactory dbContextFactory)
        {
            DbContextFactory = dbContextFactory;
            Configuration = configuration;
            Logger = logger;
        }
    }
}