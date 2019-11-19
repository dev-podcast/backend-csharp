using System;
using System.Threading.Tasks;
using DevPodcast.Domain.Interfaces;

namespace DevPodcast.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        IEpisodeRepository EpisodeRepository { get; }
        IPodcastRepository PodcastRepository { get; }
        IBasePodcastRepository BasePodcastRepository { get; }
        ITagRepository TagRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IEpisodeTagRepository EpisodeTagRepository { get; }
        IPodcastTagRepository PodcastTagRepository { get; }
        IPodcastCategoryRepository PodcastCategoryRepository { get; }
        ISearchRepository SearchRepository { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync();
    }
}