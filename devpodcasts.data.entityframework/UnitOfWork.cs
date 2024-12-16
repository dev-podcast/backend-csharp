using System;
using System.Threading.Tasks;
using devpodcasts.Data.EntityFramework.Repositories;
using devpodcasts.Domain;
using devpodcasts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace devpodcasts.Data.EntityFramework
{
    public class UnitOfWork : IUnitOfWork
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
            _podcastRepository ??= new PodcastRepository(_context);

        public IBasePodcastRepository BasePodcastRepository =>
            _basePodcastRepository ??= new BasePodcastRepository(_context);

        public IEpisodeRepository EpisodeRepository =>
            _episodeRepository ??= new EpisodeRepository(_context);

        public ITagRepository TagRepository => _tagRepository ??= new TagRepository(_context);

        public ICategoryRepository CategoryRepository =>
            _categoryRepository ??= new CategoryRepository(_context);

        public ISearchRepository SearchRepository =>
            _searchRepository ?? (_searchRepository = new SearchRepository(_context));

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
            // _podcastRepository = null;
            // _basePodcastRepository = null;
            // _episodeRepository = null;
            // _tagRepository = null;
            // _categoryRepository = null;     
            // _context.Dispose();
        }

      
    }
}