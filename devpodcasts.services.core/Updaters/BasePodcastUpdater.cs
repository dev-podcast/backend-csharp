using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevPodcast.Data.EntityFramework;
using DevPodcast.Domain.Entities;
using DevPodcast.Services.Core.Interfaces;
using DevPodcast.Services.Core.JsonObjects;
using DevPodcast.Services.Core.Updaters.Extensions;
using DevPodcast.Services.Core.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DevPodcast.Services.Core.Updaters
{
    public class BasePodcastUpdater : Updater, IUpdater
    {

        private ApplicationDbContext Context { get; set; }
        public BasePodcastUpdater(ILogger<BasePodcastUpdater> logger, IConfiguration configuration, IDbContextFactory dbContextFactory)
            : base(logger, configuration, dbContextFactory)
        {
           
            Context = dbContextFactory.CreateDbContext(Configuration);
        }

        private ICollection<BasePodcast> BasePodcasts { get;} = new List<BasePodcast>();
        private ICollection<Tag> Tags { get; } = new List<Tag>();

        public Task UpdateDataAsync()
        {
            return Task.Run(() =>
            {
                    var podListPath = Environment.CurrentDirectory;
                    Logger.LogInformation(podListPath);
                    podListPath = Path.Combine(podListPath, @"PodList/podlist.json");


                    var file = File.ReadAllText(podListPath);
                    var basePodcasts = JsonConvert.DeserializeObject<RootJsonObject>(file);

                    var properties = basePodcasts.GetType().GetProperties();
                    properties.ForEach(prop =>
                    {
                        Context = DbContextFactory.CreateDbContext(Configuration);
                        var existing = Context.BasePodcast.ToList();
                        var propertyName = prop.Name;
                        Logger.LogInformation("BasePodcast Category: " + propertyName);
                        var podcastList = (IEnumerable<BasePodcastJsonObject>)prop.GetValue(basePodcasts);

                        var diff = FindNonExisting(podcastList, existing);

                        var basePodcastJsonObjects = diff.ToList();
                        if (basePodcastJsonObjects.Any())
                        {
                            basePodcastJsonObjects.ForEach(bp =>
                            {
                                var name = bp.Title;
                                name = name.RemovePodcastFromName();
                                bp.Title = name;

                                var qs = bp.ItunesSubscriberUrl.Split('/');
                                var unformattedId = qs[qs.Length - 1].Split('?');
                                var itunesId = unformattedId[0];
                                bp.ItunesId = itunesId;
                            });

                            BasePodcasts.AddRange(basePodcastJsonObjects.Select(d => d.CreateBasePodcast()));
                        }
                    });

                    Save();
            });
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

        private void Save()
        {
            //Insert base podcasts
            if (BasePodcasts.Any())
            {
                Context.BasePodcast.AddRange(BasePodcasts);
            }

            Context.SaveChanges();

            Context.Dispose();
        }
    }
}