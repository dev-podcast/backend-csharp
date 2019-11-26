using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using DevPodcast.Data.EntityFramework;
using DevPodcast.Domain.Entities;
using DevPodcast.Services.Core.Interfaces;
using DevPodcast.Services.Core.JsonObjects;
using DevPodcast.Services.Core.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevPodcast.Services.Core.Updaters
{
    internal class ItunesPodcastUpdater : Updater, IUpdater
    {
        private readonly ICollection<Podcast> _podcasts = new List<Podcast>();
        private readonly ICollection<PodcastTag> _podcastTags = new List<PodcastTag>();
        private readonly ICollection<Tag> _tags = new List<Tag>();

        public ItunesPodcastUpdater(ILogger<ItunesPodcastUpdater> logger, IConfiguration configuration,
            IDbContextFactory dbContextFactory)
            : base(logger, configuration, dbContextFactory)
        {
            Context = dbContextFactory.CreateDbContext(Configuration);
        }

        private ApplicationDbContext Context { get; }

        public Task UpdateDataAsync()
        {
            return Task.Run(async () =>
            {
                var itunesIds = Context.BasePodcast.Select(x => x.ItunesId).ToImmutableList();
                Context.Dispose();
                foreach (var itunesId in itunesIds)
                    await CreatePodcast(itunesId).ConfigureAwait(false);
            });
        }


        private async Task CreatePodcast(string itunesId)
        {
            var context = DbContextFactory.CreateDbContext(Configuration);

            var result =
                await QueryService.QueryItunesId(itunesId)
                    .ConfigureAwait(true);

            if (!result.HasValues) return;

            var podcastResult = result[0].ToObject<PodcastResult>();

            var trackName = podcastResult.TrackName;
            trackName = trackName.CleanUpTitle();

            var existingPod = context.Podcast.FirstOrDefault(x => x.ItunesId == itunesId);

            if (existingPod == null)
            {
                var basePod = context.BasePodcast.FirstOrDefault(x => x.ItunesId == itunesId);
                var podcast = await CreatePodcast(itunesId, trackName, podcastResult, basePod, context).ConfigureAwait(false);

                await CreatePodcastTags(podcast, result, context).ConfigureAwait(false);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }

            context.Dispose();
        }


        private static async Task<Podcast> CreatePodcast(string itunesId, string trackName, PodcastResult podcastResult,
            BasePodcast basePodcast,
            ApplicationDbContext context)
        {
            Logger.LogInformation("Creating new podcast " + trackName);
            var podcast = new Podcast();
            podcast.ItunesId = itunesId;
            podcast.CreatedDate = DateTime.Now;

            podcast.Title = trackName;
            if (basePodcast != null)
            {
                if (podcast.Title.Length > 100 && basePodcast.Title.Length < 100)
                    podcast.Title = basePodcast.Title;
                else if (podcast.Title.Length > 100) podcast.Title = podcast.Title.Substring(0, 99);
                podcast.Description = StringCleaner.CleanHtml(basePodcast.Description);
                podcast.ShowUrl = basePodcast.PodcastSite;
            }

            podcast.ImageUrl = podcastResult.ImageUrl600;
            podcast.FeedUrl = podcastResult.FeedUrl;
            podcast.EpisodeCount = podcastResult.TrackCount;
            podcast.Country = podcastResult.Country;
            podcast.Artists = podcastResult.Artists;

            if (podcast.Artists.Length > 100) podcast.Artists = podcast.Artists.Substring(0, 99);

            var date = podcastResult.ReleaseDate;

            podcast.LatestReleaseDate = date;


            context.Podcast.Add(podcast);

            await context.SaveChangesAsync().ConfigureAwait(false);

            return podcast;
        }

        private static async Task CreatePodcastTags(Podcast podcast, JArray result, ApplicationDbContext context)
        {
            var newTags = new List<Tag>();
            Logger.LogInformation("Creating podcast tags");
            foreach (var genreResult in result)
            {
                dynamic genres = genreResult;
                JArray data = genres.genres;
                var listGenres = data.ToList();
                if (listGenres.Any())
                    foreach (var genre in listGenres)
                    {
                        var tagDescription = genre.Value<string>().ToLowerInvariant();
                        var existingTag = context.Tag.Any(x =>
                            x.Description.ToLowerInvariant() == tagDescription);
                        if (!existingTag) newTags.Add(new Tag {Description = genre.Value<string>()});
                    }
            }

            Logger.LogInformation(string.Join(", ", newTags.Select(x => x.Description)));
            await context.Tag.AddRangeAsync(newTags).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            foreach (var newTag in newTags)
            {
                var tagExists = podcast.PodcastTags.Any(t =>
                    t.Tag?.Description?.ToLowerInvariant() == newTag?.Description?.ToLowerInvariant());
                if (!tagExists)
                {
                    if (!podcast.PodcastTags.Any()) podcast.PodcastTags = new List<PodcastTag>();
                    podcast.PodcastTags.Add(new PodcastTag
                    {
                        PodcastId = podcast.Id,
                        TagId = newTag.Id
                    });
                }
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
            Logger.LogInformation("Saved tags for podcast: " + podcast.Title);
        }


       
    }
}