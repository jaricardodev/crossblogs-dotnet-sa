using System;
using System.Linq;
using System.Threading.Tasks;
using crossblog.Domain;
using Microsoft.EntityFrameworkCore;

namespace crossblog.Repositories
{
    public abstract class GenericRepository<T> : IGenericRepository<T>
        where T : BaseEntity, new()
    {
        protected CrossBlogDbContext _dbContext { get; set; }

        public async Task<T> GetAsync(int id)
        {
            return await _dbContext.FindAsync<T>(id);
        }

        public IQueryable<T> Query()
        {
           
            return _dbContext.Set<T>().AsQueryable();
        }

        public async Task InsertAsync(T entity)
        {
            entity.Created_At = DateTime.UtcNow;
            entity.Updated_At = DateTime.UtcNow;

            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            entity.Updated_At = DateTime.UtcNow;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            T entity = new T() { Id = id };
            await DeleteAsync(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();
        }
    }
}