using DNE.CS.Inventory.Repository.Interface;
using GenericRepositoryServiceDto;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace DNE.CS.Inventory.Repository.Repository
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

        public virtual async Task<IEnumerable<TEntity?>> InsertAsync(IEnumerable<TEntity?> entity)
        {
            try
            {
                if (!entity.Any()) return Enumerable.Empty<TEntity>();
                entity = entity.Select(x =>
                {
                    PropertyInfo? idProperty = entity.GetType().GetProperty("Id");
                    if (idProperty != null)
                    {
                        idProperty.SetValue(entity, Guid.NewGuid());
                    }
                    return x;
                }).ToList();
                await _dbSet.AddRangeAsync(entity!);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
                }
                else
                {
                    _dbSet.Attach(entity);
                }

                _context.Entry(entity).State = EntityState.Modified;
                isUpdate = await _context.SaveChangesAsync() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return isUpdate;
        }

        public virtual async Task<IEnumerable<TEntity?>> UpdateAsync(IEnumerable<TEntity?> entities)
        {
            try
            {
                if (!entities.Any()) return Enumerable.Empty<TEntity>();

                entities.ToList().ForEach(entity =>
                {
                    _context.Entry(entity!).State = EntityState.Detached;
                });

                _dbSet.UpdateRange(entities!);
                await _context.SaveChangesAsync();

                return entities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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

        public virtual async Task<List<TEntity>> GetListAsync(bool isTracking,
            Expression<Func<TEntity, bool>> predicate, bool isSortByAscending,
            string orderName, int skip, int limit)
        {
            try
            {
                orderName = orderName.Replace(" ", "");
                PropertyInfo? propertyInfo = typeof(TEntity).GetProperty(orderName);
                if (propertyInfo == null) throw new Exception("Property info is empty.");

                ParameterExpression param = Expression.Parameter(typeof(TEntity), "item");
                MemberExpression property = Expression.Property(param, propertyInfo);
                Expression<Func<TEntity, object>>? lambda = Expression
                    .Lambda<Func<TEntity, object>>(Expression.Convert(property, typeof(object)), param);
                Func<TEntity, object>? compiled = lambda.Compile();

                if (isTracking)
                {
                    if (isSortByAscending)
                    {
                        return await Task.FromResult((_dbSet
                       .Where(predicate)
                       .OrderBy(compiled)
                       .Skip(skip)
                       .Take(limit))
                       .ToList());
                    }

                    return await Task.FromResult((_dbSet
                       .Where(predicate)
                       .OrderByDescending(compiled)
                       .Skip(skip)
                       .Take(limit))
                       .ToList());

                }

                if (isSortByAscending)
                {
                    return await Task.FromResult((_dbSet
                       .AsNoTracking()
                       .Where(predicate)
                       .OrderBy(compiled)
                       .Skip(skip)
                       .Take(limit))
                       .ToList());
                }

                return await Task.FromResult((_dbSet
                       .AsNoTracking()
                       .Where(predicate)
                       .OrderByDescending(compiled)
                       .Skip(skip)
                       .Take(limit))
                       .ToList());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
