using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Specification;

public static class ExpressionToSqlTranslator
{
    private static string GetSqlOperator(ExpressionType type)
    {
        switch (type)
        {
            case ExpressionType.Equal:
                return "=";
            case ExpressionType.NotEqual:
                return "<>";
            case ExpressionType.LessThan:
                return "<";
            case ExpressionType.LessThanOrEqual:
                return "<=";
            case ExpressionType.GreaterThan:
                return ">";
            case ExpressionType.GreaterThanOrEqual:
                return ">=";
            // Add more cases for other expression types as needed.
            default:
                throw new NotSupportedException($"Unsupported expression type: {type}");
        }
    }

    private static string VisitExpression(
        Expression expression,
        IDictionary<string, object> parameters
    )
    {
        switch (expression)
        {
            case BinaryExpression binaryExpr:
                var left = VisitExpression(binaryExpr.Left, parameters);
                var right = VisitExpression(binaryExpr.Right, parameters);
                var op = GetSqlOperator(binaryExpr.NodeType);
                return $"({left} {op} {right})";

            case MemberExpression memberExpr:
                return memberExpr.Member.Name;

            case ConstantExpression constantExpr:
                var paramName = $"@param{parameters.Count}";
                parameters.Add(paramName, constantExpr.Value);
                return paramName;

            case MethodCallExpression methodCallExpr
                when IsEnumerableContainsMethod(methodCallExpr):
                return HandleEnumerableContains(methodCallExpr, parameters);

            // Add cases for other expression types as needed.

            default:
                throw new NotSupportedException($"Unsupported expression type: {expression.Type}");
        }
    }

    private static bool IsEnumerableContainsMethod(MethodCallExpression expression)
    {
        return expression.Method.Name == "Contains"
            && expression.Method.DeclaringType.GetInterfaces().Any(i => i == typeof(IEnumerable));
    }

    private static string HandleEnumerableContains(
        MethodCallExpression expression,
        IDictionary<string, object> parameters
    )
    {
        var memberExpr = expression.Arguments[0] as MemberExpression;
        if (memberExpr == null)
            throw new ArgumentException(
                "The Contains method is expected to be called on a MemberExpression."
            );

        var collectionExpr =
            expression.Object as ConstantExpression
            ?? (expression.Object as MemberExpression)?.Expression as ConstantExpression;

        if (collectionExpr == null || !(collectionExpr.Value is IEnumerable collection))
            throw new ArgumentException(
                "The Contains method is expected to be called on an IEnumerable."
            );

        var inClause = new StringBuilder();
        inClause.Append($"{memberExpr.Member.Name} IN (");

        var paramNames = new List<string>();
        foreach (var item in collection)
        {
            var paramName = $"@param{parameters.Count}";
            parameters.Add(paramName, item);
            paramNames.Add(paramName);
        }

        inClause.Append(string.Join(", ", paramNames));
        inClause.Append(")");

        return inClause.ToString();
    }

    public static string Translate<T>(
        Expression<Func<T, bool>> expression,
        out IDictionary<string, object> parameters
    )
    {
        parameters = new Dictionary<string, object>();
        return VisitExpression(expression.Body, parameters);
    }
}
