using DevPodcast.Data.EntityFramework;
using DevPodcast.Services.Core.Interfaces;
using Kralizek.Extensions.Configuration.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DevPodcast.Services.Core
{
    public class DbContextFactory : IDbContextFactory
    {
        public ApplicationDbContext CreateDbContext(IConfiguration configuration)
        {
            if (configuration == null) return null;
            string connStringKey = configuration.GetSection("ConnectionStrings").GetSection("PodcastDb").Value;
            var connString = configuration.GetSection(connStringKey).GetValue<string>(connStringKey);
            return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connString).Options);
        }
    }
}