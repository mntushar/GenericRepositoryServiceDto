using System.Linq.Expressions;

namespace GenericRepositoryServiceDto
{
    public static class CustomerDTO
    {
        public static Entity? Map(ViewModel? viewModel)
        {
            if (viewModel == null)
                return null;

            return new Entity
            {
                CustomerName = viewModel.CustomerName,
                BusinessName = viewModel.BusinessName,
                Email = viewModel.Email,
                ContactName = viewModel.ContactName,
                CountryId = viewModel.CountryId,
                StateId = viewModel.StateId,
                CityId = viewModel.CityId,
            };
        }

        public static ViewModel? Map(Entity? entity)
        {
            if (entity == null)
                return null;

            return new ViewModel
            {
                BusinessName = entity.BusinessName,
                Email = entity.Email,
                ContactName = entity.ContactName,
                CountryId = entity.CountryId,
                StateId = entity.StateId,
                CustomerName = entity.CustomerName,
                CityId = entity.CityId,
            };
        }

        public static Expression<Func<Entity, bool>> Map(
            Expression<Func<ViewModel, bool>> predicate)
        {
            ParameterExpression entityParameter = Expression
                .Parameter(typeof(Entity),
                predicate.Parameters[0].Name);
            Expression entityBody = ParameterTypeVisitor
                .ReplaceParameterType(predicate.Body, predicate.Parameters[0], entityParameter);

            return Expression
                .Lambda<Func<Entity, bool>>(entityBody, entityParameter);
        }

        public static IEnumerable<ViewModel?> Map(IEnumerable<Entity?> list)
        {
            if (list == null)
                yield break;

            foreach (var entity in list)
                yield return Map(entity);
        }
    }
}