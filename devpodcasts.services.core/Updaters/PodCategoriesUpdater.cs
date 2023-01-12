using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevPodcast.Data.EntityFramework;
using DevPodcast.Domain.Entities;
using DevPodcast.Services.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace DevPodcast.Services.Core.Updaters
{
    public class PodCategoriesUpdater : IPodCategoriesUpdater
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IPodCategoriesUpdater> _logger;
        private readonly IDbContextFactory _dbContextFactory;
        private ICollection<Category> _categories { get; } = new List<Category>();

        public PodCategoriesUpdater(ILogger<IPodCategoriesUpdater> logger, IDbContextFactory dbContextFactory)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
            _context = dbContextFactory.CreateDbContext();           
        }

        public Task UpdateDataAsync()
        {
            return Task.Run(async () =>
            {
                var rootData = await GetRootDataAsync();

                if (rootData != null)
                {
                    _logger.LogInformation("********Updating Podcast Categories");
                    var dictionary = rootData.ToObject<IDictionary<string, JToken>>();

                    var tasks = new List<Task>();
                    foreach (var kvp in dictionary)
                        tasks.Add(ProcessCategoryAsync(kvp));

                    await Task.WhenAll(tasks).ConfigureAwait(false);

                    await ComitDataAsync();

                    _logger.LogInformation("Podcast Categories Update DONE!");

                    Dispose();
                }
            });
        }
    
        private async Task ProcessCategoryAsync(KeyValuePair<string, JToken> item)
        {
            var podId = item.Key;
            var catArray = JArray.Parse(item.Value.ToString());
            var podcast = await _context.Podcast.FindAsync(Convert.ToInt32(podId, CultureInfo.InvariantCulture));

            if (podcast != null)
            {
                _logger.LogInformation("********Updating: " + podcast.Title);
                catArray.ForEach(async cat =>
                {
                    var catId = Convert.ToInt32(cat, CultureInfo.InvariantCulture);
                    using (var innerContext = _dbContextFactory.CreateDbContext())
                    {
                        var category = await innerContext.Category.Where(x => x.Id == catId).SingleOrDefaultAsync();
                        if (category != null)
                        {
                            //Category exists and just need to add podcast to it.
                            category.Podcasts.Add(podcast);
                           await  innerContext.SaveChangesAsync();
                        }
                        _categories.Add(category);
                    }
                });
            }

            //if (_categories.Any())
            //    _categories.AddRange(_categories);        
        } 

        private async Task<JObject> GetRootDataAsync()
        {
            var categoriesPath = Environment.CurrentDirectory;
            _logger.LogInformation(categoriesPath);
            categoriesPath = Path.Combine(categoriesPath, @"PodList/podcategories.json");

            var jsonObject = await File.ReadAllTextAsync(categoriesPath).ConfigureAwait(false);

            return JObject.Parse(jsonObject);
        }

        private async Task ComitDataAsync()
        {
            //TODO: Need to look this over since the flow of code has changed
            await _context.Category.AddRangeAsync(_categories).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

    public interface IPodCategoriesUpdater : IUpdater
    {
       
    }
}