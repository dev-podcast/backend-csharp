using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevPodcast.Domain.Entities;
using DevPodcast.Services.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevPodcast.Services.Core.Updaters
{
    public class PodCategoriesUpdater : Updater, IUpdater
    {
        public PodCategoriesUpdater(ILogger<PodCategoriesUpdater> logger, IConfiguration configuration, IDbContextFactory dbContextFactory)
            : base(logger, configuration, dbContextFactory)
        {
        }

        public Task UpdateDataAsync()
        {      
            return Task.Run(async () =>
            {
                var rootData = await GetRootData().ConfigureAwait(false);

                if (rootData != null)
                {
                    Logger.LogInformation("********Updating Podcast Categories");
                    var dictionary = rootData.ToObject <IDictionary<string, JToken>>();

                    var tasks = new List<Task>();
                    foreach (var kvp in dictionary)
                        tasks.Add(ProcessCategory(kvp));

                    await Task.WhenAll(tasks).ConfigureAwait(false);

                    Logger.LogInformation("Podcast Categories Update DONE!");
                }

            });
        }

        private async Task ProcessCategory(KeyValuePair<string, JToken> item)
        {
                var context = DbContextFactory.CreateDbContext(Configuration);
                
                var podId = item.Key;         
                var catArray = JArray.Parse(item.Value.ToString());
                var podcast = context.Podcast.Find(Convert.ToInt32(podId, CultureInfo.InvariantCulture));
                var categories = new List<PodcastCategory>();

                if (podcast != null) 
                {
                    Logger.LogInformation("********Updating: " + podcast.Title);
                    Parallel.ForEach(catArray, (cat) =>
                    {
                        var podcastCategory = new PodcastCategory
                        {
                            CategoryId = Convert.ToInt32(cat, CultureInfo.InvariantCulture), PodcastId = podcast.Id
                        };

                        using (var innerContext = DbContextFactory.CreateDbContext(Configuration))
                        {
                            var exists = innerContext.PodcastCategory.FirstOrDefault(x =>
                                x.CategoryId == podcastCategory.CategoryId &&
                                x.PodcastId == podcastCategory.PodcastId);
                            if (exists == null)
                                categories.Add(podcastCategory);
                        }
                    });
                }

                if (categories.Any())
                {
                    context.PodcastCategory.AddRange(categories);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }

                context.Dispose();
        }

        
        private async Task<JObject> GetRootData()
        {
            var categoriesPath = Environment.CurrentDirectory;
            Logger.LogInformation(categoriesPath);
            categoriesPath = Path.Combine(categoriesPath, @"PodList/podcategories.json");

            return JObject.Parse(await File.ReadAllTextAsync(categoriesPath).ConfigureAwait(false));
        }
    }
}