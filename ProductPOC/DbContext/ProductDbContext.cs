using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProductPOC.Models;

namespace ProductPOC.DbContext
{
    public class ProductDbContext:IProductDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly string _productsCollectionName;

        public ProductDbContext()
        {

        }
        public ProductDbContext(IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:MongoDb"];
            var databaseName = configuration["ConnectionStrings:DatabaseName"];
            _productsCollectionName = configuration["ConnectionStrings:ProductsCollectionName"];
            var client = new MongoClient(connectionString);
           _database = client.GetDatabase(databaseName);
            
        }
        

        public virtual IMongoCollection<Product> Products => _database.GetCollection<Product>(_productsCollectionName);
}
}
