﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using devpodcasts.Domain.Entities;
using devpodcasts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace devpodcasts.Data.EntityFramework.Repositories
{
    public class PodcastRepository : Repository<Podcast>, IPodcastRepository
    {
        public PodcastRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override Podcast Get(Expression<Func<Podcast, bool>> predicate)
        {
            return SingleQuery(predicate).SingleOrDefault();
        }

        public Task<List<Podcast>> GetAllAsync(Guid id)
        {
            return _context.Set<Podcast>().Where(p => p.Id == id)
                .Include(p => p.Tags)
                .Include(p => p.Categories).ToListAsync(); ;   
        }

    
        public override Task<Podcast> GetAsync(Expression<Func<Podcast, bool>> predicate)
        {
            return SingleQuery(predicate).SingleOrDefaultAsync();
        }

        public Task<List<Podcast>> GetRecentAsync(int numberToTake)
        {
            return RecentQuery(numberToTake).ToListAsync();
        }

        public Task<List<Podcast>> GetRecentAsync(int podcastLimit, int episodeLimit)
        {
            return RecentQuery(podcastLimit, episodeLimit).ToListAsync();
        }

        public Task<List<Podcast>> GetAllBySearch(Expression<Func<Podcast, bool>> predicate)
        {
            return _context.Set<Podcast>().Where(predicate).ToListAsync();
        }

        private IQueryable<Podcast> RecentQuery(int podcastLimit = 15, int episodeLimit = 15)
        {    
            return _context.Set<Podcast>().Select(x => new Podcast
            {
                Id = x.Id,
                Title = x.Title,
                Artists = x.Artists,
                Episodes = x.Episodes.OrderByDescending(e => e.PublishedDate).Take(episodeLimit).ToList(),
                Tags = x.Tags,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                ShowUrl = x.ShowUrl,
                Categories = x.Categories,
                FeedUrl = x.FeedUrl,
                EpisodeCount = x.EpisodeCount,
                LatestReleaseDate = x.LatestReleaseDate
            }).OrderByDescending(x => x.LatestReleaseDate).Take(podcastLimit);
        }

        private IQueryable<Podcast> SingleQuery(Expression<Func<Podcast, bool>> predicate)
        {
            return _context.Set<Podcast>().Where(predicate)
                .Include(p => p.Tags)
                .Include(p => p.Categories)
                .Include(p => p.Episodes);
        }
    }
}