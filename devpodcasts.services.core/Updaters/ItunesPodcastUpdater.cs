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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace DevPodcast.Services.Core.Updaters
{
    internal class ItunesPodcastUpdater : Updater, IUpdater
    {
        private static readonly IDictionary<string, Podcast> Podcasts = new Dictionary<string, Podcast>();

        private static readonly IDictionary<string, ICollection<string>> PodcastTags =
            new Dictionary<string, ICollection<string>>();

        private static readonly ICollection<Tag> Tags = new List<Tag>();

        public ItunesPodcastUpdater(ILogger<ItunesPodcastUpdater> logger,
            IDbContextFactory dbContextFactory)
            : base(logger, dbContextFactory)
        {
            Context = dbContextFactory.CreateDbContext();
        }

        private static ApplicationDbContext Context { get; set; }

        public Task UpdateDataAsync()
        {
            return Task.Run(async () =>
            {
                foreach (var itunesId in GetItunesIds())
                {
                    Logger.LogInformation("Updating id: " + itunesId);
                    await CreatePodcast(itunesId).ConfigureAwait(false);
                }

                CommitData().Wait();
                Dispose();
            });
        }

        private IReadOnlyCollection<string> GetItunesIds()
        {
            return Context.BasePodcast.Select(x => x.ItunesId).ToImmutableList();
            ;
        }

        private async Task CommitData()
        {
            if (Podcasts.Any())
            {
                await Context.Podcast.AddRangeAsync(Podcasts.Values).ConfigureAwait(false);
                await Context.SaveChangesAsync().ConfigureAwait(false);
            }


            if (Tags.Any())
            {
                await Context.Tag.AddRangeAsync(Tags).ConfigureAwait(false);
                await Context.SaveChangesAsync().ConfigureAwait(false);
            }


            if (PodcastTags.Values.Any())
                await SaveTagsAndPodcastTags(Podcasts, PodcastTags).ConfigureAwait(false);

        }


        private async Task SaveTagsAndPodcastTags(IDictionary<string, Podcast> podcasts,
            IDictionary<string, ICollection<string>> tagsToMap)
        {
            var updatedTags = new List<PodcastTag>();

            foreach (var pod in podcasts)
            {
                var tempId = pod.Key;
                var podcast = pod.Value;
                var tagDescriptions = tagsToMap[tempId];

                var matchingTags = Context.Tag
                    .Where(x => tagDescriptions.Contains(x.Description)).ToList();

                foreach (var matchingTag in matchingTags)
                    updatedTags.Add(new PodcastTag {PodcastId = podcast.Id, TagId = matchingTag.Id});
            }

            await Context.PodcastTag.AddRangeAsync(updatedTags).ConfigureAwait(false);
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }


        private async Task CreatePodcast(string itunesId)
        {
            var result =
                await QueryService.QueryItunesId(itunesId)
                    .ConfigureAwait(true);


            if (!result.HasValues) return;

            var podcastResult = result[0].ToObject<PodcastResult>();

            var trackName = podcastResult.TrackName;
            trackName = trackName.CleanUpTitle();

            if (!CheckForExistingPodcast(itunesId))
            {
                var tempId = Guid.NewGuid().ToString();
                var podcast = await CreatePodcast(itunesId, trackName, podcastResult, GetBasePodcast(itunesId), tempId)
                    .ConfigureAwait(false);

                await CreatePodcastTags(podcast, result, tempId).ConfigureAwait(false);
            }
        }

        private BasePodcast GetBasePodcast(string itunesId)
        {
            return Context.BasePodcast.FirstOrDefault(x => x.ItunesId == itunesId);
        }

        private bool CheckForExistingPodcast(string itunesId)
        {
            return Context.Podcast.Any(x => x.ItunesId == itunesId);
        }


        private static async Task<Podcast> CreatePodcast(string itunesId, string trackName, PodcastResult podcastResult,
            BasePodcast basePodcast, string tempId)
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
                podcast.Description = basePodcast.Description.CleanHtml();
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


            Podcasts.Add(tempId, podcast);
            return podcast;
        }

        private static async Task CreatePodcastTags(Podcast podcast, JArray result, string tempId)
        {
            if (podcast.Title == "Talk Python To Me - Python conversations for passionate developers")
                Console.WriteLine("here");

            var podcastTags = new List<string>();
            var existingTags = new List<Tag>();

            Logger.LogInformation("Creating podcast tags");

            foreach (var genreResult in result)
            {
                dynamic genres = genreResult;
                JArray data = genres.genres;
                var listGenres = data.ToList();
                if (listGenres.Any())
                    foreach (var genre in listGenres)
                    {
                        var tagDescription = genre.Value<string>();

                        if (Tags.All(x => x.Description != tagDescription))
                        {
                            var existingTag = Context.Tag.FirstOrDefault(x =>
                                x.Description == tagDescription);

                            if (existingTag == null)
                                Tags.Add(new Tag { Description = tagDescription });
                            else
                                existingTags.Add(existingTag);
                        }
                       
                    }
            }

            Tags.ForEach(tag =>
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


            PodcastTags.Add(tempId, podcastTags);

            Logger.LogInformation("Saved tags for podcast: " + podcast.Title);
        }

        private static bool CheckForExstingPodcastTag(Podcast podcast, Tag tag)
        {
            var tagExists = podcast.PodcastTags.Any(t =>
                t.Tag?.Description == tag?.Description);
            return tagExists;
        }


        public void Dispose()
        {
            Context.Dispose();
        }
    }
}