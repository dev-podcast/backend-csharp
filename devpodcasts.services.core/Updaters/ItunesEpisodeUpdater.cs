
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using DevPodcast.Data.EntityFramework;
using DevPodcast.Domain.Entities;
using DevPodcast.Services.Core.Interfaces;
using DevPodcast.Services.Core.Updaters.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static DevPodcast.Services.Core.Updaters.Extensions.EpisodeConstants;

namespace DevPodcast.Services.Core.Updaters
{
    public class ItunesEpisodeUpdater : Updater, IUpdater
    {
        public ItunesEpisodeUpdater(ILogger<ItunesEpisodeUpdater> logger, IConfiguration configuration, IDbContextFactory dbContextFactory)
            : base(logger,configuration, dbContextFactory)
        {
        }

        private static ICollection<Episode> episodesToAdd;
        private static ICollection<EpisodeTag> episodeTags;
        private static ICollection<Tag> tagsToAdd;

        public async Task UpdateDataAsync()
        {
            var allPodcasts = await GetPodcasts().ConfigureAwait(false);

            foreach (var podcast in allPodcasts)
            {
                var context = DbContextFactory.CreateDbContext(Configuration);

                podcast.PodcastTags = context.PodcastTag.Where(x => x.PodcastId == podcast.Id).ToList(); 

                 context.Dispose();
               
               Logger.LogInformation("Updating episodes for podcast: " + podcast.Title);
               IEnumerable<XElement> episodes = await QueryService.QueryFeedUrl(podcast.FeedUrl).ConfigureAwait(false);
                foreach (var episode in episodes)
                    await GetEpisodeDataFromXml(episode, podcast).ConfigureAwait(false);

            }
        }

        public static async Task GetEpisodeDataFromXml(XElement episode, Podcast podcast)
        {
            var context = DbContextFactory.CreateDbContext(Configuration);
            IEnumerable<XElement> childElements = episode.Elements().ToList();
            if (childElements.Any())
            {
                var title = childElements.FirstOrDefault(x => x.Name == TitleElementName);

                if (title != null)
                {
                    var existing = context.Episode.FirstOrDefault(x => x.Title == title.Value);

                    if (existing == null)
                    {
                        await CreateNewEpisode(title, podcast, childElements).ConfigureAwait(false);
                    }
                }
            }
        }

        public static Task<IEnumerable<Podcast>> GetPodcasts()
        {
            return Task.Run(() => {

                    var context = DbContextFactory.CreateDbContext(Configuration);
                    var pods = context.Podcast.Where(x => true).ToList();
                    var enumerable = pods.Select(p =>
                    {
                        p.PodcastTags = context.PodcastTag.Where(x => x.PodcastId == p.Id).ToList();
                        return p;
                    });
                    return enumerable;
            });
        }

        public static async Task CreateNewEpisode(XElement title,
            Podcast podcast, IEnumerable<XElement> childElements)
        {
            Logger.LogInformation("Adding Episode: " + title.Value + ". " + podcast.Id);

            var context = DbContextFactory.CreateDbContext(Configuration);

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

            context.Episode.Add(newEpisode);

            await context.SaveChangesAsync().ConfigureAwait(false);
            context.Dispose();

            Logger.LogInformation("Added Episode: " + newEpisode.Title);

            var tagsFromXml = GetTagsFromXml(keywords, category);

            Logger.LogInformation("Checking for new tags for episode: " + newEpisode.Title);
            await CreateTags(newEpisode, tagsFromXml, podcast.PodcastTags).ConfigureAwait(false);
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

        private static async Task CreateTags(Episode newEpisode, IEnumerable<string> tagsFromXml, IEnumerable<PodcastTag> parentTags)
        {
            var newTags = new List<Tag>();
            var context = DbContextFactory.CreateDbContext(Configuration);
           foreach (var tagDescription in tagsFromXml)
           {
               var existingTag = context.Tag.Any(x => x.Description == tagDescription);
               if (!existingTag)
               {
                   var newTag = new Tag() { Description = tagDescription };
                   newTags.Add(newTag);
               }
            }

            await context.Tag.AddRangeAsync(newTags).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            foreach (var tag in newTags)
                newEpisode.EpisodeTags.Add(new EpisodeTag() { EpisodeId = newEpisode.Id, TagId = tag.Id });

            await context.SaveChangesAsync().ConfigureAwait(false);

            var listParentTags = parentTags.ToList();
            if (listParentTags.Any())
            {
                foreach (var parentTag in listParentTags)
                {
                    var exists =
                        newEpisode.EpisodeTags.Any(t => t.TagId == parentTag.TagId);
                    if (!exists)
                        newEpisode.EpisodeTags.Add(new EpisodeTag
                            { EpisodeId = newEpisode.Id, TagId = parentTag.TagId });
                }
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
            context.Dispose();
            Logger.LogInformation("Saved tags for episode: " + newEpisode.Title);
        }
    }
}