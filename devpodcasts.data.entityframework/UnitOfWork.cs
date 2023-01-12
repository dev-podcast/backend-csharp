using System.Threading.Tasks;
using devpodcasts.Data.EntityFramework.Repositories;
using devpodcasts.Domain;
using devpodcasts.Domain.Interfaces;

namespace devpodcasts.Data.EntityFramework
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
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

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
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

        #region Fields

        private readonly ApplicationDbContext _context;
        private IPodcastRepository _podcastRepository;
        private IBasePodcastRepository _basePodcastRepository;
        private IEpisodeRepository _episodeRepository;
        private ITagRepository _tagRepository;   
        private ICategoryRepository _categoryRepository;
        private ISearchRepository _searchRepository;

        #endregion Fields
    }
}