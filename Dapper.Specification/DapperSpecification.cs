using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Dapper.Specification;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    IEnumerable<string> Includes { get; } // For possible future use with entity framework includes

    string ToSqlQuery(IDictionary<string, object> parameters);
}

public abstract class DapperSpecification<T> : ISpecification<T>
{
    public abstract Expression<Func<T, bool>> Criteria { get; }
    public virtual IEnumerable<string> Includes => Enumerable.Empty<string>();

    public virtual string ToSqlQuery(IDictionary<string, object> parameters)
    {
        return ExpressionToSqlTranslator.Translate(Criteria, out parameters);
    }
}
