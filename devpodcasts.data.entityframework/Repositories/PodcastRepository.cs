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
    internal class PodcastRepository : Repository<Podcast>, IPodcastRepository
    {
        internal PodcastRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override Podcast Get(Expression<Func<Podcast, bool>> predicate)
        {
            return SingleQuery(predicate).SingleOrDefault();
        }

        public override Task<List<Podcast>> GetAllAsync(Expression<Func<Podcast, bool>> predicate)
        {
            return (from pod in _context.Podcast
                    .Include(p => p.PodcastTags)
                    .Include(p => p.PodcastCategories)
                select pod).Where(predicate).ToListAsync();
        }

        public override Task<Podcast> GetAsync(Expression<Func<Podcast, bool>> predicate)
        {
            return SingleQuery(predicate).SingleOrDefaultAsync();
        }

        public ICollection<Podcast> GetRecent(int numberToTake)
        {
            return RecentQuery(numberToTake).ToList();
        }

        public Task<List<Podcast>> GetRecentAsync(int numberToTake)
        {
            return RecentQuery(numberToTake).ToListAsync();
        }

        public Task<List<Podcast>> GetRecentAsync(int podcastLimit, int episodeLimit)
        {
            return RecentQuery(podcastLimit, episodeLimit).ToListAsync();
        }

        private IQueryable<Podcast> RecentQuery(int podcastLimit = 15, int episodeLimit = 15)
        {
            return Set.Select(x => new Podcast
            {
                Id = x.Id,
                Title = x.Title,
                Artists = x.Artists,
                Episodes = x.Episodes.OrderByDescending(e => e.PublishedDate).Take(episodeLimit).ToList(),
                PodcastTags = x.PodcastTags,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                ShowUrl = x.ShowUrl,
                PodcastCategories = x.PodcastCategories,
                FeedUrl = x.FeedUrl,
                EpisodeCount = x.EpisodeCount,
                LatestReleaseDate = x.LatestReleaseDate
            }).OrderByDescending(x => x.LatestReleaseDate).Take(podcastLimit);
        }

        private IQueryable<Podcast> SingleQuery(Expression<Func<Podcast, bool>> predicate)
        {
            return Set.Where(predicate)
                .Include(p => p.PodcastTags)
                .Include(p => p.PodcastCategories)
                .Include(p => p.Episodes);
        }
    }
}