using System.Collections.Generic;
using DevPodcast.Domain.Entities;

namespace DevPodcast.Domain.Interfaces
{
    public interface IBasePodcastRepository : IRepository<BasePodcast>
    {
        ICollection<string> GetAllItunesIds();
    }
}