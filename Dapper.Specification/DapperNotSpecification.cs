using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dapper.Specification;

public static class SpecificationNot
{
    // Existing And and Or extension methods...

    public static ISpecification<T> Not<T>(this ISpecification<T> specification)
    {
        return new NotSpecification<T>(specification);
    }
}

public class NotSpecification<T> : DapperSpecification<T>
{
    private readonly ISpecification<T> _specification;

    public NotSpecification(ISpecification<T> specification)
    {
        _specification = specification ?? throw new ArgumentNullException(nameof(specification));
    }

    public override Expression<Func<T, bool>> Criteria
    {
        get
        {
            var originalCriteria = _specification.Criteria;
            var negatedExpression = Expression.Not(originalCriteria.Body);

            return Expression.Lambda<Func<T, bool>>(negatedExpression, originalCriteria.Parameters);
        }
    }

    public override string ToSqlQuery(IDictionary<string, object> parameters)
    {
        var innerSql = _specification.ToSqlQuery(parameters);
        return $"NOT ({innerSql})";
    }
}
