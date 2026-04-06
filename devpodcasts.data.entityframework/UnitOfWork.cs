using System;
using System.Threading.Tasks;
using devpodcasts.Domain;
using devpodcasts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace devpodcasts.Data.EntityFramework
{
    internal class UnitOfWork : IUnitOfWork
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationDbContext _context;
        private IPodcastRepository _podcastRepository;
        private IBasePodcastRepository _basePodcastRepository;
        private IEpisodeRepository _episodeRepository;
        private ITagRepository _tagRepository;   
        private ICategoryRepository _categoryRepository;
        private ISearchRepository _searchRepository;
        
        private IDbContextTransaction _currentTransaction;

        public UnitOfWork(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }
        
        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            return (IRepository<TEntity>)_serviceProvider.GetService(typeof(IRepository<TEntity>));
        }


        public IPodcastRepository PodcastRepository =>
            _podcastRepository ??= _serviceProvider.GetRequiredService<IPodcastRepository>();

        public IBasePodcastRepository BasePodcastRepository =>
            _basePodcastRepository ??= _serviceProvider.GetRequiredService<IBasePodcastRepository>();

        public IEpisodeRepository EpisodeRepository =>
            _episodeRepository ??= _serviceProvider.GetRequiredService<IEpisodeRepository>();

        public ITagRepository TagRepository => _tagRepository ??= _serviceProvider.GetRequiredService<ITagRepository>();

        public ICategoryRepository CategoryRepository =>
            _categoryRepository ??= _serviceProvider.GetRequiredService<ICategoryRepository>();

        public ISearchRepository SearchRepository =>
            _searchRepository ??= _serviceProvider.GetRequiredService<ISearchRepository>();

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
        
        public IExecutionStrategy CreateExecutionStrategy()
        {
            return _context.Database.CreateExecutionStrategy();
        }
        
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _currentTransaction = await _context.Database.BeginTransactionAsync();
            return _currentTransaction;
        }
        
        public async Task CommitTransactionAsync()
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("No transaction is in progress.");
            }

            await _currentTransaction.CommitAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
        
        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("No transaction is in progress.");
            }

            await _currentTransaction.RollbackAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        public void Dispose()
        {
             _podcastRepository = null;
             _basePodcastRepository = null;
             _episodeRepository = null;
             _tagRepository = null;
             _categoryRepository = null;     
             _context.Dispose();
        }

      
    }
}