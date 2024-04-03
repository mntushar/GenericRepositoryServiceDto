using System.Linq.Expressions;

namespace GenericRepositoryServiceDto
{
    public interface IService<TEntity> where TEntity : class
    {
        Task<Guid?> InsertAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
        Task<TEntity?> GetAsync(string id);
        Task<IEnumerable<TEntity?>> GetListAsync(bool isTracking);
        Task<IEnumerable<TEntity?>> GetListAsync(bool isTracking, int skip, int limit);
        Task<bool> RemoveAsync(string id);
    }
}