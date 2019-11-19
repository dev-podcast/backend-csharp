using DevPodcast.Data.EntityFramework;
using Microsoft.Extensions.Configuration;

namespace DevPodcast.Services.Core.Interfaces
{
    public interface IDbContextFactory
    {
        ApplicationDbContext CreateDbContext(IConfiguration configuration);
    }
}