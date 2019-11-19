using System.Collections.Generic;
using System.Threading.Tasks;
using DevPodcast.Domain.Entities;

namespace DevPodcast.Domain.Interfaces
{
    public interface IEpisodeRepository : IRepository<Episode>, IDisplayData<Episode>
    {
        Task<List<Episode>> GetRecentAsync(int Id, int numberToTake);
        ICollection<Episode> GetRecent(int Id,int numberToTake);
    }
}
