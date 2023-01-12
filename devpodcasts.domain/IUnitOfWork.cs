using System;
using System.Threading.Tasks;
using devpodcasts.Domain.Interfaces;

namespace devpodcasts.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        IEpisodeRepository EpisodeRepository { get; }
        IPodcastRepository PodcastRepository { get; }
        IBasePodcastRepository BasePodcastRepository { get; }
        ITagRepository TagRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ISearchRepository SearchRepository { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync();
    }
}