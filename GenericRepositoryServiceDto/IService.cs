using System.Linq.Expressions;

namespace GenericRepositoryServiceDto
{
    public interface IService<TEntity> where TEntity : class
    {
        Task<Guid?> InsertAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
        Task<TEntity?> GetAsync(string id);
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity?>> GetListAsync(bool isTracking);
        Task<IEnumerable<TEntity?>> GetListAsync(bool isTracking, int skip, int limit);
        Task<IEnumerable<TEntity?>> GetListAsync(bool isTracking, Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity?>> GetListAsync(bool isTracking, Expression<Func<TEntity, bool>> predicate,
            int skip, int limit);
        Task<bool> RemoveAsync(string id);
        Task<bool> RemoveAsync(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    }
}