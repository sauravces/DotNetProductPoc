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

        public async Task<IEnumerable<Product>> GetAllProductsAsync() =>
           await _productRepository.GetAllAsync();
    }
}
