using devpodcasts.Data.EntityFramework;
using devpodcasts.common.Interfaces;
using Microsoft.Extensions.Logging;

namespace devpodcasts.common.Updaters;

internal class DataCleaner(ILogger<DataCleaner> logger, ApplicationDbContext context) : IDataCleaner
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<DataCleaner> _logger = logger;

    public async Task UpdateDataAsync()
    {

        _logger.LogInformation("Starting data cleaner...");
        await RemovePodcastsWithoutEpisodes().ConfigureAwait(false);
        _logger.LogInformation("Finished cleaning data");
    }

    private async Task RemovePodcastsWithoutEpisodes()
    {
        var podcasts = _context.Podcast.Where(p => p.Episodes.Count == 0).ToList();
        _context.Podcast.RemoveRange(podcasts);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

internal interface IDataCleaner : IUpdater
{

}