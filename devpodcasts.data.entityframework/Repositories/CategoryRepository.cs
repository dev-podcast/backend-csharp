using DevPodcast.Domain.Entities;
using DevPodcast.Domain.Interfaces;

namespace DevPodcast.Data.EntityFramework.Repositories
{
    internal class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        internal CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}