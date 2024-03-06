using System.Globalization;
using devpodcasts.Data.EntityFramework;
using devpodcasts.Domain.Entities;
using devpodcasts.common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using devpodcasts.common.Extensions;
using devpodcasts.Domain.Interfaces;

namespace devpodcasts.common.Updaters;

public class PodCategoriesUpdater : IPodCategoriesUpdater
{
    private readonly ILogger<IPodCategoriesUpdater> _logger;
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ICategoryRepository _categoryRepository;
    private ICollection<Category> _categories { get; } = new List<Category>();

    public PodCategoriesUpdater(ILogger<IPodCategoriesUpdater> logger, IDbContextFactory<ApplicationDbContext> dbContextFactory, ICategoryRepository categoryRepository)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _categoryRepository = categoryRepository;
       
    }

    public async Task UpdateDataAsync()
    {

        var rootData = await GetRootDataAsync();

        if (rootData != null)
        {
            _logger.LogInformation("********Updating Podcast Categories");
            var dictionary = rootData.ToObject<IDictionary<string, JToken>>();

            if (dictionary == null)
            {
                _logger.LogError("Error casting root data to dictionary object");
                return;
            }

            var tasks = new List<Task>();


            foreach(var kvp in dictionary)
            {
                var context = _dbContextFactory.CreateDbContext();

                var podcastId = kvp.Key;
                var categories = JArray.Parse(kvp.Value.ToString()).ToList();
                var podcast = await context.Podcast.Where(x => x.Id == Convert.ToInt32(podcastId, CultureInfo.InvariantCulture)).FirstOrDefaultAsync();

                if(podcast == null) { continue; }

                var catList = categories.Select(cat => Convert.ToInt32(cat, CultureInfo.InvariantCulture)).ToList();
                var cats = _categoryRepository.GetAll(cat => catList.Contains(cat.Id));

                if(cats.Any())
                {
                    _logger.LogInformation($"{catList.Count} categories");
                    foreach(var cat in cats)
                    {
                        cat.Podcasts.Add(podcast);
                        await context.SaveChangesAsync();
                    }
                }else
                {
                    _logger.LogInformation("Could not find categories to update");
                }

                await context.DisposeAsync();
            }

                

          





            //foreach (var kvp in dictionary)
            //    tasks.Add(ProcessCategoryAsync(kvp));

            //await Task.WhenAll(tasks).ConfigureAwait(false);

           // await ComitDataAsync();

            _logger.LogInformation("Podcast Categories Update DONE!");


           // context

           // Dispose();
        }
    }

    private async Task ProcessCategoryAsync(KeyValuePair<string, JToken> item)
    {
        var context = _dbContextFactory.CreateDbContext();
        var podId = item.Key;
        var catArray = JArray.Parse(item.Value.ToString());
        var podcast = await context.Podcast.FindAsync(Convert.ToInt32(podId, CultureInfo.InvariantCulture));

        if (podcast != null)
        {
            _logger.LogInformation("********Updating: " + podcast.Title);
            catArray.ForEach(async cat =>
            {
                var catId = Convert.ToInt32(cat, CultureInfo.InvariantCulture);
              
                    var category = await context.Category.Where(x => x.Id == catId).SingleOrDefaultAsync();
                    if (category != null)
                    {
                        //Category exists and just need to add podcast to it.
                        category.Podcasts.Add(podcast);
                        await context.SaveChangesAsync();
                    }
                    _categories.Add(category);
                
            });
        }
        await context.DisposeAsync();
    }

    private async Task<JObject> GetRootDataAsync()
    {
        var categoriesPath = Environment.CurrentDirectory;
        _logger.LogInformation(categoriesPath);
        categoriesPath = Path.Combine(categoriesPath, @"PodList/podcategories.json");

        var jsonObject = await File.ReadAllTextAsync(categoriesPath).ConfigureAwait(false);

        return JObject.Parse(jsonObject);
    }

    //private async Task ComitDataAsync()
    //{
    //    //TODO: Need to look this over since the flow of code has changed
    //    await _context.Category.AddRangeAsync(_categories).ConfigureAwait(false);
    //    await _context.SaveChangesAsync().ConfigureAwait(false);
    //}

    public void Dispose()
    {
        this.Dispose();
       // _context.Dispose();
    }
}

public interface IPodCategoriesUpdater : IUpdater
{

}