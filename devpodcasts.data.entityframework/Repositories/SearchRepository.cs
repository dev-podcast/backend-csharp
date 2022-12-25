using System.Threading.Tasks;
using DevPodcast.Domain;
using DevPodcast.Domain.Entities;
using DevPodcast.Domain.Interfaces;

namespace DevPodcast.Data.EntityFramework.Repositories
{
    internal class SearchRepository : Repository<SearchResult>, ISearchRepository
    {
        private SearchResult _searchResult;

        internal SearchRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<SearchResult> GetSearchResultAsync(IUnitOfWork unitOfWork, string searchString)
        {
            _searchResult = new SearchResult
            {
                Podcasts = await unitOfWork.PodcastRepository.GetAllAsync(p => p.Title.Contains(searchString)
                                                                               || p.Description.Contains(
                                                                                   searchString) ||
                                                                               p.Artists.Contains(
                                                                                   searchString)),
                Episodes = await unitOfWork.EpisodeRepository.GetAllAsync(e => e.Title.Contains(searchString))
            };

            return _searchResult;
        }
    }
}