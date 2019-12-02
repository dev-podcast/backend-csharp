using DevPodcast.Data.EntityFramework;
using DevPodcast.Services.Core.Interfaces;
using Kralizek.Extensions.Configuration.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DevPodcast.Services.Core
{
    public class DbContextFactory : IDbContextFactory
    {
        public IConfiguration Configuration { get; set; }

        public DbContextFactory(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public ApplicationDbContext CreateDbContext()
        {
            if (Configuration == null) return null;
            string connStringKey = Configuration.GetSection("ConnectionStrings").GetSection("PodcastDb").Value;
            var connString = Configuration.GetSection(connStringKey).GetValue<string>(connStringKey);
            return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connString).Options);
        }
    }
}