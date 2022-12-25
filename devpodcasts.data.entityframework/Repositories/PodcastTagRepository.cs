using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevPodcast.Domain.Entities;
using DevPodcast.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevPodcast.Data.EntityFramework.Repositories
{
    internal class PodcastTagRepository : Repository<PodcastTag>, IPodcastTagRepository
    {
        internal PodcastTagRepository(ApplicationDbContext context) : base(context)
        {
        }

        public ICollection<Podcast> GetByTagId(int tagId)
        {
            return Set.Where(x => x.TagId == tagId).Select(p => p.Podcast)
                .Include(p => p.PodcastCategories)
                .Include(p => p.PodcastTags)
                .OrderBy(p => p.Id).ToList();
        }

        public Task<List<Podcast>> GetByTagIdAsync(int tagId)
        {
            return Set.Where(x => x.TagId == tagId).Select(p => p.Podcast)
                .Include(p => p.PodcastCategories)
                .Include(p => p.PodcastTags)
                .OrderBy(p => p.Id).ToListAsync();
        }

        public ICollection<Podcast> GetByTagName(string tagName)
        {
            return Set.Where(x => x.Tag.Description.Contains(tagName)).Select(p => p.Podcast)
                .Include(p => p.PodcastCategories)
                .Include(p => p.PodcastTags)
                .OrderBy(p => p.Id).ToList();
        }

        public Task<List<Podcast>> GetByTagNameAsync(string tagName)
        {
            return Set.Where(x => x.Tag.Description.Contains(tagName)).Select(p => p.Podcast)
                .Include(p => p.PodcastCategories)
                .Include(p => p.PodcastTags)
                .OrderBy(p => p.Id).ToListAsync();
        }

        public List<PodcastTag> GetPodcastTags(int podcastId)
        {
            return Set.Where(p => p.PodcastId == podcastId).ToList();
        }

        public Task<List<PodcastTag>> GetPodcastTagsAsync(int podcastId)
        {
            return Set.Where(p => p.PodcastId == podcastId).ToListAsync();
        }
    }
}