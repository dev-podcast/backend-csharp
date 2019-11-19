using DevPodcast.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevPodcast.Domain.Interfaces
{
    public interface IPodcastRepository : IRepository<Podcast>, IDisplayData<Podcast>
    {
        Task<List<Podcast>> GetRecentAsync(int podcastLimit, int episodeLimit);
        Task<List<Podcast>> GetRecentAsync(int numberToTake);
        ICollection<Podcast> GetRecent(int numberToTake);
    }
}
