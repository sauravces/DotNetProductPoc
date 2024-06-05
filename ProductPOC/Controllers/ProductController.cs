using AutoMapper;
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
        private readonly IMapper _mapper;

        public ProductController(IProductService productService,IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            var productDtos=_mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(productDtos);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetByIdProductAsync([FromRoute] Guid id)
        {
            var product = await _productService.GetByIdProductAsync(id);
            var productDtos = _mapper.Map<ProductDto>(product);
            return Ok(productDtos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductDto createProductDto)
        {
            var product=_mapper.Map<Product>(createProductDto);
            product=await _productService.CreateProductAsync(product);
            var productDto=_mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProductAsync([FromRoute] Guid id,[FromBody] UpdateProductDto updateProductDto)
        {
            var product = _mapper.Map<Product>(updateProductDto);
            product = await _productService.UpdateProductAsync(id,product);
            if (product == null)
            {
                return NotFound();
            }
            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }
    }
}
