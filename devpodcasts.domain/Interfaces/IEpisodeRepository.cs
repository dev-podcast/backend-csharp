using System.Collections.Generic;
using System.Threading.Tasks;
using devpodcasts.Domain.Entities;

namespace devpodcasts.Domain.Interfaces
{
    public interface IEpisodeRepository : IRepository<Episode>, IDisplayData<Episode>
    {
        Task<List<Episode>> GetRecentAsync(int Id, int numberToTake);
        ICollection<Episode> GetRecent(int Id,int numberToTake);
    }
}
