using System.Linq.Expressions;

namespace GenericRepositoryServiceDto
{
    public class Service : IService<ViewModel>
    {
        private readonly IEntityRepository _entityRepository;

        public Service(IEntityRepository entity)
        {
            _entityRepository = entity;
        }
        public Task<int> CountAsync(Expression<Func<Entity, bool>> predicate)
        {
            return _entityRepository.CountAsync(predicate);
        }

        public async Task<ViewModel?> GetAsync(string id)
        {
            return CustomerDTO.Map(await _entityRepository.GetAsync(id));
        }

        public async Task<ViewModel?> GetAsync(Expression<Func<Entity, bool>> predicate)
        {
            return CustomerDTO.Map(await _entityRepository.GetAsync(predicate));
        }

        public async Task<IEnumerable<ViewModel?>> GetListAsync(bool isTracking)
        {
            return CustomerDTO.Map(await _entityRepository.GetListAsync(isTracking));
        }

        public async Task<IEnumerable<ViewModel?>> GetListAsync(bool isTracking, int skip, int limit)
        {
            return CustomerDTO.Map(await _entityRepository.GetListAsync(isTracking, skip, limit));
        }

        public async Task<IEnumerable<ViewModel?>> GetListAsync(
            bool isTracking, Expression<Func<Entity, bool>> predicate)
        {
            return CustomerDTO.Map(await _entityRepository.GetListAsync(isTracking,
                predicate));
        }

        public async Task<IEnumerable<ViewModel?>> GetListAsync(
            bool isTracking, Expression<Func<Entity, bool>> predicate,
            int skip, int limit)
        {
            return CustomerDTO.Map(await _entityRepository
                .GetListAsync(isTracking, predicate, skip, limit));
        }

        public async Task<Guid?> InsertAsync(ViewModel entity)
        {
            return await _entityRepository.InsertAsync(CustomerDTO.Map(entity));
        }

        public async Task<bool> RemoveAsync(string id)
        {
            return await _entityRepository.RemoveAsync(id);
        }

        public async Task<bool> RemoveAsync(Expression<Func<Entity, bool>> predicate)
        {
            return await _entityRepository.RemoveAsync(predicate);
        }

        public async Task<bool> UpdateAsync(ViewModel entity)
        {
            return await _entityRepository.UpdateAsync(CustomerDTO.Map(entity));
        }
    }
}
