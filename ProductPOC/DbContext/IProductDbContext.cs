using MongoDB.Driver;
using ProductPOC.Models;

namespace ProductPOC.DbContext
{
    public interface IProductDbContext
    {
        IMongoCollection<Product> Products { get; }
    }
}
