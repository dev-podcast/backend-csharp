using devpodcasts.Data.EntityFramework;
using devpodcasts.Domain.Entities;
using devpodcasts.common.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Immutable;
using devpodcasts.common.JsonObjects;
using devpodcasts.common.Extensions;

namespace devpodcasts.common.Updaters;

public class BasePodcastUpdater : IBasePodcastUpdater
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BasePodcastUpdater> _logger;

    public BasePodcastUpdater(ILogger<BasePodcastUpdater> logger, IDbContextFactory dbContextFactory)
    {
        _logger = logger;
        _context = dbContextFactory.CreateDbContext();
    }

    private ICollection<BasePodcast> _basePodcasts { get;} = new List<BasePodcast>();
    private ICollection<Tag> _tags { get; } = new List<Tag>();

    public Task UpdateDataAsync()
    {
        return Task.Run(() =>
        {
                var basePodcasts = GetBasePodcastsFromJson();

                var properties = basePodcasts.GetType().GetProperties();
                foreach (var prop in properties)
                {
                    var existingBasePodcasts = _context.BasePodcast.ToImmutableList();
                    var podcastList = (IEnumerable<BasePodcastJsonObject>)prop.GetValue(basePodcasts);
                    var propertyName = prop.Name;

                    _logger.LogInformation("BasePodcast Category: " + propertyName);

                    var basePodcastJsonObjects = FindNonExisting(podcastList, existingBasePodcasts).ToList();
                    if (!basePodcastJsonObjects.Any()) continue;
                    _basePodcasts.AddRange(basePodcastJsonObjects.Select(d => d.CreateBasePodcast()));
                   
                }

                Save().Wait();

                _logger.LogInformation("Updating base podcasts is complete...");
        });
    }

    private RootJsonObject GetBasePodcastsFromJson()
    {
        var podListPath = Environment.CurrentDirectory;
        podListPath = Path.Combine(podListPath, @"PodList/podlist.json");
        var file = File.ReadAllText(podListPath);
        var basePodcasts = JsonConvert.DeserializeObject<RootJsonObject>(file);
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