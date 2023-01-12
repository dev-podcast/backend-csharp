using DevPodcast.Domain.Entities;
using DevPodcast.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPodcast.Data.EntityFramework.Repositories
{
    internal class TagRepository : Repository<Tag>, ITagRepository
    {
        internal TagRepository(ApplicationDbContext context) : base(context)
        {
        }

        public ICollection<Podcast> GetByTagId(int tagId)
        {
            return _context.Tag.Where(x => x.Id == tagId).Select(p => p.Podcasts).SingleOrDefault();          
        }

        public Task<ICollection<Podcast>> GetByTagIdAsync(int tagId)
        {
            return _context.Tag.Where(x => x.Id == tagId).Select(p => p.Podcasts).SingleOrDefaultAsync();
        }

        public ICollection<Podcast> GetByTagName(string tagName)
        {
            return _context.Tag.Where(x => x.Description.Contains(tagName)).Select(p => p.Podcasts).SingleOrDefault();          
        }

        public Task<ICollection<Podcast>> GetByTagNameAsync(string tagName)
        {
            return _context.Tag.Where(x => x.Description.Contains(tagName)).Select(p => p.Podcasts).SingleOrDefaultAsync();           
        }   
    }
}