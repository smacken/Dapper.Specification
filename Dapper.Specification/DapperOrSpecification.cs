using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dapper.Specification;

public class OrSpecification<T> : DapperSpecification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public override Expression<Func<T, bool>> Criteria
    {
        get
        {
            var leftExpr = _left.Criteria;
            var rightExpr = _right.Criteria;

            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(leftExpr.Parameters[0], parameter);
            var left = leftVisitor.Visit(leftExpr.Body);

            var rightVisitor = new ReplaceExpressionVisitor(rightExpr.Parameters[0], parameter);
            var right = rightVisitor.Visit(rightExpr.Body);

            var orExpression = Expression.OrElse(left, right);
            return Expression.Lambda<Func<T, bool>>(orExpression, parameter);
        }
    }

    public override string ToSqlQuery(IDictionary<string, object> parameters)
    {
        var leftSql = _left.ToSqlQuery(parameters);
        var rightSql = _right.ToSqlQuery(parameters);
        return $"({leftSql}) OR ({rightSql})";
    }

    // Helper class to replace parameters in the expression tree
    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            if (node == _oldValue)
                return _newValue;
            return base.Visit(node);
        }
    }
}

public static class SpecificationOr
{
    public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
    {
        return new OrSpecification<T>(left, right);
    }
}
