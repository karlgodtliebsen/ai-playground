using System.Linq.Expressions;

namespace AI.CaaP.Repository.Repositories;

public static class RepositoryExpressionExtensions
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> where1, Expression<Func<T, bool>> where2)
    {
        InvocationExpression invocationExpression = Expression.Invoke(where2, where1.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(where1.Body, invocationExpression), where1.Parameters);
    }
}