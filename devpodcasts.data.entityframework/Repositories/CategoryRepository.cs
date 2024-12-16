using devpodcasts.Domain.Entities;
using devpodcasts.Domain.Interfaces;

namespace devpodcasts.Data.EntityFramework.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}