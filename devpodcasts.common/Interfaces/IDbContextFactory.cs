using devpodcasts.Data.EntityFramework;
using Microsoft.Extensions.Configuration;

namespace devpodcasts.common.Interfaces
{
    public interface IDbContextFactory
    {
        ApplicationDbContext CreateDbContext();
    }
}
