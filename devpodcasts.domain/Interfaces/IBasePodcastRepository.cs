using System.Collections.Generic;
using devpodcasts.Domain.Entities;

namespace devpodcasts.Domain.Interfaces
{
    public interface IBasePodcastRepository : IRepository<BasePodcast>
    {
        ICollection<string> GetAllItunesIds();
    }
}