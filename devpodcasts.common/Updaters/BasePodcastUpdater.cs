using devpodcasts.Data.EntityFramework;
using devpodcasts.Domain.Entities;
using devpodcasts.common.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Immutable;
using devpodcasts.common.JsonObjects;
using devpodcasts.common.Extensions;
using devpodcasts.Domain;
using Microsoft.EntityFrameworkCore;

namespace devpodcasts.common.Updaters;

public class BasePodcastUpdater : IBasePodcastUpdater
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<BasePodcastUpdater> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public BasePodcastUpdater(ILogger<BasePodcastUpdater> logger, IDbContextFactory<ApplicationDbContext> dbContextFactory, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _contextFactory = dbContextFactory;
        _unitOfWork = unitOfWork;
    }

    private ICollection<BasePodcast> _basePodcasts { get; } = new List<BasePodcast>();
    private ICollection<Tag> _tags { get; } = new List<Tag>();
    
    
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

                _logger.LogInformation("BasePodcast Category: " + propertyName);

                var basePodcastJsonObjects = FindNonExisting(jsonObjectList, existingBasePodcasts).ToList();
                if (!basePodcastJsonObjects.Any()) continue;

                var basePodcastsToAdd = basePodcastJsonObjects.Select(d => d.CreateBasePodcast()).ToList();
                await AddBasePodcastsAsync(basePodcastsToAdd);
            }
        }

        _logger.LogInformation("Updating base podcasts is complete...");
    }

    // public async Task UpdateDataAsync()
    // {
    //
    //     var basePodcasts = GetBasePodcastsFromJson();
    //
    //     if(basePodcasts == null) 
    //     {
    //         return; 
    //     }
    //
    //     var properties = basePodcasts.GetType().GetProperties();
    //     foreach (var prop in properties)
    //     {
    //         var basePodcastList = prop.GetValue(basePodcasts);
    //            
    //         if (basePodcastList != null)
    //         {
    //             var existingBasePodcasts = _context.BasePodcast.ToImmutableList();
    //             var jsonObjectList = (IEnumerable<BasePodcastJsonObject>)basePodcastList;
    //             var propertyName = prop.Name;
    //
    //             _logger.LogInformation("BasePodcast Category: " + propertyName);
    //
    //             var basePodcastJsonObjects = FindNonExisting(jsonObjectList, existingBasePodcasts).ToList();
    //             if (!basePodcastJsonObjects.Any()) continue;
    //             _basePodcasts.AddRange(basePodcastJsonObjects.Select(d => d.CreateBasePodcast()));
    //         }       
    //     }
    //
    //     Save().Wait();
    //
    //     _logger.LogInformation("Updating base podcasts is complete...");
    //
    // }
    
    private async Task AddBasePodcastsAsync(IEnumerable<BasePodcast> basePodcasts)
    {
        // using var context = _contextFactory.CreateDbContext();
        // await context.BasePodcast.AddRangeAsync(basePodcasts);
        // await context.SaveChangesAsync();
        await _unitOfWork.BasePodcastRepository.AddRangeAsync(basePodcasts);
        await _unitOfWork.BasePodcastRepository.SaveAsync();
    }
    
    private async Task<List<BasePodcast>> GetExistingBasePodcastsAsync()
    {
      //  using var context = _contextFactory.CreateDbContext();
      var repo = _unitOfWork.Repository<BasePodcast>();


        return await repo.GetAllAsync();
        //return await context.BasePodcast.ToListAsync();
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


public interface IBasePodcastUpdater : IUpdater
{
}