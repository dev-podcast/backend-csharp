using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devpodcasts.Domain.Entities;
using devpodcasts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace devpodcasts.Data.EntityFramework.Repositories
{
    public class BasePodcastRepository : Repository<BasePodcast>, IBasePodcastRepository
    {
        public BasePodcastRepository(ApplicationDbContext context) : base(context)
        {
        }

        public ICollection<string> GetAllItunesIds()
        {
            return Set.Select(x => x.ItunesId).ToList();
        }

        public Task<List<string>> GetAllItunesIdsAsync()
        {
            return Set.Select(x => x.ItunesId).ToListAsync();
        }
    }
}