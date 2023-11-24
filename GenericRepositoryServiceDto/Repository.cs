using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GenericRepositoryServiceDto
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<Guid?> InsertAsync(TEntity? entity)
        {
            Guid? id = null;
            if (entity == null) return id;

            try
            {
                id = (Guid?)entity.GetType().GetProperty("Id")
                        ?.GetValue(entity, null);

                if (id == null)
                    return id;

                await _dbSet.AddAsync(entity);
                bool isSave = await _context.SaveChangesAsync() > 0 ? true : false;

                if (isSave)
                {
                    id = (Guid?)entity.GetType().GetProperty("Id")
                        ?.GetValue(entity, null);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return id;
        }

        public virtual async Task<bool> UpdateAsync(TEntity? entity)
        {
            bool isUpdate = false;
            if (entity == null) return isUpdate;

            try
            {
                var local = _dbSet
                    .Local.FirstOrDefault(x =>
                    x.GetType().GetProperty("Id")?.GetValue(x, null)
                    ?.Equals(entity.GetType().GetProperty("Id")
                    ?.GetValue(entity, null)) == true);

                if (local != null)
                {
                    _context.Entry(local).State = EntityState.Detached;

                    _context.Entry(entity).State = EntityState.Modified;

                    isUpdate = await _context.SaveChangesAsync() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        public virtual async Task<TEntity?> GetAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<List<TEntity>> GetListAsync(bool isTracking)
        {
            if (isTracking)
                return await _dbSet.ToListAsync();
            else
                return await _dbSet.AsNoTracking().ToListAsync();
        }

        public virtual async Task<List<TEntity>> GetListAsync(bool isTracking, int skip, int limit)
        {
            if (isTracking)
            {
                return await _dbSet.Skip(skip)
                .Take(limit).ToListAsync();
            }
            else
            {
                return await _dbSet.AsNoTracking().Skip(skip)
               .Take(limit).ToListAsync();
            }

        }

        public virtual async Task<List<TEntity>> GetListAsync(bool isTracking,
            Expression<Func<TEntity, bool>> predicate)
        {
            if (isTracking)
            {
                return await _dbSet.Where(predicate)
                    .ToListAsync();
            }
            else
            {
                return await _dbSet.AsNoTracking().Where(predicate)
                    .ToListAsync();
            }

        }

        public virtual async Task<List<TEntity>> GetListAsync(bool isTracking,
            Expression<Func<TEntity, bool>> predicate,
            int skip, int limit)
        {
            if (isTracking)
            {
                return await _dbSet.Where(predicate)
                .Skip(skip).Take(limit).ToListAsync();
            }
            else
            {
                return await _dbSet.AsNoTracking().Where(predicate)
                .Skip(skip).Take(limit).ToListAsync();
            }
        }

        public virtual async Task<bool> RemoveAsync(string id)
        {
            bool isRemove = false;
            var entityLookup = await _dbSet.FindAsync(id);
            if (entityLookup != null)
            {
                _dbSet.Remove(entityLookup);
                isRemove = _context.SaveChanges() > 0 ? true : false;
            }

            return isRemove;
        }

        public virtual async Task<bool> RemoveAsync(Expression<Func<TEntity, bool>> predicate)
        {
            bool isRemove = false;
            var entityLookup = await _dbSet.FirstOrDefaultAsync(predicate);
            if (entityLookup != null)
            {
                _dbSet.Remove(entityLookup);
                isRemove = _context.SaveChanges() > 0 ? true : false;
            }

            return isRemove;
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

    }
}