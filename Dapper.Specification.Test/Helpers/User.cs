using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dapper.Specification.Test.Helpers;

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Age { get; set; }
    public string Status { get; set; } = "Active";
}

public class UserByAgeSpecification : DapperSpecification<User>
{
    private readonly int _age;

    public UserByAgeSpecification(int age)
    {
        _age = age;
    }

    public override Expression<Func<User, bool>> Criteria => user => user.Age >= _age;

    public override string ToSqlQuery(IDictionary<string, object> parameters)
    {
        parameters.Add("@Age", _age);
        return $"Age >= @Age";
    }
}

public class UserByStatusSpecification : DapperSpecification<User>
{
    private readonly string _status;

    public UserByStatusSpecification(string status)
    {
        _status = status;
    }

    public override Expression<Func<User, bool>> Criteria => user => user.Status == _status;

    public override string ToSqlQuery(IDictionary<string, object> parameters)
    {
        var paramName = "@Status";
        parameters.Add(paramName, _status);
        return $"Status = {paramName}";
    }
}
