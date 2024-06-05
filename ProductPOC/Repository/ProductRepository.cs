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

        public async Task<Product> CreateAsync(Product product)
        {
            await _products.InsertOneAsync(product);
            return product;
        }

        public async Task<IEnumerable<Product>> GetAllAsync() =>
           await _products.Find(p => true).ToListAsync();

        public async Task<Product> GetByIdAsync(Guid id)
        {
            return await _products.Find(x=>x.Id==id).FirstOrDefaultAsync();
        }

        public async Task<Product> UpdateAsync(Guid id, Product product)
        {
            var existingProduct = await _products.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (existingProduct == null)
            {
                return null;
            }
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            var result = await _products.ReplaceOneAsync(x => x.Id == id, existingProduct);
            return existingProduct;
        }

    }
}
