using ProductPOC.Models;

namespace ProductPOC.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
    }
}
