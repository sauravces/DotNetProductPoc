using ProductPOC.Models;
using ProductPOC.Repository;

namespace ProductPOC.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            if(product.Id == Guid.Empty)
            {
                product.Id = Guid.NewGuid();
            }
           return await _productRepository.CreateAsync(product);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync() 
        {
          return  await _productRepository.GetAllAsync();
        }
          
        public async Task<Product> GetByIdProductAsync(Guid id)
        {
           return await _productRepository.GetByIdAsync(id);
        }

        public async Task<Product> UpdateProductAsync(Guid id, Product product)
        {
            return await _productRepository.UpdateAsync(id, product);
        }

        public async Task<Product> DeleteByIdProductAsync(Guid id)
        {
            return await _productRepository.DeleteByIdAsync(id);
        }

        public async Task DeleleAllProductAsync()
        {
            await _productRepository.DeleteAsync();
        }
    }
}
