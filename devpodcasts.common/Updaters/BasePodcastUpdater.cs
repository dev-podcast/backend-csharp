using devpodcasts.Domain.Entities;
using devpodcasts.common.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using devpodcasts.common.JsonObjects;
using devpodcasts.common.Extensions;
using devpodcasts.Domain;

namespace devpodcasts.common.Updaters;

internal class BasePodcastUpdater(ILogger<BasePodcastUpdater> logger, IUnitOfWork unitOfWork) : IBasePodcastUpdater
{
    private readonly ILogger<BasePodcastUpdater> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    private ICollection<BasePodcast> _basePodcasts { get; } = [];
    private ICollection<Tag> _tags { get; } = [];
    
    
    public async Task UpdateDataAsync()
    {
        var basePodcasts = GetBasePodcastsFromJson();

        if (basePodcasts == null)
        {
            return;
        }

        var existingBasePodcasts = await GetExistingBasePodcastsAsync();
        foreach (var prop in basePodcasts.GetType().GetProperties())
        {
            var basePodcastList = prop.GetValue(basePodcasts);

            if (basePodcastList != null)
            {
                var jsonObjectList = (IEnumerable<BasePodcastJsonObject>)basePodcastList;
                var propertyName = prop.Name;

        _logger.LogInformation("Updating base podcasts for category {category}...", propertyName);
        var basePodcastJsonObjects = FindNonExisting(jsonObjectList, existingBasePodcasts).ToList();
        if (!basePodcastJsonObjects.Any())
        {
            _logger.LogInformation("No new podcasts to add for category {category}.", propertyName);
            continue;
        }

        _logger.LogInformation("Adding {count} new base podcasts for category {category}.", basePodcastJsonObjects.Count, propertyName);
        var basePodcastsToAdd = basePodcastJsonObjects.Select(d => d.CreateBasePodcast()).ToList();
        await AddBasePodcastsAsync(basePodcastsToAdd);
            }
        }

        _logger.LogInformation("Updating base podcasts is complete...");
    }
    
    private async Task AddBasePodcastsAsync(IEnumerable<BasePodcast> basePodcasts)
    {
        await _unitOfWork.BasePodcastRepository.AddRangeAsync(basePodcasts);
        await _unitOfWork.BasePodcastRepository.SaveAsync();
    }
    
    private async Task<List<BasePodcast>> GetExistingBasePodcastsAsync()
    {
        var repo = _unitOfWork.Repository<BasePodcast>();
        return await repo.GetAllAsync();
    }

    private RootJsonObject? GetBasePodcastsFromJson()
    {
        var podListPath = Path.Combine(Environment.CurrentDirectory, @"PodList/podlist.json");
        var file = File.ReadAllText(podListPath);
        var basePodcasts = JsonConvert.DeserializeObject<RootJsonObject>(file);

        if (basePodcasts != null) return basePodcasts;
        _logger.LogError("Could not parse base podcast list from json");
        throw new Exception("Could not parse base podcast list from json");

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
    
}


internal interface IBasePodcastUpdater : IUpdater
{
}