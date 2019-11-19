using System.Threading.Tasks;
using DevPodcast.Data.EntityFramework.Repositories;
using DevPodcast.Domain;
using DevPodcast.Domain.Interfaces;

namespace DevPodcast.Data.EntityFramework
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IPodcastRepository PodcastRepository =>
            _podcastRepository ?? (_podcastRepository = new PodcastRepository(_context));

        public IBasePodcastRepository BasePodcastRepository =>
            _basePodcastRepository ?? (_basePodcastRepository = new BasePodcastRepository(_context));

        public IEpisodeRepository EpisodeRepository =>
            _episodeRepository ?? (_episodeRepository = new EpisodeRepository(_context));

        public ITagRepository TagRepository => _tagRepository ?? (_tagRepository = new TagRepository(_context));

        public ICategoryRepository CategoryRepository =>
            _categoryRepository ?? (_categoryRepository = new CategoryRepository(_context));

        public IEpisodeTagRepository EpisodeTagRepository =>
            _episodeTagRepository ?? (_episodeTagRepository = new EpisodeTagRepository(_context));

        public IPodcastTagRepository PodcastTagRepository =>
            _podcastTagRepository ?? (_podcastTagRepository = new PodcastTagRepository(_context));

        public IPodcastCategoryRepository PodcastCategoryRepository =>
            _podcastCategoryRepository ?? (_podcastCategoryRepository = new PodcastCategoryRepository(_context));

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
            _episodeTagRepository = null;
            _podcastTagRepository = null;
            _podcastCategoryRepository = null;
            _context.Dispose();
        }

        #region Fields

        private readonly ApplicationDbContext _context;
        private IPodcastRepository _podcastRepository;
        private IBasePodcastRepository _basePodcastRepository;
        private IEpisodeRepository _episodeRepository;
        private ITagRepository _tagRepository;
        private IPodcastTagRepository _podcastTagRepository;
        private IEpisodeTagRepository _episodeTagRepository;
        private IPodcastCategoryRepository _podcastCategoryRepository;
        private ICategoryRepository _categoryRepository;
        private ISearchRepository _searchRepository;

        #endregion Fields
    }
}