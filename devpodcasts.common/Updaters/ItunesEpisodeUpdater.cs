using System.Collections.Concurrent;
using System.Xml.Linq;
using devpodcasts.Data.EntityFramework;
using devpodcasts.Domain.Entities;
using devpodcasts.common.Interfaces;
using devpodcasts.common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static devpodcasts.common.Constants.EpisodeConstants;
using devpodcasts.common.Builders;
using devpodcasts.common.Extensions;
using devpodcasts.Domain;
using devpodcasts.Domain.Interfaces;

namespace devpodcasts.common.Updaters;

public class ItunesEpisodeUpdater : IITunesEpisodeUpdater
{
    private readonly ILogger<ItunesEpisodeUpdater> _logger;
    private readonly IItunesHttpClient _itunesHttpClient;
    private readonly ConcurrentDictionary<string, Episode> _episodes = new ConcurrentDictionary<string, Episode>();
    private readonly ConcurrentDictionary<string, ICollection<string>> _episodeTags = new ConcurrentDictionary<string, ICollection<string>>();
    private readonly ConcurrentBag<Tag> _tags = new ConcurrentBag<Tag>();
    private readonly IPodcastRepository _podcastRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IEpisodeRepository _episodeRepository;

    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="dbContextFactory"></param>
    /// <param name="itunesQueryService"></param>
    public ItunesEpisodeUpdater(ILogger<ItunesEpisodeUpdater> logger,
        IItunesHttpClient itunesHttpClient, IEpisodeRepository episodeRepository, IPodcastRepository podcastRepository, ITagRepository tagRepository, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _itunesHttpClient = itunesHttpClient;
        _episodeRepository = episodeRepository;
        _tagRepository = tagRepository;
        _podcastRepository = podcastRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task UpdateDataAsync()
    {
        var allPodcasts = await _unitOfWork.PodcastRepository.GetAllAsync();

        foreach (var podcast in allPodcasts)
        {
            _logger.LogInformation("Updating episodes for podcast: " + podcast.Title);
            IEnumerable<XElement> episodes = await _itunesHttpClient.QueryFeedUrl(podcast.FeedUrl);
            foreach (var episode in episodes)
                await GetEpisodeDataFromXml(episode, podcast);

            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                if (_tags.Any())
                {
                    await _unitOfWork.TagRepository.AddRangeAsync(_tags);
                }
                
                if (_episodes.Values .Any())
                {
                    await _unitOfWork.EpisodeRepository.AddRangeAsync(_episodes.Values);
                }

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                // Roll back the transaction if something goes wrong
                await transaction.RollbackAsync();
                throw; // Rethrow the exception for logging or further handling
            }
            // if (_tags.Any())
            // {
            //     await _unitOfWork.TagRepository.AddRangeAsync(_tags);
            //
            //     try
            //     {
            //         await _unitOfWork.TagRepository.SaveAsync();
            //     }
            //     catch (DbUpdateException ex)
            //     {
            //         _logger.LogError("Could not add tags", ex);
            //     }
            //     
            // }
            //
            // if (_episodes.Values .Any())
            // {
            //     await _unitOfWork.EpisodeRepository.AddRangeAsync(_episodes.Values);
            //     await _unitOfWork.EpisodeRepository.SaveAsync();
            // }
            //

            var dbEpisodes = await _unitOfWork.EpisodeRepository.GetAllAsync(x => x.PodcastId == podcast.Id);

            //var dbEpisodes = await _episodeRepository.GetAllAsync(x => x.PodcastId == podcast.Id);

            await SaveTagsAndEpisodeTags(dbEpisodes, _episodeTags);
        }


      

    }
    private async Task CommitData()
    {
        if (_episodes.Any())
        {
          //  await _context.Episode.AddRangeAsync(_episodes.Values).ConfigureAwait(false);
          //  await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        if (_tags.Any())
        {
          //  await _context.Tag.AddRangeAsync(_tags).ConfigureAwait(false);
           // await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        if (_episodeTags.Any())
        {
           // await SaveTagsAndEpisodeTags(_episodes, _episodeTags).ConfigureAwait(false);
        }
    }
    private async Task SaveTagsAndEpisodeTags(IEnumerable<Episode> episodes, IDictionary<string, ICollection<string>> tagsToMap)
    {
        if (!tagsToMap.Any()) return;
        var updatedTags = new List<EpisodeTag>();
        foreach (var episode in episodes)
        {
           var tagDescription = tagsToMap[episode.Title];

           if (!tagDescription.Any()) return;
           var matchingTags = await _unitOfWork.TagRepository.GetAllAsync(x => tagDescription.Contains(x.Description));

           if (!matchingTags.Any()) return;
      
           episode.Tags.AddRange(matchingTags);
            
           _unitOfWork.EpisodeRepository.Update(episode);
           await _unitOfWork.EpisodeRepository.SaveAsync();
        }
    }
    private async Task GetEpisodeDataFromXml(XElement episode, Podcast podcast)
    {
        IEnumerable<XElement> childElements = episode.Elements().ToList();
        if (childElements.Any())
        {
            var title = childElements.FirstOrDefault(x => x.Name == TitleElementName);

            if (title != null)
            {
                var existingEpisode = await CheckForExistingEpisode(title);
                if (existingEpisode == null)
                {
                    await CreateNewEpisode(title, podcast, childElements);
                }
                else
                {
                    _episodeTags.TryAdd(existingEpisode.Title, new List<string>());
                    _unitOfWork.EpisodeRepository.Update(existingEpisode);
                }
                // if (!await CheckForExistingEpisode(title))
                // {
                //     await CreateNewEpisode(title, podcast, childElements);
                // }
                // else
                // {
                //    // var existingEpisode = await  _episodeRepository.GetAsync(x => x.Title == title.Value);
                //     _episodeTags.TryAdd(existingEpisode.Title, new List<string>());
                // }
            }
                
                    
        }
    }
    private async Task<Episode?> CheckForExistingEpisode(XElement title)
    {
        var existingEpisode = await  _episodeRepository.GetAsync(x => x.Title == title.Value);
        return existingEpisode;
    }

    private async Task CreateNewEpisode(XElement title,
        Podcast podcast, IEnumerable<XElement> childElements)
    {
        _logger.LogInformation("Adding Episode: " + title.Value + ". " + podcast.Id);

        var episode = new EpisodeBuilder();
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

        var newEpisode = episode
            .WithId(Guid.NewGuid())
            .AddTitle(title.Value)
            .AddImageUrl(podcast.ImageUrl)
            .AddAudioTypeAndAudioUrl(enclosure)
            .AddSourceUrl(link)
            .AddPodcast(podcast)
            .AddPublishedDate(publishedDate)
            .AddAuthor(itunesAuthor, author)
            .AddDescription(description, itunesSummary, summary)
            .AddAudioDuration(itunesDuration, duration)
            .Build();

        var tempId = Guid.NewGuid().ToString();

        _episodes.TryAdd(tempId, newEpisode);

        _logger.LogInformation("Added Episode: " + newEpisode.Title);

        var tagsFromXml = GetTagsFromXml(keywords, category);

        _logger.LogInformation("Checking for new tags for episode: " + newEpisode.Title);
        await CreateTags(newEpisode, tagsFromXml, podcast.Tags, tempId).ConfigureAwait(false);
    }
    private ICollection<string> GetTagsFromXml(XElement? keywords, XElement? category)
    {
        var tagsFromXml = new List<string>();

        if (keywords != null)
        {
            var tags = keywords.Value;
            if (!string.IsNullOrEmpty(tags)) tagsFromXml = new List<string>(tags.Split(','));
        }
        else if (category != null)
        {
            var tags = category.Value;
            if (!string.IsNullOrEmpty(tags)) tagsFromXml = new List<string>(tags.Split(','));
        }

        return tagsFromXml;
    }
    private async Task CreateTags(Episode newEpisode, IEnumerable<string> tagsFromXml,
        IEnumerable<Tag> parentTags, string tempId)
    {

        var episodeTags = new List<string>();
        var existingTags = new List<Tag>();
        foreach (var tagDescription in tagsFromXml)
        {
            if (_tags.All(x => x.Description != tagDescription))
            {
                var existingTag = await _tagRepository.GetAsync(x => x.Description == tagDescription); //   _context.Tag.FirstOrDefaultAsync(x => x.Description == tagDescription);
                if (existingTag == null)
                    _tags.Add(new Tag { Description = tagDescription.TrimToMaxLength(50) });
                else
                    existingTags.Add(existingTag);
            }
        }

        _tags.ForEach(tag =>
        {
            var tagExists = CheckForExistingEpisodeTag(newEpisode, tag);
            if (!tagExists && !episodeTags.Contains(tag.Description))
                episodeTags.Add(tag.Description);
        });

        existingTags.ForEach(tag =>
        {
            var tagExists = CheckForExistingEpisodeTag(newEpisode, tag);
            if (!tagExists && !episodeTags.Contains(tag.Description))
                episodeTags.Add(tag.Description);
        });

        var listParentTags = parentTags.ToList();


        var newEpisodeTags = newEpisode.Tags;

        //TODO: Check if same result;
        var result = listParentTags.Where(pt => !newEpisodeTags.Any(x => x.Id == pt.Id)).Select(x => x);

        var parentTagsToUse = from parentTag in listParentTags
                              let exists = newEpisode.Tags.Any(t => t.Id == parentTag.Id)
                              where !exists
                              select parentTag;


        foreach (var parentTag in parentTagsToUse)
        {
            if (parentTag == null) continue;

            var tagExists = CheckForExistingEpisodeTag(newEpisode, parentTag);
            if (!tagExists && !episodeTags.Contains(parentTag.Description))

                episodeTags.Add(parentTag.Description);
        }

        if (_episodes.ContainsKey(newEpisode.Title))
        {
            var conflictTags = _episodeTags[newEpisode.Title];
            
            conflictTags.AddRange(episodeTags);
            _episodeTags[newEpisode.Title].AddRange(conflictTags);
        }
        else
        {
            _episodeTags.TryAdd(newEpisode.Title, episodeTags);
        }

       

        _logger.LogInformation("Saved tags for episode: " + newEpisode.Title);
    }
    private bool CheckForExistingEpisodeTag(Episode episode, Tag tag)
    {
        var tagExists = episode.Tags.Any(t => t?.Description == tag?.Description);
        return tagExists;
    }
}

public interface IITunesEpisodeUpdater : IUpdater
{

}