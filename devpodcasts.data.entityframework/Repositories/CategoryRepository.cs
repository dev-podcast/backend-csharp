using devpodcasts.Domain.Entities;
using devpodcasts.Domain.Interfaces;

namespace devpodcasts.Data.EntityFramework.Repositories
{
    internal class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        internal CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}