using System.Collections.Generic;
using System.Linq;
using devpodcasts.Domain.Entities;
using devpodcasts.Domain.Interfaces;

namespace devpodcasts.Data.EntityFramework.Repositories
{
    internal class BasePodcastRepository : Repository<BasePodcast>, IBasePodcastRepository
    {
        internal BasePodcastRepository(ApplicationDbContext context) : base(context)
        {
        }

        public ICollection<string> GetAllItunesIds()
        {
            return Set.Select(x => x.ItunesId).ToList();
        }
    }
}