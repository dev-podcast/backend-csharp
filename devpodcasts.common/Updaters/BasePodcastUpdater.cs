using devpodcasts.Data.EntityFramework;
using devpodcasts.Domain.Entities;
using devpodcasts.common.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Immutable;
using devpodcasts.common.JsonObjects;
using devpodcasts.common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace devpodcasts.common.Updaters;

public class BasePodcastUpdater<TDbContext> where TDbContext : DbContext,  IBasePodcastUpdater
{
    private readonly IDbContextFactory<TDbContext> _contextFactory;
    private readonly ILogger<BasePodcastUpdater<TDbContext>> _logger;

    public BasePodcastUpdater(ILogger<BasePodcastUpdater<TDbContext>> logger, IDbContextFactory<TDbContext> dbContextFactory)
    {
        ArgumentNullException.ThrowIfNull(nameof(dbContextFactory));
        _logger = logger;
        _contextFactory = dbContextFactory;
    }

    private ICollection<BasePodcast> _basePodcasts { get; } = new List<BasePodcast>();
    private ICollection<Tag> _tags { get; } = new List<Tag>();

    public async Task UpdateDataAsync()
    {

        using var dbContext = _contextFactory.CreateDbContext();
        var context = dbContext as ApplicationDbContext;
        var basePodcasts = GetBasePodcastsFromJson();

        if(basePodcasts == null) {
            return; 
        }

        var properties = basePodcasts.GetType().GetProperties();
        foreach (var prop in properties)
        {
            var basePodcastList = prop.GetValue(basePodcasts);
               
            if (basePodcastList != null)
            {
                var existingBasePodcasts = context.BasePodcast.ToImmutableList();
                var jsonObjectList = (IEnumerable<BasePodcastJsonObject>)basePodcastList;
                var propertyName = prop.Name;

                _logger.LogInformation("BasePodcast Category: " + propertyName);

                var basePodcastJsonObjects = FindNonExisting(jsonObjectList, existingBasePodcasts).ToList();
                if (!basePodcastJsonObjects.Any()) continue;
                _basePodcasts.AddRange(basePodcastJsonObjects.Select(d => d.CreateBasePodcast()));
            }       
        }

        Save().Wait();

        _logger.LogInformation("Updating base podcasts is complete...");

    }

    private RootJsonObject? GetBasePodcastsFromJson()
    {
        var podListPath = Environment.CurrentDirectory;
        podListPath = Path.Combine(podListPath, @"PodList/podlist.json");
        var file = File.ReadAllText(podListPath);
        var basePodcasts = JsonConvert.DeserializeObject<RootJsonObject>(file);

        if(basePodcasts == null)
        {
            _logger.LogError("Could not parse base podcast list from json");
            throw new Exception("Could not parse base podcast list from json");
        }

        return basePodcasts;
    }

    private IEnumerable<BasePodcastJsonObject> FindNonExisting(IEnumerable<BasePodcastJsonObject> newPods,
        IEnumerable<BasePodcast> existing)
    {
        var newPodcasts = newPods.ToList();
        var existingPodcasts = existing.ToList();

        var newTitles = newPodcasts
            .Select(x => x.Title.RemovePodcastFromName())
            .Except(existingPodcasts.Select(x => x.Title)).ToList();
        var diff = (from n in newPodcasts
                    join nt in newTitles on n.Title equals nt
                    select n).ToList();
        return diff;
    }

    private async Task Save()
    {
        using var context 
        if (_basePodcasts.Any())
        {
            await _context.BasePodcast.AddRangeAsync(_basePodcasts).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        _context.Dispose();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}


public interface IBasePodcastUpdater : IUpdater
{
}