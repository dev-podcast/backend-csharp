﻿using System;
using devpodcasts.Domain.Entities;
using devpodcasts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devpodcasts.Data.EntityFramework.Repositories
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public TagRepository(ApplicationDbContext context) : base(context)
        {
        }

        public ICollection<Podcast> GetByTagId(Guid tagId)
        {
            return _context.Tag.Where(x => x.Id == tagId).Select(p => p.Podcasts).SingleOrDefault();          
        }

        public Task<ICollection<Podcast>> GetByTagIdAsync(Guid tagId)
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