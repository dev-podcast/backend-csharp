using DevPodcast.Domain.Entities;
using DevPodcast.Domain.Interfaces;

namespace DevPodcast.Data.EntityFramework.Repositories
{
    internal class TagRepository : Repository<Tag>, ITagRepository
    {
        internal TagRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}