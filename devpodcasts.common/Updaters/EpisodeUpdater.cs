using System.Collections.Concurrent;
using System.Xml.Linq;
using devpodcasts.common.Builders;
using devpodcasts.common.Extensions;
using devpodcasts.common.Interfaces;
using devpodcasts.common.Services;
using devpodcasts.Data.EntityFramework;
using devpodcasts.Domain;
using devpodcasts.Domain.Entities;
using devpodcasts.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using static devpodcasts.common.Constants.EpisodeConstants;
using ExecutionStrategyExtensions = Microsoft.EntityFrameworkCore.ExecutionStrategyExtensions;

namespace devpodcasts.common.Updaters;

public class EpisodeUpdater(
    ILogger<EpisodeUpdater> logger,
    IItunesHttpClient itunesHttpClient,
    IUnitOfWork unitOfWork)
    : IITunesEpisodeUpdater
{
    private ConcurrentBag<Episode> _episodesToAdd = [];

    private ConcurrentBag<Episode> _episodesToUpdate = [];
    // private ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();
    // private List<Tag> _tags = [];

    public Task UpdateDataAsync()
    {
        return BeginUpdateAsync();
    }

    private async Task BeginUpdateAsync()
    {
        logger.LogInformation("Starting episode update process.");
        
        
        var strategy = unitOfWork.CreateExecutionStrategy(); // Get the execution strategy

        await ExecutionStrategyExtensions.ExecuteAsync(strategy, async () =>
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync();

            try
            {
                await ProcessPodcastsAsync();

                if (_episodesToAdd.Any())
                {
                    await unitOfWork.EpisodeRepository.AddRangeAsync(_episodesToAdd);
                    logger.LogInformation($"Added {_episodesToAdd.Count} new episodes.");
                }

                if (_episodesToUpdate.Any())
                {
                    // foreach (var episode in _episodesToUpdate)
                    // {
                    //     unitOfWork.EpisodeRepository.Update(episode);
                    // }

                    logger.LogInformation($"Updated {_episodesToUpdate.Count} existing episodes.");
                }

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitTransactionAsync();

                logger.LogInformation("Episode update process completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError("Error during episode update process.", ex);
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }).ConfigureAwait(false);

     
    }

    private async Task ProcessPodcastsAsync()
    {
        var podcasts = await GetPodcastsAsync();

        foreach (var podcast in podcasts)
        {
            logger.LogInformation($"Updating episodes for podcast: {podcast.Title}");

            var itunesEpisodes = await GetPodcastDataFromItunes(podcast.FeedUrl);

            foreach (var episode in itunesEpisodes)
            {
                await GetEpisodeDataFromXml(episode, podcast);
            }
        }
    }


    private Task<List<Podcast>> GetPodcastsAsync()
    {
        return unitOfWork.PodcastRepository.GetAllAsync();
    }

    private Task<IReadOnlyCollection<XElement>> GetPodcastDataFromItunes(string feedUrl)
    {
        return itunesHttpClient.QueryFeedUrl(feedUrl);
    }

    private async Task GetEpisodeDataFromXml(XElement episode, Podcast podcast)
    {
        var childElements = episode.Elements().ToList();


        if (!childElements.Any())
        {
            logger.LogWarning("Episode XML is missing child elements. Skipping.");
            return;
        }
        
        var titleElement = childElements.FirstOrDefault(x => x.Name == TitleElementName);

        if (titleElement == null || string.IsNullOrEmpty(titleElement.Value))
        {
            logger.LogWarning("Episode XML is missing a title. Skipping.");
            return;
        }

        var title = titleElement.Value;

        if (title != null)
        {
            if (!await CheckForExistingEpisode(title, podcast.Id))
            {
                var newEpisode = CreateNewEpisode(title, podcast, childElements);
                _episodesToAdd.Add(newEpisode);
            }
            else
            {
                var existingEpisode = await unitOfWork.EpisodeRepository.GetAsync(x => x.Title == title && x.PodcastId == podcast.Id);
                if (existingEpisode != null)
                {
                    UpdateExistingEpisode(existingEpisode, childElements);
                    _episodesToUpdate.Add(existingEpisode);
                }
            }
        }
    }

    private async Task<bool> CheckForExistingEpisode(string title, Guid podcastId)
    {
        if (string.IsNullOrEmpty(title))
            throw new InvalidOperationException("Title is null cannot look up existing episode");

        logger.LogError($"Episode title {title}");
        var result = await unitOfWork.EpisodeRepository.GetAsync(x => x.Title == title && x.PodcastId == podcastId);
        return result != null;
    }

    private Episode CreateNewEpisode(string title, Podcast podcast, IEnumerable<XElement> childElements)
    {
        logger.LogInformation("Attempting to create episode: " + title + ". For pdocast:" + podcast.Id);

        var children = childElements.ToList();
        var enclosure = children.FirstOrDefault(x => x.Name == EnclosureElementName);
        var link = children.FirstOrDefault(x => x.Name == LinkElementName);
        var publishedDate = children.FirstOrDefault(x => x.Name == PublishedDateElementName);

        //Author
        var itunesAuthor = children.FirstOrDefault(x => x.Name.LocalName == AuthorElementName);
        var author = children.FirstOrDefault(x => x.Name == AuthorElementName);

        //Description
        var description = children.FirstOrDefault(x => x.Name == DescriptionElementName);
        var itunesSummary = children.FirstOrDefault(x => x.Name.LocalName == SummaryElementName);
        var summary = children.FirstOrDefault(x => x.Name == SummaryElementName);

        //Duration
        var itunesDuration = children.FirstOrDefault(x => x.Name.LocalName == DurationElementName);
        var duration = children.FirstOrDefault(x => x.Name == DurationElementName);

        //Tags
        var keywords = children.FirstOrDefault(x => x.Name.LocalName == KeywordsElementName);
        var category = children.FirstOrDefault(x => x.Name == CategoryElementName);

        var episodeBuilder = new EpisodeBuilder();

        var newEpisode = episodeBuilder
            .WithId(Guid.NewGuid())
            .AddTitle(title)
            .AddImageUrl(podcast.ImageUrl)
            .AddAudioTypeAndAudioUrl(enclosure)
            .AddSourceUrl(link)
            .AddPodcast(podcast)
            .AddPublishedDate(publishedDate)
            .AddAuthor(itunesAuthor, author)
            .AddDescription(description, itunesSummary, summary)
            .AddAudioDuration(itunesDuration, duration)
            .Build();

        newEpisode.Id = Guid.NewGuid();

        return newEpisode;
    }
    
    private void UpdateExistingEpisode(Episode existingEpisode, IEnumerable<XElement> childElements)
    {
        logger.LogInformation($"Updating existing episode: {existingEpisode.Title}");

        var description = childElements.FirstOrDefault(x => x.Name == DescriptionElementName)?.Value;

        if (!string.IsNullOrEmpty(description))
        {
            existingEpisode.Description = description;
        }

        // Add more update logic if necessary, e.g., published date, audio URL, etc.
    }
}