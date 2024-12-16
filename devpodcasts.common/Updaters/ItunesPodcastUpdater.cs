using System.Collections.Immutable;
using devpodcasts.Domain.Entities;
using devpodcasts.common.Interfaces;
using devpodcasts.common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using devpodcasts.common.JsonObjects;
using devpodcasts.common.Extensions;
using devpodcasts.common.Builders;
using devpodcasts.Domain.Interfaces;
namespace devpodcasts.common.Updaters;

public class ItunesPodcastUpdater : IItunesPodcastUpdater
{
    private readonly ICollection<Podcast> _podcasts = new List<Podcast>();
    private readonly IDictionary<string, ICollection<string>> _podcastTags =
        new Dictionary<string, ICollection<string>>();
    private readonly ICollection<Tag> _tags = new List<Tag>();
    private readonly IItunesHttpClient _itunesHttpClient;
    private readonly ILogger<ItunesPodcastUpdater> _logger;
    private readonly IPodcastRepository _podcastRepository;
    private readonly IBasePodcastRepository _basePodcastRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ICategoryRepository _categoryRepository;


    public ItunesPodcastUpdater(ILogger<ItunesPodcastUpdater> logger,
        IPodcastRepository podcastRepository,IBasePodcastRepository basePodcastRepository, ITagRepository tagRepository, 
        ICategoryRepository categoryRepository,  IItunesHttpClient itunesHttpClient)
    {
        _logger = logger;
        _itunesHttpClient = itunesHttpClient;
        _podcastRepository = podcastRepository;
        _basePodcastRepository = basePodcastRepository;
        _tagRepository = tagRepository;
        _categoryRepository = categoryRepository;
    }


    public async Task UpdateDataAsync()
    {
        var listOfItunesIds = await _basePodcastRepository.GetAllItunesIdsAsync(); 
        var existingPodcasts = await _podcastRepository.GetAllAsync();    // _context.Podcast.Select(x => x.ItunesId).ToList();
        var existingItunesIds = existingPodcasts.Select(x => x.ItunesId);
        var podcastToCreate = listOfItunesIds.Except(existingItunesIds).ToList(); //existingItunesIds.Except(listOfItunesIds).ToList();

        foreach (var itunesId in podcastToCreate)
        {

            if (itunesId == null)
            {
                _logger.LogError("ItunesId was null. Moving to next podcast to create");
                continue;
            }

            _logger.LogInformation("Updating id: " + itunesId);
            await CreatePodcastData(itunesId).ConfigureAwait(false);
        }

        await _tagRepository.AddRangeAsync(_tags);
        await _tagRepository.SaveAsync();


        await _podcastRepository.AddRangeAsync(_podcasts);
        await _podcastRepository.SaveAsync();


        var podcasts = await _podcastRepository.GetAllAsync(x => _podcasts.Select(p => p.ItunesId).Contains(x.ItunesId));

        foreach (var pod in podcasts)
        {
            var tagDescriptions =  _podcastTags[pod.Title];
            var matchingTags = await _tagRepository.GetAllAsync(x => tagDescriptions.Contains(x.Description));

            pod.Tags.AddRange(matchingTags);

            _podcastRepository.Update(pod);
            await _podcastRepository.SaveAsync();


        }

        
    }

    private async Task CreatePodcastData(string itunesId)
    {
        JArray? result = default!;
        try
        {
            result =
            await _itunesHttpClient.QueryItunesId(itunesId);            
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to create podcast");
        }
        
        if (result == null || !result.HasValues) return;

        var podcastResult = result[0].ToObject<PodcastResult>();

        if (podcastResult == null)
        {
            _logger.LogError("Podcast result was null");
            return;
        }

        var trackName = podcastResult.TrackName;
        if (trackName == null) return;

        trackName = trackName.CleanUpTitle();

        var exists = await CheckForExistingPodcast(itunesId);

        if (!exists)
        {
            var basePodcast = await _basePodcastRepository.GetAsync(x => x.ItunesId == itunesId);
            var podcast = await CreatePodcast(itunesId, trackName, podcastResult, basePodcast);

            await CreatePodcastTags(podcast, result).ConfigureAwait(false);
        }
    }

    private async Task<bool> CheckForExistingPodcast(string itunesId)
    {     
       var result = await _podcastRepository.GetAllAsync(x => x.ItunesId == itunesId);   //_context.Podcast.AnyAsync(x => x.ItunesId == itunesId);
       if (result.Any()) return true;
    
       return false;
    }

    private async Task<Podcast> CreatePodcast(string itunesId, string trackName, PodcastResult podcastResult,
        BasePodcast basePodcast)
    {
        _logger.LogInformation("Creating new podcast " + trackName);

        if(string.IsNullOrEmpty(trackName))
        {
            return null;
        }

        var podcast = new PodcastBuilder()
            .WithId(Guid.NewGuid())
            .AddItunesId(itunesId)
            .AddCreatedDate(DateTime.Now)
            .AddTitle(trackName, basePodcast)
            .AddDescription(basePodcast.Description)
            .AddShowUrl(basePodcast.PodcastSite)
            .AddFeedUrl(podcastResult.FeedUrl)
            .AddImageUrl(podcastResult.ImageUrl600)
            .AddEpisodeCount(podcastResult.TrackCount)
            .AddCountry(podcastResult.Country)
            .AddArtists(podcastResult.Artists)
            .AddLatestReleaseDate(podcastResult.ReleaseDate)
            .Build();

        _podcasts.Add(podcast);

        await _podcastRepository.SaveAsync();
        return podcast;
    }

    private async Task CreatePodcastTags(Podcast podcast, JArray result)
    {
        if (podcast == null) return;
        var podcastTags = new List<string>();
        var existingTags = new List<Tag>();

        _logger.LogInformation("Creating podcast tags");

        foreach (var genreResult in result)
        {
            dynamic genres = genreResult;
            JArray data = genres.genres;
            if(data != null && data.Any())
            {
                var listGenres = data.ToList();
                if (listGenres.Any())
                    foreach (var genre in listGenres)
                    {
                        _logger.LogInformation($"Added tag/genre: {genre}");
                        var tagDescription = genre.Value<string>();

                        if (_tags.All(x => x.Description != tagDescription))
                        {
                            var existingTag = await _tagRepository.GetAsync(x => !string.IsNullOrEmpty(tagDescription) &&
                            tagDescription.Equals(x.Description));
                           
                            if (existingTag == null)
                                _tags.Add(new Tag { Description = tagDescription });
                            else
                                existingTags.Add(existingTag);
                        }

                    }
            }       
        }

        _tags.ForEach(tag =>
        {
            var tagExists = CheckForExstingPodcastTag(podcast, tag);
            if (!tagExists && !podcastTags.Contains(tag.Description))
                podcastTags.Add(tag.Description);
        });

        existingTags.ForEach(tag =>
        {
            var tagExists = CheckForExstingPodcastTag(podcast, tag);
            if (!tagExists && !podcastTags.Contains(tag.Description))
                podcastTags.Add(tag.Description);
        });


        _podcastTags.Add(podcast.Title, podcastTags);



        _logger.LogInformation("Saved tags for podcast: " + podcast.Title);

    }

    private bool CheckForExstingPodcastTag(Podcast podcast, Tag tag)
    {
        var tagExists = podcast.Tags.Any(t =>
            t?.Description == tag?.Description);
        return tagExists;
    }
}

public interface IItunesPodcastUpdater : IUpdater
{

}