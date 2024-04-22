namespace Dapper.Specification.Test;

using System.Collections.Generic;
using Dapper.Specification.Test.Helpers;
using Xunit;

public class NotSpecificationTests
{
    [Fact]
    public void ToSqlQuery_ShouldGenerateCorrectSql_ForNotSpecification()
    {
        // Arrange
        var spec = new UserByStatusSpecification("Active");
        var notSpec = new NotSpecification<User>(spec);
        var parameters = new Dictionary<string, object>();

        // Act
        var sqlQuery = notSpec.ToSqlQuery(parameters);

        // Assert
        Assert.Equal("NOT (Status = @Status)", sqlQuery);
        Assert.Single(parameters);
        Assert.Equal("Active", parameters["@Status"]);
    }

    [Fact]
    public void Criteria_ShouldMatchUsersThatDoNotSatisfySpecification()
    {
        // Arrange
        var spec = new UserByStatusSpecification("Active");
        var notSpec = new NotSpecification<User>(spec);
        var users = new List<User>
        {
            new User
            {
                Id = 1,
                Name = "Alice",
                Age = 17,
                Status = "Active"
            },
            new User
            {
                Id = 2,
                Name = "Bob",
                Age = 18,
                Status = "Inactive"
            },
        };

        // Act
        var compiledCriteria = notSpec.Criteria.Compile();
        var matchingUsers = users.FindAll(user => compiledCriteria(user));

        // Assert
        Assert.Single(matchingUsers);
        Assert.DoesNotContain(users[0], matchingUsers); // Active status
        Assert.Contains(users[1], matchingUsers); // Not Active status
    }
}
