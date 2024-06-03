using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProductPOC.Models;

namespace ProductPOC.DbContext
{
    public class ProductDbContext:IProductDbContext
    {
        private readonly IMongoDatabase _database;
        public ProductDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDb");
           var client = new MongoClient(connectionString);
           _database = client.GetDatabase(configuration["DatabaseSettings:DatabaseName"]);
        }
        

        public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
}
}
