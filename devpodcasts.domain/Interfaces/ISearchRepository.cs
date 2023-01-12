using System.Threading.Tasks;
using devpodcasts.Domain.Entities;

namespace devpodcasts.Domain.Interfaces
{
    public interface ISearchRepository
    {
        Task<SearchResult> GetSearchResultAsync(IUnitOfWork unitOfWork, string searchString);
    }
}