using System.Threading.Tasks;
using DevPodcast.Domain.Entities;

namespace DevPodcast.Domain.Interfaces
{
    public interface ISearchRepository
    {
        Task<SearchResult> GetSearchResultAsync(IUnitOfWork unitOfWork, string searchString);
    }
}