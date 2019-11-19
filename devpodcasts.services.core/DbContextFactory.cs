using DevPodcast.Data.EntityFramework;
using DevPodcast.Services.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DevPodcast.Services.Core
{
    public class DbContextFactory : IDbContextFactory
    {
        public ApplicationDbContext CreateDbContext(IConfiguration configuration)
        {
            if (configuration == null) return null;
            string connString = configuration.GetSection("ConnectionStrings").GetSection("PodcastDb").Value;
            return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connString).Options);
        }
    }
}