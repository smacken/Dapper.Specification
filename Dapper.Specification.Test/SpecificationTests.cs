using System;
using FluentAssertions;
using SqlKata;
using SqlKata.Compilers;
using Xunit;

namespace Dapper.Specification.Test
{
    public class SpecificationTests : TestSupport
    {
        public class Product
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        public class ProductSpec : Specification<Product>
        {
            private readonly int _id;

            public ProductSpec(int id)
            {
                _id = id;
            }

            public override Query ToQuery(Query q) => q.Where(nameof(Product.Id), _id);

            public override Query ToQuery() => new Query().Where(nameof(Product.Id), _id);
        }

        public class ProductNameSpec : Specification<Product>
        {
            private readonly string _name;

            public ProductNameSpec(string name)
            {
                _name = name;
            }
            public override Query ToQuery(Query query) => query.Where(nameof(Product.Name), _name);
            public override Query ToQuery() => new Query(nameof(Product)).Where(nameof(Product.Name), _name);
        }

        public class ProductSpecification
        {
            private Query Spec => new Query(nameof(Product));

            public static Specification<Product> Specs(Func<Query> func) => new ConcreteSpec<Product>().WithQuery(func);
            public static Specification<Product> Inline(Func<Query,Query> func) => new ConcreteSpec<Product>().WithQuery(func);

            // ByName takes a string parameter and returns a query using a ConcreteSpec of Product
            public Specification<Product> ByName(string name) => Specs(() => Spec.Where(nameof(Product.Name), name));
            //public Specification<Product> ById => Specs((int id) => Spec.Where(nameof(Product.Id), id));
            private Func<Query, Query> byId = (Query q) => q.Where(nameof(Product.Id), 1);

            public static Specification<Product> ById => Inline(q => q.Where(nameof(Product.Id), 1));
            //public Specification<Product> ById => Specs((Query q, Query query) =>
            //{
            //    return q.Where(nameof(Product.Id), 1);
            //});
            //public Specification<Product> ByNameAndId => ByName.And(ById);
        }

        [Fact]
        public void CreateInlineSpec()
        {
            var productQuery = new Query(nameof(Product)).Where(ProductSpecification.ById).Select("Id", "Name");
            var query = Compile(productQuery);
            query[EngineCodes.SqlServer].Should().BeEquivalentTo("SELECT [Id], [Name] FROM [Product] WHERE ([Id] = 1)");
        }

        [Fact]
        public void CreateInlineSpecStrippedDown()
        {
            var productQuery = new Query(nameof(Product))
                .Where(q => q.Where(nameof(Product.Id), 1))
                .Select("Id", "Name");
            var query = Compile(productQuery);
            query[EngineCodes.SqlServer].Should().BeEquivalentTo("SELECT [Id], [Name] FROM [Product] WHERE ([Id] = 1)");
        }

        [Fact]
        public void CreateSpec()
        {
            var byProductId = new ProductSpec(1);
            var productQuery = new Query(nameof(Product)).Where(byProductId);
            var query = Compile(productQuery);
            query[EngineCodes.SqlServer].Should().BeEquivalentTo("SELECT * FROM [Product] WHERE ([Id] = 1)");
        }

        [Fact]
        public void AndSpec()
        {
            var byProductId = new ProductSpec(1);
            var byName = new ProductSpecification().ByName;
            var productQuery = new Query(nameof(Product)).Where(byProductId.And(byName("product")));
            var query = Compile(productQuery);
            query[EngineCodes.SqlServer].Should().BeEquivalentTo("SELECT * FROM [Product] WHERE ([Id] = 1 AND [Name] = 'product')");
        }

        [Fact]
        public void SubWhere()
        {
            var productQuery = new Query(nameof(Product)).Where(q => q.Where("Id", 1)).Select("Id", "Name");
            var query = Compile(productQuery);
            query[EngineCodes.SqlServer].Should().BeEquivalentTo("SELECT [Id], [Name] FROM [Product] WHERE ([Id] = 1)");
        }

        [Fact]
        public void SubWhereWithSpec()
        {
            var byProductId = new ProductSpec(1);
            var productQuery = new Query(nameof(Product)).Where(byProductId).Select("Id", "Name");
            var query = Compile(productQuery);
            query[EngineCodes.SqlServer].Should().BeEquivalentTo("SELECT [Id], [Name] FROM [Product] WHERE ([Id] = 1)");
        }

        [Fact]
        public void SubWhereWithAndSpec()
        {
            var byProductId = new ProductSpec(1);
            var byName = new ProductNameSpec("rest");
            var productQuery = new Query(nameof(Product)).Where(byProductId.And(byName)).Select("Id", "Name");
            var query = Compile(productQuery);
            query[EngineCodes.SqlServer].Should().BeEquivalentTo("SELECT [Id], [Name] FROM [Product] WHERE ([Id] = 1 AND [Name] = 'rest')");
        }
    }
}
