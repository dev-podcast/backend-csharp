using DevPodcast.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevPodcast.Domain.Interfaces
{
    public interface IPodcastRepository : IRepository<Podcast>, IDisplayData<Podcast>
    {
        Task<ICollection<Podcast>> GetRecentAsync(int podcastLimit, int episodeLimit);
        Task<ICollection<Podcast>> GetRecentAsync(int numberToTake);
    }
}
