using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevPodcast.Domain.Entities;
using DevPodcast.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevPodcast.Data.EntityFramework.Repositories
{
    internal class EpisodeTagRepository : Repository<EpisodeTag>, IEpisodeTagRepository
    {
        internal EpisodeTagRepository(ApplicationDbContext context) : base(context)
        {
        }

        public ICollection<Episode> GetByTagId(int tagId)
        {
            return Set.Where(x => x.TagId == tagId).Select(e => e.Episode).OrderBy(e => e.PodcastId).ToList();
        }

        public Task<List<Episode>> GetByTagIdAsync(int tagId)
        {
            return Set.Where(x => x.TagId == tagId).Select(e => e.Episode).OrderBy(e => e.PodcastId).ToListAsync();
        }

        public ICollection<Episode> GetByTagName(string tagName)
        {
            return Set.Where(x => x.Tag.Description.Contains(tagName)).Select(e => e.Episode).OrderBy(e => e.PodcastId)
                .ToList();
        }

        public Task<List<Episode>> GetByTagNameAsync(string tagName)
        {
            return Set.Where(x => x.Tag.Description.Contains(tagName)).Select(e => e.Episode).OrderBy(e => e.PodcastId)
                .ToListAsync();
        }

        public List<EpisodeTag> GetPodcastTags(int episodeId)
        {
            return Set.Where(p => p.EpisodeId == episodeId).ToList();
        }

        public Task<List<EpisodeTag>> GetPodcastTagsAsync(int episodeId)
        {
            return Set.Where(p => p.EpisodeId == episodeId).ToListAsync();
        }
    }
}