using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using DevPodcast.Data.EntityFramework;
using DevPodcast.Domain.Entities;
using DevPodcast.Services.Core.Interfaces;
using DevPodcast.Services.Core.Updaters.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static DevPodcast.Services.Core.Updaters.Extensions.EpisodeConstants;

namespace DevPodcast.Services.Core.Updaters
{
    public class ItunesEpisodeUpdater : Updater, IUpdater
    {
        public ItunesEpisodeUpdater(ILogger<ItunesEpisodeUpdater> logger, IDbContextFactory dbContextFactory)
            : base(logger, dbContextFactory)
        {
            Context = dbContextFactory.CreateDbContext();
        }

        private static readonly IDictionary<string, Episode> Episodes = new Dictionary<string, Episode>();
        private static readonly IDictionary<string, ICollection<string>> EpisodeTags = new Dictionary<string, ICollection<string>>();
        private static readonly ICollection<Tag> Tags = new List<Tag>();     
        private static ApplicationDbContext Context { get; set; }

        public async Task UpdateDataAsync()
        {
            var allPodcasts = await GetPodcasts().ConfigureAwait(false);

            foreach (var podcast in allPodcasts)
            {
                Logger.LogInformation("Updating episodes for podcast: " + podcast.Title);
                IEnumerable<XElement> episodes = await QueryService.QueryFeedUrl(podcast.FeedUrl).ConfigureAwait(false);
                foreach (var episode in episodes)
                    await GetEpisodeDataFromXml(episode, podcast).ConfigureAwait(false);
            }
            CommitData().Wait();

            Dispose();
        }

        private async Task CommitData()
        {
            if (Episodes.Any())
            {
                await Context.Episode.AddRangeAsync(Episodes.Values).ConfigureAwait(false);
                await Context.SaveChangesAsync().ConfigureAwait(false);
            }

            if (Tags.Any())
            {
                await Context.Tag.AddRangeAsync(Tags).ConfigureAwait(false);
                await Context.SaveChangesAsync().ConfigureAwait(false);
            }

            if (EpisodeTags.Any())
            {
                await SaveTagsAndEpisodeTags(Episodes, EpisodeTags).ConfigureAwait(false);
            }
        }

        private static async Task SaveTagsAndEpisodeTags(IDictionary<string, Episode> episodes, IDictionary<string, ICollection<string>> tagsToMap)
        {
            var updatedTags = new List<EpisodeTag>();
            foreach (var keyValue in episodes)
            {
                var tempId = keyValue.Key;
                var episode = keyValue.Value;
                var tagDescription = tagsToMap[tempId];


                var matchingTags = Context.Tag
                    .Where(x => tagDescription.Contains(x.Description)).ToList();

                foreach(var matchingTag in matchingTags)
                    updatedTags.Add(new EpisodeTag(){EpisodeId = episode.Id, TagId = matchingTag.Id});

            }

            await Context.EpisodeTag.AddRangeAsync(updatedTags).ConfigureAwait(false);
            await Context.SaveChangesAsync().ConfigureAwait(false);

        }

        public static async Task GetEpisodeDataFromXml(XElement episode, Podcast podcast)
        {
            IEnumerable<XElement> childElements = episode.Elements().ToList();
            if (childElements.Any())
            {
                var title = childElements.FirstOrDefault(x => x.Name == TitleElementName);

                if (title != null)
                    if (!CheckForExistingEpisode(title))
                        await CreateNewEpisode(title, podcast, childElements).ConfigureAwait(false);
            }
        }

        private static bool CheckForExistingEpisode(XElement title)
        {
            return Context.Episode.Any(x => x.Title == title.Value);
        }

        public static Task<List<Podcast>> GetPodcasts()
        {
            return Task.Run(() =>
            {
                return Context.Podcast
                    .Where(x => true)
                    .Include(p => p.PodcastTags)
                    .Include("PodcastTags.Tag")
                    .ToList(); ;
            });
        }

        public static async Task CreateNewEpisode(XElement title,
            Podcast podcast, IEnumerable<XElement> childElements)
        {
            Logger.LogInformation("Adding Episode: " + title.Value + ". " + podcast.Id);

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

            Episodes.Add(tempId, newEpisode);

            Logger.LogInformation("Added Episode: " + newEpisode.Title);

            var tagsFromXml = GetTagsFromXml(keywords, category);

            Logger.LogInformation("Checking for new tags for episode: " + newEpisode.Title);
            await CreateTags(newEpisode, tagsFromXml, podcast.PodcastTags, tempId).ConfigureAwait(false);
        }

        private static ICollection<string> GetTagsFromXml(XElement keywords, XElement category)
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

        private static Task CreateTags(Episode newEpisode, IEnumerable<string> tagsFromXml,
            IEnumerable<PodcastTag> parentTags, string tempId)
        {
            return Task.Run(() =>
            {

                var episodeTags = new List<string>();
                var existingTags = new List<Tag>();
                foreach (var tagDescription in tagsFromXml)
                {
                    if (Tags.All(x => x.Description != tagDescription))
                    {
                        var existingTag = Context.Tag.FirstOrDefault(x => x.Description == tagDescription);
                        if (existingTag == null)
                            Tags.Add(new Tag {Description = tagDescription});
                        else
                            existingTags.Add(existingTag);
                    }
                }

                Tags.ForEach(tag =>
                {
                    var tagExists = CheckForExistingEpisodeTag(newEpisode, tag);
                    if(!tagExists && !episodeTags.Contains(tag.Description))
                        episodeTags.Add(tag.Description);
                });

                existingTags.ForEach(tag =>
                {
                    var tagExists = CheckForExistingEpisodeTag(newEpisode, tag);
                    if (!tagExists && !episodeTags.Contains(tag.Description))
                        episodeTags.Add(tag.Description);
                });

                var listParentTags = parentTags.ToList();
                foreach (var parentTag in from parentTag
                        in listParentTags
                        let exists = newEpisode.EpisodeTags.Any(t => t.TagId == parentTag.TagId)
                        where !exists
                        select parentTag)
                {
                    var tagExists = CheckForExistingEpisodeTag(newEpisode, parentTag.Tag);
                    if (!tagExists && !episodeTags.Contains(parentTag.Tag?.Description))

                        episodeTags.Add(parentTag?.Tag?.Description);
                }

                EpisodeTags.Add(tempId, episodeTags);

                Logger.LogInformation("Saved tags for episode: " + newEpisode.Title);
            });
        }


        private static bool CheckForExistingEpisodeTag(Episode episode, Tag tag)
        {
            var tagExists = episode.EpisodeTags.Any(t => t.Tag?.Description == tag?.Description);
            return tagExists;
        }


        public void Dispose()
        {
            Context.Dispose();
        }
    }
}