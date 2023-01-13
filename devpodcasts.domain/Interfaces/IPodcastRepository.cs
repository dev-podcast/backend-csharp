using devpodcasts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace devpodcasts.Domain.Interfaces
{
    public interface IPodcastRepository : IRepository<Podcast>, IDisplayData<Podcast>
    {
        Task<List<Podcast>> GetRecentAsync(int podcastLimit, int episodeLimit);
        Task<List<Podcast>> GetRecentAsync(int numberToTake);

        Task<List<Podcast>> GetAllAsync(int id);
        Task<List<Podcast>> GetAllBySearch(Expression<Func<Podcast, bool>> predicate);
    }
}
