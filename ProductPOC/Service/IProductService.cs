using ProductPOC.Models;

namespace ProductPOC.Service
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
    }
}
