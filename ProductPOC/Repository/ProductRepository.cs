using MongoDB.Driver;
using ProductPOC.DbContext;
using ProductPOC.Models;

namespace ProductPOC.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _products;
        public ProductRepository(ProductDbContext context)
        {
            _products = context.Products;
        }
        public async Task<IEnumerable<Product>> GetAllAsync() =>
           await _products.Find(p => true).ToListAsync();
    }
}
