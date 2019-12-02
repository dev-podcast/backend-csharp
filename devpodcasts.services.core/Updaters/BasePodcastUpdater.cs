using DevPodcast.Data.EntityFramework;
using DevPodcast.Domain.Entities;
using DevPodcast.Services.Core.Interfaces;
using DevPodcast.Services.Core.JsonObjects;
using DevPodcast.Services.Core.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DevPodcast.Services.Core.Updaters
{
    public class BasePodcastUpdater : Updater, IUpdater
    {

        private ApplicationDbContext Context { get; set; }
        public BasePodcastUpdater(ILogger<BasePodcastUpdater> logger, IDbContextFactory dbContextFactory)
            : base(logger, dbContextFactory)
        {
           
            Context = dbContextFactory.CreateDbContext();
        }

        private ICollection<BasePodcast> BasePodcasts { get;} = new List<BasePodcast>();
        private ICollection<Tag> Tags { get; } = new List<Tag>();

        public Task UpdateDataAsync()
        {
            return Task.Run(() =>
            {
                    var basePodcasts = GetBasePodcastsFromJson();

                    var properties = basePodcasts.GetType().GetProperties();
                    foreach (var prop in properties)
                    {
                        var existingBasePodcasts = Context.BasePodcast.ToImmutableList();
                        var podcastList = (IEnumerable<BasePodcastJsonObject>)prop.GetValue(basePodcasts);
                        var propertyName = prop.Name;

                        Logger.LogInformation("BasePodcast Category: " + propertyName);

                        var basePodcastJsonObjects = FindNonExisting(podcastList, existingBasePodcasts).ToList();
                        if (!basePodcastJsonObjects.Any()) continue;
                        BasePodcasts.AddRange(basePodcastJsonObjects.Select(d => d.CreateBasePodcast()));
                       
                    }

                    Save().Wait();

                    Logger.LogInformation("Updating base podcasts is complete...");
            });
        }

        private static RootJsonObject GetBasePodcastsFromJson()
        {
            var podListPath = Environment.CurrentDirectory;
            podListPath = Path.Combine(podListPath, @"PodList/podlist.json");
            var file = File.ReadAllText(podListPath);
            var basePodcasts = JsonConvert.DeserializeObject<RootJsonObject>(file);
            return basePodcasts;
        }

        private IEnumerable<BasePodcastJsonObject> FindNonExisting(IEnumerable<BasePodcastJsonObject> newPods,
            IEnumerable<BasePodcast> existing)
        {
            var newPodcasts = newPods.ToList();
            var existingPodcasts = existing.ToList();

            var newTitles = newPodcasts
                .Select(x => x.Title.RemovePodcastFromName())
                .Except(existingPodcasts.Select(x => x.Title)).ToList();
            var diff = (from n in newPodcasts
                join nt in newTitles on n.Title equals nt
                select n).ToList();
            return diff;
        }

        private async Task Save()
        {
            if (BasePodcasts.Any())
            {
                await Context.BasePodcast.AddRangeAsync(BasePodcasts).ConfigureAwait(false);
                await Context.SaveChangesAsync().ConfigureAwait(false);
            }
            Context.Dispose();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}