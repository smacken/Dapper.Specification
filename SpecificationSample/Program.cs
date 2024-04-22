//write an example domain model entity which uses the specification pattern to perform a complex query by combining multiple specification

using Dapper.Specification;
using Microsoft.Data.Sqlite;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

public class Program
{
    public static void Main()
    {
        var connectionString = $"Data Source=file::memory:;Cache=Shared";
        var connection = new SqliteConnection(connectionString);
        var db = new QueryFactory(connection, new SqliteCompiler());

        var product = new Product("Product 1", 100, true);
        db.Query(nameof(Product)).AsInsert(new { Name = product.Name, Price = product.Price, IsInStock = product.IsInStock });

        var byName = new ProductNameSpecification("Product 1");
        var isAvailable = new ProductAvailableSpec();
        var query = db.Query(nameof(Product)).Where(isAvailable.And(byName));
        var result = query.Get<Product>();
        //console writeline query sql
        
    }
}

public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool IsInStock { get; set; }

    public Product(string name, decimal price, bool isInStock)
    {
        Name = name;
        Price = price;
        IsInStock = isInStock;
    }

    public override string ToString()
    {
        return $"Name: {Name}, Price: {Price}, IsInStock: {IsInStock}";
    }
}

public class ProductNameSpecification : Specification<Product>
{
    private readonly string _name;

    public ProductNameSpecification(string name)
    {
        _name = name;
    }

    public override Query ToQuery(Query query) => query.Where(nameof(Product.Name), _name);

    public override Query ToQuery() =>
        new Query(nameof(Product)).Where(nameof(Product.Name), _name);
}

public class ProductAvailableSpec : Specification<Product>
{
    public override Query ToQuery(Query query) => query.Where(nameof(Product.IsInStock), true);

    public override Query ToQuery() =>
        new Query(nameof(Product)).Where(nameof(Product.IsInStock), true);
}
