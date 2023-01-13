using System.Threading.Tasks;
using devpodcasts.Domain;
using devpodcasts.Domain.Entities;
using devpodcasts.Domain.Interfaces;

namespace devpodcasts.Data.EntityFramework.Repositories
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
                Podcasts = await unitOfWork.PodcastRepository.GetAllBySearch(p => p.Title.Contains(searchString)
                                                                               || p.Description.Contains(
                                                                                   searchString) ||
                                                                               p.Artists.Contains(
                                                                                   searchString)),
                Episodes = await unitOfWork.EpisodeRepository.GetAllBySearch(e => e.Title.Contains(searchString))
            };

            return _searchResult;
        }
    }
}