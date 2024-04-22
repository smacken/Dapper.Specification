using System.Collections.Generic;
using Dapper.Specification.Test.Helpers;
using Xunit;

namespace Dapper.Specification.Test;

using System.Collections.Generic;
using Xunit;

public class OrSpecificationTests
{
    [Fact]
    public void ToSqlQuery_ShouldGenerateCorrectSql_AndParametersForOrSpecification()
    {
        // Arrange
        var spec1 = new UserByStatusSpecification("Active");
        var spec2 = new UserByAgeSpecification(18);
        var orSpec = new OrSpecification<User>(spec1, spec2);
        var parameters = new Dictionary<string, object>();

        // Act
        var sqlQuery = orSpec.ToSqlQuery(parameters);

        // Assert
        Assert.Equal("(Status = @Status) OR (Age >= @Age)", sqlQuery);
        Assert.Equal(2, parameters.Count);
        Assert.Equal("Active", parameters["@Status"]);
        Assert.Equal(18, parameters["@Age"]);
    }

    [Fact]
    public void Criteria_ShouldMatchUsersThatSatisfyEitherSpecification()
    {
        // Arrange
        var spec1 = new UserByStatusSpecification("Active");
        var spec2 = new UserByAgeSpecification(18);
        var orSpec = new OrSpecification<User>(spec1, spec2);
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
            },
            new User
            {
                Id = 4,
                Name = "David",
                Age = 16,
                Status = "Inactive"
            }
        };

        // Act
        var compiledCriteria = orSpec.Criteria.Compile();
        var matchingUsers = users.FindAll(user => compiledCriteria(user));

        // Assert
        Assert.Equal(3, matchingUsers.Count);
        Assert.Contains(users[0], matchingUsers); // Active status
        Assert.Contains(users[1], matchingUsers); // Age >= 18
        Assert.Contains(users[2], matchingUsers); // Both conditions
        Assert.DoesNotContain(users[3], matchingUsers); // Neither condition
    }
}
