using devpodcasts.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace devpodcasts.Services.Core.Services;

public interface IDatabaseService
{
    Task<bool> CanConnectAsync();
    Task<IEnumerable<string>> GetPendingMigrationsAsync();
    Task MigrateAsync();
}

public class DatabaseService : IDatabaseService
{
    private readonly ApplicationDbContext _context;

    public DatabaseService(ApplicationDbContext context)
    {
        _context = context;
    }
    public Task<bool> CanConnectAsync()
    {
        return _context.Database.CanConnectAsync();
    }

    public Task<IEnumerable<string>> GetPendingMigrationsAsync()
    {
        return _context.Database.GetPendingMigrationsAsync();
    }

    public Task MigrateAsync()
    {
        return _context.Database.MigrateAsync();
    }
}

