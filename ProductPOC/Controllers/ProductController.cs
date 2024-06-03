using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductPOC.Dto;
using ProductPOC.Models;
using ProductPOC.Service;

namespace ProductPOC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price
            });

            return Ok(productDtos);
        }
    }
}
