using System.Collections.Generic;
using System.Threading.Tasks;
using devpodcasts.Domain.Entities;

namespace devpodcasts.Domain.Interfaces
{
    public interface IBasePodcastRepository : IRepository<BasePodcast>
    {
        ICollection<string> GetAllItunesIds();
        Task<List<string>> GetAllItunesIdsAsync();
    }
}