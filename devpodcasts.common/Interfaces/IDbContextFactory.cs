using devpodcasts.Data.EntityFramework;

namespace devpodcasts.common.Interfaces
{
    internal interface IDbContextFactory
    {
        ApplicationDbContext CreateDbContext();
        
    }
}
