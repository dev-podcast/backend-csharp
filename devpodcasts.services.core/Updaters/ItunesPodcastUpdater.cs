using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using DevPodcast.Data.EntityFramework;
using DevPodcast.Domain.Entities;
using DevPodcast.Services.Core.Interfaces;
using DevPodcast.Services.Core.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevPodcast.Services.Core.Updaters
{
    internal class ItunesPodcastUpdater : Updater, IUpdater
    {
        public ItunesPodcastUpdater(ILogger<ItunesPodcastUpdater> logger, IConfiguration configuration, IDbContextFactory dbContextFactory)
            : base(logger, configuration, dbContextFactory)
        {
        }

        public Task UpdateDataAsync()
        {
            var context = DbContextFactory.CreateDbContext(Configuration);
            var itunesIds = context.BasePodcast.Select(x => x.ItunesId).ToImmutableList();
            var podcastsToCreate = new List<Task>();

            Task.Run(() =>
            {
                itunesIds.ForEach(itunesId =>
                {
                    podcastsToCreate.Add(CreatePodcast(itunesId));
                });
            });
            
            return Task.WhenAll(podcastsToCreate);
        }

        private async Task CreatePodcast(string itunesId)
        {
            Context = DbContextFactory.CreateDbContext(Configuration);

                dynamic result = 
                    await QueryService.QueryItunesId(itunesId)
                    .ConfigureAwait(true);

                if (result != null)
                {
                    dynamic responsePod = result[0];
                    if (responsePod != null)
                    {
                        string trackName = responsePod.trackName;
                        trackName = trackName.CleanUpTitle();

                        var existingPod = Context.Podcast.FirstOrDefault(x => x.ItunesId == itunesId);

                        if (existingPod == null)
                        {
                            var basePod = Context.BasePodcast.FirstOrDefault(x => x.ItunesId == itunesId);
                            
                            await CreatePodcastTags(await CreatePodcast(itunesId, trackName, responsePod, basePod, Context), result).ConfigureAwait(false);

                            await Context.SaveChangesAsync().ConfigureAwait(false);
                        }
                    }
                }
        }

        private Task<Podcast> CreatePodcast(string itunesId, string trackName, dynamic responsePod, BasePodcast basePodcast, ApplicationDbContext context)
        {
            return Task.Run(() =>
            {
                Logger.LogInformation("Creating new podcast " + trackName);
                var podcast = new Podcast();
                podcast.ItunesId = itunesId;
                podcast.CreatedDate = DateTime.Now;

                podcast.Title = trackName;
                if (basePodcast != null)
                {
                    if (podcast.Title.Length > 100 && basePodcast.Title.Length < 100)
                    {
                        podcast.Title = basePodcast.Title;
                    }
                    else if (podcast.Title.Length > 100)
                    {
                        podcast.Title = podcast.Title.Substring(0, 99);
                    }
                    podcast.Description = StringCleaner.CleanHtml(basePodcast.Description);
                    podcast.ShowUrl = basePodcast.PodcastSite;
                }

                podcast.ImageUrl = responsePod.artworkUrl600;
                podcast.FeedUrl = responsePod.feedUrl;
                podcast.EpisodeCount = Convert.ToInt32(responsePod.trackCount);
                podcast.Country = responsePod.country;
                podcast.Artists = responsePod.artistName;

                if (podcast.Artists.Length > 100)
                {
                    podcast.Artists = podcast.Artists.Substring(0, 99);
                }

                var date = responsePod.releaseDate;

                if (date != null)
                {
                    DateTime d = date.Value;
                    podcast.LatestReleaseDate = d;
                }

                context.Podcast.Add(podcast);

                return podcast;
            });

            
        }

        private Task CreatePodcastTags(Podcast podcast, JArray result)
        {
            return Task.Run(() =>
            {
                Logger.LogInformation("Creating podcast tags");
                result.ForEach((genre) =>
                {
                    var tag = Context.Tag.FirstOrDefault(x => x.Description == genre.ToString());
                    if (tag == null)
                    {
                        tag = new Tag() {Description = genre.ToString()};
                        Context.Tag.Add(tag);
                    }

                    var tagExists = false;
                    if (podcast.PodcastTags.Any())
                        tagExists = podcast.PodcastTags
                                        .FirstOrDefault(t => tag != null && t.Tag.Description == tag.Description) !=
                                    null;

                    if (!tagExists)
                        tag.PodcastTags.Add(new PodcastTag
                        {
                            Podcast = podcast,
                            PodcastId = podcast.Id,
                            Tag = tag,
                            TagId = tag.Id
                        });

                });
            });

        }

    }
}