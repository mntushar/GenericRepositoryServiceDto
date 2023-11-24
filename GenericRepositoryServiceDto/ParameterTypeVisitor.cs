using System.Linq.Expressions;

namespace GenericRepositoryServiceDto
{
    public class ParameterTypeVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ParameterTypeVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter ?? throw new ArgumentNullException(nameof(oldParameter));
            _newParameter = newParameter ?? throw new ArgumentNullException(nameof(newParameter));
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }

        public static Expression ReplaceParameterType(Expression expression, ParameterExpression oldParameter,
            ParameterExpression newParameter)
        {
            return new ParameterTypeVisitor(oldParameter, newParameter).Visit(expression);
        }
    }
}