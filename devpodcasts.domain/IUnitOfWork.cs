using System;
using System.Threading.Tasks;
using devpodcasts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace devpodcasts.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;
        IEpisodeRepository EpisodeRepository { get; }
        IPodcastRepository PodcastRepository { get; }
        IBasePodcastRepository BasePodcastRepository { get; }
        ITagRepository TagRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ISearchRepository SearchRepository { get; }
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        void SaveChanges();
        Task<IDbContextTransaction> BeginTransactionAsync();
        IExecutionStrategy CreateExecutionStrategy(); // Add this
        Task<int> SaveChangesAsync();
    }
}