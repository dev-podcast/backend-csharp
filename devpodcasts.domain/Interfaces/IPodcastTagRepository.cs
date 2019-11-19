using DevPodcast.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevPodcast.Domain.Interfaces
{
    public interface IPodcastTagRepository : IRepository<PodcastTag>
    {
        Task<List<PodcastTag>> GetPodcastTagsAsync(int podcastId);
        List<PodcastTag> GetPodcastTags(int podcastId);
        ICollection<Podcast> GetByTagName(string tagName);
        ICollection<Podcast> GetByTagId(int tagId);
        Task<List<Podcast>> GetByTagNameAsync(string tagName);
        Task<List<Podcast>> GetByTagIdAsync(int tagId);
    }
}
