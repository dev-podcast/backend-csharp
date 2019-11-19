using System.Collections.Generic;
using System.Threading.Tasks;
using DevPodcast.Domain.Entities;

namespace DevPodcast.Domain.Interfaces
{
    public interface IPodcastCategoryRepository : IRepository<PodcastCategory>
    {
        Task<List<PodcastCategory>> GetPodcastCategoriesAsync(int podcastId);
        List<PodcastCategory> GetPodcastCategories(int podcastId);
        ICollection<Podcast> GetByCategoryName(string categoryName);
        ICollection<Podcast> GetByCategoryId(int categoryId);
        Task<List<Podcast>> GetByCategoryNameAsync(string categoryName);
        Task<List<Podcast>> GetByCategoryIdAsync(int categoryId);
    }
}
