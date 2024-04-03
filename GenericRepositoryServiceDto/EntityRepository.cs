using DNE.CS.Inventory.Repository.Repository;

namespace GenericRepositoryServiceDto
{
    public class EntityRepository : Repository<Entity>, IEntityRepository
    {
        public EntityRepository(AppDbContext context) : base(context) { }
    }
}
