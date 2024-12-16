using devpodcasts.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using devpodcasts.common.Interfaces;

namespace devpodcasts.common.Factories
{
    public class DbContextFactory : IDbContextFactory
    {
        private IConfiguration _configuration { get; set; }

        public DbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ApplicationDbContext CreateDbContext()
        {
           
                var connString = _configuration.GetSection("ConnectionStrings").GetSection("PodcastDb").Value;
                return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer(connString).Options);
        }
    }
}