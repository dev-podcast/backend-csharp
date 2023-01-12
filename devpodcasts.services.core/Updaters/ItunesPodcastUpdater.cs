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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace DevPodcast.Services.Core.Updaters
{
    internal class ItunesPodcastUpdater : IItunesPodcastUpdater
    {
        private readonly IDictionary<string, Podcast> _podcasts = new Dictionary<string, Podcast>();
        private readonly IDictionary<string, ICollection<string>> _podcastTags =
            new Dictionary<string, ICollection<string>>();
        private readonly ICollection<Tag> _tags = new List<Tag>();
        private readonly IItunesQueryService _itunesQueryService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IItunesPodcastUpdater> _logger;

        public ItunesPodcastUpdater(ILogger<IItunesPodcastUpdater> logger,
            IDbContextFactory dbContextFactory, IItunesQueryService itunesQueryService)
        {
            _logger = logger;
            _itunesQueryService = itunesQueryService;
            _context = dbContextFactory.CreateDbContext();
        }

     
        public Task UpdateDataAsync()
        {
            return Task.Run(async () =>
            {
                var listOfItunesIds = GetItunesIds();
                var existingItunesIds = _context.Podcast.Select(x => x.ItunesId).ToList();

                var podcastToCreate = existingItunesIds.Except(listOfItunesIds).ToList();

                foreach (var itunesId in podcastToCreate)
                {
                    _logger.LogInformation("Updating id: " + itunesId);
                    await CreatePodcast(itunesId).ConfigureAwait(false);
                }

                CommitData().Wait();
                Dispose();
            });
        }

        private IReadOnlyCollection<string> GetItunesIds()
        {
            return _context.BasePodcast.Select(x => x.ItunesId).ToImmutableList();
        }

        private async Task CommitData()
        {
            if (_podcasts.Any())
            {
                await _context.Podcast.AddRangeAsync(_podcasts.Values).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }


            if (_tags.Any())
            {
                await _context.Tag.AddRangeAsync(_tags).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }


            if (_podcastTags.Values.Any())
                await SaveTagsAndPodcastTags(_podcasts, _podcastTags).ConfigureAwait(false);

        }


        private async Task SaveTagsAndPodcastTags(IDictionary<string, Podcast> podcasts,
            IDictionary<string, ICollection<string>> tagsToMap)
        {
            var updatedTags = new List<Tag>();

            foreach (var pod in podcasts)
            {
                var tempId = pod.Key;
                var podcast = pod.Value;
                var tagDescriptions = tagsToMap[tempId];

                var matchingTags = _context.Tag
                    .Where(x => tagDescriptions.Contains(x.Description)).ToList();

                foreach (var matchingTag in matchingTags)
                {
                    var tag = await _context.Tag.Where(x => x.Id == matchingTag.Id).FirstOrDefaultAsync();
                    tag.Podcasts.Add(podcast);
                    await _context.SaveChangesAsync();
               
                }
                    
            }
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }


        private async Task CreatePodcast(string itunesId)
        {
            var result =
                await _itunesQueryService.QueryItunesId(itunesId)
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
            return _context.BasePodcast.FirstOrDefault(x => x.ItunesId == itunesId);
        }

        private bool CheckForExistingPodcast(string itunesId)
        {
            return _context.Podcast.Any(x => x.ItunesId == itunesId);
        }


        private Task<Podcast> CreatePodcast(string itunesId, string trackName, PodcastResult podcastResult,
            BasePodcast basePodcast, string tempId)
        {
            _logger.LogInformation("Creating new podcast " + trackName);
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


            _podcasts.Add(tempId, podcast);
            return Task.FromResult(podcast);
        }

        private Task CreatePodcastTags(Podcast podcast, JArray result, string tempId)
        {
            return Task.Run(() =>
            {
                if (podcast.Title == "Talk Python To Me - Python conversations for passionate developers")
                    Console.WriteLine("here");

                var podcastTags = new List<string>();
                var existingTags = new List<Tag>();

                _logger.LogInformation("Creating podcast tags");

                foreach (var genreResult in result)
                {
                    dynamic genres = genreResult;
                    JArray data = genres.genres;
                    var listGenres = data.ToList();
                    if (listGenres.Any())
                        foreach (var genre in listGenres)
                        {
                            var tagDescription = genre.Value<string>();

                            if (_tags.All(x => x.Description != tagDescription))
                            {
                                var existingTag = _context.Tag.FirstOrDefault(x =>
                                    x.Description == tagDescription);

                                if (existingTag == null)
                                    _tags.Add(new Tag { Description = tagDescription });
                                else
                                    existingTags.Add(existingTag);
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


                _podcastTags.Add(tempId, podcastTags);

                _logger.LogInformation("Saved tags for podcast: " + podcast.Title);
            });
        }

        private bool CheckForExstingPodcastTag(Podcast podcast, Tag tag)
        {
            var tagExists = podcast.Tags.Any(t =>
                t?.Description == tag?.Description);
            return tagExists;
        }


        public void Dispose()
        {
            _context.Dispose();
        }
    }

    public interface IItunesPodcastUpdater : IUpdater
    {

    }
}