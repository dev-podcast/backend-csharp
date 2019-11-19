using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DevPodcast.Domain.Entities;
using DevPodcast.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevPodcast.Data.EntityFramework
{
    internal class EpisodeRepository : Repository<Episode>, IEpisodeRepository
    {
        internal EpisodeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override Episode Get(Expression<Func<Episode, bool>> predicate)
        {
            return Set.Where(predicate).Include(e => e.EpisodeTags).SingleOrDefault();
        }

        public override Task<Episode> GetAsync(Expression<Func<Episode, bool>> predicate)
        {
            return Set.Where(predicate).Include(e => e.EpisodeTags).SingleOrDefaultAsync();
        }

        public ICollection<Episode> GetRecent(int Id, int numberToTake)
        {
            return Set.Where(e => e.PodcastId == Id).OrderByDescending(p => p.PublishedDate).Include(e => e.Podcast)
                .Include(e => e.EpisodeTags).Take(numberToTake).ToList();
        }

        public Task<List<Episode>> GetRecentAsync(int Id, int numberToTake)
        {
            return Set.Where(e => e.PodcastId == Id).OrderByDescending(p => p.PublishedDate).Include(e => e.Podcast)
                .Include(e => e.EpisodeTags).Take(numberToTake).ToListAsync();
        }

        public Episode GetByTag(Expression<Func<Episode, bool>> predicate)
        {
            return Set.Where(predicate).Include(e => e.EpisodeTags).SingleOrDefault();
        }

        public Task<Episode> GetByTagAsync(Expression<Func<Episode, bool>> predicate)
        {
            return Set.Where(predicate).Include(e => e.EpisodeTags).SingleOrDefaultAsync();
        }
    }
}