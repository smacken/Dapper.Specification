namespace Dapper.Specification.Test;

using System.Collections.Generic;
using Dapper.Specification.Test.Helpers;
using Xunit;

public class AndSpecificationTests
{
    [Fact]
    public void ToSqlQuery_ShouldGenerateCorrectSql_AndParametersForAndSpecification()
    {
        // Arrange
        var spec1 = new UserByStatusSpecification("Active");
        var spec2 = new UserByAgeSpecification(18);
        var andSpec = new AndSpecification<User>(spec1, spec2);
        var parameters = new Dictionary<string, object>();

        // Act
        var sqlQuery = andSpec.ToSqlQuery(parameters);

        // Assert
        Assert.Equal("(Status = @Status) AND (Age >= @Age)", sqlQuery);
        Assert.Equal(2, parameters.Count);
        Assert.Equal("Active", parameters["@Status"]);
        Assert.Equal(18, parameters["@Age"]);
    }

    [Fact]
    public void Criteria_ShouldMatchUsersThatSatisfyBothSpecifications()
    {
        // Arrange
        var spec1 = new UserByStatusSpecification("Active");
        var spec2 = new UserByAgeSpecification(18);
        var andSpec = new AndSpecification<User>(spec1, spec2);
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
            new User
            {
                Id = 3,
                Name = "Charlie",
                Age = 20,
                Status = "Active"
            }
        };

        // Act
        var compiledCriteria = andSpec.Criteria.Compile();
        var matchingUsers = users.FindAll(user => compiledCriteria(user));

        // Assert
        Assert.Single(matchingUsers);
        Assert.DoesNotContain(users[0], matchingUsers);
        Assert.DoesNotContain(users[1], matchingUsers);
        Assert.Contains(users[2], matchingUsers);
    }
}
