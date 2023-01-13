using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using devpodcasts.Domain.Entities;

namespace devpodcasts.Domain.Interfaces
{
    public interface IEpisodeRepository : IRepository<Episode>, IDisplayData<Episode>
    {
        Task<List<Episode>> GetRecentAsync(int Id, int numberToTake);
        List<Episode> GetRecent(int Id,int numberToTake);
        Task<List<Episode>> GetByShowIdAsync(int ShowId);
        Task<List<Episode>> GetAllBySearch(Expression<Func<Episode, bool>> predicate);
    }
}
