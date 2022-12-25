using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevPodcast.Domain.Entities;
using DevPodcast.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevPodcast.Data.EntityFramework.Repositories
{
    internal class PodcastCategoryRepository : Repository<PodcastCategory>, IPodcastCategoryRepository
    {
        internal PodcastCategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public ICollection<Podcast> GetByCategoryId(int categoryId)
        {
            return (from pod in _context.Podcast.Include(p => p.PodcastCategories).Include(p => p.PodcastTags)
                join cat in _context.PodcastCategory on pod.Id equals cat.PodcastId
                where cat.CategoryId == categoryId
                select pod).ToList();

        }

        public Task<List<Podcast>> GetByCategoryIdAsync(int categoryId)
        {
            return (from pod in _context.Podcast.AsNoTracking().Include(p => p.PodcastCategories)
                    .Include(p => p.PodcastTags)
                join cat in _context.PodcastCategory on pod.Id equals cat.PodcastId
                where cat.CategoryId == categoryId
                select pod).ToListAsync();
        }

        public ICollection<Podcast> GetByCategoryName(string categoryName)
        {
            return (from pod in _context.Podcast.Include(p => p.PodcastCategories).Include(p => p.PodcastTags)
                join cat in _context.PodcastCategory on pod.Id equals cat.PodcastId
                where cat.Category.Description == categoryName
                select pod).ToList();
        }

        public Task<List<Podcast>> GetByCategoryNameAsync(string categoryName)
        {
            // var data = SingleQuery(categoryName);
            return (from pod in _context.Podcast.Include(p => p.PodcastCategories).Include(p => p.PodcastTags)
                join cat in _context.PodcastCategory on pod.Id equals cat.PodcastId
                where cat.Category.Description == categoryName
                select pod).ToListAsync();
        }

        public List<PodcastCategory> GetPodcastCategories(int podcastId)
        {
            return Set.Where(p => p.PodcastId == podcastId).ToList();
        }

        public Task<List<PodcastCategory>> GetPodcastCategoriesAsync(int podcastId)
        {
            return Set.Where(p => p.PodcastId == podcastId).ToListAsync();
        }

        private List<Podcast> SingleQuery(string categoryName)
        {
            var initialQuery = Set.Where(x => x.Category.Description.Contains(categoryName));

            var finalQuery = (from pod in _context.Podcast.Include(p => p.PodcastCategories).Include(p => p.PodcastTags)
                    .Include(p => p.Episodes)
                join cat in initialQuery on pod.Id equals cat.PodcastId
                select new Podcast
                {
                    Id = pod.Id,
                    Title = pod.Title,
                    Artists = pod.Artists,
                    Episodes = pod.Episodes.OrderByDescending(e => e.PublishedDate).ToList(),
                    PodcastTags = pod.PodcastTags,
                    Description = pod.Description,
                    ImageUrl = pod.ImageUrl,
                    ShowUrl = pod.ShowUrl,
                    PodcastCategories = pod.PodcastCategories,
                    FeedUrl = pod.FeedUrl,
                    EpisodeCount = pod.EpisodeCount,
                    LatestReleaseDate = pod.LatestReleaseDate
                }).ToList();

            return finalQuery;
        }
    }
}