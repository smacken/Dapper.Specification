namespace Dapper.Specification.Test;

using System.Collections.Generic;
using Dapper.Specification.Test.Helpers;
using Xunit;

public class UserByAgeSpecificationTests
{
    [Fact]
    public void ToSqlQuery_ShouldGenerateCorrectSql_AndParameters()
    {
        // Arrange
        var specification = new UserByAgeSpecification(18);
        var parameters = new Dictionary<string, object>();

        // Act
        var sqlQuery = specification.ToSqlQuery(parameters);

        // Assert
        Assert.Equal("Age >= @Age", sqlQuery);
        Assert.Single(parameters);
        Assert.Equal(18, parameters["@Age"]);
    }

    [Fact]
    public void Criteria_ShouldMatchUsersWithAgeGreaterThanOrEqual()
    {
        // Arrange
        var specification = new UserByAgeSpecification(18);
        var users = new List<User>
        {
            new User
            {
                Id = 1,
                Name = "Alice",
                Age = 17
            },
            new User
            {
                Id = 2,
                Name = "Bob",
                Age = 18
            },
            new User
            {
                Id = 3,
                Name = "Charlie",
                Age = 20
            }
        };

        // Act
        var compiledCriteria = specification.Criteria.Compile();
        var matchingUsers = users.FindAll(user => compiledCriteria(user));

        // Assert
        Assert.Equal(2, matchingUsers.Count);
        Assert.DoesNotContain(users[0], matchingUsers);
        Assert.Contains(users[1], matchingUsers);
        Assert.Contains(users[2], matchingUsers);
    }
}
