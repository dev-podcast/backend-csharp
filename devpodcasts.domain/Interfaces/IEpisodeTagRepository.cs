using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DevPodcast.Domain.Entities;

namespace DevPodcast.Domain.Interfaces
{
    public interface IEpisodeTagRepository : IRepository<EpisodeTag>
    {
        Task<List<EpisodeTag>> GetPodcastTagsAsync(int podcastId);
        List<EpisodeTag> GetPodcastTags(int podcastId);
        ICollection<Episode> GetByTagName(string tagName);
        ICollection<Episode> GetByTagId(int tagId);
        Task<List<Episode>> GetByTagNameAsync(string tagName);
        Task<List<Episode>> GetByTagIdAsync(int tagId);
    }
}
