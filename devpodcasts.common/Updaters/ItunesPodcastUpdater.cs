using System.Collections.Immutable;
using devpodcasts.Domain.Entities;
using devpodcasts.common.Interfaces;
using devpodcasts.common.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using devpodcasts.common.JsonObjects;
using devpodcasts.common.Extensions;
using devpodcasts.common.Builders;
using devpodcasts.Domain.Interfaces;
namespace devpodcasts.common.Updaters;

internal class ItunesPodcastUpdater(
    ILogger<ItunesPodcastUpdater> logger,
    IPodcastRepository podcastRepository,
    IBasePodcastRepository basePodcastRepository,
    ITagRepository tagRepository,
    ICategoryRepository categoryRepository,
    IItunesHttpClient itunesHttpClient) : IItunesPodcastUpdater
{
    private readonly ICollection<Podcast> _podcasts = [];
    private readonly IDictionary<string, ICollection<string>> _podcastTags = new Dictionary<string, ICollection<string>>();
    private readonly ICollection<Tag> _tags = [];
    private readonly IItunesHttpClient _itunesHttpClient = itunesHttpClient;
    private readonly ILogger<ItunesPodcastUpdater> _logger = logger;
    private readonly IPodcastRepository _podcastRepository = podcastRepository;
    private readonly IBasePodcastRepository _basePodcastRepository = basePodcastRepository;
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task UpdateDataAsync()
    {
        var listOfItunesIds = await _basePodcastRepository.GetAllItunesIdsAsync();
        var existingPodcasts = await _podcastRepository.GetAllAsync();
        var existingItunesIds = existingPodcasts.Select(x => x.ItunesId).ToList();

        // 1. Create new podcasts
        var podcastToCreate = listOfItunesIds.Except(existingItunesIds).ToList();
        foreach (var itunesId in podcastToCreate)
        {
            if (itunesId == null) continue;
            _logger.LogInformation("Creating podcast with id: " + itunesId);
            await CreatePodcastData(itunesId).ConfigureAwait(false);
        }

        // Save new tags
        if (_tags.Any())
        {
            await _tagRepository.AddRangeAsync(_tags);
            await _tagRepository.SaveAsync();
        }

        // Save new podcasts
        if (_podcasts.Any())
        {
            await _podcastRepository.AddRangeAsync(_podcasts);
            await _podcastRepository.SaveAsync();
        }

        // 2. Update existing podcasts (Sync with Itunes)
        _logger.LogInformation("Syncing existing podcasts with Itunes...");
        foreach (var podcast in existingPodcasts)
        {
            try
            {
                var result = await _itunesHttpClient.QueryItunesId(podcast.ItunesId);
                if (result == null || !result.HasValues) continue;

                var podcastResult = result[0].ToObject<PodcastResult>();
                if (podcastResult == null) continue;

                // Update metadata if changed
                bool updated = false;
                if (podcast.EpisodeCount != podcastResult.TrackCount)
                {
                    podcast.EpisodeCount = podcastResult.TrackCount;
                    updated = true;
                }
                if (podcast.LatestReleaseDate != podcastResult.ReleaseDate)
                {
                    podcast.LatestReleaseDate = podcastResult.ReleaseDate;
                    updated = true;
                }
                if (podcast.ImageUrl != podcastResult.ImageUrl600)
                {
                    podcast.ImageUrl = podcastResult.ImageUrl600;
                    updated = true;
                }

                if (updated)
                {
                    _logger.LogInformation("Updating podcast metadata for: {title}", podcast.Title);
                    _podcastRepository.Update(podcast);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating podcast {title}", podcast.Title);
            }
        }
        await _podcastRepository.SaveAsync();

        // Map tags for newly created podcasts
        var newPodIds = _podcasts.Select(p => p.ItunesId).ToList();
        if (newPodIds.Any())
        {
            var podcastsWithTags = await _podcastRepository.GetAllAsync(x => newPodIds.Contains(x.ItunesId));
            foreach (var pod in podcastsWithTags)
            {
                if (!_podcastTags.ContainsKey(pod.Title)) continue;
                var tagDescriptions = _podcastTags[pod.Title];
                var matchingTags = await _tagRepository.GetAllAsync(x => tagDescriptions.Contains(x.Description));

                foreach (var tag in matchingTags)
                {
                    if (!pod.Tags.Any(t => t.Id == tag.Id))
                    {
                        pod.Tags.Add(tag);
                    }
                }
                _podcastRepository.Update(pod);
            }
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

internal interface IItunesPodcastUpdater : IUpdater
{

}