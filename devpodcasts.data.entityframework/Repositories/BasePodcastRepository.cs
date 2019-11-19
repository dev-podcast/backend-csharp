using System.Collections.Generic;
using System.Linq;
using DevPodcast.Domain.Entities;
using DevPodcast.Domain.Interfaces;

namespace DevPodcast.Data.EntityFramework.Repositories
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