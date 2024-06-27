using ProductPOC.Models;

namespace ProductPOC.Service
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetByIdProductAsync(Guid id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Guid id,Product product);
        Task<Product> DeleteByIdProductAsync(Guid id);
        Task DeleleAllProductAsync();
    }
}
