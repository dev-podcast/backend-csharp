using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using devpodcasts.Domain.Entities;
using devpodcasts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace devpodcasts.Data.EntityFramework.Repositories
{
    internal class EpisodeRepository : Repository<Episode>, IEpisodeRepository
    {
        public EpisodeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override Episode Get(Expression<Func<Episode, bool>> predicate)
        {
            return _context.Set<Episode>().Where(predicate).Include(e => e.Tags).FirstOrDefault();
        }

        public override Task<Episode> GetAsync(Expression<Func<Episode, bool>> predicate)
        {
            return _context.Set<Episode>().Where(predicate).Include(e => e.Tags).FirstOrDefaultAsync();
        }

        public List<Episode> GetRecent(Guid Id, int numberToTake)
        {
            return _context.Set<Episode>().Where(e => e.PodcastId == Id).OrderByDescending(p => p.PublishedDate).Include(e => e.Podcast)
                .Include(e => e.Tags).Take(numberToTake).ToList();
        }

        public Task<List<Episode>> GetRecentAsync(Guid Id, int numberToTake)
        {
            return _context.Set<Episode>().Where(e => e.PodcastId == Id).OrderByDescending(p => p.PublishedDate).Include(e => e.Podcast)
                .Include(e => e.Tags).Take(numberToTake).ToListAsync();
        }

        public Episode GetByTag(Expression<Func<Episode, bool>> predicate)
        {
            return _context.Set<Episode>().Where(predicate).Include(e => e.Tags).FirstOrDefault();
        }

        public Task<Episode> GetByTagAsync(Expression<Func<Episode, bool>> predicate)
        {
            return _context.Set<Episode>().Where(predicate).Include(e => e.Tags).FirstOrDefaultAsync();
        }

        public override Task<List<Episode>> GetAllAsync()
        {
            return _context.Set<Episode>()
                .Include(e => e.Podcast)
                .Include(e => e.Tags)
                .Include(e => e.Categories)
                .ToListAsync();
        }

        public override Task<List<Episode>> GetAllAsync(Expression<Func<Episode, bool>> predicate)
        {
            return _context.Set<Episode>().Where(predicate)
                .Include(e => e.Podcast)
                .Include(e => e.Tags)
                .Include(e => e.Categories)
                .ToListAsync();
        }

        public Task<List<Episode>> GetByShowIdAsync(Guid ShowId)
        {
            return _context.Set<Episode>().Where(x => x.PodcastId == ShowId)
                .Include(e => e.Podcast)
                .Include(e => e.Tags)
                .Include(e => e.Categories)
                .ToListAsync();
        }

        public Task<List<Episode>> GetAllBySearch(Expression<Func<Episode, bool>> predicate)
        {
            return _context.Set<Episode>().Where(predicate)
                .Include(e => e.Podcast)
                .Include(e => e.Tags)
                .Include(e => e.Categories)
                .ToListAsync();
        }

    }
}