using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using devpodcasts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace devpodcasts.Data.EntityFramework.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public ApplicationDbContext _context;
        private DbSet<TEntity> _set;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        protected DbSet<TEntity> Set => _set ?? (_set = _context.Set<TEntity>());

        public void Add(TEntity entity)
        {
            Set.Add(entity);
        }

        public ValueTask<EntityEntry<TEntity>> AddAsync(TEntity entity)
        {
            return Set.AddAsync(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            Set.AddRange(entities);
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            return Set.AddRangeAsync(entities);
        }

        public virtual TEntity Get(Expression<Func<TEntity, bool>> condition)
        {
            return Set.Where(condition).SingleOrDefault();
        }

        public virtual ICollection<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.Where(predicate).ToList();
        }

        public virtual Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.Where(predicate).ToListAsync();
        }

        public virtual Task<List<TEntity>> GetAllAsync()
        {
            return Set.ToListAsync();
        }

        public virtual Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.Where(predicate).SingleOrDefaultAsync();
        }

        public void Remove(TEntity entity)
        {
            Set.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            Set.RemoveRange(entities);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void Update(TEntity entity)
        {
            Set.Entry(entity).State = EntityState.Modified;
        }
    }
}