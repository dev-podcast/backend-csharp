using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevPodcast.Data.EntityFramework;
using DevPodcast.Domain.Entities;
using DevPodcast.Services.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace DevPodcast.Services.Core.Updaters
{
    public class PodCategoriesUpdater : Updater, IUpdater
    {
        public PodCategoriesUpdater(ILogger<PodCategoriesUpdater> logger, IDbContextFactory dbContextFactory)
            : base(logger, dbContextFactory)
        {
            Context = dbContextFactory.CreateDbContext();
            Context.Database.AutoTransactionsEnabled = false;
        }

        private static ApplicationDbContext Context { get; set; }

        private ICollection<PodcastCategory> Categories { get; } = new List<PodcastCategory>();

        public Task UpdateDataAsync()
        {
            return Task.Run(async () =>
            {
                var rootData = await GetRootData().ConfigureAwait(false);

                if (rootData != null)
                {
                    Logger.LogInformation("********Updating Podcast Categories");
                    var dictionary = rootData.ToObject<IDictionary<string, JToken>>();

                    var tasks = new List<Task>();
                    foreach (var kvp in dictionary)
                        tasks.Add(ProcessCategory(kvp));

                    await Task.WhenAll(tasks).ConfigureAwait(false);

                    await ComitData().ConfigureAwait(false);

                    Logger.LogInformation("Podcast Categories Update DONE!");

                    Dispose();
                }
            });
        }

        private async Task ComitData()
        {
            await Context.PodcastCategory.AddRangeAsync(Categories).ConfigureAwait(false);
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task ProcessCategory(KeyValuePair<string, JToken> item)
        {
            var podId = item.Key;
            var catArray = JArray.Parse(item.Value.ToString());
            var podcast = GetPodcast(podId);

            if (podcast != null)
            {
                Logger.LogInformation("********Updating: " + podcast.Title);
                Parallel.ForEach(catArray, cat =>
                {
                    var podcastCategory = new PodcastCategory
                    {
                        CategoryId = Convert.ToInt32(cat, CultureInfo.InvariantCulture), PodcastId = podcast.Id
                    };

                    using (var innerContext = DbContextFactory.CreateDbContext())
                    {
                        var exists = PodcastCategoryExists(podcastCategory);
                        if (exists == null)
                            Categories.Add(podcastCategory);
                    }
                });
            }

            if (Categories.Any())
                Categories.AddRange(Categories);
        }

        private static PodcastCategory PodcastCategoryExists(PodcastCategory podcastCategory)
        {
            return Context.PodcastCategory.FirstOrDefault(x =>
                x.CategoryId == podcastCategory.CategoryId &&
                x.PodcastId == podcastCategory.PodcastId);
        }

        private static Podcast GetPodcast(string podId)
        {
            return Context.Podcast.Find(Convert.ToInt32(podId, CultureInfo.InvariantCulture));
        }

        private async Task<JObject> GetRootData()
        {
            var categoriesPath = Environment.CurrentDirectory;
            Logger.LogInformation(categoriesPath);
            categoriesPath = Path.Combine(categoriesPath, @"PodList/podcategories.json");

            return JObject.Parse(await File.ReadAllTextAsync(categoriesPath).ConfigureAwait(false));
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}